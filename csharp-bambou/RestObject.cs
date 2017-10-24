using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Reflection;
using System.Net;
using System.IO;

namespace net.nuagenetworks.bambou
{
    public abstract class RestObject
    {
        [JsonProperty("id")]
        private String id;

        [JsonProperty("parentId")]
        protected String parentId;

        [JsonProperty("parentType")]
        protected Type parentType;

        [JsonProperty("creationDate")]
        protected String creationDate;

        [JsonProperty("lastUpdatedDate")]
        protected String lastUpdatedDate;

        [JsonProperty("owner")]
        protected String owner;

        [JsonIgnore]
        protected RestSessionBase session;

        public string NUId { get => id; set => id = value; }
        public string NUParentId { get => parentId; set => parentId = value; }
        public Type NUParentType { get => parentType; set => parentType = value; }
        public string NUCreationDate { get => creationDate; set => creationDate = value; }
        public string NULastUpdatedDate { get => lastUpdatedDate; set => lastUpdatedDate = value; }
        public string NUOwner { get => owner; set => owner = value; }


        public String getResourceUrl(RestSessionBase session, bool ignoreRoot = false)
        {
            MethodInfo info = this.GetType().GetMethod("getResourceName");
            var resourceName = info.Invoke(null, null);

            String url = session.getRestBaseUrl();

            if (NUId != null)
            {
                return String.Format("{0}/{1}/{2}", url, resourceName, NUId);
            }
            else if (ignoreRoot)
            {
                return String.Format("{0}", url);
            }
            else
            {
                return String.Format("{0}/{1}", url, resourceName);
            }
        }

        public String getResourceUrlForChildType(RestSessionBase session, Type childRestObjClass)
        {
            MethodInfo info = childRestObjClass.GetMethod("getResourceName");
            var childResourceName = info.Invoke(null, null);
            
            return String.Format("{0}/{1}", getResourceUrl(session,session.amIRootObject(this)), childResourceName);
        }

        public void createChild<T>(RestSessionBase session, T childRestObj)
        {
            this.createChild(session, childRestObj, -1, true);
        }

        public void createChild<T>(RestSessionBase session, T childRestObj, int responseChoice, bool commit)
        {
            String parameters = null;
            if (responseChoice != -1) parameters = "responseChoice=" + responseChoice.ToString();

            string jsonToPost = JsonConvert.SerializeObject(childRestObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            HttpWebResponse response = session.sendRequestWithRetry("POST", getResourceUrlForChildType(session, childRestObj.GetType()), parameters, null, jsonToPost);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                String data = reader.ReadToEnd();
                reader.Close();
                response.Close();

                List<T> list = JsonConvert.DeserializeObject<List<T>>(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
                T responseRestObj = list[0];
                if (commit)
                {
                    copyJsonProperties(responseRestObj, childRestObj);
                    //TODO: addChild(childRestObj);
                }
            }
        }

        private void copyJsonProperties<T>(T source, T destination)
        {
            Type sourceType = source.GetType();
            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                PropertyInfo pi = destination.GetType().GetProperty(p.Name);
                if (pi == null) continue;

                pi.SetValue(destination,p.GetValue(source,null));
            }
        }

        /*private void addChild(RestObject childRestObj)
        {
            // Get the object's resource name
            String restName = getRestName(childRestObj.getClass());

        // Add child object to registered fetcher for child type
        RestFetcher<RestObject> children = (RestFetcher<RestObject>)fetcherRegistry.get(restName);
        if (children == null) {
            throw new RestException(String.format("Could not find fetcher with name %s while adding %s in parent %s", restName, childRestObj, this));
        }

        if (!children.contains(childRestObj)) {
            children.add(childRestObj);
        }
        }*/
    }
}

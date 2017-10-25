/*
  Copyright (c) 2017, Nokia
  All rights reserved.

  Redistribution and use in source and binary forms, with or without
  modification, are permitted provided that the following conditions are met:
      * Redistributions of source code must retain the above copyright
        notice, this list of conditions and the following disclaimer.
      * Redistributions in binary form must reproduce the above copyright
        notice, this list of conditions and the following disclaimer in the
        documentation and/or other materials provided with the distribution.
      * Neither the name of the copyright holder nor the names of its contributors
        may be used to endorse or promote products derived from this software without
        specific prior written permission.

  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
  DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY
  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;

namespace net.nuagenetworks.bambou
{
    public abstract class RestObject
    {
        [JsonProperty("creationDate")]
        protected String creationDate;

        [JsonProperty("lastUpdatedDate")]
        protected String lastUpdatedDate;

        [JsonProperty("owner")]
        protected String owner;

        [JsonProperty("parentId")]
        protected String parentId;

        [JsonProperty("parentType")]
        protected string parentType;

        [JsonIgnore]
        protected RestSessionBase session;

        [JsonProperty("id")]
        private String id;

        [JsonIgnore]
        public string NUCreationDate { get => creationDate; set => creationDate = value; }
        [JsonIgnore]
        public string NUId { get => id; set => id = value; }
        [JsonIgnore]
        public string NULastUpdatedDate { get => lastUpdatedDate; set => lastUpdatedDate = value; }
        [JsonIgnore]
        public string NUOwner { get => owner; set => owner = value; }
        [JsonIgnore]
        public string NUParentId { get => parentId; set => parentId = value; }

        public Type NUParentType
        {
            get
            {
                if (parentType == null || parentType == "") return null;

                String ns = this.GetType().Namespace;
                String asm = this.GetType().Assembly.FullName;
                Type t = Type.GetType(ns + "." + this.parentType+", "+asm,false,true);
                return t;
            }
            set
            {
                if (value == null)
                {
                    this.parentType = "";
                }
                else
                {
                    parentType = value.Name;
                }
            }
        }

        public void createChild<T>(RestSessionBase session, T childRestObj)
        {
            this.createChild(session, childRestObj, -1);
        }

        public void createChild<T>(RestSessionBase session, T childRestObj, int responseChoice)
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
                copyJsonProperties(responseRestObj, childRestObj);
            }
        }

        public void fetch(RestSessionBase session)
        {
            HttpWebResponse response = session.sendRequestWithRetry("GET", getResourceUrl(session), null, null, null);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                String data = reader.ReadToEnd();
                reader.Close();
                response.Close();

                var objList = JsonConvert.DeserializeObject<List<JObject>>(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore });
                JObject jobject = objList[0];
                var obj = jobject.ToObject(this.GetType());
                copyJsonProperties(obj, this);
                return;
            }

            response.Close();
            throw new RestException(response.StatusDescription);
        }

        public void delete(RestSessionBase session)
        {
            delete(session, -1);
        }

        public void delete(RestSessionBase session, int responseChoice)
        {
            String parameters = null;
            if (responseChoice != -1) parameters = "responseChoice=" + responseChoice.ToString();
            string jsonToPost = JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            HttpWebResponse response = session.sendRequestWithRetry("DELETE", getResourceUrl(session), parameters, null, jsonToPost);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                response.Close();
                return;
            }

            response.Close();
            throw new RestException("Response received with status code: " + response.StatusCode);
        }

        public void save(RestSessionBase session)
        {
            save(session, -1);
        }

        public void save(RestSessionBase session, int responseChoice)
        {
            String parameters = null;
            if (responseChoice != -1) parameters = "responseChoice=" + responseChoice.ToString();
            string jsonToPost = JsonConvert.SerializeObject(this, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            HttpWebResponse response = session.sendRequestWithRetry("PUT", getResourceUrl(session), parameters, null, jsonToPost);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                response.Close();
                return;
            }

            response.Close();
            throw new RestException("Response received with status code: " + response.StatusCode);
        }

        public void assign<T>(RestSessionBase session, List<T> children) where T : RestObject
        {
            List<string> ids = new List<string>();
            foreach (T child in children)
            {
                ids.Add(child.NUId);
            }

            assign<T>(session, ids);
        }

        public void assign<T>(RestSessionBase session, List<string> children)
        {
            if (typeof(T) == typeof(RestObject)) throw new RestException("Must use a derived type of RestObject");

            string jsonToPost = JsonConvert.SerializeObject(children, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            HttpWebResponse response = session.sendRequestWithRetry("PUT", getResourceUrlForChildType(session,typeof(T)), null, null, jsonToPost);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                response.Close();
                return;
            }

            response.Close();
            throw new RestException("Response received with status code: " + response.StatusCode);
        }

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

            return String.Format("{0}/{1}", getResourceUrl(session, session.amIRootObject(this)), childResourceName);
        }

        private void copyJsonProperties<T>(T source, T destination)
        {
            Type sourceType = source.GetType();
            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in properties)
            {
                PropertyInfo pi = destination.GetType().GetProperty(p.Name);
                if (pi == null) continue;

                pi.SetValue(destination, p.GetValue(source, null));
            }
        }
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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace net.nuagenetworks.bambou
{
    public class RestSession<T> : RestSessionBase where T: RestObject,new()
    {
        private T rootObject;

        private const String ORGANIZATION_HEADER = "X-Nuage-Organization";
        private const String CONTENT_TYPE_JSON = "application/json";

        protected RestSession()
        {
            this.apiKey = "";
        }


        public void setUsername(String value)
        {
            this.username = value;
        }
        public void setPassword(String value)
        {
            this.password = value;
        }
        public void setEnterprise(String value)
        {
            this.enterprise = value;
        }
        public void setApiUrl(String value)
        {
            this.apiUrl = value;
        }
        public void setApiPrefix(String value)
        {
            this.apiPrefix = value;
        }
        public void setVersion(double value)
        {
            this.version = value;
        }
        public void setCertificate(String value)
        {
            this.certificate = value;
        }
        public void setPrivateKey(String value)
        {
            this.privateKey = value;
        }

        public T getRootObject()
        {
            if (this.rootObject == null) this.rootObject = new T();
            return this.rootObject;
        }

        private string getAuthenticationHeader()
        {
            if (this.apiKey == "")
            {
                return "XREST " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(this.username+":"+this.password));
            }
            else
            {
                return "XREST " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(this.username + ":" + this.apiKey));
            }
        }

        public override HttpWebResponse sendRequestWithRetry(string method, String url, String parameters, WebHeaderCollection headers)
        {
            if (parameters != null)
            {
                url += (url.IndexOf('?') >= 0) ? ";" + parameters : "?" + parameters;
            }

            if (headers == null)
            {
                headers = new WebHeaderCollection();
            }

            headers.Set(ORGANIZATION_HEADER, this.enterprise);
            string auth = getAuthenticationHeader();
            headers.Set(HttpRequestHeader.Authorization, auth);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = CONTENT_TYPE_JSON;
            request.Method = method;
            request.Headers = headers;
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return response;
            }
            catch (WebException we)
            {
                HttpWebResponse response = (HttpWebResponse)we.Response;
                if (response == null) throw new RestException(we.Message);

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (!(method == "GET" && url == rootObject.getResourceUrl(this)))
                    {
                        // Re-authenticate the session and try to send the same
                        // request again. A new API key might get issued as a result
                        this.authenticate();

                        return sendRequestWithRetry(method, url, parameters, headers);
                    }
                }

              throw new RestException(we.Message);
            }
        }

        private void authenticate()
        {
            string url = rootObject.getResourceUrl(this);
            HttpWebResponse r = sendRequestWithRetry("GET", url, null, null);
            if (r.StatusCode != HttpStatusCode.OK) throw new RestException("Could not authenticate");

            StreamReader reader = new StreamReader(r.GetResponseStream());
            String data = reader.ReadToEnd();
            reader.Close();
            r.Close();

            dynamic jdata = JArray.Parse(data);
            this.apiKey = jdata[0].APIKey;
        }
    }
}

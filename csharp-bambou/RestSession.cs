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

using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace net.nuagenetworks.bambou
{
    public class RestSession<T> : RestSessionBase where T : RestObject, new()
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
                return "XREST " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(this.username + ":" + this.password));
            }
            else
            {
                return "XREST " + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(this.username + ":" + this.apiKey));
            }
        }

        public override HttpWebResponse sendRequestWithRetry(string method, String url, String parameters, WebHeaderCollection headers, string body = null)
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
            request.Method = method;
            request.Headers = headers;
            request.ContentType = CONTENT_TYPE_JSON;
            request.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            if (body != null)
            {
                var byteData = Encoding.ASCII.GetBytes(body);
                request.ContentLength = byteData.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                    stream.Close();
                }
            }

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

                        return sendRequestWithRetry(method, url, parameters, headers, body);
                    }
                }

                StreamReader reader = new StreamReader(response.GetResponseStream());
                String data = reader.ReadToEnd();
                reader.Close();
                response.Close();

                if (response.StatusCode == HttpStatusCode.MultipleChoices)
                {
                    throw new RestMultipleChoiceException(data);
                }

                throw new RestException(we.Message + ": " + data);
            }
        }

        public override bool amIRootObject(Object obj)
        {
            return this.rootObject.GetType() == obj.GetType();
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
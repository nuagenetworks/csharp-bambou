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
using net.nuagenetworks.bambou.operation;
using System;
using System.Collections.Generic;

namespace net.nuagenetworks.bambou
{
    public class GenericRestSession : RestSessionOperations
    {
        public void assign(RestObject restObj, List<RestObject> childRestObjs)
        {
            restObj.assign(this, childRestObjs);
        }

        public void assign(RestObject restObj, List<RestObject> childRestObjs, bool commit)
        {
            restObj.assign(this, childRestObjs, commit);
        }

        public int count<T>(RestFetcher<T> fetcher) where T : RestObject
        {
            return count(fetcher, null, null, null, null, null, null, true);
        }

        public int count<T>(RestFetcher<T> fetcher, string filter, string orderBy, string[] groupBy, int page, int pageSize, string queryParameters, bool commit) where T : RestObject
        {
            return fetcher.count(this, filter, orderBy, groupBy, page, pageSize, queryParameters, commit);
        }

        public void createChild(RestObject restObj, RestObject childRestObj)
        {
            restObj.createChild(this, childRestObj);
        }

        public void createChild(RestObject restObj, RestObject childRestObj, int responseChoice, bool commit)
        {
            restObj.createChild(this, childRestObj, responseChoice, commit);
        }

        public void delete(RestObject restObj)
        {
            restObj.delete(this);
        }

        public void delete(RestObject restObj, int responseChoice)
        {
            restObj.delete(this, responseChoice);
        }

        public void fetch(RestObject restObj)
        {
            restObj.fetch(this);
        }

        public List<T> fetch<T>(RestFetcher<T> fetcher) where T : RestObject
        {
            return fetch(fetcher, null, null, null, null, null, null, true);
        }

        public List<T> fetch<T>(RestFetcher<T> fetcher, string filter, string orderBy, string[] groupBy, int page, int pageSize, string queryParameters, bool commit) where T : RestObject
        {
            return fetcher.fetch(this, filter, orderBy, groupBy, page, pageSize, queryParameters, commit);
        }

        public List<T> get<T>(RestFetcher<T> fetcher) where T : RestObject
        {
            return get(fetcher, null, null, null, null, null, null, true);
        }

        public List<T> get<T>(RestFetcher<T> fetcher, string filter, string orderBy, string[] groupBy, int page, int pageSize, string queryParameters, bool commit) where T : RestObject
        {
            return fetcher.get(this, filter, orderBy, groupBy, page, pageSize, queryParameters, commit);
        }

        public T getFirst<T>(RestFetcher<T> fetcher) where T : RestObject
        {
            return getFirst(fetcher, null, null, null, null, null, null, true);
        }

        public T getFirst<T>(RestFetcher<T> fetcher, string filter, string orderBy, string[] groupBy, int page, int pageSize, string queryParameters, bool commit) where T : RestObject
        {
            return fetcher.getFirst(this, filter, orderBy, groupBy, page, pageSize, queryParameters, commit);
        }

        public void instantiateChild(RestObject restObj, RestObject childRestObj, RestObject fromTemplate)
        {
            restObj.instantiateChild(this, childRestObj, fromTemplate);
        }

        public void instantiateChild(RestObject restObj, RestObject childRestObj, RestObject fromTemplate, int responseChoice, bool commit)
        {
            restObj.instantiateChild(this, childRestObj, fromTemplate, responseChoice, commit);
        }

        public void reset()
        {
            restRootObj = null;
            apiKey = null;
            currentSession.set(null);
        }

        public void save(RestObject restObj)
        {
            restObj.save(this);
        }

        public void save(RestObject restObj, int responseChoice)
        {
            restObj.save(this, responseChoice);
        }

        public void start()
        {
            currentSession.set(this);
            restClientService.prepareSSLAuthentication(certificate, privateKey);
            authenticate();
        }
    }

    public class RestSession<R> : GenericRestSession where R: RestRootObject 
        {

        private const String ORGANIZATION_HEADER = "X-Nuage-Organization";
        private const String CONTENT_TYPE_JSON = "application/json";
//        private const ThreadLocal<RestSession<?>> currentSession = new ThreadLocal<RestSession<?>>();

        private RestClientService restClientService;

        private String username;
        private String password;
        private String enterprise;
        private String apiUrl;
        private String apiPrefix;
        private String certificate;
        private String privateKey;
        private double version;
        private String apiKey;
        private Type restRootObjClass;
        private R restRootObj;

        public RestSession(Type restRootObjClass) {
            this.restRootObjClass = restRootObjClass;
        }

        public String getUsername() {
            return username;
        }

        public void setUsername(String username) {
            this.username = username;
        }

        public String getPassword() {
            return password;
        }

        public void setPassword(String password) {
            this.password = password;
        }

        public String getEnterprise() {
            return enterprise;
        }

        public void setEnterprise(String enterprise) {
            this.enterprise = enterprise;
        }

        public String getApiUrl() {
            return apiUrl;
        }

        public void setApiUrl(String apiUrl) {
            this.apiUrl = apiUrl;
        }

        public String getApiPrefix() {
            return apiPrefix;
        }

        public void setApiPrefix(String apiPrefix) {
            this.apiPrefix = apiPrefix;
        }

        public String getCertificate() {
            return certificate;
        }

        public void setCertificate(String certificate) {
            this.certificate = certificate;
        }

        public String getPrivateKey() {
            return privateKey;
        }

        public void setPrivateKey(String privateKey) {
            this.privateKey = privateKey;
        }

        public double getVersion() {
            return version;
        }

        public void setVersion(double version) {
            this.version = version;
        }

        protected void setApiKey(String apiKey) {
            this.apiKey = apiKey;
        }

        protected static GenericRestSession getCurrentSession() {
            return currentSession.get();
        }

        public R getRootObject() {
            return restRootObj;
        }


        protected <T, U> ResponseEntity<T> sendRequestWithRetry(HttpMethod method, String url, String params, HttpHeaders headers, U requestObj,
                Class<T> responseType) throws RestException {
            if (params != null) {
                url += (url.indexOf('?') >= 0) ? ";" + params : "?" + params;
            }

            if (headers == null) {
                headers = new HttpHeaders();
            }

            headers.set(HttpHeaders.CONTENT_TYPE, CONTENT_TYPE_JSON);
            headers.set(ORGANIZATION_HEADER, getEnterprise());
            headers.set(HttpHeaders.AUTHORIZATION, getAuthenticationHeader());

            try {
                return restClientService.sendRequest(method, url, headers, requestObj, responseType);
            } catch (RestStatusCodeException ex) {
                if (ex.getStatusCode() == HttpStatus.UNAUTHORIZED) {
                    // Debug
                    logger.info("HTTP 401/Unauthorized response received");

                    // Make sure we are not already re-authenticating
                    // in order to avoid infinite recursion
                    if (!(method == HttpMethod.GET && url.equals(restRootObj.getResourceUrl(this)))) {
                        // Re-authenticate the session and try to send the same
                        // request again. A new API key might get issued as a result
                        reset();
                        authenticate();

                        // Update authorization header with new API key
                        headers.set(HttpHeaders.AUTHORIZATION, getAuthenticationHeader());

                        return restClientService.sendRequest(method, url, headers, requestObj, responseType);
                    } else {
                        throw ex;
                    }
                } else {
                    throw ex;
                }
            }
        }

        protected String getRestBaseUrl() {
            return String.format("%s/%s/v%s", apiUrl, apiPrefix, String.valueOf(version).replace('.', '_'));
        }

        private synchronized void authenticate() throws RestException {
            // Create the root object if needed
            if (restRootObj == null) {
                restRootObj = createRootObject();
                fetch(restRootObj);
            }

            // Copy the API key from the root object
            apiKey = restRootObj.getApiKey();

            // Debug
            logger.debug("Started session with username: " + username + " in enterprise: " + enterprise);
        }

        private R createRootObject() throws RestException {
            try {
                return restRootObjClass.newInstance();
            } catch (InstantiationException | IllegalAccessException ex) {
                throw new RestException(ex);
            }
        }

        private String getAuthenticationHeader() {
            if (apiKey != null) {
                return String.format("XREST %s", Base64.encodeBase64String(String.format("%s:%s", username, apiKey).getBytes()));
            } else {
                return String.format("XREST %s", Base64.encodeBase64String(String.format("%s:%s", username, password).getBytes()));
            }
        }

        public override String toString() {
            return "RestSession [restClientService=" + restClientService + ", username=" + username + ", password=" + password + ", enterprise=" + enterprise
                    + ", apiUrl=" + apiUrl + ", apiPrefix=" + apiPrefix + ", certificate=" + certificate + ", privateKey=" + privateKey + ", version=" + version
                    + ", apiKey=" + apiKey + ", restRootObjClass=" + restRootObjClass + ", restRootObj=" + restRootObj + "]";
        }
    }
}
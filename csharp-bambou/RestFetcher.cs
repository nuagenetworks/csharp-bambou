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
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace net.nuagenetworks.bambou
{
    public class RestFetcher<T>
    {
        private const String ATTRIBUTES_HEADER = "X-Nuage-Attributes";
        private const String COUNT_HEADER = "X-Nuage-Count";
        private const String FILTER_HEADER = "X-Nuage-Filter";
        private const String GROUP_BY_HEADER = "X-Nuage-GroupBy";
        private const String ORDER_BY_HEADER = "X-Nuage-OrderBy";
        private const String PAGE_HEADER = "X-Nuage-Page";
        private const String PAGE_SIZE_HEADER = "X-Nuage-PageSize";
        private Type childType;
        private RestObject parentObject;

        public RestFetcher(RestObject obj, Type type)
        {
            this.parentObject = obj;
            this.childType = type;
        }

        public List<T> fetch(RestSessionBase session)
        {
            return fetch(session, null, null, null, -1, -1, null, true);
        }

        public List<T> fetch(RestSessionBase session, String filter, String orderBy, String[] groupBy, int page, int pageSize, String queryParameters, bool commit)
        {
            String resourceUrl = getResourceUrl(session);
            WebHeaderCollection headers = prepareHeaders(filter, orderBy, groupBy, page, pageSize);
            HttpWebResponse response = session.sendRequestWithRetry("GET", resourceUrl, queryParameters, headers);
            if ((int)response.StatusCode >= 200 && (int)response.StatusCode < 300)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                String data = reader.ReadToEnd();
                reader.Close();
                response.Close();

                return JsonConvert.DeserializeObject<List<T>>(data, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            throw new RestException("Response received with status code: " + response.StatusCode);
        }

        private String getResourceUrl(RestSessionBase session)
        {
            return parentObject.getResourceUrlForChildType(session, childType);
        }

        private WebHeaderCollection prepareHeaders(String filter, String orderBy, String[] groupBy, int page, int pageSize)
        {
            WebHeaderCollection headers = new WebHeaderCollection();

            if (filter != null)
            {
                headers.Add(FILTER_HEADER, filter);
            }

            if (orderBy != null)
            {
                headers.Add(ORDER_BY_HEADER, orderBy);
            }

            if (page != -1)
            {
                headers.Add(PAGE_HEADER, page.ToString());
            }

            if (pageSize != -1)
            {
                headers.Add(PAGE_SIZE_HEADER, pageSize.ToString());
            }

            if (groupBy != null && groupBy.Length > 0)
            {
                string header = string.Join(",", groupBy);
                headers.Add(GROUP_BY_HEADER, "true");
                headers.Add(ATTRIBUTES_HEADER, header);
            }

            return headers;
        }
    }
}
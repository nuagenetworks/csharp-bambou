using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Net.Http;
using System.IO;

namespace net.nuagenetworks.bambou
{
    public class RestFetcher<T>
    {
        private RestObject parentObject;
        private Type childType;

        private const String FILTER_HEADER = "X-Nuage-Filter";
        private const String ORDER_BY_HEADER = "X-Nuage-OrderBy";
        private const String PAGE_HEADER = "X-Nuage-Page";
        private const String PAGE_SIZE_HEADER = "X-Nuage-PageSize";
        private const String GROUP_BY_HEADER = "X-Nuage-GroupBy";
        private const String ATTRIBUTES_HEADER = "X-Nuage-Attributes";
        private const String COUNT_HEADER = "X-Nuage-Count";

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

                return JsonConvert.DeserializeObject<List<T>>(data);
            }

            throw new RestException("Response received with status code: " + response.StatusCode);
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
        private String getResourceUrl(RestSessionBase session)
        {
            return parentObject.getResourceUrlForChildType(session, childType);
        }
    }
}

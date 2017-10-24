using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace net.nuagenetworks.bambou
{
    public abstract class RestSessionBase
    {
        protected String username;
        protected String password;
        protected String enterprise;
        protected String apiUrl;
        protected String apiPrefix;
        protected String certificate;
        protected String privateKey;
        protected double version;
        protected String apiKey;

        public string getRestBaseUrl()
        {
            return String.Format("{0}/{1}/v{2}", apiUrl, apiPrefix, version.ToString("#0.0#############", CultureInfo.CurrentCulture).Replace('.', '_'));
        }

        public abstract HttpWebResponse sendRequestWithRetry(string method, String url, String parameters, WebHeaderCollection headers, string body=null);

        public abstract bool amIRootObject(Object obj);
    }
}

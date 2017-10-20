using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.nuagenetworks.bambou
{
    public class RestSession<T>
    {
        private String username;
        private String password;
        private String enterprise;
        private String apiUrl;
        private String apiPrefix;
        private String certificate;
        private String privateKey;
        private double version;
        private String apiKey;
        private T rootObject;

        public void Authenticate()
        {

        }

        public void Start()
        {

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
            return rootObject;
        }
    }

}

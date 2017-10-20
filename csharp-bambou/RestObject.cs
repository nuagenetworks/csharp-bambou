using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace net.nuagenetworks.bambou
{
    public class RestObject
    {
        [JsonProperty("id")]
        protected String id;

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


        public String getId()
        {
            return id;
        }

        public void setId(String value)
        {
            this.id = value;
        }

        public Type getParentType()
        {
            return parentType;
        }

        public void setParentType(Type value)
        {
            this.parentType = value;
        }

        public String getParentId()
        {
            return parentId;
        }

        public void setParentId(String value)
        {
            this.parentId = value;
        }

        public String getCreationDate()
        {
            return creationDate;
        }

        public void setCreationDate(String value)
        {
            this.creationDate = value;
        }

        public String getLastUpdatedDate()
        {
            return lastUpdatedDate;
        }

        public void setLastUpdatedDate(String value)
        {
            this.lastUpdatedDate = value;
        }

        public String getOwner()
        {
            return owner;
        }

        public void setOwner(String value)
        {
            this.owner = value;
        }
    }
}

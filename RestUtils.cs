using net.nuagenetworks.bambou.util;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace net.nuagenetworks.bambou
{ 
    public class RestUtils {

        public static T createRestObjectWithContent<T>(JObject jsonNode)
        {
            try
            {
                T ret = jsonNode.ToObject<T>();
                return ret;
            }
            catch (JsonException ex)
            { 
                throw new RestException(ex.Message, ex);
            }
        }

        public static String toString(Object content)
        {
            return BambouUtils.toString(content);
        }
    }

}
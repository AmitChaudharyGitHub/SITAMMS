using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MMS
{
    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.MaxJsonLength = 99999999;
            return serializer.Serialize(obj).Replace("\n","");
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(obj);
        }
        public static string Truncate(this string source, int length)
        {
            if (source!=null)
            {
                if (source.Length > length)
                {
                    source = source.Substring(0, length);
                }
            }

            return source;
        }
    }
   
}
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;

namespace Auditor.Utilities
{
    public static class ConvertingUtilities
    {
        /**
         * <summary>This can be exploited if its not a secure set of bytes</summary>
         */
        [Obsolete]
        public static T ByteToType<T>(byte[] bytes)
        {
            if (bytes == null)
            {
                return default(T);
            }

            BinaryFormatter binaryFormatter = new();
            using (MemoryStream memoryStream = new(bytes))
            {
                object o = binaryFormatter.Deserialize(memoryStream);
                return (T) o;
            }
        }

        public static T JsonToClass<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
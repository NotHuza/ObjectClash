namespace CR.Servers.CoC.Extensions.Helper
{
    using System;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using Newtonsoft.Json.Linq;

    public class JsonHelper
    {
        internal static bool GetJsonArray(JToken Token, string Key, out JArray JArray)
        {
            return (JArray = (JArray) Token[Key]) != null;
        }

        internal static bool GetJsonObject(JToken Token, string Key, out JToken JToken)
        {
            return (JToken = Token[Key]) != null;
        }

        internal static bool GetJsonString(JToken Token, string Key, out string String)
        {
            return (String = (string) Token[Key]) != null;
        }

        internal static bool GetJsonData(JToken Token, string Key, out Data Data)
        {
            int Id = 0;
            return (Data = JsonHelper.GetJsonNumber(Token, Key, out Id) ? CSV.Tables.GetWithGlobalId(Id) : null) != null;
        }

        internal static bool GetJsonData<T>(JToken Token, string Key, out T Data) where T : Data
        {
            int Id = 0;
            return (Data = JsonHelper.GetJsonNumber(Token, Key, out Id) ? CSV.Tables.GetWithGlobalId(Id) as T : null) != null;
        }

        internal static bool GetJsonBoolean(JToken Token, string Key, out bool Bool)
        {
            JToken KeyValue = Token[Key];

            if (KeyValue != null)
            {
                Bool = (bool) KeyValue;
                return true;
            }

            Bool = false;

            return false;
        }

        internal static bool GetJsonNumber(JToken Token, string Key, out int Int)
        {
            JToken KeyValue = Token[Key];

            if (KeyValue != null)
            {
                Int = (int) KeyValue;
                return true;
            }

            Int = -1;

            return false;
        }

        internal static bool GetJsonNumber(JToken Token, string Key, out long Long)
        {
            JToken KeyValue = Token[Key];

            if (KeyValue != null)
            {
                Long = (long) KeyValue;
                return true;
            }

            Long = -1;

            return false;
        }

        internal static bool GetJsonByte(JToken Token, string Key, out byte Byte)
        {
            JToken KeyValue = Token[Key];

            if (KeyValue != null)
            {
                Byte = (byte) KeyValue;
                return true;
            }

            Byte = 0;

            return false;
        }

        internal static bool GetJsonDateTime(JToken Token, string Key, out DateTime Time)
        {
            JToken KeyValue = Token[Key];

            if (KeyValue != null)
            {
                Time = (DateTime) KeyValue;
                return true;
            }

            Time = DateTime.UtcNow;

            return false;
        }
    }
}
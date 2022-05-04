using System;
using System.ComponentModel;
using System.Reflection;

namespace PluginICAOClientSDK {
    public class Utils {
        public static readonly int SUCCESS = 0;
        public static readonly int ERR_FOR_DENIED_AUTH = 1302;
        public static readonly int ERR_FOR_DENIED_CONNECTION = 1008;
        public static readonly int ERR_PUT_CARD = 1101;
        public static readonly int READ_TIMEOUT = 60;

        public static string getUUID() {
            return Guid.NewGuid().ToString();
        }

        public static string ToDescription(Enum value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}

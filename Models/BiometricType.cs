using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;

namespace PluginICAOClientSDK.Models {
    public enum BiometricType {
        [Description("faceID")]
        [JsonConverter(typeof(StringEnumConverter))]
        TYPE_FACE,
        [Description("fingerLeftIndex")]
        [JsonConverter(typeof(StringEnumConverter))]
        TYPE_FINGER_LEFT,
        [Description("fingerRightIndex")]
        [JsonConverter(typeof(StringEnumConverter))]
        TYPE_FINGER_RIGHT,
    }
}

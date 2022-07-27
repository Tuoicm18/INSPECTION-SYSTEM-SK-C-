using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PluginICAOClientSDK.Models {
    public enum ChallengeType {
        [Description("string")]
        [JsonConverter(typeof(StringEnumConverter))]
        TYPE_STRING,
        [Description("object")]
        [JsonConverter(typeof(StringEnumConverter))]
        TYPE_OBJECT,
    }
}

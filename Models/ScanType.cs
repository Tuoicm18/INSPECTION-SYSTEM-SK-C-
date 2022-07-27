using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel;

namespace PluginICAOClientSDK.Models {
    public enum ScanType {
        [Description("JPG")]
        [JsonConverter(typeof(StringEnumConverter))]
        JPG,
        [JsonConverter(typeof(StringEnumConverter))]
        [Description("PDF")]
        PDF,
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PluginICAOClientSDK.Models;
using System;

namespace PluginICAOClientSDK.Response.DisplayInformation {
    public class DataDisplayInformation {
        public string title { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DisplayType type { get; set; }
        public string value { get; set; }
    }
}

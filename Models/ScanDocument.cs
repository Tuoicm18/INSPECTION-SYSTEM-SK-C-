using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Models {
    public class ScanDocument {
        [JsonConverter(typeof(StringEnumConverter))]
        public ScanType scanType { get; set; }
        public bool saveEnabled { get; set; }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Models {
    public class ScanDocument {
        public string scanType { get; set; }
        public bool saveEnabled { get; set; }
    }
}

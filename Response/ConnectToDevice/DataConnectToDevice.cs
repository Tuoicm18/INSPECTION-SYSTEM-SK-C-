using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.ConnectToDevice {
    public class DataConnectToDevice {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string deviceName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string deviceSN { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string deviceIP { get; set; }
    }
}

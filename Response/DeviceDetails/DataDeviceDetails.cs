using Newtonsoft.Json;
using System;

namespace PluginICAOClientSDK.Response.DeviceDetails {
    public class DataDeviceDetails {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string deviceName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string deviceSN { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string lastScanTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int totalPreceeded { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool activePresenceDetection { get; set; }
    }
}

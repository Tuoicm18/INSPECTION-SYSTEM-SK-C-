using Newtonsoft.Json;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Request;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.BiometricAuth {
    public class DataBiometricAuth {
        public string biometricType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int score { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JWT {get; set;}

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string issueDetails { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AuthorizationData authorizationData { get; set; }
    }
}

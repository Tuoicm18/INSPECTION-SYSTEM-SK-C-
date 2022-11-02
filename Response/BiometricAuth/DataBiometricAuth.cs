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
        public string jwt {get; set;}

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int issueDetailCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string issueDetailMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string biometricEvidence { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ChallengeBiometricAuth challenge { get; set; }
    }
}

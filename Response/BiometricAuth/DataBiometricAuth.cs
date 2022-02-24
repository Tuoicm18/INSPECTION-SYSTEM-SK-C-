using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Request;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.BiometricAuth {
    public class DataBiometricAuth {
        public string biometricType { get; set; }
        public bool result { get; set; }
        public float score { get; set; }
        public string JWT {get; set;}
        public string issueDetails { get; set; }
        public AuthorizationData authorizationData { get; set; }
    }
}

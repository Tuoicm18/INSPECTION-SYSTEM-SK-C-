using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.BiometricAuth {
    public class BaseBiometricAuthResp {
        public string cmdType { get; set; }
        public string requestID { get; set; }
        public int timeOutInterVal { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DataBiometricAuth data { get; set; }
    }
}

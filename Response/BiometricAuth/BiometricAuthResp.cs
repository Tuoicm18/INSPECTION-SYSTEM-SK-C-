using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.BiometricAuth {
    public class BiometricAuthResp : BaseResponse {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DataBiometricAuth data { get; set; }
    }
}

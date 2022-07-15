using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.BiometricAuth {
    public class BaseBiometricAuthResp : BaseResponse {

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DataBiometricAuth data { get; set; }
    }
}

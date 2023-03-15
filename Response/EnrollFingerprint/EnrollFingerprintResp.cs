using Newtonsoft.Json;
using System;

namespace PluginICAOClientSDK.Response.EnrollFingerprint {
    public class EnrollFingerprintResp : BaseResponse {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DataEnrollFingerprintcs data { get; set; }
    }
}

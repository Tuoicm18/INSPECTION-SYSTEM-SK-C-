using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Response.BiometricEvidence {
    public class BiometricEvidenceResp : BaseResponse {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DataBiometricEvidence data { get; set; }
    }
}

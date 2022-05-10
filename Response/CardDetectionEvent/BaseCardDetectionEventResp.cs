using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Response.CardDetectionEvent {
    public class BaseCardDetectionEventResp {
        public string cmdType { get; set; }
        public string requestID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int timeOutInterval { get; set; }

        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DataCardDetectionEvent data { get; set; }
    }
}

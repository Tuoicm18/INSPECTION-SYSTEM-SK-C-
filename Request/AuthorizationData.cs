using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Request {
    public class AuthorizationData {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string authorizationTitle { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AuthorizationElement> authContentList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AuthorizationElement> multipleSelectList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AuthorizationElement> singleSelectList { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AuthorizationElement> nameValuePairList { get; set; }
    }
}

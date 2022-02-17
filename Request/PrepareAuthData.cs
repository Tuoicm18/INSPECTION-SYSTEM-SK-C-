using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginICAOClientSDK.Request {
    public class PrepareAuthData {
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

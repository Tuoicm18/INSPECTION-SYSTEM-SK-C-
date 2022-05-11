using Newtonsoft.Json;
using PluginICAOClientSDK.Models;
using System;

namespace PluginICAOClientSDK.Response.GetDocumentDetails {
    public class DataDocumentDetails {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool paceEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool bacEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool activeAuthenticationEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool chipAuthenticationEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool terminalAuthenticationEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool passiveAuthenticationEnabled { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string efCom { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string efSod { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string efCardAccess { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string mrzString { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte[] image { get; set; }
        //Update 2022.05.11
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string jwt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DataGroup dataGroup { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public OptionalDetails optionalDetails { get; set; }
    }
}

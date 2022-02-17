using PluginICAOClientSDK.Models;
using System;

namespace PluginICAOClientSDK.Response.GetDocumentDetails {
    public class DataDocumentDetails {
        public bool paceEnabled { get; set; }
        public bool bacEnabled { get; set; }
        public bool activeAuthenticationEnabled { get; set; }
        public bool chipAuthenticationEnabled { get; set; }
        public bool terminalAuthenticationEnabled { get; set; }
        public bool passiveAuthenticationEnabled { get; set; }
        public string efCom { get; set; }
        public string efSod { get; set; }
        public string efCardAccess { get; set; }
        public string mrzString { get; set; }
        public byte[] image { get; set; }
        public DataGroup dataGroup { get; set; }
        public OptionalDetails optionalDetails { get; set; }
    }
}

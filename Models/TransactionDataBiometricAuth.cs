using PluginICAOClientSDK.Request;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Models {
    public class TransactionDataBiometricAuth {
        public string transactionTitle { get; set; }
        public List<AuthorizationElement> authContentList { get; set; }
        public List<AuthorizationElement> multipleSelectList { get; set; }
        public List<AuthorizationElement> singleSelectList { get; set; }
        public List<AuthorizationElement> nameValuePairList { get; set; }
        public List<AuthorizationElement> documentDigestList { get; set; }
    }
}

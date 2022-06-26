using PluginICAOClientSDK.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Models {
    public class TransactionDataBiometricAuth {
        public string transactionTitle { get; set; }
        public List<AuthorizationElement> authContentList { get; set; }
        public List<AuthorizationElement> multipleSelectList { get; set; }
        public List<AuthorizationElement> singleSelectList { get; set; }
        public List<AuthorizationElement> nameValuePairList { get; set; }
    }
}

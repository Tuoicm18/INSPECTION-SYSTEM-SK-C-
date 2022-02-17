using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Request {
    public class AuthorizationData {
        public List<AuthorizationElement> authContentList { get; set; }
        public List<AuthorizationElement> multipleSelectedList { get; set; }
        public List<AuthorizationElement> singleSelectedList { get; set; }
        public List<AuthorizationElement> nameValuePairList { get; set; }
    }
}

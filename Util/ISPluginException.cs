using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Util {
    public class ISPluginException : Exception {
        public int errCode { get; set; }
        public string errMsg { get; set; }
        public ISPluginException(int errCode, string errMsg) : base(errMsg) {
            this.errCode = errCode;
            this.errMsg = errMsg;
        }
        public ISPluginException (string str) : base(str) {}

        public ISPluginException (Exception ex) : base("", ex) {}
    } 
}

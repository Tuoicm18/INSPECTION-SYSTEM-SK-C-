using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Util {
    public class ISPluginException : Exception {
        public ISPluginException (string str) : base(str) {}

        public ISPluginException (Exception ex) : base("", ex) {}
    } 
}

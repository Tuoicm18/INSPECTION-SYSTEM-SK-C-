using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Response.DisplayInformation {
    public class BaseDisplayInformation {
        public string cmdType { get; set; }
        public string requestID { get; set; }
        public int timeOutInterval { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public DataDisplayInformation data { get; set; }
    }
}

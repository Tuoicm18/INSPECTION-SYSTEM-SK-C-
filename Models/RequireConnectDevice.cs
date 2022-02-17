using PluginICAOClientSDK.Models;
using System;

namespace PluginICAOClientSDK.Models {
    public class RequireConnectDevice {
        public bool confirmEnabled { get; set; }
        public string confirmCode { get; set; }
        public string clientName { get; set; }
        public ConfigConnect configuration { get; set; }
    }
}

using System;

namespace PluginICAOClientSDK.Models {
    public class RequireInfoDetails {
        public bool mrzEnabled { get; set; }
        public bool imageEnabled { get; set; }
        public bool dataGroupEnabled { get; set; }
        public bool optionalDetailsEnabled { get; set; }
        public string canValue { get; set; }
        public string challenge { get; set; }
        public bool caEnabled { get; set; }
        public bool taEnabled { get; set; }
        public bool paEnabled { get; set; }
    }
}

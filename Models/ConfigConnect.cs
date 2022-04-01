using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Models {
    public class ConfigConnect {
        public bool automaticEnabled { get; set; }
        public bool mrzEnabled { get; set; }
        public bool imageEnabled { get; set; }
        public bool dataGroupEnabled { get; set; }
        public bool optionalDetailsEnabled { get; set; }
    }
}

using PluginICAOClientSDK.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Models {
    public class RequireBiometricAuth {
        public string biometricType { get; set; }
        public string cardNo { get; set; }
        public bool livenessEnabled { get; set; }
        public string challengeType { get; set; }
        public object challenge { get; set; }
    }
}

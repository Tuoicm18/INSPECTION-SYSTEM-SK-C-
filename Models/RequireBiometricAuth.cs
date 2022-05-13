using PluginICAOClientSDK.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Models {
    public class RequireBiometricAuth {
        public string biometricType { get; set; }
        public AuthorizationData authorizationData { get; set; }
    }
}

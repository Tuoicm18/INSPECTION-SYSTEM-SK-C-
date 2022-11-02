using System;

namespace PluginICAOClientSDK.Models {
    public class RequireBiometricAuth {
        public string biometricType { get; set; }

        public string cardNo { get; set; }

        public bool livenessEnabled { get; set; }
        public bool biometricEvidenceEnabled { get; set; }

        public string challengeType { get; set; }

        public object challenge { get; set; }
    }
}

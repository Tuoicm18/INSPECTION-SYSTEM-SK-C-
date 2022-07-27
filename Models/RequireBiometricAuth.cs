using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace PluginICAOClientSDK.Models {
    public class RequireBiometricAuth {
        [JsonConverter(typeof(StringEnumConverter))]
        public BiometricType biometricType { get; set; }

        public string cardNo { get; set; }

        public bool livenessEnabled { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ChallengeType challengeType { get; set; }

        public object challenge { get; set; }
    }
}

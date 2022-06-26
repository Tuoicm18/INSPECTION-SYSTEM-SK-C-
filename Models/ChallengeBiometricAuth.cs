using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Models {
    public class ChallengeBiometricAuth {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string challengeValue { get; set; }
        public TransactionDataBiometricAuth transactionData { get; set; }
    }
}

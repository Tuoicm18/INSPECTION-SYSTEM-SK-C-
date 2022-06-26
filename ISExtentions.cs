using Newtonsoft.Json;
using PluginICAOClientSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK {
    public class ISExtentions {
        //Setting Jsons For KeyValuPair<string, object>
        public static JsonSerializerSettings settingsJsonDuplicateDic = new JsonSerializerSettings { Converters = new[] { new KeyValuePairConverter() } };
        //JsonConverter For List<KeyValuePair<string, object>
        private class KeyValuePairConverter : JsonConverter {
            public override bool CanConvert(Type objectType) {
                return objectType == typeof(List<KeyValuePair<string, object>>);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
                throw new NotImplementedException();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
                List<KeyValuePair<string, object>> list = value as List<KeyValuePair<string, object>>;
                writer.WriteStartObjectAsync();
                foreach (var item in list) {
                    //writer.WriteStartObject();
                    writer.WritePropertyName(item.Key);
                    writer.WriteValue(item.Value);
                    //writer.WriteEndObject();
                }
                writer.WriteEndObjectAsync();
            }
        }

        //Deserialize Authorization Data To Json String For Request Authentication
        public static TransactionDataBiometricAuth deserializeJsonAuthorizationData(string jsonAuthorizationData) {
            try {
                if (jsonAuthorizationData.Equals(string.Empty)) {
                    return null;
                }
                else {
                    TransactionDataBiometricAuth authorizationData = JsonConvert.DeserializeObject<TransactionDataBiometricAuth>(jsonAuthorizationData);
                    return authorizationData;
                }
            } catch (Exception e) {
                throw e;
            }
        }
    }
}

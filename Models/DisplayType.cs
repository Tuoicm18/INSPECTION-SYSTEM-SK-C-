using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Models {
    public enum DisplayType {
        [Description("TEXT")]
        [JsonConverter(typeof(StringEnumConverter))]
        TEXT,
        [JsonConverter(typeof(StringEnumConverter))]
        [Description("HTML")]
        HTML,
    }
}

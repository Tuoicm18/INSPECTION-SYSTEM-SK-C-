using System;
using System.Collections.Generic;
namespace PluginICAOClientSDK.Request {
    public class NVP {
        public string title { get; set; }
        public Dictionary<string, string> listKeyValue { get; set; }
    }
}

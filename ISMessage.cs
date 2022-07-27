using Newtonsoft.Json;
using PluginICAOClientSDK.Models;
using System;

namespace PluginICAOClientSDK {
    public class ISMessage<T> {
        public string cmdType { get; set; }
        public string requestID { get; set; }
        public int timeoutInterval { get; set; }
        public T data { get; set; }
    }
}

using App.Metrics.Concurrency;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK {
    //ISMessage<Option<T>>
    public class ISResponse<T> : ISMessage<T> {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}

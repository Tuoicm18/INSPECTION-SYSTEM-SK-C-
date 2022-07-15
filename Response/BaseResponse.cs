using System;

namespace PluginICAOClientSDK.Response {
    public class BaseResponse {
        public string cmdType { get; set; }
        public string requestID { get; set; }
        public int timeOutInterval { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
    }
}

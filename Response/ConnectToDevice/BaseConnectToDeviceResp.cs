﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginICAOClientSDK.Response.ConnectToDevice {
    public class BaseConnectToDeviceResp {
        public string cmdType { get; set; }
        public string requestID { get; set; }
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public DataConnectToDevice data { get; set; }
    }
}

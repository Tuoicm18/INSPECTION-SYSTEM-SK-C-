using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginICAOClientSDK.Response.ConnectToDevice {
    public class ConnectToDeviceResp : BaseResponse  {
        public DataConnectToDevice data { get; set; }
    }
}

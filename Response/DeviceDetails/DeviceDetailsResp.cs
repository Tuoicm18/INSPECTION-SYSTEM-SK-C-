using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.DeviceDetails {
    public class DeviceDetailsResp : BaseResponse {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DataDeviceDetails data { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Response.CardDetectionEvent {
    public class CardDetectionEventResp : BaseResponse {
        public DataCardDetectionEvent data { get; set; }
    }
}

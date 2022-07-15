using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.GetDocumentDetails {
    public class BaseDocumentDetailsResp : BaseResponse  {
        public DataDocumentDetails data { get; set; }
    }
}

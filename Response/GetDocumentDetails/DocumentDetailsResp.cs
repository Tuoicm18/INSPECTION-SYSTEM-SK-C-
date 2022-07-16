using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.GetDocumentDetails {
    public class DocumentDetailsResp : BaseResponse  {
        public DataDocumentDetails data { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.ScanDocument {
    public class ScanDocumentResp : BaseResponse{
        public DataScanDocument data { get; set; }
    }
}

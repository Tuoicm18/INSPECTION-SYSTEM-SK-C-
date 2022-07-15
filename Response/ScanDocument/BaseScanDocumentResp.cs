using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.ScanDocument {
    public class BaseScanDocumentResp : BaseResponse{
        public DataScanDocument data { get; set; }
    }
}

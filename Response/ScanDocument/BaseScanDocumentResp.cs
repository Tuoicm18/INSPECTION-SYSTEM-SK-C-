using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.ScanDocument {
    public class BaseScanDocumentResp {
        public string cmdType { get; set; }
        public string requestID { get; set; }
        public int timeOutInterval { get; set; }

        public int errorCode { get; set; }
        public string errorMessage { get; set; }

        public DataScanDocument data { get; set; }
    }
}

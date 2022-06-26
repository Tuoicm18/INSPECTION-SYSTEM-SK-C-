﻿using System;
using System.Collections.Generic;

namespace PluginICAOClientSDK.Response.GetDocumentDetails {
    public class BaseDocumentDetailsResp {
        public string cmdType { get; set; }
        public string requestID { get; set; }
        public int timeOutInterval { get; set; }

        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public DataDocumentDetails data { get; set; }
    }
}

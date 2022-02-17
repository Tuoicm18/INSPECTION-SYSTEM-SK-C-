﻿using System;
using System.ComponentModel;

namespace PluginICAOClientSDK.Models {
    public enum CmdType {
        [Description("GetDeviceDetails")]
        GetDeviceDetails,
        [Description("GetInfoDetails")]
        GetInfoDetails,
        [Description("SendInfoDetails")]
        SendInfoDetails,
        [Description("BiometricAuthentication")]
        BiometricAuthentication,
        [Description("ConnectToDevice")]
        ConnectToDevice,
    }
}

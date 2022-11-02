using System;
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
        [Description("DisplayInformation")]
        DisplayInformation,
        [Description("SendBiometricAuthentication")]
        SendBiometricAuthentication,
        [Description("CardDetectionEvent")]
        CardDetectionEvent,
        [Description("Refresh")]
        Refresh,
        [Description("ScanDocument")]
        ScanDocument,
        [Description("BiometricEvidence")]
        BiometricEvidence,
    }
}

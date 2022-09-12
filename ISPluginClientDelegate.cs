using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.CardDetectionEvent;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using System;

namespace PluginICAOClientSDK {
    public delegate void DelegateDefault<T>(T param);
    public delegate void DelegateDefaultSend<T>(T param, T param2, T param3);
    public delegate void DelegateDefaultReceive<T>(T param, T param2, T param3, T param4);
    public class ISPluginClientDelegate {
        public DelegateDefault<bool> dlgConnected;
        public DelegateDefault<bool> dlgDisConnected;
        public DelegateDefault<bool> dlgReConnect;
        public DelegateDefault<bool> dlgConnectDenied;
        public DelegateDefaultSend<object> dlgSend;
        public DelegateDefaultReceive<object> dlgReceive;
        public DelegateDefault<DocumentDetailsResp> dlgReceivedDocument;
        public DelegateDefault<BiometricAuthResp> dlgReceivedBiometricResult;
        public DelegateDefault<CardDetectionEventResp> dlgReceviedCardDetectionEvent;
    }
}

﻿using Newtonsoft.Json;
using PluginICAOClientSDK.Request;
using System;
using WebSocketSharp;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.ConnectToDevice;
using PluginICAOClientSDK.Response.DisplayInformation;

namespace PluginICAOClientSDK {
    public class ISPluginClient {

        #region DELARCE VARIABLE
        private Logger LOGGER = new Logger(LogLevel.Debug);

        private WebSocketClientHandler wsClient;
        private ISListener listener;
        public DelegateAutoDocument delegateAuto;
        public DelegateAutoBiometricResult delegateBiometricResult;
        public DelegateAutoReadNofity delegateAutoReadNofity;
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        ///  The constructor websocket client, init, connect to socket server.
        /// </summary>
        /// <param name="endPointUrl">End point URL Websocket Server</param>
        /// <param name="listener">Listenner for Client Webscoket DeviceDetails, DocumentDetais...etc</param>
        public ISPluginClient(string endPointUrl, bool secureConnect, DelegateAutoDocument delegateAuto, 
                              ISListener listener, DelegateAutoBiometricResult delegateBiometricResult, DelegateAutoReadNofity delegateAutoReadNofity) {
            wsClient = new WebSocketClientHandler(endPointUrl, secureConnect, delegateAuto, listener, delegateBiometricResult, delegateAutoReadNofity);
        }

        public ISPluginClient() { }
        #endregion

        #region INTERFACE INNER 
        public interface DetailsListener {
            void onError(Exception error);
        }

        public interface DeviceDetailsListener : DetailsListener {
            void onReceivedDeviceDetails(Response.DeviceDetails.BaseDeviceDetailsResp device);
        }

        public interface DocumentDetailsListener : DetailsListener {
            void onReceivedDocumentDetails(BaseDocumentDetailsResp document);
        }

        public interface BiometricAuthenticationListener : DetailsListener {
            void onReceviedBiometricAuthenticaton(BaseBiometricAuthResp biometricAuthenticationResp);
        }

        public interface ConnectToDeviceListener : DetailsListener {
            void onReceviedConnectToDevice(BaseConnectToDeviceResp connectToDeviceResp);
        }

        public interface DisplayInformationListener : DetailsListener {
            void onReceviedDisplayInformation(BaseDisplayInformation baseDisplayInformation);
        }

        public interface ISListener {
            bool onReceivedDocument(BaseDocumentDetailsResp document);
            bool onReceivedBiometricResult(BaseBiometricAuthResp baseBiometricAuth);
            void onPreConnect();
            void onConnected();
            void onDisconnected();
            void doSend(string cmd, String id, ISMessage<object> data);
            void onReceive(string cmd, String id, int error, ISMessage<object> data);
        }
        #endregion

        #region SET LISTENER
        //
        // Summary:
        //     Set the listener for websocket client.
        //
        // Remarks:
        //     The set listener is more convenient during operationr.
        public void setListener(ISListener listener) {
            this.listener = listener;
        }
        #endregion

        #region SHUTDOWN CLIENT
        //
        // Summary:
        //     Disconnect from the websocket server.
        //
        // Remarks:
        //     Disconnect completely until a new connection is available to the websocket server.
        //
        // Exception: Unconnected exception occurs, some other exceptions.
        public void shutDown() {
            wsClient.shutdown();
        }
        #endregion

        #region CHECK CONNECT FUNC
        public bool checkConnect() {
            return wsClient.IsConnect;
        }
        #endregion

        #region CHECK CONNECTION DENIED
        public int codeConnectionDenied() {
            return wsClient.CheckConnectionDenied;
        }
        #endregion

        #region GET DEVICE DETAILS FUNC
        //
        // Summary:
        //     The function returns connected device information.
        //
        // Return:
        //     Return device information.
        //
        // Exception: Unconnected exception occurs, some other exceptions.
        public Response.DeviceDetails.BaseDeviceDetailsResp getDeviceDetails(bool deviceDetailsEnabled, bool presenceEnabled, TimeSpan timeoutMilliSec, int timeOutInterVal) {
            return (Response.DeviceDetails.BaseDeviceDetailsResp)getDeviceDetailsAsync(deviceDetailsEnabled, presenceEnabled, null, timeOutInterVal).waitResponse(timeoutMilliSec);
        }

        private ResponseSync<object> getDeviceDetailsAsync(bool deviceDetailsEnabled, bool presenceEnabled, DeviceDetailsListener deviceDetailsListener, int timeOutInterVal) {
            string cmdType = Utils.ToDescription(CmdType.GetDeviceDetails);
            string reqID = Utils.getUUID();
            RequireDeviceDetails requireDeviceDetails = new RequireDeviceDetails();
            requireDeviceDetails.deviceDetailsEnabled = deviceDetailsEnabled;
            requireDeviceDetails.presenceEnabled = presenceEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = Utils.ToDescription(CmdType.GetDeviceDetails);
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterVal;
            req.data = requireDeviceDetails;

            LOGGER.Debug(">>> SEND: [" + JsonConvert.SerializeObject(req) + "]");

            ResponseSync<object> responseSync = new ResponseSync<object>();
            responseSync.cmdType = cmdType;
            responseSync.deviceDetailsListener = deviceDetailsListener;
            responseSync.Wait = new System.Threading.CountdownEvent(1);

            wsClient.request.Add(reqID, responseSync);

            if (this.listener != null) {
                this.listener.doSend(cmdType, reqID, req);
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region GET DOCUMENT DETAILS
        //
        // Summary:
        //     The function returns document informations details MRZ, Image, Data Group..etc
        //
        // Return:
        //     Return document informations details.
        //
        // Exception: Unconnected exception occurs, some other exceptions.
        public BaseDocumentDetailsResp getDocumentDetails(bool mrzEnabled, bool imageEnabled,
                                                          bool dataGroupEnabled, bool optionalDetailsEnabled,
                                                          TimeSpan timeoutMilliSec, DocumentDetailsListener documentDetailsListener,
                                                          int timeOutInterVal, string canValue) {
            return (BaseDocumentDetailsResp)getDocumentDetailsAsync(mrzEnabled, imageEnabled, dataGroupEnabled,
                                                                    optionalDetailsEnabled, documentDetailsListener, 
                                                                    timeOutInterVal, canValue).waitResponse(timeoutMilliSec);
        }
        private ResponseSync<object> getDocumentDetailsAsync(bool mrzEnabled, bool imageEnabled,
                                                             bool dataGroupEnabled, bool optionalDetailsEnabled,
                                                             DocumentDetailsListener documentDetailsListener, int timeOutInterVal,
                                                             string canValue) {
            string cmdType = Utils.ToDescription(CmdType.GetInfoDetails);
            string reqID = Utils.getUUID();
            RequireInfoDetails requireInfoDetails = new RequireInfoDetails();
            requireInfoDetails.mrzEnabled = mrzEnabled;
            requireInfoDetails.imageEnabled = imageEnabled;
            requireInfoDetails.dataGroupEnabled = dataGroupEnabled;
            requireInfoDetails.optionalDetailsEnabled = optionalDetailsEnabled;
            requireInfoDetails.canValue = canValue;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterVal;
            req.data = requireInfoDetails;

            LOGGER.Debug(">>> SEND: [" + JsonConvert.SerializeObject(req) + "]");

            ResponseSync<object> responseSync = new ResponseSync<object>();
            responseSync.cmdType = cmdType;
            responseSync.Wait = new System.Threading.CountdownEvent(1);
            responseSync.documentDetailsListener = documentDetailsListener;

            wsClient.request.Add(reqID, responseSync);

            if (this.listener != null) {
                this.listener.doSend(cmdType, reqID, req);
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region AUTO GET DOCUMENT DETAILS
        //public DocumentDetailsResp autoGetDocumentDetails() {
        //    return wsClient.documentRespAuto;
        //}
        #endregion

        #region BIOMETRIC AUTHENTICATION
        //
        // Summary:
        //     Return result biometric authentication true | false. * NOTE: Finger Type more score.
        //
        // Return:
        //     Return result biometric authentication.
        //
        // Exception: Unconnected exception occurs, some other exceptions.
        public BaseBiometricAuthResp biometricAuthentication(string biometricType, AuthorizationData authorizationData,
                                                             TimeSpan timeoutSec, int timeOutInterVal) {
            return (BaseBiometricAuthResp)biometricAuthenticationAsync(biometricType, authorizationData, null, timeOutInterVal)
                   .waitResponse(timeoutSec);
        }

        private ResponseSync<object> biometricAuthenticationAsync(string biometricType, AuthorizationData authorizationData,
                                                                  ISPluginClient.BiometricAuthenticationListener biometricAuthenticationListener,
                                                                  int timeOut) {
            string cmdType = Utils.ToDescription(CmdType.BiometricAuthentication);
            string reqID = Utils.getUUID();

            RequireBiometricAuth requireBiometricAuth = new RequireBiometricAuth();
            requireBiometricAuth.biometricType = biometricType;
            requireBiometricAuth.authorizationData = authorizationData;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeOutInterval = timeOut;
            req.data = requireBiometricAuth;

            LOGGER.Debug(">>> SEND: [" + JsonConvert.SerializeObject(req) + "]");

            ResponseSync<object> responseSync = new ResponseSync<object>();
            responseSync.cmdType = cmdType;
            responseSync.Wait = new System.Threading.CountdownEvent(1);
            responseSync.biometricAuthenticationListener = biometricAuthenticationListener;

            wsClient.request.Add(reqID, responseSync);

            if (this.listener != null) {
                this.listener.doSend(cmdType, reqID, req);
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region CONNECT TO DEVICE
        public BaseConnectToDeviceResp connectToDevice(bool confirmEnabled, string confirmCode,
                                                   string clientName, ConfigConnect configConnect,
                                                   TimeSpan timeoutMilliSec, int timeOutInterVal) {
            return (BaseConnectToDeviceResp)connectToDeviceSync(confirmEnabled, confirmCode, clientName, configConnect, null, timeOutInterVal).waitResponse(timeoutMilliSec);
        }
        private ResponseSync<object> connectToDeviceSync(bool confirmEnabled, string confirmCode,
                                                         string clientName, ConfigConnect configConnect,
                                                         ConnectToDeviceListener connectToDeviceListener,
                                                         int timeOutInterVal) {
            string cmdType = Utils.ToDescription(CmdType.ConnectToDevice);
            string reqID = Utils.getUUID();

            RequireConnectDevice requireConnectDevice = new RequireConnectDevice();
            requireConnectDevice.confirmEnabled = confirmEnabled;
            requireConnectDevice.confirmCode = confirmCode;
            requireConnectDevice.clientName = clientName;
            requireConnectDevice.configuration = configConnect;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterVal;
            req.data = requireConnectDevice;

            LOGGER.Debug(">>> SEND: [" + JsonConvert.SerializeObject(req) + "]");

            ResponseSync<object> responseSync = new ResponseSync<object>();
            responseSync.cmdType = cmdType;
            responseSync.Wait = new System.Threading.CountdownEvent(1);
            responseSync.connectToDeviceListener = connectToDeviceListener;

            wsClient.request.Add(reqID, responseSync);

            if (this.listener != null) {
                this.listener.doSend(cmdType, reqID, req);
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region DISPLAY INFORMATION
        public BaseDisplayInformation displayInformation(string title, string type, string value,
                                                         TimeSpan timeoutMilliSec, int timeOutInterVal) {
            return (BaseDisplayInformation)displayInformationSync(title, type, value, null, timeOutInterVal).waitResponse(timeoutMilliSec);
        }
        private ResponseSync<object> displayInformationSync(string title, string type, string value,
                                                            DisplayInformationListener displayInformationListener, int timeOutInterVal) {
            string cmdType = Utils.ToDescription(CmdType.DisplayInformation);
            string reqID = Utils.getUUID();

            DataDisplayInformation dataDisplay = new DataDisplayInformation();
            dataDisplay.title = title;
            dataDisplay.type = type;
            dataDisplay.value = value;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterVal;
            req.data = dataDisplay;

            LOGGER.Debug(">>> SEND: [" + JsonConvert.SerializeObject(req) + "]");

            ResponseSync<object> responseSync = new ResponseSync<object>();
            responseSync.cmdType = cmdType;
            responseSync.Wait = new System.Threading.CountdownEvent(1);
            responseSync.displayInformationListener = displayInformationListener;

            wsClient.request.Add(reqID, responseSync);

            if (this.listener != null) {
                this.listener.doSend(cmdType, reqID, req);
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion
    }
}

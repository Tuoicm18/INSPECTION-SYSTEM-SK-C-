using Newtonsoft.Json;
using PluginICAOClientSDK.Request;
using System;
using WebSocketSharp;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.ConnectToDevice;
using PluginICAOClientSDK.Response.DisplayInformation;
using PluginICAOClientSDK.Response.CardDetectionEvent;
using System.Collections.Generic;
using PluginICAOClientSDK.Response.ScanDocument;

namespace PluginICAOClientSDK {
    public class ISPluginClient {

        #region DELARCE VARIABLE
        private Logger LOGGER = new Logger(LogLevel.Debug);

        private WebSocketClientHandler wsClient;
        private ISListener listener;
        public double timeIntervalReconnect;

        #region DELEGATE 
        public DelegateAutoDocument delegateAutoGetDocument;
        public DelegateAutoBiometricResult delegateBiometricResult;
        public DelegateCardDetectionEvent delegateCardEvent;
        public DelegateConnect delegateConnectSocket;
        public DelegateNotifyMessage delegateNotifyMessage;
        #endregion

        #endregion

        #region CONSTRUCTOR
        /// <summary>
        ///  The constructor websocket client, init, connect to socket server.
        /// </summary>
        /// <param name="endPointUrl">End point URL Websocket Server</param>
        /// <param name="listener">Listenner for Client Webscoket DeviceDetails, DocumentDetais...etc</param>
        public ISPluginClient(string ip, int port,
                              bool secureConnect, DelegateAutoDocument delegateAutoGetDocument,
                              DelegateAutoBiometricResult delegateBiometricResult, DelegateCardDetectionEvent delegateCardEvent,
                              DelegateConnect delegateConnectSocket, DelegateNotifyMessage delegateNotifyMessage) {
            wsClient = new WebSocketClientHandler(ip, port,
                                                  secureConnect, delegateAutoGetDocument,
                                                  delegateBiometricResult, delegateCardEvent,
                                                  delegateConnectSocket, delegateNotifyMessage);
        }

        public ISPluginClient(string ip, int port, bool secureConnect, ISListener listener) {
            wsClient = new WebSocketClientHandler(ip, port, secureConnect, listener);
        }
        #endregion

        #region INTERFACE INNER 
        public interface DetailsListener {
            void onError(Exception error);
        }

        public interface DeviceDetailsListener : DetailsListener {
            void onDeviceDetails(Response.DeviceDetails.DeviceDetailsResp device);
        }

        public interface RefreshListenner : DetailsListener {
            void onRefresh(Response.DeviceDetails.DeviceDetailsResp deviceRefresh);
        }

        public interface DocumentDetailsListener : DetailsListener {
            void onDocumentDetails(DocumentDetailsResp document);
        }

        public interface BiometricAuthenticationListener : DetailsListener {
            void onBiometricAuthenticaton(BiometricAuthResp biometricAuthenticationResp);
        }

        public interface ConnectToDeviceListener : DetailsListener {
            void onConnectToDevice(ConnectToDeviceResp connectToDeviceResp);
        }

        public interface DisplayInformationListener : DetailsListener {
            void onDisplayInformation(DisplayInformationResp baseDisplayInformation);
        }

        public interface ScanDocumentListenner : DetailsListener {
            void onScanDocument(ScanDocumentResp baseScanDocumentResp);
        }

        public interface ISListener {
            bool onReceivedDocument(DocumentDetailsResp document);
            bool onReceivedBiometricResult(BiometricAuthResp baseBiometricAuth);
            bool onReceviedCardDetectionEvent(BaseCardDetectionEventResp baseCardDetectionEvent);
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

        #region CONNECT FUNC
        //public bool checkConnect() {
        //    //return wsClient.IsConnect;
        //    return wsClient.IsConnect;
        //}

        public void connectSocketServer() {
            wsClient.wsConnect();
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
        public Response.DeviceDetails.DeviceDetailsResp getDeviceDetails(bool deviceDetailsEnabled, bool presenceEnabled,
                                                                         long timeoutMilliSec, int timeOutInterval) {
            return (Response.DeviceDetails.DeviceDetailsResp)getDeviceDetailsAsync(deviceDetailsEnabled, presenceEnabled, timeOutInterval, null).waitResponse(timeoutMilliSec);
        }

        public ResponseSync<object> getDeviceDetailsAsync(bool deviceDetailsEnabled, bool presenceEnabled,
                                                          int timeOutInterval, DeviceDetailsListener deviceDetailsListener) {
            string cmdType = Utils.ToDescription(CmdType.GetDeviceDetails);
            string reqID = Utils.getUUID();
            RequireDeviceDetails requireDeviceDetails = new RequireDeviceDetails();
            requireDeviceDetails.deviceDetailsEnabled = deviceDetailsEnabled;
            requireDeviceDetails.presenceEnabled = presenceEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = Utils.ToDescription(CmdType.GetDeviceDetails);
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterval;
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
        public DocumentDetailsResp getDocumentDetails(bool mrzEnabled, bool imageEnabled,
                                                      bool dataGroupEnabled, bool optionalDetailsEnabled,
                                                      string canValue, string challenge,
                                                      bool caEnabled, bool taEnabled, 
                                                      long timeoutMilliSec, int timeOutInterval) {

            return (DocumentDetailsResp)getDocumentDetailsAsync(mrzEnabled, imageEnabled,
                                                                dataGroupEnabled, optionalDetailsEnabled,
                                                                canValue, challenge,
                                                                caEnabled, taEnabled,
                                                                timeOutInterval, null).waitResponse(timeoutMilliSec);
        }
        
        public ResponseSync<object> getDocumentDetailsAsync(bool mrzEnabled, bool imageEnabled,
                                                            bool dataGroupEnabled, bool optionalDetailsEnabled,
                                                            string canValue, string challenge,
                                                            bool caEnabled, bool taEnabled,
                                                            int timeOutInterval, DocumentDetailsListener documentDetailsListener) {
            string cmdType = Utils.ToDescription(CmdType.GetInfoDetails);
            string reqID = Utils.getUUID();
            RequireInfoDetails requireInfoDetails = new RequireInfoDetails();
            requireInfoDetails.mrzEnabled = mrzEnabled;
            requireInfoDetails.imageEnabled = imageEnabled;
            requireInfoDetails.dataGroupEnabled = dataGroupEnabled;
            requireInfoDetails.optionalDetailsEnabled = optionalDetailsEnabled;
            requireInfoDetails.canValue = canValue;
            requireInfoDetails.challenge = challenge;
            requireInfoDetails.caEnabled = caEnabled;
            requireInfoDetails.taEnabled = taEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterval;
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

        #region BIOMETRIC AUTHENTICATION
        //
        // Summary:
        //     Return result biometric authentication true | false. * NOTE: Finger Type more score.
        //
        // Return:
        //     Return result biometric authentication.
        //
        // Exception: Unconnected exception occurs, some other exceptions.
        public BiometricAuthResp biometricAuthentication(BiometricType biometricType, object challengeBiometric,
                                                         ChallengeType challengeType, bool livenessEnabled, string cardNo,
                                                         long timeoutMilliSec, int timeOutInterval) {
            return (BiometricAuthResp)biometricAuthenticationAsync(biometricType, challengeBiometric,
                                                                   challengeType, livenessEnabled, cardNo,
                                                                   timeOutInterval, null).waitResponse(timeoutMilliSec);
        }

        public ResponseSync<object> biometricAuthenticationAsync(BiometricType biometricType, object challengeBiometric,
                                                                 ChallengeType challengeType, bool livenessEnabled, string cardNo,
                                                                 int timeOutInterval, BiometricAuthenticationListener biometricAuthenticationListener) {
            string cmdType = Utils.ToDescription(CmdType.BiometricAuthentication);
            string reqID = Utils.getUUID();

            RequireBiometricAuth requireBiometricAuth = new RequireBiometricAuth();
            requireBiometricAuth.biometricType = Utils.ToDescription(biometricType);
            requireBiometricAuth.cardNo = cardNo;
            requireBiometricAuth.challengeType = Utils.ToDescription(challengeType);
            requireBiometricAuth.challenge = challengeBiometric;
            requireBiometricAuth.livenessEnabled = livenessEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterval;
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
        public ConnectToDeviceResp connectToDevice(bool confirmEnabled, string confirmCode,
                                                   string clientName, ConfigConnect configConnect,
                                                   long timeoutMilliSec, int timeOutInterval) {
            return (ConnectToDeviceResp)connectToDeviceSync(confirmEnabled, confirmCode, clientName, configConnect, timeOutInterval, null).waitResponse(timeoutMilliSec);
        }
        public ResponseSync<object> connectToDeviceSync(bool confirmEnabled, string confirmCode,
                                                        string clientName, ConfigConnect configConnect,
                                                        int timeOutInterval, ConnectToDeviceListener connectToDeviceListener) {
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
            req.timeOutInterval = timeOutInterval;
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
        public DisplayInformationResp displayInformation(string title, DisplayType type,
                                                         string value, int timeOutInterval,
                                                         long timeoutMilliSec) {
            return (DisplayInformationResp)displayInformationSync(title, type, value, timeOutInterval, null).waitResponse(timeoutMilliSec);
        }
        public ResponseSync<object> displayInformationSync(string title, DisplayType type,
                                                           string value, int timeOutInterval,
                                                           DisplayInformationListener displayInformationListener) {
            string cmdType = Utils.ToDescription(CmdType.DisplayInformation);
            string reqID = Utils.getUUID();

            DataDisplayInformation dataDisplay = new DataDisplayInformation();
            dataDisplay.title = title;
            dataDisplay.type = Utils.ToDescription(type);
            dataDisplay.value = value;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterval;
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

        #region REFRESH READER
        public Response.DeviceDetails.DeviceDetailsResp refreshReader(bool deviceDetailsEnabled, bool presenceEnabled,
                                                                      long timeoutMilliSec, int timeOutInterval) {
            return (Response.DeviceDetails.DeviceDetailsResp)refreshReaderAsync(deviceDetailsEnabled, presenceEnabled, timeOutInterval, null).waitResponse(timeoutMilliSec);
        }

        public ResponseSync<object> refreshReaderAsync(bool deviceDetailsEnabled, bool presenceEnabled, 
                                                       int timeOutInterval, DeviceDetailsListener deviceDetailsListener) {
            string cmdType = Utils.ToDescription(CmdType.Refresh);
            string reqID = Utils.getUUID();
            RequireDeviceDetails requireDeviceDetails = new RequireDeviceDetails();
            requireDeviceDetails.deviceDetailsEnabled = deviceDetailsEnabled;
            requireDeviceDetails.presenceEnabled = presenceEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = Utils.ToDescription(CmdType.Refresh);
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterval;
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

        #region SCAN DOCUMENT
        public ScanDocumentResp scanDocument(ScanType scanType, bool saveEnabled,
                                             long timeoutMilliSec, int timeOutInterval) {
            return (ScanDocumentResp)scanDocumentAsync(scanType, saveEnabled, timeOutInterval, null).waitResponse(timeoutMilliSec);
        }
        public ResponseSync<object> scanDocumentAsync(ScanType scanType, bool saveEnabled,
                                                      int timeOutInterval, ScanDocumentListenner scanDocumentListenner) {
            string cmdType = Utils.ToDescription(CmdType.ScanDocument);
            string reqID = Utils.getUUID();
            ScanDocument scanDocument = new ScanDocument();
            scanDocument.scanType = Utils.ToDescription(scanType);
            scanDocument.saveEnabled = saveEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = Utils.ToDescription(CmdType.ScanDocument);
            req.requestID = reqID;
            req.timeOutInterval = timeOutInterval;
            req.data = scanDocument;

            LOGGER.Debug(">>> SEND: [" + JsonConvert.SerializeObject(req) + "]");

            ResponseSync<object> responseSync = new ResponseSync<object>();
            responseSync.cmdType = cmdType;
            responseSync.scanDocumentListenner = scanDocumentListenner;
            responseSync.Wait = new System.Threading.CountdownEvent(1);

            wsClient.request.Add(reqID, responseSync);

            if (this.listener != null) {
                this.listener.doSend(cmdType, reqID, req);
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region FOR TEST
        public void reConnectSocket(int interval, int totalOfTimes) {
            try {
                wsClient.reconnectSocket(interval, totalOfTimes);
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        #endregion
    }
}

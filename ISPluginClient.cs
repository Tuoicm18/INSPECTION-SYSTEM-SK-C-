using Newtonsoft.Json;
using System;
using WebSocketSharp;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.ConnectToDevice;
using PluginICAOClientSDK.Response.DisplayInformation;
using PluginICAOClientSDK.Response.CardDetectionEvent;
using PluginICAOClientSDK.Response.ScanDocument;
using PluginICAOClientSDK.Response.BiometricEvidence;

namespace PluginICAOClientSDK {
    public class ISPluginClient {

        #region VARIABLE
        private Logger LOGGER = new Logger(LogLevel.Debug);

        private WebSocketClientHandler wsClient;
        private ISListener listener;
        private ISPluginClientDelegate pluginClientDelegate;

        #region DELEGATE 
        public DelegateAutoDocument delegateAutoGetDocument;
        public DelegateAutoBiometricResult delegateBiometricResult;
        public DelegateCardDetectionEvent delegateCardEvent;
        public DelegateConnect delegateConnectSocket;
        public DelegateReceive delegateReceive;
        #endregion

        #endregion

        #region CONSTRUCTOR
        /// <summary>
        ///  The constructor websocket client, init, connect to socket server.
        /// </summary>
        /// <param name="endPointUrl">End point URL Websocket Server</param>
        /// <param name="listener">Listenner for Client Webscoket DeviceDetails, DocumentDetais...etc</param>
        private ISPluginClient(string ip, int port,
                              bool isSecure, DelegateAutoDocument delegateAutoGetDocument,
                              DelegateAutoBiometricResult delegateBiometricResult, DelegateCardDetectionEvent delegateCardEvent,
                              DelegateConnect delegateConnectSocket, DelegateReceive delegateReceive) {
            wsClient = new WebSocketClientHandler(ip, port,
                                                  isSecure, delegateAutoGetDocument,
                                                  delegateBiometricResult, delegateCardEvent,
                                                  delegateConnectSocket, delegateReceive);
        }

        public ISPluginClient(string ip, int port, bool isSecure, ISListener listener) {
            wsClient = new WebSocketClientHandler(ip, port, isSecure, listener);
            this.listener = listener;
        }

        public ISPluginClient(string ip, int port, bool isSecure, ISPluginClientDelegate pluginClientDelegate) {
            wsClient = new WebSocketClientHandler(ip, port, isSecure, pluginClientDelegate);
            this.pluginClientDelegate = pluginClientDelegate;
        }
        #endregion

        #region INTERFACE INNER 
        public interface DetailsListener {
            void onError(Exception error);
        }

        public interface DeviceDetailsListener : DetailsListener {
            void onDeviceDetails(Response.DeviceDetails.DeviceDetailsResp device);
        }

        public interface DocumentDetailsListener : DetailsListener {
            void onDocumentDetails(DocumentDetailsResp document);
        }

        public interface BiometricAuthListener : DetailsListener {
            void onBiometricAuthenticaton(BiometricAuthResp biometricAuth);
        }

        public interface ConnectToDeviceListener : DetailsListener {
            void onConnectToDevice(ConnectToDeviceResp device);
        }

        public interface DisplayInformationListener : DetailsListener {
            void onDisplayInformation(DisplayInformationResp resultDisplayInformation);
            void onSuccess();
        }

        public interface ScanDocumentListenner : DetailsListener {
            void onScanDocument(ScanDocumentResp resultScanDocument);
        }

        public interface BiometricEvidenceListenner : DetailsListener {
            void onBiometricEvidence(BiometricEvidenceResp biometricEvidenceResp);
        }

        public interface ISListener {
            bool onReceivedDocument(DocumentDetailsResp document);
            bool onReceivedBiometricResult(BiometricAuthResp resultBiometricAuth);
            bool onReceviedCardDetectionEvent(CardDetectionEventResp resultCardDetectionEvent);
            void onPreConnect();
            void onConnected();
            void onDisconnected();
            void onConnectDenied();
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

        #region SET DELEGATE
        public void setDelegate(ISPluginClientDelegate pluginClientDelegate) {
            this.pluginClientDelegate = pluginClientDelegate;
        }
        #endregion

        #region SET TIMEOUT INTERVAL RE-CONNECT
        public void setTimeIntervalReConnect(double timeInterval) {
            wsClient.timeIntervalReConnect = timeInterval;
        }
        #endregion

        #region SET TIMEOUT INTERVAL RE-CONNECT
        public void setTotalOfTimesReConnect(int totalReConnect) {
            wsClient.totalOfTimesReConnect = totalReConnect;
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
        public void shutdown() {
            wsClient.shutdown();
        }
        #endregion

        #region CONNECT FUNC
        //public bool checkConnect() {
        //    //return wsClient.IsConnect;
        //    return wsClient.IsConnect;
        //}

        public void connect() {
            try {
                wsClient.wsConnect();
            }
            catch (Exception ex) {
                LOGGER.Error(ex.ToString());
            }
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
        public Response.DeviceDetails.DeviceDetailsResp getDeviceDetails(bool deviceDetailsEnabled, bool presenceEnabled, int timeoutInterval) {
            return (Response.DeviceDetails.DeviceDetailsResp)getDeviceDetailsAsync(deviceDetailsEnabled, presenceEnabled, timeoutInterval, null).waitResponse(timeoutInterval);
        }

        public ResponseSync<object> getDeviceDetailsAsync(bool deviceDetailsEnabled, bool presenceEnabled,
                                                          int timeoutInterval, DeviceDetailsListener deviceDetailsListener) {
            string cmdType = Utils.ToDescription(CmdType.GetDeviceDetails);
            string reqID = Utils.getUUID();
            RequireDeviceDetails requireDeviceDetails = new RequireDeviceDetails();
            requireDeviceDetails.deviceDetailsEnabled = deviceDetailsEnabled;
            requireDeviceDetails.presenceEnabled = presenceEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = Utils.ToDescription(CmdType.GetDeviceDetails);
            req.requestID = reqID;
            req.timeoutInterval = timeoutInterval;
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

            if(null != this.pluginClientDelegate) {
                if(null != this.pluginClientDelegate.dlgSend) {
                    pluginClientDelegate.dlgSend(cmdType, reqID, req);
                }
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
                                                      bool paEnabled, int timeoutInterval) {

            return (DocumentDetailsResp)getDocumentDetailsAsync(mrzEnabled, imageEnabled,
                                                                dataGroupEnabled, optionalDetailsEnabled,
                                                                canValue, challenge,
                                                                caEnabled, taEnabled,
                                                                paEnabled, timeoutInterval, null).waitResponse(timeoutInterval);
        }

        public ResponseSync<object> getDocumentDetailsAsync(bool mrzEnabled, bool imageEnabled,
                                                            bool dataGroupEnabled, bool optionalDetailsEnabled,
                                                            string canValue, string challenge,
                                                            bool caEnabled, bool taEnabled,
                                                            bool paEnabled, int timeoutInterval,
                                                            DocumentDetailsListener documentDetailsListener) {
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
            requireInfoDetails.paEnabled = paEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeoutInterval = timeoutInterval;
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

            if (null != this.pluginClientDelegate) {
                if (null != this.pluginClientDelegate.dlgSend) {
                    pluginClientDelegate.dlgSend(cmdType, reqID, req);
                }
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
                                                         ChallengeType challengeType, bool livenessEnabled,
                                                         string cardNo, int timeoutInterval,
                                                         bool biometricEvidenceEnabled) {
            return (BiometricAuthResp)biometricAuthenticationAsync(biometricType, challengeBiometric,
                                                                   challengeType, livenessEnabled,
                                                                   cardNo, timeoutInterval,
                                                                   biometricEvidenceEnabled, null).waitResponse(timeoutInterval);
        }

        public ResponseSync<object> biometricAuthenticationAsync(BiometricType biometricType, object challengeBiometric,
                                                                 ChallengeType challengeType, bool livenessEnabled,
                                                                 string cardNo, int timeoutInterval,
                                                                 bool biometricEvidenceEnabled, BiometricAuthListener biometricAuthListener) {
            string cmdType = Utils.ToDescription(CmdType.BiometricAuthentication);
            string reqID = Utils.getUUID();

            RequireBiometricAuth requireBiometricAuth = new RequireBiometricAuth();
            requireBiometricAuth.biometricType = Utils.ToDescription(biometricType);
            requireBiometricAuth.cardNo = cardNo;
            requireBiometricAuth.challengeType = Utils.ToDescription(challengeType);
            requireBiometricAuth.challenge = challengeBiometric;
            requireBiometricAuth.livenessEnabled = livenessEnabled;
            requireBiometricAuth.biometricEvidenceEnabled = biometricEvidenceEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = cmdType;
            req.requestID = reqID;
            req.timeoutInterval = timeoutInterval;
            req.data = requireBiometricAuth;

            LOGGER.Debug(">>> SEND: [" + JsonConvert.SerializeObject(req) + "]");

            ResponseSync<object> responseSync = new ResponseSync<object>();
            responseSync.cmdType = cmdType;
            responseSync.Wait = new System.Threading.CountdownEvent(1);
            responseSync.biometricAuthenticationListener = biometricAuthListener;

            wsClient.request.Add(reqID, responseSync);

            if (this.listener != null) {
                this.listener.doSend(cmdType, reqID, req);
            }

            if (null != this.pluginClientDelegate) {
                if (null != this.pluginClientDelegate.dlgSend) {
                    pluginClientDelegate.dlgSend(cmdType, reqID, req);
                }
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region CONNECT TO DEVICE
        public ConnectToDeviceResp connectToDevice(bool confirmEnabled, string confirmCode,
                                                   string clientName, ConfigConnect configConnect,
                                                   int timeoutInterval) {
            return (ConnectToDeviceResp)connectToDeviceSync(confirmEnabled, confirmCode, clientName, configConnect, timeoutInterval, null).waitResponse(timeoutInterval);
        }
        public ResponseSync<object> connectToDeviceSync(bool confirmEnabled, string confirmCode,
                                                        string clientName, ConfigConnect configConnect,
                                                        int timeoutInterval, ConnectToDeviceListener connectToDeviceListener) {
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
            req.timeoutInterval = timeoutInterval;
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

            if (null != this.pluginClientDelegate) {
                if (null != this.pluginClientDelegate.dlgSend) {
                    pluginClientDelegate.dlgSend(cmdType, reqID, req);
                }
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region DISPLAY INFORMATION
        public DisplayInformationResp displayInformation(string title, DisplayType type,
                                                         string value, int timeoutInterval) {
            return (DisplayInformationResp)displayInformationSync(title, type, value, timeoutInterval, null).waitResponse(timeoutInterval);
        }
        public ResponseSync<object> displayInformationSync(string title, DisplayType type,
                                                           string value, int timeoutInterval,
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
            req.timeoutInterval = timeoutInterval;
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

            if (null != this.pluginClientDelegate) {
                if (null != this.pluginClientDelegate.dlgSend) {
                    pluginClientDelegate.dlgSend(cmdType, reqID, req);
                }
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region REFRESH READER
        public Response.DeviceDetails.DeviceDetailsResp refreshReader(bool deviceDetailsEnabled, bool presenceEnabled, int timeoutInterval) {
            return (Response.DeviceDetails.DeviceDetailsResp)refreshReaderAsync(deviceDetailsEnabled, presenceEnabled, timeoutInterval, null).waitResponse(timeoutInterval);
        }

        public ResponseSync<object> refreshReaderAsync(bool deviceDetailsEnabled, bool presenceEnabled,
                                                       int timeoutInterval, DeviceDetailsListener deviceDetailsListener) {
            string cmdType = Utils.ToDescription(CmdType.Refresh);
            string reqID = Utils.getUUID();
            RequireDeviceDetails requireDeviceDetails = new RequireDeviceDetails();
            requireDeviceDetails.deviceDetailsEnabled = deviceDetailsEnabled;
            requireDeviceDetails.presenceEnabled = presenceEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = Utils.ToDescription(CmdType.Refresh);
            req.requestID = reqID;
            req.timeoutInterval = timeoutInterval;
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

            if (null != this.pluginClientDelegate) {
                if (null != this.pluginClientDelegate.dlgSend) {
                    pluginClientDelegate.dlgSend(cmdType, reqID, req);
                }
            }
            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region SCAN DOCUMENT
        public ScanDocumentResp scanDocument(ScanType scanType, bool saveEnabled, int timeoutInterval) {
            return (ScanDocumentResp)scanDocumentAsync(scanType, saveEnabled, timeoutInterval, null).waitResponse(timeoutInterval);
        }
        public ResponseSync<object> scanDocumentAsync(ScanType scanType, bool saveEnabled,
                                                      int timeoutInterval, ScanDocumentListenner scanDocumentListenner) {
            string cmdType = Utils.ToDescription(CmdType.ScanDocument);
            string reqID = Utils.getUUID();
            ScanDocument scanDocument = new ScanDocument();
            scanDocument.scanType = Utils.ToDescription(scanType);
            scanDocument.saveEnabled = saveEnabled;

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = Utils.ToDescription(CmdType.ScanDocument);
            req.requestID = reqID;
            req.timeoutInterval = timeoutInterval;
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

            if (null != this.pluginClientDelegate) {
                if (null != this.pluginClientDelegate.dlgSend) {
                    pluginClientDelegate.dlgSend(cmdType, reqID, req);
                }
            }

            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region BIOMETRIC EVIDENCE
        public BiometricEvidenceResp biometricEvidence(BiometricType biometricType, int timeoutInterval) {
            return (BiometricEvidenceResp)biometricEvidenceAsync(biometricType, timeoutInterval, null).waitResponse(timeoutInterval);
        }

        public ResponseSync<object> biometricEvidenceAsync(BiometricType biometricType, int timeoutInterval, BiometricEvidenceListenner biometricEvidenceListenner) {
            string cmdType = Utils.ToDescription(CmdType.BiometricEvidence);
            string reqID = Utils.getUUID();
            RequireBiometricEvidence requireBiometricEvidence = new RequireBiometricEvidence();
            requireBiometricEvidence.biometricType = Utils.ToDescription(biometricType);

            ISRequest<object> req = new ISRequest<object>();
            req.cmdType = Utils.ToDescription(CmdType.BiometricEvidence);
            req.requestID = reqID;
            req.timeoutInterval = timeoutInterval;
            req.data = requireBiometricEvidence;

            LOGGER.Debug(">>> SEND: [" + JsonConvert.SerializeObject(req) + "]");

            ResponseSync<object> responseSync = new ResponseSync<object>();
            responseSync.cmdType = cmdType;
            responseSync.biometricEvidenceListenner = biometricEvidenceListenner;
            responseSync.Wait = new System.Threading.CountdownEvent(1);

            wsClient.request.Add(reqID, responseSync);

            if (this.listener != null) {
                this.listener.doSend(cmdType, reqID, req);
            }

            if (null != this.pluginClientDelegate) {
                if (null != this.pluginClientDelegate.dlgSend) {
                    pluginClientDelegate.dlgSend(cmdType, reqID, req);
                }
            }

            wsClient.sendData(JsonConvert.SerializeObject(req));
            return responseSync;
        }
        #endregion

        #region FOR TEST
        public void reConnectSocket() {
            try {
                wsClient.reconnectSocket();
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        #endregion
    }
}

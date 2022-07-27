using Newtonsoft.Json;
using PluginICAOClientSDK.Response;
using PluginICAOClientSDK.Models;
using PluginICAOClientSDK.Response.DeviceDetails;
using PluginICAOClientSDK.Response.GetDocumentDetails;
using PluginICAOClientSDK.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.ConnectToDevice;
using PluginICAOClientSDK.Response.DisplayInformation;
using PluginICAOClientSDK.Response.CardDetectionEvent;
using WebSocketSharp;
using PluginICAOClientSDK.Response.ScanDocument;

namespace PluginICAOClientSDK {

    #region DELEGATE
    public delegate void DelegateAutoDocument(DocumentDetailsResp documentDetailsResp);
    public delegate void DelegateAutoBiometricResult(BiometricAuthResp biometricAuthResp);
    public delegate void DelegateCardDetectionEvent(CardDetectionEventResp cardDetectionEventResp);
    public delegate void DelegateConnect(bool isConnect);
    public delegate void DelegateReceive(string json);
    #endregion

    public class WebSocketClientHandler {

        #region VARIABLE
        private static readonly Logger LOGGER = new Logger(LogLevel.Debug);

        #region CONST
        private const string FUNC_GET_DEVICE_DETAILS = "GetDeviceDetails";
        private const string FUNC_GET_INFO_DETAILS = "GetInfoDetails";
        private const string FUNC_BIOMETRIC_AUTH = "BiometricAuthentication";
        private const string FUNC_CONNECT_DEVICE = "ConnectToDevice";
        private const string FUNC_DISPLAY_INFO = "DisplayInformation";
        private const string FUNC_REFRESH = "Refresh";
        private const string FUNC_SCAN_DOCUMENT = "ScanDocument";
        #endregion

        #region DELEGATE 
        private DelegateAutoDocument delegateAuto;
        private DelegateAutoBiometricResult delegatebiometricResult;
        private DelegateCardDetectionEvent delegateCardEvent;
        public DelegateConnect delegateConnect;
        public DelegateReceive delegateReceive;
        #endregion

        private StringBuilder response;
        private readonly ISPluginClient.ISListener listener;
        private readonly int MAX_PING = 15;
        private double timeIntervalReconnect = 10;
        public readonly Dictionary<string, ResponseSync<object>> request = new Dictionary<string, ResponseSync<object>>();

        private Timer timeoutTimer;
        private readonly object timeoutTimerLock = new object();
        private WebSocket ws;
        private bool isShutdown { get; set; }
        private bool isConnect;
        public bool IsConnect {
            get { return isConnect; }
        }

        //Test Timer Connect Socket
        private System.Timers.Timer timerConnect = new System.Timers.Timer();
        private object locker = new object();
        private System.Threading.ManualResetEvent timerDead = new System.Threading.ManualResetEvent(false);
        private int countReConnect = 0;

        #endregion

        #region CONSTRUCTOR
        public WebSocketClientHandler(string ip, int port,
                                      bool secureConnect, DelegateAutoDocument dlgAutoGetDocument,
                                      DelegateAutoBiometricResult delegateAutoBiometric, DelegateCardDetectionEvent dlgCardEvent,
                                      DelegateConnect delegateConnect, DelegateReceive delegateReceive) {

            try {
                if (secureConnect) {
                    string endPointUrlWSS = "wss://" + ip + ":" + port + "/ISPlugin";
                    ws = new WebSocket(endPointUrlWSS);
                    ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                }
                else {
                    string endPointUrlWS = "ws://" + ip + ":" + port + "/ISPlugin";
                    ws = new WebSocket(endPointUrlWS);
                }
                this.delegateAuto = dlgAutoGetDocument;
                this.delegatebiometricResult = delegateAutoBiometric;
                this.delegateCardEvent = dlgCardEvent;
                this.delegateConnect = delegateConnect;
                this.delegateReceive = delegateReceive;
                SetWebSocketSharpEvents();
            }
            catch (Exception e) {
                throw e;
            }
        }

        public WebSocketClientHandler(string ip, int port, bool secureConnect, ISPluginClient.ISListener listener) {
            try {
                if (secureConnect) {
                    string endPointUrlWSS = "wss://" + ip + ":" + port + "/ISPlugin";
                    ws = new WebSocket(endPointUrlWSS);
                    ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                }
                else {
                    string endPointUrlWS = "ws://" + ip + ":" + port + "/ISPlugin";
                    ws = new WebSocket(endPointUrlWS);
                }
                this.listener = listener;
                SetWebSocketSharpEvents();
            }
            catch (Exception e) {
                throw e;
            }
        }

        #endregion

        #region TIMER RE-CONECT
        //private void ResetTimeoutTimer() {
        //    // if you are sure you will never access this from multiple threads at the same time - remove lock
        //    lock (timeoutTimerLock) {
        //        // initialize or reset the timer to fire once, after 10 seconds
        //        if (timeoutTimer == null)
        //            //timeoutTimer = new System.Threading.Timer(ReconnectAfterTimeout, null, TimeSpan.FromSeconds(this.timeIntervalReconnect), Timeout.InfiniteTimeSpan);
        //            timeoutTimer = new Timer(ReconnectAfterTimeout, null, TimeSpan.FromSeconds(2), Timeout.InfiniteTimeSpan);
        //        else {
        //            timeoutTimer.Change(TimeSpan.FromSeconds(2), Timeout.InfiniteTimeSpan);
        //        }

        //    }
        //}

        //private void StopTimeoutTimer() {
        //    // if you are sure you will never access this from multiple threads at the same time - remove lock
        //    lock (timeoutTimerLock) {
        //        if (timeoutTimer != null)
        //            timeoutTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        //    }
        //}
        #endregion

        #region RE-CONECT HANDLE
        private void ReconnectAfterTimeout(object state) {
            // reconnect here
            wsConnect();
            LOGGER.Debug("RE-CONNECT");
        }

        public void reconnectSocket(int interval, int totalOfTimes) {
            try {
                timerDead.Reset();
                timerConnect.Interval = interval;
                timerConnect.Elapsed += Timer_Elapsed;
                timerConnect.Start();

                if (this.countReConnect == totalOfTimes) {
                    StopTimer();
                }
            }
            catch (Exception ex) {
                LOGGER.Error(ex.ToString());
                StopTimer();
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            lock (locker) {
                if (timerDead.WaitOne(0)) return;
                // etc...
                countReConnect++;
                wsConnect();
            }
        }

        private void StopTimer() {
            lock (locker) {
                timerDead.Set();
                timerConnect.Stop();
                countReConnect = 0;
                ws.Close();
            }
        }

        #endregion

        #region WEBSOCKET EVENTS
        public void SetWebSocketSharpEvents() {
            try {
                wsOnOpenHandle();
                wsOnMessageHandle();
                wsOnErrorHandle();
                wsOnCloseHandle();
                wsConnect();
            }
            catch (Exception e) {
                throw e;
            }
        }
        #endregion

        #region CONNECT HANDLE
        public void wsConnect() {
            ws.Connect();
        }
        #endregion

        #region OPEN HANDLE
        private void wsOnOpenHandle() {
            try {
                ws.OnOpen += (sender, e) => {
                    try {
                        //ResetTimeoutTimer();
                        isConnect = true;
                        if(null != this.delegateConnect) {
                            delegateConnect(true);
                        }
                        timerDead.Set();
                        this.countReConnect = 0;

                        LOGGER.Debug("CONNECT SUCCESSFULLY");
                        if (listener != null) {
                            listener.onConnected();
                        }
                    }
                    catch (WebSocketException eConnect) {
                        LOGGER.Debug("WEBSOCKET CLIETN FAILED TO CONNECT " + eConnect.ToString());
                    }
                    return;
                };
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        #endregion

        #region MESSAGE HANDLE
        private void wsOnMessageHandle() {
            try {
                ws.OnMessage += (sender, e) => {
                    //ResetTimeoutTimer();
                    if(null != this.delegateReceive) {
                        this.delegateReceive(e.Data);
                    }
                    BaseResponse baseResponse = JsonConvert.DeserializeObject<BaseResponse>(e.Data);
                    if(null != baseResponse) {
                        if(null != listener) {
                            if(baseResponse.errorCode == Utils.ERR_FOR_DENIED_CONNECTION) {
                                listener.onConnectDenied();
                            }
                        }
                    }
                    ws.EmitOnPing = true;
                    if (!ws.Ping()) {
                        //!ws.Ping()
                        //Try Ping RE-CONNECT
                        this.isConnect = false;
                        //delegateConnect(false);

                        int countPong = 0;
                        int countPing = 0;
                        System.Timers.Timer timerPingPong = new System.Timers.Timer();
                        timerPingPong.Interval = this.timeIntervalReconnect * 1000;
                        timerPingPong.Elapsed += (senderPingPong, ePingPong) => {
                            countPing++;
                            LOGGER.Debug("PING");
                            if (ws.Ping()) {
                                isConnect = true;
                                //delegateConnect(true);

                                countPong++;
                                LOGGER.Debug("PONG++");
                            }
                            else {
                                countPong--;
                                LOGGER.Debug("PONG--");
                            }
                            if (countPing == MAX_PING) {
                                isConnect = true;
                                //delegateConnect(true);

                                LOGGER.Debug("PING = MAX PING");
                                timerPingPong.Stop();
                                if (countPong < MAX_PING) {
                                    isConnect = false;
                                    //delegateConnect(false);

                                    LOGGER.Debug("PONG < PING");
                                    ws.Close();
                                    this.isShutdown = true;
                                }
                                else {
                                    isConnect = true;
                                    //delegateConnect(true);

                                    LOGGER.Debug("PONG == PING");
                                    if (e.IsText) {
                                        if (!e.Data.Equals(string.Empty)) {
                                            response = new StringBuilder();
                                            response.Append(e.Data);
                                            processResponse(response.ToString());
                                            //LOGGER.Debug("DATA RECIVED [TRY-PING-PONG] " + response.ToString());
                                        }
                                        else {
                                            LOGGER.Debug("DATA RECIVED TRY-PING-PONG] IS NULL");
                                        }
                                    }
                                }
                            }
                        };
                        timerPingPong.Start();
                    }
                    else {
                        isConnect = true;
                        //delegateConnect(true);

                        if (e.IsText) {
                            if (!e.Data.Equals(string.Empty)) {
                                response = new StringBuilder();
                                response.Append(e.Data);
                                processResponse(response.ToString());
                                LOGGER.Debug("DATA RECIVED [DEFAULT] " + response.ToString());
                            }
                            else {
                                LOGGER.Debug("DATA RECIVED [DEFAULT] IS NULL");
                            }
                        }
                    }
                };
            }
            catch (Exception eMsg) {
                LOGGER.Debug("ON MESSAGE ERROR " + eMsg.ToString());
            }
        }
        #endregion

        #region ERROR HANDLE
        private void wsOnErrorHandle() {
            try {
                ws.OnError += (sender, e) => {
                    // stop it here
                    //StopTimeoutTimer();
                    LOGGER.Debug("SOCKET CLIENT ERROR MESSAGE " + e.Message);
                    //An exception has occurred during an OnMessage event. An error has occurred in closing the connection.
                    if (!ws.IsAlive) {
                        this.isShutdown = true;
                        ws.Close();
                    }
                    else {
                        ws.EmitOnPing = true;
                        if (ws.Ping()) {
                            //ResetTimeoutTimer();
                        }
                    }
                };
            }
            catch (Exception ex) {
                throw ex;
            }
        }
        #endregion

        #region CLOSE HANDLE
        private void wsOnCloseHandle() {
            ws.OnClose += (sender, e) => {
                // and here
                this.isConnect = false;
                if(null != this.delegateConnect) {
                    delegateConnect(false);
                }

                if (listener != null) {
                    listener.onDisconnected();
                }

                if (this.isShutdown) {
                    //StopTimeoutTimer();
                }
                else {
                    //ResetTimeoutTimer();
                }
            };
        }
        #endregion

        #region SEND DATA
        public void sendData(string json) {
            try {
                ws.Send(json);
            }
            catch (Exception ex) {
                LOGGER.Debug(ex.ToString());
            }
        }
        #endregion

        #region SHUTDOWN CLIENT
        public void shutdown() {
            try {
                this.isShutdown = true;
                this.ws.Close();
                StopTimer();
                LOGGER.Debug("SOCKET CLIENT SHUTDOWN");
                //StopTimeoutTimer();
            }
            catch (Exception e) {
                //StopTimeoutTimer();
                LOGGER.Debug("SOCKET CLIENT SHUTDOWN ERROR " + e.ToString());
            }
        }
        #endregion

        #region PROCESS RESPONSE
        private void processResponse(string json) {
            try {
                ISResponse<object> resp = JsonConvert.DeserializeObject<ISResponse<object>>(json);
                string reqID = resp.requestID;
                LOGGER.Debug("<<< REC: RequestID [" + reqID + "]" + " CmdType [" + resp.cmdType + "] " + "Error [" + resp.errorCode + "]");
                if (listener != null) {
                    this.listener.onReceive(resp.cmdType, reqID, resp.errorCode, resp);
                }

                if (request.ContainsKey(reqID)) {
                    ResponseSync<object> sync = request[reqID];
                    try {
                        if (!Enum.IsDefined(typeof(CmdType), resp.cmdType) || !resp.cmdType.Equals(sync.cmdType)) {
                            throw new ISPluginException("CmdType not match expect [" + sync.cmdType + "] but get [" + resp.cmdType + "]");
                        }

                        if (resp.errorCode != Utils.SUCCESS && resp.errorCode != Utils.ERR_FOR_DENIED_AUTH) {
                            //throw new ISPluginException(resp.errorMessage + ", Error Code [" + resp.errorCode + "]");
                            throw new ISPluginException(resp.errorCode, resp.errorMessage);
                        }

                        if (resp.errorCode != Utils.SUCCESS) {
                            //throw new ISPluginException(resp.errorMessage + ", Error Code [" + resp.errorCode + "]");
                            throw new ISPluginException(resp.errorCode, resp.errorMessage);
                        }

                        string cmd = resp.cmdType;
                        switch (cmd) {
                            case FUNC_GET_DEVICE_DETAILS: //Func 2.1
                            case FUNC_REFRESH: //Func 2.9
                                DeviceDetailsResp respDeviceDetails = JsonConvert.DeserializeObject<DeviceDetailsResp>(json);
                                sync.setSuccess(respDeviceDetails);
                                if (sync.deviceDetailsListener != null) {
                                    sync.deviceDetailsListener.onDeviceDetails(respDeviceDetails);
                                }
                                break;
                            case "SendInfoDetails":
                                break;
                            case FUNC_GET_INFO_DETAILS: //Func 2.2
                                //BaseDocumentDetailsResp baseDocumentDetailsResp = JsonConvert.DeserializeObject<BaseDocumentDetailsResp>(json);
                                DocumentDetailsResp baseDocumentDetailsResp = getDocumentDetails(json);
                                sync.setSuccess(baseDocumentDetailsResp);
                                if (sync.documentDetailsListener != null) {
                                    sync.documentDetailsListener.onDocumentDetails(baseDocumentDetailsResp);
                                }
                                break;
                            case FUNC_BIOMETRIC_AUTH: //Func 2.4
                                BiometricAuthResp biometricAuthenticationResp = biometricAuthentication(json);
                                sync.setSuccess(biometricAuthenticationResp);
                                if (sync.biometricAuthenticationListener != null) {
                                    sync.biometricAuthenticationListener.onBiometricAuthenticaton(biometricAuthenticationResp);
                                }
                                break;
                            case FUNC_CONNECT_DEVICE: //Func 2.5
                                ConnectToDeviceResp connectToDeviceResp = connectToDevice(json);
                                sync.setSuccess(connectToDeviceResp);
                                if (sync.connectToDeviceListener != null) {
                                    sync.connectToDeviceListener.onConnectToDevice(connectToDeviceResp);
                                }
                                break;
                            case FUNC_DISPLAY_INFO: //Func 2.6
                                DisplayInformationResp displayInfor = displayInformation(json);
                                sync.setSuccess(null);
                                if (sync.displayInformationListener != null) {
                                    sync.displayInformationListener.onDisplayInformation(displayInfor);
                                    sync.displayInformationListener.onSuccess();
                                }
                                break;
                            case FUNC_SCAN_DOCUMENT: //Func 2.10
                                ScanDocumentResp scanDocumentResp = scanDocument(json);
                                sync.setSuccess(scanDocumentResp);
                                if (sync.scanDocumentListenner != null) {
                                    sync.scanDocumentListenner.onScanDocument(scanDocumentResp);
                                }
                                break;
                        }
                    }
                    catch (Exception ex) {
                        sync.setError(ex);
                        if (sync.documentDetailsListener != null) {
                            sync.documentDetailsListener.onError(ex);
                        }
                        if (sync.deviceDetailsListener != null) {
                            sync.deviceDetailsListener.onError(ex);
                        }
                        if (sync.biometricAuthenticationListener != null) {
                            sync.deviceDetailsListener.onError(ex);
                        }
                        if (sync.displayInformationListener != null) {
                            sync.displayInformationListener.onError(ex);
                        }
                        if (sync.connectToDeviceListener != null) {
                            sync.connectToDeviceListener.onError(ex);
                        }
                        if (sync.scanDocumentListenner != null) {
                            sync.scanDocumentListenner.onError(ex);
                        }
                    } finally {
                        request.Remove(reqID);
                    }
                }
                else if (Utils.ToDescription(CmdType.SendInfoDetails).Equals(resp.cmdType)) { //Func 2.3
                    if (this.listener != null) {
                        DocumentDetailsResp documentDetails = getDocumentDetails(json);
                        listener.onReceivedDocument(documentDetails);
                    }
                    else {
                        DocumentDetailsResp documentDetails = getDocumentDetails(json);
                        if (null != delegateAuto) {
                            delegateAuto(documentDetails);
                        }
                        //documentRespAuto = documentDetails;
                    }
                }
                else if (Utils.ToDescription(CmdType.SendBiometricAuthentication).Equals(resp.cmdType)) { //Func 2.7
                    if (this.listener != null) {
                        BiometricAuthResp baseBiometricAuthResp = biometricAuthentication(json);
                        listener.onReceivedBiometricResult(baseBiometricAuthResp);
                    }
                    else {
                        BiometricAuthResp baseBiometricAuthResp = biometricAuthentication(json);
                        if (null != this.delegatebiometricResult) {
                            delegatebiometricResult(baseBiometricAuthResp);
                        }
                    }
                }
                else if (Utils.ToDescription(CmdType.CardDetectionEvent).Equals(resp.cmdType)) { //Func 2.8
                    if (this.listener != null) {
                        CardDetectionEventResp baseCardDetectionEventResp = cardDetectionEvent(json);
                        listener.onReceviedCardDetectionEvent(baseCardDetectionEventResp);
                    }
                    else {
                        CardDetectionEventResp baseCardDetectionEventResp = cardDetectionEvent(json);
                        if(null != this.delegateCardEvent) {
                            delegateCardEvent(baseCardDetectionEventResp);
                        }
                    }
                }
                else {
                    LOGGER.Debug("Not found Request with RequestID [" + reqID + "]" + " skip Response [" + json + "]");
                    if(resp.errorCode == Utils.ERR_FOR_DENIED_CONNECTION) {
                        if(null != listener) {
                            listener.onConnectDenied();
                        }
                    }
                    if (resp.errorCode != Utils.SUCCESS) {
                        //throw new ISPluginException(resp.errorMessage + ", Error Code [" + resp.errorCode + "]");
                        throw new ISPluginException(resp.errorCode, resp.errorMessage);
                    }
                }
            }
            catch (Exception ex) {
                LOGGER.Debug("Skip Response [" + json + "]" + " caused by " + ex.ToString());
            }
        }
        #endregion

        #region GET DOCUMENT DETAILS
        private DocumentDetailsResp getDocumentDetails(string json) {
            DocumentDetailsResp doc = JsonConvert.DeserializeObject<DocumentDetailsResp>(json);
            return doc;
        }
        #endregion

        #region BIOMETRIC AUTHENTICATIOn
        private BiometricAuthResp biometricAuthentication(string json) {
            BiometricAuthResp biometricAuth = JsonConvert.DeserializeObject<BiometricAuthResp>(json);
            return biometricAuth;
        }
        #endregion

        #region 4.5 CONNECT TO DEVICE
        private ConnectToDeviceResp connectToDevice(string json) {
            ConnectToDeviceResp connectDevice = JsonConvert.DeserializeObject<ConnectToDeviceResp>(json);
            return connectDevice;
        }
        #endregion

        #region 4.6 DISPLAY INFORMATION
        private DisplayInformationResp displayInformation(string json) {
            DisplayInformationResp displayInformation = JsonConvert.DeserializeObject<DisplayInformationResp>(json);
            return displayInformation;
        }
        #endregion

        #region CARD DETECTION EVENT 2022.05.10
        private CardDetectionEventResp cardDetectionEvent(string json) {
            CardDetectionEventResp baseCardDetectionEvent = JsonConvert.DeserializeObject<CardDetectionEventResp>(json);
            return baseCardDetectionEvent;
        }
        #endregion

        #region SCAN DOCUMENT
        private ScanDocumentResp scanDocument(string json) {
            ScanDocumentResp scanDocument = JsonConvert.DeserializeObject<ScanDocumentResp>(json);
            return scanDocument;
        }
        #endregion
    }
}
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
using WebSocketSharp;
using PluginICAOClientSDK.Response.BiometricAuth;
using PluginICAOClientSDK.Response.ConnectToDevice;
using PluginICAOClientSDK.Response.DisplayInformation;

namespace PluginICAOClientSDK {
    public delegate void DelegateAutoDocument(BaseDocumentDetailsResp documentDetailsResp);
    public delegate void DelegateAutoBiometricResult(BaseBiometricAuthResp baseBiometricAuthResp);
    public delegate void DelegateAutoReadNofity(string json);
    public class WebSocketClientHandler {
        #region VARIABLE
        private static readonly Logger LOGGER = new Logger(LogLevel.Debug);

        private StringBuilder response;
        private readonly ISPluginClient.ISListener listener;
        private static readonly double TIME_RECONNECT = 20;
        private static readonly int MAX_PING = 15;

        public readonly Dictionary<string, ResponseSync<object>> request = new Dictionary<string, ResponseSync<object>>();
        private DelegateAutoDocument delegateAuto;
        private DelegateAutoBiometricResult delegatebiometricResult;
        private DelegateAutoReadNofity delegateAutoReadNofity;

        private Timer timeoutTimer;
        private readonly object timeoutTimerLock = new object();
        private WebSocket ws;
        private bool isShutdown { get; set; }
        private bool isConnect = false;
        public bool IsConnect {
            get { return isConnect; }
        }
        private int checkConnectionDenied = 0;
        public int CheckConnectionDenied {
            get { return this.checkConnectionDenied; }
        }

        public BaseDocumentDetailsResp documentRespAuto { get; set; }
        #endregion

        #region CONSTRUCTOR
        public WebSocketClientHandler(string endPointUrl, bool secureConnect, DelegateAutoDocument dlgAuto,
                                      ISPluginClient.ISListener listener, DelegateAutoBiometricResult delegateAutoBiometric,
                                      DelegateAutoReadNofity dlgAutoReadNofity) {

            try {
                ws = new WebSocket(endPointUrl);
                if (secureConnect) {
                    ws.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                }
                this.listener = listener;
                this.delegateAuto = dlgAuto;
                this.delegatebiometricResult = delegateAutoBiometric;
                this.delegateAutoReadNofity = dlgAutoReadNofity;
                SetWebSocketSharpEvents();
            } catch (Exception e) {
                throw e;
            }
        }
        #endregion

        #region TIMER RE-CONECT
        private void ResetTimeoutTimer() {
            // if you are sure you will never access this from multiple threads at the same time - remove lock
            lock (timeoutTimerLock) {
                // initialize or reset the timer to fire once, after 10 seconds
                if (timeoutTimer == null)
                    timeoutTimer = new System.Threading.Timer(ReconnectAfterTimeout, null, TimeSpan.FromSeconds(TIME_RECONNECT), Timeout.InfiniteTimeSpan);
                else
                    timeoutTimer.Change(TimeSpan.FromSeconds(TIME_RECONNECT), Timeout.InfiniteTimeSpan);
            }
        }

        private void StopTimeoutTimer() {
            // if you are sure you will never access this from multiple threads at the same time - remove lock
            lock (timeoutTimerLock) {
                if (timeoutTimer != null)
                    timeoutTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            }
        }
        #endregion

        #region RE-CONECT HANDLE
        private void ReconnectAfterTimeout(object state) {
            // reconnect here
            wsConnect();
            LOGGER.Debug("RE-CONNECT");
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
            } catch (Exception e) {
                throw e;
            }
        }
        #endregion

        #region CONNECT HANDLE
        public void wsConnect() {
            ws.ConnectAsync();
        }
        #endregion

        #region OPEN HANDLE
        private void wsOnOpenHandle() {
            try {
                ws.OnOpen += (sender, e) => {
                    try {
                        isConnect = true;
                        LOGGER.Debug("CONNECT SUCCESSFULLY");
                        //ResetTimeoutTimer();
                        if (listener != null) {
                            listener.onConnected();
                        }
                    }
                    catch (WebSocketException eConnect) {
                        LOGGER.Debug("WEBSOCKET CLIETN FAILED TO CONNECT " + eConnect.ToString());
                    }
                    return;
                };
            } catch (Exception ex) {
                throw ex;
            }
        }
        #endregion

        #region MESSAGE HANDLE
        private void wsOnMessageHandle() {
            try {
                ws.OnMessage += (sender, e) => {
                    delegateAutoReadNofity(e.Data);

                    BaseDeviceDetailsResp baseDeviceDetailsResp = JsonConvert.DeserializeObject<BaseDeviceDetailsResp>(e.Data);
                    if(null != baseDeviceDetailsResp) {
                        checkConnectionDenied = baseDeviceDetailsResp.errorCode;
                    }

                    if (!ws.Ping()) {
                        //!ws.Ping()
                        //Try Ping RE-CONNECT
                        this.isConnect = false;

                        ws.EmitOnPing = true;
                        int countPong = 0;
                        int countPing = 0;
                        System.Timers.Timer timerPingPong = new System.Timers.Timer();
                        timerPingPong.Interval = 5000;
                        timerPingPong.Elapsed += (senderPingPong, ePingPong) => {
                            countPing++;
                            LOGGER.Debug("PING");
                            if (ws.Ping()) {
                                isConnect = true;

                                countPong++;
                                LOGGER.Debug("PONG++");
                            }
                            else {
                                countPong--;
                                LOGGER.Debug("PONG--");
                            }
                            if (countPing == MAX_PING) {
                                isConnect = true;
                                LOGGER.Debug("PING = 10");
                                timerPingPong.Stop();
                                if (countPong < MAX_PING) {
                                    isConnect = false;

                                    LOGGER.Debug("PONG < PING");
                                    this.isShutdown = true;
                                    ws.CloseAsync();
                                }
                                else {
                                    isConnect = true;
                                    LOGGER.Debug("PONG == PING");
                                    if (e.IsText) {
                                        if (!e.Data.Equals(string.Empty)) {
                                            response = new StringBuilder();
                                            response.Append(e.Data);
                                            processResponse(response.ToString());
                                            delegateAutoReadNofity(response.ToString());
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

                        if (e.IsText) {
                            if (!e.Data.Equals(string.Empty)) {
                                response = new StringBuilder();
                                response.Append(e.Data);
                                processResponse(response.ToString());
                                delegateAutoReadNofity(response.ToString());
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
                    StopTimeoutTimer();
                    LOGGER.Debug("SOCKET CLIENT ERROR MESSAGE " + e.Message);
                    //An exception has occurred during an OnMessage event. An error has occurred in closing the connection.
                    if (!ws.IsAlive) {
                        //this.isShutdown = true;
                        ws.Close();
                    }
                    else {
                        if (ws.Ping()) {
                            ResetTimeoutTimer();
                        }
                    }
                };
            } catch (Exception ex) {
                throw ex;
            }
        }
        #endregion

        #region CLOSE HANDLE
        private void wsOnCloseHandle() {
            ws.OnClose += (sender, e) => {
                // and here
                this.isConnect = false;
                if (this.isShutdown) {
                    StopTimeoutTimer();
                }
                else {
                    ResetTimeoutTimer();
                }
            };
        }
        #endregion

        #region SEND DATA
        public void sendData(string json) {
            ws.Send(json);
        }
        #endregion

        #region SHUTDOWN CLIENT
        public void shutdown() {
            try {
                this.isShutdown = true;
                this.ws.CloseAsync();
                LOGGER.Debug("SOCKET CLIENT SHUTDOWN");
            }
            catch (Exception e) {
                StopTimeoutTimer();
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
                            case "GetDeviceDetails":
                                BaseDeviceDetailsResp respDeviceDetails = JsonConvert.DeserializeObject<BaseDeviceDetailsResp>(json);
                                sync.setSuccess(respDeviceDetails);
                                if (sync.deviceDetailsListener != null) {
                                    sync.deviceDetailsListener.onReceivedDeviceDetails(respDeviceDetails);
                                }

                                //ISMessage<DeviceDetailsResp> ism = JsonConvert.DeserializeObject<ISMessage<DeviceDetailsResp>>(json);
                                //ism.data.cmdType = resp.cmdType;
                                //ism.data.requestID = resp.requestID;
                                //ism.data.errorCode = resp.errorCode;
                                //ism.data.errorMessage = resp.errorMessage;
                                //sync.setSuccess(ism.data);
                                //if (sync.deviceDetailsListener != null) {
                                //    sync.deviceDetailsListener.onReceivedDeviceDetails(ism.data);
                                //}
                                break;
                            case "SendInfoDetails":
                            case "GetInfoDetails":
                                //BaseDocumentDetailsResp baseDocumentDetailsResp = JsonConvert.DeserializeObject<BaseDocumentDetailsResp>(json);
                                BaseDocumentDetailsResp baseDocumentDetailsResp = getDocumentDetails(json);
                                sync.setSuccess(baseDocumentDetailsResp);
                                if (sync.documentDetailsListener != null) {
                                    sync.documentDetailsListener.onReceivedDocumentDetails(baseDocumentDetailsResp);
                                }
                                //DocumentDetailsResp documentDetails = getDocumentDetails(json);
                                //documentDetails.cmdType = resp.cmdType;
                                //documentDetails.requestID = resp.requestID;
                                //documentDetails.errorCode = resp.errorCode;
                                //documentDetails.errorMessage = resp.errorMessage;
                                //sync.setSuccess(documentDetails);
                                //if (sync.documentDetailsListener != null) {
                                //    sync.documentDetailsListener.onReceivedDocumentDetails(documentDetails);
                                //}
                                break;
                            case "BiometricAuthentication":
                                BaseBiometricAuthResp biometricAuthenticationResp = biometricAuthentication(json);
                                sync.setSuccess(biometricAuthenticationResp);
                                if (sync.biometricAuthenticationListener != null) {
                                    sync.biometricAuthenticationListener.onReceviedBiometricAuthenticaton(biometricAuthenticationResp);
                                }
                                break;
                            case "ConnectToDevice":
                                BaseConnectToDeviceResp connectToDeviceResp = connectToDevice(json);
                                sync.setSuccess(connectToDeviceResp);
                                if (sync.connectToDeviceListener != null) {
                                    sync.connectToDeviceListener.onReceviedConnectToDevice(connectToDeviceResp);
                                }

                                //connectToDeviceResp.cmdType = resp.cmdType;
                                //connectToDeviceResp.requestID = resp.requestID;
                                //connectToDeviceResp.errorCode = resp.errorCode;
                                //connectToDeviceResp.errorMessage = resp.errorMessage;
                                //sync.setSuccess(connectToDeviceResp);
                                //if (sync.connectToDeviceListener != null) {
                                //    sync.connectToDeviceListener.onReceviedConnectToDevice(connectToDeviceResp);
                                //}
                                break;
                            case "DisplayInformation":
                                BaseDisplayInformation displayInfor = displayInformation(json);
                                sync.setSuccess(displayInfor);
                                if(sync.displayInformationListener != null) {
                                    sync.displayInformationListener.onReceviedDisplayInformation(displayInfor);
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
                    } finally {
                        request.Remove(reqID);
                    }
                }
                else if (Utils.ToDescription(CmdType.SendInfoDetails).Equals(resp.cmdType)) {
                    if (this.listener != null) {
                        BaseDocumentDetailsResp documentDetails = getDocumentDetails(json);
                        listener.onReceivedDocument(documentDetails);
                    }
                    else {
                        BaseDocumentDetailsResp documentDetails = getDocumentDetails(json);
                        delegateAuto(documentDetails);
                        //documentRespAuto = documentDetails;
                    }
                }
                else if (Utils.ToDescription(CmdType.SendBiometricAuthentication).Equals(resp.cmdType)) {
                    if(this.listener != null) {
                        BaseBiometricAuthResp baseBiometricAuthResp = biometricAuthentication(json);
                        listener.onReceivedBiometricResult(baseBiometricAuthResp);
                    } else {
                        BaseBiometricAuthResp baseBiometricAuthResp = biometricAuthentication(json);
                        delegatebiometricResult(baseBiometricAuthResp);
                    }
                }
                else {
                    LOGGER.Debug("Not found Request with RequestID [" + reqID + "]" + " skip Response [" + json + "]");
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
        private BaseDocumentDetailsResp getDocumentDetails(string json) {
            BaseDocumentDetailsResp doc = JsonConvert.DeserializeObject<BaseDocumentDetailsResp>(json);
            return doc;
        }
        #endregion

        #region BIOMETRIC AUTHENTICATIOn
        private BaseBiometricAuthResp biometricAuthentication(string json) {
            BaseBiometricAuthResp biometricAuth = JsonConvert.DeserializeObject<BaseBiometricAuthResp>(json);
            return biometricAuth;
        }
        #endregion

        #region 4.5 CONNECT TO DEVICE
        private BaseConnectToDeviceResp connectToDevice(string json) {
            BaseConnectToDeviceResp connectDevice = JsonConvert.DeserializeObject<BaseConnectToDeviceResp>(json);
            return connectDevice;
        }
        #endregion

        #region 4.6 DISPLAY INFORMATION
        private BaseDisplayInformation displayInformation(string json) {
            BaseDisplayInformation displayInformation = JsonConvert.DeserializeObject<BaseDisplayInformation>(json);
            return displayInformation;
        }
        #endregion
    }
}
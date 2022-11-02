using PluginICAOClientSDK.Util;
using PluginICAOClientSDK;
using System;
using System.Threading;

namespace PluginICAOClientSDK {
    public class ResponseSync<T>{
        public string cmdType { get; set; }
        public T response { get; set; }
        private CountdownEvent wait;
        public CountdownEvent Wait {
            get { return wait; }
            set { wait = value; }
        }
        private Exception error;
        public ISPluginClient.DeviceDetailsListener deviceDetailsListener { get; set; }
        public ISPluginClient.DocumentDetailsListener documentDetailsListener { get; set; }
        public ISPluginClient.BiometricAuthListener biometricAuthenticationListener { get; set; }
        public ISPluginClient.ConnectToDeviceListener connectToDeviceListener { get; set; }
        public ISPluginClient.DisplayInformationListener displayInformationListener { get; set; }
        public ISPluginClient.ScanDocumentListenner scanDocumentListenner { get; set; }
        public ISPluginClient.BiometricEvidenceListenner biometricEvidenceListenner { get; set; }

        public T waitResponse (int timeout) {
            try {
                if (!wait.Wait(TimeSpan.FromMilliseconds(timeout))) {
                    throw new ISPluginException("Timeout to receive response");
                }
                if(error != null) {
                    throw error;
                }
                return response;
            } catch (ISPluginException ex) {
                throw ex;
            } catch (Exception ex) {
                throw new ISPluginException(ex);
            }
        }

        public void setError(Exception error) {
            this.error = error;
            wait.Signal();
        }

        public void setSuccess(T res) {
            this.response = res;
            wait.Signal();
        }
    }
}

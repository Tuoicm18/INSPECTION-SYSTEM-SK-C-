using System;

namespace PluginICAOClientSDK.Response.DeviceDetails {
    public class DataDeviceDetails {
        public string deviceName { get; set; }
        public string deviceSN { get; set; }
        public string lastScanTime { get; set; }
        public int totalPreceeded { get; set; }
        public bool activePresenceDetection { get; set; }
    }
}

using System;
using System.ComponentModel;

namespace PluginICAOClientSDK.Models {
    public enum BiometricType {
        [Description("faceID")]
        FACE_ID,
        [Description("fingerLeftIndex")]
        LEFT_FINGER,
        [Description("fingerRightIndex")]
        RIGHT_FINGER
    }
}

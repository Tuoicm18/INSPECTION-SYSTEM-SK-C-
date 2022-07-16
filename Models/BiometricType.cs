using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace PluginICAOClientSDK.Models {
    public enum BiometricType {
        [Description("faceID")]
        TYPE_FACE,
        [Description("fingerLeftIndex")]
        TYPE_FINGER_LEFT,
        [Description("fingerRightIndex")]
        TYPE_FINGER_RIGHT,
    }
}

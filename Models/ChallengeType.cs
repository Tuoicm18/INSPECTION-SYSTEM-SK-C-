﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Models {
    public enum ChallengeType {
        [Description("string")]
        TYPE_STRING,
        [Description("object")]
        TYPE_OBJECT,
    }
}

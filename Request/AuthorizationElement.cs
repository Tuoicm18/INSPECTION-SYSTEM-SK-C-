﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Request {
    public class AuthorizationElement {
        [JsonIgnore]
        public AuthElementType type { get; set; }

        public int ordinary { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string label { get; set; }
        public string title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string text { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, bool> multipleSelect { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, bool> singleSelect { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> nameValuePair { get; set; }
    }

    public enum AuthElementType {
        [Description("Content")]
        Content,
        [Description("Multiple")]
        Multiple,
        [Description("Single")]
        Single,
        [Description("NVP")]
        NVP
    }

    internal static class Extensions {
        public static string ToDescription(this Enum value) {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }
}

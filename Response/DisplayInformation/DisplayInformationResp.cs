using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Response.DisplayInformation {
    public class DisplayInformationResp : BaseResponse  {
        public DataDisplayInformation data { get; set; }
    }
}

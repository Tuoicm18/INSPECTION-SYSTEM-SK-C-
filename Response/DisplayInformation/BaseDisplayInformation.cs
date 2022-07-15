using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginICAOClientSDK.Response.DisplayInformation {
    public class BaseDisplayInformation : BaseResponse  {
        public DataDisplayInformation data { get; set; }
    }
}

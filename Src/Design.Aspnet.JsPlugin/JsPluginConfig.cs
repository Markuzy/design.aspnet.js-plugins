using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Design.Aspnet.JsPlugin
{
    public sealed class JsPluginConfig
    {
        public string DestinationMappedPath { get; set; } = "/plugin";

        public bool OutputToWwwroot { get; set; } = false;
    }
}

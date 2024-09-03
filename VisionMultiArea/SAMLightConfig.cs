using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionMultiArea
{
    public class SAMLightConfig
    {
        public string IpSamlight { get; set; } = "127.0.0.1";
        public int PortSamlight { get; set; } = 3500;
        public string LayoutFolder { get; set; } = String.Empty;
        public bool? DesignScaps { get; set; }
        /// <summary>
        /// Default is 65001 == UTF8 should Match the Codepage in Scaps
        /// </summary>
        public int Codepage { get; set; } = 65001;
    }
}

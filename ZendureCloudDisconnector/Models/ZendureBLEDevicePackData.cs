using System;
using System.Collections.Generic;
using System.Text;

namespace ZendureCloudDisconnector.Models
{
    class ZendureBLEDevicePackData
    {
        public string sn { get; set; }
        public int? socLevel { get; set; }
        public int? maxTemp { get; set; }
        public int? minVol { get; set; }
        public int? maxVol { get; set; }
        public int? totalVol { get; set; }
    }
}

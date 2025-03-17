using System;
using System.Collections.Generic;
using System.Text;

namespace ZendureCloudDisconnector.Models
{
    class ZendureBLEResponse
    {
        public string method { get; set; }
        public string deviceId { get; set; }
        public ZendureBLEDeviceProperties properties { get; set; }
        public List<ZendureBLEDevicePackData> packData { get; set; }

    }
}

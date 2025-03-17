using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Xml;

namespace ZendureCloudDisconnector.Models
{
    class ZendureBLEDeviceProperties
    {
        public int? electricLevel { get; set; }
        public int? packState { get; set; }
        public int? pass { get; set; }
        public int? passMode { get; set; }
        public int? autoRecover { get; set; }
        public int? outputHomePower { get; set; }
        public int? outputLimit { get; set; }
        public int? buzzerSwitch { get; set; }
        public int? outputPackPower { get; set; }
        public int? packInputPower { get; set; }
        public int? solarInputPower { get; set; }
        public int? pvPower1 { get; set; }
        public int? pvPower2 { get; set; }
        public int? solarPower1 { get; set; }
        public int? solarPower2 { get; set; }
        public int? remainOutTime { get; set; }
        public int? remainInputTime { get; set; }
        public int? socSet { get; set; }
        public int? minSoc { get; set; }
        public int? pvBrand { get; set; }
        public int? inverseMaxPower { get; set; }
        public int? wifiState { get; set; }
        public int? hubState { get; set; }
        public string? deviceId { get; set; }
        public string? name { get; set; }
        public string? sn { get; set; }
        public int? inputLimit { get; set; }
        public int? gridInputPower { get; set; }
        public int? acOutputPower { get; set; }
        public int? acSwitch { get; set; }
        public int? dcSwitch { get; set; }
        public int? dcOutputPower { get; set; }
        public int? packNum { get; set; }
        public int? gridPower { get; set; }
        public int? energyPower { get; set; }
        public int? batteryElectric { get; set; }
        public int? acMode { get; set; }
        public int? hyperTmp { get; set; }
        public int? autoModel { get; set; }
        public int? heatState { get; set; }
    }
}

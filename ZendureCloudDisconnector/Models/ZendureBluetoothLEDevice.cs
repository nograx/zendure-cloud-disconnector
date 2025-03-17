using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Bluetooth;

namespace ZendureCloudDisconnector.Models
{
    public class ZendureBluetoothLEDevice
    {
        public string DeviceType { get; set; }
        public BluetoothLEDevice BluetoothLEDevice { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZendureCloudDisconnector.Models;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Media.Ocr;
using Windows.Media.Protection.PlayReady;
using Windows.Storage.Streams;
using System.Text.Json;
using Newtonsoft.Json;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using Windows.ApplicationModel.Appointments.DataProvider;

namespace ZendureCloudDisconnector.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        private string SF_COMMAND_CHAR = "0000c304-0000-1000-8000-00805f9b34fb";
        private string SF_NOTIFY_CHAR = "0000c305-0000-1000-8000-00805f9b34fb";

        private GattCharacteristic CommandGattCharacteristic = null;
        private GattCharacteristic NotifyGattCharacteristic = null;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _Paired;
        public string Paired
        {
            get { return _Paired; }
            set { _Paired = value; OnPropertyChanged("Paired"); }
        }

        private string _BluetoothServices;
        public string BluetoothServices // Set if BLE Serrvices found
        {
            get { return _BluetoothServices; }
            set { _BluetoothServices = value; OnPropertyChanged("BluetoothServices"); }
        }

        private string _GattChar;
        public string GattChar // Set if GATT Chars found
        {
            get { return _GattChar; }
            set { _GattChar = value; OnPropertyChanged("GattChar"); }
        }

        private ZendureBLEDeviceProperties _ZendureBLEDeviceProperties;
        public ZendureBLEDeviceProperties ZendureBLEDeviceProperties
        {
            get { return _ZendureBLEDeviceProperties; }
            set 
            { 
                _ZendureBLEDeviceProperties = value; 
                OnPropertyChanged("ZendureBLEDeviceProperties"); 
            }
        }

        private ObservableCollection<ZendureBluetoothLEDevice> _Devices;
        public ObservableCollection<ZendureBluetoothLEDevice> Devices
        {
            get { return _Devices; }
            set { _Devices = value; }
        }

        private ZendureBluetoothLEDevice _SelectedBluetoothLEDevice;
        public ZendureBluetoothLEDevice SelectedBluetoothLEDevice
        {
            get { return _SelectedBluetoothLEDevice; }
            set 
            { 
                _SelectedBluetoothLEDevice = value;

                OnPropertyChanged("SelectedBluetoothLEDevice");

                if (value != null)
                { 
                    Debug.WriteLine(value.BluetoothLEDevice.Name); 
                }
            }
        }

        private bool _SearchInactive;
        public bool SearchInactive
        {
            get { return _SearchInactive; }
            set { 
                _SearchInactive = value; 
                OnPropertyChanged("SearchInactive"); 
            }
        }

        private string _WifiName;
        public string WifiName
        {
            get { return _WifiName; }
            set
            {
                _WifiName = value;
            }
        }

        private string _WifiPassword;
        public string WifiPassword
        {
            get { return _WifiPassword; }
            set
            {
                _WifiPassword = value;
            }
        }

        private string _MqttServer;
        public string MqttServer
        {
            get { return _MqttServer; }
            set
            {
                _MqttServer = value;
            }
        }

        public MainViewModel()
        {
            _Devices = new ObservableCollection<ZendureBluetoothLEDevice>();
            SearchInactive = true;

            WifiName = "";
            WifiPassword = "";
            MqttServer = "";
        }
        private bool CheckReconnectEnabled()
        {
            if (WifiName == null || WifiPassword == null || WifiName == "" || WifiPassword == "" || SelectedBluetoothLEDevice == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CheckDisconnectEnabled()
        {
            if (WifiName == null || WifiPassword == null || WifiName == "" || WifiPassword == "" || MqttServer == null || MqttServer == "" || SelectedBluetoothLEDevice == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task StartWatcher()
        {
            Devices.Clear();
            var watcher = new BluetoothLEAdvertisementWatcher();
            watcher = new BluetoothLEAdvertisementWatcher()
            {
                ScanningMode = BluetoothLEScanningMode.Active
            };

            watcher.Received += Watcher_Received;

            SearchInactive = false;
            watcher.Start();

            await Task.Delay(10000);

            watcher.Stop();

            if (Devices.Count == 0)
            {
                MessageBox.Show("No Zendure device found!");
            }

            SearchInactive = true;
        }

        private async void Watcher_Received(
            BluetoothLEAdvertisementWatcher sender,
            BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var device = await BluetoothLEDevice.FromBluetoothAddressAsync(args.BluetoothAddress);
            if (device != null && device.Name.ToLower().Contains("zen"))
            {
                string zenDeviceType = "";

                switch (device.Name.ToLower().Substring(0, 4))
                {
                    case "zenp":
                        zenDeviceType = "SolarFlow HUB 1200";
                        break;
                    case "zenh":
                        zenDeviceType = "SolarFlow HUB 2000";
                        break;
                    case "zenr":
                        zenDeviceType = "AIO 2400";
                        break;
                    case "zene":
                        zenDeviceType = "Hyper 2000";
                        break;
                    case "zenf":
                        zenDeviceType = "ACE 1500";
                        break;
                    default:
                        zenDeviceType = "Uknown Zendure Device (" + device.Name + ")";
                        break;
                }

                if (!_Devices.Any(x => x.BluetoothLEDevice.BluetoothAddress == device.BluetoothAddress))
                {
                    ZendureBluetoothLEDevice zenBleDevice = new ZendureBluetoothLEDevice() { BluetoothLEDevice = device, DeviceType = zenDeviceType };
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        Devices.Add(zenBleDevice);
                    });
                }
            }
        }

        public async Task<bool> DisconnectFromZendureCloud()
        {
            if (CheckDisconnectEnabled())
            {
                await SetIOTUrl(MqttServer, WifiName, WifiPassword);
                return true;
            }
            else
            {
                MessageBox.Show("Please select a Zendure device, provide WiFi SSID, password and MQTT server url/ip!");
                return false;
            }            
        }

        public async Task<bool> ReconnectToZendureCloud()
        {
            if (CheckReconnectEnabled())
            {
                await SetIOTUrl("mq.zen-iot.com", WifiName, WifiPassword);
                return true;
            }
            else
            {
                MessageBox.Show("Please select a Zendure device and provide WiFi SSID and password!");
                return false;
            }
        }

        public async Task<bool> SetIOTUrl(string iotUrl, string ssid, string password)
        {
            await Pair();

            if (CommandGattCharacteristic == null)
            {
                // Fehler!
                MessageBox.Show("GATT characteristic not found!");
            }

            var paras = new IotUrlParams() { iotUrl = iotUrl, ssid = ssid, password = password, messageId = "1002", method = "token", token = "ababcdefgh", timeZone = "GMT+02:00" };
            var paramsStr = JsonConvert.SerializeObject(paras);
            byte[] bytes = Encoding.ASCII.GetBytes(paramsStr);

            IBuffer writer = bytes.AsBuffer();
            try
            {
                // BT_Code: Writes the value from the buffer to the characteristic.         
                var result = await CommandGattCharacteristic.WriteValueAsync(writer);
                if (result == GattCommunicationStatus.Success)
                { 
                    var paras2 = new IotUrlParams() { messageId = "1003", method = "station" };
                    var paramsStr2 = JsonConvert.SerializeObject(paras);
                    byte[] bytes2 = Encoding.ASCII.GetBytes(paramsStr);

                    IBuffer writer2 = bytes2.AsBuffer();

                    var result2 = await CommandGattCharacteristic.WriteValueAsync(writer);
                    if (result2 == GattCommunicationStatus.Success)
                    {
                        // Hat funktioniert!
                        MessageBox.Show("Successfully set the new IOT URL and Wifi! Please reboot your Zendure Device for these changes to take effect!");
                    }
                }
                else
                {
                    
                }
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x80650003 || (uint)ex.HResult == 0x80070005)
            {
                // E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED or E_ACCESSDENIED
                // This usually happens when a device reports that it
                //support writing, but it actually doesn't.
                MessageBox.Show("Error setting the new IOT Url!");
            }

            return true;
        }

        public async Task<bool> Pair()
        {
            SelectedBluetoothLEDevice.BluetoothLEDevice.DeviceInformation.Pairing.Custom.PairingRequested +=
                    (ss, ev) =>
                    {
                        ev.Accept();
                    };
            var result = await SelectedBluetoothLEDevice.BluetoothLEDevice.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ConfirmOnly);

            if (result.Status == DevicePairingResultStatus.AlreadyPaired)
            {
                var a = await SelectedBluetoothLEDevice.BluetoothLEDevice.DeviceInformation.Pairing.UnpairAsync();
                var result2 = await SelectedBluetoothLEDevice.BluetoothLEDevice.DeviceInformation.Pairing.Custom.PairAsync(DevicePairingKinds.ConfirmOnly);

                if (result2.Status == DevicePairingResultStatus.Paired)
                {
                    Paired = "OK";
                }
            }

            if (result.Status == DevicePairingResultStatus.Paired)
            {
                Paired = "OK";
            }

            await GetServices();

            return true;
        }

        private async Task<bool> GetServices()
        {
            GattDeviceServicesResult result = await SelectedBluetoothLEDevice.BluetoothLEDevice.GetGattServicesAsync();
            if (result.Status == GattCommunicationStatus.Success)
            {
                BluetoothServices = "OK";
                var services = result.Services;
                await GetCharacterstics(services);

                return true;
            }

            return false;
        }

        private async Task<bool> GetCharacterstics(IReadOnlyList<GattDeviceService> services)
        {
            foreach (GattDeviceService s in services)
            {
                GattCharacteristicsResult result = await s.GetCharacteristicsAsync();
                if (result.Status == GattCommunicationStatus.Success)
                {
                    var characteristicss = result.Characteristics;
                    foreach (GattCharacteristic c in characteristicss)
                    {
                        if (c.Uuid == Guid.Parse(SF_COMMAND_CHAR))
                        {
                            CommandGattCharacteristic = c;
                        }

                        if (c.Uuid == Guid.Parse(SF_NOTIFY_CHAR))
                        {
                            NotifyGattCharacteristic = c;
                        }
                    }
                }
            }

            if (CommandGattCharacteristic != null && NotifyGattCharacteristic != null)
            {
                GattChar = "OK";
            }

            return true;
        }

        public async void SubscribeToNotifyGattCharacteristic()
        {
            await Pair();

            if (NotifyGattCharacteristic == null)
            {
                // Mööööp
            }
            else if (NotifyGattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                GattCommunicationStatus status = await NotifyGattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                        GattClientCharacteristicConfigurationDescriptorValue.None);
                if (status == GattCommunicationStatus.Success)
                {
                    NotifyGattCharacteristic.ValueChanged += RecieveDataAsync;
                    Console.WriteLine("Recieving notifications from device.");
                }
                else
                {
                    MessageBox.Show("Error subscribing to Telemetry data!");
                }
            }
        }

        private void RecieveDataAsync(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var reader = DataReader.FromBuffer(args.CharacteristicValue);
            reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
            uint bufferLength = (uint)reader.UnconsumedBufferLength;
            string receivedString = "";
            receivedString += reader.ReadString(bufferLength) + "\n";
            ZendureBLEResponse response = JsonConvert.DeserializeObject<ZendureBLEResponse>(receivedString);

            Debug.WriteLine(receivedString);

            if (ZendureBLEDeviceProperties == null)
            {
                ZendureBLEDeviceProperties = response.properties;
            }
            else
            {
                if (response != null && response.properties != null)
                {
                    if (response.deviceId != null)
                    {
                        ZendureBLEDeviceProperties.deviceId = response.deviceId;
                    }

                    if (response.properties.electricLevel != null)
                    {
                        ZendureBLEDeviceProperties.electricLevel = response.properties.electricLevel;
                    }

                    if (response.properties.solarInputPower != null)
                    {
                        ZendureBLEDeviceProperties.solarInputPower = response.properties.solarInputPower;
                    }

                    if (response.properties.packInputPower != null)
                    {
                        ZendureBLEDeviceProperties.packInputPower = response.properties.packInputPower;
                    }

                    if (response.properties.outputPackPower != null)
                    {
                        ZendureBLEDeviceProperties.outputPackPower = response.properties.outputPackPower;
                    }

                    OnPropertyChanged("ZendureBLEDeviceProperties");
                }                
            }
        }
    }
}

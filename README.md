# Zendure Cloud Disconnector

This project is a simple tool to modify the internal MQTT server url/ip from Zendure SolarFlow devices via Bluetooth LE.

## Donate

If you want to support my work, feel free to donate via the folowwing Paypal ME link. THANK YOU!<br />

[![Donate](https://img.shields.io/badge/PayPal-00457C?style=for-the-badge&logo=paypal&logoColor=white)](https://www.paypal.com/paypalme/PeterFrommert)

## Features

You can use this simple tool to connect to your Zendure device (HUB1200, HUB 2000, Hyper 2000, AIO 2400 and Ace 1500) via Bluetooth LE. You can then get basic telemetry data from the device.

The main goal of this tool is to modify the MQTT server address of the device to sent all data to your own MQTT server and also read commands from it. After modifying the MQTT server you have to reboot the device (power off and power on again) for these changes to take effect.

The device will then sent it's MQTT data to your server (in an somehow unusual way). There will be an JSON string in /productKey/deviceKey/properties/report which you have to deserialize in a script with the smart home system you like. It's also possible to gain full control of the device. You have to subscribe to iot/productId/deviceId/properties/write and then sent a payload like this "{"properties": { "outputLimit": 200 }}".

If you use ioBroker you can simply use my adapter zendure-solarflow, which can also connect to your MQTT instance and deserialize all information and provide the data in readable data points. It's also possible to control the device like with the official app. You can find the ioBroker adapter project on github: [ioBroker.zendure-solarflow](https://github.com/nograx/ioBroker.zendure-solarflow)

Note: You are still able to connect to the device with the official app via bluetooth. It's also possible to do firmware updates via bluetooth.

You can also revert the changes to it's default MQTT server.

## Credits

Credits goes to https://github.com/reinhard-brandstaedter/solarflow which helped a lot with the knowledge about the MQTT server from Zendure! Thanks!

## Changelog

###

## License

MIT License

Copyright (c) 2025 Peter Frommert

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

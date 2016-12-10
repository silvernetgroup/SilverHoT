using GrovePi;
using GrovePi.I2CDevices;
using GrovePi.Sensors;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RPITest4
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static int i = 0;
        public MainPage()
        {
            this.InitializeComponent();           
            SendDeviceToCloudMessagesAsync();
        }

        static async void SendDeviceToCloudMessagesAsync()
        {
            IButtonSensor button = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin2);
            IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();
            string iotHubUri = "SilverHoT.azure-devices.net"; // ! put in value !
            string deviceId = "SilverRPI3"; // ! put in value !
            string deviceKey = "MQfjkzuhYQ6eBH7NU02joMZ3NC2RNzEYFdWCQtepv8I="; // ! put in value !

            var deviceClient = DeviceClient.Create(iotHubUri,
        AuthenticationMethodFactory.
        CreateAuthenticationWithRegistrySymmetricKey(deviceId, deviceKey),
        TransportType.Http1);

            

            while (true)
            {
                string buttonon = button.CurrentState.ToString();
                if (buttonon.Equals("On", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var str = "Hello, Cloud!";
                        var message = new Message(Encoding.ASCII.GetBytes(str));
                        display.SetText("Start").SetBacklightRgb(255, 50, 255);
                        await deviceClient.SendEventAsync(message);
                        i++;
                        display.SetText(i.ToString()).SetBacklightRgb(255, 50, 255);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    
                }
            }            
        }
    }
}

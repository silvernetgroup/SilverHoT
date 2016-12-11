using GrovePi;
using GrovePi.I2CDevices;
using GrovePi.Sensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace RpiSender
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IButtonSensor button = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin8);
        IRgbLcdDisplay display = DeviceFactory.Build.RgbLcdDisplay();
        IDHTTemperatureAndHumiditySensor tempSensor = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin7, DHTModel.Dht11);
        //IUltrasonicRangerSensor sensor = DeviceFactory.Build.UltraSonicSensor(Pin.DigitalPin4);
        ILightSensor lightSensor = DeviceFactory.Build.LightSensor(Pin.AnalogPin0);
        ILed led = DeviceFactory.Build.Led(Pin.DigitalPin8);
        IBuzzer buzzer = DeviceFactory.Build.Buzzer(Pin.DigitalPin3);
        DispatcherTimer sendTimer = new DispatcherTimer();
        DispatcherTimer measureTimer = new DispatcherTimer();
        string lightSensorValue = "";
        string temperatureSensorValue = "";
        string humiditySensorValue = "";
        bool measure = true;
        int time;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public MainPage()
        {
            this.InitializeComponent();
            time = Convert.ToInt32(localSettings.Values["time"]);
            sendTimer.Tick += Timer_Tick;
            sendTimer.Interval = TimeSpan.FromSeconds(10);

            measureTimer.Tick += MeasureTimer_Tick;
            measureTimer.Interval = TimeSpan.FromSeconds(2);

            measureTimer.Start();
            sendTimer.Start();
        }

        private void MeasureTimer_Tick(object sender, object e)
        {
            if (measure)
            {
                tempSensor.Measure();
                lightSensorValue = lightSensor.SensorValue().ToString();
                humiditySensorValue = tempSensor.Humidity.ToString();
                temperatureSensorValue = tempSensor.TemperatureInCelsius.ToString();
                display.SetText("Light:" + lightSensorValue +"Hum:" + humiditySensorValue + "\n" + "Temperature: " + temperatureSensorValue).SetBacklightRgb(0, 255, 0);
                if(Convert.ToUInt32(lightSensorValue)<500)
                {
                    if(led.CurrentState== SensorStatus.Off)
                        Buzz();
                    led.ChangeState(SensorStatus.On);                    
                }
                else
                {
                    if (led.CurrentState == SensorStatus.On)
                        Buzz();
                    led.ChangeState(SensorStatus.Off);
                }
            }

        }

        private async void Timer_Tick(object sender, object e)
        {
            measure = false;
            measureTimer.Stop();
            display.SetText("Wysylam").SetBacklightRgb(255, 50, 255);
            await AzureIoTHub.SendDeviceToCloudMessageAsync(lightSensorValue, temperatureSensorValue,humiditySensorValue, time);
            display.SetText("Wyslano").SetBacklightRgb(255, 50, 255);
            time++;
            localSettings.Values["time"] = time;
            measure = true;
            measureTimer.Start();
        }
        public void Buzz()
        {
            buzzer.ChangeState(SensorStatus.On);
            Task.Delay(200).Wait();
            buzzer.ChangeState(SensorStatus.Off);
        }
    }
}

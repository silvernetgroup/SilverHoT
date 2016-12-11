using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

static class AzureIoTHub
{
    //
    // Note: this connection string is specific to the device "SilverRPI". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "HostName=SilverHoTH.azure-devices.net;DeviceId=SilverRPI;SharedAccessKey=V0koXVTKMZ5cMcaO75pAXHo8MujIsdTYLMfEooBQOs0=";
    static int i = 0;

    //
    // To monitor messages sent to device "SilverRPI" use iothub-explorer as follows:
    //    iothub-explorer HostName=SilverHoTH.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=hEgI5+kDvERHFcrXW1xFoFkR4pK3K0h6/giRp6WkwBM= monitor-events "SilverRPI"
    //

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub

    public static async Task SendDeviceToCloudMessageAsync(string lightValue, string temperatureValue, string humidityValue, int time)
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);


        string str = "{\"deviceid\": \"SilverRPI\",\"time\":" + time.ToString()

                + ",\"lightValue\":" + lightValue.ToString() +",\"temperatureValue\":" + temperatureValue.ToString() + ",\"humidityValue\":" + humidityValue.ToString() +  "}";

        var message = new Message(Encoding.UTF8.GetBytes(str));

        await deviceClient.SendEventAsync(message);
    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Amqp);

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}

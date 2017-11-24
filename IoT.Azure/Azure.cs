using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace IoT.Azure
{
    public class Azure : IDisposable
    {
        private DeviceClient deviceClient;
        public string Message;
        public Azure(IDevice client, TransportType transportType = TransportType.Http1)
        {
            deviceClient = DeviceClient.CreateFromConnectionString(client.ConnectionString, client.DeviceId, transportType);
        }
        /// <summary>
        /// Send message from sensor to azure.
        /// </summary>
        /// <param name="messages">Message obejcts <see cref="Message"/>.</param>
        public void SendMessage(Message message)
        {
            this.SendMessageAsync(message);
        }
        /// <summary>
        /// Send messages from sensor to azure.
        /// </summary>
        /// <param name="messages">Bunch og message obejcts <see cref="Message"/>.</param>
        public void SendMessages(IEnumerable<Message> messages)
        {
            SendMessagesAsync(messages);
        }
        /// <summary>
        /// Recieve data from Azure IoT Hub.
        /// </summary>
        public void RecieveData()
        {
            var tmp =Task.Run(ReceiveDataAsync);
        }
        protected async void SendMessageAsync(Message message)
        {
            try
            {
                if (message != null)
                {
                    await deviceClient.SendEventAsync(message);
                    await deviceClient.CompleteAsync(message);
                }
                else
                    Message = "Message is not initialized (Sensors.DHT11.SendMessageToCloud()";
            }
            catch(Exception ex)
            {
                LoggerHelper.Log(ex);
            }
            finally
            {
                await deviceClient.RejectAsync(message);
            }
        }
        protected async void SendMessagesAsync(IEnumerable<Message> messages)
        {
            try
            {
                if (messages != null || messages.Count() <= 0)
                {
                    await deviceClient.SendEventBatchAsync(messages);
                    await deviceClient.CompleteAsync("Messages sent correctly!");
                }
                else
                    Message = "Message is not initialized (Sensors.DHT11.SendMessageToCloud()";
            }
            catch (Exception ex)
            {
                LoggerHelper.Log(ex);
                Message = LoggerHelper.log;
            }
        }
        protected async Task ReceiveDataAsync()
        {

            while (true)
            {
                Message message = await deviceClient.ReceiveAsync();
                if (message != null)
                {
                    try
                    {
                        dynamic command = DeSerialize(message.GetBytes());
                        if (command.Name == "TriggerAlarm")
                        {
                            // Received a new message, display it
                            
                            //await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            //async () =>
                            //{
                            //    var dialogbox = new MessageDialog("Received message from Azure IoT Hub: " + command.Parameters.Message.ToString());
                            //    await dialogbox.ShowAsync();
                            //});

                            // We received the message, indicate IoTHub we treated it
                            await deviceClient.CompleteAsync(message);
                        }
                    }
                    catch
                    {
                        await deviceClient.RejectAsync(message);
                    }
                }
            }
        }
        private byte[] Serialize(object obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);

        }

        private dynamic DeSerialize(byte[] data)
        {
            string text = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject(text);
        }
        /// <summary>
        /// Send Metadata about controller and its properties to cloud Azure IoT.
        /// </summary>
        /// <param name="meta">Metadata: Descirbe properties about controller and messages types etc. <see cref="Metadata"/>.</param>
        /// <param name="command">Type of command what we sent to Azure IoT hub. <see cref="Command"/>.</param>
        public async void sendDeviceMetaData(Metadata meta, Command command)
        {
            try
            {
                var msg = new Message(Serialize(meta));
                if (deviceClient != null)
                {
                    await deviceClient.SendEventAsync(msg);
                }
            }
            catch (System.Exception e)
            {
                Debug.Write("Exception while sending device meta data :\n" + e.Message.ToString());
                LoggerHelper.Log(e);
            }
            Debug.Write("Sent meta data to IoT Suite\n" + meta.Device.HubHostname);
        }
        /// <summary>
        /// Dispose Device client for azure.
        /// </summary>
        public void Dispose()
        {
            this.deviceClient = null;
        }
    }
}

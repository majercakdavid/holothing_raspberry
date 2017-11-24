using IoT.Azure;
using IoTHelpers.Gpio.Modules;
using IoTHelpers.Runtime;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio.Provider;

namespace IoT.Raspberry
{
    public static class Sensors
    {
        /// <summary>
        /// Represents DHT11 type sensor connectod to device.
        /// </summary>
        public class DHT11: INotifyPropertyChanged
        {
            const int DHT11_PIN = 4;
            private int pinNumber;
            private IoTHelpers.Gpio.Modules.Dht11HumitureSensor m_sensor;
            private Pin nativePin;
            private IGpioControllerProvider m_controller;
            public IEnumerable<Message> HumitaireMessages;
            private string _message;
            public string Message
            {
                get { return this._message; }
                set
                {
                    if (value != null)
                    {
                        this._message = value;
                        OnPropertyChanged();
                    }
                }
            }

            /// <summary>
            /// Create Instance of DHT11 sensor.<see cref="IoTHelpers.Gpio.Modules.Dht11HumitureSensor"/>
            /// </summary>
            /// <param name="pinNumber">Pin position on GPIO Border. GPIO is PIN board of controller.</param>
            public DHT11(int pinNumber = DHT11_PIN)
            {
                this.m_sensor = new IoTHelpers.Gpio.Modules.Dht11HumitureSensor(pinNumber, IoTHelpers.ReadingMode.Manual);
                if (this.m_sensor == null)
                {
                    Message = "IoTHelpers.Gpio.Modules.Dht11HumitureSensor intialization (Open Pin) FAILED! \n \n Program Exit!";
                    return;
                }
                this.pinNumber = pinNumber;
            }
            /// <summary>
            /// Create Instance of Native DHT11 pin <see cref="Pin"/>
            /// </summary>
            /// <param name="boardController">Board controller <see cref="GpioRaspberryController"/>.</param>
            /// <param name="pinNumber">Pin position on GPIO Border. GPIO is PIN board of controller.</param>
            public DHT11(IGpioControllerProvider boardController, int pinNumber = DHT11_PIN)
            {
                m_controller = boardController;
                this.nativePin = (Pin)m_controller.OpenPinProvider(pinNumber, ProviderGpioSharingMode.Exclusive);
                if (this.nativePin == null)
                {
                    Message = "Native DHT11 intialization (Open Pin) FAILED! \n \n Program Exit!";
                    return;
                }
                this.nativePin.ValueChanged += NativePin_ValueChanged;
                this.nativePin.SetDriveMode(ProviderGpioPinDriveMode.Input);
            }
            /// <summary>
            /// Initialize getting data from sensor Dht11. By Defualt it takes 60 seconds.
            /// </summary>
            /// <param name="cancellation">Cancellation token.</param>
            public void InitializeGettingMessages(CancellationToken cancellation, bool useNativePin = false)
            {
                try
                {
                    this.Message = "Getting data...\n";
                    Task.WaitAll(new Task[1] { GetHumitureMessages(useNativePin) }, 60000, cancellation);
                }
                catch (Exception ex)
                {
                    LoggerHelper.Log(ex);
                }
            }
            /// <summary>
            /// Create async task for recieveing message from Dht11 pin controller.
            /// </summary>
            /// <param name="timeCollectinggMessages">Time while task will be in loop. 10 s by default.</param>
            /// <param name="useNativePin"> If <c>True</c> runtime.dht11 pin object will be used.</param>
            /// <returns>Colletion of messages rescieved from DHt11 sensor.</returns>
            private async Task<IEnumerable<Message>> GetHumitureMessages(bool useNativePin = false, int timeCollectinggMessages = 15000)
            {

                int start = System.Environment.TickCount;
                List<Message> messges = new List<Microsoft.Azure.Devices.Client.Message>();

                while (true)
                {
                    Message msg = null;
                    if (useNativePin)
                    {
                        IoTHelpers.Runtime.Dht11 dht11 = new IoTHelpers.Runtime.Dht11(this.nativePin.pin); // here can fuckin crash.

                        var readging = await dht11.GetReadingAsync(20).AsTask();
                        msg = GetTelemetryeMessage(readging);
                    }
                    else
                    {
                        var humitire = await m_sensor.GetHumitureAsync();
                        msg = GetTelemetryMessage(humitire);
                    }
                    messges.Add(msg);
                    this.HumitaireMessages = messges;
                    // break loop.
                    var ellapsed = System.Environment.TickCount - start;
                    if (ellapsed >= timeCollectinggMessages)
                        break;
                }
                return messges;
            }
            /// <summary>
            /// Get telemetry data from sensor.
            /// </summary>
            /// <param name="useNativePin">If use native pin. By default <c>True</c></param>
            /// <param name="timeCollectinggMessage">Canceletion time.</param>
            /// <returns>Mesage object <see cref="Microsoft.Azure.Devices.Client.Message"/>.</returns>
            public async Task<Message> GetTelemetryData(bool useNativePin = true, int timeCollectinggMessage = 15000)
            {
                int start = System.Environment.TickCount;
                Message msg;

                while (true)
                {
                    if (useNativePin)
                    {
                        IoTHelpers.Runtime.Dht11 dht11 = new IoTHelpers.Runtime.Dht11(this.nativePin.pin); // here can fuckin crash.

                        var readging = await dht11.GetReadingAsync(20).AsTask();
                        msg = GetTelemetryeMessage(readging);
                    }
                    else
                    {
                        var humitire = await m_sensor.GetHumitureAsync();
                        msg = GetTelemetryMessage(humitire);
                    }

                    // break loop.
                    var ellapsed = System.Environment.TickCount - start;
                    if (ellapsed >= timeCollectinggMessage)
                        break;
                }
                return msg;
            }
            /// <summary>
            /// Current telemetry message of Dht11 pin controller.
            /// </summary>
            /// <returns>Async return of data mesage with current telemetry state of Dht11 pin controller <see cref="Microsoft.Azure.Devices.Message"/>.</returns>
            private Message GetTelemetryMessage(Humiture humiture)
            {
                if (humiture.IsValid)
                {
                    this.Humidity = humiture.Humidity.Value;
                    this.Temperature = humiture.Temperature.Value;
                    this.LastUpdate = DateTime.Now.ToString("YYYY-MM-DDTHH:MM:SS");
                    this.TotalSuccess++;

                    return GetMessage();
                }
                return null;
            }
            /// <summary>
            /// Asynchronously get telemetry message from native pin.
            /// </summary>
            /// <param name="dht11">Runtime Dht11 pin. <see cref="IoTHelpers.Runtime.Dht11"/>.</param>
            private Message GetTelemetryeMessage(Dht11Reading readings)
            {
                if (readings.IsValid)
                {
                    this.Humidity = readings.Humidity;
                    this.Temperature = readings.Temperature;
                    this.LastUpdate = DateTime.Now.ToString("YYYY-MM-DDTHH:MM:SS");
                    this.TotalSuccess++;

                    return GetMessage();
                }
                return null;
            }
            /// <summary>
            /// Get Message from read data recieved from sensor.
            /// </summary>
            /// <returns>Message object for azzure <see cref="Message"/>.</returns>
            private Message GetMessage()
            {
                var telemetryDataPoint = new
                {
                    messageId = DateTime.Now.Millisecond,
                    deviceId = Device.Info.DeviceId,
                    this.Humidity,
                    this.Temperature,
                    this.LastUpdate,
                    this.TotalSuccess
                };
                this.Message += "\nTemperature: " + this.Temperature + " Humidity: " + this.Temperature;

                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                message.Properties.Add("\ntemperatureAlert", (this.Temperature > 30) ? "true" : "false");

                return message;
            }

            #region Sensor telemetry data
            public double Temperature;
            public double Humidity;
            public int TotalSuccess;
            public string LastUpdate;
            #endregion

            #region Implemetation Reading values by bites
            /// <summary>
            /// Setup Native Dht11 sensor.
            /// </summary>
            private void SetUpNativeSensor()
            {
                this.nativePin.SetDriveMode(ProviderGpioPinDriveMode.Output);
                this.nativePin.Write(ProviderGpioPinValue.Low);
                Task.Delay(18).Wait();
                this.nativePin.Write(ProviderGpioPinValue.High);
                Task.Delay(18).Wait();
                this.nativePin.SetDriveMode(ProviderGpioPinDriveMode.Input);
            }
            /// <summary>
            /// Create native reader for values (read bytes vaules)
            /// </summary>
            public void ReadValues()
            {
                int MAX_TIMINGS = 85;
                int[] dht11_dat = new int[5] { 0, 0, 0, 0, 0 };
                dht11_dat[0] = dht11_dat[1] = dht11_dat[2] = dht11_dat[3] = dht11_dat[4] = 0;
                byte counter = 0;
                byte j = 0, i;

                ProviderGpioPinValue laststate = ProviderGpioPinValue.High;

                SetUpNativeSensor();
                for(int k = 0;k < 20; k++)
                {
                    for (i = 0; i < MAX_TIMINGS; i++)
                    {
                        counter = 0;
                        while (this.nativePin.Read() == (ProviderGpioPinValue)laststate)
                        {
                            counter++;
                            Task.Delay(1);
                            if (counter == 255)
                            {
                                break;
                            }
                        }
                        laststate = (ProviderGpioPinValue)this.nativePin.Read();
                        if (counter == 255)
                        {
                            break;
                        }
                        // Skip first 3 samples
                        if ((i >= 4) && (i % 2 == 0))
                        {
                            dht11_dat[j / 8] <<= 1;
                            if (counter > 16)
                                dht11_dat[j / 8] |= 1;
                            j++;
                        }
                    }
                    if ((j >= 40) && (dht11_dat[4] == ((dht11_dat[0] + dht11_dat[1] + dht11_dat[2] + dht11_dat[3]) & 0xFF)))
                    {

                        if (!(dht11_dat[0] == 0) && (dht11_dat[2] == 0))
                        {
                            this.Message += "\n\n" + string.Format("Humidity = {0}.{1} | Temperature = {2}.{3} * C",
                            dht11_dat[0], dht11_dat[1], dht11_dat[2], dht11_dat[3]);
                        }
                    }
                    this.Message += "\n\n" + string.Format("Humidity = {0}.{1} | Temperature = {2}.{3} * C", 0, 0, 0, 0);
                }
            }
            #endregion
            private void NativePin_ValueChanged(IGpioPinProvider sender, GpioPinProviderValueChangedEventArgs args)
            {
                var tmp = (Pin)sender;
                var arguments = args;

                this.Message += string.Format("\nGpio Pin EDGE value: {0}", args.Edge.ToString());
            }
            /// <summary>Dispose all objects.</summary>
            public void Clear()
            {
                this.nativePin.Dispose();
                this.m_sensor.Dispose();

            }
            public event PropertyChangedEventHandler PropertyChanged = null;

            protected void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }

    /// OBSOLATE
    //public class TelemetryData : Azure.IMessage
    //{
    //    public int Id { get; set; }
    //    public DateTime LastUpdate { get; set; }
    //    public MessageType messageType { get => MessageType.Telemetry; }

    //    public double Temperature;
    //    public double Humidity;
    //    public string DeviceId;
    //    public object data
    //    {
    //        get
    //        {
    //            return new
    //            {
    //                DeviceId = this.DeviceId,
    //                MessageId = this.Id,
    //                Temperature = this.Temperature,
    //                Humidity = this.Humidity,
    //                LastUpdate = this.LastUpdate.ToLocalTime()
    //            };
    //        }
    //    }   
        

    //    /// <summary>
    //    /// Create Relemetry data point.
    //    /// </summary>
    //    /// <param name="deviceId">Id of Device.</param>
    //    /// <param name="temperature">Temperature.</param>
    //    /// <param name="humidity">Humidity.</param>
    //    /// <param name="lastUpdate">Last update of data.</param>
    //    public TelemetryData(string deviceId, double temperature, double humidity, DateTime lastUpdate)
    //    {
    //        this.Id = DateTime.Now.Millisecond;
    //        this.DeviceId = deviceId;
    //        this.Temperature = temperature;
    //        this.Humidity = humidity;
    //        this.LastUpdate = lastUpdate;
    //    }
    //}
}

# define DEBUG

using IoT.Azure.Common;
using IoT.Raspberry;
using Windows.UI.Popups;

namespace Raspberry
{
    public class ViewModelDht11 : BindableBase
    {
        #region Fields and properties.
        private double m_temperature;
        private double m_humidity;
        private string m_sensorMsg;
        private string m_lastUpdate;
        /// <summary>Sensor informative character message.</summary>
        public string SensorMessage
        {
            get { return this.m_sensorMsg; }
            set { this.SetProperty<string>(ref this.m_sensorMsg, value); }
        }
        /// <summary>Current Temperature.</summary>
        public double Temeperature
        {
            get { return this.m_temperature; }
            set { this.SetProperty<double>(ref this.m_temperature, value); }
        }
        /// <summary>Current Humidity.</summary>
        public double Humidity
        {
            get { return this.m_humidity; }
            set { this.SetProperty<double>(ref this.m_humidity, value); }
        }
        /// <summary>Last Update.</summary>
        public string LastUpdate
        {
            get { return this.m_lastUpdate; }
            set { this.SetProperty<string>(ref this.m_lastUpdate, value); }
        }
        #endregion

        // Commands
        private DelegateCommand m_updateCommand;
        /// <summary> Update Command delegate. </summary>
        public DelegateCommand UpdateData
        {
            get
            {
                return m_updateCommand ?? new DelegateCommand(UpdateTelemetryData);
            }
        }

        private Sensors.DHT11 dht11;
        /// <summary>Initiate DHT11 View model.</summary>
        public ViewModelDht11()
        {
            GpioRaspberryController controller = new GpioRaspberryController();
            dht11 = new Sensors.DHT11(controller);
#if DEBUG
            dht11.Humidity = 27.0;
            dht11.Temperature = 25.2;
#endif
        }
        /// <summary>
        /// Task to retrieve message from DHT11 Sensor.
        /// </summary>
        /// <returns>Task with telemetry dht11 message. <see cref="Sensors.DHT11.GetMessage"/>.</returns>
        private async void UpdateTelemetryData()
        {
            var data = await dht11.GetTelemetryData();
            this.m_temperature = dht11.Temperature;
            this.m_humidity = dht11.Humidity;
            this.m_lastUpdate = dht11.LastUpdate;
            if (data == null)
            {
                //var messageDialog = new MessageDialog("Data not avaliable", "Info");
                //messageDialog.Commands.Add(new UICommand("Try Again", new UICommandInvokedHandler((v)=> 
                //{
                //    UpdateData.Execute();
                //})));

                //messageDialog.DefaultCommandIndex = 0;
                //messageDialog.CancelCommandIndex = 1;

                //await messageDialog.ShowAsync();
                return;
            }
        }
    }
}

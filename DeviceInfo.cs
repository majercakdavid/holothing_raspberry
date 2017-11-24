using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace IoT.Azure
{
    /// <summary>
    /// Describe Device (raspberry pi 3);
    /// </summary>

    [DataContract]
    internal class Device : IDevice
    {
        private Geolocator geolocator = new Geolocator();
        private float? m_latitude;
        private float? m_longitude;

        private static Device m_info;
        public static Device Info
        {
            get
            {
                if (m_info == null)
                    m_info = new Device();

                return m_info;
            }
        }

        public Device() { }

        //private Geoposition GetLocation()
        //{
        //    return Task.Run<Geoposition>(()=> { return geolocator.GetGeopositionAsync()});
        //}
        #region Implement Azure IDevice
        public double Latitude {
            get
            {
                //if (m_latitude.HasValue)
                //    return m_latitude.Value;
                //else
                //{

                //    m_latitude = (float)pos.Coordinate.Point.Position.Latitude;
                //}
                throw new NotImplementedException();
            }
        }

        public double Longitude => throw new NotImplementedException();

        public string DeviceId => "Maros_Pi";

        public string SharedAccessKeyName => "device";

        public string PrimaryKey => "JkAdHR0NZFlIJotx+v34rGgS69vqcYe5u2wwvYLtPUQ=";

        public string SecondaryKey => "xZKDZuA9aRekI8G3B9YW6XPJ1XN97qFRYdMF8uUDeD8=";

        public string HubHostname => "RescoIoTTest.azure-devices.net";

        public string ConnectionString => "HostName=" + HubHostname + ";DeviceId=" + DeviceId + ";SharedAccessKeyName=" + SharedAccessKeyName + ";SharedAccessKey=" + PrimaryKey;

        public string DeviceName => "raspberry";

        public string IpAddress => "192.168.11.99";

        public string OS => "Windows IoT Core 10.0.16299.15";

        public bool HubEnabledState { get; set; }

        public DeviceState DeviceState { get; set; }

        public string UpdatedTime { get; set; }

        public string Manufacturer => "Unknown";

        public string Model => "Model B Vi 2, 2015";

        public string SerialNumber => "Unknown";

        public string FirmwareVersion => "2015";
        public string InstalledRAM => "1GB";

        public Platform Platform => Platform.Windows;

        public ProccessorType Processor => ProccessorType.ARM;

        public DeviceType DeviceType => DeviceType.Raspberry_Pi_3;
        #endregion
    }
}

using IoT.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.Gpio.Provider;
using Windows.Foundation;

namespace IoT.Raspberry
{
    public class GpioRaspberryController : IGpioControllerProvider, IDisposable
    {
        public int PinCount => 26;

        public List<Pin> Pins;

        public GpioRaspberryController()
        {
            m_controller = GpioController.GetDefault();
        }
        protected GpioController m_controller;

        public static Azure.Metadata Metadata()
        {
            return null;
        }

        #region implement Common instruction what controller can provide , IGpioControllerProvider
        public GpioPin OpenPin(int pinNumber, GpioSharingMode sharingMode = GpioSharingMode.Exclusive)
        {
            GpioPin pin = null;
            GpioOpenStatus status;
            if (m_controller.TryOpenPin(pinNumber, sharingMode, out pin, out status))
            {
                if (Pins == null)
                    Pins = new List<Pin>();

                Pins.Add(new Pin(pinNumber, pin, status));
            }

            return pin;
        }
        /// <summary>
        /// Represents action for general purpose pin provider (I/O);
        /// </summary>
        /// <param name="pinNumber"></param>
        /// <param name="sharingMode"></param>
        /// <returns>Object of <see cref="IGpioPinProvider"/>.</returns>
        public IGpioPinProvider OpenPinProvider(int pinNumber, ProviderGpioSharingMode sharingMode)
        {
            if (Pins == null)
                OpenPin(pinNumber, (GpioSharingMode)sharingMode);

            return (IGpioPinProvider)Pins.FirstOrDefault(x => x.PinNumber == pinNumber);
        }
        /// <summary> Dispose current controller.</summary>
        public void Dispose()
        {
            this.m_controller = null;
        }
        #endregion
    }
}
//    public class Metadata
//    {
//        public Met

//        Thermostat thermostat = new Thermostat();

//        Geolocator geolocator = new Geolocator();
//        Geoposition pos = await geolocator.GetGeopositionAsync();

//        device.Latitude = (float) pos.Coordinate.Point.Position.Latitude;
//        device.Longitude = (float) pos.Coordinate.Point.Position.Longitude;

//        thermostat.DeviceProperties = device;

//            Command TriggerAlarm = new Command();
//        TriggerAlarm.Name = "TriggerAlarm";
//            CommandParameter param = new CommandParameter();
//        param.Name = "Message";
//            param.Type = "String";
//            TriggerAlarm.Parameters = new CommandParameter[] { param
//    };

//    thermostat.Commands = new Command[] { TriggerAlarm
//};

//thermostat.Telemetry = new TelemetryType[] { new TelemetryType { Name = "Temperature", DisplayName = "Temperature", Type = "double" },
//                                                         new TelemetryType { Name = "Humidity", DisplayName = "Humidity", Type = "double" }};
//    }

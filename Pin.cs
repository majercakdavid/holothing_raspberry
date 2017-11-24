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
    /// <summary>
    /// Instance of board contrroler (I/O) pin.
    /// </summary>
    public class Pin : IGpioPinProvider, IDisposable
    {
        public GpioPin pin;
        private GpioOpenStatus status;
        private int position;
        /// <summary>
        /// Crete new (I/O) pin.
        /// </summary>
        /// <param name="index">position on board.</param>
        /// <param name="pin">Pin instance <see cref="GpioPin"/>.</param>
        /// <param name="status">Describe pin status. <see cref="GpioOpenStatus"/>.</param>
        public Pin(int index, GpioPin pin, GpioOpenStatus status)
        {
            this.position = index;
            this.pin = pin;
            this.status = status;
        }
        #region Implement IGpioPinProvider
        /// <summary>
        ///  Gets whether the general-purpose I/O (GPIO) pin supports the specified drive module.
        /// </summary>
        /// <param name="driveMode"></param>
        /// <returns>If<c>True</c>the GPIO pin supports the drive mode that driveMode specifies.</returns>
        public bool IsDriveModeSupported(ProviderGpioPinDriveMode driveMode)
        {
            return this.pin.IsDriveModeSupported((GpioPinDriveMode)driveMode);
        }
        /// <summary>Gets the pin's currently configured drive mode.</summary>
        /// <returns> Pin drive mode.</returns>
        public ProviderGpioPinDriveMode GetDriveMode()
        {
            return (ProviderGpioPinDriveMode)this.pin.GetDriveMode();
        }
        /// <summary>
        ///  Sets the pin's drive mode.
        /// </summary>
        /// <param name="value">Drive Pin Mode <see cref="GpioPinDriveMode"/>.</param>
        public void SetDriveMode(ProviderGpioPinDriveMode value)
        {
            this.pin.SetDriveMode((GpioPinDriveMode)value);
        }
        /// <summary>
        /// Writes a value to the pin.
        /// </summary>
        /// <param name="value">Write Value (0/1) <see cref="GpioPinValue"/>.</param>
        public void Write(ProviderGpioPinValue value)
        {
            this.pin.Write(GpioPinValue.High);
            var val = (GpioPinValue)value;
            this.pin.Write(val);
        }
        /// <summary>
        /// Reads the current value of the pin.
        /// </summary>
        /// <returns>The Pin's value <see cref="GpioPinValue"/>.</returns>
        public ProviderGpioPinValue Read()
        {
            return (ProviderGpioPinValue)this.pin.Read();
        }
        /// <summary>
        /// Dispose Pin object.
        /// </summary>
        public void Dispose()
        {
            this.pin.Dispose();
        }

        /// <summary>
        ///  Gets or sets the debounce timeout for the general-purpose I/O (GPIO) pin, which
        ///  is an interval during which changes to the value of the pin are filtered out and do not generate ValueChanged events.
        /// </summary>
        /// <returns>The debounce timeout for the GPIO pin, which is an interval during which changes
        ///  to the value of the pin are filtered out and do not generate ValueChanged events.
        ///  If the length of this interval is 0, all changes to the value of the pin generate.
        ///</returns>
        public TimeSpan DebounceTimeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <summary>Gets the pin number of the general-purpose I/O (GPIO) pin.</summary>
        public int PinNumber => this.position;
        /// <summary>
        ///  Gets the sharing mode in which the general-purpose I/O (GPIO) pin is open.
        /// </summary>
        public ProviderGpioSharingMode SharingMode => (ProviderGpioSharingMode)this.pin.SharingMode;

        public event TypedEventHandler<IGpioPinProvider, GpioPinProviderValueChangedEventArgs> ValueChanged;
    }
    #endregion
}

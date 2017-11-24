using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Azure
{
    public interface IDevice
    {
        double Latitude { get; }
        double Longitude { get; }
        string DeviceId { get; }
        string SharedAccessKeyName { get; }
        string PrimaryKey { get; }
        string SecondaryKey { get; }
        string HubHostname { get; }
        string ConnectionString { get; }
        DeviceType DeviceType { get; }
        string DeviceName { get; }
        string IpAddress { get; }
        string OS { get; }
        bool HubEnabledState { get; set; }
        DeviceState DeviceState { get; }
        string UpdatedTime { get; set; }
        string Manufacturer { get; }
        string Model { get; }
        string SerialNumber { get; }
        string FirmwareVersion { get; }
        Platform Platform { get; }
        ProccessorType Processor { get; }
        string InstalledRAM { get; }
    }
    public enum DeviceState { ON, OFF,
        ///<summary>Represents time when device is in "sleeping mode" ReadyToListen.</summary> 
        PENDING
    }
    public enum ProccessorType { ARM, x86, x64 }
    /// <summary>Describe what type of device it is.</summary>
    public enum DeviceType { Raspberry_Pi_2, Raspberry_Pi_3, HoloLens }
    public enum Platform { Windows, Android, Debian}
}

using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoT.Azure
{
    /// <summary>Describe Azure device metadata.</summary>
    public class Metadata
    {
        public IDevice Device;
        public bool IsSimulatedDevice;
        public Command[] commands;
    }
    /// <summary>Describe Azure device metadata.</summary>
    public class Command
    {
        public static CommandType Type;
        public static CommandParameter Parameter;
    }
    /// <summary>Describe Azure device metadata.</summary>
    public interface IMessage {
        //int Id { get; set; }
        //DateTime LastUpdate{ get; set; }
        //object data { get; }
        //MessageType messageType { get;}
        Message GetMessage();
        IEnumerable<Message> GetMessages();
    }
    /// <summary>Describe Message type of azzure.</summary>
    public enum MessageType { Telemetry }
    /// <summary>Describe type of command.</summary>
    public enum CommandType { String }
    /// <summary>Command parameter helps to desciribe command type</summary>
    public enum CommandParameter { TriggerAlarm }
}

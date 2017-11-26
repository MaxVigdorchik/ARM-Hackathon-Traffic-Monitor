using System;
using System.Collections.Generic;

namespace DataAnalysis
{
    public class NodeJSON : INodeJSON
    {
        public int NodeID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

    public class DataPacketJSON : IDataPacketJSON
    {
        public int deviceID { get; set; }
        public List<InteractionJSON> interactions { get; set; }
    }

    public class InteractionJSON : IInteractionJSON
    {
        public string start { get; set; } // integer number of ms since epoch
        public double duration { get; set; }
    }

    public class DeviceJSON : IDeviceJSON
    {
        public int DeviceID { get; set; }
        public int NodeAID { get; set; }
        public int NodeBID { get; set; }
        public bool InFlow { get; set; }
    }
}

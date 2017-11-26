using System;

namespace DataAnalysis
{
    public class Device : IDevice
    {
        public int DeviceID { get; set; }
        public IEdge Edge { get; set; }
        public bool InFlow { get; set; }

        public Device(IDeviceJSON deviceJSON)
        {
            INode NodeA = Dictionaries.Nodes[deviceJSON.NodeAID];
            INode NodeB = Dictionaries.Nodes[deviceJSON.NodeBID];

            this.DeviceID = deviceJSON.DeviceID;
            this.Edge = Dictionaries.ReturnIfPresentElseAdd(NodeA, NodeB);
            this.InFlow = deviceJSON.InFlow;
        }
    }
}

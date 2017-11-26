using System;
using System.Collections.Generic;

namespace DataAnalysis
{
    //Dictionary items
    public interface IDevice
    {
        IEdge Edge { get;}
        bool InFlow { get;}
    }

    public interface IEdge
    {
        INode NodeA { get;}
        INode NodeB { get;}

        void Update(bool inFlow, IDataPacketJSON packet);
        double GetWeight();
    }

    public interface INode
    {
        int NodeID { get; }
        double Longitude { get; }
        double Latitude { get; }

        List<IEdge> Edges { get; }
    }

    //JSON formats
    public interface INodeJSON
    {
        int NodeID { get; }
        double Longitude { get; }
        double Latitude { get; }
    }

    public interface IDeviceJSON
    {
        int DeviceID { get; set; }
        int NodeAID { get; set; }
        int NodeBID { get; set; }
        bool InFlow { get; set; }
    }

    public interface IDataPacketJSON
    {
        int deviceID { get; set; }
        List<InteractionJSON> interactions { get; set; }
    }

    public interface IInteractionJSON
    {
        string start { get; set; }
        double duration { get; set; }
    }
}
using System;
using System.Collections.Generic;
using DataAnalysis;

public static class Dictionaries
{
    public static Dictionary<int, IDevice> Devices = new Dictionary<int, IDevice>();
    public static Dictionary<int, INode> Nodes = new Dictionary<int, INode>();
    public static Dictionary<int, IEdge> Edges = new Dictionary<int, IEdge>();

    public static int NextEdgeID = 0;

    public static IEdge ReturnIfPresentElseAdd(INode nodeA, INode nodeB)
    {
        foreach (var edge in Edges.Values)
        {
            if (edge.NodeA.NodeID == nodeA.NodeID && edge.NodeB.NodeID == nodeB.NodeID)
            {
                return edge;
            }
        }

        Edge newEdge = new Edge(nodeA, nodeB);
        Edges.Add(NextEdgeID, newEdge);
        return newEdge;
    }
}

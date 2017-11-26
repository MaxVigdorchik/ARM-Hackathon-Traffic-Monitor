using System;
using System.Collections.Generic;
using DataAnalysis;
using System.Windows.Shapes;

using HSVtoRGB;
using System.Windows.Media;

public static class Dictionaries
{
    public static Dictionary<int, IDevice> Devices = new Dictionary<int, IDevice>();
    public static Dictionary<int, INode> Nodes = new Dictionary<int, INode>();
    public static Dictionary<int, IEdge> Edges = new Dictionary<int, IEdge>();

    public static Dictionary<int, Ellipse> NodeEllipses = new Dictionary<int, Ellipse>();
    public static Dictionary<int, Line> EdgeLines = new Dictionary<int, Line>();

    //Edge Dealing Methods
    public static int NextEdgeID = 0;
    private static double MaxWeighting = 0;

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
        nodeA.Edges.Add(newEdge);

        NextEdgeID++;
        return newEdge;
    }

    private static void SetMaxWeighting()
    {
        double weighting;
        MaxWeighting = 0;

        foreach (var edgeItem in Edges)
        {
            IEdge edgeValue = edgeItem.Value;
            weighting = edgeValue.GetWeight();

            if (weighting > MaxWeighting)
            {
                MaxWeighting = weighting;
            }
        }
    }

    public static void SetColours()
    {
        SetMaxWeighting();
        int MAX_THICKNESS = 9;

        if (MaxWeighting == 0)
        {
            return; // avoid divideByZeroError
        }

        double weighting;
        double ratio;
        Line L;

        foreach (var edgeItem in Edges)
        {
            IEdge edgeValue = edgeItem.Value;
            weighting = edgeValue.GetWeight();
            L = EdgeLines[edgeItem.Key];
            ratio = edgeValue.GetWeight() / MaxWeighting;
            ratio = Math.Sqrt(ratio);

            Color col = HSV2RGB.Convert((float)ratio, 1.0f, 1.0f, 1.0f);

            try
            {
                L.Dispatcher.Invoke(() =>
                {
                    L.Stroke = new SolidColorBrush(col);
                    L.StrokeThickness = MAX_THICKNESS * ratio + 1;
                });
            }
            catch { }          
        }
    }
}

using System;
using System.Collections.Generic;

public class RouteFinding
{
    private int OriginID;
    private int DestinationID;
    private List<NodeItem> PriorityQueue = new List<NodeItem>();

    public RouteFinding(int originID, int destinationID)
    {
        this.OriginID = originID;
        this.DestinationID = destinationID;
    }

    public void DijkstraForwards()
    {

    }
}

public class NodeItem
{
    public int Score { get; set; }
    public int NodeID { get; set; }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalysis
{
    public class Node : INode
    {
        public int NodeID { get; private set; }
        public double Longitude { get; private set; }
        public double Latitude { get; private set; }

        public List<IEdge> Edges {get; set;}

        public Node(INodeJSON n)
        {
            this.NodeID = n.NodeID;
            this.Longitude = n.Longitude;
            this.Latitude = n.Latitude;
        }
    }
}

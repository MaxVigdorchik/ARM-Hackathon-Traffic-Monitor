using System;
using System.Collections.Generic;

using HSVtoRGB;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DataAnalysis
{
    public class Edge : IEdge
    {
        private const double RED_THRESHOLD = 5;
        private const double ORANGE_THRESHOLD = 3;

        private const int MAX_STORE = 5;

        private int CarCount { get; set; }
        private double MeanDuration { get; set; }

        private List<double> Durations { get; set; }
        private double Sum;

        public INode NodeA { get; private set; }
        public INode NodeB { get; private set; }

        public Edge(INode nodeA, INode nodeB)
        {
            Durations = new List<double>();
            CarCount = 0;
            Sum = 0;

            this.NodeA = nodeA;
            this.NodeB = nodeB;
        }

        public void Update(bool inFlow, IDataPacketJSON packet)
        {
            if (inFlow == true)
            {
                CarCount += packet.interactions.Count;
                return;
            }

            else
            {
                foreach (var interaction in packet.interactions)
                {
                    CarCount--;

                    double duration = interaction.duration;
                    Sum += duration;

                    if (Durations.Count == MAX_STORE)
                    {
                        double removed = Durations[0];
                        Durations.RemoveAt(0);
                        Sum -= removed;
                    }

                    Durations.Add(duration);
                    MeanDuration = Sum / Durations.Count;
                }
            }

            IDevice dev = Dictionaries.Devices[packet.deviceID];
            int nodeBID = dev.Edge.NodeB.NodeID;
            Ellipse e = Dictionaries.NodeEllipses[nodeBID];

            try
            {
                e.Dispatcher.Invoke(() =>
                {
                    if (MeanDuration < ORANGE_THRESHOLD)
                    {
                        e.Fill = new SolidColorBrush(Colors.GreenYellow);
                    }
                    else if (MeanDuration < RED_THRESHOLD)
                    {
                        e.Fill = new SolidColorBrush(Colors.Orange);
                    }
                    else
                    {
                        e.Fill = new SolidColorBrush(Colors.Red);
                    }
                });
            }
            catch { }      

            Dictionaries.SetColours();
        }

        public double GetWeight()
        {
            if (CarCount <= 0)
            {
                return MeanDuration;
            }
            else if (MeanDuration <= 0)
            {
                return CarCount;
            }
            else
            {
                return CarCount * MeanDuration;
            }
        }

        public override string ToString()
        {
            string res = "CarCount: " + CarCount + " MeanDur: " + Math.Truncate(MeanDuration * 100) / 100;
            return res;
        }
    }
}

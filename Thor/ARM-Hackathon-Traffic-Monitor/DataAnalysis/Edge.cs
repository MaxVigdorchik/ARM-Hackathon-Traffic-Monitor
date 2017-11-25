using System;
using System.Collections.Generic;

namespace DataAnalysis
{
    public class Edge : IEdge
    {
        private const int MAX_STORE = 100;

        private int CarCount { get; set; }
        private double MeanDuration { get; set; }

        private List<double> Durations { get; set; }
        private double Sum;
        private bool Looped;

        public INode NodeA { get; private set; }
        public INode NodeB { get; private set; }

        public Edge(INode nodeA, INode nodeB)
        {
            Durations = new List<double>();
            Sum = 0;
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

                    double duration = (interaction.end - interaction.start).TotalSeconds;
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
        }

        public double GetWeight()
        {
            return CarCount * MeanDuration;
        }
    }
}

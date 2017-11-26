﻿using System;
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

            double ratio = Math.Log(MeanDuration * 1000) / Math.Log(100000);
            IDevice dev = Dictionaries.Devices[packet.deviceID];
            int nodeBID = dev.Edge.NodeB.NodeID;
            //add code to change colour of node

            Dictionaries.SetColours();
        }

        public double GetWeight()
        {
            return CarCount * MeanDuration;
        }
    }
}

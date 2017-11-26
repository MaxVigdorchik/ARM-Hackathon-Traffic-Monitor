using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.IO;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using System.ComponentModel;

namespace DataAnalysis
{
    public static class Retrieve
    {
        private static JavaScriptSerializer Ser = new JavaScriptSerializer(); // Javascrit serializer
        private static MqttClient Client = new MqttClient("mqtt.ntomi.me"); // GET RIGHT IP ADDRESS
        private static string ClientAddress = "ClientMachine"; // figure out if right

        private static TextBox PacketBox;

        static string TrafficTopic = "Valhalla/Traffic"; // decide exact later
        static string GraphTopic = "Valhalla/Graph";
        static string TestTopic = "Mbed";

        static string LogFilePath = "packets.log";

        public static void Initialise(TextBox packetBox, bool smallGraph)
        {
            PacketBox = packetBox;
            CreateGraph(smallGraph);
            SubscribeToMQTT();
        }

        //graph creation
        private static void CreateGraph(bool smallGraph)
        {
            string NodesPath, DevicesPath;

            if (smallGraph)
            {
                NodesPath = "../../../../../queue_sim/nodes.json";
                DevicesPath = "../../../../../queue_sim/devices.json";
            }
            else
            {
                NodesPath = "nodes.json";
                DevicesPath = "devices.json";
            }

            string NodesJSON = File.ReadAllText(NodesPath);
            string DevicesJSONString = File.ReadAllText(DevicesPath);

            List<NodeJSON> nodesList = Ser.Deserialize<List<NodeJSON>>(NodesJSON);
            List<DeviceJSON> deviceList = Ser.Deserialize<List<DeviceJSON>>(DevicesJSONString);

            foreach (var nodeJSON in nodesList)
            {
                Dictionaries.Nodes.Add(nodeJSON.NodeID, new Node(nodeJSON));
            }

            foreach (var device in deviceList)
            {
                Dictionaries.Devices.Add(device.DeviceID, new Device(device));
            }
        }

        //MQTT stuff
        private static void SubscribeToMQTT()
        {
            string[] topics = new string[1];
            topics[0] = TrafficTopic;
            //topics[1] = GraphTopic;
            //topics[0] = TestTopic;

            byte[] qosLevels = new byte[1];
            qosLevels[0] = (byte)1;

            Client.Connect(ClientAddress);
            Client.Subscribe(topics, qosLevels);

            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = System.Text.Encoding.Default.GetString(e.Message); // retrieves message                   

            if (e.Topic == TrafficTopic)
            {           
                List<DataPacketJSON> packetList = Ser.Deserialize<List<DataPacketJSON>>(message); // converts from json
                if(packetList.Count == 0)
                {
                    packetList.Add(Ser.Deserialize<DataPacketJSON>(message));
                }
                else
                {
                    packetList = Shuffle(packetList);
                }

                PacketBox.Dispatcher.Invoke(() =>
                {
                    PacketBox.Text = Convert2String(packetList[0]);
                });

                foreach (var packet in packetList)
                {
                    IDevice device = Dictionaries.Devices[packet.deviceID];
                    bool inFlow = device.InFlow;
                    device.Edge.Update(inFlow, packet);
                }
            }
        }

        private static string Convert2String(IDataPacketJSON packet)
        {
            string rep = "[---RECEIVED PACKET @ " + DateTime.Now + " ---]\n";
            rep += "{\n";
            rep += "    deviceID: " + packet.deviceID + "\n";
            rep += "    interactions: [ \n";
            foreach (var interaction in packet.interactions)
            {
                rep += "    {\n";
                rep += "        start: " + interaction.start + "\n";
                rep += "        duration: " + interaction.duration + "\n";
                rep += "    },\n";
            }
            rep += "]\n";
            rep += "}\n";
            return rep;
        }

        public static void ReadFromFile(bool smallGraph)
        {
            string trafficFilePath;
            if (smallGraph)
            {
                trafficFilePath = "../../../../../queue_sim/traffic.json";
            }
            else
            {
                trafficFilePath = "packets.json";
            }

            string PacketsData = File.ReadAllText(trafficFilePath);
            List<DataPacketJSON> packets = Ser.Deserialize<List<DataPacketJSON>>(PacketsData);

            packets = Shuffle(packets);

            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += Bgw_DoWork;
            bgw.RunWorkerAsync(packets);           
        }

        private static void Bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            List<DataPacketJSON> packets = e.Argument as List<DataPacketJSON>;
            IDevice device;
            int i = 0;

            foreach (var packet in packets)
            {
                i++;
                if (i % 50 == 0)
                {
                    try
                    {
                        PacketBox.Dispatcher.Invoke(() =>
                        {
                            PacketBox.Text = Convert2String(packet);
                        });
                    }
                    catch { }
                }

                device = Dictionaries.Devices[packet.deviceID];
                bool inFlow = device.InFlow;
                device.Edge.Update(inFlow, packet);
            }
        }

        public static List<T> Shuffle<T>(List<T> list)
        {
            Random rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }

    public static class WriteToFile
    {
        private static JavaScriptSerializer Ser = new JavaScriptSerializer(); // Javascrit serializer

        public static void Write()
        {
            
            int rows = 10;
            int columns = 20;
            int N = rows * columns;

            double randomNessFactor = 2;

            NodeJSON node;
            List<NodeJSON> nodeList = new List<NodeJSON>();

            Random rand = new Random();

            for(int n = 0; n < N; n++)
            {
                node = new NodeJSON();
                node.NodeID = n;
                node.Longitude = n / rows + rand.NextDouble() / randomNessFactor;
                node.Latitude = n % rows + rand.NextDouble() / randomNessFactor;
                nodeList.Add(node);         
            }

            string nodeString = Ser.Serialize(nodeList);
            File.WriteAllText("nodes.json", nodeString);

            int idCounter = 0;
            DeviceJSON device;
            List<DeviceJSON> deviceList = new List<DeviceJSON>();

            for(int n = 0; n < N; n++)
            {
                if (n - 1 >= 0 && n % rows != 0)
                {
                    device = new DeviceJSON();
                    device.DeviceID = idCounter;
                    device.NodeAID = n;
                    device.NodeBID = n - 1;
                    device.InFlow = true;
                    idCounter++;

                    deviceList.Add(device);

                    device = new DeviceJSON();
                    device.DeviceID = idCounter;
                    device.NodeAID = n;
                    device.NodeBID = n - 1;
                    device.InFlow = false;
                    idCounter++;

                    deviceList.Add(device);
                }

                if (n + 1 < N && (n + 1) % rows != 0)
                {
                    device = new DeviceJSON();
                    device.DeviceID = idCounter;
                    device.NodeAID = n;
                    device.NodeBID = n + 1;
                    device.InFlow = true;
                    idCounter++;

                    deviceList.Add(device);

                    device = new DeviceJSON();
                    device.DeviceID = idCounter;
                    device.NodeAID = n;
                    device.NodeBID = n + 1;
                    device.InFlow = false;
                    idCounter++;

                    deviceList.Add(device);
                }

                if (n + rows < N)
                {
                    device = new DeviceJSON();
                    device.DeviceID = idCounter;
                    device.NodeAID = n;
                    device.NodeBID = n + rows;
                    device.InFlow = true;
                    idCounter++;

                    deviceList.Add(device);

                    device = new DeviceJSON();
                    device.DeviceID = idCounter;
                    device.NodeAID = n;
                    device.NodeBID = n + rows;
                    device.InFlow = false;
                    idCounter++;

                    deviceList.Add(device);
                }

                if (n - rows >= 0)
                {
                    device = new DeviceJSON();
                    device.DeviceID = idCounter;
                    device.NodeAID = n;
                    device.NodeBID = n - rows;
                    device.InFlow = true;
                    idCounter++;

                    deviceList.Add(device);

                    device = new DeviceJSON();
                    device.DeviceID = idCounter;
                    device.NodeAID = n;
                    device.NodeBID = n - rows;
                    device.InFlow = false;
                    idCounter++;

                    deviceList.Add(device);
                }
            }

            string deviceString = Ser.Serialize(deviceList);
            File.WriteAllText("devices.json", deviceString);

            DataPacketJSON packet;
            InteractionJSON interaction;
            List<DataPacketJSON> packetList = new List<DataPacketJSON>();
            int packetNumber = 10000;

            for (int i = 0; i < packetNumber; i += 3)
            {
                packet = new DataPacketJSON();
                packet.deviceID = i % deviceList.Count;
                packet.interactions = new List<InteractionJSON>();

                for(int j = 0; j < rand.Next(10); j++)
                {
                    interaction = new InteractionJSON();
                    interaction.start = DateTime.Now.ToShortTimeString();
                    interaction.duration = rand.NextDouble() * 6;

                    packet.interactions.Add(interaction);
                }

                packetList.Add(packet);
            }

            string packetString = Ser.Serialize(packetList);
            File.WriteAllText("packets.json", packetString);
        }
    }
}

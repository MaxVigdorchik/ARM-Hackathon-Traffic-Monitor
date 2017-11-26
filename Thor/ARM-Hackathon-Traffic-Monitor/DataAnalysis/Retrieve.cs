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

        public static void Initialise(TextBox packetBox)
        {
            PacketBox = packetBox;
            CreateGraph();
            SubscribeToMQTT();
        }

        //graph creation
        private static void CreateGraph()
        {
            string NodesPath = "../../../../../queue_sim/nodes.json";
            string DevicesPath = "../../../../../queue_sim/devices.json";

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
            //topics[0] = TrafficTopic;
            //topics[1] = GraphTopic;
            topics[0] = TestTopic;

            byte[] qosLevels = new byte[1];
            qosLevels[0] = (byte)1;

            Client.Connect(ClientAddress);
            Client.Subscribe(topics, qosLevels);

            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string message = System.Text.Encoding.Default.GetString(e.Message); // retrieves message        
            PacketBox.Dispatcher.Invoke(() =>
            {
                PacketBox.Text = message;
            });

            if (e.Topic == TrafficTopic)
            {
                StreamWriter w = new StreamWriter(LogFilePath);
                w.WriteLine(message);                

                IDataPacketJSON packet = Ser.Deserialize<DataPacketJSON>(message); // converts from json
                IDevice device = Dictionaries.Devices[packet.deviceID];
                bool inFlow = device.InFlow;
                device.Edge.Update(inFlow, packet);
            }
        }

        public static void ReadFromFile()
        {
            string trafficFilePath = "../../../../../queue_sim/traffic.json";
            string PacketsData = File.ReadAllText(trafficFilePath);
            List<DataPacketJSON> packets = Ser.Deserialize<List<DataPacketJSON>>(PacketsData);
            IDevice device;

            foreach (var packet in packets)
            {
                device = Dictionaries.Devices[packet.deviceID];
                bool inFlow = device.InFlow;
                device.Edge.Update(inFlow, packet);
            }
        }
    }
}

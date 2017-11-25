using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Script.Serialization;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace DataAnalysis
{
    public static class Retrieve
    {
        private static JavaScriptSerializer Ser = new JavaScriptSerializer(); // Javascrit serializer
        private static MqttClient Client = new MqttClient("mqtt.ntomi.me"); // GET RIGHT IP ADDRESS
        private static string ClientAddress = "ClientMachine"; // figure out if right

        static string TrafficTopic = "Valhalla/Traffic"; // decide exact later
        static string GraphTopic = "Valhalla/Graph";

        public static void Initialise()
        {
            CreateGraph();
            SubscribeToMQTT();
        }

        //graph creation
        private static void CreateGraph()
        {
            string NodesJSON = "PLEASE REPLACE"; // use right method of retrieval
            string DevicesJSONString = "PLEASE CHANGE ME";

            List<INodeJSON> nodesList = Ser.Deserialize<List<INodeJSON>>(NodesJSON);
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
            string[] topics = new string[2];
            topics[0] = TrafficTopic;
            topics[1] = GraphTopic;

            byte[] qosLevels = new byte[1];
            qosLevels[0] = (byte)2;

            Client.Connect(ClientAddress);
            Client.Subscribe(topics, qosLevels);

            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }

        private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            if (e.Topic == TrafficTopic)
            {
                string message = System.Text.Encoding.Default.GetString(e.Message); // retrieves message
                IDataPacketJSON packet = Ser.Deserialize<DataPacketJSON>(message); // converts from json
                IDevice device = Dictionaries.Devices[packet.deviceID];
                bool inFlow = device.InFlow;

                device.Edge.Update(inFlow, packet);
            }
        }
    }
}

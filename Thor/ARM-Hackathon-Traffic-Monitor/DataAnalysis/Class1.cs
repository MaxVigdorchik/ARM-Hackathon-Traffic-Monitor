using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace DataAnalysis
{
    interface IJsonPacket
    {
        int deviceID { get; set; }
        List<Interaction> interactions { get; set; }
    }

    interface IInteraction
    {
        DateTime startTime { get; set; }
        double durationInSeconds { get; set; }
    }

    public class Retrieve
    {
        private static HttpClient Client = new HttpClient(); // declared static to prevent clogging up of ports
        private static JavaScriptSerializer Ser = new JavaScriptSerializer(); // Javascrit serializer

        public List<IJsonPacket> RetrieveData()
        {
            string request = "InsertURLHere";
            string dataString = Client.GetStringAsync(request).Result;
            List<IJsonPacket> 
        }
    }

    public class JsonPacket : IJsonPacket
    {
        public int deviceID { get; set; }
        public List<Interaction> interactions { get; set; }        
    }

    public class Interaction : IInteraction
    {
        public DateTime startTime { get; set; }
        public double durationInSeconds { get; set; }
    }
}

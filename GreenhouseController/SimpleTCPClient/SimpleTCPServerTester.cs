using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleApplication1
{
    // Adapted from:
    // http://csharp.net-informations.com/communications/csharp-client-socket.htm
    class SimpleTCPServerTest
    {
        static void Main(string[] args)
        {

            TcpListener serverListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            
            TcpClient client = default(TcpClient);
            serverListener.Start();
            Console.WriteLine(" >> Server Started");
            client = serverListener.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");
            NetworkStream networkStream = client.GetStream();
            
            while ((true))
            {
                try
                {
                    JsonSpoof jSpoof = new JsonSpoof();
                    
                    byte[] bytesFrom = new byte[10025];
                    int[] zones = new int[5] { 1, 2, 3, 4, 5 };
                    foreach (int zone in zones)
                    {
                        string json = jSpoof.SpoofGreenhouseData(zone);
                        byte[] sendBytes = Encoding.ASCII.GetBytes(json);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                        Console.WriteLine(" >> " + $"{json}");

                        Thread.Sleep(3000);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            client.Close();
            serverListener.Stop();
            Console.WriteLine(" >> exit");
            Console.ReadLine();
        }

        public class JsonSpoof
        {
            public class Packet
            {
                public int zone;
                public double temperature;
                public double humidity;
                public double light;

                public int tempHi;
                public int tempLo;
                public int lightHi;
                public int lightLo;
                public int humidHi;
                public int humidLo;
            }
            
            public JsonSpoof() { }
            public string SpoofGreenhouseData(int zone)
            {
                int tempMin = 0;
                int tempMax = 120;
                int humidMin = 0;
                int humidMax = 100;
                int lightMin = 0;
                int lightMax = 98000;
                Random rand = new Random();

                Packet pack = new Packet()
                {
                    zone = zone,
                    temperature = rand.Next(tempMin, tempMax),
                    humidity = rand.Next(humidMin, humidMax),
                    light = rand.Next(lightMin, lightMax),
                    tempHi = 80,
                    tempLo = 65
                };

                string spoofData = JsonConvert.SerializeObject(pack);

                return spoofData;
            }
        }
    }
}
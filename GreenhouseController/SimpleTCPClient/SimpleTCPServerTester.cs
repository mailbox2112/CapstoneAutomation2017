using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GreenhouseController;

namespace ConsoleApplication1
{
    // Adapted from:
    // http://csharp.net-informations.com/communications/csharp-client-socket.htm
    class SimpleTCPServerTest
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000);
            TcpListener serverListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            
            TcpClient client = default(TcpClient);
            serverListener.Start();
            Console.WriteLine(" >> Server Started");
            client = serverListener.AcceptTcpClient();
            Console.WriteLine(" >> Accept connection from client");
            NetworkStream networkStream = client.GetStream();

            int[] zones = new int[] { 1, 2, 3, 4, 5 };
            byte[] bytesFrom = new byte[1024];

            
            {
                try
                {
                    JsonSpoof jSpoof = new JsonSpoof();
                    
                    foreach (int zone in zones)
                    {
                        string json = jSpoof.SpoofGreenhouseData(zone);
                        byte[] sendBytes = Encoding.ASCII.GetBytes(json);
                        networkStream.Write(sendBytes, 0, sendBytes.Length);
                        networkStream.Flush();
                        Console.WriteLine(" >> " + $"{json}");

                        Thread.Sleep(500);
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

        internal class JsonSpoof
        {
            
            public JsonSpoof() { }
            public string SpoofGreenhouseData(int zone)
            {
                int tempMin = 0;
                int tempMax = 120;
                int humidMin = 0;
                int humidMax = 100;
                int lightLim = 40000;
                int moistLim = 30;
                Random rand = new Random();

                DataPacket pack = new DataPacket()
                {
                    Zone = zone,
                    Temperature = 100,//rand.Next(tempMin, tempMax),
                    Humidity = 50,//rand.Next(humidMin, humidMax),
                    Light = 60000,//rand.Next(0, 100000),
                    Moisture = 40,//rand.Next(10, 100),
                    TempHi = 80,
                    TempLo = 65,
                    LightLim = lightLim,
                    MoistLim = moistLim
                };

                string spoofData = JsonConvert.SerializeObject(pack);

                return spoofData;
            }
        }
    }
}
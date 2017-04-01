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
using GreenhouseController.Data;
using GreenhouseController.Packets;

namespace ConsoleApplication1
{
    // Adapted from:
    // http://csharp.net-informations.com/communications/csharp-client-socket.htm
    public class SimpleTCPServerTest
    {
        static void Main(string[] args)
        {
            Thread.Sleep(1000);
            TcpListener serverListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            
            TcpClient client = default(TcpClient);
            serverListener.Start();

            int[] tlhZones = new int[] { 1, 2, 3, 4, 5 };
            int[] mZones = new int[] { 1, 2, 3, 4, 5, 6 };

            Dictionary<int, DateTime> waterStart = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> waterEnd = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> lightStart = new Dictionary<int, DateTime>();
            Dictionary<int, DateTime> lightEnd = new Dictionary<int, DateTime>();
            foreach(int zone in tlhZones)
            {
                lightStart.Add(zone, new DateTime(2017, 3, 23, 20, 0, 0));
                lightEnd.Add(zone, new DateTime(2017, 3, 23, 20, 50, 0));
            }

            foreach(int zone in mZones)
            {
                waterStart.Add(zone, new DateTime(2017, 3, 23, 20, 0, 0));
                waterEnd.Add(zone, new DateTime(2017, 3, 23, 20, 50, 0));
            }
            
            byte[] limitsToSend = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(new LimitPacket()
            {
                TempHi = 80,
                TempLo = 65,
                WaterStarts = waterStart,
                WaterEnds = waterEnd,
                LightStarts = lightStart,
                LightEnds = lightEnd,
                ShadeLim = 50000
            }, Formatting.Indented));

            // TODO: add ability to change greenhouse limits
            Console.WriteLine("Would you like to use manual or random mode? Press M for manual, R for random.");
            var key = Console.ReadLine();
            Console.WriteLine();
            if (key == "m" || key == "M")
            {

            }
            else if (key == "R" || key == "r")
            {
                #region Random Data
                byte[] buffer = new byte[1024];
                while(true)
                {
                    Console.WriteLine("Accepting connection...");
                    client = serverListener.AcceptTcpClient();
                    Console.WriteLine("Connection accepted...");
                    NetworkStream networkStream = client.GetStream();

                    networkStream.Read(buffer, 0, buffer.Length);
                    string received = JsonConvert.DeserializeObject<string>(Encoding.ASCII.GetString(buffer));
                    Array.Clear(buffer, 0, buffer.Length);
                    if (received == "TLH")
                    {
                        Console.WriteLine("Request for data received!");
                        try
                        {
                            List<TLHPacket> jspoofs = new List<TLHPacket>();
                            JsonSpoof jSpoof = new JsonSpoof();

                            foreach (int zone in tlhZones)
                            {
                                TLHPacket packet = jSpoof.TLHData(zone);
                                jspoofs.Add(packet);
                                Console.WriteLine($"{packet}");
                            }

                            TLHPacketContainer container = new TLHPacketContainer() { Packets = jspoofs };
                            string json = JsonConvert.SerializeObject(container);
                            byte[] sendBytes = Encoding.ASCII.GetBytes(json);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();

                            Console.WriteLine($"{json}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else if (received == "MOISTURE")
                    {
                        Console.WriteLine("Request for data received!");
                        try
                        {
                            List<MoisturePacket> jspoofs = new List<MoisturePacket>();
                            JsonSpoof jSpoof = new JsonSpoof();

                            foreach (int zone in mZones)
                            {
                                MoisturePacket packet = jSpoof.MoistureData(zone);
                                jspoofs.Add(packet);
                                Console.WriteLine($"{packet}");
                            }

                            MoisturePacketContainer container = new MoisturePacketContainer() { Packets = jspoofs };
                            string json = JsonConvert.SerializeObject(container);
                            byte[] sendBytes = Encoding.ASCII.GetBytes(json);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();

                            Console.WriteLine($"{json}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                    else if (received == "MANUAL")
                    {
                        ManualPacket packet = new ManualPacket()
                        {
                            ManualCool = null,
                            ManualHeat = null,
                            ManualLight = null,
                            ManualWater = null
                        };
                        string manual = JsonConvert.SerializeObject(packet);
                        byte[] manualBytes = Encoding.ASCII.GetBytes(manual);
                        networkStream.Write(manualBytes, 0, manualBytes.Length);
                        networkStream.Flush();

                        Console.WriteLine($"{manual}");
                    }
                    else if (received == "LIMITS")
                    {
                        networkStream.Write(limitsToSend, 0, limitsToSend.Length);
                        networkStream.Flush();
                    }
                }
                #endregion
            }
            else
            {
                Console.WriteLine("Invalid character, exiting!");
            }
            client.Close();
            serverListener.Stop();
            Console.WriteLine("Exiting...");
        }

        internal class JsonSpoof
        {
            public Random rand = new Random();
            public JsonSpoof() { }
            public TLHPacket TLHData(int zone)
            {
                int tempMin = 50;
                int tempMax = 120;
                int humidMin = 0;
                int humidMax = 100;
                TLHPacket packet = new TLHPacket()
                {
                    Temperature = rand.Next(tempMin, tempMax),
                    Light = rand.Next(20000, 65000),
                    Humidity = rand.Next(humidMin, humidMax),
                    ID = zone
                };

               
                return packet;
            }

            public MoisturePacket MoistureData(int zone)
            {
                Random rand = new Random();
                MoisturePacket packet = new MoisturePacket()
                {
                    ID = zone,
                    Probe1 = rand.Next(0,100),
                    Probe2 = rand.Next(0, 100),
                };

                return packet;
            }
        }
    }
}
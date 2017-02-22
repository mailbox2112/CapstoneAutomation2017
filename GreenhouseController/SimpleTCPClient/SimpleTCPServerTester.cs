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

            Console.WriteLine("Would you like to use manual or random mode? Press M for manual, R for random.");
            var key = Console.ReadLine();
            Console.WriteLine();
            if (key == "m" || key == "M")
            {
                #region Manual Controls
                List<int> zones = new List<int>() { 1, 2, 3, 4, 5 };
                List<DataPacket> packetsToSend = new List<DataPacket>();
                Console.WriteLine("Manual mode selected. Currently, the following commands are supported:");
                Console.WriteLine("Q to quit.");
                Console.WriteLine("H for heating.");
                Console.WriteLine("C for cooling.");
                Console.WriteLine("L for lighting.");
                Console.WriteLine("W for watering.");

                bool heat = false;
                bool cool = false;
                bool light = false;
                bool water = false;
                bool stop = false;
                bool invalidCommand = false;
                int tempHi;
                int tempLo;
                int lightLim;
                int moistLim;
                string command = null;
                while(stop == false)
                {
                    tempHi = 0;
                    tempLo = 0;
                    lightLim = 0;
                    moistLim = 0;
                    invalidCommand = false;
                    command = Console.ReadLine();
                    Console.WriteLine();
                    foreach(char c in command)
                    {
                        if (c == 'h' || c == 'H')
                        {
                            if (cool != true)
                            {
                                heat = !heat;
                            }
                            else
                            {
                                Console.WriteLine("Invalid command, cannot heat and cool simultaneously! Please try again.");
                                heat = false;
                                cool = false;
                                light = false;
                                water = false;
                                invalidCommand = true;
                                break;
                            }
                        }
                        else if (c == 'c' || c == 'C')
                        {
                            if (heat != true)
                            {
                                cool = !cool;
                            }
                            else
                            {
                                Console.WriteLine("Invalid command, cannot heat and cool simultaneously! Please try again.");
                                heat = false;
                                cool = false;
                                light = false;
                                water = false;
                                invalidCommand = true;
                                break;
                            }
                        }
                        else if (c == 'l' || c == 'L')
                        {
                            light = !light;
                        }
                        else if (c == 'w' || c == 'W')
                        {
                            water = !water;
                        }
                        else if (command == "q" || command == "Q")
                        {
                            stop = true;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid command {command}, please press one of the following keys:");
                            Console.WriteLine("Q to quit.");
                            Console.WriteLine("H for heating.");
                            Console.WriteLine("C for cooling.");
                            Console.WriteLine("L for lighting.");
                            Console.WriteLine("W for watering.");
                        }
                    }
                    if (invalidCommand == false)
                    {
                        if (heat == true)
                        {
                            tempHi = 1000;
                            tempLo = 150;

                        }
                        else
                        {
                            if (cool != true)
                            {
                                tempHi = 100;
                                tempLo = 0;
                            }
                        }
                        if (cool == true)
                        {
                            tempLo = 0;
                            tempHi = 10;
                        }
                        else
                        {
                            if (heat != true)
                            {
                                tempHi = 100;
                                tempLo = 0;
                            }
                        }
                        if (light == true)
                        {
                            lightLim = 100000;
                        }
                        else
                        {
                            lightLim = 0;
                        }
                        if (water == true)
                        {
                            moistLim = 150;
                        }
                        else
                        {
                            moistLim = 0;
                        }
                        for (int i = 1; i < 6; i++)
                        {
                            packetsToSend.Add(
                                new DataPacket()
                                {
                                    Zone = i,
                                    Humidity = 50,
                                    Temperature = 50,
                                    Light = 50,
                                    Moisture = 50,
                                    LightLim = lightLim,
                                    MoistLim = moistLim,
                                    TempHi = tempHi,
                                    TempLo = tempLo
                                });
                        }
                        foreach (var packet in packetsToSend)
                        {
                            string pack = JsonConvert.SerializeObject(packet);
                            byte[] sendBytes = Encoding.ASCII.GetBytes(pack);
                            networkStream.Write(sendBytes, 0, sendBytes.Length);
                            networkStream.Flush();
                            Console.WriteLine(" >> " + $"{pack}");
                            Thread.Sleep(500);
                        }
                        packetsToSend.Clear();
                    }
                }
                #endregion
            }
            else if (key == "r" || key == "R")
            {
                int[] zones = new int[] { 1, 2, 3, 4, 5 };
                byte[] bytesFrom = new byte[1024];

                while ((true))
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
                    Thread.Sleep(20000);
                }
            }
            else
            {
                Console.WriteLine("Invalid character, restarting!");
            }
            client.Close();
            serverListener.Stop();
            Console.WriteLine(" >> exit");
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
                    Temperature = rand.Next(tempMin, tempMax),
                    Humidity = rand.Next(humidMin, humidMax),
                    Light = rand.Next(0, 100000),
                    Moisture = rand.Next(10, 100),
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
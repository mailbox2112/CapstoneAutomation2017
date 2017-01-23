using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class GreenhouseDataProducer
    {
        private static volatile GreenhouseDataProducer instance;
        private static object syncRoot = new Object();

        private Socket _cloudConnection;
        private IPEndPoint _cloudEndpoint;
        private IPAddress _cloudIp;
        

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint"></param>
        /// <param name="hostAddress"></param>
        private GreenhouseDataProducer(IPAddress hostAddress, IPEndPoint hostEndpoint)
        {
            Console.WriteLine("Constructing data producer...");
            _cloudIp = hostAddress;
            _cloudEndpoint = hostEndpoint;
            _cloudConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Data producer constructed.");
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static GreenhouseDataProducer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock(syncRoot)
                    {
                        if (instance == null)
                        {
                            // TODO: put actual network locations in here!
                            instance = new GreenhouseDataProducer(null, null);
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Request greenhouse data from the server
        /// </summary>
        public void RequestAndReceiveGreenhouseData(BlockingCollection<byte[]> target)
        {
            while (true)
            {
                try
                {
                    //Console.WriteLine("Attempting to connect to server...");
                    //_cloudConnection.Connect(_cloudEndpoint);
                    //Console.WriteLine("Successfully connected to server.");

                    // TODO: send actual command packet!
                    byte[] buffer = new byte[1];

                    Random rand = new Random();

                    rand.NextBytes(buffer);
                    // TODO: get actual data
                    //Console.WriteLine("Attempting to receive greenhouse data...");
                    //_cloudConnection.Receive(buffer, SocketFlags.None);
                    //Console.WriteLine("Successfully received greenhouse data.");

                    target.TryAdd(buffer);
                    //_cloudConnection.Dispose();
                    Thread.Sleep(3000);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //_cloudConnection.Dispose();
                }
            }
        }
    }
}

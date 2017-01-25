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
        private static volatile GreenhouseDataProducer _instance;
        private static object syncRoot = new Object();

        //private Socket _dataProviderConnection;
        //private IPEndPoint _dataProviderEndpoint;
        //private IPAddress _dataProviderIp;
        private NetworkStream _dataStream;
        private TcpClient _client;
        

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint">Endpoint to be reached</param>
        /// <param name="hostAddress">IP address we're trying to connect to</param>
        private GreenhouseDataProducer(IPAddress hostAddress, IPEndPoint hostEndpoint)
        {
            Console.WriteLine("Constructing data producer...");
            //_dataProviderIp = hostAddress;
            //_dataProviderEndpoint = hostEndpoint;
            //_dataProviderConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //_dataProviderConnection.Connect(hostEndpoint);
            //_dataStream = new NetworkStream(_dataProviderConnection);

            _client = new TcpClient();
            _client.Connect("127.0.0.1", 8888);
            _dataStream = _client.GetStream();
            Console.WriteLine("Data producer constructed.\n");
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static GreenhouseDataProducer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(syncRoot)
                    {
                        if (_instance == null)
                        {
                            // TODO: put actual network locations in here!
                            _instance = new GreenhouseDataProducer(IPAddress.Parse("127.0.0.1"), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80));
                        }
                    }
                }
                return _instance;
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

                    //Console.WriteLine("Successfully connected to server.");

                    // TODO: send actual command packet!
                    if (_dataStream.DataAvailable)
                    {
                        byte[] buffer = new byte[10025];
                        _dataStream.ReadAsync(buffer, 0, buffer.Length);

                        target.TryAdd(buffer);

                        Thread.Sleep(3000);
                    }


                    //Random rand = new Random();

                    //rand.NextBytes(buffer);
                    // TODO: get actual data
                    //Console.WriteLine("Attempting to receive greenhouse data...");
                    //_dataProviderConnection.Receive(buffer, SocketFlags.None);
                    //Console.WriteLine("Successfully received greenhouse data.");


                    //_dataProviderConnection.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    //_cloudConnection.Dispose();
                    _dataStream.Dispose();
                }
            }
        }
    }
}

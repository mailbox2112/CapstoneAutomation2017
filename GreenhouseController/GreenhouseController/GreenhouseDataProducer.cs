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
    public class GreenhouseDataProducer
    {
        private static volatile GreenhouseDataProducer _instance;
        private static object syncRoot = new object();
        
        private byte[] _buffer = new byte[1024];
        private byte[] _tempBuffer = new byte[1024];
        private NetworkStream _dataStream;
        private TcpClient _client;
        public event EventHandler<DataEventArgs> ItemInQueue;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint">Endpoint to be reached</param>
        /// <param name="hostAddress">IP address we're trying to connect to</param>
        private GreenhouseDataProducer()
        {
            Console.WriteLine("Constructing data producer...");

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
                            _instance = new GreenhouseDataProducer();
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
                    // The read command is blocking, so this just waits until data is available
                    _dataStream.Read(_buffer, 0, _buffer.Length);
                    Array.Copy(sourceArray: _buffer, destinationArray: _tempBuffer, length: _buffer.Length);

                    target.TryAdd(_tempBuffer);
                    EventHandler<DataEventArgs> handler = ItemInQueue;
                    handler(this, new DataEventArgs() { Buffer = target });
                    Array.Clear(_buffer, 0, _buffer.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    _dataStream.Dispose();
                }
            }
        }
    }
}

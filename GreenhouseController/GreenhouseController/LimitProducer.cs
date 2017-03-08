using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LimitProducer
    {
        public event EventHandler<DataEventArgs> ItemInQueue;

        // Networking fields
        private UdpClient _client;
        private IPEndPoint _remoteIPEndPoint;

        // Singleton fields
        private static volatile LimitProducer _instance;
        private static object _syncRoot = new object();

        // Producer-consumer fields
        private byte[] _buffer;
        private byte[] _tempBuffer;
        

        /// <summary>
        /// Private constructor for singleton
        /// </summary>
        private LimitProducer()
        {
            _remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("Connecting to the cloud...");
            _client.Connect("127.0.0.1", 11000);
            Console.WriteLine("Successfully connected to cloud.");
        }

        /// <summary>
        /// Singleton instance property
        /// </summary>
        public LimitProducer Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new LimitProducer();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Listens for UDP packets that hold the limits of the greenhouse 
        /// </summary>
        /// <param name="target"></param>
        public void ReceiveGreenhouseLimits(BlockingCollection<byte[]> target)
        {
            while(true)
            {
                _buffer = _client.Receive(ref _remoteIPEndPoint);
                Array.Copy(sourceArray: _buffer, destinationArray: _tempBuffer, length: _buffer.Length);
                // TODO: send a successful receive message

                target.TryAdd(_tempBuffer);
                EventHandler<DataEventArgs> handler = ItemInQueue;
                handler(this, new DataEventArgs() { Buffer = target });

                Array.Clear(_buffer, 0, _buffer.Length);
            }
        }
    }
}

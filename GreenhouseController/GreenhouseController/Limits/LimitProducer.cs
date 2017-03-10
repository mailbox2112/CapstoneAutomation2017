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
    // Similar to the DataProducer class, this class listens for a Limit packet and acts accordingly
    public class LimitProducer
    {
        public event EventHandler<DataEventArgs> ItemInQueue;

        // Networking fields
        private NetworkStream _dataStream;
        private TcpClient _client;

        // Singleton fields
        private static volatile LimitProducer _instance;
        private static object _syncRoot = new object();

        // Producer-consumer fields
        private byte[] _buffer = new byte[1024];
        private byte[] _tempBuffer = new byte[1024];

        private bool _break;

        /// <summary>
        /// Private constructor for singleton
        /// </summary>
        private LimitProducer()
        {
            Console.WriteLine("Connecting to the cloud...");
            _client = new TcpClient();
            while (!_client.Connected)
            {
                try
                {
                    _client.Connect("127.0.0.1", 8000);
                    Console.WriteLine("Successfully connected to cloud.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not connect to cloud, retrying. {ex.Message}");
                    Thread.Sleep(1000);
                }
            }
            _dataStream = _client.GetStream();

            _break = false;
        }

        /// <summary>
        /// Singleton instance property
        /// </summary>
        public static LimitProducer Instance
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
        /// Listens for TCP packets that hold the limits of the greenhouse 
        /// </summary>
        /// <param name="target"></param>
        public void ReceiveGreenhouseLimits(BlockingCollection<byte[]> target)
        {
            // Listen for a packet with new greenhouse limits
            while (_break != true)
            {
                // The read command is blocking, so this just waits until data is available
                try
                {
                    if (_client.Connected)
                    {
                        _dataStream.Read(_buffer, 0, _buffer.Length);
                        Array.Copy(sourceArray: _buffer, destinationArray: _tempBuffer, length: _buffer.Length);

                        target.TryAdd(_tempBuffer);
                        EventHandler<DataEventArgs> handler = ItemInQueue;
                        handler(this, new DataEventArgs() { Buffer = target });

                        Array.Clear(_buffer, 0, _buffer.Length);
                    }
                }
                // Should we lose the connection, we get rid of the socket, try to start a new one,
                // and try to connect to it.
                catch
                {
                    _client.Close();
                    _client = new TcpClient();
                    while (!_client.Connected)
                    {
                        try
                        {
                            _client.Connect("127.0.0.1", 8888);
                            Console.WriteLine("Connected to data server.");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Could not connect to data server, retrying. {ex.Message}");
                            Thread.Sleep(1000);
                        }
                    }
                    _dataStream = _client.GetStream();
                }
            }
            _dataStream.Dispose();
        }
    }
}

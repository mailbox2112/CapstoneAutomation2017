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
    public class NetworkListener
    {
        private static volatile NetworkListener _instance;
        private static object _syncRoot = new object();
        private const string IP = "192.168.1.103";
        private const int PORT = 8888;
        
        private byte[] _buffer = new byte[1024];
        private byte[] _tempBuffer = new byte[1024];
        private NetworkStream _dataStream;
        private TcpClient _client;
        private bool _break;

        public event EventHandler<DataEventArgs> ItemInQueue;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint">Endpoint to be reached</param>
        /// <param name="hostAddress">IP address we're trying to connect to</param>
        private NetworkListener()
        {
            Console.WriteLine("Constructing data producer...");
            Console.WriteLine("Data producer constructed.\n");
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static NetworkListener Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new NetworkListener();
                        }
                    }
                }
                return _instance;
            }
        }

        public void TryConnect()
        {
            _client = new TcpClient();
            while (!_client.Connected)
            {
                try
                {
                    _client.Connect(IP, PORT);
                    //_client.Connect("127.0.0.1", PORT);
                    Console.WriteLine("Connected to data server.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not connect to data server, retrying. {ex.Message}");
                    Thread.Sleep(1000);
                }
            }
            _dataStream = _client.GetStream();
            _break = false;
        }

        public void RequestData()
        {
            if (!_dataStream.DataAvailable)
            {
                Console.WriteLine("\nRequesting data...");
                string request = "DATA";
                byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request));
                _dataStream.Write(data, 0, data.Length);
                _dataStream.Flush();
                Console.WriteLine("Request sent!\n");
            }
        }

        /// <summary>
        /// Read greenhouse data from the server
        /// </summary>
        public void ReadGreenhouseData(BlockingCollection<byte[]> target)
        {
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
                            //_client.Connect(IP, PORT);
                            _client.Connect("127.0.0.1", PORT);
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

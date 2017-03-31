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
        private const string IP = "192.168.1.103";
        private const int PORT = 8888;
        
        private byte[] _buffer = new byte[1024];
        private byte[] _tempBuffer = new byte[1024];
        private NetworkStream _dataStream;
        private TcpClient _client;
        private bool _break;
        private List<string> _requests = new List<string>() { "TLH", "MOISTURE", "LIMITS", "MANUAL" };
        private BlockingCollection<byte[]> _queue;

        public event EventHandler<DataEventArgs> ItemInQueue;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint">Endpoint to be reached</param>
        /// <param name="hostAddress">IP address we're trying to connect to</param>
        public NetworkListener(BlockingCollection<byte[]> queue)
        {
            Console.WriteLine("Constructing data producer...");
            _queue = queue;
            Console.WriteLine("Data producer constructed.\n");
        }

        public void TryConnect()
        {
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
            _break = false;
        }

        public void RequestData()
        {
            if (!_dataStream.DataAvailable)
            {
                foreach(string request in _requests)
                {
                    Console.WriteLine($"\nRequesting {request}...");
                    byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request));
                    _dataStream.Write(data, 0, data.Length);
                    _dataStream.Flush();
                    ReadGreenhouseData(request);
                    Console.WriteLine("Request sent!\n");
                }
            }
        }

        /// <summary>
        /// Read greenhouse data from the server
        /// </summary>
        public void ReadGreenhouseData(string type)
        {
            // The read command is blocking, so this just waits until data is available
            try
            {
                if (_client.Connected)
                {
                    _dataStream.Read(_buffer, 0, _buffer.Length);
                    Array.Copy(sourceArray: _buffer, destinationArray: _tempBuffer, length: _buffer.Length);

                    _queue.TryAdd(_tempBuffer);
                    EventHandler<DataEventArgs> handler = ItemInQueue;
                    handler(this, new DataEventArgs() { Buffer = _queue, Type = type });

                    Array.Clear(_buffer, 0, _buffer.Length);
                }
            }
            // Should we lose the connection, we get rid of the socket, try to start a new one,
            // and try to connect to it.
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                    catch (Exception e)
                    {
                        Console.WriteLine($"Could not connect to data server, retrying. {e.Message}");
                        Thread.Sleep(1000);
                    }
                }
                _dataStream = _client.GetStream();
            }
        }
    }
}

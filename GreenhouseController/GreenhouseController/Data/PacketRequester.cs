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
    public class PacketRequester
    {
        private const string IP = "192.168.1.103";
        private const int PORT = 8888;
        
        private byte[] _buffer = new byte[1024];
        private byte[] _tempBuffer = new byte[1024];
        private NetworkStream _dataStream;
        private TcpClient _client = new TcpClient();
        private List<string> _requests = new List<string>() { "TLH", "MOISTURE", "LIMITS", "MANUAL" };
        private BlockingCollection<byte[]> _queue;

        public event EventHandler<DataEventArgs> ItemInQueue;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint">Endpoint to be reached</param>
        /// <param name="hostAddress">IP address we're trying to connect to</param>
        public PacketRequester(BlockingCollection<byte[]> queue)
        {
            Console.WriteLine("Constructing packet requester...");
            _queue = queue;
            Console.WriteLine("Packet requester constructed.\n");
        }

        /// <summary>
        /// Attempt to connect to the server
        /// </summary>
        public void TryConnect()
        {
            while (!_client.Connected)
            {
                try
                {
                    //_client = new TcpClient(IP,PORT);
                    _client = new TcpClient("127.0.0.1", PORT);
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

        /// <summary>
        /// Request data and read the response from the server
        /// </summary>
        public void RequestData()
        {
            // TODO: open and close the socket after each request and read
            foreach(string request in _requests)
            {
                TryConnect();
                Console.WriteLine($"\nRequesting {request}...");
                byte[] data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(request));
                _dataStream.Write(data, 0, data.Length);
                _dataStream.Flush();
                ReadGreenhouseData(request);
                Console.WriteLine("Request sent!\n");
                _client.Close();
                _client.Dispose();
            }
        }

        /// <summary>
        /// Read greenhouse data from the server
        /// </summary>
        public void ReadGreenhouseData(string type)
        {
            // Read the data in response the request for data
            try
            {
                if (_client.Connected)
                {
                    // Read data and copy it to a temporary array/buffer
                    _dataStream.Read(_buffer, 0, _buffer.Length);
                    Array.Copy(sourceArray: _buffer, destinationArray: _tempBuffer, length: _buffer.Length);

                    // Try to add it to the blocking collection
                    _queue.TryAdd(_tempBuffer);

                    // Hook up a temporary event handler and trigger the event
                    EventHandler<DataEventArgs> handler = ItemInQueue;
                    handler(this, new DataEventArgs() { Buffer = _queue, Type = type });

                    // Clear our buffer
                    Array.Clear(_buffer, 0, _buffer.Length);
                }
            }
            // Should we lose the connection, we get rid of the socket, try to start a new one,
            // and try to connect to it.
            catch (Exception ex)
            {
                // Write the exception for debugging purposes
                Console.WriteLine(ex.Message);

                // Close the connection and create a new client
                _client.Close();
                _client = new TcpClient();
                while (!_client.Connected)
                {
                    // Try to reconnect to the server, and wait a second between each attempt
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
                // Once we're connected, get the datastream from the client
                _dataStream = _client.GetStream();
            }
        }
    }
}

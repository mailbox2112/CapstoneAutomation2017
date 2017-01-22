using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class DataReceiver
    {
        private static volatile DataReceiver instance;
        private static object syncRoot = new Object();

        private Socket _cloudConnection;
        private IPEndPoint _cloudEndpoint;
        private IPAddress _cloudIp;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint"></param>
        /// <param name="hostAddress"></param>
        private DataReceiver(IPAddress hostAddress, IPEndPoint hostEndpoint)
        {
            Console.WriteLine("Constructing data receiver...");
            _cloudIp = hostAddress;
            _cloudEndpoint = hostEndpoint;
            _cloudConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Data receiver constructed.");
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static DataReceiver Instance
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
                            instance = new DataReceiver(null, null);
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Request greenhouse data from the server
        /// </summary>
        public byte[] RequestAndReceiveGreenhouseData()
        {
            try
            {
                Console.WriteLine("Attempting to connect to cloud server...");
                //_cloudConnection.Connect(_cloudEndpoint);
                Console.WriteLine("Successfully connected to cloud.");

                // TODO: send actual command packet!
                byte[] fakeCommand = new byte[] { 75, 34, 25 };

                Console.WriteLine("Sending data request to cloud server...");
                //_cloudConnection.Send(fakeCommand, SocketFlags.None);
                Console.WriteLine("Request sent successfully");

                // TODO: get actual data
                byte[] fakeReceive = new byte[1024];

                Console.WriteLine("Attempting to receive greenhouse data...");
                //_cloudConnection.Receive(fakeReceive, SocketFlags.None);
                Console.WriteLine("Successfully received greenhouse data.");

                //_cloudConnection.Dispose();

                return fakeReceive;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //_cloudConnection.Dispose();
                return null;
            }
            
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class GreenhouseDataConsumer
    {
        private static volatile GreenhouseDataConsumer instance;
        private static object _syncRoot = new Object();
        private object _lock = new object();
        private byte[] _data;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint"></param>
        /// <param name="hostAddress"></param>
        private GreenhouseDataConsumer()
        {
            Console.WriteLine("Constructing greenhouse data analyzer...");
            Console.WriteLine("Greenhouse data analyzer constructed.");
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static GreenhouseDataConsumer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (instance == null)
                        {
                            // TODO: add whatever parameters get passed into construction!
                            instance = new GreenhouseDataConsumer();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Takes in a BlockingCollection and removes data. Sends data to be assessed elsewhere
        /// </summary>
        /// <param name="source"></param>
        public void ReceiveGreenhouseData(BlockingCollection<byte[]> source)
        {
            while (true)
            {
                if (source.Count != 0)
                {
                    try
                    {
                        source.TryTake(out _data);
                        SendDataToAnalyzer(_data);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Sends the data off to be analyzed
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public void SendDataToAnalyzer(byte[] data)
        {
            if(data != null)
            {
                Console.WriteLine(data);
                GreenhouseDataAnalyzer analyze = new GreenhouseDataAnalyzer();
                Task.Run(() => analyze.InterpretStateData(data));
            }
        }
    }
}

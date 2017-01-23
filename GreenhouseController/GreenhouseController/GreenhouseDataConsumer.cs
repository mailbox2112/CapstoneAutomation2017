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

        public void ReceiveGreenhouseDataAsync(BlockingCollection<byte[]> source)
        {
            while (true)
            {
                try
                {
                    source.TryTake(out _data);
                    AssessGreenhouseState(_data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public GreenhouseState[] AssessGreenhouseState(byte[] data)
        {
            if(data != null)
            {
                Console.WriteLine(data);
            }
            return null;
        }
    }
}

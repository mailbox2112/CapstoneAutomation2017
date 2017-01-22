using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class GreenhouseStateAnalyzer
    {
        private static volatile GreenhouseStateAnalyzer instance;
        private static object syncRoot = new Object();

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        /// <param name="hostEndpoint"></param>
        /// <param name="hostAddress"></param>
        private GreenhouseStateAnalyzer()
        {
            Console.WriteLine("Constructing greenhouse state analyzer...");
            Console.WriteLine("Greenhouse state analyzer constructed.");
        }

        /// <summary>
        /// Instance property, used for singleton pattern
        /// </summary>
        public static GreenhouseStateAnalyzer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            // TODO: add whatever parameters get passed into construction!
                            instance = new GreenhouseStateAnalyzer();
                        }
                    }
                }
                return instance;
            }
        }

        public GreenhouseState[] AssessGreenhouseState(byte[] data)
        {
            // TODO: assess fake data and act appropriately!
            GreenhouseState[] fakeState = new GreenhouseState[] { GreenhouseState.HOT, GreenhouseState.PLANTS_OKAY };
            return fakeState;
        }
    }
}

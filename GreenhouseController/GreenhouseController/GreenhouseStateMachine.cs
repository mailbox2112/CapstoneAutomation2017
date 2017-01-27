using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    /// <summary>
    /// State machine implementation for our greenhouse. Needs to be a singleton so we're not saying we're in multiple states at once.
    /// </summary>
    class GreenhouseStateMachine
    {
        // Singleton stuff
        private static volatile GreenhouseStateMachine _instance;
        private static object _syncRoot = new object();

        // State machine property
        public GreenhouseState CurrentState { get; set; }

        /// <summary>
        /// Private constructor for singleton. We always start the program in the WAITING state.
        /// </summary>
        private GreenhouseStateMachine()
        {
            CurrentState = GreenhouseState.WAITING;
        }

        // Instance property for singleton
        public static GreenhouseStateMachine Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new GreenhouseStateMachine();
                        }
                    }
                }

                return _instance;
            }
        }
        

        
    }
}

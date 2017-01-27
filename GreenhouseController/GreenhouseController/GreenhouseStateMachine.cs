using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    /// <summary>
    /// Nondeterministic State machine implementation for our greenhouse. Needs to be a singleton for persistence and preventing multiple changes to states.
    /// </summary>
    class GreenhouseStateMachine
    {
        // Singleton stuff
        private static volatile GreenhouseStateMachine _instance;
        private static object _syncRoot = new object();

        // State machine property
        public List<GreenhouseState> CurrentStates { get; set; }

        /// <summary>
        /// Private constructor for singleton. We always start the program in the WAITING state.
        /// </summary>
        private GreenhouseStateMachine()
        {
            CurrentStates = new List<GreenhouseState> { GreenhouseState.WAITING };
        }

        /// <summary>
        /// Used to construct the singleton on program startup and set the current state
        /// to waiting without creating a method that would ALWAYS set the state to waiting.
        /// </summary>
        public void Initialize() { }

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

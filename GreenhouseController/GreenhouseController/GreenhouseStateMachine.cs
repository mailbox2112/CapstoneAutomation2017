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
    public class GreenhouseStateMachine
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

        public void CalculateNewState(int stateValue)
        {
            switch(stateValue)
            {
                case 0:
                    CurrentState = GreenhouseState.WAITING;
                    break;
                case 1:
                    CurrentState = GreenhouseState.LIGHTING;
                    break;
                case 2:
                    CurrentState = GreenhouseState.WATERING;
                    break;
                case 3:
                    CurrentState = GreenhouseState.LIGHTING_WATERING;
                    break;
                case 10:
                    CurrentState = GreenhouseState.HEATING;
                    break;
                case 11:
                    CurrentState = GreenhouseState.HEATING_LIGHTING;
                    break;
                case 12:
                    CurrentState = GreenhouseState.HEATING_WATERING;
                    break;
                case 13:
                    CurrentState = GreenhouseState.HEATING_LIGHTING_WATERING;
                    break;
                case 20:
                    CurrentState = GreenhouseState.COOLING;
                    break;
                case 21:
                    CurrentState = GreenhouseState.COOLING_LIGHTING;
                    break;
                case 22:
                    CurrentState = GreenhouseState.COOLING_WATERING;
                    break;
                case 23:
                    CurrentState = GreenhouseState.COOLING_LIGHTING_WATERING;
                    break;
                default:
                    CurrentState = GreenhouseState.WAITING;
                    break;
            }
        }
    }
}

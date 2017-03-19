using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class WateringStateMachine : IStateMachine
    {
        // TODO: Add timer event-based watering
        private const int _emergencyMoist = 0;

        private GreenhouseState _currentState;
        public GreenhouseState CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                OnStateChange(new StateEventArgs() { State = CurrentState });
            }
        }

        public EventHandler<StateEventArgs> StateChanged { get; set; }

        public int? HighLimit { get; set; }

        public int? LowLimit { get; set; }

        /// <summary>
        /// Initialize the state machine
        /// </summary>
        public WateringStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }

        /// <summary>
        /// Determine the state of the greenhouse based on moisture data we receive
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GreenhouseState DetermineState(double value)
        {
            if (CurrentState == GreenhouseState.WATERING)
            {
                CurrentState = GreenhouseState.PROCESSING_WATER;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // Check the states based on data, and if we were already watering take that into account
            if (value < LowLimit && CurrentState != GreenhouseState.PROCESSING_WATER)
            {
                if (value == _emergencyMoist)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else 
                {
                    return GreenhouseState.WATERING;
                }
            }
            else if (value < LowLimit && CurrentState == GreenhouseState.PROCESSING_WATER)
            {
                if (value == _emergencyMoist)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else
                {
                    CurrentState = GreenhouseState.WATERING;
                    return GreenhouseState.NO_CHANGE;
                }
            }
            else if (value > LowLimit && CurrentState == GreenhouseState.PROCESSING_DATA)
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                return GreenhouseState.NO_CHANGE;
            }
            else
            {
                return GreenhouseState.WAITING_FOR_DATA;
            }
        }

        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            // check the state of the solenoids
            List<Commands> commandsToSend = new List<Commands>();
            
            if (state == GreenhouseState.WATERING)
            {
                commandsToSend.Add(Commands.WATER_ON);
            }
            else if (state == GreenhouseState.WAITING_FOR_DATA)
            {
                commandsToSend.Add(Commands.WATER_OFF);
            }

            return commandsToSend;
        }

        public void OnStateChange(StateEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}

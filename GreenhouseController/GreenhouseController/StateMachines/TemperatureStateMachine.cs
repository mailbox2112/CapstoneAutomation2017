using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class TemperatureStateMachine : IStateMachine
    {
        private const int _emergencyTemp = 120;

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

        public int HighLimit { get; set; }

        public int LowLimit { get; set; }

        /// <summary>
        /// Initialize the state machine
        /// </summary>
        public TemperatureStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }

        /// <summary>
        /// Determines the state of the greenhouse based on the data that's input
        /// </summary>
        /// <param name="value">Average value of temperature data from each zone</param>
        /// <returns></returns>
        public GreenhouseState DetermineState(double value)
        {
            // Determine which processing state we're in
            if (CurrentState == GreenhouseState.HEATING)
            {
                CurrentState = GreenhouseState.PROCESSING_HEATING;
            }
            else if (CurrentState == GreenhouseState.COOLING)
            {
                CurrentState = GreenhouseState.PROCESSING_COOLING;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // Determine what state to return/change to
            // If we're coming from an action state and we meet action criteria,
            // go back to the action state
            if (value <= LowLimit && CurrentState != GreenhouseState.PROCESSING_HEATING)
            {
                return GreenhouseState.HEATING;
            }
            else if (value <= LowLimit && CurrentState == GreenhouseState.PROCESSING_HEATING)
            {
                CurrentState = GreenhouseState.HEATING;
                return GreenhouseState.NO_CHANGE;
            }
            else if (value > HighLimit && CurrentState != GreenhouseState.PROCESSING_COOLING)
            {
                if (value >= _emergencyTemp)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else
                {
                    return GreenhouseState.COOLING;
                }
            }
            else if (value > HighLimit && CurrentState == GreenhouseState.PROCESSING_COOLING)
            {
                if (value >= _emergencyTemp)
                {
                    return GreenhouseState.EMERGENCY;
                }
                else
                {
                    CurrentState = GreenhouseState.COOLING;
                    return GreenhouseState.NO_CHANGE;
                }
            }
            else if (value > LowLimit && value < HighLimit && CurrentState == GreenhouseState.PROCESSING_DATA)
            {
                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                return GreenhouseState.NO_CHANGE;
            }
            else
            {
                return GreenhouseState.WAITING_FOR_DATA;
            }
        }

        /// <summary>
        /// Takes a greenhouse state and converts it to a list of commands to be sent off
        /// in order to properly transition to that state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            List<Commands> commandsToSend = new List<Commands>();
            if (state == GreenhouseState.COOLING)
            {
                commandsToSend.Add(Commands.HEAT_OFF);
                commandsToSend.Add(Commands.FANS_ON);
                commandsToSend.Add(Commands.VENT_OPEN);
                commandsToSend.Add(Commands.SHADE_EXTEND);
            }
            else if (state == GreenhouseState.HEATING)
            {
                commandsToSend.Add(Commands.FANS_OFF);
                commandsToSend.Add(Commands.HEAT_ON);
                commandsToSend.Add(Commands.VENT_CLOSE);
                commandsToSend.Add(Commands.SHADE_RETRACT);
            }
            else if (state == GreenhouseState.WAITING_FOR_DATA)
            {
                commandsToSend.Add(Commands.HEAT_OFF);
                commandsToSend.Add(Commands.FANS_OFF);
                commandsToSend.Add(Commands.VENT_CLOSE);
                commandsToSend.Add(Commands.SHADE_RETRACT);
            }

            return commandsToSend;
        }

        /// <summary>
        /// Invokes state change event handler
        /// </summary>
        /// <param name="e"></param>
        public void OnStateChange(StateEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}

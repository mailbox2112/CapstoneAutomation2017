using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LightingStateMachine : IStateMachine
    {
        // Private member for implementing custom get/set using the event handler
        private GreenhouseState _currentState;

        // Public property to access private member and throw event
        public GreenhouseState CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                // Set the value and fire off event
                _currentState = value;
                OnStateChange(new StateEventArgs() { State = CurrentState });
            }
        }

        // Upper limit for lighting, causes shades to close
        public int? HighLimit { get; set; }

        // Lower limit for lighting, causes lights to turn on
        public int? LowLimit { get; set; }

        public EventHandler<StateEventArgs> StateChanged;
        
        /// <summary>
        /// Initialize the state machine
        /// </summary>
        public LightingStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }

        /// <summary>
        /// Determinet the state of the greenhouse based on the lighting data we receive
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GreenhouseState DetermineState(double value)
        {
            if (CurrentState == GreenhouseState.LIGHTING)
            {
                CurrentState = GreenhouseState.PROCESSING_LIGHTING;
            }
            else if (CurrentState == GreenhouseState.SHADING)
            {
                CurrentState = GreenhouseState.PROCESSING_SHADING;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // Process data and take into account if we were already lighting when we received the data
            if (value < LowLimit && CurrentState != GreenhouseState.PROCESSING_LIGHTING)
            {
                return GreenhouseState.LIGHTING;
            }
            else if (value > HighLimit && CurrentState != GreenhouseState.PROCESSING_SHADING)
            {
                return GreenhouseState.SHADING;
            }
            else if (value < LowLimit && CurrentState == GreenhouseState.PROCESSING_LIGHTING)
            {
                CurrentState = GreenhouseState.LIGHTING;
                return GreenhouseState.NO_CHANGE;
            }
            else if (value > HighLimit && CurrentState == GreenhouseState.PROCESSING_SHADING)
            {
                CurrentState = GreenhouseState.SHADING;
                return GreenhouseState.NO_CHANGE;
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
        /// Convert a greenhouse state to a list of commands to send to the Arduino
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            // TODO: check the state of the lights so we don't have to send the command if it's already off
            List<Commands> commandsToSend = new List<Commands>();
            if (state == GreenhouseState.LIGHTING)
            {
                commandsToSend.Add(Commands.SHADE_RETRACT);
                commandsToSend.Add(Commands.LIGHTS_ON);
            }
            else if (state == GreenhouseState.WAITING_FOR_DATA)
            {
                commandsToSend.Add(Commands.LIGHTS_OFF);
                commandsToSend.Add(Commands.SHADE_RETRACT);
            }
            else if (state == GreenhouseState.SHADING)
            {
                commandsToSend.Add(Commands.SHADE_EXTEND);
            }

            return commandsToSend;
        }

        // Invoke the event handler
        public void OnStateChange(StateEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}

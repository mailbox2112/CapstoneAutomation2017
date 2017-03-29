using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController.StateMachines
{
    public class ShadingStateMachine : IStateMachine
    {
        private GreenhouseState _currentState;

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

        public int? HighLimit { get; set; }

        public int? LowLimit { get; set; }

        public bool? ManualShade { get; set; }

        public EventHandler<StateEventArgs> StateChanged;

        public ShadingStateMachine()
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
        }

        /// <summary>
        /// Converts a greenhouse state into a set of commands
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            List<Commands> commandsToSend = new List<Commands>();
            if (state == GreenhouseState.SHADING)
            {
                commandsToSend.Add(Commands.SHADE_EXTEND);
            }
            else
            {
                commandsToSend.Add(Commands.SHADE_RETRACT);
            }
            return commandsToSend;
        }

        /// <summary>
        /// Determines the state of the shading state machine based on lighting sensor value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GreenhouseState DetermineState(double value = 0)
        {
            if (CurrentState == GreenhouseState.SHADING)
            {
                CurrentState = GreenhouseState.PROCESSING_SHADING;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            if (ManualShade != true)
            {
                if (value >= HighLimit && CurrentState != GreenhouseState.PROCESSING_SHADING)
                {
                    return GreenhouseState.SHADING;
                }
                else if (value >= HighLimit && CurrentState == GreenhouseState.PROCESSING_SHADING)
                {
                    CurrentState = GreenhouseState.SHADING;
                    return GreenhouseState.NO_CHANGE;
                }
                else if (value < HighLimit && CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    CurrentState = GreenhouseState.WAITING_FOR_DATA;
                    return GreenhouseState.NO_CHANGE;
                }
                else
                {
                    return GreenhouseState.WAITING_FOR_DATA;
                }
            }
            else if (ManualShade == true)
            {
                if (CurrentState == GreenhouseState.PROCESSING_SHADING)
                {
                    CurrentState = GreenhouseState.SHADING;
                    return GreenhouseState.NO_CHANGE;
                }
                else
                {
                    return GreenhouseState.SHADING;
                }
            }
            //else if (ManualShade == false)
            //{
            //    ManualShade = null;
            //    if (CurrentState == GreenhouseState.PROCESSING_DATA)
            //    {
            //        CurrentState = GreenhouseState.WAITING_FOR_DATA;
            //        return GreenhouseState.NO_CHANGE;
            //    }
            //    else
            //    {
            //        return GreenhouseState.WAITING_FOR_DATA;
            //    }
            //}
            else
            {
                return GreenhouseState.ERROR;
            }
        }

        /// <summary>
        /// Invokes the eventhandler when our state changes
        /// </summary>
        /// <param name="e"></param>
        public void OnStateChange(StateEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}

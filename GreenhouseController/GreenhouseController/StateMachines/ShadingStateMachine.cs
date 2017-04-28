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
        /// Determines the state of the shading state machine based on lighting sensor value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GreenhouseState DetermineState(double value)
        {
            // TODO: revamp!
            if (CurrentState == GreenhouseState.SHADING)
            {
                CurrentState = GreenhouseState.PROCESSING_SHADING;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // If we don't have a manual shading command
            if (ManualShade != true)
            {
                // If we're coming from the wait state
                if (CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    // If we're above the temperature limit, extend the shade
                    // Otherwise, the shade stays retracted
                    if (value >= HighLimit)
                    {
                        return GreenhouseState.SHADING;
                    }
                    else
                    {
                        CurrentState = GreenhouseState.WAITING_FOR_DATA;
                        return GreenhouseState.NO_CHANGE;
                    }
                }
                // If we're already shading
                else if (CurrentState == GreenhouseState.PROCESSING_SHADING)
                {
                    // If we're 10 degrees below the high limit now, retract the shade
                    if (value < HighLimit - 10)
                    {
                        return GreenhouseState.WAITING_FOR_DATA;
                    }
                    // Otherwise, keep the shade extended
                    else
                    {
                        CurrentState = GreenhouseState.SHADING;
                        return GreenhouseState.NO_CHANGE;
                    }
                }
                // If we're in neither processing state, throw an error
                else
                {
                    return GreenhouseState.ERROR;
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
        /// Invokes the eventhandler when our state changes
        /// </summary>
        /// <param name="e"></param>
        public void OnStateChange(StateEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}

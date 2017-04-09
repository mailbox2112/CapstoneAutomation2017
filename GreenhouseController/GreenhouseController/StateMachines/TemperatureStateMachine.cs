using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class TemperatureStateMachine : IStateMachine
    {
        private const int _emergencyHigh = 120;
        private const int _emergencyLow = 32;
        // True = on, false = off
        // Keeps track of what commands we need to send on and off
        private bool _fanState = false;
        private bool _heatState = false;
        private bool _ventState = false;

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

        public bool? ManualHeat { get; set; }

        public bool? ManualCool { get; set; }

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
        public GreenhouseState DetermineState(double value = 0)
        {
            // TODO: Should we get rid off manually forcing this into the wating for data state when manual control is turned off?
            // it seems logical to immediately return to automation
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
           
            // If the state machine isn't in manual control mode
            if (ManualHeat != true && ManualCool != true)
            {
                // Determine what state to return/change to
                // If we're coming from an action state and we meet action criteria,
                // go back to the action state
                // If we're lower than the low limit
                if (value <= LowLimit && CurrentState != GreenhouseState.PROCESSING_HEATING)
                {
                    if (value <= _emergencyLow)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    else
                    {
                        return GreenhouseState.HEATING;
                    }
                }
                // If we're lower than the low limit but we're already heating
                else if (value <= LowLimit && CurrentState == GreenhouseState.PROCESSING_HEATING)
                {
                    CurrentState = GreenhouseState.HEATING;
                    return GreenhouseState.NO_CHANGE;
                }
                // If we're higher than the high limit
                else if (value >= HighLimit && CurrentState != GreenhouseState.PROCESSING_COOLING)
                {
                    if (value >= _emergencyHigh)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    else
                    {
                        return GreenhouseState.COOLING;
                    }
                }
                // If we're higher than the high limit but we're already cooling
                else if (value >= HighLimit && CurrentState == GreenhouseState.PROCESSING_COOLING)
                {
                    if (value >= _emergencyHigh)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    else
                    {
                        CurrentState = GreenhouseState.COOLING;
                        return GreenhouseState.NO_CHANGE;
                    }
                }
                // If we're neither too high nor too low 
                else if (value > LowLimit && value < HighLimit && CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    CurrentState = GreenhouseState.WAITING_FOR_DATA;
                    return GreenhouseState.NO_CHANGE;
                }
                else if (value <= HighLimit - 5 && value > LowLimit && CurrentState == GreenhouseState.PROCESSING_COOLING)
                {
                    return GreenhouseState.WAITING_FOR_DATA;
                }
                else if (value >= LowLimit + 5 && value < HighLimit && CurrentState == GreenhouseState.PROCESSING_HEATING)
                {
                    return GreenhouseState.WAITING_FOR_DATA;
                }
                else
                {
                    return GreenhouseState.WAITING_FOR_DATA;
                }
                
            }
            // If ManualHeat is on
            else if (ManualHeat == true && ManualCool != true)
            {
                if (CurrentState == GreenhouseState.PROCESSING_HEATING)
                {
                    CurrentState = GreenhouseState.HEATING;
                    return GreenhouseState.NO_CHANGE;
                }
                else
                {
                    return GreenhouseState.HEATING;
                }
            }
            // If ManualHeat is off
            //else if (ManualHeat == false)
            //{
            //    ManualHeat = null;
            //    return GreenhouseState.WAITING_FOR_DATA;
            //}
            // If ManualCool is on
            else if (ManualCool == true && ManualHeat != true)
            {
                if (CurrentState == GreenhouseState.PROCESSING_COOLING)
                {
                    CurrentState = GreenhouseState.COOLING;
                    return GreenhouseState.NO_CHANGE;
                }
                else
                {
                    return GreenhouseState.COOLING;
                }
            }
            // If ManualCool is off
            //else if (ManualCool == false)
            //{
            //    ManualCool = null;
            //    return GreenhouseState.WAITING_FOR_DATA;
            //}
            // If we don't meet any of those criteria, return an error state
            else
            {
                return GreenhouseState.ERROR;
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
                if (_heatState == true)
                {
                    commandsToSend.Add(Commands.HEAT_OFF);
                    _heatState = false;
                }
                if (_fanState == false)
                {
                    commandsToSend.Add(Commands.FANS_ON);
                    _fanState = true;
                }
                if (_ventState == false)
                {
                    commandsToSend.Add(Commands.VENTS_OPEN);
                    _ventState = true;
                }
            }
            else if (state == GreenhouseState.HEATING)
            {
                if (_fanState == true)
                {
                    commandsToSend.Add(Commands.FANS_OFF);
                    _fanState = false;
                }
                if (_heatState == false)
                {
                    commandsToSend.Add(Commands.HEAT_ON);
                    _heatState = true;
                }
                if (_ventState == true)
                {
                    commandsToSend.Add(Commands.VENTS_CLOSED);
                    _ventState = false;
                }
            }
            else if (state == GreenhouseState.WAITING_FOR_DATA)
            {
                if (_heatState == true)
                {
                    commandsToSend.Add(Commands.HEAT_OFF);
                    _heatState = false;
                }
                if (_fanState == true)
                {
                    commandsToSend.Add(Commands.FANS_OFF);
                    _fanState = false;
                }
                if (_ventState == true)
                {
                    commandsToSend.Add(Commands.VENTS_CLOSED);
                    _ventState = false;
                }
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

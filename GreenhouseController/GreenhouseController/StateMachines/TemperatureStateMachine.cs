using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class TemperatureStateMachine : IStateMachine
    {
        // TODO: separate exhaust and circulating fans!
        // Circulating fans need to be on for both heating AND cooling!
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
        public GreenhouseState DetermineState(double value)
        {
            // TODO: revamp!
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
                // If we're coming from the wait state
                if (CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    // If the value is too high but below emergency, we go to cooling
                    if (value >= HighLimit && value < _emergencyHigh)
                    {
                        return GreenhouseState.COOLING;
                    }
                    else if (value >= _emergencyHigh)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    // If the value is too low but above emergency, we go to heating
                    else if (value <= LowLimit && value > _emergencyLow)
                    {
                        return GreenhouseState.HEATING;
                    }
                    else if (value <= _emergencyLow)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    // Otherwise, we stay waiting
                    else
                    {
                        CurrentState = GreenhouseState.WAITING_FOR_DATA;
                        return GreenhouseState.NO_CHANGE;
                    }
                }
                // If we're coming from the cooling state
                else if (CurrentState == GreenhouseState.PROCESSING_COOLING)
                {
                    // If the value is still to high, but not at emergency values
                    if (value >= HighLimit - 5 && value < _emergencyHigh)
                    {
                        CurrentState = GreenhouseState.COOLING;
                        return GreenhouseState.NO_CHANGE;
                    }
                    // If the temp is above the emergency value
                    else if (value > _emergencyHigh)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    // If we're within the limits (including hysteresis)
                    else if (value < HighLimit - 5 && value >= LowLimit)
                    {
                        return GreenhouseState.WAITING_FOR_DATA;
                    }
                    // If we somehow went below the low limit
                    else if (value < LowLimit && value > _emergencyLow)
                    {
                        return GreenhouseState.HEATING;
                    }
                    // If we somehow hit the emergency low limit
                    else if (value <= _emergencyLow)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    // If we somehow meet none of those criteria, error
                    else
                    {
                        return GreenhouseState.ERROR;
                    }
                }
                else if (CurrentState == GreenhouseState.PROCESSING_HEATING)
                {
                    // If value is still below low limit but above emergency value
                    if (value <= LowLimit + 5 && value > _emergencyLow)
                    {
                        CurrentState = GreenhouseState.HEATING;
                        return GreenhouseState.NO_CHANGE;
                    }
                    // If the value drops below the emergency low
                    else if (value <= _emergencyLow)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    // If we're between the high limit and low lim + hysteresis
                    else if (value >= LowLimit + 5 && value < HighLimit)
                    {
                        return GreenhouseState.WAITING_FOR_DATA;
                    }
                    // If we somehow hit the high limit but not emergency high
                    else if (value >= HighLimit && value < _emergencyHigh)
                    {
                        return GreenhouseState.COOLING;
                    }
                    // If we hit the emergency high limit
                    else if (value >= _emergencyHigh)
                    {
                        return GreenhouseState.EMERGENCY;
                    }
                    // If we don't hit any of those conditions, error
                    else
                    {
                        return GreenhouseState.ERROR;
                    }
                }
                // If we're not in any of the processing states, error
                else
                {
                    return GreenhouseState.ERROR;
                }
            }
            // If ManualHeat is on
            else if (ManualHeat == true && ManualCool != true)
            {
                // If we're already heating, keep heating
                if (CurrentState == GreenhouseState.PROCESSING_HEATING)
                {
                    CurrentState = GreenhouseState.HEATING;
                    return GreenhouseState.NO_CHANGE;
                }
                // Otherwise, turn on the heat
                else
                {
                    return GreenhouseState.HEATING;
                }
            }
            ////If ManualHeat is off
            //else if (ManualHeat == false)
            //{
            //    // If we're heating currently, turn it off
            //    if (CurrentState == GreenhouseState.PROCESSING_HEATING)
            //    {
            //        return GreenhouseState.WAITING_FOR_DATA;
            //    }
            //    // If we're cooling currently, don't do anything
            //    else if (CurrentState == GreenhouseState.PROCESSING_COOLING)
            //    {
            //        CurrentState = GreenhouseState.COOLING;
            //        return GreenhouseState.NO_CHANGE;
            //    }
            //    // If we're doing nothing currently, continue to do nothing
            //    else
            //    {
            //        CurrentState = GreenhouseState.WAITING_FOR_DATA;
            //        return GreenhouseState.NO_CHANGE;
            //    }
            //}
            //If ManualCool is on
            else if (ManualCool == true && ManualHeat != true)
            {
                // If we're already cooling, don't do anything new
                if (CurrentState == GreenhouseState.PROCESSING_COOLING)
                {
                    CurrentState = GreenhouseState.COOLING;
                    return GreenhouseState.NO_CHANGE;
                }
                // If we're not cooling already, start cooling
                else
                {
                    return GreenhouseState.COOLING;
                }
            }
            // If ManualCool is off
            //else if (ManualCool == false)
            //{
            //    // If we're cooling currently, go back to waiting
            //    if (CurrentState == GreenhouseState.PROCESSING_COOLING)
            //    {
            //        return GreenhouseState.WAITING_FOR_DATA;
            //    }
            //    // If we're heating right now, don't do anything
            //    else if (CurrentState == GreenhouseState.PROCESSING_HEATING)
            //    {
            //        CurrentState = GreenhouseState.HEATING;
            //        return GreenhouseState.NO_CHANGE;
            //    }
            //    // If we're doing nothing right now, continue to do nothing
            //    else
            //    {
            //        CurrentState = GreenhouseState.WAITING_FOR_DATA;
            //        return GreenhouseState.NO_CHANGE;
            //    }
            //}
            // If both were set to off, go to the waiting state
            //else if (ManualCool == false && ManualHeat == false)
            //{
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
            // If we don't meet any of those criteria, return an error state
            // Such as both heating AND cooling are turn on
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

using GreenhouseController.Limits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LightingStateMachine : ITimeBasedStateMachine
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

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public EventHandler<StateEventArgs> StateChanged;

        public int Zone { get; set; }

        public bool? ManualLight { get; set; }

        public double? OverrideThreshold { get; set; }

        public ScheduleTypes ScheduleType { get; set; }

        /// <summary>
        /// Initialize the state machine
        /// </summary>
        public LightingStateMachine(int zone)
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
            Zone = zone;
        }

        /// <summary>
        /// Determinet the state of the greenhouse based on the lighting data we receive
        /// </summary>
        /// <param name="currentTime"></param>
        /// <returns></returns>
        public GreenhouseState DetermineState(DateTime currentTime, double value)
        {
            if (CurrentState == GreenhouseState.LIGHTING)
            {
                CurrentState = GreenhouseState.PROCESSING_LIGHTING;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // If we don't have a manual lighting command
            if (ManualLight != true)
            {
                /* Process data and take into account if we were already lighting when we received the data
                   If we're coming from the wait state and processing data */
                if (CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    // If we're within the scheduled time, check the override value. Otherwise, nothing happens
                    if (currentTime.TimeOfDay >= Begin.TimeOfDay && currentTime.TimeOfDay <= End.TimeOfDay)
                    {
                        // If they want sensors to override the schedule, check the threshold
                        if (ScheduleType == ScheduleTypes.SENSORS)
                        {
                            /* Since we're in the processing data state, we know the lights aren't already on. 
                                Therefore, we can just keep ourselves in the waiting for data state if we're
                                above the override threshold value */
                            if (value >= OverrideThreshold)
                            {
                                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                                return GreenhouseState.NO_CHANGE;
                            }
                            // Otherwise, we turn the lights on
                            else
                            {
                                return GreenhouseState.LIGHTING;
                            }
                        }
                        else if (ScheduleType == ScheduleTypes.BLOCKED)
                        {
                            CurrentState = GreenhouseState.WAITING_FOR_DATA;
                            return GreenhouseState.NO_CHANGE;
                        }
                        // Otherwise, see if we just turn the lights on
                        else if (ScheduleType == ScheduleTypes.CONSTANT)
                        {
                            return GreenhouseState.LIGHTING;
                        }
                        else
                        {
                            return GreenhouseState.ERROR;
                        }
                    }
                    // We're not within the scheduled time, so we just go back to waiting
                    else
                    {
                        CurrentState = GreenhouseState.WAITING_FOR_DATA;
                        return GreenhouseState.NO_CHANGE;
                    }
                }
                // If we're coming from the lighting state and processing data
                else if (CurrentState == GreenhouseState.PROCESSING_LIGHTING)
                {
                    // Check if we're within the scheduled time still
                    if (currentTime.TimeOfDay >= Begin.TimeOfDay && currentTime.TimeOfDay <= End.TimeOfDay)
                    {
                        // Check if they want the sensors to override the schedule
                        if (ScheduleType == ScheduleTypes.SENSORS)
                        {
                            // Since they want the sensors overriding the schedule, check the sensor values
                            if (value >= OverrideThreshold)
                            {
                                // The lights are currently on, so we need to turn them off
                                return GreenhouseState.WAITING_FOR_DATA;
                            }
                            // If we're not past the threshold value, keep the lights on
                            else
                            {
                                CurrentState = GreenhouseState.LIGHTING;
                                return GreenhouseState.NO_CHANGE;
                            }
                        }
                        else if (ScheduleType == ScheduleTypes.BLOCKED)
                        {
                            return GreenhouseState.WAITING_FOR_DATA;
                        }
                        // If they don't want the schedule to be overridden by sensor values, just keep the lights on
                        else if (ScheduleType == ScheduleTypes.CONSTANT)
                        {
                            CurrentState = GreenhouseState.LIGHTING;
                            return GreenhouseState.NO_CHANGE;
                        }
                        // If we don't meet any of those conditions, something bad happened!
                        else
                        {
                            return GreenhouseState.ERROR;
                        }
                    }
                    // If we're no longer within the scheduled time, turn the lights off
                    else
                    {
                        return GreenhouseState.WAITING_FOR_DATA;
                    }
                }
                // If neither, something went wrong
                else
                {
                    CurrentState = GreenhouseState.ERROR;
                    return GreenhouseState.ERROR;
                }
            }
            // Turn on the lights
            else if (ManualLight == true)
            {
                // If we're already lighting, don't send the command
                if (CurrentState == GreenhouseState.PROCESSING_LIGHTING)
                {
                    CurrentState = GreenhouseState.LIGHTING;
                    return GreenhouseState.NO_CHANGE;
                }
                // Otherwise send the command
                else
                {
                    return GreenhouseState.LIGHTING;
                }
            }
            // TURN THOSE LIGHTS OFF, NO, NO, TURN THOSE LIGHTS OFF!
            //else if (ManualLight == false)
            //{
            //    // If the lights are already off, don't do anything
            //    if (CurrentState == GreenhouseState.PROCESSING_DATA)
            //    {
            //        CurrentState = GreenhouseState.WAITING_FOR_DATA;
            //        return GreenhouseState.NO_CHANGE;
            //    }
            //    // If the lights aren't off already, send the command to turn them off
            //    else
            //    {
            //        return GreenhouseState.WAITING_FOR_DATA;
            //    }
            //}
            // If somehow we don't meet the above criteria, return an error
            else
            {
                CurrentState = GreenhouseState.ERROR;
                return GreenhouseState.ERROR;
            }
        }

        /// <summary>
        /// Convert a greenhouse state to a list of commands to send to the Arduino
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            List<Commands> commandsToSend = new List<Commands>();
            if (state == GreenhouseState.LIGHTING)
            {
                switch (Zone)
                {
                    case 1:
                        commandsToSend.Add(Commands.LIGHT1_ON);
                        break;
                    case 2:
                        commandsToSend.Add(Commands.LIGHT2_ON);
                        break;
                    case 3:
                        commandsToSend.Add(Commands.LIGHT3_ON);
                        break;
                    default:
                        break;
                }
            }
            else if (state == GreenhouseState.WAITING_FOR_DATA)
            {
                switch (Zone)
                {
                    case 1:
                        commandsToSend.Add(Commands.LIGHT1_OFF);
                        break;
                    case 2:
                        commandsToSend.Add(Commands.LIGHT2_OFF);
                        break;
                    case 3:
                        commandsToSend.Add(Commands.LIGHT3_OFF);
                        break;
                    default:
                        break;
                }
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

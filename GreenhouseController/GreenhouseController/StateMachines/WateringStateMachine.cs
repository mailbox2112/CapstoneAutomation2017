using GreenhouseController.Limits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class WateringStateMachine : ITimeBasedStateMachine
    {
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

        public DateTime Begin { get; set; }

        public DateTime End { get; set; }

        public int Zone { get; set; }

        public bool? ManualWater { get; set; }

        public double? OverrideThreshold { get; set; }

        public ScheduleTypes ScheduleType { get; set;}

        /// <summary>
        /// Initialize the state machine
        /// </summary>
        public WateringStateMachine(int zone)
        {
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
            ScheduleType = ScheduleTypes.CONSTANT;
            Zone = zone;
        }

        /// <summary>
        /// Determine the state of the greenhouse based on moisture data we receive
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GreenhouseState DetermineState(DateTime currentTime, double value)
        {
            // Figure out which processing state we're in. If we were watering,
            // we go to the Processing_water state
            // Otherwise, we just go to Processing_data
            if (CurrentState == GreenhouseState.WATERING)
            {
                CurrentState = GreenhouseState.PROCESSING_WATER;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            // If we have no manual water commands
            if (ManualWater != true)
            {
                // If we're coming from the wait state
                if (CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    // If we're within the scheduled time
                    if (currentTime.TimeOfDay >= Begin.TimeOfDay && currentTime.TimeOfDay <= End.TimeOfDay)
                    {
                        // If the user wants the sensors to override the schedule
                        if (ScheduleType == ScheduleTypes.SENSORS)
                        {
                            // Check if we're above the override threshold
                            // If so, don't turn the water on
                            if (value >= OverrideThreshold)
                            {
                                CurrentState = GreenhouseState.WAITING_FOR_DATA;
                                return GreenhouseState.NO_CHANGE;
                            }
                            // If not, turn the water on
                            else
                            {
                                return GreenhouseState.WATERING;
                            }
                        }
                        // Otherwise check if we need to just turn the water on
                        // If we're not blocked out of the scheduled time, don't do anything
                        else if (ScheduleType == ScheduleTypes.BLOCKED)
                        {
                            CurrentState = GreenhouseState.WAITING_FOR_DATA;
                            return GreenhouseState.NO_CHANGE;
                        }
                        else if (ScheduleType == ScheduleTypes.CONSTANT)
                        {
                            return GreenhouseState.WATERING;
                        }
                        // If we somehow didn't hit any of these conditions, error
                        else
                        {
                            return GreenhouseState.ERROR;
                        }
                    }
                    // If we're outside of the scheduled time, 
                    // since we're coming from the wait state, we just don't change anything
                    else
                    {
                        CurrentState = GreenhouseState.WAITING_FOR_DATA;
                        return GreenhouseState.NO_CHANGE;
                    }
                }
                // If we're coming from the watering state
                else if (CurrentState == GreenhouseState.PROCESSING_WATER)
                {
                    // If we're within the scheduled time
                    if (currentTime.TimeOfDay >= Begin.TimeOfDay && currentTime.TimeOfDay <= End.TimeOfDay)
                    {
                        // If the user wants the sensors to override the schedule
                        if (ScheduleType == ScheduleTypes.SENSORS)
                        {
                            // Check if the moisture if past the override threshold
                            // If so, turn the water off
                            if (value >= OverrideThreshold)
                            {
                                return GreenhouseState.WAITING_FOR_DATA;
                            }
                            // If not, keep it on
                            else
                            {
                                CurrentState = GreenhouseState.WATERING;
                                return GreenhouseState.NO_CHANGE;
                            }
                        }
                        else if (ScheduleType == ScheduleTypes.BLOCKED)
                        {
                            return GreenhouseState.WAITING_FOR_DATA;
                        }
                        // Otherwise, just keep the water on
                        else if (ScheduleType == ScheduleTypes.CONSTANT)
                        {
                            CurrentState = GreenhouseState.WATERING;
                            return GreenhouseState.NO_CHANGE;
                        }
                        else
                        {
                            return GreenhouseState.ERROR;
                        }
                    }
                    // If we're outside the scheduled time,
                    // turn the water off
                    else
                    {
                        return GreenhouseState.WAITING_FOR_DATA;
                    }
                }
                // If we're not in either of those states, something has gone wrong
                else
                {
                    return GreenhouseState.ERROR;
                }
                    
            }
            // If we have a manual water ON command
            else if (ManualWater == true)
            {
                // If we're coming from the watering state, the water is already on
                // so we just don't change anything
                if (CurrentState == GreenhouseState.PROCESSING_WATER)
                {
                    CurrentState = GreenhouseState.WATERING;
                    return GreenhouseState.NO_CHANGE;
                }
                // Otherwise, we turn the water on
                else
                {
                    return GreenhouseState.WATERING;
                }
            }
            // If we have a manual water OFF command
            //else if (ManualWater == false)
            //{
            //    // If we're coming from the wait state, don't do anything differently
            //    if (CurrentState == GreenhouseState.PROCESSING_DATA)
            //    {
            //        CurrentState = GreenhouseState.WAITING_FOR_DATA;
            //        return GreenhouseState.NO_CHANGE;
            //    }
            //    // Otherwise, we force the water off
            //    else
            //    {
            //        return GreenhouseState.WAITING_FOR_DATA;
            //    }
            //}
            // If out ManualWater field is somehow none of the three, we throw an error
            else
            {
                return GreenhouseState.ERROR;
            }
        }

        /// <summary>
        /// Return a list of commands appropriate for the action required by the state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            // Check the zone of the state machine calling this
            // and return the appropriate command
            List<Commands> commandsToSend = new List<Commands>();
            if (state == GreenhouseState.WATERING)
            { 
                switch (Zone)
                {
                    case 1:
                        commandsToSend.Add(Commands.WATER1_ON);
                        break;
                    case 2:
                        commandsToSend.Add(Commands.WATER2_ON);
                        break;
                    case 3:
                        commandsToSend.Add(Commands.WATER3_ON);
                        break;
                    case 4:
                        commandsToSend.Add(Commands.WATER4_ON);
                        break;
                    case 5:
                        commandsToSend.Add(Commands.WATER5_ON);
                        break;
                    case 6:
                        commandsToSend.Add(Commands.WATER6_ON);
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
                        commandsToSend.Add(Commands.WATER1_OFF);
                        break;
                    case 2:
                        commandsToSend.Add(Commands.WATER2_OFF);
                        break;
                    case 3:
                        commandsToSend.Add(Commands.WATER3_OFF);
                        break;
                    case 4:
                        commandsToSend.Add(Commands.WATER4_OFF);
                        break;
                    case 5:
                        commandsToSend.Add(Commands.WATER5_OFF);
                        break;
                    case 6:
                        commandsToSend.Add(Commands.WATER6_OFF);
                        break;
                    default:
                        break;
                }
            }

            return commandsToSend;
        }

        /// <summary>
        /// When the greenhouse state changes, invoke this event handler
        /// </summary>
        /// <param name="e"></param>
        public void OnStateChange(StateEventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class WateringStateMachine : ITimeBasedStateMachine
    {
        // TODO: add moisture sensor override stuff
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

        public double MoistureThreshold { get; set; }

        /// <summary>
        /// Initialize the state machine
        /// </summary>
        public WateringStateMachine(int zone)
        {
            MoistureThreshold = 70;
            CurrentState = GreenhouseState.WAITING_FOR_DATA;
            Zone = zone;
        }

        /// <summary>
        /// Determine the state of the greenhouse based on moisture data we receive
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public GreenhouseState DetermineState(DateTime currentTime, double value)
        {
            if (CurrentState == GreenhouseState.WATERING)
            {
                CurrentState = GreenhouseState.PROCESSING_WATER;
            }
            else
            {
                CurrentState = GreenhouseState.PROCESSING_DATA;
            }

            if (ManualWater != true)
            {
                // Check the states based on data, and if we were already watering take that into account
                if (value > MoistureThreshold && CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    CurrentState = GreenhouseState.WAITING_FOR_DATA;
                    return GreenhouseState.NO_CHANGE;
                }
                else if (value > MoistureThreshold && CurrentState == GreenhouseState.PROCESSING_WATER)
                {
                    return GreenhouseState.WAITING_FOR_DATA;
                }
                else if (currentTime < End && currentTime > Begin && CurrentState != GreenhouseState.PROCESSING_WATER)
                {
                    return GreenhouseState.WATERING;
                }
                else if (currentTime < End && currentTime > Begin && CurrentState == GreenhouseState.PROCESSING_WATER)
                {
                    CurrentState = GreenhouseState.WATERING;
                    return GreenhouseState.NO_CHANGE;
                }
                else if ((currentTime > End || currentTime < Begin) && CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    CurrentState = GreenhouseState.WAITING_FOR_DATA;
                    return GreenhouseState.NO_CHANGE;
                }
                else
                {
                    return GreenhouseState.WAITING_FOR_DATA;
                }
            }
            else if (ManualWater == true)
            {
                if (CurrentState == GreenhouseState.PROCESSING_WATER)
                {
                    CurrentState = GreenhouseState.WATERING;
                    return GreenhouseState.NO_CHANGE;
                }
                else
                {
                    return GreenhouseState.WATERING;
                }
            }
            //else if (ManualWater == false)
            //{
            //    ManualWater = null;
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
        /// Return a list of commands appropriate for the action required by the state
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public List<Commands> ConvertStateToCommands(GreenhouseState state)
        {
            // check the state of the solenoids
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

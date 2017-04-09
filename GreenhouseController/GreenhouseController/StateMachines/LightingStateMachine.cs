using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class LightingStateMachine : ITimeBasedStateMachine
    {
        // TODO: Add lighting sensor value override stuff
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

            if (ManualLight != true)
            {
                // Process data and take into account if we were already lighting when we received the data
                if (currentTime.TimeOfDay < End.TimeOfDay && currentTime.TimeOfDay > Begin.TimeOfDay && CurrentState != GreenhouseState.PROCESSING_LIGHTING)
                {
                    return GreenhouseState.LIGHTING;
                }
                else if (currentTime.TimeOfDay < End.TimeOfDay && currentTime.TimeOfDay > Begin.TimeOfDay && CurrentState == GreenhouseState.PROCESSING_LIGHTING)
                {
                    CurrentState = GreenhouseState.LIGHTING;
                    return GreenhouseState.NO_CHANGE;
                }
                else if ((currentTime.TimeOfDay > End.TimeOfDay || currentTime.TimeOfDay < Begin.TimeOfDay) && CurrentState == GreenhouseState.PROCESSING_DATA)
                {
                    CurrentState = GreenhouseState.WAITING_FOR_DATA;
                    return GreenhouseState.NO_CHANGE;
                }
                else
                {
                    return GreenhouseState.WAITING_FOR_DATA;
                }
            }
            else if (ManualLight == true)
            {
                if (CurrentState == GreenhouseState.PROCESSING_LIGHTING)
                {
                    CurrentState = GreenhouseState.LIGHTING;
                    return GreenhouseState.NO_CHANGE;
                }
                else
                {
                    return GreenhouseState.LIGHTING;
                }
            }
            //else if (ManualLight == false)
            //{
            //    ManualLight = null;
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

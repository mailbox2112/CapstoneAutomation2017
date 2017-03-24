using GreenhouseController.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class ActionAnalyzer
    {
        //TODO: Fix manual control
        private double _avgTemp;
        private double _avgLight;
        private DateTime _currentTime;
        private KeyValuePair<IStateMachine, GreenhouseState> _tempState;
        private KeyValuePair<IStateMachine, GreenhouseState> _shadeState;
        private KeyValuePair<ITimeBasedStateMachine, GreenhouseState> _lightState;
        private KeyValuePair<ITimeBasedStateMachine, GreenhouseState> _waterState;

        public ActionAnalyzer()
        {
            _avgTemp = new double();
            _avgLight = new double();
        }

        /// <summary>
        /// Processes the data received from the packets
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        public void AnalyzeData(TLHPacket[] tlhData, MoisturePacket[] moistData, DateTime currentTime)
        {
            _currentTime = currentTime;
            ArduinoControlSender.Instance.TryConnect();

            List<GreenhouseState> statesToSend = new List<GreenhouseState>();

            #region Automation Decision Making
            // Get the averages of greenhouse readings
            GetTemperatureAverage(tlhData);

            // Determine what state we need to go to and then create a KVP for it and send it
            GreenhouseState goalTempState = StateMachineContainer.Instance.Temperature.DetermineState(_avgTemp);
            if (goalTempState == GreenhouseState.HEATING || goalTempState == GreenhouseState.COOLING || goalTempState == GreenhouseState.WAITING_FOR_DATA)
            {
                _tempState = new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState);
                // Send the KVP to the control sender
                ArduinoControlSender.Instance.SendCommand(_tempState);
            }

            // Get state for shading state machine, send commands
            GreenhouseState goalShadeState = StateMachineContainer.Instance.Shading.DetermineState(_avgLight);
            if (goalShadeState == GreenhouseState.SHADING || goalShadeState == GreenhouseState.WAITING_FOR_DATA)
            {
                _shadeState = new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Shading, goalShadeState);
                ArduinoControlSender.Instance.SendCommand(_shadeState);
            }

            // Get state for lighting state machines, send commands
            for (int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i ++)
            {
                GreenhouseState goalLightState = StateMachineContainer.Instance.LightStateMachines[i].DetermineState(_currentTime);
                if (goalLightState == GreenhouseState.LIGHTING || goalLightState == GreenhouseState.SHADING || goalLightState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _lightState = new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(StateMachineContainer.Instance.LightStateMachines[i], goalLightState);
                    ArduinoControlSender.Instance.SendCommand(_lightState);
                }
            }

            // Get states for watering state machines, send commands
            for(int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i ++)
            {
                GreenhouseState goalWaterState = StateMachineContainer.Instance.WateringStateMachines[i].DetermineState(_currentTime);
                if (goalWaterState == GreenhouseState.WATERING || goalWaterState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _waterState = new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(StateMachineContainer.Instance.WateringStateMachines[i], goalWaterState);
                    ArduinoControlSender.Instance.SendCommand(_waterState);
                }
            }
            #endregion
        }

        /// <summary>
        /// Activate manual control of devices in the greenhouse
        /// </summary>
        public void ActivateManualControl()
        {
            // If we have manual commands from temperature state machines
            if (StateMachineContainer.Instance.Temperature.ManualCool != null)
            {
                GreenhouseState goalTempState = StateMachineContainer.Instance.Temperature.DetermineState();
                ArduinoControlSender.Instance.SendCommand(new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState));
            }
            else if (StateMachineContainer.Instance.Temperature.ManualHeat != null)
            {
                GreenhouseState goalTempState = StateMachineContainer.Instance.Temperature.DetermineState();
                ArduinoControlSender.Instance.SendCommand(new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState));
            }

            // If we have a manual command for the shading state machine
            if (StateMachineContainer.Instance.Shading.ManualShade != null)
            {
                GreenhouseState goalShadeState = StateMachineContainer.Instance.Shading.DetermineState();
                ArduinoControlSender.Instance.SendCommand(new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Shading, goalShadeState));
            }

            // If we have manual commands for the lighting state machines
            for (int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i++)
            {
                if (StateMachineContainer.Instance.LightStateMachines[i].ManualLight != null)
                {
                    GreenhouseState goalLightState = StateMachineContainer.Instance.LightStateMachines[i].DetermineState(DateTime.Now);
                    ArduinoControlSender.Instance.SendCommand(
                        new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(StateMachineContainer.Instance.LightStateMachines[i], goalLightState));
                }
            }
            
            // If we have manual commands for the watering state machines
            for(int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i++)
            {
                if (StateMachineContainer.Instance.WateringStateMachines[i].ManualWater != null)
                {
                    GreenhouseState goalWaterState = StateMachineContainer.Instance.WateringStateMachines[i].DetermineState(DateTime.Now);
                    ArduinoControlSender.Instance.SendCommand(
                        new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(StateMachineContainer.Instance.WateringStateMachines[i], goalWaterState));
                }
            }
        }

        /// <summary>
        /// Helper method for averaging greenhouse data
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        private void GetTemperatureAverage(TLHPacket[] data)
        {
            foreach (TLHPacket pack in data)
            {
                _avgTemp += pack.Temperature;
                _avgLight += pack.Light;
            }
            _avgTemp /= 5;
            _avgLight /= 5;
        }

        private DateTime GetCurrentTime()
        {
            // Get the approximate time from the data packet array
            return DateTime.Now;
        }
    }
}

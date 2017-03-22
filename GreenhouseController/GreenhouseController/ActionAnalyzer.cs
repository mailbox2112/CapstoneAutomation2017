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
        private DateTime _currentTime;
        private KeyValuePair<TemperatureStateMachine, GreenhouseState> _tempState;

        public ActionAnalyzer()
        {
            _avgTemp = new double();
        }

        /// <summary>
        /// Processes the data received from the packets
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        public void AnalyzeData(TLHPacket[] tlhData, MoisturePacket[] moistData, DateTime currentTime)
        {
            _currentTime = currentTime;
            ArduinoControlSender.Instance.TryConnect();
            // If any of the packets have a value for manual control in them, we change the manual variables
            // otherwise they stay null

            List<GreenhouseState> statesToSend = new List<GreenhouseState>();

            #region Automation Decision Making
            // Get the averages of greenhouse readings
            GetTemperatureAverage(tlhData);

            // TODO: Check data for shading state machine
            // Get state machine states as long as we don't have a manual command change to send
            // Determine what state we need to go to and then create a KVP for it
            GreenhouseState goalTempState = StateMachineContainer.Instance.Temperature.DetermineState(_avgTemp);
            if (goalTempState == GreenhouseState.HEATING || goalTempState == GreenhouseState.COOLING || goalTempState == GreenhouseState.WAITING_FOR_DATA)
            {
                _tempState = new KeyValuePair<TemperatureStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState);
            }

            // Send the KVP to the control sender
            ArduinoControlSender.Instance.SendCommand(_tempState);
            // If we don't have a manual light/shade command...
            // Zone 1
            for(int i = 0; i < StateMachineContainer.Instance.LightStateMachines.Count; i ++)
            {
                GreenhouseState goalLightState = StateMachineContainer.Instance.LightStateMachines[i].DetermineState(_currentTime);
                if (goalLightState == GreenhouseState.LIGHTING || goalLightState == GreenhouseState.SHADING || goalLightState == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.LightStateMachines[i], goalLightState);
                }
            }

            //GreenhouseState goalLightState1 = StateMachineContainer.Instance.LightingZone1.DetermineState(_currentTime);
            //if (goalLightState1 == GreenhouseState.LIGHTING || goalLightState1 == GreenhouseState.SHADING || goalLightState1 == GreenhouseState.WAITING_FOR_DATA)
            //{
            //    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.LightingZone1, goalLightState1);
            //}
                

            //// Zone 3
            //GreenhouseState goalLightState3 = StateMachineContainer.Instance.LightingZone3.DetermineState(_currentTime);
            //if (goalLightState3 == GreenhouseState.LIGHTING || goalLightState3 == GreenhouseState.SHADING || goalLightState3 == GreenhouseState.WAITING_FOR_DATA)
            //{
            //    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.LightingZone3, goalLightState3);
            //}
                

            //// Zone 5
            //GreenhouseState goalLightState5 = StateMachineContainer.Instance.LightingZone5.DetermineState(_currentTime);
            //if (goalLightState5 == GreenhouseState.LIGHTING || goalLightState5 == GreenhouseState.SHADING || goalLightState5 == GreenhouseState.WAITING_FOR_DATA)
            //{
            //    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.LightingZone5, goalLightState5);
            //}

            // If we don't have a manual watering command
            // Zone 1
            for(int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i ++)
            {
                GreenhouseState goalWaterState = StateMachineContainer.Instance.WateringStateMachines[i].DetermineState(_currentTime);
                if (goalWaterState == GreenhouseState.WATERING || goalWaterState == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringStateMachines[i], goalWaterState);
                }
            }
            //GreenhouseState goalWaterState1 = StateMachineContainer.Instance.WateringZone1.DetermineState(_currentTime);
            //if (goalWaterState1 == GreenhouseState.WATERING || goalWaterState1 == GreenhouseState.WAITING_FOR_DATA)
            //{
            //    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone1, goalWaterState1);
//        }
//        // Zone 2
//        GreenhouseState goalWaterState2 = StateMachineContainer.Instance.WateringZone2.DetermineState(_currentTime);
//            if (goalWaterState2 == GreenhouseState.WATERING || goalWaterState2 == GreenhouseState.WAITING_FOR_DATA)
//            {
//                ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone2, goalWaterState2);
//            }
//            // Zone 3
//            GreenhouseState goalWaterState3 = StateMachineContainer.Instance.WateringZone3.DetermineState(_currentTime);
//            if (goalWaterState3 == GreenhouseState.WATERING || goalWaterState3 == GreenhouseState.WAITING_FOR_DATA)
//            {
//                ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone3, goalWaterState3);
//            }
//            // Zone 4
//            GreenhouseState goalWaterState4 = StateMachineContainer.Instance.WateringZone4.DetermineState(_currentTime);
//            if (goalWaterState4 == GreenhouseState.WATERING || goalWaterState4 == GreenhouseState.WAITING_FOR_DATA)
//            {
//                ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone4, goalWaterState4);
//            }
//            // Zone 5
//            GreenhouseState goalWaterState5 = StateMachineContainer.Instance.WateringZone5.DetermineState(_currentTime);
//            if (goalWaterState5 == GreenhouseState.WATERING || goalWaterState5 == GreenhouseState.WAITING_FOR_DATA)
//            {
//                ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone5, goalWaterState5);
//            }
//            // Zone 6
//            GreenhouseState goalWaterState6 = StateMachineContainer.Instance.WateringZone6.DetermineState(_currentTime);
//            if (goalWaterState6 == GreenhouseState.WATERING || goalWaterState6 == GreenhouseState.WAITING_FOR_DATA)
//            {
//                ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone6, goalWaterState6);
//            }
            #endregion
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
            }
            _avgTemp /= 5;
        }

        private void GetCurrentTime(DataPacket[] data)
        {
            // Get the approximate time from the data packet array
        }
    }
}

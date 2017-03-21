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
        private bool? _manualHeat;
        private bool? _manualCool;
        private bool? _manualLight;
        private bool? _manualWater;
        private bool? _manualShade;
        private KeyValuePair<TemperatureStateMachine, GreenhouseState> _tempState;

        public ActionAnalyzer()
        {
            _avgTemp = new double();
            _manualCool = null;
            _manualHeat = null;
            _manualLight = null;
            _manualWater = null;
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
            //foreach (var packet in data)
            //{
            //    if (packet.ManualHeat != null)
            //    {
            //        _manualHeat = packet.ManualHeat;
            //    }
            //    if (packet.ManualCool != null)
            //    {
            //        _manualCool = packet.ManualCool;
            //    }
            //    // TODO: Account for manual lighting in the different zones!
            //    if (packet.ManualLight != null)
            //    {
            //        _manualLight = packet.ManualLight;
            //    }
            //    if (packet.ManualWater != null)
            //    {
            //        _manualWater = packet.ManualWater;
            //    }
            //    if (packet.ManualShade != null)
            //    {
            //        _manualShade = packet.ManualShade;
            //    }
            //}

            List<GreenhouseState> statesToSend = new List<GreenhouseState>();

            #region Automation Decision Making
            // Get the averages of greenhouse readings
            GetTemperatureAverage(tlhData);

            // TODO: Check data for shading state machine
            // Get state machine states as long as we don't have a manual command change to send
            // If we don't have any manual temperature commands...
            if (_manualHeat == null && _manualCool == null)
            {
                // Determine what state we need to go to and then create a KVP for it
                GreenhouseState goalTempState = StateMachineContainer.Instance.Temperature.DetermineState(_avgTemp);
                if (goalTempState == GreenhouseState.HEATING || goalTempState == GreenhouseState.COOLING || goalTempState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _tempState = new KeyValuePair<TemperatureStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState);
                }

                // Send the KVP to the control sender
                ArduinoControlSender.Instance.SendCommand(_tempState);
            }
            //else
            //{
            //    // If we have a manual command, do that
            //    if (_manualHeat == true)
            //    {
            //        GreenhouseState goalTempState = GreenhouseState.HEATING;
            //        _tempState = new KeyValuePair<TemperatureStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState);
            //    }
            //    else if (_manualCool == true)
            //    {
            //        GreenhouseState goalTempState = GreenhouseState.COOLING;
            //        _tempState = new KeyValuePair<TemperatureStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState);
            //    }
            //    else if (_manualHeat == false || _manualCool == false)
            //    {
            //        ArduinoControlSender.Instance.SendManualOffCommand(StateMachineContainer.Instance.Temperature);
            //    }
            //}
            // If we don't have a manual light/shade command...
            if (_manualLight == null && _manualShade == null)
            {
                // Zone 1
                GreenhouseState goalLightState1 = StateMachineContainer.Instance.LightingZone1.DetermineState(_currentTime);
                if (goalLightState1 == GreenhouseState.LIGHTING || goalLightState1 == GreenhouseState.SHADING || goalLightState1 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.LightingZone1, goalLightState1);
                }
                

                // Zone 3
                GreenhouseState goalLightState3 = StateMachineContainer.Instance.LightingZone3.DetermineState(_currentTime);
                if (goalLightState3 == GreenhouseState.LIGHTING || goalLightState3 == GreenhouseState.SHADING || goalLightState3 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.LightingZone3, goalLightState3);
                }
                

                // Zone 5
                GreenhouseState goalLightState5 = StateMachineContainer.Instance.LightingZone5.DetermineState(_currentTime);
                if (goalLightState5 == GreenhouseState.LIGHTING || goalLightState5 == GreenhouseState.SHADING || goalLightState5 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.LightingZone5, goalLightState5);
                }
            }
            //else
            //{
            //    // If we do have a manual command, do that
            //    if (_manualLight == true)
            //    {
            //        GreenhouseState goalLightState = GreenhouseState.LIGHTING;
            //        _statesToSend.Add(new LightingStateMachine(), goalLightState);
            //    }
            //    else if (_manualShade == true)
            //    {
            //        GreenhouseState goalLightState = GreenhouseState.SHADING;
            //        _statesToSend.Add(new LightingStateMachine(), goalLightState);
            //    }
            //    else
            //    {
            //        ArduinoControlSender.Instance.SendManualOffCommand(StateMachineContainer.Instance.Lighting);
            //    }
            //}
            // If we don't have a manual watering command
            if (_manualWater == null)
            {
                // Zone 1
                GreenhouseState goalWaterState1 = StateMachineContainer.Instance.WateringZone1.DetermineState(_currentTime);
                if (goalWaterState1 == GreenhouseState.WATERING || goalWaterState1 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone1, goalWaterState1);
                }
                // Zone 2
                GreenhouseState goalWaterState2 = StateMachineContainer.Instance.WateringZone2.DetermineState(_currentTime);
                if (goalWaterState2 == GreenhouseState.WATERING || goalWaterState2 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone2, goalWaterState2);
                }
                // Zone 3
                GreenhouseState goalWaterState3 = StateMachineContainer.Instance.WateringZone3.DetermineState(_currentTime);
                if (goalWaterState3 == GreenhouseState.WATERING || goalWaterState3 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone3, goalWaterState3);
                }
                // Zone 4
                GreenhouseState goalWaterState4 = StateMachineContainer.Instance.WateringZone4.DetermineState(_currentTime);
                if (goalWaterState4 == GreenhouseState.WATERING || goalWaterState4 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone4, goalWaterState4);
                }
                // Zone 5
                GreenhouseState goalWaterState5 = StateMachineContainer.Instance.WateringZone5.DetermineState(_currentTime);
                if (goalWaterState5 == GreenhouseState.WATERING || goalWaterState5 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone5, goalWaterState5);
                }
                // Zone 6
                GreenhouseState goalWaterState6 = StateMachineContainer.Instance.WateringZone6.DetermineState(_currentTime);
                if (goalWaterState6 == GreenhouseState.WATERING || goalWaterState6 == GreenhouseState.WAITING_FOR_DATA)
                {
                    ArduinoControlSender.Instance.SendCommand(StateMachineContainer.Instance.WateringZone6, goalWaterState6);
                }
            }
            //else
            //{
            //    // If we have a manual command, do that
            //    if (_manualWater == true)
            //    {
            //        GreenhouseState goalWaterState = GreenhouseState.WATERING;
            //        _statesToSend.Add(new WateringStateMachine(), goalWaterState);
            //    }
            //    else
            //    {
            //        ArduinoControlSender.Instance.SendManualOffCommand(StateMachineContainer.Instance.Watering);
            //    }
            //}

            // Send commands
            //foreach (var state in _statesToSend)
            //{
            //    // Send commands
            //    ArduinoControlSender.Instance.SendCommand(state);
            //}
            #endregion

            //// If our state ends up being in emergency
            //if (StateMachineContainer.Instance.Watering.CurrentState == GreenhouseState.EMERGENCY)
            //{
            //    // TODO: Send an emergency message to the Data Team!
            //}
            //else if (StateMachineContainer.Instance.Watering.CurrentState == GreenhouseState.ERROR)
            //{
            //    // TODO: Set a flag somewhere!
            //}
            //if (StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.EMERGENCY)
            //{
            //    // TODO: Send an emergency message to the Data Team!
            //}
            //else if (StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.ERROR)
            //{
            //    // TODO: Set a flag somewhere!
            //}
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

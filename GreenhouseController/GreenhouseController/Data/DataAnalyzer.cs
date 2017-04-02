using GreenhouseController.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class DataAnalyzer
    {
        private double _avgTemp;
        private double _avgLight;
        private DateTime _currentTime;
        private KeyValuePair<IStateMachine, GreenhouseState> _tempState;
        private KeyValuePair<IStateMachine, GreenhouseState> _shadeState;
        private KeyValuePair<ITimeBasedStateMachine, GreenhouseState> _lightState;
        private KeyValuePair<ITimeBasedStateMachine, GreenhouseState> _waterState;

        public DataAnalyzer()
        {
            _avgTemp = 0.0;
            _avgLight = 0.0;
        }

        public void ExecuteActions(TLHPacket[] temperature, MoisturePacket[] moisture, ManualPacket manual, LimitPacket limits)
        {
            // Process limit changes
            LimitsAnalyzer limitAnalyzer = new LimitsAnalyzer();
            limitAnalyzer.ChangeGreenhouseLimits(limits);
            // Process manual controls
            ManualPacketAnalyzer manualAnalyzer = new ManualPacketAnalyzer();
            manualAnalyzer.SetManualValues(manual);
            // Process sensor data
            AnalyzeData(temperature, moisture);
        }

        /// <summary>
        /// Processes the data received from the packets
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        private void AnalyzeData(TLHPacket[] tlhData, MoisturePacket[] moistData)
        {
            _currentTime = GetCurrentTime(tlhData);
            ArduinoControlSender.Instance.TryConnect();

            List<GreenhouseState> statesToSend = new List<GreenhouseState>();

            #region Automation Decision Making
            // Get the averages of greenhouse readings
            _avgTemp = GetTemperatureAverage(tlhData);
            _avgLight = GetLightAverage(tlhData);

            // Determine what state we need to go to and then create a KVP for it and send it
            GreenhouseState goalTempState = StateMachineContainer.Instance.Temperature.DetermineState(_avgTemp);
            if (goalTempState == GreenhouseState.HEATING || goalTempState == GreenhouseState.COOLING || goalTempState == GreenhouseState.WAITING_FOR_DATA)
            {
                _tempState = new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState);
                // Send the KVP to the control sender
                ArduinoControlSender.Instance.SendCommand(_tempState);
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

            // Get state for shading state machine, send commands
            GreenhouseState goalShadeState = StateMachineContainer.Instance.Shading.DetermineState(_avgLight);
            if (goalShadeState == GreenhouseState.SHADING || goalShadeState == GreenhouseState.WAITING_FOR_DATA)
            {
                _shadeState = new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Shading, goalShadeState);
                ArduinoControlSender.Instance.SendCommand(_shadeState);
            }
            #endregion
        }

        /// <summary>
        /// Helper method for averaging greenhouse data
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        private double GetTemperatureAverage(TLHPacket[] data)
        {
            double avg = 0.0;
            foreach (TLHPacket pack in data)
            {
                avg += pack.Temperature;
            }
            //_avgTemp /= 5;
            //_avgLight /= 5;

            avg /= 2.0;
            Console.WriteLine("Average Temp: " + avg.ToString());
            return avg;
        }

        private double GetLightAverage(TLHPacket[] data)
        {
            double avg = 0.0;
            foreach (TLHPacket pack in data)
            {
                avg += pack.Light;
            }
            //_avgTemp /= 5;
            //_avgLight /= 5;

            avg /= 2.0;
            Console.WriteLine("Average Light: " + avg.ToString());
            return avg;
        }

        private DateTime GetCurrentTime(TLHPacket[] data)
        {
            DateTime now = new DateTime();
            foreach(TLHPacket packet in data)
            {
                if (packet.TimeOfSend > now)
                {
                    now = packet.TimeOfSend;
                }
            }
            return now;
        }
    }
}

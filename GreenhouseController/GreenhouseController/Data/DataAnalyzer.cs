using GreenhouseController.API;
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

        /// <summary>
        /// Execute the actions required for proper greenhouse functioning, based on the input data
        /// </summary>
        /// <param name="temperature">Array of temperature, lighting, and humidity information packets</param>
        /// <param name="moisture">Array of moisture information packets</param>
        /// <param name="manual">Packet containing any manual commands we may have received</param>
        /// <param name="limits">Packet containng the greenhouse automation limits</param>
        public void ExecuteActions(TLHPacket[] temperature, MoisturePacket[] moisture, ManualPacket manual, LimitPacket limits)
        {
            ArduinoControlSender.Instance.CheckArduinoStatus();
            // Process limit changes
            LimitsAnalyzer limitAnalyzer = new LimitsAnalyzer();
            limitAnalyzer.ChangeGreenhouseLimits(limits);
            // Process manual controls
            ArduinoControlSender.Instance.CheckArduinoStatus();
            ManualPacketAnalyzer manualAnalyzer = new ManualPacketAnalyzer();
            manualAnalyzer.SetManualValues(manual);
            // Process sensor data
            ArduinoControlSender.Instance.CheckArduinoStatus();
            AnalyzeData(temperature, moisture);
        }

        /// <summary>
        /// Execute the actions required for proper greenhouse functioning, based on the input data from API calls
        /// </summary>
        /// <param name="sensorData"></param>
        /// <param name="manualControls"></param>
        /// <param name="automationData"></param>
        public void ExecuteActions(SensorPacket[] sensorData, ManualControlPacket manualControls, AutomationPacket automationData)
        {
            // Make sure the Arduino stays awake
            ArduinoControlSender.Instance.CheckArduinoStatus();

            // Process limit changes
            LimitsAnalyzer limitAnalyzer = new LimitsAnalyzer();
            limitAnalyzer.ChangeGreenhouseLimits(automationData);

            // Make sure the Arduino stays awake
            ArduinoControlSender.Instance.CheckArduinoStatus();

            // Process manual data
            ManualPacketAnalyzer manualAnalyzer = new ManualPacketAnalyzer();
            manualAnalyzer.SetManualValues(manualControls);

            // Make sure the Arduino stays awake
            ArduinoControlSender.Instance.CheckArduinoStatus();
            // Process sensor data
            AnalyzeData(sensorData);
        }

        /// <summary>
        /// Processes the data received from the packets
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        private void AnalyzeData(TLHPacket[] tlhData, MoisturePacket[] moistData)
        {
            // Get the approximate current time from the packets
            _currentTime = GetCurrentTime(tlhData);

            // Make sure the Arduino is there
            ArduinoControlSender.Instance.CheckArduinoStatus();

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
            foreach (LightingStateMachine stateMachine in StateMachineContainer.Instance.LightStateMachines)
            {
                // Get the packet from the zone we're currently operating on
                TLHPacket packet = tlhData.Where(p => p.ID == stateMachine.Zone).Single();
                double lightingValue = packet.Light;
                GreenhouseState goalLightState = stateMachine.DetermineState(_currentTime, lightingValue);
                if (goalLightState == GreenhouseState.LIGHTING || goalLightState == GreenhouseState.SHADING || goalLightState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _lightState = new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(stateMachine, goalLightState);
                    ArduinoControlSender.Instance.SendCommand(_lightState);
                }
            }

            // Get states for watering state machines, send commands
            foreach (WateringStateMachine stateMachine in StateMachineContainer.Instance.WateringStateMachines)
            {
                // Get the packet from the zone we're currently operating on
                MoisturePacket packet = moistData.Where(p => p.ID == stateMachine.Zone).Single();
                double moistureValue = (packet.Probe1 + packet.Probe2) / 2;

                // Get the state we need to transition into, then go send a command appropriate to that
                GreenhouseState goalWaterState = stateMachine.DetermineState(_currentTime, moistureValue);
                if (goalWaterState == GreenhouseState.WATERING || goalWaterState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _waterState = new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(stateMachine, goalWaterState);
                    ArduinoControlSender.Instance.SendCommand(_waterState);
                }
            }

            // Get state for shading state machine, send commands
            GreenhouseState goalShadeState = StateMachineContainer.Instance.Shading.DetermineState(_avgTemp);
            if (goalShadeState == GreenhouseState.SHADING || goalShadeState == GreenhouseState.WAITING_FOR_DATA)
            {
                _shadeState = new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Shading, goalShadeState);
                ArduinoControlSender.Instance.SendCommand(_shadeState);
            }
            #endregion
        }

        private void AnalyzeData(SensorPacket[] sensorData)
        {
            // Get the approximate current time from the packets
            _currentTime = GetCurrentTime(sensorData);

            // Make sure the Arduino is there
            ArduinoControlSender.Instance.CheckArduinoStatus();

            #region Automation Decision Making
            // Get the averages of greenhouse readings
            _avgTemp = GetTemperatureAverage(sensorData);
            _avgLight = GetLightAverage(sensorData);

            // Determine what state we need to go to and then create a KVP for it and send it
            GreenhouseState goalTempState = StateMachineContainer.Instance.Temperature.DetermineState(_avgTemp);
            if (goalTempState == GreenhouseState.HEATING || goalTempState == GreenhouseState.COOLING || goalTempState == GreenhouseState.WAITING_FOR_DATA)
            {
                _tempState = new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Temperature, goalTempState);
                // Send the KVP to the control sender
                ArduinoControlSender.Instance.SendCommand(_tempState);
            }

            // Get state for lighting state machines, send commands
            foreach (LightingStateMachine stateMachine in StateMachineContainer.Instance.LightStateMachines)
            {
                // Get the packet from the zone we're currently operating on
                SensorPacket packet = sensorData.Where(p => p.Zone == stateMachine.Zone).Single();
                double lightingValue = packet.Light;
                GreenhouseState goalLightState = stateMachine.DetermineState(_currentTime, lightingValue);
                if (goalLightState == GreenhouseState.LIGHTING || goalLightState == GreenhouseState.SHADING || goalLightState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _lightState = new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(stateMachine, goalLightState);
                    ArduinoControlSender.Instance.SendCommand(_lightState);
                }
            }

            // Get states for watering state machines, send commands
            foreach (WateringStateMachine stateMachine in StateMachineContainer.Instance.WateringStateMachines)
            {
                // Get the packet from the zone we're currently operating on
                SensorPacket packet = sensorData.Where(p => p.Zone == stateMachine.Zone).Single();
                double moistureValue = (packet.Probe1 + packet.Probe2) / 2;

                // Get the state we need to transition into, then go send a command appropriate to that
                GreenhouseState goalWaterState = stateMachine.DetermineState(_currentTime, moistureValue);
                if (goalWaterState == GreenhouseState.WATERING || goalWaterState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _waterState = new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(stateMachine, goalWaterState);
                    ArduinoControlSender.Instance.SendCommand(_waterState);
                }
            }

            // Get state for shading state machine, send commands
            GreenhouseState goalShadeState = StateMachineContainer.Instance.Shading.DetermineState(_avgTemp);
            if (goalShadeState == GreenhouseState.SHADING || goalShadeState == GreenhouseState.WAITING_FOR_DATA)
            {
                _shadeState = new KeyValuePair<IStateMachine, GreenhouseState>(StateMachineContainer.Instance.Shading, goalShadeState);
                ArduinoControlSender.Instance.SendCommand(_shadeState);
            }
            #endregion
        }

        /// <summary>
        /// Helper method for averaging greenhouse temperatures
        /// </summary>
        /// <param name="data">Array of TLHPacket objects parsed from JSON sent via data server</param>
        private double GetTemperatureAverage(TLHPacket[] data)
        {
            double avg = 0.0;
            foreach (TLHPacket pack in data)
            {
                avg += pack.Temperature;
            }
            //avg /= 5.0;

            avg /= 2.0;
            Console.WriteLine("Average Temp: " + avg.ToString());
            return avg;
        }

        /// <summary>
        /// Helper method to get the average temperature from the API sensor data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private double GetTemperatureAverage(SensorPacket[] data)
        {
            double avg = 0.0;
            foreach (SensorPacket pack in data)
            {
                avg += pack.Temperature;
            }
            //avg /= 5.0;

            avg /= 2.0;
            Console.WriteLine("Average Temp: " + avg.ToString());
            return avg;
        }

        /// <summary>
        /// Helper method for averaging greenhouse light levels
        /// </summary>
        /// <param name="data">Array of TLHPacket objects parsed from JSON sent via data server</param>
        /// <returns></returns>
        private double GetLightAverage(TLHPacket[] data)
        {
            double avg = 0.0;
            foreach (TLHPacket pack in data)
            {
                avg += pack.Light;
            }
            //avg /= 5.0;

            avg /= 2.0;
            Console.WriteLine("Average Light: " + avg.ToString());
            return avg;
        }

        /// <summary>
        /// Helper method for averaging the greenhouse light levels from API data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private double GetLightAverage(SensorPacket[] data)
        {
            double avg = 0.0;
            foreach (SensorPacket pack in data)
            {
                avg += pack.Light;
            }
            //avg /= 5.0;

            avg /= 2.0;
            Console.WriteLine("Average Light: " + avg.ToString());
            return avg;
        }

        /// <summary>
        /// Helper method for getting the approximate current time of day
        /// </summary>
        /// <param name="data">Array of TLHPacket objects parsed frmo JSON sent via data server</param>
        /// <returns></returns>
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
            Console.WriteLine(now.ToString());
            return now;
        }

        /// <summary>
        /// Helper method to get current approximate time of day from the API data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private DateTime GetCurrentTime(SensorPacket[] data)
        {
            DateTime now = new DateTime();
            foreach (SensorPacket packet in data)
            {
                if (packet.SampleTime > now)
                {
                    now = packet.SampleTime;
                }
            }
            Console.WriteLine(now.ToString());
            return now;
        }
    }
}

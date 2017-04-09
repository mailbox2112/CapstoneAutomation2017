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
            // Get the approximate current time from the packets
            _currentTime = GetCurrentTime(tlhData);

            // Try to connect to the Arduino if we aren't already
            ArduinoControlSender.Instance.TryConnect();

            // Make sure the Arduino is there
            bool result = ArduinoControlSender.Instance.CheckArduinoStatus();

            // Do something if there's a problem with the Arduino
            if (!result)
            {

            }

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
                // Get the packet from the zone we're currently operating on
                TLHPacket packet = tlhData.Where(p => p.ID == StateMachineContainer.Instance.LightStateMachines[i].Zone).Single();
                double lightingValue = packet.Light;
                GreenhouseState goalLightState = StateMachineContainer.Instance.LightStateMachines[i].DetermineState(_currentTime, lightingValue);
                if (goalLightState == GreenhouseState.LIGHTING || goalLightState == GreenhouseState.SHADING || goalLightState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _lightState = new KeyValuePair<ITimeBasedStateMachine, GreenhouseState>(StateMachineContainer.Instance.LightStateMachines[i], goalLightState);
                    ArduinoControlSender.Instance.SendCommand(_lightState);
                }
            }

            // Get states for watering state machines, send commands
            for(int i = 0; i < StateMachineContainer.Instance.WateringStateMachines.Count; i ++)
            {
                // Get the packet from the zone we're currently operating on
                MoisturePacket packet = moistData.Where(p => p.ID == StateMachineContainer.Instance.WateringStateMachines[i].Zone).Single();
                double moistureValue = (packet.Probe1 + packet.Probe2) / 2;

                // Get the state we need to transition into, then go send a command appropriate to that
                GreenhouseState goalWaterState = StateMachineContainer.Instance.WateringStateMachines[i].DetermineState(_currentTime, moistureValue);
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
    }
}

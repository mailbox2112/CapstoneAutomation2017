using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    class GreenhouseDataAnalyzer
    {
        private double _avgTemp;
        private double _avgHumid;
        private double _avgLight;
        private double _avgMoisture;
        private DateTime _currentTime;
        private double[] _tempLimits;
        private double _lightLimit;
        private double _moistureLimit;
        private List<GreenhouseCommands> _commandsToSend;

        public GreenhouseDataAnalyzer()
        {
            _avgTemp = new double();
            _avgLight = new double();
            _avgHumid = new double();
            _avgMoisture = new double();
            _tempLimits = new double[2];
            _lightLimit = new double();
            _moistureLimit = new double();
            _currentTime = DateTime.Now;
            _commandsToSend = new List<GreenhouseCommands>();
        }

        /// <summary>
        /// Processes the data received from the packets and decides what actions are appropriate
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        public void InterpretStateData(DataPacket[] data)
        {
            GetGreenhouseAverages(data);
            Console.WriteLine($"Time: {_currentTime}\nAverage Temperature: {_avgTemp}\nAverage Humidity: {_avgHumid}\nAverage Light Intensity: {_avgLight}\n");
            GetGreenhouseLimits(data);
            _commandsToSend = DecideAppropriateAction();
            ArduinoControlSender sendCommands = new ArduinoControlSender();
            Task.Run(() => sendCommands.SendCommands(_commandsToSend));
        }

        /// <summary>
        /// Helper method for averaging greenhouse data
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        private void GetGreenhouseAverages(DataPacket[] data)
        {
            foreach (DataPacket pack in data)
            {
                _avgTemp += pack.temperature;
                _avgHumid += pack.humidity;
                _avgLight += pack.light;
            }
            _avgTemp /= 5;
            _avgHumid /= 5;
            _avgLight /= 5;
        }

        /// <summary>
        /// Helper method to get the greenhouse limits from packets
        /// </summary>
        private void GetGreenhouseLimits(DataPacket[] packet)
        {
            // TODO: get light and humidity
            foreach (DataPacket pack in packet)
            {
                if (_tempLimits[0] != pack.tempHi)
                {
                    _tempLimits[0] = pack.tempHi;
                }
                if (_tempLimits[1] != pack.tempLo)
                {
                    _tempLimits[1] = pack.tempLo;
                }
            }
        }

        /// <summary>
        /// Make decisions about what to do with greenhouse based on data we receive
        /// </summary>
        /// <returns></returns>
        private List<GreenhouseCommands> DecideAppropriateAction()
        {
            // TODO: implement light and humidity, plus greenhouse state stuff
            List<GreenhouseCommands> commands = new List<GreenhouseCommands>();
            
            // Temp is too low
            if (_avgTemp <= _tempLimits[1])
            {
                // Do stuff for temperature being too low
                commands.Add(GreenhouseCommands.HEAT_ON);
                commands.Add(GreenhouseCommands.CLOSE_VENTS);
                commands.Add(GreenhouseCommands.RETRACT_SHADES);
            }

            // If the temp is too high
            else if (_avgTemp >= _tempLimits[0])
            {
                // Do stuff for temperature being too high
                commands.Add(GreenhouseCommands.OPEN_VENTS);
                commands.Add(GreenhouseCommands.FAN_ON);
                commands.Add(GreenhouseCommands.EXTEND_SHADES);
            }

            // If the temperature is juuuuuuuuuust right...
            else if (_avgTemp < _tempLimits[0] && _avgTemp > _tempLimits[1])
            {
                // Do stuff for temperature being right
                commands.Add(GreenhouseCommands.CLOSE_VENTS);
                commands.Add(GreenhouseCommands.RETRACT_SHADES);
                commands.Add(GreenhouseCommands.HEAT_OFF);
                commands.Add(GreenhouseCommands.FAN_OFF);
            }
            
            if (_avgLight <= _lightLimit)
            {
                // Do stuff for light level being too low
                commands.Add(GreenhouseCommands.LIGHT_ON);
            }
            else
            {
                // Light levels are okay
                commands.Add(GreenhouseCommands.LIGHT_OFF);
            }

            if (_avgMoisture < _moistureLimit)
            {
                // Do watering stuff
                commands.Add(GreenhouseCommands.WATER_ON);
            }
            else
            {
                commands.Add(GreenhouseCommands.WATER_OFF);
            }

            #region State Machine Decisions
            // If the command is to heat
            if (commands.Contains(GreenhouseCommands.HEAT_ON))
            {
                // and to light
                if (commands.Contains(GreenhouseCommands.LIGHT_ON))
                {
                    // and to water
                    if (commands.Contains(GreenhouseCommands.WATER_ON))
                    {
                        // state is HEATING LIGHTING WATERING
                        GreenhouseStateMachine.Instance.CurrentState = GreenhouseState.HEATING_LIGHTING_WATERING;
                    }
                    // otherwise
                    else
                    {
                        // state is HEATING LIGHTING
                        GreenhouseStateMachine.Instance.CurrentState = GreenhouseState.HEATING_LIGHTING;
                    }
                }
                // OR to water but NOT to light
                else if (commands.Contains(GreenhouseCommands.WATER_ON) && !commands.Contains(GreenhouseCommands.LIGHT_OFF))
                {
                    // state is HEATING WATERING
                    GreenhouseStateMachine.Instance.CurrentState = GreenhouseState.HEATING_WATERING;
                }
                // otherwise just heating
                else
                {
                    GreenhouseStateMachine.Instance.CurrentState = GreenhouseState.HEATING;
                }
            }
            #endregion

            // Print stuff out for debugging purposes
            Console.WriteLine($"State: {GreenhouseStateMachine.Instance.CurrentState.ToString()}");
            

            return commands;
        }
    }
}

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
        private DateTime _currentTime;
        private double[] _tempLimits;
        private double[] _lightLimits;
        private double _humidLimit;
        private List<GreenhouseCommands> _commandsToSend;

        public GreenhouseDataAnalyzer()
        {
            _avgTemp = new double();
            _avgLight = new double();
            _avgHumid = new double();
            _tempLimits = new double[2];
            _lightLimits = new double[2];
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
                if (_humidLimit != pack.humidLim)
                {
                    _humidLimit = pack.humidLim;
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

            #region Temperature Logic
            // Temp is too low
            if (_avgTemp <= _tempLimits[1])
            {
                // Change the greenhouse states
                if (GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.COOLING))
                {
                    GreenhouseStateMachine.Instance.CurrentStates.Remove(GreenhouseState.COOLING);
                    GreenhouseStateMachine.Instance.CurrentStates.Add(GreenhouseState.HEATING);
                    
                    // Make sure that we don't keep the ventilation state going
                    if (GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.VENTILATING))
                    {
                        GreenhouseStateMachine.Instance.CurrentStates.Remove(GreenhouseState.VENTILATING);
                    }

                    // Turn off any fans that might be on, close any open vents, retract shades, and turn on heaters
                    commands.Add(GreenhouseCommands.FAN_OFF);
                    commands.Add(GreenhouseCommands.HEAT_ON);
                    commands.Add(GreenhouseCommands.CLOSE_VENTS);
                    commands.Add(GreenhouseCommands.RETRACT_SHADES);
                }
                // Don't touch the states if one is already heating
                else if (GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.HEATING))
                {
                    if (GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.VENTILATING))
                    {
                        GreenhouseStateMachine.Instance.CurrentStates.Remove(GreenhouseState.VENTILATING);
                    }

                    // Turn off any fans that might be on, close any open vents, retract shades, and turn on heaters
                    commands.Add(GreenhouseCommands.HEAT_ON);
                    commands.Add(GreenhouseCommands.CLOSE_VENTS);
                    commands.Add(GreenhouseCommands.RETRACT_SHADES);
                }
                // Add the heating state if it doesn't contain the heating state but doesn't need any others removed
                else
                {
                    GreenhouseStateMachine.Instance.CurrentStates.Add(GreenhouseState.HEATING);

                    if (GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.VENTILATING))
                    {
                        GreenhouseStateMachine.Instance.CurrentStates.Remove(GreenhouseState.VENTILATING);
                    }

                    // Turn off any fans that might be on, close any open vents, retract shades, and turn on heaters
                    commands.Add(GreenhouseCommands.HEAT_ON);
                    commands.Add(GreenhouseCommands.CLOSE_VENTS);
                    commands.Add(GreenhouseCommands.RETRACT_SHADES);
                }
            }

            // If the temp is too high
            if (_avgTemp >= _tempLimits[0])
            {
                // Remove the heating state and add the cooling state if we need to
                if (GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.HEATING))
                {
                    GreenhouseStateMachine.Instance.CurrentStates.Remove(GreenhouseState.HEATING);
                    GreenhouseStateMachine.Instance.CurrentStates.Add(GreenhouseState.COOLING);

                    // Since ventilation is its own state, make sure that we don't add the ventilation state twice
                    if (!GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.VENTILATING))
                    {
                        GreenhouseStateMachine.Instance.CurrentStates.Add(GreenhouseState.VENTILATING);
                    }
                    
                    // Commands to send when cooling. Open the vents, and turn on the fans
                    commands.Add(GreenhouseCommands.HEAT_OFF);
                    commands.Add(GreenhouseCommands.FAN_ON);
                    commands.Add(GreenhouseCommands.OPEN_VENTS);
                }
                // Do nothing to the states if it contains cooling already
                else if (GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.COOLING))
                {
                    // Since ventilation is its own state, make sure that we don't add the ventilation state twice
                    if (!GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.VENTILATING))
                    {
                        GreenhouseStateMachine.Instance.CurrentStates.Add(GreenhouseState.VENTILATING);
                    }

                    // Open the vents, and turn on the fans
                    commands.Add(GreenhouseCommands.FAN_ON);
                    commands.Add(GreenhouseCommands.OPEN_VENTS);
                }
                // Add the cooling state if we need don't meet the previous two if statements
                else
                {
                    // Open the vents, extend the shades, turn on the fans
                    GreenhouseStateMachine.Instance.CurrentStates.Add(GreenhouseState.COOLING);

                    // Since ventilation is its own state, make sure that we don't add the ventilation state twice
                    if (!GreenhouseStateMachine.Instance.CurrentStates.Contains(GreenhouseState.VENTILATING))
                    {
                        GreenhouseStateMachine.Instance.CurrentStates.Add(GreenhouseState.VENTILATING);
                    }

                    // Open the vents, and turn on the fans
                    commands.Add(GreenhouseCommands.FAN_ON);
                    commands.Add(GreenhouseCommands.OPEN_VENTS);
                }
            }

            // If the temperature is juuuuuuuuuust right...
            if (_avgTemp < _tempLimits[0] && _avgTemp > _tempLimits[1])
            {
                // Get rid of any states involving temperature regulation
                GreenhouseStateMachine.Instance.CurrentStates.RemoveAll(x => x == GreenhouseState.COOLING || x == GreenhouseState.HEATING);

                // Make sure the heaters and fans are off
                commands.Add(GreenhouseCommands.HEAT_OFF);
                commands.Add(GreenhouseCommands.FAN_OFF);
            }
            #endregion
            
            // Print stuff out for debugging purposes
            foreach (GreenhouseState state in GreenhouseStateMachine.Instance.CurrentStates)
            {
                Console.WriteLine($"State: {state.ToString()}");
            }

            return commands;
        }
    }
}

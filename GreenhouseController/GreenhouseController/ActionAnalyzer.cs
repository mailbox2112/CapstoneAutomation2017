using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class ActionAnalyzer
    {
        private double _avgTemp;
        private double _avgHumid;
        private double _avgLight;
        private double _avgMoisture;
        private DateTime _currentTime;
        private int[] _tempLimits;
        private int _lightLimit;
        private int _moistureLimit;
        private bool? _manualHeat;
        private bool? _manualCool;
        private bool? _manualLight;
        private bool? _manualWater;

        public ActionAnalyzer()
        {
            _avgTemp = new double();
            _avgLight = new double();
            _avgHumid = new double();
            _avgMoisture = new double();
            _tempLimits = new int[2];
            _lightLimit = new int();
            _moistureLimit = new int();
            _manualCool = null;
            _manualHeat = null;
            _manualLight = null;
            _manualWater = null;
            _currentTime = DateTime.Now;
        }

        /// <summary>
        /// Processes the data received from the packets
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        public void AnalyzeData(DataPacket[] data)
        {
            // If any of the packets have a value for manual control in them, we change the manual variables
            // otherwise they stay null
            foreach (var packet in data)
            {
                if (packet.manualHeat != null)
                {
                    _manualHeat = packet.manualHeat;
                }
                if (packet.manualCool != null)
                {
                    _manualCool = packet.manualCool;
                }
                if (packet.manualLight != null)
                {
                    _manualLight = packet.manualLight;
                }
                if (packet.manualWater != null)
                {
                    _manualWater = packet.manualWater;
                }
            }

            List<GreenhouseState> statesToSend = new List<GreenhouseState>();

            #region Greenhouse Under Automated Control
            // Get the averages of greenhouse readings
            GetGreenhouseAverages(data);
            Console.WriteLine($"Time: {_currentTime}\nAverage Temperature: {_avgTemp}\nAverage Humidity: {_avgHumid}\nAverage Light Intensity: {_avgLight}\nAverage Soil Moisture: {_avgMoisture}\n");
            Console.WriteLine($"Manual Heating: {_manualHeat}\nManual Cooling: {_manualCool}\nManual Lighting: {_manualLight}\nManual Watering: {_manualWater}\n");
            // Get the limits we're comparing to
            GetGreenhouseLimits(data);

            // Get state machine states as long as we don't have a manual command change to send
            if (_manualHeat == null && _manualCool == null)
            {
                GreenhouseState goalTempState = StateMachineContainer.Instance.Temperature.DetermineState(_avgTemp, _tempLimits[0], _tempLimits[1]);
                if (goalTempState == GreenhouseState.HEATING || goalTempState == GreenhouseState.COOLING)
                {
                    statesToSend.Add(goalTempState);
                }
            }
            else
            {
                if (_manualHeat == true)
                {
                    GreenhouseState goalTempState = GreenhouseState.MAN_HEATING;
                    statesToSend.Add(goalTempState);
                }
                else if (_manualCool == true)
                {
                    GreenhouseState goalTempState = GreenhouseState.MAN_COOLING;
                    statesToSend.Add(goalTempState);
                }
                else if (_manualHeat == false)
                {
                    // send command to turn off heating!
                }
                else if (_manualCool == false)
                {
                    // send command to turn off cooling!
                }
            }
            if (_manualLight == null)
            {
                GreenhouseState goalLightState = StateMachineContainer.Instance.Lighting.DetermineState(_avgLight, _lightLimit);
                if (goalLightState == GreenhouseState.LIGHTING)
                {
                    statesToSend.Add(goalLightState);
                }
            }
            else
            {
                if (_manualLight == true)
                {
                    GreenhouseState goalLightState = GreenhouseState.MAN_LIGHTING;
                    statesToSend.Add(goalLightState);
                }
                else
                {
                    // send command to turn off lighting!
                }
            }
            if (_manualWater == null)
            {
                GreenhouseState goalWaterState = StateMachineContainer.Instance.Watering.DetermineState(_avgMoisture, _moistureLimit);
                if (goalWaterState == GreenhouseState.WATERING)
                {
                    statesToSend.Add(goalWaterState);
                }
            }
            else
            {
                if (_manualWater == true)
                {
                    GreenhouseState goalWaterState = GreenhouseState.MAN_WATER;
                    statesToSend.Add(goalWaterState);
                }
                else
                {
                    // send command to turn off watering!
                }
            }
            
            // TODO: how to send only the stuff that is automated and not anything that's been manually controlled?
            // Solution: see above. We send the commands regardless, and have a separate manual state for each state machine
            //              rather than the same heating/cooling state for everything. Makes the state machines a bit more complex,
            //              but the solution is probably cleaner than if we tried some other way. This way just have a nice little "if"
            //              statement that checks to see if we're in the manual state and doens't send a command unless we've received a
            //              non-null value for the manual command that's DIFFERENT than the one that's currently set in the state machine
            // Send commands
            using (ArduinoControlSender sender = new ArduinoControlSender())
            {
                // Send commands
                sender.SendCommand(statesToSend);
            }
            #endregion

            if (StateMachineContainer.Instance.Watering.CurrentState == GreenhouseState.EMERGENCY)
            {
                // TODO: Send an emergency message to the Data Team!
            }
            if (StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.EMERGENCY)
            {
                // TODO: Send an emergency message to the Data Team!
            }

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
                _avgMoisture += pack.moisture;
            }
            _avgTemp /= 5;
            _avgHumid /= 5;
            _avgLight /= 5;
            _avgMoisture /= 5;
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
                if (_lightLimit != pack.lightLim)
                {
                    _lightLimit = pack.lightLim;
                }
                if (_moistureLimit != pack.moistLim)
                {
                    _moistureLimit = pack.moistLim;
                }
            }
        }
    }
}

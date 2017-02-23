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
        private Dictionary<IStateMachine, GreenhouseState> _statesToSend;

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
            _statesToSend = new Dictionary<IStateMachine, GreenhouseState>();
        }

        /// <summary>
        /// Processes the data received from the packets
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        public void AnalyzeData(DataPacket[] data)
        {
            ArduinoControlSender.Instance.TryConnect();
            // If any of the packets have a value for manual control in them, we change the manual variables
            // otherwise they stay null
            foreach (var packet in data)
            {
                if (packet.ManualHeat != null)
                {
                    _manualHeat = packet.ManualHeat;
                }
                if (packet.ManualCool != null)
                {
                    _manualCool = packet.ManualCool;
                }
                if (packet.ManualLight != null)
                {
                    _manualLight = packet.ManualLight;
                }
                if (packet.ManualWater != null)
                {
                    _manualWater = packet.ManualWater;
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
                if (goalTempState == GreenhouseState.HEATING || goalTempState == GreenhouseState.COOLING || goalTempState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _statesToSend.Add(new TemperatureStateMachine(), goalTempState);
                }
            }
            else
            {
                if (_manualHeat == true)
                {
                    GreenhouseState goalTempState = GreenhouseState.HEATING;
                    _statesToSend.Add(new TemperatureStateMachine(), goalTempState);
                }
                else if (_manualCool == true)
                {
                    GreenhouseState goalTempState = GreenhouseState.COOLING;
                    _statesToSend.Add(new TemperatureStateMachine(), goalTempState);
                }
                else if (_manualHeat == false || _manualCool == false)
                {
                    ArduinoControlSender.Instance.SendManualOffCommand(StateMachineContainer.Instance.Temperature);
                }
            }
            if (_manualLight == null)
            {
                GreenhouseState goalLightState = StateMachineContainer.Instance.Lighting.DetermineState(_avgLight, _lightLimit);
                if (goalLightState == GreenhouseState.LIGHTING || goalLightState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _statesToSend.Add(new LightingStateMachine(), goalLightState);
                }
            }
            else
            {
                if (_manualLight == true)
                {
                    GreenhouseState goalLightState = GreenhouseState.LIGHTING;
                    _statesToSend.Add(new LightingStateMachine(), goalLightState);
                }
                else
                {
                    ArduinoControlSender.Instance.SendManualOffCommand(StateMachineContainer.Instance.Lighting);
                }
            }
            if (_manualWater == null)
            {
                GreenhouseState goalWaterState = StateMachineContainer.Instance.Watering.DetermineState(_avgMoisture, _moistureLimit);
                if (goalWaterState == GreenhouseState.WATERING || goalWaterState == GreenhouseState.WAITING_FOR_DATA)
                {
                    _statesToSend.Add(new WateringStateMachine(), goalWaterState);
                }
            }
            else
            {
                if (_manualWater == true)
                {
                    GreenhouseState goalWaterState = GreenhouseState.WATERING;
                    _statesToSend.Add(new WateringStateMachine(), goalWaterState);
                }
                else
                {
                    ArduinoControlSender.Instance.SendManualOffCommand(StateMachineContainer.Instance.Watering);
                }
            }

            // Send commands
                foreach (var state in _statesToSend)
                {
                    // Send commands
                    ArduinoControlSender.Instance.SendCommand(state);
                }
            #endregion

            if (StateMachineContainer.Instance.Watering.CurrentState == GreenhouseState.EMERGENCY)
            {
                // TODO: Send an emergency message to the Data Team!
            }
            else if (StateMachineContainer.Instance.Watering.CurrentState == GreenhouseState.ERROR)
            {
                // TODO: Set a flag somewhere!
            }
            if (StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.EMERGENCY)
            {
                // TODO: Send an emergency message to the Data Team!
            }
            else if (StateMachineContainer.Instance.Temperature.CurrentState == GreenhouseState.ERROR)
            {
                // TODO: Set a flag somewhere!
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
                _avgTemp += pack.Temperature;
                _avgHumid += pack.Humidity;
                _avgLight += pack.Light;
                _avgMoisture += pack.Moisture;
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
                if (_tempLimits[0] != pack.TempHi)
                {
                    _tempLimits[0] = pack.TempHi;
                }
                if (_tempLimits[1] != pack.TempLo)
                {
                    _tempLimits[1] = pack.TempLo;
                }
                if (_lightLimit != pack.LightLim)
                {
                    _lightLimit = pack.LightLim;
                }
                if (_moistureLimit != pack.MoistLim)
                {
                    _moistureLimit = pack.MoistLim;
                }
            }
        }
    }
}

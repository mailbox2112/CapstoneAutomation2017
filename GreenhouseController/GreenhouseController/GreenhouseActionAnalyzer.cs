using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class GreenhouseActionAnalyzer
    {
        private double _avgTemp;
        private double _avgHumid;
        private double _avgLight;
        private double _avgMoisture;
        private DateTime _currentTime;
        private int[] _tempLimits;
        private int _lightLimit;
        private int _moistureLimit;

        public GreenhouseActionAnalyzer()
        {
            _avgTemp = new double();
            _avgLight = new double();
            _avgHumid = new double();
            _avgMoisture = new double();
            _tempLimits = new int[2];
            _lightLimit = new int();
            _moistureLimit = new int();
            _currentTime = DateTime.Now;
        }

        /// <summary>
        /// Processes the data received from the packets
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        public void AnalyzeData(DataPacket[] data)
        {

            // Get the averages of greenhouse readings
            GetGreenhouseAverages(data);
            Console.WriteLine($"Time: {_currentTime}\nAverage Temperature: {_avgTemp}\nAverage Humidity: {_avgHumid}\nAverage Light Intensity: {_avgLight}\nAverage Soil Moisture: {_avgMoisture}\n");

            // Get the limits we're comparing to
            GetGreenhouseLimits(data);

            // Get Temperature state machine state
            TemperatureStateMachine.Instance.DetermineGreenhouseState(_avgTemp, _tempLimits[0], _tempLimits[1]);
            LightingStateMachine.Instance.DetermineGreenhouseState(_avgLight, _lightLimit);
            WateringStateMachine.Instance.DetermineGreenhouseState(_avgMoisture, _moistureLimit);

            using (ArduinoControlSender sender = new ArduinoControlSender())
            {
                if (TemperatureStateMachine.Instance.EndState == GreenhouseState.COOLING || TemperatureStateMachine.Instance.EndState == GreenhouseState.HEATING)
                {
                    sender.SendCommand(TemperatureStateMachine.Instance);
                }
                if (LightingStateMachine.Instance.EndState == GreenhouseState.LIGHTING)
                {
                    sender.SendCommand(LightingStateMachine.Instance);
                }
                if (WateringStateMachine.Instance.EndState == GreenhouseState.WATERING)
                {
                    sender.SendCommand(WateringStateMachine.Instance);
                }
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

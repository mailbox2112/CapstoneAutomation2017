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
        private double[] _humidLimits;
        private List<GreenhouseCommands> _commandsToSend;

        public GreenhouseDataAnalyzer()
        {
            _avgTemp = new double();
            _avgLight = new double();
            _avgHumid = new double();
            _tempLimits = new double[2];
            _lightLimits = new double[2];
            _humidLimits = new double[2];
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
            // TODO: implement light and humidity
            List<GreenhouseCommands> commands = new List<GreenhouseCommands>();
            if(_avgTemp <= _tempLimits[1])
            {
                commands.Add(GreenhouseCommands.FAN_OFF);
                commands.Add(GreenhouseCommands.HEAT_ON);
            }
            if(_avgTemp >= _tempLimits[0])
            {
                commands.Add(GreenhouseCommands.HEAT_OFF);
                commands.Add(GreenhouseCommands.FAN_ON);
            }
            return commands;
        }
    }
}

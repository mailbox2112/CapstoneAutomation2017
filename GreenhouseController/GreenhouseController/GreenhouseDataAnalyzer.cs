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
        public void InterpretStateData(Packet[] data)
        {
            GetGreenhouseAverages(data);
            Console.WriteLine($"Time: {_currentTime}\nAverage Temperature: {_avgTemp}\nAverage Humidity: {_avgHumid}\nAverage Light Intensity: {_avgLight}\n");
            GetGreenhouseLimits();
            _commandsToSend = DecideAppropriateAction();
            ArduinoControlSender sendCommands = new ArduinoControlSender();
            Task.Run(() => sendCommands.SendCommands(_commandsToSend));
        }

        /// <summary>
        /// Helper method for averaging greenhouse data
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        private void GetGreenhouseAverages(Packet[] data)
        {
            foreach (Packet pack in data)
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
        /// Helper method to get the greenhouse limits based on time of year and time of day
        /// </summary>
        private void GetGreenhouseLimits()
        {
            // TODO: implement light and humidity
            // Hour gets returned in 24 hour time
            // Check if hour is between 7:00 and 16:00 and if so use daytime limits
            if (_currentTime.Hour > 7 && _currentTime.Hour < 16)
            {
                switch (_currentTime.Month)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 11:
                    case 12:
                        _tempLimits[0] = 65;
                        _tempLimits[1] = 70;
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        _tempLimits[0] = 75;
                        _tempLimits[1] = 85;
                        break;
                }
            }
            // Check if hour is between 16:00 and 7:00 and if so use nighttime limits
            if ((_currentTime.Hour >= 16 && _currentTime.Hour < 24) || (_currentTime.Hour >= 0 && _currentTime.Hour <= 7))
            {
                switch (_currentTime.Month)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 11:
                    case 12:
                        _tempLimits[0] = 55;
                        _tempLimits[1] = 70;
                        break;
                    case 5:
                    case 6:
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        _tempLimits[0] = 60;
                        _tempLimits[1] = 75;
                        break;
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
            if(_avgTemp <= _tempLimits[0])
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

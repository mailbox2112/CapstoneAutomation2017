using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseController
{
    public class GreenhouseDataAnalyzer
    {
        private double _avgTemp;
        private double _avgHumid;
        private double _avgLight;
        private double _avgMoisture;
        private DateTime _currentTime;
        private double[] _tempLimits;
        private double _lightLimit;
        private double _moistureLimit;
        private int _greenhouseStateValue;

        public GreenhouseDataAnalyzer()
        {
            _avgTemp = new double();
            _avgLight = new double();
            _avgHumid = new double();
            _avgMoisture = new double();
            _tempLimits = new double[2];
            _lightLimit = new double();
            _moistureLimit = new double();
            _greenhouseStateValue = 0;
            _currentTime = DateTime.Now;
        }

        /// <summary>
        /// Processes the data received from the packets and decides what actions are appropriate
        /// </summary>
        /// <param name="data">Array of Packet objects parsed from JSON sent via data server</param>
        public void InterpretStateData(DataPacket[] data)
        {
            // Always reset the temporary greenhouse state value before we start changing it
            _greenhouseStateValue = 0;

            // Get the averages of greenhouse readings
            GetGreenhouseAverages(data);
            Console.WriteLine($"Time: {_currentTime}\nAverage Temperature: {_avgTemp}\nAverage Humidity: {_avgHumid}\nAverage Light Intensity: {_avgLight}\n");

            // Get the limits we're comparing to
            GetGreenhouseLimits(data);

            // Get the state data
            TransitionToAppropriateState();

            // Send the state data to the Arduino
            ArduinoControlSender sendCommands = new ArduinoControlSender();
            Task.Run(() => sendCommands.SendCommands(GreenhouseStateMachine.Instance.CurrentState));
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

        /// <summary>
        /// Make decisions about what to do with greenhouse based on data we receive
        /// </summary>
        /// <returns></returns>
        private void TransitionToAppropriateState()
        {
            
            /* How state decision is made: 
             * For each metric we measure, add a value from our state integer.
             * Once we make it through all the metric checking, convert the state integer
             * into a greenhouse state. Send that state over serial to the Arduino.
             *          WAITING = 0
             *          HEATING = 10
             *          COOLING = 20
             *          LIGHTING = 1
             *          WATERING = 2
             * Because of the way this works, we shouldn't need to check if the temperature is between the 
             * temp limits, as we'd just add 0 to our state value if it was.
            */

            // Temp is too low
            if (_avgTemp <= _tempLimits[1])
            {
                _greenhouseStateValue += 10;
            }

            // If the temp is too high
            else if (_avgTemp >= _tempLimits[0])
            {
                _greenhouseStateValue += 20;
            }
            
            // If light is too low
            if (_avgLight <= _lightLimit)
            {
                _greenhouseStateValue += 1;
            }

            // If moisture is too low
            if (_avgMoisture <= _moistureLimit)
            {
                _greenhouseStateValue += 2;
            }

            GreenhouseStateMachine.Instance.CalculateNewState(_greenhouseStateValue);

            // Print stuff out for debugging purposes
            Console.WriteLine($"State: {GreenhouseStateMachine.Instance.CurrentState.ToString()}");
            
        }
    }
}

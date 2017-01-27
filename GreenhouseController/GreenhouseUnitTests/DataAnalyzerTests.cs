using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class DataAnalyzerTests
    {
        [TestMethod]
        public void TestInterpretStateData()
        {
            GreenhouseDataAnalyzer analyzer = new GreenhouseDataAnalyzer();

            // Five packets in the array
            DataPacket[] testReturnHeating = new DataPacket[]
            {
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnHeating);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.HEATING, GreenhouseStateMachine.Instance.CurrentState.ToString());

            DataPacket[] testReturnHeatingLighting = new DataPacket[]
            {
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 0, moisture = 100, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnHeatingLighting);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.HEATING_LIGHTING);

            DataPacket[] testReturnHeatingWatering = new DataPacket[]
            {
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0, moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0, moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0, moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0, moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 100, lightLim = 0, moistLim = 100, moisture = 0, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnHeatingWatering);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.HEATING_WATERING);

            DataPacket[] testReturnHeatingLightingWatering = new DataPacket[]
            {

            };

            DataPacket[] testReturnCooling = new DataPacket[]
            {

            };

            DataPacket[] testReturnCoolingLighting = new DataPacket[]
            {

            };

            DataPacket[] testReturnCoolingWatering = new DataPacket[]
            {

            };

            DataPacket[] testReturnCoolingLightingWatering = new DataPacket[]
            {

            };

            DataPacket[] testReturnLighting = new DataPacket[]
            {

            };

            DataPacket[] testReturnLightingWatering = new DataPacket[]
            {

            };

            DataPacket[] testReturnWatering = new DataPacket[]
            {

            };

            DataPacket[] testReturnWaiting = new DataPacket[]
            {

            };

        }
    }
}

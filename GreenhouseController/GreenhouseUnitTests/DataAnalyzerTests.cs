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
            // Test to make sure it returns the heating state
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

            // Test to make sure it returns the heating and lighting state
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

            // Test to make sure it returns the heating and watering state
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

            // Test to make sure it returns the heating lighting watering state
            DataPacket[] testReturnHeatingLightingWatering = new DataPacket[]
            {
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 0, tempHi = 150, tempLo = 100, light = 0, lightLim = 100, moistLim = 100, moisture = 0, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnHeatingLightingWatering);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.HEATING_LIGHTING_WATERING);

            // Test to make sure it returns the cooling state
            DataPacket[] testReturnCooling = new DataPacket[]
            {
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 0, moisture = 100, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnCooling);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.COOLING);

            // Test to make sure it returns the cooling and lighting state
            DataPacket[] testReturnCoolingLighting = new DataPacket[]
            {
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnCoolingLighting);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.COOLING_LIGHTING);

            // Test to make sure it returns the cooling and watering state
            DataPacket[] testReturnCoolingWatering = new DataPacket[]
            {
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnCoolingWatering);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.COOLING_WATERING);

            // Test to make sure it returns the cooling lighting watering state
            DataPacket[] testReturnCoolingLightingWatering = new DataPacket[]
            {
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 10, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 10, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 10, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 10, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 100, tempHi = 10, tempLo = 0, light = 0, lightLim = 100 , moistLim = 10, moisture = 0, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnCoolingLightingWatering);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.COOLING_LIGHTING_WATERING);

            // Test to make sure it returns the lighting state
            DataPacket[] testReturnLighting = new DataPacket[]
            {
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 0, moisture = 100, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnLighting);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.LIGHTING);

            // Test to make sure it returns the lighting watering state
            DataPacket[] testReturnLightingWatering = new DataPacket[]
            {
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 0, lightLim = 100 , moistLim = 100, moisture = 0, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnLightingWatering);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.LIGHTING_WATERING);

            // Test to make sure it returns the watering state
            DataPacket[] testReturnWatering = new DataPacket[]
            {
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 100, moisture = 0, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnWatering);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.WATERING);

            // Test to make sure it returns the waiting state
            DataPacket[] testReturnWaiting = new DataPacket[]
            {
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 50, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 50, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 50, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 50, moisture = 100, humidity = 100},
                new DataPacket() { temperature = 50, tempHi = 100, tempLo = 0, light = 100, lightLim = 0 , moistLim = 50, moisture = 100, humidity = 100}
            };

            analyzer.InterpretStateData(testReturnWaiting);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.WAITING, GreenhouseStateMachine.Instance.CurrentState.ToString());
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class StateMachineControllerTests
    {
        [TestMethod]
        public void TestStateMachineControllerInitialization()
        {
            // Test that whatever it returns isn't null
            Assert.IsNotNull(StateMachineContainer.Instance.GetLightingMachine());
            Assert.IsNotNull(StateMachineContainer.Instance.GetWateringMachine());
            Assert.IsNotNull(StateMachineContainer.Instance.GetTemperatureMachine());

            // Test that it returns the right type of object. Maybe this should be its own seperate test though?
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetLightingMachine(), typeof(LightingStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetWateringMachine(), typeof(WateringStateMachine));
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetTemperatureMachine(), typeof(TemperatureStateMachine));
        }

        [TestMethod]
        public void TestGetCurrentWaterState()
        {
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetWateringState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestGetEndWaterState()
        {
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetWateringEndState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestDetermineWaterState()
        {
            // Should be watering
            StateMachineContainer.Instance.DetermineWateringState(10, 100);
            Assert.IsTrue(StateMachineContainer.Instance.GetWateringEndState() == GreenhouseState.WATERING);

            // Should be waiting
            StateMachineContainer.Instance.DetermineWateringState(100, 0);
            Assert.IsTrue(StateMachineContainer.Instance.GetWateringEndState() == GreenhouseState.WAITING_FOR_DATA);

            // SHould be emergency
            StateMachineContainer.Instance.DetermineWateringState(0, 100);
            Assert.IsTrue(StateMachineContainer.Instance.GetWateringState() == GreenhouseState.EMERGENCY);
        }

        [TestMethod]
        public void TestGetCurrentLightState()
        {
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetLightingState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestGetEndLightState()
        {
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetLightingEndState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestDetermineLightState()
        {
            // Should be lighting
            StateMachineContainer.Instance.DetermineLightingState(0, 10000);
            Assert.IsTrue(StateMachineContainer.Instance.GetLightingEndState() == GreenhouseState.LIGHTING);

            // Should be waiting
            StateMachineContainer.Instance.DetermineLightingState(10000, 0);
            Assert.IsTrue(StateMachineContainer.Instance.GetLightingEndState() == GreenhouseState.WAITING_FOR_DATA);
        }

        [TestMethod]
        public void TestGetTemperatureCurrentState()
        {
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetTemperatureState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestGetTemperatureEndState()
        {
            Assert.IsInstanceOfType(StateMachineContainer.Instance.GetTemperatureEndState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestDetermineTemperatureState()
        {
            // Should be heating
            StateMachineContainer.Instance.DetermineTemperatureState(0, 100, 50);
            Assert.IsTrue(StateMachineContainer.Instance.GetTemperatureEndState() == GreenhouseState.HEATING);

            // Should be cooling
            StateMachineContainer.Instance.DetermineTemperatureState(101, 100, 50);
            Assert.IsTrue(StateMachineContainer.Instance.GetTemperatureEndState() == GreenhouseState.COOLING);

            // Should be waiting
            StateMachineContainer.Instance.DetermineTemperatureState(75, 100, 50);
            Assert.IsTrue(StateMachineContainer.Instance.GetTemperatureEndState() == GreenhouseState.WAITING_FOR_DATA);

            // Should be emergency
            StateMachineContainer.Instance.DetermineTemperatureState(150, 100, 0);
            Assert.IsTrue(StateMachineContainer.Instance.GetTemperatureState() == GreenhouseState.EMERGENCY);
        }
    }
}

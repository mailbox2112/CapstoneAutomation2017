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
            Assert.IsNotNull(StateMachineController.Instance.GetLightingMachine());
            Assert.IsNotNull(StateMachineController.Instance.GetWateringMachine());
            Assert.IsNotNull(StateMachineController.Instance.GetTemperatureMachine());

            // Test that it returns the right type of object. Maybe this should be its own seperate test though?
            Assert.IsInstanceOfType(StateMachineController.Instance.GetLightingMachine(), typeof(LightingStateMachine));
            Assert.IsInstanceOfType(StateMachineController.Instance.GetWateringMachine(), typeof(WateringStateMachine));
            Assert.IsInstanceOfType(StateMachineController.Instance.GetTemperatureMachine(), typeof(TemperatureStateMachine));
        }

        [TestMethod]
        public void TestGetCurrentWaterState()
        {
            Assert.IsInstanceOfType(StateMachineController.Instance.GetWateringCurrentState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestGetEndWaterState()
        {
            Assert.IsInstanceOfType(StateMachineController.Instance.GetWateringEndState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestDetermineWaterState()
        {
            // Should be watering
            StateMachineController.Instance.DetermineWateringState(0, 100);
            Assert.IsTrue(StateMachineController.Instance.GetWateringEndState() == GreenhouseState.WATERING);

            // Should be waiting
            StateMachineController.Instance.DetermineWateringState(100, 0);
            Assert.IsTrue(StateMachineController.Instance.GetWateringEndState() == GreenhouseState.WAITING_FOR_DATA);
        }

        [TestMethod]
        public void TestGetCurrentLightState()
        {
            Assert.IsInstanceOfType(StateMachineController.Instance.GetLightingCurrentState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestGetEndLightState()
        {
            Assert.IsInstanceOfType(StateMachineController.Instance.GetLightingEndState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestDetermineLightState()
        {
            // Should be lighting
            StateMachineController.Instance.DetermineLightingState(0, 10000);
            Assert.IsTrue(StateMachineController.Instance.GetLightingEndState() == GreenhouseState.LIGHTING);

            // Should be waiting
            StateMachineController.Instance.DetermineLightingState(10000, 0);
            Assert.IsTrue(StateMachineController.Instance.GetLightingEndState() == GreenhouseState.WAITING_FOR_DATA);
        }

        [TestMethod]
        public void TestGetTemperatureCurrentState()
        {
            Assert.IsInstanceOfType(StateMachineController.Instance.GetTemperatureCurrentState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestGetTemperatureEndState()
        {
            Assert.IsInstanceOfType(StateMachineController.Instance.GetTemperatureEndState(), typeof(GreenhouseState));
        }

        [TestMethod]
        public void TestDetermineTemperatureState()
        {
            // Should be heating
            StateMachineController.Instance.DetermineTemperatureState(0, 100, 50);
            Assert.IsTrue(StateMachineController.Instance.GetTemperatureEndState() == GreenhouseState.HEATING);

            // Should be cooling
            StateMachineController.Instance.DetermineTemperatureState(500, 100, 50);
            Assert.IsTrue(StateMachineController.Instance.GetTemperatureEndState() == GreenhouseState.COOLING);

            // Should be waiting
            StateMachineController.Instance.DetermineTemperatureState(75, 100, 50);
            Assert.IsTrue(StateMachineController.Instance.GetTemperatureEndState() == GreenhouseState.WAITING_FOR_DATA);
        }
    }
}

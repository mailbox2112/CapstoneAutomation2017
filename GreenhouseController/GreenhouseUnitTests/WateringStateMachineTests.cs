using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class WateringStateMachineTests
    {
        WateringStateMachine testMachine;

        [TestMethod]
        public void TestWateringStateMachineCreation()
        {
            testMachine = new WateringStateMachine();
            Assert.IsNotNull(testMachine);
            Assert.IsInstanceOfType(testMachine, typeof(WateringStateMachine));
        }

        [TestMethod]
        public void TestWateringStateDecisions()
        {
            testMachine = new WateringStateMachine();
            GreenhouseState result = testMachine.DetermineState(30, 50);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.PROCESSING_DATA);
            Assert.IsTrue(result == GreenhouseState.WATERING);

            result = testMachine.DetermineState(50, 30);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            result = testMachine.DetermineState(30, 50);
            using (ArduinoControlSenderSimulator sim = new ArduinoControlSenderSimulator())
            {
                sim.SendCommand(result, testMachine);
            }
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WATERING);

            result = testMachine.DetermineState(0, 100);
            Assert.IsTrue(result == GreenhouseState.EMERGENCY);
        }

        [TestMethod]
        public void TestConvertWateringStateToCommands()
        {
            testMachine = new WateringStateMachine();
            List<Commands> results = new List<Commands>();
            results = testMachine.ConvertStateToCommands(GreenhouseState.WATERING);
            Assert.IsTrue(results[0] == Commands.WATER_ON);

            results = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(results[0] == Commands.WATER_OFF);
        }
    }
}

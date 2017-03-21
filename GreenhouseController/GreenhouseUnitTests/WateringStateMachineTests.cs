using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

namespace GreenhouseUnitTests
{
    // TODO: Write new unit tests!
    [TestClass]
    public class WateringStateMachineTests
    {
        WateringStateMachine testMachine;

        [TestMethod]
        public void TestWateringStateMachineCreation()
        {
            testMachine = new WateringStateMachine(1);
            Assert.IsNotNull(testMachine);
            Assert.IsInstanceOfType(testMachine, typeof(WateringStateMachine));
        }

        [TestMethod]
        public void TestWateringStateDecisions()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestConvertWateringStateToCommands()
        {
            throw new NotImplementedException();
        }
    }
}

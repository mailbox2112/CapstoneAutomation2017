using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;
using System.Collections.Generic;

namespace GreenhouseUnitTests
{
    // TODO: Write new unit tests!
    [TestClass]
    public class LightingStateMachineTests
    {
        LightingStateMachine testMachine;
        [TestMethod]
        public void TestLightingStateMachineCreation()
        {
            testMachine = new LightingStateMachine(1);
            Assert.IsNotNull(testMachine);
            Assert.IsInstanceOfType(testMachine, typeof(LightingStateMachine));
        }

        [TestMethod]
        public void TestLightingStateDecisions()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestConvertLightingStateToCommands()
        {
            throw new NotImplementedException();
        }
    }
}

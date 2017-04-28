using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController.StateMachines;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    /// <summary>
    /// Unit Tests for Shading state machine class
    /// </summary>
    [TestClass]
    public class ShadingStateMachineTests
    {
        ShadingStateMachine testMachine;
        public ShadingStateMachineTests()
        {
            testMachine = new ShadingStateMachine();
        }

        [TestMethod]
        public void TestShadingStateMachineCreation()
        {
            Assert.IsNotNull(testMachine);
            Assert.IsInstanceOfType(testMachine, typeof(ShadingStateMachine));
        }
        
        [TestMethod]
        public void TestShadingStateDecisions()
        {
            GreenhouseState result;
            testMachine.HighLimit = 85;
            double temperature = 65;

            #region Automated Mode Tests
            // Test case: automated mode
            // coming from wait state
            // temp below limit
            // Result: no change
            testMachine.ManualShade = null;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(temperature);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Test case: automated mode
            // coming from wait state
            // temp above limit
            // Result: shading
            testMachine.ManualShade = null;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(95);
            Assert.IsTrue(result == GreenhouseState.SHADING);

            // Test case: automated mode
            // coming from shade state
            // temp below limit w/ hysteresis
            // Result: waiting
            testMachine.ManualShade = null;
            testMachine.CurrentState = GreenhouseState.SHADING;

            result = testMachine.DetermineState(temperature);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);

            // Test case: automated mode
            // coming from shade state
            // temp above limit hysteresis, below high limit
            // Result: shading/no change
            testMachine.ManualShade = null;
            testMachine.CurrentState = GreenhouseState.SHADING;

            result = testMachine.DetermineState(78);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.SHADING);

            // Test case: automated mode
            // coming from shade state
            // temp above high limit
            // Result: shading/no change
            testMachine.ManualShade = null;
            testMachine.CurrentState = GreenhouseState.SHADING;

            result = testMachine.DetermineState(90);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.SHADING);
            #endregion

            #region Manual Mode Tests
            // Test case: coming from wait state
            // manual shade on
            // Result: shading
            testMachine.ManualShade = true;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(temperature);
            Assert.IsTrue(result == GreenhouseState.SHADING);

            // Test case: coming from shade state
            // manual shade on
            // Result: shading/no change
            testMachine.ManualShade = true;
            testMachine.CurrentState = GreenhouseState.SHADING;

            result = testMachine.DetermineState(temperature);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.SHADING);

            // Test case: coming from wait state
            // manual shade off
            // Result: waiting/no change
            testMachine.ManualShade = false;
            testMachine.CurrentState = GreenhouseState.WAITING_FOR_DATA;

            result = testMachine.DetermineState(temperature);
            Assert.IsTrue(result == GreenhouseState.NO_CHANGE);
            Assert.IsTrue(testMachine.CurrentState == GreenhouseState.WAITING_FOR_DATA);

            // Tes case: coming from shade state
            // manual shade off
            // Result: waiting
            testMachine.ManualShade = false; ;
            testMachine.CurrentState = GreenhouseState.SHADING;

            result = testMachine.DetermineState(temperature);
            Assert.IsTrue(result == GreenhouseState.WAITING_FOR_DATA);
            #endregion
        }

        [TestMethod]
        public void TestConvertShadingStateToCommands()
        {
            List<Commands> result = new List<Commands>();

            // Test case: shading
            result = testMachine.ConvertStateToCommands(GreenhouseState.SHADING);
            Assert.IsTrue(result.Contains(Commands.SHADE_EXTEND));

            // Test case: waiting
            result = testMachine.ConvertStateToCommands(GreenhouseState.WAITING_FOR_DATA);
            Assert.IsTrue(result.Contains(Commands.SHADE_RETRACT));
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GreenhouseController;

namespace GreenhouseUnitTests
{
    [TestClass]
    public class StateMachineTests
    {
        [TestMethod]
        public void InitializeTest()
        {
            GreenhouseStateMachine.Instance.Initialize();
            Assert.IsNotNull(GreenhouseStateMachine.Instance);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.WAITING);
        }

        [TestMethod]
        public void TestStateDecisionmaking()
        {
            int[] possibleStates = new int[] { 0, 1, 2, 3, 10, 11, 12, 13, 20, 21, 22, 23 };
            
            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[0]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.WAITING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[1]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.LIGHTING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[2]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.WATERING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[3]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.LIGHTING_WATERING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[4]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.HEATING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[5]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.HEATING_LIGHTING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[6]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.HEATING_WATERING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[7]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.HEATING_LIGHTING_WATERING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[8]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.COOLING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[9]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.COOLING_LIGHTING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[10]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.COOLING_WATERING);

            GreenhouseStateMachine.Instance.CalculateNewState(possibleStates[11]);
            Assert.IsTrue(GreenhouseStateMachine.Instance.CurrentState == GreenhouseState.COOLING_LIGHTING_WATERING);
        }
    }
}

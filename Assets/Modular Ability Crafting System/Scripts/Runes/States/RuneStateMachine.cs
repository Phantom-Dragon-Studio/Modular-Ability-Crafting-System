using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.States
{
    public class RuneStateMachine : MonoBehaviour
    {
        // [SerializeField] private RuneStateMachineConfig config;
        private RuneStateFactory _states;
        public RuneBaseState CurrentState { get; set; }
        // public RuneStateMachineConfig Config => config;

        private void Awake() {
            _states = new RuneStateFactory(this);
            CurrentState = _states.Locked();
            CurrentState.EnterState();
            Debug.Log("State Machine Awake: " + CurrentState, this);
        }

        private void Update() {
            CurrentState.UpdateStates();
        }
    }
}
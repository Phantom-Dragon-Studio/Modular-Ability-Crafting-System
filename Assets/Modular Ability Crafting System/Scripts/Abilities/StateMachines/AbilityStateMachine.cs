using System;
using ModularAbilityCraftingSystem.Abilities.States;
using Unity;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities.StateMachines
{
    /// <summary>
    /// Track Data used by different states here.
    /// Think of it as the single source of truth for all states using this context.
    /// </summary>
    public class AbilityStateMachine : MonoBehaviour
    {
        [SerializeField] private AbilityStateMachineConfig config;
        private AbilityStateFactory _states;
        public AbilityBaseState CurrentState { get; set; }

        private void Awake()
        {
            _states = new AbilityStateFactory(this);
            CurrentState = _states.Locked();
            CurrentState.EnterState();
            Debug.Log("State Machine Awake: " + CurrentState.ToString(), this);
        }

        private void Update() {
            CurrentState.UpdateStates();
        }
    }
}
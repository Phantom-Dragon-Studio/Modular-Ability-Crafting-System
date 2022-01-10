using ModularAbilityCraftingSystem.Abilities.StateMachines;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities.States
{
    public abstract class AbilityBaseState
    {
        private bool _isRootState = false;
        private readonly AbilityStateMachine _context;
        private readonly AbilityStateFactory _factory;
        private AbilityBaseState _currentSubState;
        private AbilityBaseState _currentSuperState;

        protected AbilityBaseState(AbilityStateMachine context, AbilityStateFactory factory) {
            _context = context;
            _factory = factory;
        }

        protected bool IsRootState {
            get => _isRootState;
            set => _isRootState = value;
        }

        protected AbilityStateMachine Context => _context;
        protected AbilityStateFactory Factory => _factory;
        protected AbilityBaseState CurrentSubState {
            get => _currentSubState;
            set => _currentSubState = value;
        }

        protected AbilityBaseState CurrentSuperState {
            get => _currentSuperState;
            set => _currentSuperState = value;
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void ExitState();
        public abstract void InitializeSubState();
        public abstract void CheckSwitchStates();

        public void UpdateStates() {
            UpdateState();
            CurrentSubState?.UpdateStates();
        }

        public void ExitStates() {
            ExitState();
            CurrentSubState?.ExitStates();
        }

        protected void SwitchState(AbilityBaseState newState) {
            ExitState();

            newState.EnterState();
            if (IsRootState)
                Context.CurrentState = newState;
            else if (CurrentSuperState != null)
                SetSubState(newState);
        }

        protected void SetSuperState(AbilityBaseState newSuperState) {
            CurrentSuperState = newSuperState;
        }

        protected void SetSubState(AbilityBaseState newSubState) {
            CurrentSubState = newSubState;
            newSubState.SetSuperState(this);
        }
    }
}
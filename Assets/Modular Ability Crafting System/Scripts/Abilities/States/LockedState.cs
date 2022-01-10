using ModularAbilityCraftingSystem.Abilities.StateMachines;

namespace ModularAbilityCraftingSystem.Abilities.States
{
    public class LockedState : AbilityBaseState
    {
        private bool _isLocked = true;

        public override void EnterState() {
            _isLocked = true;
            IsRootState = true;
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void ExitState() {
            _isLocked = false;
        }

        public override void InitializeSubState() { }

        public override void CheckSwitchStates() {
            if (!_isLocked)
                SwitchState(Factory.Unlocked());
        }

        public LockedState(AbilityStateMachine context, AbilityStateFactory factory) : base(context, factory) { }
    }
}
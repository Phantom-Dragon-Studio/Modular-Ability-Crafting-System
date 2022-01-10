using ModularAbilityCraftingSystem.Abilities.StateMachines;

namespace ModularAbilityCraftingSystem.Abilities.States
{
    public class UnlockedState : AbilityBaseState
    {
        private bool _isUnlocked = true;
        private bool _isInUse;

        public override void EnterState() {
            _isUnlocked = true;
            _isInUse = false;
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void ExitState() { }

        public override void InitializeSubState() { }

        public override void CheckSwitchStates() {
            if (!_isUnlocked)
                SwitchState(Factory.Locked());
            if (_isInUse)
                SwitchState(Factory.Idle());
        }

        public UnlockedState(AbilityStateMachine context, AbilityStateFactory factory) : base(context, factory) { }
    }
}
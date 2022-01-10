using ModularAbilityCraftingSystem.Abilities.StateMachines;

namespace ModularAbilityCraftingSystem.Abilities.States
{
    public class ActiveState : AbilityBaseState
    {
        public override void EnterState() {
            throw new System.NotImplementedException();
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void ExitState() {
            throw new System.NotImplementedException();
        }

        public override void InitializeSubState() {
            throw new System.NotImplementedException();
        }

        public override void CheckSwitchStates() {
            throw new System.NotImplementedException();
        }

        public ActiveState(AbilityStateMachine context, AbilityStateFactory factory) : base(context, factory) { }
    }
}
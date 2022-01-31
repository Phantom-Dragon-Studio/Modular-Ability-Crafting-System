using ModularAbilityCraftingSystem.Abilities.StateMachines;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities.States
{
    public class ReadyState : AbilityBaseState
    {
        public override void EnterState()
        {
            
        }

        public override void UpdateState() {
            CheckSwitchStates();
        }

        public override void ExitState()
        {
            throw new System.NotImplementedException();
        }

        public override void InitializeSubState()
        {
            throw new System.NotImplementedException();
        }

        public override void CheckSwitchStates()
        {
            throw new System.NotImplementedException();
        }

        public ReadyState(AbilityStateMachine context, AbilityStateFactory factory) : base(context, factory) { }
    }
}
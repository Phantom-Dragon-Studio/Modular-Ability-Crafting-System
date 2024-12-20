﻿using ModularAbilityCraftingSystem.Abilities.StateMachines;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities.States
{
    public class CooldownState : AbilityBaseState
    {
        public override void EnterState()
        {
            throw new System.NotImplementedException();
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

        public CooldownState(AbilityStateMachine context, AbilityStateFactory factory) : base(context, factory) { }
    }
}
using System;
using ModularAbilityCraftingSystem.Abilities.States;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities.StateMachines
{
    public class AbilityStateFactory
    {
        private readonly AbilityStateMachine _context;

        public AbilityStateFactory(AbilityStateMachine currentContext)
        {
            _context = currentContext;
        }
        
        public AbilityBaseState Locked()
        {
            return new LockedState(_context, this);
        }
        
        public AbilityBaseState Unlocked()
        {
            return new UnlockedState(_context, this);
        }
        
        public AbilityBaseState Idle()
        {
            return new IdleState(_context, this);
        } 
        
        public AbilityBaseState Cooldown()
        {
            return new CooldownState(_context, this);
        }
        
        public AbilityBaseState Active()
        {
            return new ActiveState(_context, this);
        }
    }
}
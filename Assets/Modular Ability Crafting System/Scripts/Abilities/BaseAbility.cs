using ModularAbilityCraftingSystem.Abilities.States;
using ModularAbilityCraftingSystem.Abilities.Utilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    public abstract class BaseAbility : ScriptableObject, IAbility
    {
        protected BaseAbility(ActivationType activationType)
        {
            _activationType = activationType;
        }
        
        public abstract void Activate();
        public abstract void Deactivate();
        
        private ActivationType _activationType;
    }
}

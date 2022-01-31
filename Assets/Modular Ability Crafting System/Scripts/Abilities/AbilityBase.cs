using ModularAbilityCraftingSystem.Abilities.States;
using ModularAbilityCraftingSystem.Abilities.Utilities;
using ModularAbilityCraftingSystem.Runes.Augmentations;
using ModularAbilityCraftingSystem.Runes.Behaviours;
using ModularAbilityCraftingSystem.Runes.Triggers;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    public abstract class AbilityBase : ScriptableObject, IAbility
    {
        protected AbilityBase() {
            activationType = ActivationType.Instant;
        }

        public ActivationType Type => activationType;

        public abstract void Activate();
        public abstract void Deactivate();
        
        protected ActivationType activationType;

        [SerializeField] protected BaseAugmentation[] augmentations;
        [SerializeField] protected BaseBehaviour[] behaviours;
        [SerializeField] protected BaseTrigger[] triggers;
    }
}

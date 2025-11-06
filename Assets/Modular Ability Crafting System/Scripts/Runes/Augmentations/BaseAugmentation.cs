using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Augmentations
{
    /// <summary>
    /// Base class for augmentation runes that modify spell properties.
    /// Examples: Size, Duration, Overcharge, Random
    /// Augmentations typically modify the SpellContext stats rather than adding components.
    /// </summary>
    public abstract class BaseAugmentation : BaseRune
    {
        [Header("Augmentation Settings")]
        [Tooltip("Stacks additively with other augmentations")]
        [SerializeField] protected bool isAdditive = true;

        [Tooltip("Can stack with multiple instances of this same augmentation")]
        [SerializeField] protected bool canStackWithSelf = true;

        public bool IsAdditive => isAdditive;
        public bool CanStackWithSelf => canStackWithSelf;

        /// <summary>
        /// Apply stat modifications to the spell context.
        /// This is typically called during spell building.
        /// </summary>
        public override void ApplyToSpell(SpellContext context)
        {
            // Augmentations modify the context directly
            ApplyStatModifications(context);
        }

        /// <summary>
        /// Apply the stat modifications specific to this augmentation.
        /// Override this in derived classes.
        /// </summary>
        protected abstract void ApplyStatModifications(SpellContext context);
    }
}

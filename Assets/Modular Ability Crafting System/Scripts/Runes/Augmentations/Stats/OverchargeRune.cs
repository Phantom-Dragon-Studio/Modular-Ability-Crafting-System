using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Augmentations.Stats
{
    /// <summary>
    /// Significantly increases spell power at the cost of increased mana.
    /// High-risk, high-reward augmentation.
    /// </summary>
    [CreateAssetMenu(fileName = "Overcharge Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Augmentations/Overcharge", order = 301)]
    public class OverchargeRune : BaseAugmentation
    {
        [Header("Overcharge Settings")]
        [Tooltip("Damage multiplier")]
        [SerializeField] private float damageMultiplier = 2f;

        [Tooltip("Speed multiplier")]
        [SerializeField] private float speedMultiplier = 1.3f;

        [Tooltip("Size multiplier")]
        [SerializeField] private float sizeMultiplier = 1.2f;

        [Tooltip("Mana cost increase")]
        [SerializeField] private float manaCostIncrease = 2.5f;

        private void OnEnable()
        {
            category = RuneCategory.Augmentation;
            isAdditive = false; // Multiplies stats
            canStackWithSelf = false; // Don't allow stacking (too powerful)
            manaCostMultiplier = manaCostIncrease;
        }

        protected override void ApplyStatModifications(SpellContext context)
        {
            // Boost all primary stats
            context.Damage *= damageMultiplier;
            context.Speed *= speedMultiplier;
            context.Size *= sizeMultiplier;

            // Visual indicator (make it glow/crackle)
            context.SetCustomData("IsOvercharged", true);
        }

        public override void ApplyToInstance(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance != null)
            {
                // Could add special VFX here for overcharged spells
                // e.g., crackling energy, glowing aura, etc.
            }

            base.ApplyToInstance(context, spellInstance);
        }
    }
}

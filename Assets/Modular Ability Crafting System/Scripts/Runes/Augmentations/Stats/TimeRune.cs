using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Augmentations.Stats
{
    /// <summary>
    /// Modifies the duration of spell effects.
    /// Can increase or decrease how long a spell lasts.
    /// </summary>
    [CreateAssetMenu(fileName = "Time Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Augmentations/Time", order = 302)]
    public class TimeRune : BaseAugmentation
    {
        [Header("Time Modification")]
        [Tooltip("Duration multiplier (1 = no change, 2 = double duration, 0.5 = half)")]
        [SerializeField] private float durationMultiplier = 1.5f;

        [Tooltip("Also affect cooldown?")]
        [SerializeField] private bool affectsCooldown = false;

        [Tooltip("Cooldown multiplier (if enabled)")]
        [SerializeField] private float cooldownMultiplier = 1.2f;

        private void OnEnable()
        {
            category = RuneCategory.Augmentation;
            isAdditive = false; // Multiplies duration
            canStackWithSelf = true;
            allowMultipleInstances = true;
            maxInstances = 2;
        }

        protected override void ApplyStatModifications(SpellContext context)
        {
            // Modify duration
            context.Duration *= durationMultiplier;

            // Optionally affect cooldown
            if (affectsCooldown)
            {
                context.Cooldown *= cooldownMultiplier;
            }
        }
    }
}

using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Runtime;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Movement
{
    /// <summary>
    /// Makes the spell seek and follow targets.
    /// Requires the spell to be mobile (have Move rune).
    /// </summary>
    [CreateAssetMenu(fileName = "Homing Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Homing", order = 101)]
    public class HomingRune : BaseBehaviour
    {
        [Header("Homing Settings")]
        [Tooltip("How strongly the projectile turns toward target (degrees per second)")]
        [SerializeField] private float homingStrength = 90f;

        [Tooltip("Maximum angle at which homing can activate (degrees)")]
        [SerializeField] private float maxHomingAngle = 180f;

        [Tooltip("Automatically acquire nearest target if no target specified")]
        [SerializeField] private bool autoAcquireTarget = true;

        [Tooltip("Detection radius for auto-targeting")]
        [SerializeField] private float detectionRadius = 20f;

        private void OnEnable()
        {
            category = RuneCategory.Movement;
            addedTags = new RuneTag[] { RuneTag.Homing };
            requiredTags = new RuneTag[] { RuneTag.Mobile }; // Requires movement
            executionPriority = 20; // Execute after Move
        }

        public override void ApplyToSpell(SpellContext context)
        {
            // Mark as homing
            context.IsHoming = true;
            context.AddTag(RuneTag.Homing);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            // Add homing component
            HomingBehaviour homing = spellInstance.GetComponent<HomingBehaviour>();
            if (homing == null)
            {
                homing = spellInstance.AddComponent<HomingBehaviour>();
            }

            homing.Initialize(
                context.TargetTransform,
                homingStrength,
                maxHomingAngle,
                autoAcquireTarget,
                detectionRadius
            );
        }
    }
}

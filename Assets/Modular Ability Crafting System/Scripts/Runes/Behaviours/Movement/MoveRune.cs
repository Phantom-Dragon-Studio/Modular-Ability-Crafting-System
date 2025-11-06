using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Runtime;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Movement
{
    /// <summary>
    /// Makes the spell move forward in a direction.
    /// Essential for projectiles and moving spells.
    /// </summary>
    [CreateAssetMenu(fileName = "Move Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Move", order = 100)]
    public class MoveRune : BaseBehaviour
    {
        [Header("Move Settings")]
        [Tooltip("Speed multiplier applied to base speed")]
        [SerializeField] private float speedMultiplier = 1f;

        [Tooltip("Additional speed added")]
        [SerializeField] private float speedBonus = 0f;

        private void OnEnable()
        {
            category = RuneCategory.Movement;
            addedTags = new RuneTag[] { RuneTag.Mobile, RuneTag.Projectile };
            incompatibleTags = new RuneTag[] { RuneTag.Stationary };
            executionPriority = 10; // Execute early
        }

        public override void ApplyToSpell(SpellContext context)
        {
            // Modify speed
            context.Speed = (context.Speed * speedMultiplier) + speedBonus;

            // Add projectile tag
            context.AddTag(RuneTag.Mobile);
            context.AddTag(RuneTag.Projectile);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            // Ensure rigidbody exists
            Rigidbody rb = spellInstance.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = spellInstance.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }

            // Add movement component
            ProjectileMover mover = spellInstance.GetComponent<ProjectileMover>();
            if (mover == null)
            {
                mover = spellInstance.AddComponent<ProjectileMover>();
            }

            mover.Initialize(context.Speed, context.Duration, context.Range);
        }
    }
}

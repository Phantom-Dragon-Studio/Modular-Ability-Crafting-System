using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Multiplication
{
    /// <summary>
    /// Creates multiple copies of the spell when cast.
    /// Great for shotgun-style effects or multi-projectile spells.
    /// </summary>
    [CreateAssetMenu(fileName = "Duplicate Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Duplicate", order = 200)]
    public class DuplicateRune : BaseBehaviour
    {
        [Header("Duplication Settings")]
        [Tooltip("How many additional copies to create")]
        [SerializeField] private int additionalCopies = 2;

        [Tooltip("Spread angle between projectiles (degrees)")]
        [SerializeField] private float spreadAngle = 15f;

        [Tooltip("Pattern for projectile spread")]
        [SerializeField] private SpreadPattern pattern = SpreadPattern.Arc;

        [Tooltip("Damage multiplier per projectile (to balance multiple hits)")]
        [Range(0.1f, 1f)]
        [SerializeField] private float damagePerProjectile = 0.7f;

        public enum SpreadPattern
        {
            Arc,        // Fan out in an arc
            Circle,     // Radial pattern
            Line,       // Straight line
            Random      // Random directions
        }

        private void OnEnable()
        {
            category = RuneCategory.Multiplication;
            addedTags = new RuneTag[] { RuneTag.MultiTarget };
            incompatibleTags = new RuneTag[] { RuneTag.SingleTarget };
            manaCostMultiplier = 1.5f; // More projectiles = more cost
            executionPriority = 5; // Execute very early (before movement)
        }

        public override void ApplyToSpell(SpellContext context)
        {
            // Increase projectile count
            context.ProjectileCount += additionalCopies;
            context.SpreadAngle = spreadAngle;

            // Balance damage
            context.Damage *= damagePerProjectile;

            // Add multi-target tag
            context.AddTag(RuneTag.MultiTarget);

            // Store spread pattern in custom data
            context.SetCustomData("SpreadPattern", pattern);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            // Duplication is handled by SpellExecutor at cast time
            // This rune modifies the context, execution handles instantiation
        }

        /// <summary>
        /// Get spawn positions for duplicated projectiles
        /// </summary>
        public Vector3[] GetSpawnDirections(Vector3 forward, int count)
        {
            Vector3[] directions = new Vector3[count];

            switch (pattern)
            {
                case SpreadPattern.Arc:
                    for (int i = 0; i < count; i++)
                    {
                        float angle = spreadAngle * ((i / (float)(count - 1)) - 0.5f);
                        directions[i] = Quaternion.Euler(0, angle, 0) * forward;
                    }
                    break;

                case SpreadPattern.Circle:
                    float angleStep = 360f / count;
                    for (int i = 0; i < count; i++)
                    {
                        directions[i] = Quaternion.Euler(0, angleStep * i, 0) * forward;
                    }
                    break;

                case SpreadPattern.Line:
                    for (int i = 0; i < count; i++)
                    {
                        directions[i] = forward; // All same direction
                    }
                    break;

                case SpreadPattern.Random:
                    for (int i = 0; i < count; i++)
                    {
                        float randomAngleH = Random.Range(-spreadAngle, spreadAngle);
                        float randomAngleV = Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
                        directions[i] = Quaternion.Euler(randomAngleV, randomAngleH, 0) * forward;
                    }
                    break;
            }

            return directions;
        }
    }
}

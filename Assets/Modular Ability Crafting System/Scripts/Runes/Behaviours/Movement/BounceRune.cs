using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Runtime;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Movement
{
    /// <summary>
    /// Makes the spell bounce off surfaces.
    /// Can be used for trick shots and multi-hit scenarios.
    /// </summary>
    [CreateAssetMenu(fileName = "Bounce Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Bounce", order = 102)]
    public class BounceRune : BaseBehaviour
    {
        [Header("Bounce Settings")]
        [Tooltip("Maximum number of bounces before spell expires")]
        [SerializeField] private int maxBounces = 3;

        [Tooltip("Speed retention after each bounce (1 = no loss, 0.5 = half speed)")]
        [Range(0f, 1f)]
        [SerializeField] private float bounceDamping = 0.8f;

        [Tooltip("Destroy spell when max bounces reached?")]
        [SerializeField] private bool destroyOnMaxBounces = true;

        private void OnEnable()
        {
            category = RuneCategory.Movement;
            requiredTags = new RuneTag[] { RuneTag.Mobile }; // Requires movement
            executionPriority = 25; // Execute after Move and Homing
        }

        public override void ApplyToSpell(SpellContext context)
        {
            // Set bounce properties
            context.CanBounce = true;
            context.MaxBounces = maxBounces;
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            // Add collider if not present
            Collider collider = spellInstance.GetComponent<Collider>();
            if (collider == null)
            {
                SphereCollider sphere = spellInstance.AddComponent<SphereCollider>();
                sphere.radius = 0.5f;
            }

            // Add bounce component
            BounceBehaviour bounce = spellInstance.GetComponent<BounceBehaviour>();
            if (bounce == null)
            {
                bounce = spellInstance.AddComponent<BounceBehaviour>();
            }

            bounce.Initialize(maxBounces, bounceDamping, destroyOnMaxBounces);
        }
    }
}

using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Impact
{
    /// <summary>
    /// Allows projectiles to pierce through multiple targets.
    /// Great for line-effect spells hitting multiple enemies.
    /// </summary>
    [CreateAssetMenu(fileName = "Pierce Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Pierce", order = 302)]
    public class PierceRune : BaseBehaviour
    {
        [Header("Pierce Settings")]
        [Tooltip("Maximum number of targets to pierce through")]
        [SerializeField] private int maxPierces = 3;

        [Tooltip("Damage reduction per pierce (0.9 = 90% damage after each pierce)")]
        [Range(0.1f, 1f)]
        [SerializeField] private float damageRetention = 0.9f;

        [Tooltip("Speed reduction per pierce (0.95 = 95% speed after each pierce)")]
        [Range(0.5f, 1f)]
        [SerializeField] private float speedRetention = 0.95f;

        private void OnEnable()
        {
            category = RuneCategory.Impact;
            addedTags = new RuneTag[] { RuneTag.Projectile, RuneTag.MultiTarget };
            requiredTags = new RuneTag[] { RuneTag.Mobile };
            manaCostModifier = 5f;
            executionPriority = 40;
        }

        public override void ApplyToSpell(SpellContext context)
        {
            context.CanPierce = true;
            context.MaxPierces = maxPierces;

            context.SetCustomData("PierceDamageRetention", damageRetention);
            context.SetCustomData("PierceSpeedRetention", speedRetention);

            context.AddTag(RuneTag.MultiTarget);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            var pierce = spellInstance.AddComponent<PierceBehaviour>();
            pierce.Initialize(maxPierces, damageRetention, speedRetention, context.Damage, context.CasterTransform);

            // Modify collider to be a trigger (so we can pass through)
            Collider collider = spellInstance.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }
        }
    }

    /// <summary>
    /// Runtime component that handles piercing logic
    /// </summary>
    public class PierceBehaviour : MonoBehaviour
    {
        private int maxPierces;
        private int currentPierces = 0;
        private float damageRetention;
        private float speedRetention;
        private float currentDamage;
        private Transform caster;

        public void Initialize(int maxPierceCount, float dmgRetention, float spdRetention, float baseDamage, Transform casterTransform)
        {
            maxPierces = maxPierceCount;
            damageRetention = dmgRetention;
            speedRetention = spdRetention;
            currentDamage = baseDamage;
            caster = casterTransform;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == caster) return; // Don't hit caster

            // Apply damage
            var damageable = other.GetComponent<StatusEffects.IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(currentDamage, "Pierce");
            }

            // Increment pierce count
            currentPierces++;

            // Reduce damage and speed
            currentDamage *= damageRetention;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity *= speedRetention;
            }

            // Destroy if max pierces reached
            if (currentPierces >= maxPierces)
            {
                Destroy(gameObject);
            }
        }

        // Don't collide with surfaces - only with entities
        private void OnCollisionEnter(Collision collision)
        {
            // Check if it's terrain or wall (no IDamageable)
            if (collision.gameObject.GetComponent<StatusEffects.IDamageable>() == null)
            {
                Destroy(gameObject); // Destroy on wall hit
            }
        }
    }
}

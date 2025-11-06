using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Impact
{
    /// <summary>
    /// Creates an explosion on impact, dealing AOE damage.
    /// Essential for meteor spells and area denial.
    /// </summary>
    [CreateAssetMenu(fileName = "Explosion Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Explosion", order = 300)]
    public class ExplosionRune : BaseBehaviour
    {
        [Header("Explosion Settings")]
        [Tooltip("Explosion radius")]
        [SerializeField] private float explosionRadius = 5f;

        [Tooltip("Explosion damage (multiplier of base spell damage)")]
        [SerializeField] private float damageMultiplier = 1.5f;

        [Tooltip("Explosion force")]
        [SerializeField] private float explosionForce = 500f;

        [Tooltip("Destroy spell on explosion?")]
        [SerializeField] private bool destroyOnExplosion = true;

        [Header("Visual Effects")]
        [Tooltip("Explosion VFX prefab")]
        [SerializeField] private GameObject explosionVFX;

        [Tooltip("Explosion sound")]
        [SerializeField] private AudioClip explosionSound;

        private void OnEnable()
        {
            category = RuneCategory.Impact;
            addedTags = new RuneTag[] { RuneTag.Explosive, RuneTag.AOE };
            manaCostModifier = 10f;
            executionPriority = 50; // Execute after most other runes
        }

        public override void ApplyToSpell(SpellContext context)
        {
            context.ExplodesOnImpact = true;
            context.ExplosionRadius = explosionRadius;
            context.Damage *= damageMultiplier;

            context.SetCustomData("ExplosionForce", explosionForce);
            context.SetCustomData("ExplosionVFX", explosionVFX);
            context.SetCustomData("ExplosionSound", explosionSound);
            context.SetCustomData("DestroyOnExplosion", destroyOnExplosion);

            context.AddTag(RuneTag.Explosive);
            context.AddTag(RuneTag.AOE);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            var explosion = spellInstance.AddComponent<ExplosionBehaviour>();
            explosion.Initialize(
                explosionRadius,
                context.Damage,
                explosionForce,
                explosionVFX,
                explosionSound,
                destroyOnExplosion,
                context.CasterTransform
            );
        }
    }

    /// <summary>
    /// Runtime component that handles explosion logic
    /// </summary>
    public class ExplosionBehaviour : MonoBehaviour
    {
        private float radius;
        private float damage;
        private float force;
        private GameObject vfx;
        private AudioClip sound;
        private bool destroyAfter;
        private Transform caster;
        private bool hasExploded = false;

        public void Initialize(float explosionRadius, float explosionDamage, float explosionForce, GameObject explosionVFX, AudioClip explosionSound, bool destroy, Transform casterTransform)
        {
            radius = explosionRadius;
            damage = explosionDamage;
            force = explosionForce;
            vfx = explosionVFX;
            sound = explosionSound;
            destroyAfter = destroy;
            caster = casterTransform;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!hasExploded)
            {
                Explode(collision.contacts[0].point);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!hasExploded && other.transform != caster)
            {
                Explode(transform.position);
            }
        }

        private void Explode(Vector3 explosionPoint)
        {
            hasExploded = true;

            // Spawn VFX
            if (vfx != null)
            {
                Instantiate(vfx, explosionPoint, Quaternion.identity);
            }

            // Play sound
            if (sound != null)
            {
                AudioSource.PlayClipAtPoint(sound, explosionPoint);
            }

            // Find all colliders in radius
            Collider[] colliders = Physics.OverlapSphere(explosionPoint, radius);
            foreach (var hit in colliders)
            {
                if (hit.transform == caster) continue; // Don't damage caster

                // Apply damage
                var damageable = hit.GetComponent<StatusEffects.IDamageable>();
                if (damageable != null)
                {
                    // Damage falls off with distance
                    float distance = Vector3.Distance(explosionPoint, hit.transform.position);
                    float falloff = 1f - (distance / radius);
                    float finalDamage = damage * falloff;
                    damageable.TakeDamage(finalDamage, "Explosion");
                }

                // Apply force
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(force, explosionPoint, radius, 1f, ForceMode.Impulse);
                }
            }

            // Destroy spell GameObject
            if (destroyAfter)
            {
                Destroy(gameObject);
            }
        }

        // Visual helper for editor
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}

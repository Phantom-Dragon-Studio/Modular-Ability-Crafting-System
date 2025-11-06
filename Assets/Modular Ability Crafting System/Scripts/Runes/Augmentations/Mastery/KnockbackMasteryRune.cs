using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.StatusEffects;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Augmentations.Mastery
{
    /// <summary>
    /// Adds knockback effect to spells - pushes targets away from caster.
    /// Great for crowd control and creating distance.
    /// </summary>
    [CreateAssetMenu(fileName = "Knockback Mastery", menuName = "Phantom Dragon Studio/MACS/Runes/Mastery/Knockback", order = 503)]
    public class KnockbackMasteryRune : BaseAugmentation
    {
        [Header("Knockback Settings")]
        [Tooltip("Status effect definition for knockback")]
        [SerializeField] private StatusEffectDefinition knockbackEffect;

        [Tooltip("Knockback force multiplier")]
        [SerializeField] private float forceMultiplier = 1f;

        [Tooltip("Apply upward force?")]
        [SerializeField] private bool applyUpwardForce = true;

        [Tooltip("Upward force amount")]
        [SerializeField] private float upwardForce = 2f;

        private void OnEnable()
        {
            category = RuneCategory.Mastery;
            manaCostModifier = 3f;
            allowMultipleInstances = false;
        }

        protected override void ApplyStatModifications(SpellContext context)
        {
            context.SetCustomData("KnockbackEffect", knockbackEffect);
            context.SetCustomData("KnockbackForceMult", forceMultiplier);
            context.SetCustomData("ApplyUpwardForce", applyUpwardForce);
            context.SetCustomData("UpwardForce", upwardForce);
        }

        public override void ApplyToInstance(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null || knockbackEffect == null) return;

            var applier = spellInstance.AddComponent<KnockbackApplierComponent>();
            applier.Initialize(knockbackEffect, forceMultiplier, applyUpwardForce, upwardForce, context.CasterTransform);

            base.ApplyToInstance(context, spellInstance);
        }
    }

    public class KnockbackApplierComponent : MonoBehaviour
    {
        private StatusEffectDefinition effectDef;
        private float forceMult;
        private bool upward;
        private float upwardForce;
        private Transform caster;

        public void Initialize(StatusEffectDefinition effect, float forceMult, bool applyUpward, float upwardAmt, Transform casterTransform)
        {
            effectDef = effect;
            this.forceMult = forceMult;
            upward = applyUpward;
            upwardForce = upwardAmt;
            caster = casterTransform;
        }

        private void OnCollisionEnter(Collision collision)
        {
            TryApplyEffect(collision.gameObject, collision.contacts[0].point);
        }

        private void OnTriggerEnter(Collider other)
        {
            TryApplyEffect(other.gameObject, other.ClosestPoint(transform.position));
        }

        private void TryApplyEffect(GameObject target, Vector3 hitPoint)
        {
            if (effectDef == null || target == null) return;
            if (target == caster?.gameObject) return;

            // Apply knockback force directly
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null && caster != null)
            {
                Vector3 direction = (target.transform.position - caster.position).normalized;
                float force = effectDef.KnockbackForce * forceMult;

                Vector3 knockbackVector = direction * force;
                if (upward)
                {
                    knockbackVector += Vector3.up * upwardForce;
                }

                rb.AddForce(knockbackVector, ForceMode.Impulse);
            }

            // Also apply status effect for any additional effects
            var manager = target.GetComponent<StatusEffectManager>();
            if (manager == null)
            {
                manager = target.AddComponent<StatusEffectManager>();
            }

            manager.ApplyEffect(effectDef, caster);
        }
    }
}

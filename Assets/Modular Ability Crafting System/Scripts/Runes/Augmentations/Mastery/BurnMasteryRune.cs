using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.StatusEffects;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Augmentations.Mastery
{
    /// <summary>
    /// Adds burning effect to spells - applies damage over time.
    /// Works best with fire essence.
    /// </summary>
    [CreateAssetMenu(fileName = "Burn Mastery", menuName = "Phantom Dragon Studio/MACS/Runes/Mastery/Burn", order = 500)]
    public class BurnMasteryRune : BaseAugmentation
    {
        [Header("Burn Settings")]
        [Tooltip("Status effect definition for burn")]
        [SerializeField] private StatusEffectDefinition burnEffect;

        [Tooltip("Chance to apply burn (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float applyChance = 0.8f;

        [Tooltip("Duration multiplier for burn")]
        [SerializeField] private float durationMultiplier = 1f;

        [Tooltip("Damage multiplier for burn")]
        [SerializeField] private float damageMultiplier = 1f;

        private void OnEnable()
        {
            category = RuneCategory.Mastery;
            manaCostModifier = 5f;
            allowMultipleInstances = false;
        }

        protected override void ApplyStatModifications(SpellContext context)
        {
            // Store burn configuration in context
            context.SetCustomData("BurnEffect", burnEffect);
            context.SetCustomData("BurnChance", applyChance);
            context.SetCustomData("BurnDurationMult", durationMultiplier);
            context.SetCustomData("BurnDamageMult", damageMultiplier);
        }

        public override void ApplyToInstance(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null || burnEffect == null) return;

            // Add component that applies burn on hit
            var applier = spellInstance.AddComponent<BurnApplierComponent>();
            applier.Initialize(burnEffect, applyChance, durationMultiplier, damageMultiplier, context.CasterTransform);

            base.ApplyToInstance(context, spellInstance);
        }
    }

    /// <summary>
    /// Runtime component that applies burn on collision
    /// </summary>
    public class BurnApplierComponent : MonoBehaviour
    {
        private StatusEffectDefinition effectDef;
        private float chance;
        private float durationMult;
        private float damageMult;
        private Transform caster;

        public void Initialize(StatusEffectDefinition effect, float applyChance, float durMult, float dmgMult, Transform casterTransform)
        {
            effectDef = effect;
            chance = applyChance;
            durationMult = durMult;
            damageMult = dmgMult;
            caster = casterTransform;
        }

        private void OnCollisionEnter(Collision collision)
        {
            TryApplyEffect(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            TryApplyEffect(other.gameObject);
        }

        private void TryApplyEffect(GameObject target)
        {
            if (effectDef == null || target == null) return;
            if (target == caster?.gameObject) return; // Don't burn self

            // Check chance
            if (Random.value > chance) return;

            // Get or add StatusEffectManager
            var manager = target.GetComponent<StatusEffectManager>();
            if (manager == null)
            {
                manager = target.AddComponent<StatusEffectManager>();
            }

            // Apply burn effect
            manager.ApplyEffect(effectDef, caster);
        }
    }
}

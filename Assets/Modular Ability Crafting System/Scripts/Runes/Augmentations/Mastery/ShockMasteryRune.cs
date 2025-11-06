using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.StatusEffects;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Augmentations.Mastery
{
    /// <summary>
    /// Adds shocking effect to spells - stuns targets and can chain to nearby enemies.
    /// Works best with electric/aura essence.
    /// </summary>
    [CreateAssetMenu(fileName = "Shock Mastery", menuName = "Phantom Dragon Studio/MACS/Runes/Mastery/Shock", order = 502)]
    public class ShockMasteryRune : BaseAugmentation
    {
        [Header("Shock Settings")]
        [Tooltip("Status effect definition for shock")]
        [SerializeField] private StatusEffectDefinition shockEffect;

        [Tooltip("Chance to apply shock (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float applyChance = 0.7f;

        [Tooltip("Duration multiplier for shock")]
        [SerializeField] private float durationMultiplier = 1f;

        [Header("Chain Lightning")]
        [Tooltip("Can chain to nearby targets?")]
        [SerializeField] private bool canChain = true;

        [Tooltip("Maximum chain targets")]
        [SerializeField] private int maxChains = 3;

        [Tooltip("Chain range")]
        [SerializeField] private float chainRange = 5f;

        [Tooltip("Damage reduction per chain (0.5 = 50% damage on next target)")]
        [Range(0.1f, 1f)]
        [SerializeField] private float chainDamageMultiplier = 0.7f;

        private void OnEnable()
        {
            category = RuneCategory.Mastery;
            manaCostModifier = 10f;
            allowMultipleInstances = false;
        }

        protected override void ApplyStatModifications(SpellContext context)
        {
            context.SetCustomData("ShockEffect", shockEffect);
            context.SetCustomData("ShockChance", applyChance);
            context.SetCustomData("ShockDurationMult", durationMultiplier);
            context.SetCustomData("CanChain", canChain);
            context.SetCustomData("MaxChains", maxChains);
            context.SetCustomData("ChainRange", chainRange);
            context.SetCustomData("ChainDamageMult", chainDamageMultiplier);
        }

        public override void ApplyToInstance(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null || shockEffect == null) return;

            var applier = spellInstance.AddComponent<ShockApplierComponent>();
            applier.Initialize(shockEffect, applyChance, durationMultiplier, canChain, maxChains, chainRange, chainDamageMultiplier, context.CasterTransform);

            base.ApplyToInstance(context, spellInstance);
        }
    }

    public class ShockApplierComponent : MonoBehaviour
    {
        private StatusEffectDefinition effectDef;
        private float chance;
        private float durationMult;
        private bool chain;
        private int maxChains;
        private float chainRange;
        private float chainDmgMult;
        private Transform caster;

        public void Initialize(StatusEffectDefinition effect, float applyChance, float durMult, bool canChain, int maxChainCount, float chainRng, float chainDmg, Transform casterTransform)
        {
            effectDef = effect;
            chance = applyChance;
            durationMult = durMult;
            chain = canChain;
            maxChains = maxChainCount;
            chainRange = chainRng;
            chainDmgMult = chainDmg;
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
            if (target == caster?.gameObject) return;

            if (Random.value > chance) return;

            ApplyShockRecursive(target, maxChains);
        }

        private void ApplyShockRecursive(GameObject target, int remainingChains)
        {
            if (target == null || remainingChains < 0) return;

            var manager = target.GetComponent<StatusEffectManager>();
            if (manager == null)
            {
                manager = target.AddComponent<StatusEffectManager>();
            }

            // Apply shock
            manager.ApplyEffect(effectDef, caster);

            // Chain to nearby targets
            if (chain && remainingChains > 0)
            {
                Collider[] nearbyColliders = Physics.OverlapSphere(target.transform.position, chainRange);
                foreach (var collider in nearbyColliders)
                {
                    if (collider.gameObject != target && collider.gameObject != caster?.gameObject)
                    {
                        // Apply to next target with reduced chains
                        ApplyShockRecursive(collider.gameObject, remainingChains - 1);
                        break; // Only chain to one target per iteration
                    }
                }
            }
        }
    }
}

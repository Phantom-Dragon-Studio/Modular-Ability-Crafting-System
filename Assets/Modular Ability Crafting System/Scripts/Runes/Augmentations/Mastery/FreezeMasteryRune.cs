using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.StatusEffects;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Augmentations.Mastery
{
    /// <summary>
    /// Adds freezing effect to spells - immobilizes targets.
    /// Works best with ice/aqua essence.
    /// </summary>
    [CreateAssetMenu(fileName = "Freeze Mastery", menuName = "Phantom Dragon Studio/MACS/Runes/Mastery/Freeze", order = 501)]
    public class FreezeMasteryRune : BaseAugmentation
    {
        [Header("Freeze Settings")]
        [Tooltip("Status effect definition for freeze")]
        [SerializeField] private StatusEffectDefinition freezeEffect;

        [Tooltip("Chance to apply freeze (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float applyChance = 0.6f;

        [Tooltip("Duration multiplier for freeze")]
        [SerializeField] private float durationMultiplier = 1f;

        [Tooltip("Slow targets before full freeze?")]
        [SerializeField] private bool applySlowFirst = true;

        [Tooltip("Slow effect definition (if applySlowFirst is true)")]
        [SerializeField] private StatusEffectDefinition slowEffect;

        private void OnEnable()
        {
            category = RuneCategory.Mastery;
            manaCostModifier = 8f;
            allowMultipleInstances = false;
        }

        protected override void ApplyStatModifications(SpellContext context)
        {
            context.SetCustomData("FreezeEffect", freezeEffect);
            context.SetCustomData("FreezeChance", applyChance);
            context.SetCustomData("FreezeDurationMult", durationMultiplier);
            context.SetCustomData("ApplySlowFirst", applySlowFirst);
            context.SetCustomData("SlowEffect", slowEffect);
        }

        public override void ApplyToInstance(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null || freezeEffect == null) return;

            var applier = spellInstance.AddComponent<FreezeApplierComponent>();
            applier.Initialize(freezeEffect, applyChance, durationMultiplier, applySlowFirst, slowEffect, context.CasterTransform);

            base.ApplyToInstance(context, spellInstance);
        }
    }

    public class FreezeApplierComponent : MonoBehaviour
    {
        private StatusEffectDefinition effectDef;
        private StatusEffectDefinition slowDef;
        private float chance;
        private float durationMult;
        private bool slowFirst;
        private Transform caster;

        public void Initialize(StatusEffectDefinition effect, float applyChance, float durMult, bool applySlowFirst, StatusEffectDefinition slow, Transform casterTransform)
        {
            effectDef = effect;
            chance = applyChance;
            durationMult = durMult;
            slowFirst = applySlowFirst;
            slowDef = slow;
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

            var manager = target.GetComponent<StatusEffectManager>();
            if (manager == null)
            {
                manager = target.AddComponent<StatusEffectManager>();
            }

            // Apply slow first if configured
            if (slowFirst && slowDef != null && !manager.HasEffect(StatusEffectType.Freeze))
            {
                manager.ApplyEffect(slowDef, caster);
            }
            else
            {
                // Apply freeze
                manager.ApplyEffect(effectDef, caster);
            }
        }
    }
}

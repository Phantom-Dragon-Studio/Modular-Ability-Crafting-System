using ModularAbilityCraftingSystem.Abilities.Interfaces;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Wrapper that makes a SpellTemplate castable via the ICastable interface.
    /// Handles cooldown tracking and mana cost checking.
    /// </summary>
    [System.Serializable]
    public class CastableSpell : ICastable
    {
        [SerializeField] private SpellTemplate template;
        [SerializeField] private SpellExecutor executor;
        [SerializeField] private ISpellCaster caster;

        private float cooldownEndTime = 0f;

        public SpellTemplate Template => template;

        public CastableSpell(SpellTemplate spellTemplate, SpellExecutor spellExecutor, ISpellCaster spellCaster = null)
        {
            template = spellTemplate;
            executor = spellExecutor;
            caster = spellCaster;
        }

        public string GetName()
        {
            return template != null ? template.SpellName : "Unknown Spell";
        }

        public Sprite GetIcon()
        {
            return template != null ? template.SpellIcon : null;
        }

        public string GetDescription()
        {
            return template != null ? template.SpellDescription : "";
        }

        public bool CanCast()
        {
            if (template == null || executor == null) return false;
            if (IsOnCooldown()) return false;

            // Check mana if caster is available
            if (caster != null)
            {
                SpellContext context = template.BuildContext();
                if (!caster.HasEnoughMana(context.ManaCost))
                {
                    return false;
                }
            }

            return true;
        }

        public GameObject Cast(Vector3 position, Vector3 direction, Transform casterTransform = null, Transform target = null)
        {
            if (!CanCast()) return null;

            // Consume mana
            if (caster != null)
            {
                SpellContext context = template.BuildContext();
                if (!caster.ConsumeMana(context.ManaCost))
                {
                    return null;
                }
            }

            // Cast the spell
            GameObject spellInstance = executor.CastSpell(template, position, direction, casterTransform, target);

            // Start cooldown
            if (spellInstance != null)
            {
                SpellContext context = template.BuildContext();
                cooldownEndTime = Time.time + context.Cooldown;
            }

            return spellInstance;
        }

        public float GetManaCost()
        {
            if (template == null) return 0f;
            SpellContext context = template.BuildContext();
            return context.ManaCost;
        }

        public float GetCooldown()
        {
            if (template == null) return 0f;
            SpellContext context = template.BuildContext();
            return context.Cooldown;
        }

        public float GetRemainingCooldown()
        {
            if (!IsOnCooldown()) return 0f;
            return cooldownEndTime - Time.time;
        }

        public bool IsOnCooldown()
        {
            return Time.time < cooldownEndTime;
        }

        /// <summary>
        /// Reset cooldown (useful for testing or special mechanics)
        /// </summary>
        public void ResetCooldown()
        {
            cooldownEndTime = 0f;
        }

        /// <summary>
        /// Set a new spell template
        /// </summary>
        public void SetTemplate(SpellTemplate newTemplate)
        {
            template = newTemplate;
        }

        /// <summary>
        /// Set the spell executor
        /// </summary>
        public void SetExecutor(SpellExecutor newExecutor)
        {
            executor = newExecutor;
        }

        /// <summary>
        /// Set the caster
        /// </summary>
        public void SetCaster(ISpellCaster newCaster)
        {
            caster = newCaster;
        }
    }
}

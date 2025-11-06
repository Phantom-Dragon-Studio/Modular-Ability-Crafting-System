using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModularAbilityCraftingSystem.StatusEffects
{
    /// <summary>
    /// Manages all status effects on an entity.
    /// Attach this to any GameObject that can be affected by status effects.
    /// </summary>
    public class StatusEffectManager : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Maximum number of different status effects that can be active")]
        [SerializeField] private int maxActiveEffects = 10;

        [Tooltip("Show debug information")]
        [SerializeField] private bool debugMode = false;

        // Active status effects
        private List<StatusEffectInstance> activeEffects = new List<StatusEffectInstance>();

        // Events
        public event Action<StatusEffectInstance> OnEffectApplied;
        public event Action<StatusEffectInstance> OnEffectRemoved;
        public event Action<StatusEffectInstance> OnEffectTick;

        // Public Properties
        public List<StatusEffectInstance> ActiveEffects => new List<StatusEffectInstance>(activeEffects);
        public int ActiveEffectCount => activeEffects.Count;

        private void Update()
        {
            UpdateEffects();
        }

        /// <summary>
        /// Apply a status effect to this entity
        /// </summary>
        public StatusEffectInstance ApplyEffect(StatusEffectDefinition definition, Transform caster, int initialStacks = 1)
        {
            if (definition == null)
            {
                Debug.LogWarning("Cannot apply null status effect definition");
                return null;
            }

            // Check if effect already exists
            StatusEffectInstance existing = GetEffect(definition.EffectType);
            if (existing != null)
            {
                // Try to add stacks
                if (definition.CanStack)
                {
                    existing.AddStack(initialStacks);
                    if (debugMode) Debug.Log($"Added {initialStacks} stack(s) to {definition.EffectName}");
                    return existing;
                }
                else if (definition.RefreshDuration)
                {
                    // Refresh existing effect
                    existing.AddStack(0); // This refreshes duration
                    if (debugMode) Debug.Log($"Refreshed {definition.EffectName}");
                    return existing;
                }
                else
                {
                    if (debugMode) Debug.Log($"{definition.EffectName} already active and cannot stack");
                    return existing;
                }
            }

            // Check capacity
            if (activeEffects.Count >= maxActiveEffects)
            {
                Debug.LogWarning($"Maximum active effects ({maxActiveEffects}) reached");
                return null;
            }

            // Create new effect instance
            StatusEffectInstance newEffect = new StatusEffectInstance(definition, gameObject, caster, initialStacks);
            activeEffects.Add(newEffect);
            newEffect.Apply();

            OnEffectApplied?.Invoke(newEffect);

            if (debugMode) Debug.Log($"Applied {definition.EffectName} to {gameObject.name}");

            return newEffect;
        }

        /// <summary>
        /// Remove a specific status effect
        /// </summary>
        public bool RemoveEffect(StatusEffectType effectType)
        {
            StatusEffectInstance effect = GetEffect(effectType);
            if (effect != null)
            {
                return RemoveEffect(effect);
            }
            return false;
        }

        /// <summary>
        /// Remove a status effect instance
        /// </summary>
        public bool RemoveEffect(StatusEffectInstance effect)
        {
            if (effect == null) return false;

            if (activeEffects.Remove(effect))
            {
                effect.Remove();
                OnEffectRemoved?.Invoke(effect);

                if (debugMode) Debug.Log($"Removed {effect.Definition.EffectName}");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove all status effects
        /// </summary>
        public void RemoveAllEffects()
        {
            while (activeEffects.Count > 0)
            {
                RemoveEffect(activeEffects[0]);
            }
        }

        /// <summary>
        /// Get a specific status effect by type
        /// </summary>
        public StatusEffectInstance GetEffect(StatusEffectType effectType)
        {
            return activeEffects.FirstOrDefault(e => e.Definition.EffectType == effectType);
        }

        /// <summary>
        /// Check if an effect is currently active
        /// </summary>
        public bool HasEffect(StatusEffectType effectType)
        {
            return GetEffect(effectType) != null;
        }

        /// <summary>
        /// Get all effects of a specific category
        /// </summary>
        public List<StatusEffectInstance> GetEffectsByCategory(StatusEffectCategory category)
        {
            return activeEffects.Where(e => GetEffectCategory(e.Definition.EffectType) == category).ToList();
        }

        /// <summary>
        /// Update all active effects
        /// </summary>
        private void UpdateEffects()
        {
            for (int i = activeEffects.Count - 1; i >= 0; i--)
            {
                StatusEffectInstance effect = activeEffects[i];

                // Update effect
                effect.Update(Time.deltaTime);

                // Remove if expired
                if (effect.IsExpired)
                {
                    RemoveEffect(effect);
                }
            }
        }

        /// <summary>
        /// Get the category of a status effect type
        /// </summary>
        private StatusEffectCategory GetEffectCategory(StatusEffectType type)
        {
            switch (type)
            {
                case StatusEffectType.Burn:
                case StatusEffectType.Poison:
                case StatusEffectType.Bleed:
                case StatusEffectType.Shock:
                    return StatusEffectCategory.DamageOverTime;

                case StatusEffectType.Freeze:
                case StatusEffectType.Stun:
                case StatusEffectType.Slow:
                case StatusEffectType.Root:
                    return StatusEffectCategory.Control;

                case StatusEffectType.Knockback:
                case StatusEffectType.Pull:
                case StatusEffectType.Levitate:
                    return StatusEffectCategory.Movement;

                case StatusEffectType.Haste:
                case StatusEffectType.Shield:
                case StatusEffectType.Regeneration:
                case StatusEffectType.Empowered:
                    return StatusEffectCategory.Buff;

                default:
                    return StatusEffectCategory.Debuff;
            }
        }
    }

    /// <summary>
    /// Categories for status effects
    /// </summary>
    public enum StatusEffectCategory
    {
        DamageOverTime,
        Control,
        Movement,
        Buff,
        Debuff
    }
}

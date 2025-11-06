using ModularAbilityCraftingSystem.Abilities.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Runtime context passed to runes when building and executing a spell.
    /// This allows runes to read and modify spell properties in a composable way.
    /// </summary>
    public class SpellContext
    {
        // Core Spell Definition
        public FocusDefinition Focus { get; set; }
        public EssenceDefinition Essence { get; set; }

        // Spell Instance Data
        public GameObject SpellInstance { get; set; }
        public Transform CasterTransform { get; set; }
        public Vector3 CastPosition { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Transform TargetTransform { get; set; }

        // Modifiable Stats (start with base values from Focus/Essence)
        public float Damage { get; set; }
        public float Speed { get; set; }
        public float Range { get; set; }
        public float Duration { get; set; }
        public float Cooldown { get; set; }
        public float ManaCost { get; set; }
        public float Size { get; set; } = 1f;
        public int ProjectileCount { get; set; } = 1;
        public float SpreadAngle { get; set; } = 0f;

        // Behavior Flags
        public bool IsHoming { get; set; }
        public bool CanBounce { get; set; }
        public int MaxBounces { get; set; }
        public bool CanPierce { get; set; }
        public int MaxPierces { get; set; }
        public bool ExplodesOnImpact { get; set; }
        public float ExplosionRadius { get; set; }
        public bool CanChain { get; set; }
        public int MaxChainTargets { get; set; }
        public float ChainRange { get; set; }

        // Movement and Direction
        public Vector3 Direction { get; set; }
        public Vector3 Velocity { get; set; }

        // Tags and Metadata
        public HashSet<RuneTag> Tags { get; private set; } = new HashSet<RuneTag>();

        // Linked Spells (for rune nesting/chaining)
        public List<SpellTemplate> LinkedSpells { get; private set; } = new List<SpellTemplate>();

        // Custom Data (for extensibility)
        private Dictionary<string, object> customData = new Dictionary<string, object>();

        public SpellContext(FocusDefinition focus, EssenceDefinition essence)
        {
            Focus = focus;
            Essence = essence;

            // Initialize with base stats
            if (focus != null)
            {
                Speed = focus.BaseSpeed;
                Range = focus.BaseRange;
                Duration = focus.BaseDuration;
                Cooldown = focus.BaseCooldown;
                ManaCost = focus.BaseManaCost;
            }

            if (essence != null)
            {
                Damage = essence.DamageMultiplier;
            }
        }

        /// <summary>
        /// Add a tag to this spell context
        /// </summary>
        public void AddTag(RuneTag tag)
        {
            Tags.Add(tag);
        }

        /// <summary>
        /// Remove a tag from this spell context
        /// </summary>
        public void RemoveTag(RuneTag tag)
        {
            Tags.Remove(tag);
        }

        /// <summary>
        /// Check if this spell has a specific tag
        /// </summary>
        public bool HasTag(RuneTag tag)
        {
            return Tags.Contains(tag);
        }

        /// <summary>
        /// Set custom data for extensibility
        /// </summary>
        public void SetCustomData(string key, object value)
        {
            customData[key] = value;
        }

        /// <summary>
        /// Get custom data
        /// </summary>
        public T GetCustomData<T>(string key, T defaultValue = default)
        {
            if (customData.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Add a linked spell for chaining/nesting
        /// </summary>
        public void AddLinkedSpell(SpellTemplate spell)
        {
            if (spell != null && !LinkedSpells.Contains(spell))
            {
                LinkedSpells.Add(spell);
            }
        }
    }
}

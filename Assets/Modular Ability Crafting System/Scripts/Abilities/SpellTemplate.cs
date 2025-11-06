using ModularAbilityCraftingSystem.Abilities.Utilities;
using ModularAbilityCraftingSystem.Runes;
using ModularAbilityCraftingSystem.Runes.Augmentations;
using ModularAbilityCraftingSystem.Runes.Behaviours;
using ModularAbilityCraftingSystem.Runes.Triggers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Represents a complete spell composition that can be saved, loaded, and cast.
    /// This is the core container for player-crafted or pre-built spells.
    /// </summary>
    [CreateAssetMenu(fileName = "New Spell Template", menuName = "Phantom Dragon Studio/Modular Ability Crafting System/Spell Template", order = 0)]
    public class SpellTemplate : ScriptableObject
    {
        [Header("Spell Identity")]
        [Tooltip("Name of this spell (player-defined or preset)")]
        [SerializeField] private string spellName = "Unnamed Spell";

        [Tooltip("Description of what this spell does")]
        [TextArea(3, 5)]
        [SerializeField] private string spellDescription;

        [Tooltip("Icon for this spell")]
        [SerializeField] private Sprite spellIcon;

        [Tooltip("Unique ID for this spell (auto-generated)")]
        [SerializeField] private string spellId;

        [Header("Core Components")]
        [Tooltip("The Focus determines HOW the spell manifests")]
        [SerializeField] private FocusDefinition focus;

        [Tooltip("The Essence determines the elemental nature")]
        [SerializeField] private EssenceDefinition essence;

        [Header("Runes")]
        [Tooltip("All runes that modify this spell")]
        [SerializeField] private List<BaseRune> runes = new List<BaseRune>();

        [Header("Metadata")]
        [Tooltip("Is this a pre-built spell or player-created?")]
        [SerializeField] private bool isPrebuilt = false;

        [Tooltip("Can this spell be edited by the player?")]
        [SerializeField] private bool isEditable = true;

        [Tooltip("Tags for searching and filtering")]
        [SerializeField] private string[] tags;

        [Tooltip("Date/time this spell was created (for player spells)")]
        [SerializeField] private string creationDate;

        [Tooltip("Date/time this spell was last modified")]
        [SerializeField] private string lastModifiedDate;

        // Cached references (populated at runtime)
        private List<BaseTrigger> cachedTriggers;
        private List<BaseBehaviour> cachedBehaviours;
        private List<BaseAugmentation> cachedAugmentations;
        private SpellContext cachedContext;

        // Public Properties
        public string SpellName
        {
            get => spellName;
            set => spellName = value;
        }

        public string SpellDescription
        {
            get => spellDescription;
            set => spellDescription = value;
        }

        public Sprite SpellIcon
        {
            get => spellIcon;
            set => spellIcon = value;
        }

        public string SpellId
        {
            get
            {
                if (string.IsNullOrEmpty(spellId))
                {
                    spellId = Guid.NewGuid().ToString();
                }
                return spellId;
            }
        }

        public FocusDefinition Focus
        {
            get => focus;
            set => focus = value;
        }

        public EssenceDefinition Essence
        {
            get => essence;
            set => essence = value;
        }

        public List<BaseRune> Runes => runes;

        public bool IsPrebuilt => isPrebuilt;

        public bool IsEditable => isEditable;

        public string[] Tags => tags;

        public string CreationDate => creationDate;

        public string LastModifiedDate => lastModifiedDate;

        /// <summary>
        /// Get all trigger runes in this spell
        /// </summary>
        public List<BaseTrigger> GetTriggers()
        {
            if (cachedTriggers == null)
            {
                cachedTriggers = runes.OfType<BaseTrigger>().ToList();
            }
            return cachedTriggers;
        }

        /// <summary>
        /// Get all behaviour runes in this spell
        /// </summary>
        public List<BaseBehaviour> GetBehaviours()
        {
            if (cachedBehaviours == null)
            {
                cachedBehaviours = runes.OfType<BaseBehaviour>().OrderBy(b => b.ExecutionPriority).ToList();
            }
            return cachedBehaviours;
        }

        /// <summary>
        /// Get all augmentation runes in this spell
        /// </summary>
        public List<BaseAugmentation> GetAugmentations()
        {
            if (cachedAugmentations == null)
            {
                cachedAugmentations = runes.OfType<BaseAugmentation>().ToList();
            }
            return cachedAugmentations;
        }

        /// <summary>
        /// Get runes of a specific category
        /// </summary>
        public List<BaseRune> GetRunesByCategory(RuneCategory category)
        {
            return runes.Where(r => r.Category == category).ToList();
        }

        /// <summary>
        /// Add a rune to this spell template
        /// </summary>
        public bool AddRune(BaseRune rune)
        {
            if (rune == null) return false;

            // Check if multiple instances are allowed
            if (runes.Contains(rune) && !rune.AllowMultipleInstances)
            {
                Debug.LogWarning($"Rune {rune.RuneName} does not allow multiple instances");
                return false;
            }

            // Check instance limit
            if (rune.AllowMultipleInstances)
            {
                int count = runes.Count(r => r == rune);
                if (count >= rune.MaxInstances)
                {
                    Debug.LogWarning($"Maximum instances ({rune.MaxInstances}) of rune {rune.RuneName} reached");
                    return false;
                }
            }

            runes.Add(rune);
            InvalidateCache();
            UpdateModifiedDate();
            return true;
        }

        /// <summary>
        /// Remove a rune from this spell template
        /// </summary>
        public bool RemoveRune(BaseRune rune)
        {
            if (rune == null) return false;

            bool removed = runes.Remove(rune);
            if (removed)
            {
                InvalidateCache();
                UpdateModifiedDate();
            }
            return removed;
        }

        /// <summary>
        /// Remove all runes
        /// </summary>
        public void ClearRunes()
        {
            runes.Clear();
            InvalidateCache();
            UpdateModifiedDate();
        }

        /// <summary>
        /// Check if this spell contains a specific rune
        /// </summary>
        public bool HasRune(BaseRune rune)
        {
            return runes.Contains(rune);
        }

        /// <summary>
        /// Get the count of a specific rune in this spell
        /// </summary>
        public int GetRuneCount(BaseRune rune)
        {
            return runes.Count(r => r == rune);
        }

        /// <summary>
        /// Build a spell context from this template.
        /// This is used when casting the spell.
        /// </summary>
        public SpellContext BuildContext()
        {
            // Create context with base focus and essence
            SpellContext context = new SpellContext(focus, essence);

            // Apply all runes to the context
            foreach (var rune in runes)
            {
                if (rune != null)
                {
                    rune.OnAddedToSpell(context);
                    rune.ApplyToSpell(context);
                }
            }

            cachedContext = context;
            return context;
        }

        /// <summary>
        /// Validate this spell template
        /// </summary>
        public bool Validate(out List<string> errors)
        {
            errors = new List<string>();

            // Check for required components
            if (focus == null)
            {
                errors.Add("Spell must have a Focus");
            }

            if (essence == null)
            {
                errors.Add("Spell must have an Essence");
            }

            // Check rune compatibility
            SpellContext context = new SpellContext(focus, essence);

            foreach (var rune in runes)
            {
                if (rune == null)
                {
                    errors.Add("Spell contains null rune reference");
                    continue;
                }

                // Check compatibility with current context
                if (!rune.IsCompatibleWith(context))
                {
                    errors.Add($"Rune {rune.RuneName} is not compatible with current spell configuration");
                }

                // Check for conflicts with other runes
                foreach (var otherRune in runes)
                {
                    if (otherRune != null && rune != otherRune && rune.ConflictsWith(otherRune))
                    {
                        errors.Add($"Rune {rune.RuneName} conflicts with {otherRune.RuneName}");
                    }
                }

                // Apply rune to context for next iteration
                rune.OnAddedToSpell(context);
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Clone this spell template
        /// </summary>
        public SpellTemplate Clone()
        {
            SpellTemplate clone = CreateInstance<SpellTemplate>();
            clone.spellName = spellName + " (Copy)";
            clone.spellDescription = spellDescription;
            clone.spellIcon = spellIcon;
            clone.focus = focus;
            clone.essence = essence;
            clone.runes = new List<BaseRune>(runes);
            clone.isPrebuilt = false;
            clone.isEditable = true;
            clone.tags = tags != null ? (string[])tags.Clone() : null;
            clone.creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            clone.lastModifiedDate = clone.creationDate;
            return clone;
        }

        /// <summary>
        /// Update the last modified date
        /// </summary>
        public void UpdateModifiedDate()
        {
            lastModifiedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// Invalidate cached data
        /// </summary>
        private void InvalidateCache()
        {
            cachedTriggers = null;
            cachedBehaviours = null;
            cachedAugmentations = null;
            cachedContext = null;
        }

        /// <summary>
        /// Initialize this spell template (called when created)
        /// </summary>
        private void OnEnable()
        {
            if (string.IsNullOrEmpty(spellId))
            {
                spellId = Guid.NewGuid().ToString();
            }

            if (string.IsNullOrEmpty(creationDate))
            {
                creationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                lastModifiedDate = creationDate;
            }
        }
    }
}

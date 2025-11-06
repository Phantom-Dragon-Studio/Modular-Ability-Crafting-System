using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes
{
    /// <summary>
    /// Base class for all runes in the spell crafting system.
    /// Runes modify spell behavior, visuals, and properties through composition.
    /// </summary>
    public abstract class BaseRune : ScriptableObject
    {
        [Header("Rune Identity")]
        [Tooltip("Display name shown in UI")]
        [SerializeField] protected string runeName;

        [Tooltip("Description of what this rune does")]
        [TextArea(3, 5)]
        [SerializeField] protected string description;

        [Tooltip("Icon for UI representation")]
        [SerializeField] protected Sprite icon;

        [Tooltip("Category this rune belongs to")]
        [SerializeField] protected RuneCategory category;

        [Header("Unlock Requirements")]
        [Tooltip("Is this rune unlocked by default?")]
        [SerializeField] protected bool isUnlockedByDefault = true;

        [Tooltip("Level required to unlock this rune (0 = no requirement)")]
        [SerializeField] protected int requiredLevel = 0;

        [Tooltip("Other runes that must be unlocked first")]
        [SerializeField] protected BaseRune[] prerequisiteRunes;

        [Header("Compatibility")]
        [Tooltip("Tags this rune adds to the spell")]
        [SerializeField] protected RuneTag[] addedTags;

        [Tooltip("Tags required for this rune to work")]
        [SerializeField] protected RuneTag[] requiredTags;

        [Tooltip("Tags that prevent this rune from being used")]
        [SerializeField] protected RuneTag[] incompatibleTags;

        [Tooltip("Specific runes that cannot be combined with this one")]
        [SerializeField] protected BaseRune[] incompatibleRunes;

        [Header("Costs and Limits")]
        [Tooltip("Mana cost added by this rune")]
        [SerializeField] protected float manaCostModifier = 0f;

        [Tooltip("Mana cost multiplier (1 = no change, 2 = double cost)")]
        [SerializeField] protected float manaCostMultiplier = 1f;

        [Tooltip("Can this rune be slotted multiple times in one spell?")]
        [SerializeField] protected bool allowMultipleInstances = false;

        [Tooltip("Maximum number of times this can be slotted (if multiple instances allowed)")]
        [SerializeField] protected int maxInstances = 1;

        [Header("Visual Feedback")]
        [Tooltip("Color tint applied when this rune is active")]
        [SerializeField] protected Color runeTint = Color.white;

        [Tooltip("VFX overlay added by this rune")]
        [SerializeField] protected GameObject runeVFX;

        // Public Properties
        public string RuneName => runeName;
        public string Description => description;
        public Sprite Icon => icon;
        public RuneCategory Category => category;
        public bool IsUnlockedByDefault => isUnlockedByDefault;
        public int RequiredLevel => requiredLevel;
        public BaseRune[] PrerequisiteRunes => prerequisiteRunes;
        public RuneTag[] AddedTags => addedTags;
        public RuneTag[] RequiredTags => requiredTags;
        public RuneTag[] IncompatibleTags => incompatibleTags;
        public BaseRune[] IncompatibleRunes => incompatibleRunes;
        public float ManaCostModifier => manaCostModifier;
        public float ManaCostMultiplier => manaCostMultiplier;
        public bool AllowMultipleInstances => allowMultipleInstances;
        public int MaxInstances => maxInstances;
        public Color RuneTint => runeTint;
        public GameObject RuneVFX => runeVFX;

        /// <summary>
        /// Apply this rune's effect to a spell being built.
        /// Called during spell composition.
        /// </summary>
        /// <param name="context">The spell context to modify</param>
        public abstract void ApplyToSpell(SpellContext context);

        /// <summary>
        /// Apply this rune's effect to a spell instance at runtime.
        /// Called when the spell is actually cast.
        /// </summary>
        /// <param name="context">The runtime spell context</param>
        /// <param name="spellInstance">The instantiated spell GameObject</param>
        public virtual void ApplyToInstance(SpellContext context, GameObject spellInstance)
        {
            // Default implementation: attach VFX if present
            if (runeVFX != null && spellInstance != null)
            {
                GameObject vfx = Instantiate(runeVFX, spellInstance.transform);
                vfx.transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// Check if this rune can be added to a spell with the given tags
        /// </summary>
        public virtual bool IsCompatibleWith(SpellContext context)
        {
            // Check required tags
            if (requiredTags != null)
            {
                foreach (var tag in requiredTags)
                {
                    if (!context.HasTag(tag))
                    {
                        return false;
                    }
                }
            }

            // Check incompatible tags
            if (incompatibleTags != null)
            {
                foreach (var tag in incompatibleTags)
                {
                    if (context.HasTag(tag))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Check if this rune conflicts with another rune
        /// </summary>
        public virtual bool ConflictsWith(BaseRune other)
        {
            if (other == null) return false;

            if (incompatibleRunes != null)
            {
                foreach (var incompatible in incompatibleRunes)
                {
                    if (incompatible == other)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Called when this rune is added to a spell template
        /// </summary>
        public virtual void OnAddedToSpell(SpellContext context)
        {
            // Add this rune's tags to the spell
            if (addedTags != null)
            {
                foreach (var tag in addedTags)
                {
                    context.AddTag(tag);
                }
            }

            // Modify mana cost
            context.ManaCost = (context.ManaCost + manaCostModifier) * manaCostMultiplier;
        }

        /// <summary>
        /// Called when this rune is removed from a spell template
        /// </summary>
        public virtual void OnRemovedFromSpell(SpellContext context)
        {
            // Remove this rune's tags
            if (addedTags != null)
            {
                foreach (var tag in addedTags)
                {
                    context.RemoveTag(tag);
                }
            }
        }
    }
}

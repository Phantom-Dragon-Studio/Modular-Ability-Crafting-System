using ModularAbilityCraftingSystem.Runes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Defines rules and constraints for spell crafting.
    /// This ScriptableObject allows different games to customize the spell crafting system.
    /// </summary>
    [CreateAssetMenu(fileName = "Spell Crafting Rules", menuName = "Phantom Dragon Studio/Modular Ability Crafting System/Crafting Rules", order = 10)]
    public class SpellCraftingRules : ScriptableObject
    {
        [Header("Rune Slot Limits")]
        [Tooltip("Maximum total runes allowed in a single spell (0 = unlimited)")]
        [SerializeField] private int maxTotalRunes = 8;

        [Tooltip("Maximum runes per category (0 = unlimited)")]
        [SerializeField] private int maxTriggerRunes = 2;
        [SerializeField] private int maxMovementRunes = 2;
        [SerializeField] private int maxMultiplicationRunes = 2;
        [SerializeField] private int maxAugmentationRunes = 3;
        [SerializeField] private int maxImpactRunes = 1;
        [SerializeField] private int maxMasteryRunes = 1;
        [SerializeField] private int maxTargetingRunes = 2;
        [SerializeField] private int maxConditionalRunes = 2;
        [SerializeField] private int maxUtilityRunes = 3;

        [Header("Progression")]
        [Tooltip("Starting number of rune slots available to players")]
        [SerializeField] private int startingRuneSlots = 3;

        [Tooltip("Can rune slots be upgraded?")]
        [SerializeField] private bool allowSlotUpgrades = true;

        [Tooltip("Cost curve for slot upgrades (index = slot number, value = cost)")]
        [SerializeField] private int[] slotUpgradeCosts = new int[] { 100, 200, 400, 800, 1600, 3200 };

        [Header("Core Requirements")]
        [Tooltip("Must every spell have a Focus?")]
        [SerializeField] private bool requireFocus = true;

        [Tooltip("Must every spell have an Essence?")]
        [SerializeField] private bool requireEssence = true;

        [Tooltip("Minimum number of runes required")]
        [SerializeField] private int minRequiredRunes = 0;

        [Header("Validation Options")]
        [Tooltip("Enforce rune compatibility checks?")]
        [SerializeField] private bool enforceCompatibility = true;

        [Tooltip("Enforce rune conflict checks?")]
        [SerializeField] private bool enforceConflicts = true;

        [Tooltip("Enforce prerequisite checks?")]
        [SerializeField] private bool enforcePrerequisites = true;

        [Tooltip("Enforce level requirements?")]
        [SerializeField] private bool enforceLevelRequirements = true;

        [Header("Cost Limits")]
        [Tooltip("Maximum mana cost allowed for a spell (0 = unlimited)")]
        [SerializeField] private float maxManaCost = 0f;

        [Tooltip("Should warn if spell cost is very high?")]
        [SerializeField] private bool warnHighCost = true;

        [Tooltip("Cost threshold for warnings")]
        [SerializeField] private float highCostThreshold = 100f;

        // Public Properties
        public int MaxTotalRunes => maxTotalRunes;
        public int StartingRuneSlots => startingRuneSlots;
        public bool AllowSlotUpgrades => allowSlotUpgrades;
        public int[] SlotUpgradeCosts => slotUpgradeCosts;
        public bool RequireFocus => requireFocus;
        public bool RequireEssence => requireEssence;
        public int MinRequiredRunes => minRequiredRunes;
        public bool EnforceCompatibility => enforceCompatibility;
        public bool EnforceConflicts => enforceConflicts;
        public bool EnforcePrerequisites => enforcePrerequisites;
        public bool EnforceLevelRequirements => enforceLevelRequirements;
        public float MaxManaCost => maxManaCost;
        public bool WarnHighCost => warnHighCost;
        public float HighCostThreshold => highCostThreshold;

        /// <summary>
        /// Get the maximum runes allowed for a specific category
        /// </summary>
        public int GetMaxRunesForCategory(RuneCategory category)
        {
            return category switch
            {
                RuneCategory.Trigger => maxTriggerRunes,
                RuneCategory.Movement => maxMovementRunes,
                RuneCategory.Multiplication => maxMultiplicationRunes,
                RuneCategory.Augmentation => maxAugmentationRunes,
                RuneCategory.Impact => maxImpactRunes,
                RuneCategory.Mastery => maxMasteryRunes,
                RuneCategory.Targeting => maxTargetingRunes,
                RuneCategory.Conditional => maxConditionalRunes,
                RuneCategory.Utility => maxUtilityRunes,
                _ => 0
            };
        }

        /// <summary>
        /// Validate a spell template against these rules
        /// </summary>
        public bool ValidateSpell(SpellTemplate spell, int playerLevel, List<BaseRune> unlockedRunes, out List<string> errors, out List<string> warnings)
        {
            errors = new List<string>();
            warnings = new List<string>();

            if (spell == null)
            {
                errors.Add("Spell template is null");
                return false;
            }

            // Check core requirements
            if (requireFocus && spell.Focus == null)
            {
                errors.Add("Spell must have a Focus");
            }

            if (requireEssence && spell.Essence == null)
            {
                errors.Add("Spell must have an Essence");
            }

            // Check minimum runes
            if (spell.Runes.Count < minRequiredRunes)
            {
                errors.Add($"Spell must have at least {minRequiredRunes} runes (current: {spell.Runes.Count})");
            }

            // Check total rune limit
            if (maxTotalRunes > 0 && spell.Runes.Count > maxTotalRunes)
            {
                errors.Add($"Spell exceeds maximum rune limit ({spell.Runes.Count}/{maxTotalRunes})");
            }

            // Check category limits
            var runesByCategory = spell.Runes.GroupBy(r => r.Category);
            foreach (var group in runesByCategory)
            {
                int maxForCategory = GetMaxRunesForCategory(group.Key);
                if (maxForCategory > 0 && group.Count() > maxForCategory)
                {
                    errors.Add($"Too many {group.Key} runes ({group.Count()}/{maxForCategory})");
                }
            }

            // Build context for validation
            SpellContext context = spell.BuildContext();

            // Validate each rune
            foreach (var rune in spell.Runes)
            {
                if (rune == null)
                {
                    errors.Add("Spell contains null rune reference");
                    continue;
                }

                // Check if rune is unlocked
                if (enforcePrerequisites && unlockedRunes != null && !unlockedRunes.Contains(rune))
                {
                    errors.Add($"Rune '{rune.RuneName}' is not unlocked");
                }

                // Check level requirement
                if (enforceLevelRequirements && rune.RequiredLevel > playerLevel)
                {
                    errors.Add($"Rune '{rune.RuneName}' requires level {rune.RequiredLevel} (current: {playerLevel})");
                }

                // Check compatibility
                if (enforceCompatibility && !rune.IsCompatibleWith(context))
                {
                    errors.Add($"Rune '{rune.RuneName}' is not compatible with current spell configuration");
                }

                // Check for conflicts
                if (enforceConflicts)
                {
                    foreach (var otherRune in spell.Runes)
                    {
                        if (otherRune != null && rune != otherRune && rune.ConflictsWith(otherRune))
                        {
                            errors.Add($"Rune '{rune.RuneName}' conflicts with '{otherRune.RuneName}'");
                        }
                    }
                }
            }

            // Check mana cost
            if (maxManaCost > 0 && context.ManaCost > maxManaCost)
            {
                errors.Add($"Spell mana cost ({context.ManaCost}) exceeds maximum ({maxManaCost})");
            }

            // Warnings
            if (warnHighCost && context.ManaCost > highCostThreshold)
            {
                warnings.Add($"Spell has very high mana cost ({context.ManaCost})");
            }

            return errors.Count == 0;
        }

        /// <summary>
        /// Get the cost to upgrade to a specific slot number
        /// </summary>
        public int GetSlotUpgradeCost(int targetSlot)
        {
            int slotIndex = targetSlot - startingRuneSlots;
            if (slotIndex < 0 || !allowSlotUpgrades)
            {
                return -1; // Cannot upgrade
            }

            if (slotIndex >= slotUpgradeCosts.Length)
            {
                // Extrapolate cost if beyond defined array
                int lastCost = slotUpgradeCosts[slotUpgradeCosts.Length - 1];
                return lastCost * (slotIndex - slotUpgradeCosts.Length + 2);
            }

            return slotUpgradeCosts[slotIndex];
        }
    }
}

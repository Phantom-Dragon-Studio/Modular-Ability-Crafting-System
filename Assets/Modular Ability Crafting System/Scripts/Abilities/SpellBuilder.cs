using ModularAbilityCraftingSystem.Runes;
using System.Collections.Generic;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Builder class for constructing spells at runtime.
    /// Provides a fluent API for spell composition with validation.
    /// </summary>
    public class SpellBuilder
    {
        private SpellTemplate currentSpell;
        private SpellCraftingRules rules;
        private int playerLevel;
        private List<BaseRune> unlockedRunes;

        private List<string> validationErrors = new List<string>();
        private List<string> validationWarnings = new List<string>();

        /// <summary>
        /// Create a new spell builder
        /// </summary>
        public SpellBuilder(SpellCraftingRules craftingRules = null)
        {
            currentSpell = ScriptableObject.CreateInstance<SpellTemplate>();
            rules = craftingRules;
            playerLevel = 0;
            unlockedRunes = new List<BaseRune>();
        }

        /// <summary>
        /// Create a builder from an existing spell template (for editing)
        /// </summary>
        public SpellBuilder(SpellTemplate template, SpellCraftingRules craftingRules = null)
        {
            currentSpell = template != null ? template.Clone() : ScriptableObject.CreateInstance<SpellTemplate>();
            rules = craftingRules;
            playerLevel = 0;
            unlockedRunes = new List<BaseRune>();
        }

        /// <summary>
        /// Set the player level for validation
        /// </summary>
        public SpellBuilder WithPlayerLevel(int level)
        {
            playerLevel = level;
            return this;
        }

        /// <summary>
        /// Set the list of unlocked runes for validation
        /// </summary>
        public SpellBuilder WithUnlockedRunes(List<BaseRune> runes)
        {
            unlockedRunes = runes ?? new List<BaseRune>();
            return this;
        }

        /// <summary>
        /// Set the spell name
        /// </summary>
        public SpellBuilder WithName(string name)
        {
            currentSpell.SpellName = name;
            return this;
        }

        /// <summary>
        /// Set the spell description
        /// </summary>
        public SpellBuilder WithDescription(string description)
        {
            currentSpell.SpellDescription = description;
            return this;
        }

        /// <summary>
        /// Set the spell icon
        /// </summary>
        public SpellBuilder WithIcon(Sprite icon)
        {
            currentSpell.SpellIcon = icon;
            return this;
        }

        /// <summary>
        /// Set the Focus
        /// </summary>
        public SpellBuilder WithFocus(FocusDefinition focus)
        {
            currentSpell.Focus = focus;
            return this;
        }

        /// <summary>
        /// Set the Essence
        /// </summary>
        public SpellBuilder WithEssence(EssenceDefinition essence)
        {
            currentSpell.Essence = essence;
            return this;
        }

        /// <summary>
        /// Add a rune to the spell
        /// </summary>
        public SpellBuilder AddRune(BaseRune rune)
        {
            if (rune == null)
            {
                validationErrors.Add("Cannot add null rune");
                return this;
            }

            // Check if we've hit the total rune limit
            if (rules != null && rules.MaxTotalRunes > 0)
            {
                if (currentSpell.Runes.Count >= rules.MaxTotalRunes)
                {
                    validationErrors.Add($"Cannot add rune: maximum rune limit ({rules.MaxTotalRunes}) reached");
                    return this;
                }
            }

            // Check category limit
            if (rules != null)
            {
                int categoryCount = currentSpell.GetRunesByCategory(rune.Category).Count;
                int maxForCategory = rules.GetMaxRunesForCategory(rune.Category);
                if (maxForCategory > 0 && categoryCount >= maxForCategory)
                {
                    validationErrors.Add($"Cannot add rune: maximum {rune.Category} runes ({maxForCategory}) reached");
                    return this;
                }
            }

            // Attempt to add the rune
            if (!currentSpell.AddRune(rune))
            {
                validationErrors.Add($"Failed to add rune '{rune.RuneName}'");
            }

            return this;
        }

        /// <summary>
        /// Add multiple runes at once
        /// </summary>
        public SpellBuilder AddRunes(params BaseRune[] runes)
        {
            foreach (var rune in runes)
            {
                AddRune(rune);
            }
            return this;
        }

        /// <summary>
        /// Remove a rune from the spell
        /// </summary>
        public SpellBuilder RemoveRune(BaseRune rune)
        {
            if (rune == null)
            {
                validationErrors.Add("Cannot remove null rune");
                return this;
            }

            if (!currentSpell.RemoveRune(rune))
            {
                validationWarnings.Add($"Rune '{rune.RuneName}' was not in the spell");
            }

            return this;
        }

        /// <summary>
        /// Clear all runes from the spell
        /// </summary>
        public SpellBuilder ClearRunes()
        {
            currentSpell.ClearRunes();
            return this;
        }

        /// <summary>
        /// Validate the current spell
        /// </summary>
        public bool Validate()
        {
            validationErrors.Clear();
            validationWarnings.Clear();

            // Validate using template's own validation
            List<string> templateErrors;
            if (!currentSpell.Validate(out templateErrors))
            {
                validationErrors.AddRange(templateErrors);
            }

            // Validate using crafting rules if provided
            if (rules != null)
            {
                List<string> ruleErrors, ruleWarnings;
                rules.ValidateSpell(currentSpell, playerLevel, unlockedRunes, out ruleErrors, out ruleWarnings);
                validationErrors.AddRange(ruleErrors);
                validationWarnings.AddRange(ruleWarnings);
            }

            return validationErrors.Count == 0;
        }

        /// <summary>
        /// Get validation errors
        /// </summary>
        public List<string> GetErrors()
        {
            return new List<string>(validationErrors);
        }

        /// <summary>
        /// Get validation warnings
        /// </summary>
        public List<string> GetWarnings()
        {
            return new List<string>(validationWarnings);
        }

        /// <summary>
        /// Check if the spell is valid
        /// </summary>
        public bool IsValid()
        {
            return Validate();
        }

        /// <summary>
        /// Build and return the final spell template
        /// </summary>
        public SpellTemplate Build()
        {
            if (!Validate())
            {
                Debug.LogError($"Cannot build spell '{currentSpell.SpellName}': Validation failed");
                foreach (var error in validationErrors)
                {
                    Debug.LogError($"  - {error}");
                }
                return null;
            }

            if (validationWarnings.Count > 0)
            {
                Debug.LogWarning($"Spell '{currentSpell.SpellName}' has warnings:");
                foreach (var warning in validationWarnings)
                {
                    Debug.LogWarning($"  - {warning}");
                }
            }

            currentSpell.UpdateModifiedDate();
            return currentSpell;
        }

        /// <summary>
        /// Build without validation (use with caution!)
        /// </summary>
        public SpellTemplate BuildUnsafe()
        {
            currentSpell.UpdateModifiedDate();
            return currentSpell;
        }

        /// <summary>
        /// Get a preview of the spell's stats
        /// </summary>
        public SpellContext GetPreview()
        {
            return currentSpell.BuildContext();
        }

        /// <summary>
        /// Get the current spell template being built (work in progress)
        /// </summary>
        public SpellTemplate GetCurrentSpell()
        {
            return currentSpell;
        }

        /// <summary>
        /// Reset the builder with a new spell
        /// </summary>
        public SpellBuilder Reset()
        {
            currentSpell = ScriptableObject.CreateInstance<SpellTemplate>();
            validationErrors.Clear();
            validationWarnings.Clear();
            return this;
        }

        /// <summary>
        /// Create a quick spell with minimal configuration (for testing)
        /// </summary>
        public static SpellTemplate QuickBuild(string name, FocusDefinition focus, EssenceDefinition essence, params BaseRune[] runes)
        {
            var builder = new SpellBuilder();
            builder.WithName(name)
                   .WithFocus(focus)
                   .WithEssence(essence)
                   .AddRunes(runes);

            return builder.BuildUnsafe();
        }
    }
}

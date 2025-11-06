using ModularAbilityCraftingSystem.Runes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Manages a collection of spells for a player or NPC.
    /// Provides searching, filtering, and activation functionality.
    /// </summary>
    public class Spellbook : MonoBehaviour
    {
        [Header("Spell Collection")]
        [Tooltip("All spells in this spellbook")]
        [SerializeField] private List<SpellTemplate> spells = new List<SpellTemplate>();

        [Tooltip("Active spells (assigned to hotbar/slots)")]
        [SerializeField] private List<SpellTemplate> activeSpells = new List<SpellTemplate>();

        [Tooltip("Maximum number of active spells")]
        [SerializeField] private int maxActiveSpells = 8;

        [Header("Rune Management")]
        [Tooltip("All unlocked runes available for spell crafting")]
        [SerializeField] private List<BaseRune> unlockedRunes = new List<BaseRune>();

        [Header("Configuration")]
        [Tooltip("Spell crafting rules for validation")]
        [SerializeField] private SpellCraftingRules craftingRules;

        // Events
        public event Action<SpellTemplate> OnSpellAdded;
        public event Action<SpellTemplate> OnSpellRemoved;
        public event Action<SpellTemplate> OnSpellActivated;
        public event Action<SpellTemplate> OnSpellDeactivated;
        public event Action<BaseRune> OnRuneUnlocked;

        // Public Properties
        public List<SpellTemplate> Spells => new List<SpellTemplate>(spells);
        public List<SpellTemplate> ActiveSpells => new List<SpellTemplate>(activeSpells);
        public List<BaseRune> UnlockedRunes => new List<BaseRune>(unlockedRunes);
        public SpellCraftingRules CraftingRules => craftingRules;
        public int MaxActiveSpells => maxActiveSpells;

        /// <summary>
        /// Add a spell to the spellbook
        /// </summary>
        public bool AddSpell(SpellTemplate spell)
        {
            if (spell == null)
            {
                Debug.LogWarning("Cannot add null spell to spellbook");
                return false;
            }

            if (spells.Contains(spell))
            {
                Debug.LogWarning($"Spell '{spell.SpellName}' already in spellbook");
                return false;
            }

            spells.Add(spell);
            OnSpellAdded?.Invoke(spell);
            return true;
        }

        /// <summary>
        /// Remove a spell from the spellbook
        /// </summary>
        public bool RemoveSpell(SpellTemplate spell)
        {
            if (spell == null) return false;

            // Deactivate if active
            if (activeSpells.Contains(spell))
            {
                DeactivateSpell(spell);
            }

            bool removed = spells.Remove(spell);
            if (removed)
            {
                OnSpellRemoved?.Invoke(spell);
            }
            return removed;
        }

        /// <summary>
        /// Activate a spell (add to active slots)
        /// </summary>
        public bool ActivateSpell(SpellTemplate spell)
        {
            if (spell == null) return false;

            if (!spells.Contains(spell))
            {
                Debug.LogWarning($"Cannot activate spell '{spell.SpellName}' - not in spellbook");
                return false;
            }

            if (activeSpells.Contains(spell))
            {
                Debug.LogWarning($"Spell '{spell.SpellName}' is already active");
                return false;
            }

            if (activeSpells.Count >= maxActiveSpells)
            {
                Debug.LogWarning($"Cannot activate spell - maximum active spells ({maxActiveSpells}) reached");
                return false;
            }

            activeSpells.Add(spell);
            OnSpellActivated?.Invoke(spell);
            return true;
        }

        /// <summary>
        /// Deactivate a spell (remove from active slots)
        /// </summary>
        public bool DeactivateSpell(SpellTemplate spell)
        {
            if (spell == null) return false;

            bool removed = activeSpells.Remove(spell);
            if (removed)
            {
                OnSpellDeactivated?.Invoke(spell);
            }
            return removed;
        }

        /// <summary>
        /// Check if a spell is active
        /// </summary>
        public bool IsSpellActive(SpellTemplate spell)
        {
            return activeSpells.Contains(spell);
        }

        /// <summary>
        /// Search spells by name
        /// </summary>
        public List<SpellTemplate> SearchByName(string query)
        {
            if (string.IsNullOrEmpty(query)) return new List<SpellTemplate>(spells);

            return spells.Where(s => s.SpellName.ToLower().Contains(query.ToLower())).ToList();
        }

        /// <summary>
        /// Filter spells by essence type
        /// </summary>
        public List<SpellTemplate> FilterByEssence(Utilities.EssenceType essence)
        {
            return spells.Where(s => s.Essence != null && s.Essence.EssenceType == essence).ToList();
        }

        /// <summary>
        /// Filter spells by focus type
        /// </summary>
        public List<SpellTemplate> FilterByFocus(Utilities.FocusType focus)
        {
            return spells.Where(s => s.Focus != null && s.Focus.FocusType == focus).ToList();
        }

        /// <summary>
        /// Filter spells by tag
        /// </summary>
        public List<SpellTemplate> FilterByTag(string tag)
        {
            return spells.Where(s => s.Tags != null && s.Tags.Contains(tag)).ToList();
        }

        /// <summary>
        /// Filter spells by rune
        /// </summary>
        public List<SpellTemplate> FilterByRune(BaseRune rune)
        {
            return spells.Where(s => s.Runes.Contains(rune)).ToList();
        }

        /// <summary>
        /// Get spells sorted by creation date
        /// </summary>
        public List<SpellTemplate> GetSortedByDate(bool ascending = false)
        {
            return ascending
                ? spells.OrderBy(s => s.CreationDate).ToList()
                : spells.OrderByDescending(s => s.CreationDate).ToList();
        }

        /// <summary>
        /// Get spells sorted by name
        /// </summary>
        public List<SpellTemplate> GetSortedByName(bool ascending = true)
        {
            return ascending
                ? spells.OrderBy(s => s.SpellName).ToList()
                : spells.OrderByDescending(s => s.SpellName).ToList();
        }

        /// <summary>
        /// Get spells sorted by mana cost
        /// </summary>
        public List<SpellTemplate> GetSortedByManaCost(bool ascending = true)
        {
            return ascending
                ? spells.OrderBy(s => s.BuildContext().ManaCost).ToList()
                : spells.OrderByDescending(s => s.BuildContext().ManaCost).ToList();
        }

        /// <summary>
        /// Unlock a rune for crafting
        /// </summary>
        public bool UnlockRune(BaseRune rune)
        {
            if (rune == null) return false;

            if (unlockedRunes.Contains(rune))
            {
                Debug.LogWarning($"Rune '{rune.RuneName}' is already unlocked");
                return false;
            }

            unlockedRunes.Add(rune);
            OnRuneUnlocked?.Invoke(rune);
            return true;
        }

        /// <summary>
        /// Check if a rune is unlocked
        /// </summary>
        public bool IsRuneUnlocked(BaseRune rune)
        {
            return unlockedRunes.Contains(rune);
        }

        /// <summary>
        /// Get unlocked runes by category
        /// </summary>
        public List<BaseRune> GetUnlockedRunesByCategory(RuneCategory category)
        {
            return unlockedRunes.Where(r => r.Category == category).ToList();
        }

        /// <summary>
        /// Clear all spells from the spellbook
        /// </summary>
        public void ClearSpells()
        {
            activeSpells.Clear();
            spells.Clear();
        }

        /// <summary>
        /// Get available active spell slots
        /// </summary>
        public int GetAvailableSlots()
        {
            return maxActiveSpells - activeSpells.Count;
        }

        /// <summary>
        /// Set a spell at a specific active slot index
        /// </summary>
        public bool SetActiveSpellAtSlot(int slotIndex, SpellTemplate spell)
        {
            if (slotIndex < 0 || slotIndex >= maxActiveSpells)
            {
                Debug.LogWarning($"Invalid slot index: {slotIndex}");
                return false;
            }

            if (spell != null && !spells.Contains(spell))
            {
                Debug.LogWarning($"Spell '{spell.SpellName}' not in spellbook");
                return false;
            }

            // Expand active spells list if needed
            while (activeSpells.Count <= slotIndex)
            {
                activeSpells.Add(null);
            }

            // Deactivate old spell at this slot if present
            if (activeSpells[slotIndex] != null)
            {
                OnSpellDeactivated?.Invoke(activeSpells[slotIndex]);
            }

            activeSpells[slotIndex] = spell;

            if (spell != null)
            {
                OnSpellActivated?.Invoke(spell);
            }

            return true;
        }

        /// <summary>
        /// Get spell at specific active slot
        /// </summary>
        public SpellTemplate GetActiveSpellAtSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= activeSpells.Count)
            {
                return null;
            }
            return activeSpells[slotIndex];
        }

        /// <summary>
        /// Validate all spells in the spellbook
        /// </summary>
        public Dictionary<SpellTemplate, List<string>> ValidateAllSpells(int playerLevel)
        {
            Dictionary<SpellTemplate, List<string>> validationResults = new Dictionary<SpellTemplate, List<string>>();

            foreach (var spell in spells)
            {
                List<string> errors;
                spell.Validate(out errors);

                if (craftingRules != null)
                {
                    List<string> warnings;
                    craftingRules.ValidateSpell(spell, playerLevel, unlockedRunes, out var ruleErrors, out warnings);
                    errors.AddRange(ruleErrors);
                }

                if (errors.Count > 0)
                {
                    validationResults[spell] = errors;
                }
            }

            return validationResults;
        }
    }
}

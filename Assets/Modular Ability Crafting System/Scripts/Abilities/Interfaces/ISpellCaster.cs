using System.Collections.Generic;

namespace ModularAbilityCraftingSystem.Abilities.Interfaces
{
    /// <summary>
    /// Interface for entities that can cast spells (players, NPCs, objects, etc.)
    /// </summary>
    public interface ISpellCaster
    {
        /// <summary>
        /// Get all available spells for this caster
        /// </summary>
        List<SpellTemplate> GetAvailableSpells();

        /// <summary>
        /// Get all active spells (equipped to hotbar/slots)
        /// </summary>
        List<SpellTemplate> GetActiveSpells();

        /// <summary>
        /// Check if caster has enough mana to cast a spell
        /// </summary>
        bool HasEnoughMana(float cost);

        /// <summary>
        /// Consume mana for casting
        /// </summary>
        bool ConsumeMana(float cost);

        /// <summary>
        /// Get current mana
        /// </summary>
        float GetCurrentMana();

        /// <summary>
        /// Get maximum mana
        /// </summary>
        float GetMaxMana();

        /// <summary>
        /// Get caster level (for rune requirements)
        /// </summary>
        int GetLevel();
    }
}

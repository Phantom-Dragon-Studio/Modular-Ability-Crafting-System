using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities.Interfaces
{
    /// <summary>
    /// Interface for anything that can be cast (spells, abilities, items, etc.)
    /// External systems like hotbars, VR gestures, etc. can use this interface.
    /// </summary>
    public interface ICastable
    {
        /// <summary>
        /// Get the name of this castable
        /// </summary>
        string GetName();

        /// <summary>
        /// Get the icon for UI display
        /// </summary>
        Sprite GetIcon();

        /// <summary>
        /// Get the description
        /// </summary>
        string GetDescription();

        /// <summary>
        /// Check if this can currently be cast
        /// </summary>
        bool CanCast();

        /// <summary>
        /// Cast this ability
        /// </summary>
        /// <param name="position">Cast position</param>
        /// <param name="direction">Cast direction</param>
        /// <param name="caster">The caster (optional)</param>
        /// <param name="target">The target (optional)</param>
        /// <returns>The spawned spell GameObject, or null if failed</returns>
        GameObject Cast(Vector3 position, Vector3 direction, Transform caster = null, Transform target = null);

        /// <summary>
        /// Get the mana cost
        /// </summary>
        float GetManaCost();

        /// <summary>
        /// Get the cooldown duration
        /// </summary>
        float GetCooldown();

        /// <summary>
        /// Get remaining cooldown time
        /// </summary>
        float GetRemainingCooldown();

        /// <summary>
        /// Check if currently on cooldown
        /// </summary>
        bool IsOnCooldown();
    }
}

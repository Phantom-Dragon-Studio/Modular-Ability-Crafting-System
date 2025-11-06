namespace ModularAbilityCraftingSystem.Runes
{
    /// <summary>
    /// Tags used for rune compatibility and filtering.
    /// Runes can require or conflict with specific tags.
    /// </summary>
    public enum RuneTag
    {
        // Spell Type Tags
        Projectile,
        Melee,
        AOE,
        Beam,
        Summon,
        Buff,
        Debuff,

        // Property Tags
        Mobile,         // Spell can move
        Stationary,     // Spell stays in place
        Targetable,     // Can be targeted by other effects
        Persistent,     // Lasts over time
        Instant,        // Immediate effect

        // Element Tags
        Elemental,      // Has an elemental essence
        Physical,       // Physical damage
        Magical,        // Magical damage

        // Special Tags
        Explosive,      // Can explode
        Homing,         // Can track targets
        Chainable,      // Can chain to multiple targets
        Spawning,       // Creates entities
        Transforming,   // Changes form

        // Restriction Tags
        NoMovement,     // Cannot be combined with movement
        NoMultiplication, // Cannot be duplicated
        SingleTarget,   // Only affects one target
        MultiTarget     // Can affect multiple targets
    }
}

namespace ModularAbilityCraftingSystem.StatusEffects
{
    /// <summary>
    /// Types of status effects that can be applied
    /// </summary>
    public enum StatusEffectType
    {
        None = 0,

        // Damage over Time
        Burn,           // Fire damage over time
        Poison,         // Poison damage over time
        Bleed,          // Physical damage over time
        Shock,          // Electric damage over time

        // Control Effects
        Freeze,         // Immobilize target
        Stun,           // Disable target actions
        Slow,           // Reduce movement speed
        Root,           // Prevent movement

        // Debuffs
        Weaken,         // Reduce damage output
        Vulnerable,     // Increase damage taken
        Silence,        // Prevent casting
        Blind,          // Reduce accuracy

        // Knockback/Movement
        Knockback,      // Push target away
        Pull,           // Pull target towards caster
        Levitate,       // Lift target off ground

        // Special
        Fear,           // Force target to flee
        Charm,          // Control target
        Confusion,      // Randomize target actions
        Curse,          // Generic curse effect

        // Buffs (for self-targeting spells)
        Haste,          // Increase speed
        Shield,         // Absorb damage
        Regeneration,   // Heal over time
        Empowered       // Increase damage
    }
}

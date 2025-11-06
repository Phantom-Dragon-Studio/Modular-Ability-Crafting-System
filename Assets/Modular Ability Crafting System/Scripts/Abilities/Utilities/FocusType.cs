namespace ModularAbilityCraftingSystem.Abilities.Utilities
{
    /// <summary>
    /// Defines the base behavior and delivery method of a spell.
    /// Focus determines HOW the spell manifests in the world.
    /// </summary>
    public enum FocusType
    {
        /// <summary>Instant/melee spells with immediate effect (slashes, explosions)</summary>
        Immedi = 0,

        /// <summary>Projectile/active spells with conditional duration (orbs, projectiles, mines)</summary>
        Actus = 1,

        /// <summary>Create/summon spells with durable effects (bridges, walls, platforms)</summary>
        Creo = 2,

        /// <summary>Self-targeting spells affecting the caster (shields, buffs, dashes)</summary>
        Ego = 3,

        /// <summary>Area of effect spells (zones, auras, pulses)</summary>
        Area = 4,

        /// <summary>Beam/ray spells with continuous effect</summary>
        Beam = 5,

        /// <summary>Trap/delayed spells that activate on trigger conditions</summary>
        Trap = 6,

        /// <summary>Totem/turret spells that spawn autonomous entities</summary>
        Totem = 7
    }
}

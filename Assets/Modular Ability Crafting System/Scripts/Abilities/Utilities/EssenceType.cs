namespace ModularAbilityCraftingSystem.Abilities.Utilities
{
    /// <summary>
    /// Defines the elemental nature and damage type of a spell.
    /// Essence determines the visual effects, damage properties, and special interactions.
    /// </summary>
    public enum EssenceType
    {
        // Core Elements (Traditional 4)
        /// <summary>Fire element - burning, damage over time</summary>
        Igni = 0,

        /// <summary>Water/Ice element - freezing, slowing, bridging</summary>
        Aqua = 1,

        /// <summary>Electric element - shocking, chaining, activation</summary>
        Aura = 2,

        /// <summary>Earth/Nature element - protection, poison, terrain</summary>
        Gaea = 3,

        // Extended Elements
        /// <summary>Wind/Air element - knockback, movement, levitation</summary>
        Ventus = 4,

        /// <summary>Light/Holy element - healing, undead damage, revealing</summary>
        Lux = 5,

        /// <summary>Dark/Shadow element - life drain, fear, concealment</summary>
        Umbra = 6,

        /// <summary>Arcane/Pure magic - raw damage, mana manipulation</summary>
        Arcanum = 7,

        /// <summary>Void/Chaos element - unstable, random effects, antimagic</summary>
        Chaos = 8,

        /// <summary>Crystal/Gem element - reflective, piercing, resource generation</summary>
        Crystal = 9,

        /// <summary>Blood/Life element - health sacrifice, vampirism, corruption</summary>
        Vitae = 10,

        /// <summary>Gravity/Force element - pull/push, crushing, orbit</summary>
        Gravitas = 11,

        /// <summary>Time/Temporal element - slow, haste, rewind</summary>
        Tempus = 12,

        /// <summary>Poison/Acid element - corrosion, DoT, armor reduction</summary>
        Toxin = 13,

        /// <summary>Sound/Sonic element - shockwave, stun, echo</summary>
        Sonus = 14,

        /// <summary>Neutral/Pure - no element, for utility spells</summary>
        Neutral = 15
    }
}

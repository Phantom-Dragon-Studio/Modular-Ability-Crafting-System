namespace ModularAbilityCraftingSystem.Runes
{
    /// <summary>
    /// Categorizes runes by their primary function in spell composition.
    /// This helps with UI organization and validation.
    /// </summary>
    public enum RuneCategory
    {
        /// <summary>Defines WHEN a spell activates or triggers</summary>
        Trigger = 0,

        /// <summary>Modifies spell movement and trajectory</summary>
        Movement = 1,

        /// <summary>Creates multiple instances or repetitions of effects</summary>
        Multiplication = 2,

        /// <summary>Modifies spell properties (size, duration, power)</summary>
        Augmentation = 3,

        /// <summary>Adds impact and explosion effects</summary>
        Impact = 4,

        /// <summary>Applies status effects and special conditions</summary>
        Mastery = 5,

        /// <summary>Modifies targeting and selection behavior</summary>
        Targeting = 6,

        /// <summary>Adds conditional logic and branching</summary>
        Conditional = 7,

        /// <summary>Utility and miscellaneous effects</summary>
        Utility = 8
    }
}

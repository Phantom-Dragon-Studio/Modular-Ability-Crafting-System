using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Triggers
{
    /// <summary>
    /// Base class for trigger runes that define WHEN a spell activates or triggers.
    /// Examples: OnImpact, OnTimer, OnProximity, Periodic
    /// </summary>
    public abstract class BaseTrigger : BaseRune
    {
        [Header("Trigger Settings")]
        [Tooltip("Can this trigger activate multiple times?")]
        [SerializeField] protected bool canRepeat = false;

        [Tooltip("Cooldown between trigger activations (if repeatable)")]
        [SerializeField] protected float triggerCooldown = 0f;

        [Tooltip("Delay before first trigger can activate")]
        [SerializeField] protected float activationDelay = 0f;

        public bool CanRepeat => canRepeat;
        public float TriggerCooldown => triggerCooldown;
        public float ActivationDelay => activationDelay;

        /// <summary>
        /// Attach trigger logic to a spell instance.
        /// This is called at runtime when the spell is cast.
        /// </summary>
        /// <param name="context">The spell context</param>
        /// <param name="spellInstance">The spell GameObject to attach trigger to</param>
        public abstract void AttachTrigger(SpellContext context, GameObject spellInstance);
    }
}

using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours
{
    /// <summary>
    /// Base class for behaviour runes that define WHAT a spell does.
    /// Examples: Move, Homing, Bounce, Orbit, Rain
    /// </summary>
    public abstract class BaseBehaviour : BaseRune
    {
        [Header("Behaviour Settings")]
        [Tooltip("Priority order for behaviour execution (lower = earlier)")]
        [SerializeField] protected int executionPriority = 100;

        [Tooltip("Should this behaviour update every frame?")]
        [SerializeField] protected bool requiresUpdate = true;

        public int ExecutionPriority => executionPriority;
        public bool RequiresUpdate => requiresUpdate;

        /// <summary>
        /// Attach behaviour logic to a spell instance.
        /// This is called at runtime when the spell is cast.
        /// </summary>
        /// <param name="context">The spell context</param>
        /// <param name="spellInstance">The spell GameObject to attach behaviour to</param>
        public abstract void AttachBehaviour(SpellContext context, GameObject spellInstance);
    }
}

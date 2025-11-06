using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Abilities.Interfaces;
using ModularAbilityCraftingSystem.Abilities.StateMachines;
using System;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Sockets
{
    /// <summary>
    /// Represents a spell slot that can hold and cast a spell.
    /// Used by hotbars, quick-cast menus, or any UI that needs spell slots.
    /// </summary>
    public class AbilitySocket : MonoBehaviour
    {
        [Header("Socket Configuration")]
        [Tooltip("The spell assigned to this socket")]
        [SerializeField] private SpellTemplate assignedSpell;

        [Tooltip("Spell executor for casting")]
        [SerializeField] private SpellExecutor executor;

        [Tooltip("The caster using this socket")]
        [SerializeField] private MonoBehaviour casterComponent;

        [Header("Socket Properties")]
        [Tooltip("Unique identifier for this socket")]
        [SerializeField] private string socketId;

        [Tooltip("Socket index in a hotbar/grid")]
        [SerializeField] private int socketIndex = 0;

        [Tooltip("Is this socket locked (cannot be changed)?")]
        [SerializeField] private bool isLocked = false;

        [Header("References")]
        [SerializeField] private AbilityStateMachine stateMachine;

        private CastableSpell castableSpell;
        private ISpellCaster spellCaster;

        // Events
        public event Action<SpellTemplate> OnSpellAssigned;
        public event Action<SpellTemplate> OnSpellCleared;
        public event Action<GameObject> OnSpellCast;

        // Public Properties
        public SpellTemplate AssignedSpell => assignedSpell;
        public int SocketIndex => socketIndex;
        public bool IsLocked => isLocked;
        public bool IsEmpty => assignedSpell == null;
        public string SocketId => socketId;

        private void Awake()
        {
            // Generate socket ID if not set
            if (string.IsNullOrEmpty(socketId))
            {
                socketId = Guid.NewGuid().ToString();
            }

            // Try to get spell caster interface
            if (casterComponent != null && casterComponent is ISpellCaster caster)
            {
                spellCaster = caster;
            }

            // Auto-find executor if not set
            if (executor == null)
            {
                executor = FindObjectOfType<SpellExecutor>();
            }

            // Initialize castable spell if we have a spell assigned
            if (assignedSpell != null)
            {
                UpdateCastableSpell();
            }
        }

        /// <summary>
        /// Assign a spell to this socket
        /// </summary>
        public bool AssignSpell(SpellTemplate spell)
        {
            if (isLocked)
            {
                Debug.LogWarning($"Socket {socketIndex} is locked");
                return false;
            }

            if (spell == null)
            {
                return ClearSpell();
            }

            assignedSpell = spell;
            UpdateCastableSpell();
            OnSpellAssigned?.Invoke(spell);
            return true;
        }

        /// <summary>
        /// Clear the spell from this socket
        /// </summary>
        public bool ClearSpell()
        {
            if (isLocked)
            {
                Debug.LogWarning($"Socket {socketIndex} is locked");
                return false;
            }

            SpellTemplate oldSpell = assignedSpell;
            assignedSpell = null;
            castableSpell = null;
            OnSpellCleared?.Invoke(oldSpell);
            return true;
        }

        /// <summary>
        /// Cast the spell in this socket
        /// </summary>
        public GameObject Cast(Vector3 position, Vector3 direction, Transform caster = null, Transform target = null)
        {
            if (!CanCast())
            {
                Debug.LogWarning($"Cannot cast spell in socket {socketIndex}");
                return null;
            }

            GameObject spellInstance = castableSpell.Cast(position, direction, caster, target);
            if (spellInstance != null)
            {
                OnSpellCast?.Invoke(spellInstance);
            }

            return spellInstance;
        }

        /// <summary>
        /// Check if this socket can cast its spell
        /// </summary>
        public bool CanCast()
        {
            return castableSpell != null && castableSpell.CanCast();
        }

        /// <summary>
        /// Get the castable spell wrapper
        /// </summary>
        public CastableSpell GetCastableSpell()
        {
            return castableSpell;
        }

        /// <summary>
        /// Lock this socket (prevent spell changes)
        /// </summary>
        public void Lock()
        {
            isLocked = true;
        }

        /// <summary>
        /// Unlock this socket
        /// </summary>
        public void Unlock()
        {
            isLocked = false;
        }

        /// <summary>
        /// Set the socket index
        /// </summary>
        public void SetIndex(int index)
        {
            socketIndex = index;
        }

        /// <summary>
        /// Set the caster for this socket
        /// </summary>
        public void SetCaster(ISpellCaster caster)
        {
            spellCaster = caster;
            if (castableSpell != null)
            {
                castableSpell.SetCaster(caster);
            }
        }

        /// <summary>
        /// Set the executor for this socket
        /// </summary>
        public void SetExecutor(SpellExecutor newExecutor)
        {
            executor = newExecutor;
            if (castableSpell != null)
            {
                castableSpell.SetExecutor(newExecutor);
            }
        }

        /// <summary>
        /// Update the castable spell wrapper
        /// </summary>
        private void UpdateCastableSpell()
        {
            if (assignedSpell != null && executor != null)
            {
                castableSpell = new CastableSpell(assignedSpell, executor, spellCaster);
            }
            else
            {
                castableSpell = null;
            }
        }

        /// <summary>
        /// Legacy method for compatibility
        /// </summary>
        public void Activate()
        {
            if (casterComponent != null && casterComponent.transform != null)
            {
                Cast(casterComponent.transform.position, casterComponent.transform.forward, casterComponent.transform);
            }
        }
    }
}

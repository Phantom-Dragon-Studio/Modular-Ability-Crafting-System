using ModularAbilityCraftingSystem.Abilities;
using ModularAbilityCraftingSystem.Abilities.Interfaces;
using ModularAbilityCraftingSystem.Sockets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularAbilityCraftingSystem.Examples
{
    /// <summary>
    /// Example player controller demonstrating spell casting integration.
    /// Uses Unity's NEW Input System (not legacy Input).
    /// Attach this to your player GameObject along with Spellbook and SpellExecutor.
    /// Requires: Unity Input System package (com.unity.inputsystem)
    /// </summary>
    [RequireComponent(typeof(Spellbook))]
    [RequireComponent(typeof(PlayerInput))]
    public class ExampleSpellCasterPlayer : MonoBehaviour, ISpellCaster
    {
        [Header("Mana System")]
        [Tooltip("Current mana")]
        [SerializeField] private float currentMana = 100f;

        [Tooltip("Maximum mana")]
        [SerializeField] private float maxMana = 100f;

        [Tooltip("Mana regeneration per second")]
        [SerializeField] private float manaRegenRate = 5f;

        [Header("Spell Casting")]
        [Tooltip("Spell executor reference")]
        [SerializeField] private SpellExecutor executor;

        [Tooltip("Spell sockets (hotbar)")]
        [SerializeField] private AbilitySocket[] spellSlots = new AbilitySocket[6];

        [Tooltip("Cast point offset from player")]
        [SerializeField] private Vector3 castPointOffset = new Vector3(0, 1.5f, 0.5f);

        [Header("Player Stats")]
        [Tooltip("Player level (for rune requirements)")]
        [SerializeField] private int playerLevel = 1;

        [Header("Debug")]
        [Tooltip("Show debug information")]
        [SerializeField] private bool showDebugInfo = false;

        // References
        private Spellbook spellbook;
        private Camera playerCamera;
        private PlayerInput playerInput;
        private InputAction[] slotActions;
        private InputAction primaryAction;
        private InputAction secondaryAction;

        // Properties
        public float CurrentMana => currentMana;
        public float MaxMana => maxMana;
        public int PlayerLevel => playerLevel;

        private void Awake()
        {
            spellbook = GetComponent<Spellbook>();
            playerCamera = Camera.main;
            playerInput = GetComponent<PlayerInput>();

            // Auto-find executor if not set
            if (executor == null)
            {
                executor = FindObjectOfType<SpellExecutor>();
            }

            // Initialize sockets
            for (int i = 0; i < spellSlots.Length; i++)
            {
                if (spellSlots[i] != null)
                {
                    spellSlots[i].SetIndex(i);
                    spellSlots[i].SetCaster(this);
                    spellSlots[i].SetExecutor(executor);
                }
            }

            // Setup input actions (New Input System)
            SetupInputActions();
        }

        private void OnEnable()
        {
            EnableInputActions();
        }

        private void OnDisable()
        {
            DisableInputActions();
        }

        private void Update()
        {
            // Regenerate mana
            RegenerateMana();

            // Debug display
            if (showDebugInfo)
            {
                DisplayDebugInfo();
            }
        }

        /// <summary>
        /// Regenerate mana over time
        /// </summary>
        private void RegenerateMana()
        {
            if (currentMana < maxMana)
            {
                currentMana = Mathf.Min(currentMana + manaRegenRate * Time.deltaTime, maxMana);
            }
        }

        /// <summary>
        /// Setup input actions using New Input System
        /// </summary>
        private void SetupInputActions()
        {
            if (playerInput == null) return;

            var map = playerInput.actions.FindActionMap("SpellCasting");
            if (map == null)
            {
                Debug.LogWarning("SpellCasting action map not found. Make sure SpellCastingInputActions is assigned to PlayerInput component.");
                return;
            }

            // Setup slot actions (1-6)
            slotActions = new InputAction[6];
            for (int i = 0; i < 6; i++)
            {
                int slotIndex = i; // Capture for lambda
                slotActions[i] = map.FindAction($"CastSlot{i + 1}");
                if (slotActions[i] != null)
                {
                    slotActions[i].performed += ctx => OnCastSlot(slotIndex);
                }
            }

            // Setup primary/secondary actions
            primaryAction = map.FindAction("CastPrimary");
            if (primaryAction != null)
            {
                primaryAction.performed += ctx => OnCastSlot(0);
            }

            secondaryAction = map.FindAction("CastSecondary");
            if (secondaryAction != null)
            {
                secondaryAction.performed += ctx => OnCastSlot(1);
            }
        }

        /// <summary>
        /// Enable input actions
        /// </summary>
        private void EnableInputActions()
        {
            if (slotActions != null)
            {
                foreach (var action in slotActions)
                {
                    action?.Enable();
                }
            }
            primaryAction?.Enable();
            secondaryAction?.Enable();
        }

        /// <summary>
        /// Disable input actions
        /// </summary>
        private void DisableInputActions()
        {
            if (slotActions != null)
            {
                foreach (var action in slotActions)
                {
                    action?.Disable();
                }
            }
            primaryAction?.Disable();
            secondaryAction?.Disable();
        }

        /// <summary>
        /// Called when a cast slot input is triggered
        /// </summary>
        private void OnCastSlot(int slotIndex)
        {
            CastSpellFromSlot(slotIndex);
        }

        /// <summary>
        /// Cast spell from a specific slot
        /// </summary>
        public void CastSpellFromSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= spellSlots.Length)
            {
                Debug.LogWarning($"Invalid slot index: {slotIndex}");
                return;
            }

            AbilitySocket socket = spellSlots[slotIndex];
            if (socket == null || socket.IsEmpty)
            {
                if (showDebugInfo) Debug.Log($"Slot {slotIndex} is empty");
                return;
            }

            if (!socket.CanCast())
            {
                if (showDebugInfo) Debug.Log($"Cannot cast spell in slot {slotIndex}");
                return;
            }

            // Get cast position and direction
            Vector3 castPosition = transform.position + transform.TransformDirection(castPointOffset);
            Vector3 castDirection = GetAimDirection();

            // Cast the spell
            GameObject spellInstance = socket.Cast(castPosition, castDirection, transform);

            if (spellInstance != null && showDebugInfo)
            {
                Debug.Log($"Cast spell: {socket.AssignedSpell.SpellName}");
            }
        }

        /// <summary>
        /// Get aim direction (forward by default, or camera direction if available)
        /// </summary>
        private Vector3 GetAimDirection()
        {
            if (playerCamera != null)
            {
                return playerCamera.transform.forward;
            }
            return transform.forward;
        }

        /// <summary>
        /// Assign a spell to a slot
        /// </summary>
        public void AssignSpellToSlot(SpellTemplate spell, int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= spellSlots.Length)
            {
                Debug.LogWarning($"Invalid slot index: {slotIndex}");
                return;
            }

            if (spellSlots[slotIndex] != null)
            {
                spellSlots[slotIndex].AssignSpell(spell);
                if (showDebugInfo) Debug.Log($"Assigned {spell?.SpellName ?? "null"} to slot {slotIndex}");
            }
        }

        /// <summary>
        /// Level up the player
        /// </summary>
        public void LevelUp()
        {
            playerLevel++;
            if (showDebugInfo) Debug.Log($"Level up! Now level {playerLevel}");
        }

        /// <summary>
        /// Add mana
        /// </summary>
        public void AddMana(float amount)
        {
            currentMana = Mathf.Min(currentMana + amount, maxMana);
        }

        /// <summary>
        /// Display debug information on screen
        /// </summary>
        private void DisplayDebugInfo()
        {
            // This would typically be done with a UI system, but here's console output
            // In a real game, you'd update UI Text elements
        }

        // ISpellCaster implementation
        public List<SpellTemplate> GetAvailableSpells()
        {
            return spellbook != null ? spellbook.Spells : new List<SpellTemplate>();
        }

        public List<SpellTemplate> GetActiveSpells()
        {
            return spellbook != null ? spellbook.ActiveSpells : new List<SpellTemplate>();
        }

        public bool HasEnoughMana(float cost)
        {
            return currentMana >= cost;
        }

        public bool ConsumeMana(float cost)
        {
            if (HasEnoughMana(cost))
            {
                currentMana -= cost;
                return true;
            }
            return false;
        }

        public float GetCurrentMana()
        {
            return currentMana;
        }

        public float GetMaxMana()
        {
            return maxMana;
        }

        public int GetLevel()
        {
            return playerLevel;
        }

        // Editor helper
        private void OnDrawGizmos()
        {
            // Draw cast point
            Gizmos.color = Color.cyan;
            Vector3 castPos = transform.position + transform.TransformDirection(castPointOffset);
            Gizmos.DrawWireSphere(castPos, 0.1f);
        }
    }
}

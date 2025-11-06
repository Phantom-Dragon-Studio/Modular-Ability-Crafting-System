using ModularAbilityCraftingSystem.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModularAbilityCraftingSystem.Examples.UI
{
    /// <summary>
    /// UI Toolkit controller for spell hotbar.
    /// Uses modern UI Toolkit (not legacy UI).
    /// Requires: UI Document component with SpellHotbarUI.uxml
    /// </summary>
    [RequireComponent(typeof(UIDocument))]
    public class SpellHotbarUIController : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Player spell caster")]
        [SerializeField] private ExampleSpellCasterPlayer player;

        [Header("Settings")]
        [Tooltip("Number of spell slots to display")]
        [SerializeField] private int slotCount = 6;

        // UI Elements
        private UIDocument uiDocument;
        private VisualElement root;
        private VisualElement[] slotElements;
        private VisualElement[] iconElements;
        private Label[] cooldownLabels;
        private ProgressBar[] cooldownBars;
        private ProgressBar manaBar;
        private Label manaText;

        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;

            // Initialize arrays
            slotElements = new VisualElement[slotCount];
            iconElements = new VisualElement[slotCount];
            cooldownLabels = new Label[slotCount];
            cooldownBars = new ProgressBar[slotCount];

            // Get UI elements
            for (int i = 0; i < slotCount; i++)
            {
                slotElements[i] = root.Q<VisualElement>($"Slot{i}");
                iconElements[i] = root.Q<VisualElement>($"Icon{i}");
                cooldownLabels[i] = root.Q<Label>($"Cooldown{i}");
                cooldownBars[i] = root.Q<ProgressBar>($"CooldownBar{i}");

                // Setup click handlers
                int slotIndex = i; // Capture for lambda
                slotElements[i]?.RegisterCallback<ClickEvent>(evt => OnSlotClicked(slotIndex));
            }

            // Get mana elements
            manaBar = root.Q<ProgressBar>("ManaBar");
            manaText = root.Q<Label>("ManaText");

            // Auto-find player if not set
            if (player == null)
            {
                player = FindObjectOfType<ExampleSpellCasterPlayer>();
            }
        }

        private void Update()
        {
            UpdateUI();
        }

        /// <summary>
        /// Update all UI elements
        /// </summary>
        private void UpdateUI()
        {
            if (player == null) return;

            UpdateSpellSlots();
            UpdateManaBar();
        }

        /// <summary>
        /// Update spell slot displays
        /// </summary>
        private void UpdateSpellSlots()
        {
            for (int i = 0; i < slotCount; i++)
            {
                AbilitySocket socket = player.GetSocket(i);
                if (socket == null || socket.IsEmpty)
                {
                    // Empty slot
                    UpdateEmptySlot(i);
                }
                else
                {
                    // Filled slot
                    UpdateFilledSlot(i, socket);
                }
            }
        }

        /// <summary>
        /// Update empty slot display
        /// </summary>
        private void UpdateEmptySlot(int slotIndex)
        {
            if (iconElements[slotIndex] != null)
            {
                iconElements[slotIndex].style.backgroundImage = null;
            }

            if (cooldownLabels[slotIndex] != null)
            {
                cooldownLabels[slotIndex].text = "";
            }

            if (cooldownBars[slotIndex] != null)
            {
                cooldownBars[slotIndex].value = 0;
                cooldownBars[slotIndex].style.display = DisplayStyle.None;
            }

            // Remove state classes
            slotElements[slotIndex]?.RemoveFromClassList("spell-slot--on-cooldown");
            slotElements[slotIndex]?.RemoveFromClassList("spell-slot--no-mana");
            slotElements[slotIndex]?.RemoveFromClassList("spell-slot--ready");
        }

        /// <summary>
        /// Update filled slot display
        /// </summary>
        private void UpdateFilledSlot(int slotIndex, AbilitySocket socket)
        {
            var spell = socket.AssignedSpell;
            var castable = socket.GetCastableSpell();

            // Update icon
            if (iconElements[slotIndex] != null && spell.SpellIcon != null)
            {
                iconElements[slotIndex].style.backgroundImage = new StyleBackground(spell.SpellIcon);
            }

            // Update cooldown
            if (castable != null && castable.IsOnCooldown())
            {
                float remaining = castable.GetRemainingCooldown();
                float total = castable.GetCooldown();

                if (cooldownLabels[slotIndex] != null)
                {
                    cooldownLabels[slotIndex].text = $"{remaining:F1}s";
                }

                if (cooldownBars[slotIndex] != null)
                {
                    cooldownBars[slotIndex].value = (1f - (remaining / total)) * 100f;
                    cooldownBars[slotIndex].style.display = DisplayStyle.Flex;
                }

                // Add cooldown class
                slotElements[slotIndex]?.AddToClassList("spell-slot--on-cooldown");
                slotElements[slotIndex]?.RemoveFromClassList("spell-slot--ready");
            }
            else
            {
                // Not on cooldown
                if (cooldownLabels[slotIndex] != null)
                {
                    cooldownLabels[slotIndex].text = "";
                }

                if (cooldownBars[slotIndex] != null)
                {
                    cooldownBars[slotIndex].style.display = DisplayStyle.None;
                }

                slotElements[slotIndex]?.RemoveFromClassList("spell-slot--on-cooldown");
            }

            // Check mana
            if (castable != null && player != null)
            {
                bool hasEnoughMana = player.HasEnoughMana(castable.GetManaCost());

                if (hasEnoughMana && socket.CanCast())
                {
                    slotElements[slotIndex]?.AddToClassList("spell-slot--ready");
                    slotElements[slotIndex]?.RemoveFromClassList("spell-slot--no-mana");
                }
                else if (!hasEnoughMana)
                {
                    slotElements[slotIndex]?.AddToClassList("spell-slot--no-mana");
                    slotElements[slotIndex]?.RemoveFromClassList("spell-slot--ready");
                }
            }
        }

        /// <summary>
        /// Update mana bar
        /// </summary>
        private void UpdateManaBar()
        {
            if (player == null) return;

            float current = player.CurrentMana;
            float max = player.MaxMana;
            float percentage = (current / max) * 100f;

            if (manaBar != null)
            {
                manaBar.value = percentage;
            }

            if (manaText != null)
            {
                manaText.text = $"{current:F0} / {max:F0}";
            }
        }

        /// <summary>
        /// Handle slot click
        /// </summary>
        private void OnSlotClicked(int slotIndex)
        {
            if (player != null)
            {
                player.CastSpellFromSlot(slotIndex);
            }
        }
    }

    /// <summary>
    /// Extension methods for ExampleSpellCasterPlayer to support UI
    /// </summary>
    public static class SpellCasterUIExtensions
    {
        public static AbilitySocket GetSocket(this ExampleSpellCasterPlayer player, int index)
        {
            // Use reflection to get private spellSlots field
            var field = typeof(ExampleSpellCasterPlayer).GetField("spellSlots",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                var slots = field.GetValue(player) as AbilitySocket[];
                if (slots != null && index >= 0 && index < slots.Length)
                {
                    return slots[index];
                }
            }
            return null;
        }
    }
}

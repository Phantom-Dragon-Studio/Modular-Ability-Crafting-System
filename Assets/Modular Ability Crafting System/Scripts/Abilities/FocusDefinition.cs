using ModularAbilityCraftingSystem.Abilities.Utilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Defines the base behavior and properties of a spell Focus.
    /// Each Focus type (Immedi, Actus, Creo, etc.) has different base properties.
    /// </summary>
    [CreateAssetMenu(fileName = "New Focus Definition", menuName = "Phantom Dragon Studio/Modular Ability Crafting System/Focus Definition", order = 1)]
    public class FocusDefinition : ScriptableObject
    {
        [Header("Focus Identity")]
        [Tooltip("The type of focus this represents")]
        [SerializeField] private FocusType focusType;

        [Tooltip("Display name for UI")]
        [SerializeField] private string displayName;

        [Tooltip("Description of what this focus does")]
        [TextArea(3, 6)]
        [SerializeField] private string description;

        [Tooltip("Icon for UI representation")]
        [SerializeField] private Sprite icon;

        [Header("Base Prefab")]
        [Tooltip("The base prefab spawned when this focus is used (can be null for instant effects)")]
        [SerializeField] private GameObject basePrefab;

        [Header("Activation Properties")]
        [Tooltip("How this spell is activated")]
        [SerializeField] private ActivationType activationType = ActivationType.Instant;

        [Tooltip("How this spell targets")]
        [SerializeField] private TargetingType targetingType = TargetingType.Point;

        [Header("Base Stats")]
        [Tooltip("Base duration in seconds (0 = instant)")]
        [SerializeField] private float baseDuration = 0f;

        [Tooltip("Base cooldown in seconds")]
        [SerializeField] private float baseCooldown = 1f;

        [Tooltip("Base range in units")]
        [SerializeField] private float baseRange = 10f;

        [Tooltip("Base speed (for projectiles/moving effects)")]
        [SerializeField] private float baseSpeed = 10f;

        [Tooltip("Base mana cost")]
        [SerializeField] private float baseManaCost = 10f;

        [Header("Visual Feedback")]
        [Tooltip("Color tint for this focus (blended with essence color)")]
        [SerializeField] private Color focusColor = Color.white;

        [Tooltip("VFX played on spell cast")]
        [SerializeField] private GameObject castVFX;

        // Public Properties
        public FocusType FocusType => focusType;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public GameObject BasePrefab => basePrefab;
        public ActivationType ActivationType => activationType;
        public TargetingType TargetingType => targetingType;
        public float BaseDuration => baseDuration;
        public float BaseCooldown => baseCooldown;
        public float BaseRange => baseRange;
        public float BaseSpeed => baseSpeed;
        public float BaseManaCost => baseManaCost;
        public Color FocusColor => focusColor;
        public GameObject CastVFX => castVFX;
    }
}

using UnityEngine;

namespace ModularAbilityCraftingSystem.StatusEffects
{
    /// <summary>
    /// Defines the properties of a status effect.
    /// ScriptableObject for designer configuration.
    /// </summary>
    [CreateAssetMenu(fileName = "New Status Effect", menuName = "Phantom Dragon Studio/MACS/Status Effect", order = 20)]
    public class StatusEffectDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("The type of status effect")]
        [SerializeField] private StatusEffectType effectType;

        [Tooltip("Display name")]
        [SerializeField] private string effectName;

        [Tooltip("Description")]
        [TextArea(2, 4)]
        [SerializeField] private string description;

        [Tooltip("Icon for UI")]
        [SerializeField] private Sprite icon;

        [Header("Properties")]
        [Tooltip("Base duration in seconds")]
        [SerializeField] private float baseDuration = 3f;

        [Tooltip("Can this effect stack multiple times?")]
        [SerializeField] private bool canStack = false;

        [Tooltip("Maximum stacks if stacking is enabled")]
        [SerializeField] private int maxStacks = 3;

        [Tooltip("Refresh duration on reapplication?")]
        [SerializeField] private bool refreshDuration = true;

        [Header("Damage/Healing")]
        [Tooltip("Damage/healing per tick (if applicable)")]
        [SerializeField] private float damagePerTick = 5f;

        [Tooltip("Time between ticks in seconds")]
        [SerializeField] private float tickInterval = 1f;

        [Header("Movement")]
        [Tooltip("Movement speed multiplier (1 = normal, 0.5 = half speed, 2 = double speed)")]
        [SerializeField] private float movementSpeedMultiplier = 1f;

        [Tooltip("Knockback force (for knockback effects)")]
        [SerializeField] private float knockbackForce = 10f;

        [Header("Visual Effects")]
        [Tooltip("VFX prefab to spawn on affected target")]
        [SerializeField] private GameObject vfxPrefab;

        [Tooltip("Color tint to apply to target")]
        [SerializeField] private Color tintColor = Color.white;

        [Tooltip("Audio clip to play when effect is applied")]
        [SerializeField] private AudioClip applySound;

        [Tooltip("Audio clip to play on each tick")]
        [SerializeField] private AudioClip tickSound;

        // Public Properties
        public StatusEffectType EffectType => effectType;
        public string EffectName => effectName;
        public string Description => description;
        public Sprite Icon => icon;
        public float BaseDuration => baseDuration;
        public bool CanStack => canStack;
        public int MaxStacks => maxStacks;
        public bool RefreshDuration => refreshDuration;
        public float DamagePerTick => damagePerTick;
        public float TickInterval => tickInterval;
        public float MovementSpeedMultiplier => movementSpeedMultiplier;
        public float KnockbackForce => knockbackForce;
        public GameObject VfxPrefab => vfxPrefab;
        public Color TintColor => tintColor;
        public AudioClip ApplySound => applySound;
        public AudioClip TickSound => tickSound;
    }
}

using ModularAbilityCraftingSystem.Abilities.Utilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Defines the elemental properties and visual effects of a spell Essence.
    /// Each Essence type (Igni, Aqua, Aura, etc.) has unique properties and effects.
    /// </summary>
    [CreateAssetMenu(fileName = "New Essence Definition", menuName = "Phantom Dragon Studio/Modular Ability Crafting System/Essence Definition", order = 2)]
    public class EssenceDefinition : ScriptableObject
    {
        [Header("Essence Identity")]
        [Tooltip("The type of essence this represents")]
        [SerializeField] private EssenceType essenceType;

        [Tooltip("Display name for UI")]
        [SerializeField] private string displayName;

        [Tooltip("Description of what this essence does")]
        [TextArea(3, 6)]
        [SerializeField] private string description;

        [Tooltip("Icon for UI representation")]
        [SerializeField] private Sprite icon;

        [Header("Damage Properties")]
        [Tooltip("Base damage multiplier for this element")]
        [SerializeField] private float damageMultiplier = 1f;

        [Tooltip("Damage type identifier (for resistances/weaknesses)")]
        [SerializeField] private string damageType;

        [Header("Status Effects")]
        [Tooltip("Can this essence apply status effects?")]
        [SerializeField] private bool canApplyStatus = false;

        [Tooltip("Status effect to apply (if enabled)")]
        [SerializeField] private string statusEffectId;

        [Tooltip("Chance to apply status effect (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float statusChance = 0.3f;

        [Tooltip("Duration of status effect in seconds")]
        [SerializeField] private float statusDuration = 3f;

        [Header("Visual Effects")]
        [Tooltip("Primary color of this essence")]
        [SerializeField] private Color primaryColor = Color.white;

        [Tooltip("Secondary color for gradients/effects")]
        [SerializeField] private Color secondaryColor = Color.white;

        [Tooltip("Material to apply to spell visuals")]
        [SerializeField] private Material elementMaterial;

        [Tooltip("Particle system for continuous effects")]
        [SerializeField] private GameObject particleVFX;

        [Tooltip("VFX played on spell impact")]
        [SerializeField] private GameObject impactVFX;

        [Tooltip("Trail VFX for projectiles")]
        [SerializeField] private GameObject trailVFX;

        [Header("Audio")]
        [Tooltip("Sound played on spell cast")]
        [SerializeField] private AudioClip castSound;

        [Tooltip("Sound played on spell impact/hit")]
        [SerializeField] private AudioClip impactSound;

        [Tooltip("Looping ambient sound for continuous effects")]
        [SerializeField] private AudioClip ambientSound;

        [Header("Special Properties")]
        [Tooltip("Custom properties specific to this essence (e.g., 'ignites terrain', 'freezes water')")]
        [SerializeField] private string[] specialProperties;

        // Public Properties
        public EssenceType EssenceType => essenceType;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;
        public float DamageMultiplier => damageMultiplier;
        public string DamageType => damageType;
        public bool CanApplyStatus => canApplyStatus;
        public string StatusEffectId => statusEffectId;
        public float StatusChance => statusChance;
        public float StatusDuration => statusDuration;
        public Color PrimaryColor => primaryColor;
        public Color SecondaryColor => secondaryColor;
        public Material ElementMaterial => elementMaterial;
        public GameObject ParticleVFX => particleVFX;
        public GameObject ImpactVFX => impactVFX;
        public GameObject TrailVFX => trailVFX;
        public AudioClip CastSound => castSound;
        public AudioClip ImpactSound => impactSound;
        public AudioClip AmbientSound => ambientSound;
        public string[] SpecialProperties => specialProperties;
    }
}

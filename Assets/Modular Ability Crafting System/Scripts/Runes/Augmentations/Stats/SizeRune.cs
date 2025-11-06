using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Augmentations.Stats
{
    /// <summary>
    /// Increases or decreases the size of the spell.
    /// Affects visual scale, hitbox, and potentially damage/area.
    /// </summary>
    [CreateAssetMenu(fileName = "Size Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Augmentations/Size", order = 300)]
    public class SizeRune : BaseAugmentation
    {
        [Header("Size Modification")]
        [Tooltip("Size multiplier (1 = normal, 2 = double size, 0.5 = half size)")]
        [SerializeField] private float sizeMultiplier = 1.5f;

        [Tooltip("Should size affect damage?")]
        [SerializeField] private bool affectsDamage = true;

        [Tooltip("Damage scaling with size (if enabled)")]
        [Range(0f, 2f)]
        [SerializeField] private float damageScaling = 0.5f; // 50% of size increase affects damage

        [Tooltip("Should size affect mana cost?")]
        [SerializeField] private bool affectsManaCost = true;

        private void OnEnable()
        {
            category = RuneCategory.Augmentation;
            isAdditive = false; // Size multiplies
            canStackWithSelf = true;
            allowMultipleInstances = true;
            maxInstances = 3;
            manaCostMultiplier = affectsManaCost ? sizeMultiplier : 1f;
        }

        protected override void ApplyStatModifications(SpellContext context)
        {
            // Apply size multiplier
            context.Size *= sizeMultiplier;

            // Optionally affect damage
            if (affectsDamage)
            {
                float damageModifier = 1f + ((sizeMultiplier - 1f) * damageScaling);
                context.Damage *= damageModifier;
            }
        }

        public override void ApplyToInstance(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance != null)
            {
                // Apply visual scale
                spellInstance.transform.localScale *= sizeMultiplier;

                // Scale colliders if present
                foreach (var collider in spellInstance.GetComponents<Collider>())
                {
                    if (collider is SphereCollider sphere)
                    {
                        sphere.radius *= sizeMultiplier;
                    }
                    else if (collider is BoxCollider box)
                    {
                        box.size *= sizeMultiplier;
                    }
                    else if (collider is CapsuleCollider capsule)
                    {
                        capsule.radius *= sizeMultiplier;
                        capsule.height *= sizeMultiplier;
                    }
                }
            }

            base.ApplyToInstance(context, spellInstance);
        }
    }
}

using System;
using System.Collections;
using UnityEngine;

namespace ModularAbilityCraftingSystem.StatusEffects
{
    /// <summary>
    /// Runtime instance of a status effect applied to an entity.
    /// Handles ticking, duration, and effect logic.
    /// </summary>
    public class StatusEffectInstance
    {
        private StatusEffectDefinition definition;
        private GameObject affectedTarget;
        private Transform casterTransform;
        private float remainingDuration;
        private int currentStacks;
        private float nextTickTime;
        private GameObject vfxInstance;

        public StatusEffectDefinition Definition => definition;
        public GameObject AffectedTarget => affectedTarget;
        public float RemainingDuration => remainingDuration;
        public int CurrentStacks => currentStacks;
        public bool IsExpired => remainingDuration <= 0;

        public StatusEffectInstance(StatusEffectDefinition def, GameObject target, Transform caster, int stacks = 1)
        {
            definition = def;
            affectedTarget = target;
            casterTransform = caster;
            remainingDuration = def.BaseDuration;
            currentStacks = Mathf.Clamp(stacks, 1, def.MaxStacks);
            nextTickTime = Time.time + def.TickInterval;
        }

        /// <summary>
        /// Apply the effect (called when first applied)
        /// </summary>
        public void Apply()
        {
            // Spawn VFX
            if (definition.VfxPrefab != null && affectedTarget != null)
            {
                vfxInstance = UnityEngine.Object.Instantiate(definition.VfxPrefab, affectedTarget.transform);
                vfxInstance.transform.localPosition = Vector3.zero;
            }

            // Play sound
            if (definition.ApplySound != null && affectedTarget != null)
            {
                AudioSource.PlayClipAtPoint(definition.ApplySound, affectedTarget.transform.position);
            }

            // Apply immediate effects based on type
            ApplyImmediateEffect();
        }

        /// <summary>
        /// Update the effect (called each frame)
        /// </summary>
        public void Update(float deltaTime)
        {
            remainingDuration -= deltaTime;

            // Handle ticking effects
            if (Time.time >= nextTickTime)
            {
                Tick();
                nextTickTime = Time.time + definition.TickInterval;
            }
        }

        /// <summary>
        /// Called on each tick interval
        /// </summary>
        private void Tick()
        {
            if (affectedTarget == null) return;

            // Apply tick damage/healing
            if (definition.DamagePerTick != 0)
            {
                var damageable = affectedTarget.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    float totalDamage = definition.DamagePerTick * currentStacks;
                    damageable.TakeDamage(totalDamage, definition.EffectType.ToString());
                }
            }

            // Play tick sound
            if (definition.TickSound != null)
            {
                AudioSource.PlayClipAtPoint(definition.TickSound, affectedTarget.transform.position);
            }
        }

        /// <summary>
        /// Apply immediate effect based on type
        /// </summary>
        private void ApplyImmediateEffect()
        {
            if (affectedTarget == null) return;

            switch (definition.EffectType)
            {
                case StatusEffectType.Knockback:
                    ApplyKnockback();
                    break;

                case StatusEffectType.Freeze:
                case StatusEffectType.Stun:
                case StatusEffectType.Root:
                    DisableMovement();
                    break;

                case StatusEffectType.Slow:
                    ModifyMovementSpeed();
                    break;
            }
        }

        /// <summary>
        /// Apply knockback force
        /// </summary>
        private void ApplyKnockback()
        {
            Rigidbody rb = affectedTarget.GetComponent<Rigidbody>();
            if (rb != null && casterTransform != null)
            {
                Vector3 direction = (affectedTarget.transform.position - casterTransform.position).normalized;
                rb.AddForce(direction * definition.KnockbackForce, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Disable movement (freeze, stun, root)
        /// </summary>
        private void DisableMovement()
        {
            // This would interface with your movement system
            var movementController = affectedTarget.GetComponent<IMovementController>();
            if (movementController != null)
            {
                movementController.SetMovementEnabled(false);
            }
        }

        /// <summary>
        /// Modify movement speed
        /// </summary>
        private void ModifyMovementSpeed()
        {
            var movementController = affectedTarget.GetComponent<IMovementController>();
            if (movementController != null)
            {
                movementController.SetSpeedMultiplier(definition.MovementSpeedMultiplier);
            }
        }

        /// <summary>
        /// Add stacks to this effect
        /// </summary>
        public bool AddStack(int amount = 1)
        {
            if (!definition.CanStack) return false;

            int newStacks = currentStacks + amount;
            if (newStacks > definition.MaxStacks)
            {
                currentStacks = definition.MaxStacks;
                return false;
            }

            currentStacks = newStacks;

            // Refresh duration if configured
            if (definition.RefreshDuration)
            {
                remainingDuration = definition.BaseDuration;
            }

            return true;
        }

        /// <summary>
        /// Remove the effect
        /// </summary>
        public void Remove()
        {
            // Clean up VFX
            if (vfxInstance != null)
            {
                UnityEngine.Object.Destroy(vfxInstance);
            }

            // Restore movement if it was disabled
            if (definition.EffectType == StatusEffectType.Freeze ||
                definition.EffectType == StatusEffectType.Stun ||
                definition.EffectType == StatusEffectType.Root)
            {
                var movementController = affectedTarget.GetComponent<IMovementController>();
                if (movementController != null)
                {
                    movementController.SetMovementEnabled(true);
                }
            }

            // Restore movement speed
            if (definition.EffectType == StatusEffectType.Slow)
            {
                var movementController = affectedTarget.GetComponent<IMovementController>();
                if (movementController != null)
                {
                    movementController.SetSpeedMultiplier(1f);
                }
            }
        }
    }

    /// <summary>
    /// Interface for entities that can take damage
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount, string damageType);
        void Heal(float amount);
        float GetCurrentHealth();
        float GetMaxHealth();
        bool IsAlive();
    }

    /// <summary>
    /// Interface for movement control
    /// </summary>
    public interface IMovementController
    {
        void SetMovementEnabled(bool enabled);
        void SetSpeedMultiplier(float multiplier);
        float GetCurrentSpeed();
    }
}

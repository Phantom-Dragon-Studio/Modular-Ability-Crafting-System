using ModularAbilityCraftingSystem.StatusEffects;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Examples
{
    /// <summary>
    /// Example target that can receive damage and status effects.
    /// Attach this to enemies or destructible objects.
    /// </summary>
    [RequireComponent(typeof(StatusEffectManager))]
    public class ExampleDamageableTarget : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [Tooltip("Current health")]
        [SerializeField] private float currentHealth = 100f;

        [Tooltip("Maximum health")]
        [SerializeField] private float maxHealth = 100f;

        [Header("Visual Feedback")]
        [Tooltip("Material to flash on damage")]
        [SerializeField] private Material damageMaterial;

        [Tooltip("Flash duration")]
        [SerializeField] private float flashDuration = 0.1f;

        [Header("Destruction")]
        [Tooltip("Destroy on death?")]
        [SerializeField] private bool destroyOnDeath = true;

        [Tooltip("Death VFX prefab")]
        [SerializeField] private GameObject deathVFX;

        [Tooltip("Death sound")]
        [SerializeField] private AudioClip deathSound;

        [Header("Debug")]
        [Tooltip("Show damage numbers")]
        [SerializeField] private bool showDamageNumbers = true;

        // References
        private StatusEffectManager statusEffectManager;
        private Renderer meshRenderer;
        private Material originalMaterial;

        private void Awake()
        {
            statusEffectManager = GetComponent<StatusEffectManager>();
            meshRenderer = GetComponent<Renderer>();

            if (meshRenderer != null)
            {
                originalMaterial = meshRenderer.material;
            }
        }

        public void TakeDamage(float amount, string damageType)
        {
            if (!IsAlive()) return;

            currentHealth -= amount;

            if (showDamageNumbers)
            {
                Debug.Log($"{gameObject.name} took {amount:F1} {damageType} damage. Health: {currentHealth:F1}/{maxHealth:F1}");
            }

            // Visual feedback
            if (meshRenderer != null && damageMaterial != null)
            {
                StartCoroutine(FlashDamage());
            }

            // Check death
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive()) return;

            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);

            if (showDamageNumbers)
            {
                Debug.Log($"{gameObject.name} healed for {amount:F1}. Health: {currentHealth:F1}/{maxHealth:F1}");
            }
        }

        public float GetCurrentHealth()
        {
            return currentHealth;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public bool IsAlive()
        {
            return currentHealth > 0;
        }

        /// <summary>
        /// Handle death
        /// </summary>
        private void Die()
        {
            if (showDamageNumbers)
            {
                Debug.Log($"{gameObject.name} died");
            }

            // Spawn death VFX
            if (deathVFX != null)
            {
                Instantiate(deathVFX, transform.position, Quaternion.identity);
            }

            // Play death sound
            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }

            // Destroy or disable
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Flash material on damage
        /// </summary>
        private System.Collections.IEnumerator FlashDamage()
        {
            if (meshRenderer != null && damageMaterial != null)
            {
                meshRenderer.material = damageMaterial;
                yield return new WaitForSeconds(flashDuration);
                meshRenderer.material = originalMaterial;
            }
        }

        // Editor helper
        private void OnDrawGizmos()
        {
            // Draw health bar
            Gizmos.color = Color.red;
            Vector3 healthBarPos = transform.position + Vector3.up * 2f;
            float healthPercent = currentHealth / maxHealth;
            Gizmos.DrawLine(healthBarPos, healthBarPos + Vector3.right * healthPercent);
        }
    }
}

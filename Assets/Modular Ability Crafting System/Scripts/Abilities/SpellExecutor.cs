using ModularAbilityCraftingSystem.Runes.Behaviours;
using ModularAbilityCraftingSystem.Runes.Behaviours.Multiplication;
using ModularAbilityCraftingSystem.Runes.Triggers;
using System.Collections.Generic;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Abilities
{
    /// <summary>
    /// Core system responsible for executing spells at runtime.
    /// Handles instantiation, rune application, and spell lifecycle.
    /// </summary>
    public class SpellExecutor : MonoBehaviour
    {
        [Header("Default Prefabs")]
        [Tooltip("Default projectile prefab if Focus doesn't specify one")]
        [SerializeField] private GameObject defaultProjectilePrefab;

        [Tooltip("Default effect prefab for non-projectile spells")]
        [SerializeField] private GameObject defaultEffectPrefab;

        [Header("Pooling Settings")]
        [Tooltip("Use object pooling for performance?")]
        [SerializeField] private bool usePooling = true;

        [Tooltip("Initial pool size per spell type")]
        [SerializeField] private int poolSize = 10;

        // Object pools (spell ID -> pool)
        private Dictionary<string, Queue<GameObject>> spellPools = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// Cast a spell from a template at a specific position
        /// </summary>
        public GameObject CastSpell(SpellTemplate template, Vector3 position, Vector3 direction, Transform caster = null, Transform target = null)
        {
            if (template == null)
            {
                Debug.LogError("Cannot cast null spell template");
                return null;
            }

            if (template.Focus == null)
            {
                Debug.LogError($"Spell '{template.SpellName}' has no Focus");
                return null;
            }

            // Build context from template
            SpellContext context = template.BuildContext();
            context.CastPosition = position;
            context.Direction = direction.normalized;
            context.CasterTransform = caster;
            context.TargetTransform = target;
            context.TargetPosition = target != null ? target.position : position + direction * 100f;

            // Handle duplication (multiple projectiles)
            if (context.ProjectileCount > 1)
            {
                return CastMultipleProjectiles(template, context);
            }
            else
            {
                return CastSingleSpell(template, context);
            }
        }

        /// <summary>
        /// Cast spell with custom direction (overload)
        /// </summary>
        public GameObject CastSpell(SpellTemplate template, Vector3 position, Vector3 direction)
        {
            return CastSpell(template, position, direction, null, null);
        }

        /// <summary>
        /// Cast a single spell instance
        /// </summary>
        private GameObject CastSingleSpell(SpellTemplate template, SpellContext context)
        {
            // Get or create spell instance
            GameObject spellInstance = GetSpellInstance(template, context);
            if (spellInstance == null) return null;

            // Position and orient
            spellInstance.transform.position = context.CastPosition;
            spellInstance.transform.rotation = Quaternion.LookRotation(context.Direction);

            // Apply essence visuals
            ApplyEssenceVisuals(spellInstance, context);

            // Apply all behaviours
            var behaviours = template.GetBehaviours();
            foreach (var behaviour in behaviours)
            {
                if (behaviour != null)
                {
                    behaviour.AttachBehaviour(context, spellInstance);
                    behaviour.ApplyToInstance(context, spellInstance);
                }
            }

            // Apply all triggers
            var triggers = template.GetTriggers();
            foreach (var trigger in triggers)
            {
                if (trigger != null)
                {
                    trigger.AttachTrigger(context, spellInstance);
                }
            }

            // Apply all augmentations
            var augmentations = template.GetAugmentations();
            foreach (var augmentation in augmentations)
            {
                if (augmentation != null)
                {
                    augmentation.ApplyToInstance(context, spellInstance);
                }
            }

            // Play cast VFX/sound
            PlayCastEffects(template, context);

            // Activate the spell
            spellInstance.SetActive(true);

            return spellInstance;
        }

        /// <summary>
        /// Cast multiple projectiles (for Duplicate rune)
        /// </summary>
        private GameObject CastMultipleProjectiles(SpellTemplate template, SpellContext context)
        {
            // Get spread pattern from context
            DuplicateRune.SpreadPattern pattern = context.GetCustomData("SpreadPattern", DuplicateRune.SpreadPattern.Arc);

            // Find the duplicate rune to get spawn directions
            DuplicateRune duplicateRune = null;
            foreach (var behaviour in template.GetBehaviours())
            {
                if (behaviour is DuplicateRune dup)
                {
                    duplicateRune = dup;
                    break;
                }
            }

            // Calculate spawn directions
            Vector3[] directions = duplicateRune != null
                ? duplicateRune.GetSpawnDirections(context.Direction, context.ProjectileCount)
                : GetDefaultSpawnDirections(context.Direction, context.ProjectileCount, context.SpreadAngle);

            GameObject firstInstance = null;

            // Spawn each projectile
            for (int i = 0; i < context.ProjectileCount; i++)
            {
                // Create a context copy for each projectile
                SpellContext instanceContext = new SpellContext(context.Focus, context.Essence)
                {
                    CastPosition = context.CastPosition,
                    Direction = directions[i],
                    CasterTransform = context.CasterTransform,
                    TargetTransform = context.TargetTransform,
                    TargetPosition = context.TargetPosition,
                    Damage = context.Damage,
                    Speed = context.Speed,
                    Range = context.Range,
                    Duration = context.Duration,
                    Size = context.Size
                };

                GameObject instance = CastSingleSpell(template, instanceContext);
                if (i == 0) firstInstance = instance;
            }

            return firstInstance;
        }

        /// <summary>
        /// Get default spawn directions if no DuplicateRune is found
        /// </summary>
        private Vector3[] GetDefaultSpawnDirections(Vector3 forward, int count, float spreadAngle)
        {
            Vector3[] directions = new Vector3[count];
            for (int i = 0; i < count; i++)
            {
                float angle = spreadAngle * ((i / (float)(count - 1)) - 0.5f);
                directions[i] = Quaternion.Euler(0, angle, 0) * forward;
            }
            return directions;
        }

        /// <summary>
        /// Get a spell instance (from pool or create new)
        /// </summary>
        private GameObject GetSpellInstance(SpellTemplate template, SpellContext context)
        {
            GameObject prefab = context.Focus.BasePrefab;
            if (prefab == null)
            {
                // Use default based on focus type
                prefab = context.Focus.ActivationType == Utilities.ActivationType.Instant
                    ? defaultEffectPrefab
                    : defaultProjectilePrefab;
            }

            if (prefab == null)
            {
                Debug.LogError($"No prefab available for spell '{template.SpellName}'");
                return null;
            }

            GameObject instance;

            // Try to get from pool
            if (usePooling && spellPools.TryGetValue(template.SpellId, out Queue<GameObject> pool) && pool.Count > 0)
            {
                instance = pool.Dequeue();
                instance.SetActive(false);

                // Reset instance
                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one;
            }
            else
            {
                // Create new instance
                instance = Instantiate(prefab);
                instance.name = $"{template.SpellName} (Spell Instance)";
            }

            return instance;
        }

        /// <summary>
        /// Apply essence visual effects to spell instance
        /// </summary>
        private void ApplyEssenceVisuals(GameObject spellInstance, SpellContext context)
        {
            if (context.Essence == null) return;

            // Apply material if available
            if (context.Essence.ElementMaterial != null)
            {
                Renderer renderer = spellInstance.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = context.Essence.ElementMaterial;
                }
            }

            // Apply color tint
            Renderer[] renderers = spellInstance.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.color = context.Essence.PrimaryColor;
            }

            // Spawn particle VFX if available
            if (context.Essence.ParticleVFX != null)
            {
                GameObject vfx = Instantiate(context.Essence.ParticleVFX, spellInstance.transform);
                vfx.transform.localPosition = Vector3.zero;
            }

            // Add trail if available
            if (context.Essence.TrailVFX != null)
            {
                GameObject trail = Instantiate(context.Essence.TrailVFX, spellInstance.transform);
                trail.transform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// Play cast effects (VFX and sound)
        /// </summary>
        private void PlayCastEffects(SpellTemplate template, SpellContext context)
        {
            // Play focus cast VFX
            if (context.Focus != null && context.Focus.CastVFX != null)
            {
                Instantiate(context.Focus.CastVFX, context.CastPosition, Quaternion.identity);
            }

            // Play essence cast sound
            if (context.Essence != null && context.Essence.CastSound != null)
            {
                AudioSource.PlayClipAtPoint(context.Essence.CastSound, context.CastPosition);
            }
        }

        /// <summary>
        /// Return a spell instance to the pool
        /// </summary>
        public void ReturnToPool(string spellId, GameObject instance)
        {
            if (!usePooling)
            {
                Destroy(instance);
                return;
            }

            if (!spellPools.ContainsKey(spellId))
            {
                spellPools[spellId] = new Queue<GameObject>();
            }

            instance.SetActive(false);
            spellPools[spellId].Enqueue(instance);
        }

        /// <summary>
        /// Pre-populate pool for a spell
        /// </summary>
        public void WarmupPool(SpellTemplate template)
        {
            if (!usePooling || template == null) return;

            if (!spellPools.ContainsKey(template.SpellId))
            {
                spellPools[template.SpellId] = new Queue<GameObject>();
            }

            SpellContext context = template.BuildContext();
            GameObject prefab = context.Focus.BasePrefab ?? defaultProjectilePrefab;
            if (prefab == null) return;

            for (int i = 0; i < poolSize; i++)
            {
                GameObject instance = Instantiate(prefab);
                instance.SetActive(false);
                instance.transform.SetParent(transform);
                spellPools[template.SpellId].Enqueue(instance);
            }
        }
    }
}

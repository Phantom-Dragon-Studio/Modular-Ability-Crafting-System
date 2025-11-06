using ModularAbilityCraftingSystem.Abilities;
using System.Collections.Generic;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Impact
{
    /// <summary>
    /// Makes spells chain to additional nearby targets.
    /// Perfect for crowd control and multi-target scenarios.
    /// </summary>
    [CreateAssetMenu(fileName = "Chain Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Chain", order = 301)]
    public class ChainRune : BaseBehaviour
    {
        [Header("Chain Settings")]
        [Tooltip("Maximum number of additional targets to chain to")]
        [SerializeField] private int maxChainTargets = 3;

        [Tooltip("Range to search for chain targets")]
        [SerializeField] private float chainRange = 8f;

        [Tooltip("Damage reduction per chain (0.7 = 70% damage on next target)")]
        [Range(0.1f, 1f)]
        [SerializeField] private float damageReductionPerChain = 0.7f;

        [Tooltip("Delay between chains (seconds)")]
        [SerializeField] private float chainDelay = 0.1f;

        [Tooltip("Can chain back to previously hit targets?")]
        [SerializeField] private bool canChainToSameTarget = false;

        [Header("Visual")]
        [Tooltip("Chain lightning VFX")]
        [SerializeField] private GameObject chainVFX;

        private void OnEnable()
        {
            category = RuneCategory.Impact;
            addedTags = new RuneTag[] { RuneTag.Chainable, RuneTag.MultiTarget };
            manaCostModifier = 8f;
            executionPriority = 45;
        }

        public override void ApplyToSpell(SpellContext context)
        {
            context.CanChain = true;
            context.MaxChainTargets = maxChainTargets;
            context.ChainRange = chainRange;

            context.SetCustomData("ChainDamageReduction", damageReductionPerChain);
            context.SetCustomData("ChainDelay", chainDelay);
            context.SetCustomData("CanChainToSame", canChainToSameTarget);
            context.SetCustomData("ChainVFX", chainVFX);

            context.AddTag(RuneTag.Chainable);
            context.AddTag(RuneTag.MultiTarget);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            var chain = spellInstance.AddComponent<ChainBehaviour>();
            chain.Initialize(
                maxChainTargets,
                chainRange,
                damageReductionPerChain,
                chainDelay,
                canChainToSameTarget,
                context.Damage,
                chainVFX,
                context.CasterTransform
            );
        }
    }

    /// <summary>
    /// Runtime component that handles chain logic
    /// </summary>
    public class ChainBehaviour : MonoBehaviour
    {
        private int maxChains;
        private float range;
        private float damageReduction;
        private float delay;
        private bool allowSameTarget;
        private float baseDamage;
        private GameObject chainVFX;
        private Transform caster;
        private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
        private bool hasChained = false;

        public void Initialize(int maxTargets, float chainRange, float dmgReduction, float chainDelay, bool canRepeat, float damage, GameObject vfx, Transform casterTransform)
        {
            maxChains = maxTargets;
            range = chainRange;
            damageReduction = dmgReduction;
            delay = chainDelay;
            allowSameTarget = canRepeat;
            baseDamage = damage;
            chainVFX = vfx;
            caster = casterTransform;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!hasChained)
            {
                hasChained = true;
                hitTargets.Add(collision.gameObject);
                StartChain(collision.gameObject, collision.contacts[0].point, maxChains, baseDamage);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!hasChained && other.transform != caster)
            {
                hasChained = true;
                hitTargets.Add(other.gameObject);
                StartChain(other.gameObject, other.ClosestPoint(transform.position), maxChains, baseDamage);
            }
        }

        private void StartChain(GameObject firstTarget, Vector3 hitPoint, int remainingChains, float currentDamage)
        {
            if (remainingChains <= 0) return;

            // Apply damage to current target
            var damageable = firstTarget.GetComponent<StatusEffects.IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(currentDamage, "Chain");
            }

            // Find next target
            Collider[] nearby = Physics.OverlapSphere(firstTarget.transform.position, range);
            GameObject nextTarget = null;
            float closestDistance = float.MaxValue;

            foreach (var collider in nearby)
            {
                if (collider.gameObject == firstTarget) continue;
                if (collider.transform == caster) continue;
                if (!allowSameTarget && hitTargets.Contains(collider.gameObject)) continue;

                float distance = Vector3.Distance(firstTarget.transform.position, collider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nextTarget = collider.gameObject;
                }
            }

            if (nextTarget != null)
            {
                hitTargets.Add(nextTarget);

                // Spawn chain VFX
                if (chainVFX != null)
                {
                    GameObject vfxInstance = Instantiate(chainVFX, firstTarget.transform.position, Quaternion.identity);
                    LineRenderer line = vfxInstance.GetComponent<LineRenderer>();
                    if (line != null)
                    {
                        line.SetPosition(0, firstTarget.transform.position);
                        line.SetPosition(1, nextTarget.transform.position);
                        Destroy(vfxInstance, 0.5f);
                    }
                }

                // Chain to next target with reduced damage
                float nextDamage = currentDamage * damageReduction;
                Invoke(nameof(ContinueChain), delay);
                cachedNextTarget = nextTarget;
                cachedRemainingChains = remainingChains - 1;
                cachedNextDamage = nextDamage;
            }
        }

        // Cache for Invoke
        private GameObject cachedNextTarget;
        private int cachedRemainingChains;
        private float cachedNextDamage;

        private void ContinueChain()
        {
            if (cachedNextTarget != null)
            {
                StartChain(cachedNextTarget, cachedNextTarget.transform.position, cachedRemainingChains, cachedNextDamage);
            }
        }
    }
}

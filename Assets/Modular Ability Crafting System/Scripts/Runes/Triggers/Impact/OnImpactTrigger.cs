using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Triggers.Impact
{
    /// <summary>
    /// Triggers when the spell hits something.
    /// Can trigger explosions, spell chains, or other effects.
    /// </summary>
    [CreateAssetMenu(fileName = "OnImpact Trigger", menuName = "Phantom Dragon Studio/MACS/Runes/Triggers/OnImpact", order = 400)]
    public class OnImpactTrigger : BaseTrigger
    {
        [Header("Impact Settings")]
        [Tooltip("What layers trigger the impact?")]
        [SerializeField] private LayerMask triggerLayers = -1;

        [Tooltip("Destroy spell on impact?")]
        [SerializeField] private bool destroyOnImpact = true;

        [Tooltip("Linked spell to cast on impact (for chaining)")]
        [SerializeField] private SpellTemplate linkedSpell;

        private void OnEnable()
        {
            category = RuneCategory.Trigger;
            canRepeat = false; // Usually triggers once
        }

        public override void ApplyToSpell(SpellContext context)
        {
            // Add linked spell if configured
            if (linkedSpell != null)
            {
                context.AddLinkedSpell(linkedSpell);
            }
        }

        public override void AttachTrigger(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            // Add impact trigger component
            var impactComponent = spellInstance.AddComponent<OnImpactTriggerComponent>();
            impactComponent.Initialize(triggerLayers, destroyOnImpact, linkedSpell, context);

            // Ensure collider exists
            if (spellInstance.GetComponent<Collider>() == null)
            {
                SphereCollider collider = spellInstance.AddComponent<SphereCollider>();
                collider.isTrigger = false; // Use collision, not trigger
                collider.radius = 0.5f;
            }
        }
    }

    /// <summary>
    /// Runtime component that handles impact detection
    /// </summary>
    public class OnImpactTriggerComponent : MonoBehaviour
    {
        private LayerMask triggerLayers;
        private bool destroyOnImpact;
        private SpellTemplate linkedSpell;
        private SpellContext spellContext;
        private bool hasTriggered = false;

        public void Initialize(LayerMask layers, bool destroy, SpellTemplate linked, SpellContext context)
        {
            triggerLayers = layers;
            destroyOnImpact = destroy;
            linkedSpell = linked;
            spellContext = context;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasTriggered) return;

            // Check if collision is on a trigger layer
            if (((1 << collision.gameObject.layer) & triggerLayers) != 0)
            {
                hasTriggered = true;

                // Cast linked spell if configured
                if (linkedSpell != null && spellContext != null)
                {
                    // Find SpellExecutor in scene and cast linked spell
                    SpellExecutor executor = FindObjectOfType<SpellExecutor>();
                    if (executor != null)
                    {
                        executor.CastSpell(linkedSpell, collision.contacts[0].point, collision.contacts[0].normal);
                    }
                }

                // Destroy spell if configured
                if (destroyOnImpact)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}

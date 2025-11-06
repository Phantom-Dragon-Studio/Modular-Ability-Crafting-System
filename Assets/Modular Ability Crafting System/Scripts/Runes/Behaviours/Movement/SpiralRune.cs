using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Movement
{
    /// <summary>
    /// Makes projectiles move in a spiral pattern.
    /// Creates unpredictable, visually interesting trajectories.
    /// </summary>
    [CreateAssetMenu(fileName = "Spiral Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Spiral", order = 105)]
    public class SpiralRune : BaseBehaviour
    {
        [Header("Spiral Settings")]
        [Tooltip("Spiral rotation speed (degrees per second)")]
        [SerializeField] private float rotationSpeed = 180f;

        [Tooltip("Spiral radius")]
        [SerializeField] private float spiralRadius = 1f;

        [Tooltip("Expand spiral over time?")]
        [SerializeField] private bool expandSpiral = false;

        [Tooltip("Expansion rate (units per second)")]
        [SerializeField] private float expansionRate = 0.5f;

        private void OnEnable()
        {
            category = RuneCategory.Movement;
            requiredTags = new RuneTag[] { RuneTag.Mobile };
            executionPriority = 22; // Execute after Move and Homing
        }

        public override void ApplyToSpell(SpellContext context)
        {
            context.SetCustomData("SpiralRotation", rotationSpeed);
            context.SetCustomData("SpiralRadius", spiralRadius);
            context.SetCustomData("ExpandSpiral", expandSpiral);
            context.SetCustomData("ExpansionRate", expansionRate);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            var spiral = spellInstance.AddComponent<SpiralBehaviour>();
            spiral.Initialize(rotationSpeed, spiralRadius, expandSpiral, expansionRate);
        }
    }

    /// <summary>
    /// Runtime component that adds spiral motion to projectiles
    /// </summary>
    public class SpiralBehaviour : MonoBehaviour
    {
        private float rotSpeed;
        private float radius;
        private bool expand;
        private float expandRate;
        private float currentAngle;
        private float currentRadius;
        private Vector3 baseDirection;
        private Vector3 centerLine;

        public void Initialize(float rotation, float spiralRadius, bool expanding, float expansion)
        {
            rotSpeed = rotation;
            radius = spiralRadius;
            currentRadius = spiralRadius;
            expand = expanding;
            expandRate = expansion;
            currentAngle = 0f;

            // Store initial direction
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                baseDirection = rb.velocity.normalized;
            }
            else
            {
                baseDirection = transform.forward;
            }
        }

        private void Start()
        {
            centerLine = transform.position;
        }

        private void FixedUpdate()
        {
            // Update angle
            currentAngle += rotSpeed * Time.fixedDeltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f;

            // Expand if configured
            if (expand)
            {
                currentRadius += expandRate * Time.fixedDeltaTime;
            }

            // Calculate spiral offset
            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 right = Vector3.Cross(baseDirection, Vector3.up).normalized;
            Vector3 up = Vector3.Cross(right, baseDirection).normalized;

            Vector3 spiralOffset = (right * Mathf.Cos(radians) + up * Mathf.Sin(radians)) * currentRadius;

            // Apply to rigidbody
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                float speed = rb.velocity.magnitude;
                centerLine += baseDirection * speed * Time.fixedDeltaTime;
                rb.MovePosition(centerLine + spiralOffset);
            }
        }
    }
}

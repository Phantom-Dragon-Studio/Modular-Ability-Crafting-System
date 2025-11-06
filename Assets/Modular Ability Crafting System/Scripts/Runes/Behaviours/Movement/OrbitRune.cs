using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Movement
{
    /// <summary>
    /// Makes the spell orbit around the caster or a target.
    /// Great for shield-like effects or defensive spells.
    /// </summary>
    [CreateAssetMenu(fileName = "Orbit Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Orbit", order = 103)]
    public class OrbitRune : BaseBehaviour
    {
        [Header("Orbit Settings")]
        [Tooltip("Orbit speed (degrees per second)")]
        [SerializeField] private float orbitSpeed = 90f;

        [Tooltip("Orbit radius")]
        [SerializeField] private float orbitRadius = 3f;

        [Tooltip("Height offset from orbit center")]
        [SerializeField] private float heightOffset = 0f;

        [Tooltip("Orbit around target instead of caster?")]
        [SerializeField] private bool orbitAroundTarget = false;

        private void OnEnable()
        {
            category = RuneCategory.Movement;
            requiredTags = new RuneTag[] { }; // Can work standalone
            incompatibleTags = new RuneTag[] { RuneTag.NoMovement };
            executionPriority = 15;
        }

        public override void ApplyToSpell(SpellContext context)
        {
            context.SetCustomData("OrbitSpeed", orbitSpeed);
            context.SetCustomData("OrbitRadius", orbitRadius);
            context.SetCustomData("OrbitHeight", heightOffset);
            context.SetCustomData("OrbitTarget", orbitAroundTarget);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            var orbit = spellInstance.AddComponent<OrbitBehaviour>();
            Transform center = orbitAroundTarget && context.TargetTransform != null
                ? context.TargetTransform
                : context.CasterTransform;

            orbit.Initialize(center, orbitSpeed, orbitRadius, heightOffset);
        }
    }

    /// <summary>
    /// Runtime component that handles orbital movement
    /// </summary>
    public class OrbitBehaviour : MonoBehaviour
    {
        private Transform orbitCenter;
        private float speed;
        private float radius;
        private float height;
        private float currentAngle;

        public void Initialize(Transform center, float orbitSpeed, float orbitRadius, float heightOffset)
        {
            orbitCenter = center;
            speed = orbitSpeed;
            radius = orbitRadius;
            height = heightOffset;
            currentAngle = Random.Range(0f, 360f); // Random starting angle
        }

        private void Update()
        {
            if (orbitCenter == null)
            {
                Destroy(gameObject);
                return;
            }

            // Update angle
            currentAngle += speed * Time.deltaTime;
            if (currentAngle >= 360f) currentAngle -= 360f;

            // Calculate position
            float radians = currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(radians) * radius,
                height,
                Mathf.Sin(radians) * radius
            );

            transform.position = orbitCenter.position + offset;

            // Face forward in orbit direction
            Vector3 tangent = new Vector3(-Mathf.Sin(radians), 0, Mathf.Cos(radians));
            transform.rotation = Quaternion.LookRotation(tangent);
        }
    }
}

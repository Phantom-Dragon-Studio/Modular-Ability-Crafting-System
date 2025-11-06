using ModularAbilityCraftingSystem.Abilities;
using UnityEngine;

namespace ModularAbilityCraftingSystem.Runes.Behaviours.Movement
{
    /// <summary>
    /// Makes spells fall from above like rain.
    /// Perfect for AOE bombardment effects.
    /// </summary>
    [CreateAssetMenu(fileName = "Rain Rune", menuName = "Phantom Dragon Studio/MACS/Runes/Behaviours/Rain", order = 104)]
    public class RainRune : BaseBehaviour
    {
        [Header("Rain Settings")]
        [Tooltip("Height above target to spawn projectiles")]
        [SerializeField] private float spawnHeight = 15f;

        [Tooltip("Fall speed")]
        [SerializeField] private float fallSpeed = 10f;

        [Tooltip("Spread area radius")]
        [SerializeField] private float spreadRadius = 5f;

        [Tooltip("Random spread within radius?")]
        [SerializeField] private bool randomSpread = true;

        private void OnEnable()
        {
            category = RuneCategory.Movement;
            addedTags = new RuneTag[] { RuneTag.AOE };
            executionPriority = 8; // Execute very early (modifies spawn position)
        }

        public override void ApplyToSpell(SpellContext context)
        {
            // Modify spawn position to be above target
            Vector3 targetPos = context.TargetPosition;

            if (randomSpread)
            {
                Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
                targetPos.x += randomOffset.x;
                targetPos.z += randomOffset.y;
            }

            targetPos.y += spawnHeight;
            context.CastPosition = targetPos;

            // Set downward direction
            context.Direction = Vector3.down;
            context.Speed = fallSpeed;

            context.SetCustomData("IsRaining", true);
            context.AddTag(RuneTag.AOE);
        }

        public override void AttachBehaviour(SpellContext context, GameObject spellInstance)
        {
            if (spellInstance == null) return;

            // Ensure the projectile falls
            Rigidbody rb = spellInstance.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false; // Use custom fall speed
                rb.velocity = Vector3.down * fallSpeed;
            }
        }
    }
}

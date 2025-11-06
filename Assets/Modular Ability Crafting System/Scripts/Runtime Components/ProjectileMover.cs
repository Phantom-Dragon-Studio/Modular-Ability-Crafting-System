using UnityEngine;

namespace ModularAbilityCraftingSystem.Runtime
{
    /// <summary>
    /// Runtime component that handles projectile movement.
    /// Attached to spell instances that need to move.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileMover : MonoBehaviour
    {
        private Rigidbody rb;
        private float speed;
        private float lifetime;
        private float maxRange;
        private Vector3 startPosition;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            startPosition = transform.position;
        }

        public void Initialize(float moveSpeed, float duration, float range)
        {
            speed = moveSpeed;
            lifetime = duration;
            maxRange = range;
            startPosition = transform.position;

            // Set initial velocity
            rb.velocity = transform.forward * speed;

            // Auto-destroy after lifetime
            if (lifetime > 0)
            {
                Destroy(gameObject, lifetime);
            }
        }

        private void Update()
        {
            // Check range
            if (maxRange > 0)
            {
                float distanceTraveled = Vector3.Distance(startPosition, transform.position);
                if (distanceTraveled >= maxRange)
                {
                    Destroy(gameObject);
                }
            }
        }

        private void FixedUpdate()
        {
            // Maintain constant speed
            if (rb.velocity.magnitude > 0)
            {
                rb.velocity = rb.velocity.normalized * speed;
            }
        }

        public void SetVelocity(Vector3 velocity)
        {
            rb.velocity = velocity;
        }

        public Vector3 GetVelocity()
        {
            return rb.velocity;
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;
            if (rb.velocity.magnitude > 0)
            {
                rb.velocity = rb.velocity.normalized * speed;
            }
        }
    }
}

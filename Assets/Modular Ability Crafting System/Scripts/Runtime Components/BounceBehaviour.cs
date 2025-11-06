using UnityEngine;

namespace ModularAbilityCraftingSystem.Runtime
{
    /// <summary>
    /// Runtime component that makes a projectile bounce off surfaces.
    /// </summary>
    public class BounceBehaviour : MonoBehaviour
    {
        private int maxBounces = 3;
        private int currentBounces = 0;
        private float bounceDamping = 0.8f; // Speed retention after bounce
        private bool destroyOnMaxBounces = true;

        public void Initialize(int maxBounceCount, float damping = 0.8f, bool destroyWhenExhausted = true)
        {
            maxBounces = maxBounceCount;
            bounceDamping = damping;
            destroyOnMaxBounces = destroyWhenExhausted;
            currentBounces = 0;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (currentBounces >= maxBounces)
            {
                if (destroyOnMaxBounces)
                {
                    Destroy(gameObject);
                }
                return;
            }

            // Reflect velocity
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null && collision.contacts.Length > 0)
            {
                Vector3 reflectedVelocity = Vector3.Reflect(rb.velocity, collision.contacts[0].normal);
                rb.velocity = reflectedVelocity * bounceDamping;
                transform.rotation = Quaternion.LookRotation(reflectedVelocity);
            }

            currentBounces++;

            // Trigger bounce event (can be subscribed to by other systems)
            OnBounce?.Invoke(currentBounces, collision);
        }

        public int GetCurrentBounces() => currentBounces;
        public int GetMaxBounces() => maxBounces;
        public int GetRemainingBounces() => Mathf.Max(0, maxBounces - currentBounces);

        // Event for when a bounce occurs
        public System.Action<int, Collision> OnBounce;
    }
}

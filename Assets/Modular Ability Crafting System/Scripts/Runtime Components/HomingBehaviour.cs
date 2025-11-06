using UnityEngine;

namespace ModularAbilityCraftingSystem.Runtime
{
    /// <summary>
    /// Runtime component that makes a projectile home towards a target.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class HomingBehaviour : MonoBehaviour
    {
        private Transform target;
        private Rigidbody rb;
        private float homingStrength = 5f;
        private float maxHomingAngle = 180f;
        private bool acquireNewTarget = true;
        private float detectionRadius = 20f;
        private LayerMask targetLayers = -1;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Initialize(Transform targetTransform, float strength, float maxAngle, bool autoAcquire = true, float radius = 20f)
        {
            target = targetTransform;
            homingStrength = strength;
            maxHomingAngle = maxAngle;
            acquireNewTarget = autoAcquire;
            detectionRadius = radius;

            if (target == null && acquireNewTarget)
            {
                AcquireTarget();
            }
        }

        private void FixedUpdate()
        {
            // Try to acquire target if we don't have one
            if (target == null && acquireNewTarget)
            {
                AcquireTarget();
            }

            // Apply homing if we have a target
            if (target != null && rb.velocity.magnitude > 0)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 currentDirection = rb.velocity.normalized;

                // Check if target is within max homing angle
                float angle = Vector3.Angle(currentDirection, directionToTarget);
                if (angle <= maxHomingAngle)
                {
                    // Smoothly rotate towards target
                    Vector3 newDirection = Vector3.RotateTowards(
                        currentDirection,
                        directionToTarget,
                        homingStrength * Mathf.Deg2Rad * Time.fixedDeltaTime,
                        0f
                    );

                    // Maintain speed while changing direction
                    rb.velocity = newDirection * rb.velocity.magnitude;
                    transform.rotation = Quaternion.LookRotation(newDirection);
                }
            }
        }

        private void AcquireTarget()
        {
            // Find nearest target in range
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, targetLayers);
            float nearestDistance = float.MaxValue;
            Transform nearestTarget = null;

            foreach (var hit in hits)
            {
                if (hit.transform == transform) continue;

                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = hit.transform;
                }
            }

            target = nearestTarget;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public Transform GetTarget()
        {
            return target;
        }

        public void SetTargetLayers(LayerMask layers)
        {
            targetLayers = layers;
        }
    }
}

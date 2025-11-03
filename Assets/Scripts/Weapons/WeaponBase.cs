using UnityEngine;

namespace dutpekmezi
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Assigned Data")]
        [SerializeField] private WeaponData weaponData;

        [Header("Orbit Settings")]
        public Transform player;            // The player this weapon orbits around
        public float orbitRadius = 2f;      // Distance from the player
        public float orbitSpeed = 90f;      // Orbit speed in degrees per second
        public bool clockwise = true;       // Determines orbit direction

        [Header("Rotation Settings")]
        public float selfRotationSpeed = 180f; // Rotation speed around its own axis
        public bool selfRotationClockwise = true;

        [Header("Weapon Parts")]
        public Transform front;             // Front tip of the weapon
        public Transform back;              // Back side of the weapon
        public Transform selfOrbitCenter;   // Local pivot around which the weapon rotates

        protected float currentAngle;       // Current angle along the orbit
        protected Vector3 orbitCenter;      // Cached position of the player

        private bool canRotate = true;

        protected virtual void Start()
        {
            if (player == null)
            {
                enabled = false;
                return;
            }

            if (selfOrbitCenter == null)
            {
                selfOrbitCenter = transform;
            }

            orbitCenter = player.position;
            Vector3 offset = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0f) * orbitRadius;
            transform.position = orbitCenter + offset;
        }

        protected virtual void Update()
        {
            Orbit();
            RotateSelf();
            Ability();
        }

        private void Orbit() 
        {
            if (!canRotate) return;

            float direction = clockwise ? 1f : -1f;

            currentAngle += orbitSpeed * direction * Time.deltaTime;
            if (currentAngle > 360f) currentAngle -= 360f;
            if (currentAngle < 0f) currentAngle += 360f;

            orbitCenter = player.position;
            Vector3 offset = new Vector3(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                0f
            ) * orbitRadius;

            transform.position = orbitCenter + offset;
        }

        private void RotateSelf()
        {
            if (selfOrbitCenter == null)
            {
                // Fall back to simple local rotation if no pivot is assigned
                transform.Rotate(Vector3.forward * selfRotationSpeed * (selfRotationClockwise ? -1f : 1f) * Time.deltaTime);
                return;
            }

            // Rotate the entire weapon around its local selfOrbitCenter
            transform.RotateAround(
                selfOrbitCenter.position,
                Vector3.forward,
                selfRotationSpeed * (selfRotationClockwise ? -1f : 1f) * Time.deltaTime
            );
        }

        protected void SetRotate(bool canRotate)
        {
            this.canRotate = canRotate;
        }

        // Each weapon defines its own unique behavior
        protected abstract void Ability();
    }
}

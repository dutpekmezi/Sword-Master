using UnityEngine;

namespace dutpekmezi
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Assigned Data")]
        [SerializeField] protected WeaponData weaponData;

        [Header("Orbit Settings")]
        public float orbitRadius = 2f;
        public float orbitSpeed = 90f;
        public bool clockwise = true;

        [Header("Rotation Settings")]
        public float selfRotationSpeed = 180f;
        public bool selfRotationClockwise = true;

        [Header("Weapon Parts")]
        public Transform front;
        public Transform back;
        public Transform selfOrbitCenter;

        protected float currentAngle;
        protected Vector2 orbitCenter;

        private bool canRotate = true;

        protected virtual void Start()
        {
            if (selfOrbitCenter == null)    
            {
                selfOrbitCenter = transform;
            }
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

            Transform characterTransform = CharacterSystem.Instance.GetCharacterTransform();
            orbitCenter = characterTransform.position;
            Vector2 offset = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            ) * orbitRadius;

            transform.position = new Vector3(orbitCenter.x + offset.x, orbitCenter.y + offset.y, transform.position.z);
        }

        private void RotateSelf()
        {
            transform.Rotate(Vector3.forward * selfRotationSpeed * (selfRotationClockwise ? -1f : 1f) * Time.deltaTime);
        }

        protected void SetRotate(bool canRotate)
        {
            this.canRotate = canRotate;
        }

        protected abstract void Ability();
    }
}

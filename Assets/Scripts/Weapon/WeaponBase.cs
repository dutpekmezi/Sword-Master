using UnityEngine;

namespace dutpekmezi
{
    public abstract class WeaponBase : MonoBehaviour
    {
        [Header("Assigned Data")]
        [SerializeField] protected WeaponData weaponData;

        [Header("Orbit Settings")]
        public bool clockwise = true;

        [Header("Rotation Settings")]
        public bool selfRotationClockwise = true;

        protected float currentAngle;
        protected Vector2 orbitCenter;

        private bool canRotate = true;


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

            currentAngle += weaponData.OrbitSpeed * direction * Time.deltaTime;
            if (currentAngle > 360f) currentAngle -= 360f;
            if (currentAngle < 0f) currentAngle += 360f;

            Transform characterTransform = CharacterSystem.Instance.GetCurrentCharacterTransform();
            orbitCenter = characterTransform.position;
            Vector2 offset = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            ) * weaponData.OrbitRadius;

            transform.position = new Vector3(orbitCenter.x + offset.x, orbitCenter.y + offset.y, transform.position.z);
        }

        private void RotateSelf()
        {
            transform.Rotate(Vector3.forward * weaponData.SelfOrbitSpeed * (selfRotationClockwise ? -1f : 1f) * Time.deltaTime);
        }

        protected void SetRotate(bool canRotate)
        {
            this.canRotate = canRotate;
        }

        protected abstract void Ability();
    }
}

using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

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


        public void Tick()
        {
            Orbit();
            RotateSelf();
        }

        private void Update()
        {
            Ability();
        }

        private void Orbit()
        {
            float direction = 1f;
            currentAngle += weaponData.OrbitSpeed * direction * LogicTimer.FixedDelta;

            Vector3 charPos = CharacterSystem.Instance.GetCurrentCharacter().transform.position;
            Vector2 offset = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            ) * weaponData.OrbitRadius;

            transform.position = charPos + (Vector3)offset;
        }

        private void RotateSelf()
        {
            transform.Rotate(Vector3.forward *
                weaponData.SelfOrbitSpeed *
                LogicTimer.FixedDelta);
        }

        protected void SetRotate(bool canRotate)
        {
            this.canRotate = canRotate;
        }

        protected abstract void Ability();
    }
}

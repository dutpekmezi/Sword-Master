using System.Collections.Generic;
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

        protected Dictionary<StatType, Stat> _runtimeStats = new Dictionary<StatType, Stat>();

        public virtual void Initialize()
        {
            _runtimeStats.Clear();

            foreach (var baseStat in weaponData.BaseStats)
            {
                Stat runtimeStat = new Stat(baseStat.BaseValue);

                _runtimeStats.Add(baseStat.Type, runtimeStat);
            }

            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(ApplySelectedModifier);
        }

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
            var character = CharacterSystem.Instance.GetCurrentCharacter();

            float linearSpeed = GetStatValue(StatType.WeaponOrbitSpeed);
            float radius = GetStatValue(StatType.WeaponOrbitRadius);

            float direction = clockwise ? 1f : -1f;

            float angularSpeed;
            const float BaseAngularFactor = 100f;

            if (radius > 0)
            {
                angularSpeed = (linearSpeed / radius) * BaseAngularFactor;
            }
            else
            {
                angularSpeed = linearSpeed * BaseAngularFactor;
            }

            currentAngle += angularSpeed * direction * LogicTimer.FixedDelta;

            Vector3 charPos = character.transform.position;
            Vector2 offset = new Vector2(
                Mathf.Cos(currentAngle * Mathf.Deg2Rad),
                Mathf.Sin(currentAngle * Mathf.Deg2Rad)
            ) * radius;

            transform.position = charPos + (Vector3)offset;
        }

        private void RotateSelf()
        {
            transform.Rotate(Vector3.forward *
                GetStatValue(StatType.WeaponSelfOrbitSpeed) *
                LogicTimer.FixedDelta);
        }

        protected virtual void ApplyModifier(StatModifier modifier)
        {
            if (_runtimeStats.TryGetValue(modifier.Type, out Stat stat))
            {
                stat.AddModifier(modifier);
            }
        }

        protected virtual void ApplySelectedModifier(StatModifier modifier)
        {
            if (modifier.Target != StatTarget.Weapon) return;

            ApplyModifier(modifier);
        }

        public float GetStatValue(StatType type)
        {
            if (_runtimeStats.TryGetValue(type, out Stat stat))
            {
                return stat.Value;
            }
            return 0f;
        }

        protected void SetRotate(bool canRotate)
        {
            this.canRotate = canRotate;
        }

        protected abstract void Ability();
    }
}
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

        [Header("Ability Data")]
        [SerializeField] protected AbilityBase<WeaponBase> abilityData;

        [Header("Orbit Settings")]
        public bool clockwise = true;

        [Header("Rotation Settings")]
        public bool selfRotationClockwise = true;

        public float currentAngle;
        protected Vector2 orbitCenter;

        private bool canRotate = true;

        private float abilityCooldownTimer = 0f;
        private bool isAbilityReady = true;

        protected Dictionary<StatType, BaseStatConfig> _runtimeStats = new Dictionary<StatType, BaseStatConfig>();

        public virtual void Initialize()
        {
            _runtimeStats.Clear();

            foreach (var baseStatConfig in weaponData.BaseStatConfigs)
            {
                Stat runtimeStat = new Stat(baseStatConfig);

                BaseStatConfig _baseStatConfig = new BaseStatConfig(runtimeStat);

                _runtimeStats.Add(baseStatConfig.BaseStat.Type, _baseStatConfig);
            }

            isAbilityReady = true;
            abilityCooldownTimer = 0f;

            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(ApplySelectedModifier);
        }

        public void Tick()
        {
            Orbit();
            RotateSelf();

            if (abilityCooldownTimer > 0f)
            {
                abilityCooldownTimer -= LogicTimer.FixedDelta;
                if (abilityCooldownTimer <= 0f)
                {
                    OnAbilityCooldownFinished();
                }
            }
        }

        private void Update()
        {
            if (CanUseAbility())
            {
                Ability();
                StartAbilityCooldown();
            }
        }

        protected void StartAbilityCooldown()
        {
            float cooldown = GetStatValue(StatType.AbilityCooldown);

            if (cooldown > 0f)
            {
                abilityCooldownTimer = cooldown;
                isAbilityReady = false;
            }
            else
            {
                isAbilityReady = true;
                abilityCooldownTimer = 0f;
            }
        }

        private void OnAbilityCooldownFinished()
        {
            isAbilityReady = true;
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
            if (_runtimeStats.TryGetValue(modifier.Type, out BaseStatConfig statConfig))
            {
                statConfig.BaseStat.AddModifier(modifier);
            }
        }

        protected virtual void ApplySelectedModifier(StatModifier modifier)
        {
            if (modifier.Target != StatTarget.Weapon) return;

            ApplyModifier(modifier);
        }

        public float GetStatValue(StatType type)
        {
            if (_runtimeStats.TryGetValue(type, out BaseStatConfig statConfig))
            {
                return statConfig.BaseStat.Value;
            }
            return 0f;
        }

        public void SetRotate(bool canRotate)
        {
            this.canRotate = canRotate;
        }

        protected virtual void Ability()
        {
            abilityData.UseAbility(this);
        }

        protected virtual bool CanUseAbility()
        {
            return isAbilityReady;
        }
    }
}
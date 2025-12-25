using DG.Tweening.Core.Easing;
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
            SignalBus.Get<InputManager.OnAbilityButtonClick>().Subscribe(Ability);
        }

        public void Tick()
        {
            Orbit();
            RotateSelf();

            if (!isAbilityReady)
            {
                abilityCooldownTimer -= LogicTimer.FixedDelta;
                if (abilityCooldownTimer <= 0f)
                {
                    abilityCooldownTimer = 0f;
                    isAbilityReady = true;
                }
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

        protected virtual void Ability()
        {
            if (!CanUseAbility()) return;

            if (weaponData.AbilityData != null)
            {
                var genericAbility = weaponData.AbilityData as AbilityBase<WeaponBase>;

                if (genericAbility != null)
                {
                    genericAbility.UseAbility(this);
                    StartAbilityCooldown();
                }
            }
        }

        protected virtual bool CanUseAbility()
        {
            var abilitySystem = AbilitySystem.Instance;
            bool isCorrectMode = abilitySystem.CurrentMode == AbilitySystem.AbilityMode.Weapon;

            return isAbilityReady && isCorrectMode;
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
            if (!canRotate) return;

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
            if (modifier == null) return;
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

        public void OnDispose()
        {
            SignalBus.Get<StatSystem.OnStatSelected>().Unsubscribe(ApplySelectedModifier);
            SignalBus.Get<InputManager.OnAbilityButtonClick>().Unsubscribe(Ability);
        }
    }
}
using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

namespace dutpekmezi
{
    public abstract class WeaponBase : Entity
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

        public override void Initialize()
        {
            if (weaponData == null)
            {
                Debug.LogError($"{name} is missing WeaponData assignment.");
                return;
            }

            entityData = weaponData;

            base.Initialize();

            isAbilityReady = true;
            abilityCooldownTimer = 0f;

            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(ApplySelectedModifier);
            SignalBus.Get<InputManager.OnAbilityButtonClick>().Subscribe(Ability);
        }

        public override void Tick()
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
            float baseCooldown = GetStatValue(StatType.AbilityCooldown);
            float cdr = CharacterSystem.Instance.GetCurrentCharacter().GetStatValue(StatType.CooldownReduction);

            float finalCooldown = baseCooldown * (1f - cdr);

            if (finalCooldown > 0f)
            {
                abilityCooldownTimer = finalCooldown;
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

        protected override void ApplySelectedModifier(StatModifier modifier)
        {
            if (modifier == null) return;
            if (modifier.Target != StatTarget.Weapon) return;

            ApplyModifier(modifier);
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

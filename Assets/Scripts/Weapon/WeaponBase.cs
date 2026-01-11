using System.Net.NetworkInformation;
using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;
using static dutpekmezi.CharacterBase;

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

        private Vector3 baseScale;

        private float abilityCooldownTimer = 0f;
        private float abilityCooldownDuration = 0f;
        private bool isAbilityReady = true;

        public float AbilityCooldownRemaining => abilityCooldownTimer;
        public float AbilityCooldownDuration => abilityCooldownDuration;

        public override void Initialize()
        {
            if (weaponData == null)
            {
                Debug.LogError($"{name} is missing WeaponData assignment.");
                return;
            }

            entityData = weaponData;

            base.Initialize();

            baseScale = transform.localScale; // stash prefab size once, pool might reuse
            UpdateScale();

            isAbilityReady = true;
            abilityCooldownTimer = 0f;
            abilityCooldownDuration = CalculateAbilityCooldownDuration();

            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(ApplySelectedModifier);
            SignalBus.Get<InputManager.OnAbilityButtonClick>().Subscribe(Ability);
            SignalBus.Get<OnlevelUp>().Subscribe(OnlevelUpHandler);
            SignalBus.Get<OnEnemyKill>().Subscribe(OnEnemyKillHandler);

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        public override void Tick()
        {
            Orbit();
            RotateSelf();
            HandleAbilityCooldown();
        }

        private void HandleAbilityCooldown()
        {
            if (!isAbilityReady)
            {
                abilityCooldownTimer -= LogicTimer.FixedDelta;
                if (abilityCooldownTimer <= 0f)
                {
                    abilityCooldownTimer = 0f;
                    isAbilityReady = true;
                    abilityCooldownDuration = CalculateAbilityCooldownDuration();
                }
            }
            else
            {
                abilityCooldownDuration = CalculateAbilityCooldownDuration();
            }
        }

        protected void StartAbilityCooldown()
        {
            float finalCooldown = CalculateAbilityCooldownDuration();

            abilityCooldownDuration = finalCooldown;

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

        private float CalculateAbilityCooldownDuration()
        {
            float baseCooldown = GetStatValue(StatType.AbilityCooldown);
            float cdr = CharacterSystem.Instance.GetCurrentCharacter().GetStatValue(StatType.CooldownReduction);

            return Mathf.Max(0f, baseCooldown * (1f - cdr));
        }

        protected virtual void Ability()
        {
            if (!CanUseAbility()) return;

            if (weaponData.AbilityData != null)
            {
                weaponData.AbilityData.UseAbility(this);
                StartAbilityCooldown();
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
            const float BaseAngularFactor = 100f; // fiddled so orbit speed feels ok

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

        protected virtual void OnTrigger(Entity entity)
        {

        }

        protected override void ApplyModifier(StatModifier modifier)
        {
            base.ApplyModifier(modifier);

            if (modifier.Type == StatType.Scale)
            {
                UpdateScale(); // keep size tied to stat bumps
            }

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        protected override void SetHealth(float amount)
        {
            base.SetHealth(amount);

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        protected virtual void OnlevelUpHandler(int level)
        {

        }

        public override void Gainlevel(int amount = 1)
        {
            base.Gainlevel(amount);
            Scalelevel();

            SignalBus.Get<OnStatsChange>().Invoke(this);
            SignalBus.Get<OnLevelUp>().Invoke(currentLevel);
        }

        private void Scalelevel()
        {
            var modifier = StatSystem.Instance.CreateModifier(StatType.ExpToLevelUp, 15);

            ApplyModifier(modifier);
        }

        protected override void GainExp(float amount)
        {
            base.GainExp(amount);

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        protected override void ApplySelectedModifier(StatModifier modifier)
        {
            if (modifier == null) return;
            if (modifier.Target != StatTarget.Weapon) return;

            ApplyModifier(modifier);
        }

        private void OnEnemyKillHandler(float amount)
        {
            GainExp(amount);
        }

        public void SetRotate(bool canRotate)
        {
            this.canRotate = canRotate;
        }

        private void UpdateScale()
        {
            float scaleValue = GetStatValue(StatType.Scale);

            if (scaleValue <= 0f)
            {
                scaleValue = 1f; // fallback so pooled stuff doesn't vanish
            }

            transform.localScale = baseScale * scaleValue;
        }

        protected override void Die()
        {
            isDead = true;

            WeaponSystem.Instance.UnequipWeapon();
        }

        public void OnDispose()
        {
            SignalBus.Get<StatSystem.OnStatSelected>().Unsubscribe(ApplySelectedModifier);
            SignalBus.Get<InputManager.OnAbilityButtonClick>().Unsubscribe(Ability);
            SignalBus.Get<OnlevelUp>().Unsubscribe(OnlevelUpHandler);
            SignalBus.Get<OnEnemyKill>().Unsubscribe(OnEnemyKillHandler);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (!col.TryGetComponent(out Entity entity) || entity == this)
                return;

            OnTakeDamageHandler(this, entity.GetStatValue(StatType.BodyDamage));

            if (entity is EnemyBase enemy)
            {
                OnTrigger(enemy);
                return;
            }

            if (entity is ChestBase chest && !chest.IsOpened)
            {
                chest.OnTakeDamageHandler(chest, weaponData.AttackDamage);
            }
        }

        public class OnStatsChange : Signal<WeaponBase> { }
        public class OnLevelUp : Signal<int> { }
    }
}

using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

namespace dutpekmezi
{
    public class CharacterBase : Entity
    {
        [Header("Assigned Data")]
        [SerializeField] private CharacterData characterData;

        [Header("Current Info")]
        [SerializeField] private float currentEnergy;
        [SerializeField] private float everySecondTickDuration;

        private float tickTimer;
        private Vector2 moveInput;
        private Vector2 moveVelocity;

        private float abilityCooldownTimer = 0f;
        private float abilityCooldownDuration = 0f;
        private bool isAbilityReady = true;

        public float CurrentEnergy => currentEnergy;
        public bool isEnergyFull => currentEnergy >= GetStatValue(StatType.Energy);
        public float AbilityCooldownRemaining => abilityCooldownTimer;
        public float AbilityCooldownDuration => abilityCooldownDuration;

        public override void Initialize()
        {
            base.Initialize();

            currentEnergy = 0;
            currentLevel = 1;
            tickTimer = everySecondTickDuration;
            isAbilityReady = true;
            abilityCooldownTimer = 0f;
            abilityCooldownDuration = CalculateFinalAbilityCooldown();

            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(ApplySelectedModifier);
            SignalBus.Get<OnEnemyKill>().Subscribe(GainExp);
            SignalBus.Get<OnlevelUp>().Subscribe(OnLevelUpHandler);
            SignalBus.Get<InputManager.OnAbilityButtonClick>().Subscribe(UseAbility);

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        public override void Tick()
        {
            if (isDead) return;

            HandleInput();
            MoveCharacter();
            OnEverySecondTick();
            HandleCooldown();
        }

        private void HandleCooldown()
        {
            if (!isAbilityReady)
            {
                abilityCooldownTimer -= LogicTimer.FixedDelta;
                if (abilityCooldownTimer <= 0f)
                {
                    abilityCooldownTimer = 0f;
                    isAbilityReady = true;
                    abilityCooldownDuration = CalculateFinalAbilityCooldown();
                }
            }
            else
            {
                abilityCooldownDuration = CalculateFinalAbilityCooldown();
            }
        }

        private void OnEverySecondTick()
        {
            float dt = LogicTimer.FixedDelta;
            tickTimer -= dt;

            if (tickTimer <= 0f)
            {
                float regenAmount = GetStatValue(StatType.HealthRegen);

                if (regenAmount > 0)
                    Heal(regenAmount);

                tickTimer = everySecondTickDuration;
            }
        }

        public void UseAbility()
        {
            if (isDead || !isAbilityReady) return;

            var abilitySystem = AbilitySystem.Instance;

            if (abilitySystem.CurrentMode != AbilitySystem.AbilityMode.Character) return;

            if (characterData.AbilityData != null)
            {
                characterData.AbilityData.UseAbility(this);
                StartAbilityCooldown();
            }
        }

        private void StartAbilityCooldown()
        {
            float finalCooldown = CalculateFinalAbilityCooldown();

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

        private float CalculateFinalAbilityCooldown()
        {
            float baseCooldown = GetStatValue(StatType.AbilityCooldown);
            float cdr = GetStatValue(StatType.CooldownReduction);

            return Mathf.Max(0f, baseCooldown * (1f - cdr));
        }

        private void HandleInput()
        {
            moveInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        }

        private void MoveCharacter()
        {
            Vector2 targetVelocity = moveInput * GetStatValue(StatType.MoveSpeed);

            moveVelocity = Vector2.Lerp(
                moveVelocity,
                targetVelocity,
                0.15f
            );

            rb.MovePosition(rb.position + moveVelocity * Utils.LogicTimer.LogicTimer.FixedDelta);
        }

        protected override void ApplyModifier(StatModifier modifier)
        {
            base.ApplyModifier(modifier);

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        protected override void SetHealth(float amount)
        {
            base.SetHealth(amount);

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        private void SetEnergy(float amount)
        {
            if (isDead || isEnergyFull) return;

            currentEnergy += amount;

            if (currentEnergy > GetStatValue(StatType.Energy))
            {
                currentEnergy = GetStatValue(StatType.Energy);
            }

            if (currentEnergy < 0)
            {
                currentEnergy = 0;
            }

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        public override void Gainlevel(int amount = 1)
        {
            base.Gainlevel(amount);

            Scalelevel();

            SignalBus.Get<OnStatsChange>().Invoke(this);
            SignalBus.Get<OnlevelUp>().Invoke(amount);
        }

        protected override void GainExp(float amount)
        {
            base.GainExp(amount);

            SetEnergy(amount);
        }

        private void Scalelevel()
        {
            var modifier = StatSystem.Instance.CreateModifier(StatType.ExpToLevelUp, 10);

            ApplyModifier(modifier);
        }

        private void OnLevelUpHandler(int level)
        {
            SignalBus.Get<StatSystem.OnStatSelection>().Invoke();
        }

        public Vector2 GetMoveDirection()
        {
            return moveInput;
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var enemy = col.gameObject.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                OnTakeDamageHandler(this, enemy.GetStatValue(StatType.BodyDamage));
                SignalBus.Get<OnCollideWithEnemy>().Invoke(enemy, this, GetStatValue(StatType.BodyDamage));
            }
        }

        public class OnEnemyKill : Signal<float> { }
        public class OnCollideWithEnemy : Signal<EnemyBase, CharacterBase, float> { }
        public class OnStatsChange : Signal<CharacterBase> { }
        public class OnlevelUp : Signal<int> { }
    }
}

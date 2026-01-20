using Dutpekmezi.Services.PoolService;
using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

namespace dutpekmezi
{
    public class EnemyBase : Entity
    {
        public override void Initialize()
        {
            base.Initialize();

            currentLevel = CharacterSystem.Instance.GetCurrentCharacter().CurrentLevel;

            var upgradableStatsType = entityData.GetUpgradableStatsType();

            _runtimeStats = StatSystem.Instance.ScaleStats(_runtimeStats, currentLevel);

            SignalBus.Get<CharacterBase.OnCollideWithEnemy>().Subscribe(OnCollideWithEnemyHandler);
            SignalBus.Get<OnTakeDamage>().Subscribe(OnTakeDamageHandler);
        }

        public void Tick(Vector2 playerPos)
        {
            if (isDead) return;

            Vector2 currentPos = transform.position;
            Vector2 dir = (playerPos - currentPos).normalized;

            transform.position = Vector2.MoveTowards(
                currentPos,
                playerPos,
                GetStatValue(StatType.MoveSpeed) * LogicTimer.FixedDelta
            );
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

        protected override void Die()
        {
            base.Die();

            SignalBus.Get<CharacterBase.OnEnemyKill>().Invoke(GetStatValue(StatType.ExpOnDeath));
        }

        private void OnCollideWithEnemyHandler(EnemyBase enemy, CharacterBase character, float dmg)
        {
            if (enemy != this) return;
            SignalBus.Get<OnTakeDamage>().Invoke(enemy, dmg);
        }

        public class OnStatsChange : Signal<EnemyBase> { }
        public class OnTakeDamage : Signal<Entity, float> { }
    }
}

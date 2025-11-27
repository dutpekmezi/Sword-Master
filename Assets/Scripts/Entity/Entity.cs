using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using static dutpekmezi.CharacterBase;
using Utils.Signal;
using Dutpekmezi.Services.PoolService;
using static dutpekmezi.EnemyBase;

namespace dutpekmezi
{
    public class Entity : MonoBehaviour
    {
        protected Dictionary<StatType, Stat> _runtimeStats = new Dictionary<StatType, Stat>();

        [SerializeField] protected float currentHealth;

        protected bool isDead = false;

        public float CurrentHealth => currentHealth;
        public Transform Transform => transform;

        public virtual void Initialize()
        {
            isDead = false;

            _runtimeStats.Clear();

            /*foreach (var baseStat in characterData.BaseStats)
            {
                Stat runtimeStat = new Stat(baseStat.BaseValue);

                _runtimeStats.Add(baseStat.Type, runtimeStat);
            }

            currentHealth = (int)GetStatValue(StatType.MaxHealth);
            currentEnergy = 0;*/

            SignalBus.Get<OnTakeDamage>().Subscribe(OnTakeDamageHandler);

            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(ApplySelectedModifier);
        }

        public virtual void Tick()
        {

        }

        public virtual void ApplyModifier(StatModifier modifier)
        {
            if (_runtimeStats.TryGetValue(modifier.Type, out Stat stat))
            {
                stat.AddModifier(modifier);

                if (modifier.Type == StatType.MaxHealth)
                {
                    if (currentHealth > stat.Value)
                    {
                        currentHealth = stat.Value;
                    }
                }

                SignalBus.Get<OnStatsChange>().Invoke(this);
            }
        }

        protected virtual void ApplySelectedModifier(StatModifier modifier)
        {
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

        private void OnTakeDamageHandler(Entity entity, float dmg)
        {
            if (isDead) return;

            TakeDamage(dmg);
        }

        protected virtual void TakeDamage(float damageAmount)
        {
            SetHealth(-damageAmount);
        }

        private void SetHealth(float amount)
        {
            if (isDead) return;

            currentHealth += amount;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
            }

            SignalBus.Get<OnStatsChange>().Invoke(this);
        }

        protected virtual void Die()
        {
            isDead = true;
            ObjectPoolManager.DeSpawn(gameObject);

            SignalBus.Get<OnDeath>().Invoke(this);
        }

        public class OnTakeDamage : Signal<Entity, float> { }
        public class OnStatsChange : Signal<Entity> { }
        public class OnDeath : Signal<Entity> { }
    }
}
using Dutpekmezi.Services.PoolService;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace dutpekmezi
{
    public class Entity : MonoBehaviour
    {
        [Header("Assigned Data")]
        [SerializeField] protected EntityData entityData;

        [Header("Current Info")]
        [SerializeField] protected float currentHealth;
        [SerializeField] protected int currentLevel;
        [SerializeField] protected float currentExp;

        [Header("Runtime Stats (LIVE)")]
        [SerializeField]
        private List<Stat> _runtimeStatsList = new List<Stat>();

        [Header("References")]
        [SerializeField] protected Rigidbody2D rb;
        [SerializeField] protected Collider2D col;

        protected Dictionary<StatType, BaseStatConfig> _runtimeStats = new Dictionary<StatType, BaseStatConfig>();
        protected bool isDead = false;

        public EntityData EntityData => entityData;
        public bool IsDead => isDead;
        public float CurrentHealth => currentHealth;
        public float CurrentExp => currentExp;
        public int CurrentLevel => currentLevel;
        public Transform Transform => transform;
        public Rigidbody2D Rb => rb;

        public virtual void Initialize()
        {
            isDead = false;

            _runtimeStats.Clear();

            foreach (var baseStatConfig in entityData.BaseStatConfigs)
            {
                Stat runtimeStat = new Stat(baseStatConfig.BaseStat.BaseValue);

                BaseStatConfig _baseStatConfig = new BaseStatConfig(runtimeStat);

                _runtimeStats.Add(baseStatConfig.BaseStat.Type, _baseStatConfig);
            }

            currentHealth = (int)GetStatValue(StatType.MaxHealth);
        }

        public virtual void Tick()
        {
            
        }

        protected virtual void ApplyModifier(StatModifier modifier)
        {
            if (_runtimeStats.TryGetValue(modifier.Type, out BaseStatConfig statConfig))
            {
                statConfig.BaseStat.AddModifier(modifier);

                if (modifier.Type == StatType.MaxHealth)
                {
                    if (currentHealth > statConfig.BaseStat.Value)
                    {
                        currentHealth = statConfig.BaseStat.Value;
                    }
                }
            }
        }

        protected virtual void ApplySelectedModifier(StatModifier modifier)
        {
            if (modifier.Target != StatTarget.Entity) return;

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

        public void OnTakeDamageHandler(Entity entity, float dmg)
        {
            if (isDead || entity != this) return;

            TakeDamage(dmg);
        }

        protected virtual void TakeDamage(float damageAmount)
        {
            SetHealth((int)-damageAmount);
        }

        protected virtual void SetHealth(float amount)
        {
            if (isDead) return;

            currentHealth += amount;

            if (currentHealth > GetStatValue(StatType.MaxHealth))
            {
                currentHealth = GetStatValue(StatType.MaxHealth);
            }

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }

        protected virtual void Heal(float amount)
        {
            if (isDead) return;

            SetHealth(amount);
        }

        protected virtual void Die()
        {
            isDead = true;

            ObjectPoolManager.DeSpawn(this.gameObject);
        }

        public virtual void Gainlevel(int amount = 1)
        {
            currentLevel += amount;
            currentExp = 0;
        }

        protected virtual void GainExp(float amount)
        {
            currentExp += amount;

            if (currentExp >= GetStatValue(StatType.ExpToLevelUp))
            {
                Gainlevel();
            }
        }
    }
}
using Dutpekmezi.Services.PoolService;
using System.Collections.Generic;
using UnityEngine;
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

        [Header("References")]
        [SerializeField] protected Rigidbody2D rb;
        [SerializeField] protected Collider2D col;

        protected Dictionary<StatType, Stat> _runtimeStats = new Dictionary<StatType, Stat>();
        protected bool isDead = false;

        public EntityData EntityData => entityData;
        public bool IsDead => isDead;
        public float CurrentHealth => currentHealth;
        public Transform Transform => transform;
        public Rigidbody2D Rb => rb;

        public virtual void Initialize()
        {
            isDead = false;

            _runtimeStats.Clear();

            foreach (var baseStat in entityData.BaseStats)
            {
                Stat runtimeStat = new Stat(baseStat.BaseValue);

                _runtimeStats.Add(baseStat.Type, runtimeStat);
            }

            currentHealth = (int)GetStatValue(StatType.MaxHealth);
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

        public void OnTakeDamageHandler(Entity entity, float dmg)
        {
            if (isDead ||entity != this) return;

            TakeDamage(dmg);
        }

        protected virtual void TakeDamage(float damageAmount)
        {
            SetHealth((int)-damageAmount);
        }

        protected virtual void SetHealth(int amount)
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

        protected void Heal(int amount)
        {
            if (isDead) return;

            SetHealth(amount);
        }

        protected virtual void Die()
        {
            isDead = true;

            ObjectPoolManager.DeSpawn(this.gameObject);
        }

        protected virtual void Gainlevel(int amount = 1)
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
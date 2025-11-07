using UnityEngine;
using UnityEngine.AI;

namespace dutpekmezi
{
    public class EnemyBase : MonoBehaviour
    {
        [Header("Assigned Datas")]
        [SerializeField] private EnemyData enemyData;

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D col;
        [SerializeField] private NavMeshAgent agent;

        public Transform Transform => rb.transform;

        private int currentHealth;

        private bool isDead = false;

        private Vector2 spawnPos;

        public delegate void OnDeathEvent(EnemyBase enemy);
        public event OnDeathEvent OnDeath;


        private void Start()
        {
            CharacterBase character = CharacterSystem.Instance.GetCurrentCharacter();

            character.OnKillEnemy += DieWithoutEvent;

            agent.enabled = false;

            spawnPos = transform.position;

            if (enemyData == null)
            {
                Debug.LogError($"{gameObject.name}: Missing EnemyData");
                enabled = false;
                return;
            }

            Init();
        }

        public void Init()
        {
            isDead = false;
            currentHealth = enemyData.MaxHealth;

            agent.updateRotation = false;
            agent.updateUpAxis = false;

            agent.Warp(transform.position);

            agent.enabled = true;
        }

        private void Update()
        {
            if (isDead) return;

            MoveToPlayer();
        }

        private void MoveToPlayer()
        {
            if (enemyData == null || CharacterSystem.Instance == null) return;

            Transform targetTransform = CharacterSystem.Instance.GetCurrentCharacterTransform();
            if (targetTransform == null) return;

            Vector2 targetPos = targetTransform.position;

            agent.speed = enemyData.MoveSpeed;
            agent.SetDestination(targetPos);
        }

        public void TakeDamage(int damage, bool eventlessDamage = false)
        {
            SetHealth(-damage, eventlessDamage);
        }

        private void SetHealth(int amount, bool eventlessDamage = false)
        {
            currentHealth += amount;

            if (currentHealth <= 0 && !eventlessDamage)
            {
                currentHealth = 0;
                Die();
            }
            else if (eventlessDamage)
            {
                currentHealth = 0;
                DieWithoutEvent();
            }
        }

        private void Die()
        {
            if (isDead) return;

            if (OnDeath != null)
                OnDeath.Invoke(this);

            isDead = true;

            Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(this.gameObject);
        }

        private void DieWithoutEvent(CharacterBase character = null)
        {
            if (isDead) return;

            if (OnDeath != null)
                OnDeath.Invoke(this);

            isDead = true;

            Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            var character = col.GetComponent<CharacterBase>();

            if (character != null)
            {
                character.TakeDamage(enemyData.AttackDamage);
                TakeDamage(1);
            }
        }
    }
}

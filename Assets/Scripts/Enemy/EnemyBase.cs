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
        [SerializeField] private bool isLeader = false;

        public delegate void OnDeathEvent(EnemyBase enemy);
        public event OnDeathEvent OnDeath;

        public bool IsDead => isDead;
        public bool IsLeader => isLeader;
        public EnemyData EnemyData => enemyData;
        public NavMeshAgent Agent => agent;

        private void Start()
        {
            agent.enabled = false;

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

            EnemyNavHelper.EnsureAgentOnNavMesh(this);

            EnemySystem.Instance.RegisterEnemy(this);
        }

        public void Tick(Vector2 playerPos)
        {
            if (isDead) return;

            agent.speed = enemyData.MoveSpeed;
            agent.SetDestination(playerPos);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            agent.isStopped = true;

            OnDeath?.Invoke(this);

            Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(gameObject);
        }

        public void SetAsLeader()
        {
            isLeader = true;
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

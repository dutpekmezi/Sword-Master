using UnityEngine;

namespace dutpekmezi
{
    public class EnemyBase : MonoBehaviour
    {
        [Header("Assigned Datas")]
        [SerializeField] private EnemyData enemyData;

        [Header("References")]
        [SerializeField] private Collider2D col;

        public Transform Transform => transform;

        private int currentHealth;
        private bool isDead = false;
        [SerializeField] private bool isLeader = false;

        public delegate void OnDeathEvent(EnemyBase enemy);
        public event OnDeathEvent OnDeath;

        public bool IsDead => isDead;
        public bool IsLeader => isLeader;
        public EnemyData EnemyData => enemyData;

        private void Start()
        {
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

            EnemySystem.Instance.RegisterEnemy(this);
        }

        public void Tick(Vector2 playerPos)
        {
            if (isDead) return;

            Vector2 currentPos = Transform.position;
            Vector2 direction = (playerPos - currentPos).normalized;

            Transform.position = Vector2.MoveTowards(
                currentPos,
                playerPos,
                enemyData.MoveSpeed * Time.deltaTime
            );

            if (direction.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            }
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

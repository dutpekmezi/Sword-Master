using UnityEngine;

namespace dutpekmezi
{
    public class EnemyBase : MonoBehaviour
    {
        [Header("Assigned Datas")]
        [SerializeField] private EnemyData enemyData;

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private BoxCollider2D col;

        public Transform Transform => rb.transform;

        private int currentHealth;

        private bool isDead = false;

        public delegate void OnDeathEvent(EnemyBase enemy);
        public event OnDeathEvent OnDeath;

        private void Start()
        {
            if (enemyData == null)
            {
                Debug.LogError($"{gameObject.name}: Missing EnemyData reference!");
                enabled = false;
                return;
            }

            Init();
        }

        public void Init()
        {
            isDead = false;
            currentHealth = enemyData.MaxHealth;
        }

        private void Update()
        {
            MoveToPlayer();
        }

        private void MoveToPlayer()
        {
            if (enemyData == null || CharacterSystem.Instance == null) return;

            Transform targetTransform = CharacterSystem.Instance.GetCharacterTransform();
            if (targetTransform == null) return;

            Vector2 currentPos = rb.position;
            Vector2 targetPos = targetTransform.position;
            Vector2 dir = (targetPos - currentPos).normalized;

            float moveSpeed = enemyData.MoveSpeed;
            Vector2 newPos = currentPos + dir * moveSpeed * Time.deltaTime;

            rb.MovePosition(newPos);
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
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

        private void OnTriggerEnter2D(Collider2D col)
        {
            var character = col.GetComponent<CharacterBase>();

            if (character != null)
            {
                character.TakeDamage(enemyData.AttackDamage);
                Die();
            }
        }
    }
}

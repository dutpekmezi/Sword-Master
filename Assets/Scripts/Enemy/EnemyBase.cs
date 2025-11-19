using Dutpekmezi.Services.PoolService;
using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

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

        public bool IsDead => isDead;
        public bool IsLeader => isLeader;
        public EnemyData EnemyData => enemyData;

        public void Initialize()
        {
            isDead = false;
            currentHealth = enemyData.MaxHealth;

            SignalBus.Get<OnTakeDamage>().Subscribe(OnTakeDamagehandler);
        }

        public void Tick(Vector2 playerPos)
        {
            if (isDead) return;

            Vector2 currentPos = transform.position;
            Vector2 dir = (playerPos - currentPos).normalized;

            transform.position = Vector2.MoveTowards(
                currentPos,
                playerPos,
                enemyData.MoveSpeed * LogicTimer.FixedDelta
            );
        }

        private void OnTakeDamagehandler(EnemyBase enemy, int dmg)
        {
            TakeDamage(dmg);
        }

        private void TakeDamage(int dmg)
        {
            currentHealth -= dmg;
            if (currentHealth <= 0)
                Die();
        }

        private void Die()
        {
            isDead = true;
            ObjectPoolManager.DeSpawn(gameObject);

            SignalBus.Get<OnDeath>().Invoke(this);
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
                SignalBus.Get<CharacterBase.OnTakeDamage>().Invoke(character, enemyData.AttackDamage);
            }
        }

        public class OnDeath : Signal<EnemyBase> {}
        public class OnTakeDamage : Signal<EnemyBase, int> {}
    }
}

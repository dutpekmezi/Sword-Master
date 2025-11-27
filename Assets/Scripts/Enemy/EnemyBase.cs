using Dutpekmezi.Services.PoolService;
using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

namespace dutpekmezi
{
    public class EnemyBase : Entity
    {
        [Header("Assigned Datas")]
        [SerializeField] private EnemyData enemyData;

        [Header("References")]
        [SerializeField] private Collider2D col;

        [SerializeField] private bool isLeader = false;

        public bool IsDead => isDead;
        public bool IsLeader => isLeader;
        public EnemyData EnemyData => enemyData;

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

        public void OnTakeDamagehandler(int dmg)
        {
            if (isDead) return;

            TakeDamage(dmg);
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
                SignalBus.Get<CharacterBase.OnTakeDamage>().Invoke(character, GetStatValue(StatType.BodyDamage));

                OnTakeDamagehandler(1);
            }
        }
    }
}

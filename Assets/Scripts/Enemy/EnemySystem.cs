using UnityEngine;

namespace dutpekmezi
{
    public class EnemySystem : MonoBehaviour
    {
        [SerializeField] private EnemyDatas enemyDatas;

        [SerializeField] private Transform enemyHolder;

        private static EnemySystem instance;
        public static EnemySystem Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(instance);
            }

            instance = this;
        }

        public EnemyBase CreateRandomEnemy()
        {
            if (enemyDatas != null)
            {
                var randomIndex = Random.Range(0, enemyDatas.Enemies.Count);

                var enemyData = enemyDatas.Enemies[randomIndex];

                EnemyBase instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(enemyData.Prefab, enemyHolder);
                instance.Init();

                return instance;
            }

            return null;
        }
    }
}

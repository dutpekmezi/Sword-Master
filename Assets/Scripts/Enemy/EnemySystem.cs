using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace dutpekmezi
{
    public class EnemySystem : MonoBehaviour
    {
        [Header("Enemy Data")]
        [SerializeField] private EnemyDatas enemyDatas;
        [SerializeField] private Transform enemyHolder;

        private List<EnemyBase> activeEnemies = new List<EnemyBase>();
        private List<EnemyBase> activeLeaderEnemies = new List<EnemyBase>();
        private List<EnemyGroup> enemyGroups = new List<EnemyGroup>();

        private Transform playerTransform;

        private static EnemySystem instance;
        public static EnemySystem Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(instance);

            instance = this;
        }

        private void Start()
        {
            playerTransform = CharacterSystem.Instance.GetCurrentCharacterTransform();
        }

        private void Update()
        {
            if (playerTransform == null) return;
            Vector3 playerPos = playerTransform.position;

            if (enemyGroups.Count > 0)
            {
                foreach (var group in enemyGroups)
                {
                    if (group != null)
                    {
                        group.UpdateGroup();

                        if (group.leader != null) group.leader.Tick(playerPos);
                    }
                }
            }
        }

        public void RegisterEnemy(EnemyBase enemy)
        {
            if (!activeEnemies.Contains(enemy))
            {
                activeEnemies.Add(enemy);
            }
        }

        public EnemyBase CreateRandomEnemy()
        {
            if (enemyDatas == null) return null;

            var randomIndex = Random.Range(0, enemyDatas.Enemies.Count);
            var enemyData = enemyDatas.Enemies[randomIndex];

            EnemyBase instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(enemyData.Prefab, enemyHolder);
            instance.Init();

            return instance;
        }

        public EnemyBase CreateRandomEnemy(Vector2 pos)
        {
            if (enemyDatas == null) return null;

            var randomIndex = Random.Range(0, enemyDatas.Enemies.Count);
            var enemyData = enemyDatas.Enemies[randomIndex];

            EnemyBase instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(enemyData.Prefab, pos);
            instance.transform.SetParent(enemyHolder);
            instance.Init();

            return instance;
        }

        public void CreateEnemyGroup(int totalEnemies)
        {
            List<EnemyBase> createdEnemies = new List<EnemyBase>();

            for (int i = 0; i < totalEnemies; i++)
            {
                var randomEnemy = enemyDatas.Enemies[Random.Range(0, enemyDatas.Enemies.Count)];
                EnemyBase instance = CreateRandomEnemy(
                    i == 0
                        ? WaveManager.Instance.GenerateRandomPos(WaveManager.Instance.SpawRadius, CharacterSystem.Instance.GetCurrentCharacterTransform().position)
                        : WaveManager.Instance.GenerateRandomPos(WaveManager.Instance.EnemyGroupRadius, createdEnemies[0].transform.position)
                );
                instance.Init();
                createdEnemies.Add(instance);
            }

            EnemyGroup newGroup = new EnemyGroup();

            for (int i = 0; i < createdEnemies.Count; i += WaveManager.Instance.EnemiesPerGroup)
            {
                int end = Mathf.Min(i + WaveManager.Instance.EnemiesPerGroup, createdEnemies.Count);
                

                for (int j = i + 1; j < end; j++)
                {
                    newGroup.followers.Add(createdEnemies[j]);
                    newGroup.allMembers.Add(createdEnemies[j]);
                }
                    
            }

            newGroup.leader = FindFastestEnemy(newGroup);
            if (newGroup.leader != null)
                newGroup.leader.SetAsLeader();

            newGroup.allMembers.Add(newGroup.leader);

            enemyGroups.Add(newGroup);

            activeLeaderEnemies.Add(newGroup.leader);
            newGroup.SetSubscribes(newGroup.allMembers);
        }

        public EnemyBase FindFastestEnemy(EnemyGroup enemyGroup)
        {
            EnemyBase fastestEnemy = null;

            foreach (EnemyBase enemy in enemyGroup.allMembers)
            {
                fastestEnemy = enemy;

                if (fastestEnemy.EnemyData.MoveSpeed < enemy.EnemyData.MoveSpeed)
                {
                    fastestEnemy = enemy;
                }
            }

            return fastestEnemy;
        }
    }
}

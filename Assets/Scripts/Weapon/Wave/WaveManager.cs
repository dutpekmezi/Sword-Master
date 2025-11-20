using System.Collections.Generic;
using UnityEngine;

namespace dutpekmezi
{
    public class WaveManager : BaseSystem
    {
        private EnemySystem enemySystem;
        private CharacterSystem characterSystem;

        public int EnemiesPerGroup { get; private set; }
        public float GroupSpawnRadius { get; private set; }
        public float GroupSpawnDeflection { get; private set; }
        public float EnemyGroupRadius { get; private set; }
        public float EnemyGroupDeflection { get; private set; }


        public int EnemiesPerWawe { get; private set; }
        public float WaweSpawnRadius { get; private set; }
        public float WaweSpawnDeflection { get; private set; }

        public WaveManager(
            EnemySystem enemySystem,
            CharacterSystem characterSystem,
            int enemiesPerWawe,
            int enemiesPerGroup,
            float groupSpawnRadius,
            float groupSpawnDeflection,
            float enemyGroupRadius,
            float enemyGroupDeflection,
            float waweSpawnRadius,
            float waweSpawnDeflection
        )
        {
            this.enemySystem = enemySystem;
            this.characterSystem = characterSystem;

            EnemiesPerGroup = enemiesPerGroup;
            GroupSpawnRadius = groupSpawnRadius;
            GroupSpawnDeflection = groupSpawnDeflection;
            EnemyGroupRadius = enemyGroupRadius;
            EnemyGroupDeflection = enemyGroupDeflection;

            EnemiesPerWawe = enemiesPerWawe;
            WaweSpawnRadius = waweSpawnRadius;
            WaweSpawnDeflection = waweSpawnDeflection;

            OnInitialize();
        }

        protected override void OnInitialize()
        {
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GenerateEnemyWawe(EnemiesPerWawe);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                
            }
        }

        public void GenerateEnemyWawe(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var randomPos = GenerateRandomPos(WaweSpawnRadius, WaweSpawnDeflection, CharacterSystem.Instance.GetCurrentCharacter().transform.position);

                var instance = enemySystem.CreateRandomEnemy(randomPos);
            }
        }

        public void GenerateEnemyGroup(int totalEnemies)
        {
            List<EnemyBase> createdEnemies = new List<EnemyBase>();


            for (int i = 0; i < totalEnemies; i++)
            {
                var randomCenter = GenerateRandomPos(
                    GroupSpawnRadius,
                    GroupSpawnDeflection,
                    (Vector2)characterSystem.GetCurrentCharacter().transform.position);

                EnemyBase instance = enemySystem.CreateRandomEnemy(
                    GenerateRandomPos(
                        EnemyGroupRadius,
                        EnemyGroupDeflection,
                        randomCenter));

                createdEnemies.Add(instance);
            }

            EnemyGroup newGroup = new EnemyGroup();

            for (int i = 0; i < createdEnemies.Count; i += EnemiesPerGroup)
            {
                int end = Mathf.Min(i + EnemiesPerGroup, createdEnemies.Count);

                for (int j = i + 1; j < end; j++)
                {
                    newGroup.members.Add(createdEnemies[j]);
                }
            }

            enemySystem.EnemyGroups.Add(newGroup);
            newGroup.SetSubscribes(newGroup.members);
        }

        public Vector2 GenerateRandomPos(float radius, float deflection, Vector2 center)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = radius + Random.Range(-deflection, deflection);

            float x = center.x + Mathf.Cos(angle) * distance;
            float y = center.y + Mathf.Sin(angle) * distance;

            return new Vector2(x, y);
        }
    }
}
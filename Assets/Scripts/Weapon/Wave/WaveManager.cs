using System.Collections.Generic;
using UnityEngine;
using Utils.LogicTimer;

namespace dutpekmezi
{
    public enum WaveState
    {
        PreChaos,
        Chaos,
        Pause
    }

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

        public float PreChaosDuration { get; private set; }
        public float PreChaosWaweSpawnRate { get; private set; }
        public float PreChaosGroupSpawnRate { get; private set; }
        public float CurrentPreChaosTime => currentPreChaosTime;

        private WaveState currentWaveState = WaveState.PreChaos;
        private float currentPreChaosTime = 0f;
        private float waveSpawnTimer = 0f;
        private float groupSpawnTimer = 0f;

        public static WaveManager Instance { get; private set; }

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
            float waweSpawnDeflection,
            float preChaosDuration,
            float preChaosWaweSpawnRate,
            float preChaosGroupSpawnRate
        )
        {
            Instance = this;

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

            PreChaosDuration = preChaosDuration;
            PreChaosGroupSpawnRate = preChaosGroupSpawnRate;
            PreChaosWaweSpawnRate = preChaosWaweSpawnRate;

            OnInitialize();
        }

        protected override void OnInitialize()
        {
            currentWaveState = WaveState.PreChaos;
            currentPreChaosTime = PreChaosDuration;
            waveSpawnTimer = 0f;
            groupSpawnTimer = 0f;
        }

        public override void Tick()
        {
            if (currentWaveState == WaveState.PreChaos)
            {
                HandlePreChaosSpawning();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                GenerateEnemyWawe(EnemiesPerWawe);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                GenerateEnemyGroup(EnemiesPerGroup);
            }
        }

        private void HandlePreChaosSpawning()
        {
            float dt = LogicTimer.FixedDelta;

            currentPreChaosTime -= dt;
            waveSpawnTimer += dt;
            groupSpawnTimer += dt;

            if (currentPreChaosTime >= PreChaosDuration)
            {
                currentWaveState = WaveState.Chaos;
                return;
            }

            if (waveSpawnTimer >= PreChaosWaweSpawnRate && PreChaosWaweSpawnRate > 0)
            {
                GenerateEnemyWawe(EnemiesPerWawe);
                waveSpawnTimer = 0f;
            }

            if (groupSpawnTimer >= PreChaosGroupSpawnRate && PreChaosGroupSpawnRate > 0)
            {
                GenerateEnemyGroup(EnemiesPerGroup);
                groupSpawnTimer = 0f;
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


            for (int i = 0; i < createdEnemies.Count; i++)
            {
                newGroup.members.Add(createdEnemies[i]);
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
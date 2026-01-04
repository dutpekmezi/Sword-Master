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
        private WaveConfig waveConfig;
        public float CurrentPreChaosTime => currentPreChaosTime;

        private WaveState currentWaveState = WaveState.PreChaos;
        private float currentPreChaosTime = 0f;
        private float waveSpawnTimer = 0f;
        private float groupSpawnTimer = 0f;

        public static WaveManager Instance { get; private set; }
        public WaveConfig WaveConfig => waveConfig;

        public WaveManager(EnemySystem enemySystem,
            CharacterSystem characterSystem,
            WaveConfig waveConfig
        )
        {
            Instance = this;

            this.waveConfig = waveConfig;

            this.enemySystem = enemySystem;
            this.characterSystem = characterSystem;

            OnInitialize();
        }

        protected override void OnInitialize()
        {
            currentWaveState = WaveState.PreChaos;
            currentPreChaosTime = waveConfig.preChaosDuration;
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
                GenerateEnemyWawe(waveConfig.enemiesPerWave);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                GenerateEnemyGroup(waveConfig.enemiesPerGroup);
            }
        }

        private void HandlePreChaosSpawning()
        {
            float dt = LogicTimer.FixedDelta;

            currentPreChaosTime -= dt;
            waveSpawnTimer += dt;
            groupSpawnTimer += dt;

            if (currentPreChaosTime >= waveConfig.preChaosDuration)
            {
                currentWaveState = WaveState.Chaos;
                return;
            }

            if (waveSpawnTimer >= waveConfig.preChaosWaveSpawnRate && waveConfig.preChaosWaveSpawnRate > 0)
            {
                GenerateStatStatues(waveConfig.statuesPerWave);
                GenerateIndicatorsForStatStatues();
                GenerateEnemyWawe(waveConfig.enemiesPerWave + characterSystem.GetCurrentCharacter().CurrentLevel - 1);
                waveSpawnTimer = 0f;
            }

            if (groupSpawnTimer >= waveConfig.preChaosGroupSpawnRate && waveConfig.preChaosGroupSpawnRate > 0)
            {
                GenerateEnemyGroup(waveConfig.enemiesPerGroup);
                groupSpawnTimer = 0f;
            }
        }

        public void GenerateEnemyWawe(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var randomPos = GenerateRandomPos(waveConfig.waveSpawnRadius, waveConfig.waveSpawnDeflection, CharacterSystem.Instance.GetCurrentCharacter().transform.position);

                enemySystem.CreateRandomEnemy(randomPos);
            }
        }

        public void GenerateEnemyGroup(int totalEnemies)
        {
            List<EnemyBase> createdEnemies = new List<EnemyBase>();

            for (int i = 0; i < totalEnemies; i++)
            {
                var randomCenter = GenerateRandomPos(
                    waveConfig.groupSpawnRadius,
                    waveConfig.groupSpawnDeflection,
                    (Vector2)characterSystem.GetCurrentCharacter().transform.position);

                EnemyBase instance = enemySystem.CreateRandomEnemy(
                    GenerateRandomPos(
                        waveConfig.enemyGroupRadius,
                        waveConfig.enemyGroupDeflection,
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

        public void GenerateStatStatues(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (StatueManager.Instance.ActiveStatStatues.Count >= waveConfig.maxStatStatue || StatueManager.Instance.ActiveStatues.Count >= waveConfig.maxStatue) return;

                var statue = StatueManager.Instance.CreateStatStatue();

                var randomPos = GenerateRandomPos(waveConfig.statueSpawnRadius, waveConfig.statueSpawnDeflection, characterSystem.GetCurrentCharacter().transform.position);
                statue.transform.position = randomPos;
            }
        }

        public void GenerateWeaponStatues(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (StatueManager.Instance.ActiveWeaponStatues.Count >= waveConfig.maxWeaponStatue || StatueManager.Instance.ActiveStatues.Count >= waveConfig.maxStatue) return;

                var statue = StatueManager.Instance.CreateWeaponStatue();

                var randomPos = GenerateRandomPos(waveConfig.statueSpawnRadius, waveConfig.statueSpawnDeflection, characterSystem.GetCurrentCharacter().transform.position);
                statue.transform.position = randomPos;
            }
        }

        public void GenerateIndicatorsForStatStatues()
        {
            var targetlist = StatueManager.Instance.GetStatStatuesTransform();

            IndicatorManager.Instance.CreateTargetIndicators(targetlist, characterSystem.GetCurrentCharacter().transform);
        }

        public void GenerateIndicatorsForWeaponStatues()
        {
            var targetlist = StatueManager.Instance.GetWeaponStatuesTransform();

            IndicatorManager.Instance.CreateTargetIndicators(targetlist, characterSystem.GetCurrentCharacter().transform);
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

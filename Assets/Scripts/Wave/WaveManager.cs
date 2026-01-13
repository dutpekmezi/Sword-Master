using System.Collections.Generic;
using UnityEngine;
using Utils.LogicTimer;
using Utils.Signal;

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
        public float CurrentTimer => currentTimer;

        private WaveState currentWaveState = WaveState.PreChaos;
        private float currentTimer = 0f;
        private float waveSpawnTimer = 0f;
        private float groupSpawnTimer = 0f;
        private readonly List<StatModifier> waveModifiers = new List<StatModifier>();

        public static WaveManager Instance { get; private set; }
        public WaveConfig WaveConfig => waveConfig;
        public WaveState CurrentWaveState => currentWaveState;

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
            currentTimer = waveConfig.preChaosDuration;
            waveSpawnTimer = 0f;
            groupSpawnTimer = 0f;
            waveModifiers.Clear();

            SignalBus.Get<StatSystem.OnStatSelected>().Subscribe(ApplyWaveModifier);
        }

        public override void Tick()
        {
            if (currentWaveState == WaveState.PreChaos)
            {
                HandlePreChaosSpawning();
            }
            else if(currentWaveState == WaveState.Chaos)
            {
                HandleChaosSpawning();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (waveConfig.enemyWaveConfig != null)
                {
                    GenerateEnemyWawe(waveConfig.enemyWaveConfig.enemiesPerWave);
                }
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                if (waveConfig.enemyWaveConfig != null)
                {
                    GenerateEnemyGroup(waveConfig.enemyWaveConfig.enemiesPerGroup);
                }
            }
        }

        private void HandlePreChaosSpawning()
        {
            float dt = LogicTimer.FixedDelta;
            var enemyConfig = waveConfig.enemyWaveConfig;
            var statueConfig = waveConfig.statueSpawnConfig;
            var chestConfig = waveConfig.chestSpawnConfig;

            currentTimer -= dt;
            waveSpawnTimer += dt;
            groupSpawnTimer += dt;

            if (currentTimer <= 0)
            {
                EnterChaosState();
                return;
            }

            float adjustedWaveSpawnRate = GetAdjustedSpawnInterval(enemyConfig != null ? enemyConfig.preChaosWaveSpawnRate : 0f);
            if (waveSpawnTimer >= adjustedWaveSpawnRate && adjustedWaveSpawnRate > 0)
            {
                if (statueConfig != null)
                {
                    GenerateStatStatues(statueConfig.statuesPerWave);
                    GenerateIndicatorsForStatStatues();
                }

                if (chestConfig != null)
                {
                    GenerateChests(chestConfig.chestsPerWave);
                    GenerateIndicatorsForChests();
                }

                if (enemyConfig != null)
                {
                    var enemyCount = enemyConfig.enemiesPerWave + characterSystem.GetCurrentCharacter().CurrentLevel - 1;
                    GenerateEnemyWawe(GetAdjustedEnemyCount(enemyCount));
                }
                waveSpawnTimer = 0f;
            }

            float adjustedGroupSpawnRate = GetAdjustedSpawnInterval(enemyConfig != null ? enemyConfig.preChaosGroupSpawnRate : 0f);
            if (groupSpawnTimer >= adjustedGroupSpawnRate && adjustedGroupSpawnRate > 0)
            {
                if (enemyConfig != null)
                {
                    GenerateEnemyGroup(GetAdjustedEnemyCount(enemyConfig.enemiesPerGroup));
                }
                groupSpawnTimer = 0f;
            }
        }

        private void HandleChaosSpawning()
        {
            float dt = LogicTimer.FixedDelta;
            var enemyConfig = waveConfig.enemyWaveConfig;

            currentTimer += dt;
            waveSpawnTimer += dt;
            groupSpawnTimer += dt;

            float chaosWaveSpawnRate = GetAdjustedSpawnInterval(enemyConfig != null ? enemyConfig.chaosWaveSpawnRate : 0f);
            if (waveSpawnTimer >= chaosWaveSpawnRate && chaosWaveSpawnRate > 0)
            {
                if (enemyConfig != null)
                {
                    GenerateEnemyWawe(GetAdjustedEnemyCount(enemyConfig.chaosEnemiesPerWave));
                }
                waveSpawnTimer = 0f;
            }

            float chaosGroupSpawnRate = GetAdjustedSpawnInterval(enemyConfig != null ? enemyConfig.chaosGroupSpawnRate : 0f);
            if (groupSpawnTimer >= chaosGroupSpawnRate && chaosGroupSpawnRate > 0)
            {
                if (enemyConfig != null)
                {
                    GenerateEnemyGroup(GetAdjustedEnemyCount(enemyConfig.chaosEnemiesPerGroup));
                }
                groupSpawnTimer = 0f;
            }
        }

        private void EnterChaosState()
        {
            currentWaveState = WaveState.Chaos;
            currentTimer = 0f;
            waveSpawnTimer = 0f;
            groupSpawnTimer = 0f;
            GenerateWeaponStatues();
        }

        private float GetAdjustedSpawnInterval(float baseInterval)
        {
            if (baseInterval <= 0)
            {
                return 0f;
            }

            return baseInterval / (1f + GetDifficulty());
        }

        private int GetAdjustedEnemyCount(int baseCount)
        {
            if (baseCount <= 0)
            {
                return 0;
            }

            float difficultyScale = 1f + GetDifficulty();
            return Mathf.Max(1, Mathf.CeilToInt(baseCount * difficultyScale));
        }

        private float GetDifficulty()
        {
            float finalValue = GetBaseDifficulty();

            waveModifiers.Sort((a, b) => a.Operation.CompareTo(b.Operation));

            float percentSum = 0f;
            for (int i = 0; i < waveModifiers.Count; i++)
            {
                StatModifier modifier = waveModifiers[i];

                if (modifier.Type != StatType.Difficulty) continue;

                if (modifier.Operation == ModifierOperation.FlatAdd)
                {
                    finalValue += modifier.Value;
                }
                else if (modifier.Operation == ModifierOperation.PercentMultiply)
                {
                    percentSum += modifier.Value;
                }
            }

            finalValue *= 1f + percentSum;

            return Mathf.Max(0f, finalValue);
        }

        public void GenerateEnemyWawe(int count)
        {
            var enemyConfig = waveConfig.enemyWaveConfig;
            if (enemyConfig == null)
            {
                return;
            }

            for (int i = 0; i < count; i++)
            {
                var randomPos = GenerateRandomPos(enemyConfig.waveSpawnRadius, enemyConfig.waveSpawnDeflection, CharacterSystem.Instance.GetCurrentCharacter().transform.position);

                enemySystem.CreateRandomEnemy(randomPos);
            }
        }

        public void GenerateEnemyGroup(int totalEnemies)
        {
            var enemyConfig = waveConfig.enemyWaveConfig;
            if (enemyConfig == null)
            {
                return;
            }

            List<EnemyBase> createdEnemies = new List<EnemyBase>();

            for (int i = 0; i < totalEnemies; i++)
            {
                var randomCenter = GenerateRandomPos(
                    enemyConfig.groupSpawnRadius,
                    enemyConfig.groupSpawnDeflection,
                    (Vector2)characterSystem.GetCurrentCharacter().transform.position);

                EnemyBase instance = enemySystem.CreateRandomEnemy(
                    GenerateRandomPos(
                        enemyConfig.enemyGroupRadius,
                        enemyConfig.enemyGroupDeflection,
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
            var statueConfig = waveConfig.statueSpawnConfig;
            if (statueConfig == null)
            {
                return;
            }

            var characterPosition = (Vector2)characterSystem.GetCurrentCharacter().transform.position;
            List<Vector2> occupiedPositions = GetCurrentOccupiedPositions();

            for (int i = 0; i < count; i++)
            {
                if (StatueManager.Instance.ActiveStatStatues.Count >= statueConfig.maxStatStatue || StatueManager.Instance.ActiveStatues.Count >= statueConfig.maxStatue) return;

                if (!TryGetStatueSpawnPosition(characterPosition, occupiedPositions, statueConfig, out var spawnPosition))
                {
                    continue;
                }

                var statue = StatueManager.Instance.CreateStatStatue();
                statue.transform.position = spawnPosition;
                occupiedPositions.Add(spawnPosition);
            }
        }

        public void GenerateWeaponStatues(int count = 1)
        {
            var statueConfig = waveConfig.statueSpawnConfig;
            if (statueConfig == null)
            {
                return;
            }

            var characterPosition = (Vector2)characterSystem.GetCurrentCharacter().transform.position;
            List<Vector2> occupiedPositions = GetCurrentOccupiedPositions();

            for (int i = 0; i < count; i++)
            {
                if (StatueManager.Instance.ActiveWeaponStatues.Count >= statueConfig.maxWeaponStatue || StatueManager.Instance.ActiveStatues.Count >= statueConfig.maxStatue) return;

                if (!TryGetStatueSpawnPosition(characterPosition, occupiedPositions, statueConfig, out var spawnPosition))
                {
                    continue;
                }

                var statue = StatueManager.Instance.CreateWeaponStatue();
                statue.transform.position = spawnPosition;
                occupiedPositions.Add(spawnPosition);
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

        public void GenerateChests(int count)
        {
            var chestConfig = waveConfig.chestSpawnConfig;
            if (chestConfig == null || ChestSystem.Instance == null)
            {
                return;
            }

            var characterPosition = (Vector2)characterSystem.GetCurrentCharacter().transform.position;
            List<Vector2> occupiedPositions = GetCurrentOccupiedPositions();

            for (int i = 0; i < count; i++)
            {
                if (ChestSystem.Instance.ActiveChests.Count >= chestConfig.maxChest) return;

                if (!TryGetChestSpawnPosition(characterPosition, occupiedPositions, chestConfig, out var spawnPosition))
                {
                    continue;
                }

                ChestSystem.Instance.CreateRandomChest(spawnPosition);
                occupiedPositions.Add(spawnPosition);
            }
        }

        public void GenerateIndicatorsForChests()
        {
            if (ChestSystem.Instance == null)
            {
                return;
            }

            var targetList = ChestSystem.Instance.GetChestsTransform();
            var character = characterSystem.GetCurrentCharacter();

            if (character == null)
            {
                return;
            }

            IndicatorManager.Instance.CreateTargetIndicators(targetList, character.transform, IndicatorManager.Instance.IndicatorConfig.chestIndicator);
        }

        public Vector2 GenerateRandomPos(float radius, float deflection, Vector2 center)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = radius + Random.Range(-deflection, deflection);

            float x = center.x + Mathf.Cos(angle) * distance;
            float y = center.y + Mathf.Sin(angle) * distance;

            return new Vector2(x, y);
        }

        private bool TryGetStatueSpawnPosition(Vector2 center, List<Vector2> occupiedPositions, StatueSpawnConfig statueConfig, out Vector2 spawnPosition)
        {
            const int maxAttempts = 20;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var candidate = GenerateRandomPos(statueConfig.statueSpawnRadius, statueConfig.statueSpawnDeflection, center);

                if (IsPositionFarEnough(candidate, occupiedPositions, statueConfig.statueMinimumDistance))
                {
                    spawnPosition = candidate;
                    return true;
                }
            }

            spawnPosition = Vector2.zero;
            return false;
        }

        private bool IsPositionFarEnough(Vector2 candidate, List<Vector2> occupiedPositions, float minimumDistance)
        {
            foreach (var position in occupiedPositions)
            {
                if (Vector2.Distance(candidate, position) < minimumDistance)
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryGetChestSpawnPosition(Vector2 center, List<Vector2> occupiedPositions, ChestSpawnConfig chestConfig, out Vector2 spawnPosition)
        {
            const int maxAttempts = 20;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var candidate = GenerateRandomPos(chestConfig.chestSpawnRadius, chestConfig.chestSpawnDeflection, center);

                if (IsPositionFarEnough(candidate, occupiedPositions, chestConfig.chestMinimumDistance))
                {
                    spawnPosition = candidate;
                    return true;
                }
            }

            spawnPosition = Vector2.zero;
            return false;
        }

        private List<Vector2> GetCurrentStatuePositions()
        {
            List<Vector2> positions = new List<Vector2>();

            foreach (var statue in StatueManager.Instance.ActiveStatues)
            {
                positions.Add(statue.transform.position);
            }

            return positions;
        }

        private List<Vector2> GetCurrentChestPositions()
        {
            List<Vector2> positions = new List<Vector2>();

            if (ChestSystem.Instance == null)
            {
                return positions;
            }

            foreach (var chest in ChestSystem.Instance.ActiveChests)
            {
                positions.Add(chest.transform.position);
            }

            return positions;
        }

        private List<Vector2> GetCurrentOccupiedPositions()
        {
            List<Vector2> positions = GetCurrentStatuePositions();

            if (ChestSystem.Instance == null)
            {
                return positions;
            }

            foreach (var chest in ChestSystem.Instance.ActiveChests)
            {
                positions.Add(chest.transform.position);
            }

            return positions;
        }

        private float GetBaseDifficulty()
        {
            var currentCharacter = characterSystem.GetCurrentCharacter();

            if (currentCharacter == null)
            {
                return 0f;
            }

            return Mathf.Max(0f, currentCharacter.GetStatValue(StatType.Difficulty));
        }

        private void ApplyWaveModifier(StatModifier modifier)
        {
            if (modifier == null) return;
            if (modifier.Target != StatTarget.Wave) return;
            if (modifier.Type != StatType.Difficulty) return;

            waveModifiers.Add(modifier);
        }

        protected override void OnDispose()
        {
            SignalBus.Get<StatSystem.OnStatSelected>().Unsubscribe(ApplyWaveModifier);
        }
    }
}

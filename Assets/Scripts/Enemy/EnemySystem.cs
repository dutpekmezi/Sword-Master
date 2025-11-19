using Dutpekmezi.Services.PoolService;
using System.Collections.Generic;
using Utils.Signal;
using UnityEngine;

namespace dutpekmezi
{
    public class EnemySystem : BaseSystem
    {
        private readonly EnemyDatas enemyDatas;

        private readonly List<EnemyBase> activeEnemies = new();
        private readonly List<EnemyGroup> enemyGroups = new();

        public EnemyDatas EnemyDatas => enemyDatas;
        public List<EnemyBase> ActiveEnemies => activeEnemies;
        public List<EnemyGroup> EnemyGroups => enemyGroups;

        public static EnemySystem Instance { get; private set; }

        public class OnEnemyDiedSignal : Signal<EnemyBase> { }
        public class OnEnemySpawnedSignal : Signal<EnemyBase> { }

        public EnemySystem(EnemyDatas enemyDatas)
        {
            Instance = this;
            this.enemyDatas = enemyDatas;
            OnInitialize();
        }

        protected override void OnInitialize()
        {
            SignalBus.Get<CharacterSystem.OnCharacterSpawnedSignal>()
                     .Subscribe(OnCharacterSpawned);
        }

        private void OnCharacterSpawned(CharacterBase character)
        {
        }

        public EnemyBase CreateRandomEnemy()
        {
            if (enemyDatas == null || enemyDatas.Enemies.Count == 0)
                return null;

            int idx = Random.Range(0, enemyDatas.Enemies.Count);
            EnemyData data = enemyDatas.Enemies[idx];

            var go = ObjectPoolManager.SpawnObject(data.Prefab, Vector2.zero);
            var enemy = go.GetComponent<EnemyBase>();

            enemy.Initialize();
            RegisterEnemy(enemy);

            SignalBus.Get<OnEnemySpawnedSignal>().Invoke(enemy);

            return enemy;
        }

        public EnemyBase CreateRandomEnemy(Vector2 pos)
        {
            if (enemyDatas == null || enemyDatas.Enemies.Count == 0)
                return null;

            int idx = Random.Range(0, enemyDatas.Enemies.Count);
            EnemyData data = enemyDatas.Enemies[idx];

            var go = ObjectPoolManager.SpawnObject(data.Prefab, pos);
            var enemy = go.GetComponent<EnemyBase>();

            enemy.Initialize();
            RegisterEnemy(enemy);

            SignalBus.Get<OnEnemySpawnedSignal>().Invoke(enemy);

            return enemy;
        }

        public EnemyData GetRandomEnemyData()
        {
            var randomIndex = Random.Range(0, enemyDatas.Enemies.Count);

            var randomEnemy = enemyDatas.Enemies[randomIndex];

            if (randomEnemy != null) return randomEnemy;

            return null;
        }


        public override void Tick()
        {
            var player = CharacterSystem.Instance.GetCurrentCharacter();
            if (player == null)
                return;

            var playerPos = player.transform.position;

            foreach (var enemy in activeEnemies)
                enemy.Tick(playerPos);
        }

        public EnemyBase RegisterEnemy(EnemyBase enemy)
        {
            if (!activeEnemies.Contains(enemy))
                activeEnemies.Add(enemy);

            return enemy;
        }

        protected override void OnDispose()
        {
            foreach (var e in activeEnemies)
                ObjectPoolManager.DeSpawn(e.gameObject);

            activeEnemies.Clear();
            enemyGroups.Clear();
        }
    }
}
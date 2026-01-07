using Dutpekmezi.Services.PoolService;
using System.Collections.Generic;
using Utils.Signal;
using UnityEngine;

namespace dutpekmezi
{
    public class EnemySystem : EntitySystem
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

        public EnemyBase CreateRandomEnemy(Vector2 pos)
        {
            EnemyData data = GetRandomData(enemyDatas?.Enemies);
            if (data == null)
                return null;

            EnemyBase enemy = (EnemyBase)ObjectPoolManager.SpawnObject(data.Prefab, pos);

            enemy.Initialize();
            RegisterEnemy(enemy);

            SignalBus.Get<OnEnemySpawnedSignal>().Invoke(enemy);

            return enemy;
        }

        public EnemyBase CreateRandomEnemy()
        {
            EnemyData data = GetRandomData(enemyDatas?.Enemies);
            if (data == null)
                return null;

            EnemyBase enemy = (EnemyBase)ObjectPoolManager.SpawnObject(data.Prefab, Vector2.zero);

            enemy.Initialize();
            RegisterEnemy(enemy);

            SignalBus.Get<OnEnemySpawnedSignal>().Invoke(enemy);

            return enemy;
        }

        public EnemyData GetRandomEnemyData()
        {
            return GetRandomData(enemyDatas?.Enemies);
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
            activeEnemies.Clear();
            enemyGroups.Clear();
        }
    }
}

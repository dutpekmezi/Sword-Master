using Dutpekmezi.Services.PoolService;
using System.Collections.Generic;
using UnityEngine;
using Utils.Signal;

namespace dutpekmezi
{
    public class ChestSystem : EntitySystem
    {
        private readonly ChestDatas chestDatas;

        private readonly List<ChestBase> activeChests = new();
        public IReadOnlyList<ChestBase> ActiveChests => activeChests;

        public static ChestSystem Instance { get; private set; }

        public ChestSystem(ChestDatas chestDatas)
        {
            Instance = this;
            this.chestDatas = chestDatas;
            OnInitialize();
        }

        protected override void OnInitialize()
        {
            SignalBus.Get<OnChestOpenedSignal>().Subscribe(OnChestOpened);
        }

        public override void Tick()
        {
        }

        public ChestBase CreateRandomChest(Vector2 position)
        {
            var data = GetRandomData(chestDatas?.Chests);
            return CreateChest(data, position);
        }

        public ChestBase CreateChest(ChestData data, Vector2 position)
        {
            if (data == null)
                return null;

            var instance = ObjectPoolManager.SpawnObject(data.Prefab, position);
            instance.Initialize();

            var chest = instance.GetComponent<ChestBase>();
            RegisterChest(chest);

            SignalBus.Get<OnChestSpawnedSignal>().Invoke(chest);

            return chest;
        }

        public List<Transform> GetChestsTransform()
        {
            var transformList = new List<Transform>();

            foreach (var chest in activeChests)
            {
                transformList.Add(chest.transform);
            }

            return transformList;
        }

        private void RegisterChest(ChestBase chest)
        {
            if (chest == null)
                return;

            if (!activeChests.Contains(chest))
                activeChests.Add(chest);
        }

        protected override void OnDispose()
        {
            SignalBus.Get<OnChestOpenedSignal>().Unsubscribe(OnChestOpened);
            activeChests.Clear();
        }

        private void OnChestOpened(ChestBase chest)
        {
            if (chest == null)
                return;

            activeChests.Remove(chest);
        }

        public class OnChestSpawnedSignal : Signal<ChestBase> { }
        public class OnChestOpenedSignal : Signal<ChestBase> { }
    }
}

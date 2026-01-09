using dutpekmezi;
using System.Collections.Generic;
using UnityEngine;
using Utils.Signal;
using Dutpekmezi.Services.PoolService;

public class IndicatorManager : BaseSystem
{
    private IndicatorConfig indicatorConfig;

    private Dictionary<TargetIndicator, Transform> targetIndicators = new Dictionary<TargetIndicator, Transform>();
    private List<TargetIndicator> indicatorsToDispose = new List<TargetIndicator>();

    public IndicatorConfig IndicatorConfig => indicatorConfig;

    public static IndicatorManager Instance { get; private set; }

    public IndicatorManager(IndicatorConfig indicatorConfig)
    {
        Instance = this;

        this.indicatorConfig = indicatorConfig;

        OnInitialize();
    }

    protected override void OnInitialize()
    {
        SignalBus.Get<StatueManager.OnStatueDispose>().Subscribe(OnTargetDestroyed);
        SignalBus.Get<ChestSystem.OnChestSpawnedSignal>().Subscribe(OnChestSpawned);
        SignalBus.Get<ChestSystem.OnChestOpenedSignal>().Subscribe(OnChestOpened);
    }

    public override void Tick()
    {
        foreach (var indicatorPair in targetIndicators)
        {
            TargetIndicator indicator = indicatorPair.Key;

            indicator.Tick();
        }
    }

    protected override void OnDispose()
    {
        SignalBus.Get<StatueManager.OnStatueDispose>().Unsubscribe(OnTargetDestroyed);
        SignalBus.Get<ChestSystem.OnChestSpawnedSignal>().Unsubscribe(OnChestSpawned);
        SignalBus.Get<ChestSystem.OnChestOpenedSignal>().Unsubscribe(OnChestOpened);
    }

    public TargetIndicator CreateTargetIndicator(Transform target, Transform center)
    {
        return CreateIndicator(indicatorConfig.statueIndicator, target, center);
    }

    public List<TargetIndicator> CreateTargetIndicators(List<Transform> targetList, List<Transform> centerList)
    {
        var createdIndicators = new List<TargetIndicator>();

        for (int i = 0; i < targetList.Count; i++)
        {
            var instance = CreateTargetIndicator(targetList[i], centerList[i]);

            if (instance != null)
            {
                createdIndicators.Add(instance);
            }
        }

        return createdIndicators;
    }

    public List<TargetIndicator> CreateTargetIndicators(List<Transform> targetList, Transform center)
    {
        return CreateTargetIndicators(targetList, center, indicatorConfig.statueIndicator);
    }

    public List<TargetIndicator> CreateTargetIndicators(List<Transform> targetList, Transform center, TargetIndicator indicatorPrefab)
    {
        var createdIndicators = new List<TargetIndicator>();

        for (int i = 0; i < targetList.Count; i++)
        {
            var instance = CreateIndicator(indicatorPrefab, targetList[i], center);

            if (instance != null)
            {
                createdIndicators.Add(instance);
            }
        }

        return createdIndicators;
    }

    private void OnTargetDestroyed(Transform destroyedTarget)
    {
        DisposeIndicatorsForTarget(destroyedTarget);
    }

    private void OnChestSpawned(ChestBase chest)
    {
        if (chest == null)
        {
            return;
        }

        var character = CharacterSystem.Instance?.GetCurrentCharacter();
        if (character == null)
        {
            return;
        }

        CreateIndicator(indicatorConfig.chestIndicator, chest.transform, character.transform);
    }

    private void OnChestOpened(ChestBase chest)
    {
        if (chest == null)
        {
            return;
        }

        DisposeIndicatorsForTarget(chest.transform);
    }

    private TargetIndicator CreateIndicator(TargetIndicator indicatorPrefab, Transform target, Transform center)
    {
        if (indicatorPrefab == null || target == null || center == null)
        {
            return null;
        }

        if (targetIndicators.ContainsValue(target))
        {
            return null;
        }

        var instance = ObjectPoolManager.SpawnObject(indicatorPrefab, Vector2.zero);
        instance.Init(target, center);

        targetIndicators.Add(instance, target);

        return instance;
    }

    private void DisposeIndicatorsForTarget(Transform destroyedTarget)
    {
        indicatorsToDispose = new List<TargetIndicator>();

        TargetIndicator[] keys = new TargetIndicator[targetIndicators.Count];
        targetIndicators.Keys.CopyTo(keys, 0);

        for (int i = 0; i < keys.Length; i++)
        {
            TargetIndicator indicator = keys[i];

            if (targetIndicators.ContainsKey(indicator) && targetIndicators[indicator] == destroyedTarget)
            {
                indicatorsToDispose.Add(indicator);
            }
        }

        foreach (var indicator in indicatorsToDispose)
        {
            ObjectPoolManager.DeSpawn(indicator.gameObject);

            targetIndicators.Remove(indicator);
        }
    }
}

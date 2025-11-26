using dutpekmezi;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Utils.Signal;

public class IndicatorManager : BaseSystem
{
    private IndicatorConfig indicatorConfig;

    private Dictionary<TargetIndicator, Transform> targetIndicators = new Dictionary<TargetIndicator, Transform>();

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
    }

    public override void Tick()
    {
        List<TargetIndicator> indicatorsToRemove = new List<TargetIndicator>();

        foreach (var indicatorPair in targetIndicators)
        {
            TargetIndicator indicator = indicatorPair.Key;

            if (indicator == null || !indicator.gameObject.activeInHierarchy)
            {
                indicatorsToRemove.Add(indicator);
            }
            else
            {
                indicator.Tick();
            }
        }

        foreach (var indicator in indicatorsToRemove)
        {
            targetIndicators.Remove(indicator);
        }
    }

    protected override void OnDispose()
    {
        SignalBus.Get<StatueManager.OnStatueDispose>().Unsubscribe(OnTargetDestroyed);
    }

    public TargetIndicator CreateTargetIndicator(Transform target, Transform center)
    {
        var indicator = indicatorConfig.targetIndicator;

        var instance = Dutpekmezi.Services.PoolService.ObjectPoolManager.SpawnObject(indicator, Vector2.zero);
        instance.Init(target, center);

        targetIndicators.Add(instance, target);

        return instance;
    }

    public List<TargetIndicator> CreateTargetIndicators(List<Transform> targetList, List<Transform> centerList)
    {
        var createdIndicators = new List<TargetIndicator>();

        for (int i = 0; i < targetList.Count; i++)
        {
            var instance = CreateTargetIndicator(targetList[i], centerList[i]);

            createdIndicators.Add(instance);
        }

        return createdIndicators;
    }

    public List<TargetIndicator> CreateTargetIndicators(List<Transform> targetList, Transform center)
    {
        var createdIndicators = new List<TargetIndicator>();

        for (int i = 0; i < targetList.Count; i++)
        {
            var instance = CreateTargetIndicator(targetList[i], center);

            createdIndicators.Add(instance);
        }

        return createdIndicators;
    }

    private void OnTargetDestroyed(Transform destroyedTarget)
    {
        List<TargetIndicator> indicatorsToDispose = new List<TargetIndicator>();

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
            Dutpekmezi.Services.PoolService.ObjectPoolManager.DeSpawn(indicator.gameObject);

            targetIndicators.Remove(indicator);
        }
    }
}
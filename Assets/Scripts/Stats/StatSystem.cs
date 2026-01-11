using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Utils.Signal;

namespace dutpekmezi
{
    public class StatSystem : BaseSystem
    {
        private readonly StatConfigData _statConfigData;

        public StatConfigData StatConfigData => _statConfigData;

        public static StatSystem Instance { get; private set; }

        public StatSystem(StatConfigData statConfigData)
        {
            Instance = this;

            _statConfigData = statConfigData;

            OnInitialize();
        }

        protected override void OnInitialize()
        {
            if (_statConfigData != null)
            {
                _statConfigData.InitializeLookup();
            }
        }

        public StatConfig GetStatConfig(StatType statType)
        {
            if (_statConfigData == null)
            {
                return new StatConfig { Type = statType, Color = Color.white, Icon = null };
            }

            return _statConfigData.GetConfig(statType);
        }

        public StatType GetRandomStatType(List<StatType> availableTypes)
        {
            if (availableTypes == null || availableTypes.Count == 0)
            {
                var allTypes = System.Enum.GetValues(typeof(StatType)).Cast<StatType>().ToList();
                return allTypes[Random.Range(0, allTypes.Count)];
            }

            return availableTypes[Random.Range(0, availableTypes.Count)];
        }

        public StatModifier CreateRandomModifier(StatType type, float scaleFactor = 1, object source = null)
        {
            StatConfig config = GetStatConfig(type);

            float value;
            ModifierOperation operation;
            StatTarget target = config.Target;

            if (config.FlatAddChance > 0 && Random.value < config.FlatAddChance)
            {
                operation = ModifierOperation.FlatAdd;
                value = config.DirectValue > 0f ? config.DirectValue : config.BaseFlatValue + (config.BaseFlatValuePerLevel * scaleFactor);
            }
            else
            {
                operation = config.DefaultOperation;
                value = config.BasePercentValuePerLevel * scaleFactor;

                if (type == StatType.CooldownReduction)
                {
                    value += 0.02f;
                }
            }

            return new StatModifier(value, operation, type, source, target);
        }

        public StatModifier CreateModifier(StatType type, float value, object source = null)
        {
            StatConfig config = GetStatConfig(type);

            return new StatModifier(value, config.DefaultOperation, type, source, config.Target);
        }

        public float GetDefaultModifierValue(StatType type, float scaleFactor = 1)
        {
            StatConfig config = GetStatConfig(type);

            if (config.DefaultOperation == ModifierOperation.FlatAdd)
            {
                return config.DirectValue > 0f
                    ? config.DirectValue
                    : config.BaseFlatValue + (config.BaseFlatValuePerLevel * scaleFactor);
            }

            float value = config.BasePercentValuePerLevel * scaleFactor;

            if (type == StatType.CooldownReduction)
            {
                value += 0.02f;
            }

            return value;
        }

        public Dictionary<StatType, BaseStatConfig> ScaleStats(Dictionary<StatType, BaseStatConfig> targetStats, float scaleAmount)
        {
            var returnList = new Dictionary<StatType, BaseStatConfig>();
            var enemyWaveConfig = WaveManager.Instance != null ? WaveManager.Instance.WaveConfig.enemyWaveConfig : null;

            foreach (var stat in targetStats)
            {
                if (enemyWaveConfig == null)
                {
                    if (stat.Value.IsUpgradable)
                    {
                        returnList.Add(stat.Key, stat.Value);
                    }
                    continue;
                }

                var modifier = new StatModifier(scaleAmount * enemyWaveConfig.enemyStatScaleFactor, ModifierOperation.PercentMultiply, stat.Key);

                if (stat.Value.IsUpgradable)
                {
                    stat.Value.BaseStat.AddModifier(modifier);
                    returnList.Add(stat.Key, stat.Value);
                }
            }

            return returnList;
        }

        public List<StatType> GetUpgradableStatTypes()
        {
            if (_statConfigData == null)
            {
                return new List<StatType>();
            }

            return _statConfigData.StatConfigs
                .Where(config => config.IsUpgradable)
                .Select(config => config.Type)
                .ToList();
        }

        public StatTarget GetStatTargetByStatType(StatType type)
        {
            return GetStatConfig(type).Target;
        }

        public float ClampStatValue(StatType statType, float currentValue)
        {
            StatConfig config = GetStatConfig(statType);

            if (config.ShouldClamp)
            {
                return Mathf.Clamp(currentValue, config.MinValue, config.MaxValue);
            }
            return currentValue;
        }
        protected override void OnDispose()
        {

        }


        public class OnStatSelection : Signal { }

        public class OnStatSelected : Signal<StatModifier> { }
    }
}

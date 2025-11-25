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

        public static StatSystem Instance {  get; private set; }

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

        public StatModifier CreateRandomModifier(StatType type, int level = 1, object source = null)
        {
            float value = 0f;
            ModifierOperation operation;

            switch (type)
            {
                case StatType.MaxHealth:
                    operation = Random.value > 0.6f ? ModifierOperation.FlatAdd : ModifierOperation.PercentAdd;
                    value = operation == ModifierOperation.FlatAdd ? (2f * level) : (0.05f * level);
                    type = StatType.MaxHealth;
                    break;

                case StatType.MoveSpeed:
                    operation = ModifierOperation.PercentAdd;
                    value = 0.05f + (level * 0.01f);
                    type = StatType.MoveSpeed;
                    break;

                case StatType.BodyDamage:
                    operation = Random.value > 0.7f ? ModifierOperation.PercentAdd : ModifierOperation.FlatAdd;
                    value = operation == ModifierOperation.FlatAdd ? (1f * level) : (0.03f * level);
                    type = StatType.BodyDamage;
                    break;

                case StatType.CooldownReduction:
                    operation = ModifierOperation.PercentAdd;
                    value = 0.02f + (level * 0.01f);
                    type = StatType.CooldownReduction;
                    break;

                case StatType.Energy:
                    operation = ModifierOperation.FlatAdd;
                    value = 5f + (level * 2f);
                    type = StatType.Energy;
                    break;

                default:
                    operation = ModifierOperation.FlatAdd;
                    value = 1f;
                    type = StatType.MaxHealth;
                    break;
            }

            return new StatModifier(value, operation, type, source);
        }

        public float ClampStatValue(StatType statType, float currentValue)
        {
            switch (statType)
            {
                case StatType.CooldownReduction:
                    return Mathf.Clamp(currentValue, 0f, 0.90f);

                case StatType.MoveSpeed:
                    return Mathf.Clamp(currentValue, 0.1f, 15f);

                default:
                    return currentValue;
            }
        }
        protected override void OnDispose()
        {
            
        }


        public class OnStatSelection : Signal { }

        public class OnStatSelected : Signal<StatModifier> { }
    }
}
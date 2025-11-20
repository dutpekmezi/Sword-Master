using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace dutpekmezi
{
    public class StatSystem : BaseSystem
    {
        private readonly StatColorData _statColorData;

        public StatSystem(StatColorData statColorData)
        {
            _statColorData = statColorData;

            OnInitialize();
        }

        protected override void OnInitialize()
        {
            if (_statColorData != null)
            {
                _statColorData.InitializeLookup();
            }
        }

        public Color GetStatColor(StatType statType)
        {
            if (_statColorData == null) return Color.white;

            return _statColorData.GetColor(statType);
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

        public StatModifier GenerateRandomModifier(StatType type, int level = 1, object source = null)
        {
            float value = 0f;
            ModifierOperation operation;

            switch (type)
            {
                case StatType.MaxHealth:
                    operation = Random.value > 0.6f ? ModifierOperation.FlatAdd : ModifierOperation.PercentAdd;
                    value = operation == ModifierOperation.FlatAdd ? (2f * level) : (0.05f * level);
                    break;

                case StatType.MoveSpeed:
                    operation = ModifierOperation.PercentAdd;
                    value = 0.05f + (level * 0.01f);
                    break;

                case StatType.BodyDamage:
                    operation = Random.value > 0.7f ? ModifierOperation.PercentAdd : ModifierOperation.FlatAdd;
                    value = operation == ModifierOperation.FlatAdd ? (1f * level) : (0.03f * level);
                    break;

                case StatType.CooldownReduction:
                    operation = ModifierOperation.PercentAdd;
                    value = 0.02f + (level * 0.01f);
                    break;

                case StatType.Energy:
                    operation = ModifierOperation.FlatAdd;
                    value = 5f + (level * 2f);
                    break;

                default:
                    operation = ModifierOperation.FlatAdd;
                    value = 1f;
                    break;
            }

            return new StatModifier(value, operation, source);
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
    }
}
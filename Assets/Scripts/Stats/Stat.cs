using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace dutpekmezi
{
    [System.Serializable]
    public class Stat
    {
        public StatType Type;

        [SerializeField] private float baseValue;
        public float BaseValue
        {
            get { return baseValue; }
            set
            {
                float newValue = value;

                if (StatSystem.Instance != null)
                {
                    newValue = StatSystem.Instance.ClampStatValue(Type, value);
                }

                if (baseValue != newValue)
                {
                    baseValue = newValue;
                    isDirty = true;
                }
            }
        }

        private bool isDirty = true;
        private float calculatedValue;
        public float Value
        {
            get
            {
                if (isDirty)
                {
                    calculatedValue = CalculateFinalValue();
                    isDirty = false;
                }
                return calculatedValue;
            }
        }

        private readonly List<StatModifier> statModifiers;
        public readonly ReadOnlyCollection<StatModifier> Modifiers;

        public Stat(float baseValue)
        {
            this.baseValue = baseValue;
            statModifiers = new List<StatModifier>();
            Modifiers = statModifiers.AsReadOnly();
        }

        public void AddModifier(StatModifier mod)
        {
            statModifiers.Add(mod);
            isDirty = true;
        }

        public bool RemoveModifier(StatModifier mod)
        {
            if (statModifiers.Remove(mod))
            {
                isDirty = true;
                return true;
            }
            return false;
        }

        public bool RemoveAllModifiersFromSource(object source)
        {
            int numRemoved = statModifiers.RemoveAll(mod => mod.Source == source);

            if (numRemoved > 0)
            {
                isDirty = true;
                return true;
            }
            return false;
        }

        private float CalculateFinalValue()
        {
            float finalValue = baseValue;

            statModifiers.Sort((a, b) => a.Operation.CompareTo(b.Operation));

            for (int i = 0; i < statModifiers.Count; i++)
            {
                StatModifier mod = statModifiers[i];

                if (mod.Operation == ModifierOperation.FlatAdd)
                {
                    finalValue += mod.Value;
                }
                else if (mod.Operation == ModifierOperation.PercentMultiply)
                {
                    finalValue *= (1 + mod.Value);
                }
            }

            finalValue = (float)Mathf.Round(finalValue * 100) / 100;

            if (StatSystem.Instance != null)
            {
                finalValue = StatSystem.Instance.ClampStatValue(Type, finalValue);
            }

            return finalValue;
        }
    }

    public enum StatType
    {
        MaxHealth,
        MoveSpeed,
        BodyDamage,
        WeaponOrbitSpeed,
        WeaponOrbitRadius,
        WeaponSelfOrbitSpeed,
        CooldownReduction,
        Energy,
        HealthRegen,
        EnergyRegen,
        LifeSteel,
        ExpToLevelUp,
        ExpOnDeath,
        PushForce
    }

    public enum StatTarget
    {
        Entity,
        Weapon
    }

    [Serializable]
    public class BaseStatConfig
    {
        public Stat BaseStat;
        public bool IsUpgradable = true;
    }

    public static class StatTypeExtensions
    {
        public static string GetName(this StatType statType)
        {
            string name = statType.ToString();

            string result = "";
            foreach (char c in name)
            {
                if (char.IsUpper(c) && result.Length > 0 && result[result.Length - 1] != ' ')
                {
                    result += " ";
                }
                result += c;
            }

            return result.Trim();
        }
    }
}
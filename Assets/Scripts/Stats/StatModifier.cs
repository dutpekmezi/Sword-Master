namespace dutpekmezi
{
    public enum ModifierOperation
    {
        FlatAdd = 0,
        PercentAdd = 1,
        PercentMultiply = 2
    }

    [System.Serializable]
    public class StatModifier
    {
        public readonly StatType Type;

        public readonly float Value;
        public readonly ModifierOperation Operation;
        public readonly object Source;

        public StatModifier(float value, ModifierOperation operation, StatType type)
        {
            Value = value;
            Operation = operation;
            Source = null;
            Type = type;
        }

        public StatModifier(float value, ModifierOperation operation, StatType type, object source)
        {
            Value = value;
            Operation = operation;
            Source = source;
            Type = type;
        }
    }
}

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
        public readonly float Value;
        public readonly ModifierOperation Operation;
        public readonly object Source;

        public StatModifier(float value, ModifierOperation operation)
        {
            Value = value;
            Operation = operation;
            Source = null;
        }

        public StatModifier(float value, ModifierOperation operation, object source)
        {
            Value = value;
            Operation = operation;
            Source = source;
        }
    }
}
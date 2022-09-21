namespace Countdown.ValueImplementations
{
    public class IntValue : IStringRepresentable<IntValue>
    {
        public int Value { get; private set; }

        public IntValue(int val)
        {
            Value = val;
        }

        public string AsString() => Value.ToString();

        public IntValue FromString(string value) => int.Parse(value);

        public static implicit operator IntValue(int val) => new(val);
        public static implicit operator int(IntValue iVal) => iVal.Value;
    }
}

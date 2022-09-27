namespace Countdown.ValueImplementations.Values
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

        public bool IsEquivalentTo(IntValue? val)
        {
            if (val is null)
                return false;

            return val.Value == Value;
        }

        public static implicit operator IntValue(int val) => new(val);
        public static implicit operator int(IntValue iVal) => iVal.Value;
    }
}

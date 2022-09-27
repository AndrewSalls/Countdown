namespace Countdown.ValueImplementations.Values
{
    public class IntValue : IStringRepresentable<int>
    {
        public IntValue(int val) : base(val) { }

        public override string AsString() => Value.ToString();

        public override int FromString(string value) => int.Parse(value);

        public override bool IsEquivalentTo(IRepresentable<int, string> val)
        {
            if (val is not IntValue)
                return false;

            return val.Value == Value;
        }

        public static implicit operator IntValue(int val) => new(val);
        public static implicit operator int(IntValue iVal) => iVal.Value;
    }
}

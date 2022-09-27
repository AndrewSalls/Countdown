namespace Countdown.ValueImplementations.Values
{
    public abstract class IRepresentable<T, U>
    {
        public T Value { get; private set; }

        public IRepresentable(T val)
        {
            Value = val;
        }

        public abstract U AsRepresentation();

        public abstract T FromRepresentation(U representation);

        public abstract bool IsEquivalentTo(IRepresentable<T, U> value);
    }
}

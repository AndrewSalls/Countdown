namespace Countdown.ValueImplementations.Values
{
    public abstract class IStringRepresentable<T> : IRepresentable<T, string>
    {
        public IStringRepresentable(T value) : base(value) { }

        public abstract string AsString();
        public abstract T FromString(string value);

        public sealed override string AsRepresentation() => AsString();
        public sealed override T FromRepresentation(string representation) => FromString(representation);
    }
}

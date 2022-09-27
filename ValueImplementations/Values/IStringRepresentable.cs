namespace Countdown.ValueImplementations.Values
{
    public interface IStringRepresentable<T>
    {
        string AsString();
        T FromString(string value);

        bool IsEquivalentTo(T? val);
    }
}

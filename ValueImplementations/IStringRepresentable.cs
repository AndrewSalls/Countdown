namespace Countdown
{
    public interface IStringRepresentable<T>
    {
        string AsString();
        T FromString(string value);

        bool IsEquivalentTo(T? val);
    }
}

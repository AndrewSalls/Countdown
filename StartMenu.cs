namespace Countdown
{
    public class StartMenu
    {
        public static void Main(string[] _)
        {
            NumberPickerMenu<int> _1 = new(ValueGenerator<int>.GetDefaultNumberGenerator());
        }
    }
}

namespace Countdown
{
    public class StartMenu
    {
        public static void Main(string[] _)
        {
            NumberPickerMenu<int> menu = new(ValueGenerator<int>.GetDefaultNumberGenerator());
        }
    }
}

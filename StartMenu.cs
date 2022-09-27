using Countdown.ValueImplementations.Values;

namespace Countdown
{
    public class StartMenu
    {
        public static void Main(string[] _)
        {
            NumberPickerMenu<IntValue> _1 = new(ValueGenerator<IntValue>.GetDefaultNumberGenerator());
        }
    }
}

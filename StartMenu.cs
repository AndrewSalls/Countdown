using Countdown.ValueImplementations;

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

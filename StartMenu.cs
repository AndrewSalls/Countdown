using Countdown.ValueImplementations.Representation;
using Countdown.ValueImplementations.Values;

namespace Countdown
{
    public class StartMenu
    {
        public static void Main(string[] _)
        {
            NumberPickerMenu<int, string> _1 = NumberPickerMenu<int, string>.CreateDefaultGame();
        }
    }
}

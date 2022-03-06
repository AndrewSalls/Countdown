namespace Countdown
{
    public class NumberPickerMenu<T>
    {
        public static readonly int SPINNER_TICK_INTERVAL = 100;
        public static readonly int MAX_BIG_ROW_BUTTON_COUNT = 4;
        public static readonly int MAX_SMALL_ROW_BUTTON_COUNT = 10;

        private readonly TableLayoutPanel _windowContainer;

        private readonly List<Button> _bigNumbers;
        private readonly List<Button> _smallNumbers;

        private readonly FlowLayoutPanel _bigNumberContainer;
        private readonly FlowLayoutPanel _smallNumberContainer;

        private readonly Label _goalSpinner;
        private readonly System.Windows.Forms.Timer _spinnerTicker;

        private readonly Button _spinnerStop;

        private readonly ValueGenerator<T> _game;

        public NumberPickerMenu(ValueGenerator<T> game)
        {
            _game = game;

            _bigNumbers = new();
            for(int i = 0; i < _game.BigNumbers.Count; i++)
                _bigNumbers.Add(CreateButton(i, true));

            _smallNumbers = new();
            for (int i = 0; i < _game.SmallNumbers.Count; i++)
                _smallNumbers.Add(CreateButton(i, false));

            _goalSpinner = new()
            {
                BackColor = Color.Black,
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = true,
                ForeColor = Color.Yellow,
                Visible = true
            };
            _spinnerTicker = new();
            _spinnerStop = new Button()
            {
                BackColor = Color.Red,
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = false,
                ForeColor = Color.Black,
                Visible = true
            };

            _spinnerTicker.Interval = SPINNER_TICK_INTERVAL;
            _spinnerTicker.Tick += (o, e) =>
            {
                _game.RandomizeGoal();
                _goalSpinner.Text = _game.State == ValueGenerator<T>.GenerationPhase.ERROR ? "" : _game.Goal?.ToString();
                _spinnerStop.Enabled = true;
            };
            _spinnerStop.Click += (o, e) =>
            {
                _spinnerTicker.Stop();
                for(int i = 0; i < new Random().Next(1, 5); i++)
                {
                    _game.RandomizeGoal();
                    _goalSpinner.Text = _game.State == ValueGenerator<T>.GenerationPhase.ERROR ? "" : _game.Goal?.ToString();
                }
            };

            _bigNumberContainer = new()
            {
                AutoScroll = true,
                BackColor = Color.LightBlue,
                Dock = DockStyle.Fill,
                Enabled = true,
                FlowDirection = FlowDirection.TopDown,
                Visible = true,
                WrapContents = false
            };
            _bigNumberContainer.Controls.Add(CreateButtonRows(_bigNumbers, MAX_BIG_ROW_BUTTON_COUNT));
            _smallNumberContainer = new()
            {
                AutoScroll = true,
                BackColor = Color.LightBlue,
                Dock = DockStyle.Fill,
                Enabled = true,
                FlowDirection = FlowDirection.TopDown,
                Visible = true,
                WrapContents = false
            };
            _smallNumberContainer.Controls.Add(CreateButtonRows(_smallNumbers, MAX_SMALL_ROW_BUTTON_COUNT));
        }

        private Button CreateButton(int pos, bool isBig)
        {
            Button output = new()
            {
                BackColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleCenter,
                Enabled = true,
                ForeColor = Color.White,
                Visible = true
            };
            if (isBig)
            {
                string? v = _game.BigNumbers[pos]?.ToString();
                if (v == null)
                    throw new NullReferenceException();
                output.Text = v;
            }
            else
            {
                string? v = _game.SmallNumbers[pos]?.ToString();
                if (v == null)
                    throw new NullReferenceException();
                output.Text = v;
            }

            output.Click += (o, e) =>
            {
                _game.ChooseNumber(pos, isBig);
                output.Enabled = false;

                if(_game.State == ValueGenerator<T>.GenerationPhase.RANDOMIZING)
                {
                    _bigNumbers.ForEach(b => b.Enabled = false);
                    _smallNumbers.ForEach(b => b.Enabled = false);
                    _spinnerTicker.Start();
                }
            };

            return output;
        }
    }
}

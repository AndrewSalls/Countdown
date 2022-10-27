using Countdown.ValueImplementations;
using Countdown.ValueImplementations.Representation;
using StoI = Countdown.ValueModel.Representation.ImageFactory;

namespace Countdown.GameController
{
    public class GamePage<T> : TableLayoutPanel
    {
        public const string SHOW_VALUES = "Show Button Values";
        public const string HIDE_VALUES = "Hide Button Values";
        public const int SPINNER_TICK_INTERVAL = 200;
        public const int MAX_BIG_ROW_BUTTON_COUNT = 4;
        public const int MAX_SMALL_ROW_BUTTON_COUNT = 10;
        public const string STOP_TEXT_WORDS = "STOP!";
        public static readonly Color BACKGROUND = Color.FromArgb(255, 255, 240);
        public static readonly Color STEPS_BACKGROUND = Color.LightGoldenrodYellow;
        public static readonly Color SPINNER_BACKGROUND = Color.Black;
        public static readonly Color RESET_BACKGROUND = Color.DarkCyan;
        public static readonly Color RESET_TEXT = Color.Black;
        public static readonly Color STOP_PRE_BACKGROUND = Color.Red;
        public static readonly Color STOP_POST_BACKGROUND = Color.Goldenrod;
        public static readonly Color SPINNER_TEXT = Color.Yellow;
        public static readonly Color PLAIN_TEXT = Color.Black;
        public static readonly Color INFO_BACKGROUND = Color.White;
        public static readonly Color INFO_TEXT = Color.Black;
        public static readonly Color BUTTON_BACKGROUND = Color.DarkBlue;
        public static readonly Color BUTTON_TEXT = Color.White;
        public static readonly Color CLICKED_BUTTON_BACKGROUND = ControlPaint.Light(BUTTON_BACKGROUND, 0.5F);
        public static readonly Color CLICKED_BUTTON_TEXT = Color.DarkBlue;
        public static readonly Color BUTTON_BACKGROUND_BACKGROUND = Color.LightBlue;

       private readonly Form _window;

        private readonly List<Button> _bigValues;
        private readonly List<Button> _smallValues;

        private TableLayoutPanel _bigValueContainer;
        private TableLayoutPanel _smallValueContainer;

        private readonly Label _goalSpinner;
        private readonly System.Windows.Forms.Timer _spinnerTicker;

        private readonly Button _reset;
        private readonly Button _spinnerStop;
        private readonly Button _openSteps;
        private readonly Button _toggleLabels;
        private readonly Button _enterSolution;

        private readonly StepPage<T> _stepDisplay;
        private readonly SolutionPage<T> _solutionDisplay;

        private readonly ValueGenerator<T> _game;
        private readonly ExpressionConverter<T> _converter;

        public static GamePage<int> CreateDefaultGame()
        {
            List<Operation<int>> ops = new()
            {
                new Operation<int>(1, true, true, (a, b) => a + b, (a, b) => true),
                new Operation<int>(1, false, false, (a, b) => a - b, (a, b) => a - b >= 0),
                new Operation<int>(2, true, true, (a, b) => a * b, (a, b) => true),
                new Operation<int>(2, false, false, (a, b) => a / b, (a, b) => b != 0 && a % b == 0)
            };
            ValueGenerator<int> game = new(
                new List<int>() { 25, 50, 75, 100 },
                new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                ops, 6, 6);

            Dictionary<Operation<int>, SymbolRepresentation> opDisplay = new()
            {
                {ops[0], new SymbolRepresentation(StoI.CreateImage("+", PLAIN_TEXT), (l, s, r) => new ImageTreeNode(l, s, r))},
                {ops[1], new SymbolRepresentation(StoI.CreateImage("-", PLAIN_TEXT), (l, s, r) => new ImageTreeNode(l, s, r))},
                {ops[2], new SymbolRepresentation(StoI.CreateImage("*", PLAIN_TEXT), (l, s, r) => new ImageTreeNode(l, s, r))},
                {ops[3], new SymbolRepresentation(StoI.CreateImage("/", PLAIN_TEXT), (l, s, r) => new ImageTreeNode(l, s, r))},
            };
            ExpressionConverter<int> converter = new(new ImageRepresentation<int>(i => StoI.CreateImage(i.ToString(), BUTTON_TEXT)), opDisplay);

            return new GamePage<int>(game, converter);
        }

        public GamePage(ValueGenerator<T> game, ExpressionConverter<T> converter)
        {
            _game = game;
            _converter = converter;

            double width = Screen.PrimaryScreen.WorkingArea.Width;
            double height = Screen.PrimaryScreen.WorkingArea.Height;

            _window = new Form()
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                Icon = Properties.Resources.CountdownIcon,
                IsMdiContainer = true,
                StartPosition = FormStartPosition.CenterScreen,
                Text = "Countdown - Numbers Round Advanced",
                Width = (int)Math.Round(width * 0.8),
                Height = (int)Math.Round(height * 0.8)
            };

            RowCount = 9;
            ColumnCount = 7;
            BackColor = BACKGROUND;
            Dock = DockStyle.Fill;
            Enabled = true;
            Visible = true;

            RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 9));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

            _stepDisplay = new StepPage<T>(_window, this);
            _solutionDisplay = new SolutionPage<T>(_window, this, _game, _converter);

            _bigValues = new();
            _smallValues = new();
            _bigValueContainer = new();
            _smallValueContainer = new();

            _openSteps = new()
            {
                BackColor = STEPS_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = PLAIN_TEXT,
                Text = "Open Solution",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _openSteps.FlatAppearance.BorderSize = 0;

            _toggleLabels = new()
            {
                BackColor = STEPS_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = PLAIN_TEXT,
                Text = SHOW_VALUES,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _toggleLabels.FlatAppearance.BorderSize = 0;

            _goalSpinner = new()
            {
                BackColor = SPINNER_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                ForeColor = SPINNER_TEXT,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _spinnerTicker = new();
            _spinnerStop = new()
            {
                BackColor = STOP_PRE_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = PLAIN_TEXT,
                Text = STOP_TEXT_WORDS,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _spinnerStop.FlatAppearance.BorderSize = 0;
            _spinnerStop.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            _reset = new()
            {
                BackColor = RESET_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = RESET_TEXT,
                Text = "Reset",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _reset.FlatAppearance.BorderSize = 0;
            _reset.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            _enterSolution = new()
            {
                BackColor = STEPS_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = PLAIN_TEXT,
                Text = "Enter Solution",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _enterSolution.FlatAppearance.BorderSize = 0;

            InitializePage();
            Application.Run(_window);
        }

        private void InitializePage()
        {
            for (int i = 0; i < _game.BigValues.Count; i++)
                _bigValues.Add(CreateButton(i, true));

            for (int i = 0; i < _game.SmallValues.Count; i++)
                _smallValues.Add(CreateButton(i, false));

            _toggleLabels.Click += (o, e) =>
            {
                if (_toggleLabels.Text.Equals(SHOW_VALUES))
                {
                    for (int i = 0; i < _bigValues.Count; i++)
                    {
                        if (!_bigValues[i].BackColor.Equals(CLICKED_BUTTON_BACKGROUND))
                            RenderOnControl(_game.BigValues[i], _bigValues[i], CLICKED_BUTTON_TEXT);
                    }

                    for (int i = 0; i < _smallValues.Count; i++)
                    {
                        if (!_smallValues[i].BackColor.Equals(CLICKED_BUTTON_BACKGROUND))
                            RenderOnControl(_game.SmallValues[i], _smallValues[i], CLICKED_BUTTON_TEXT);
                    }

                    _toggleLabels.Text = HIDE_VALUES;
                }
                else
                {
                    for (int i = 0; i < _bigValues.Count; i++)
                    {
                        if (!_bigValues[i].BackColor.Equals(CLICKED_BUTTON_BACKGROUND))
                            _bigValues[i].BackgroundImage = null;
                    }

                    for (int i = 0; i < _smallValues.Count; i++)
                    {
                        if (!_smallValues[i].BackColor.Equals(CLICKED_BUTTON_BACKGROUND))
                            _smallValues[i].BackgroundImage = null;
                    }

                    _toggleLabels.Text = SHOW_VALUES;
                }
            };

            _openSteps.Click += (o, e) =>
            {
                _window.Controls.Remove(this);
                _window.Controls.Add(_stepDisplay);

            };

            _spinnerTicker.Interval = SPINNER_TICK_INTERVAL;
            _spinnerTicker.Tick += (o, e) =>
            {
                _game.RandomizeGoal();
                RenderOnControl(_game.Goal, _goalSpinner, SPINNER_TEXT);
                _spinnerStop.Enabled = true;
            };
            _spinnerStop.Click += (o, e) =>
            {
                if (!_spinnerTicker.Enabled)
                {
                    _openSteps.Enabled = false;
                    _enterSolution.Enabled = false;
                    _stepDisplay.Clear();
                    _spinnerTicker.Enabled = true;
                    _spinnerTicker.Start();
                    _spinnerStop.BackColor = STOP_PRE_BACKGROUND;
                    _spinnerStop.Text = STOP_TEXT_WORDS;
                }
                else
                {
                    _spinnerTicker.Stop();
                    for (int i = 0; i < new Random().Next(0, 2); i++)
                    {
                        _game.RandomizeGoal();
                        RenderOnControl(_game.Goal, _goalSpinner, SPINNER_TEXT);
                    }

                    _spinnerTicker.Enabled = false;
                    _spinnerStop.BackColor = STOP_POST_BACKGROUND;
                    _spinnerStop.Text = "Resume Spinner";

                    var steps = _game.GetIntendedSolution();

                    _stepDisplay.Display(_converter.CreateDisplayableRepresentation(steps));

                    _openSteps.Enabled = true;
                    _enterSolution.Enabled = true;
                }
            };
            _reset.Click += (o, e) =>
            {
                _game.Reset();

                _spinnerStop.BackColor = STOP_PRE_BACKGROUND;
                _spinnerStop.ForeColor = PLAIN_TEXT;
                _spinnerStop.Text = STOP_TEXT_WORDS;

                for (int i = 0; i < _game.BigValues.Count; i++)
                {
                    _bigValues[i].BackColor = BUTTON_BACKGROUND;
                    _bigValues[i].Enabled = true;
                    _bigValues[i].ForeColor = BUTTON_TEXT;
                    if (_toggleLabels.Text.Equals(HIDE_VALUES))
                        RenderOnControl(_game.BigValues[i], _bigValues[i], BUTTON_TEXT);
                    else
                        _bigValues[i].BackgroundImage = null;
                }

                for (int i = 0; i < _game.SmallValues.Count; i++)
                {
                    _smallValues[i].BackColor = BUTTON_BACKGROUND;
                    _smallValues[i].Enabled = true;
                    _smallValues[i].ForeColor = BUTTON_TEXT;
                    if (_toggleLabels.Text.Equals(HIDE_VALUES))
                        RenderOnControl(_game.SmallValues[i], _smallValues[i], BUTTON_TEXT);
                    else
                        _smallValues[i].BackgroundImage = null;
                }


                Controls.Add(_bigValueContainer, 2, 3);
                SetRowSpan(_bigValueContainer, 1);
                SetColumnSpan(_bigValueContainer, 3);

                Controls.Add(_smallValueContainer, 1, 5);
                SetRowSpan(_smallValueContainer, 1);
                SetColumnSpan(_smallValueContainer, 5);

                _spinnerTicker.Stop();
                _spinnerTicker.Enabled = false;
                _spinnerStop.Enabled = false;
                _goalSpinner.BackgroundImage = null;
                _openSteps.Enabled = false;
                _enterSolution.Enabled = false;
                _stepDisplay.Clear();
            };

            _enterSolution.Click += (o, e) =>
            {
                _window.Controls.Remove(this);
                _window.Controls.Add(_solutionDisplay);
            };

            _bigValueContainer = CreateButtonRows(_bigValues, MAX_BIG_ROW_BUTTON_COUNT);
            _smallValueContainer = CreateButtonRows(_smallValues, MAX_SMALL_ROW_BUTTON_COUNT);

            Controls.Add(_bigValueContainer, 2, 3);
            SetRowSpan(_bigValueContainer, 1);
            SetColumnSpan(_bigValueContainer, 3);

            Controls.Add(_smallValueContainer, 1, 5);
            SetRowSpan(_smallValueContainer, 1);
            SetColumnSpan(_smallValueContainer, 5);

            Controls.Add(_openSteps, 4, 1);
            SetRowSpan(_openSteps, 1);
            SetColumnSpan(_openSteps, 1);

            Controls.Add(_toggleLabels, 3, 1);
            SetRowSpan(_toggleLabels, 1);
            SetColumnSpan(_toggleLabels, 1);

            Controls.Add(_goalSpinner, 5, 1);
            SetRowSpan(_goalSpinner, 1);
            SetColumnSpan(_goalSpinner, 1);

            Controls.Add(_reset, 1, 3);
            SetRowSpan(_reset, 1);
            SetColumnSpan(_reset, 1);

            Controls.Add(_spinnerStop, 5, 3);
            SetRowSpan(_spinnerStop, 1);
            SetColumnSpan(_spinnerStop, 1);

            Controls.Add(_enterSolution, 2, 7);
            SetRowSpan(_enterSolution, 1);
            SetColumnSpan(_enterSolution, 3);

            _window.Controls.Add(this);
            _window.Show();
            _window.BringToFront();
            _window.Focus();
        }

        private Button CreateButton(int pos, bool isBig)
        {
            Button output = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = BUTTON_BACKGROUND,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = BUTTON_TEXT,
                Margin = new Padding(0),
                Padding = new Padding(0),
                TabStop = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            output.FlatAppearance.BorderSize = 0;
            output.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            output.Click += (o, e) =>
            {
                output.BackColor = CLICKED_BUTTON_BACKGROUND;
                output.ForeColor = CLICKED_BUTTON_TEXT;
                _game.ChooseNumber(pos, isBig);
                RenderOnControl(isBig ? _game.BigValues[pos] : _game.SmallValues[pos], output, CLICKED_BUTTON_TEXT);
                output.Enabled = false;

                if (_game.State == ValueGenerator<T>.GenerationPhase.RANDOMIZING)
                {
                    _bigValues.ForEach(b => b.Enabled = false);
                    _smallValues.ForEach(b => b.Enabled = false);
                    _spinnerTicker.Enabled = true;
                    _spinnerTicker.Start();
                }
            };

            return output;
        }

        private static TableLayoutPanel CreateButtonRows(List<Button> entries, int rowAmt)
        {
            if (rowAmt < 1)
                throw new ArgumentOutOfRangeException(nameof(rowAmt));

            int extraRow = entries.Count % rowAmt == 0 ? 0 : 1;
            TableLayoutPanel output = new()
            {
                RowCount = entries.Count / rowAmt + extraRow + 1,
                ColumnCount = rowAmt,
                BackColor = BUTTON_BACKGROUND_BACKGROUND,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None,
                Dock = DockStyle.Fill,
                Enabled = true,
                Visible = true
            };
            output.HorizontalScroll.Maximum = 0;
            output.HorizontalScroll.Enabled = false;
            output.HorizontalScroll.Visible = false;
            output.AutoScroll = true;

            output.Resize += (o, e) =>
            {
                int width = output.Width / rowAmt;

                entries.ForEach(b =>
                {
                    b.Width = width;
                    b.Height = width;
                });

                output.VerticalScroll.Maximum = (output.RowCount - 1) * width;
            };

            for (int i = 0; i < rowAmt; i++)
                output.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f / rowAmt));

            for (int r = 0; r < entries.Count / rowAmt; r++)
            {
                for (int c = 0; c < rowAmt; c++)
                {
                    Button current = entries[rowAmt * r + c];
                    output.Controls.Add(current);
                    output.SetRow(current, r);
                    output.SetColumn(current, c);
                    output.SetRowSpan(current, 1);
                    output.SetColumnSpan(current, 1);
                }
            }

            if (extraRow != 0)
            {
                TableLayoutPanel extraValues = new()
                {
                    RowCount = 1,
                    ColumnCount = entries.Count % rowAmt,
                    BackColor = BUTTON_BACKGROUND_BACKGROUND,
                    Dock = DockStyle.Fill,
                    Enabled = true,
                    Visible = true
                };
                for (int i = 0; i < entries.Count % rowAmt; i++)
                {
                    Button endVal = entries[entries.Count / rowAmt * rowAmt + i];
                    extraValues.Controls.Add(endVal);
                    extraValues.SetRow(endVal, 0);
                    extraValues.SetColumn(endVal, i);
                    extraValues.SetRowSpan(endVal, 1);
                    extraValues.SetColumnSpan(endVal, 1);
                }

                output.Controls.Add(extraValues);
                output.SetRow(extraValues, output.RowCount - 1);
                output.SetColumn(extraValues, 0);
                output.SetRowSpan(extraValues, 1);
                output.SetColumnSpan(extraValues, 1);

            }

            return output;
        }

        public void RenderOnControl(T? value, Control control, Color renderingColor)
        {
            Image displayRep = _game.State == ValueGenerator<T>.GenerationPhase.ERROR ? _converter.Representer.CreateErrorRepresentation() : _converter.Representer.AsRepresentation(value!);
            control.BackgroundImage = displayRep;
        }
    }
}

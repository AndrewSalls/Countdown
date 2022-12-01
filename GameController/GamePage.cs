using Countdown.ValueImplementations;
using Countdown.ValueImplementations.Representation;
using Countdown.ValueModel.Representation;

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
        public static readonly Color BUTTON_HIGHLIGHTED_BACKGROUND = ControlPaint.Light(BUTTON_BACKGROUND, 0.15F);
        public static readonly Color CLICKED_BUTTON_BACKGROUND = ControlPaint.Light(BUTTON_BACKGROUND, 0.35F);
        public static readonly Color CLICKED_BUTTON_TEXT = Color.DarkBlue;
        public static readonly Color BUTTON_BACKGROUND_BACKGROUND = Color.LightBlue;

       private readonly Form _window;

        private readonly List<Panel> _bigValues;
        private readonly List<Panel> _smallValues;

        private TableLayoutPanel _bigValueContainer;
        private TableLayoutPanel _smallValueContainer;
        private int _bigMinScale;
        private int _smallMinScale;

        private readonly Panel _goalSpinner;
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
                {ops[0], new SymbolRepresentation(ImageFactory.CreateImage("+", PLAIN_TEXT, ImageFactory.DEFAULT_SIZE.bBox), (l, s, r) => new ImageTreeNode(l, s, r))},
                {ops[1], new SymbolRepresentation(ImageFactory.CreateImage("-", PLAIN_TEXT, ImageFactory.DEFAULT_SIZE.bBox), (l, s, r) => new ImageTreeNode(l, s, r))},
                {ops[2], new SymbolRepresentation(ImageFactory.CreateImage("*", PLAIN_TEXT, ImageFactory.DEFAULT_SIZE.bBox), (l, s, r) => new ImageTreeNode(l, s, r))},
                {ops[3], new SymbolRepresentation(ImageFactory.CreateImage("/", PLAIN_TEXT, ImageFactory.DEFAULT_SIZE.bBox), (l, s, r) => new ImageTreeNode(l, s, r))},
            };
            ExpressionConverter<int> converter = new(new ImageRepresentation<int>((i, bb, minScale) => {
                return ImageFactory.CreateImage(i.ToString(), BUTTON_TEXT, bb, minScale);
            }, (i, bb) =>
            {
                return ImageFactory.MaximizeTextFont(i.ToString(), bb).scale;
            }), opDisplay);

            return new GamePage<int>(game, converter);
        }

        public GamePage(ValueGenerator<T> game, ExpressionConverter<T> converter)
        {
            _game = game;
            _converter = converter;
            _smallMinScale = -1;
            _bigMinScale = -1;

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
                    _toggleLabels.Text = HIDE_VALUES;
                else
                    _toggleLabels.Text = SHOW_VALUES;

                for (int i = 0; i < _game.BigValues.Count; i++)
                    _bigValues[i].Invalidate();

                for (int i = 0; i < _game.SmallValues.Count; i++)
                    _smallValues[i].Invalidate();
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
                _goalSpinner.Invalidate();
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
                        _goalSpinner.Invalidate();
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
                    _bigValues[i].Invalidate();
                }

                for (int i = 0; i < _game.SmallValues.Count; i++)
                {
                    _smallValues[i].BackColor = BUTTON_BACKGROUND;
                    _smallValues[i].Enabled = true;
                    _smallValues[i].ForeColor = BUTTON_TEXT;
                    _smallValues[i].Invalidate();
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
                _goalSpinner.Invalidate();
                _openSteps.Enabled = false;
                _enterSolution.Enabled = false;
                _stepDisplay.Clear();
            };
            _goalSpinner.Paint += (o, e) =>
            {
                Graphics g = e.Graphics;
                Color temp = ImageRepresentation<T>.RenderColor;
                ImageRepresentation<T>.RenderColor = SPINNER_TEXT;
                if (_game.State.Equals(ValueGenerator<T>.GenerationPhase.RANDOMIZING) || _game.State.Equals(ValueGenerator<T>.GenerationPhase.EVALUATING))
                {
                    Image displayRep = _converter.Representer.AsRepresentation(_game.Goal!, _goalSpinner.Size);
                    g.DrawImage(displayRep, 0, 0, _goalSpinner.Width, _goalSpinner.Height);
                }
                else if (_game.State.Equals(ValueGenerator<T>.GenerationPhase.ERROR))
                    g.DrawImage(_converter.Representer.CreateErrorRepresentation(_goalSpinner.Size), 0, 0, _goalSpinner.Width, _goalSpinner.Height);
            };

            _enterSolution.Click += (o, e) =>
            {
                _window.Controls.Remove(this);
                _window.Controls.Add(_solutionDisplay);
            };

            _bigValueContainer = CreateButtonRows(_bigValues, MAX_BIG_ROW_BUTTON_COUNT);
            _smallValueContainer = CreateButtonRows(_smallValues, MAX_SMALL_ROW_BUTTON_COUNT);
            _bigValueContainer.Resize += (o, e) =>
            {
                _bigMinScale = _game.BigValues.Min(v => _converter.Representer.GetMaximalScaling(v, _bigValueContainer.Controls[0].Size));
                //System.Diagnostics.Debug.WriteLine($"Minimum Big Scaling: {_bigMinScale}");
            };
            _smallValueContainer.Resize += (o, e) =>
            {
                _smallMinScale = _game.SmallValues.Min(v => _converter.Representer.GetMaximalScaling(v, _bigValueContainer.Controls[0].Size));
                //System.Diagnostics.Debug.WriteLine($"Minimum Small Scaling: {_smallMinScale}");
            };

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

        private Panel CreateButton(int pos, bool isBig)
        {
            Panel output = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = BUTTON_BACKGROUND,
                Enabled = true,
                ForeColor = BUTTON_TEXT,
                Margin = new Padding(0),
                Padding = new Padding(0),
                TabStop = false,
                Visible = true
            };

            output.Click += (o, e) =>
            {
                output.BackColor = CLICKED_BUTTON_BACKGROUND;
                output.ForeColor = CLICKED_BUTTON_TEXT;
                _game.ChooseNumber(pos, isBig);
                output.Enabled = false;

                if (_game.State == ValueGenerator<T>.GenerationPhase.RANDOMIZING)
                {
                    _bigValues.ForEach(b => b.Enabled = false);
                    _smallValues.ForEach(b => b.Enabled = false);
                    _spinnerTicker.Enabled = true;
                    _spinnerTicker.Start();
                }
            };
            output.MouseEnter += (o, e) =>
            {
                if (output.Enabled)
                    output.BackColor = BUTTON_HIGHLIGHTED_BACKGROUND;
            };
            output.MouseLeave += (o, e) =>
            {
                if (output.Enabled)
                    output.BackColor = BUTTON_BACKGROUND;
            };
            output.Paint += (o, e) =>
            {
                if(output.BackColor.Equals(CLICKED_BUTTON_BACKGROUND) || _toggleLabels.Text.Equals(HIDE_VALUES))
                {
                    Graphics g = e.Graphics;
                    T value = isBig ? _game.BigValues[pos] : _game.SmallValues[pos];
                    Image displayRep;
                    if (_game.State == ValueGenerator<T>.GenerationPhase.ERROR)
                        displayRep = _converter.Representer.CreateErrorRepresentation(output.Size);
                    else
                    {
                        if (isBig)
                            displayRep = _converter.Representer.AsRepresentation(value, output.Size, _bigMinScale);
                        else
                            displayRep = _converter.Representer.AsRepresentation(value, output.Size, _smallMinScale);
                    }
                    g.DrawImage(displayRep, 0, 0, output.Width, output.Height);
                }
            };

            return output;
        }

        private static TableLayoutPanel CreateButtonRows(List<Panel> entries, int rowAmt)
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
                    Panel current = entries[rowAmt * r + c];
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
                    Panel endVal = entries[entries.Count / rowAmt * rowAmt + i];
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
    }
}

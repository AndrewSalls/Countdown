using Countdown.ValueImplementations;
using Countdown.ValueImplementations.Values;

namespace Countdown
{
    public class NumberPickerMenu<T> where T : IStringRepresentable<T>
    {
        public static readonly int SPINNER_TICK_INTERVAL = 200;
        public static readonly int MAX_BIG_ROW_BUTTON_COUNT = 4;
        public static readonly int MAX_SMALL_ROW_BUTTON_COUNT = 10;
        public static readonly string STOP_TEXT_WORDS = "STOP!";
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

        private readonly TableLayoutPanel _windowContainer;
        private readonly TableLayoutPanel _stepContainer;
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

        private readonly TextBox _stepInfo;
        private readonly Button _stepReturn;

        private readonly ValueGenerator<T> _game;

        public NumberPickerMenu(ValueGenerator<T> game)
        {
            _game = game;

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

            _windowContainer = new()
            {
                RowCount = 9,
                ColumnCount = 6,
                BackColor = BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                Visible = true
            };
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 15));
            _windowContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 2));
            _windowContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _windowContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _windowContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            _windowContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            _windowContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _windowContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

            _stepContainer = new()
            {
                RowCount = 4,
                ColumnCount = 3,
                BackColor = BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                Visible = true
            };
            _stepContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            _stepContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            _stepContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            _stepContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            _stepContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            _stepContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
            _stepContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

            _bigValues = new();
            _smallValues = new();
            _bigValueContainer = new();
            _smallValueContainer = new();

            _openSteps = new()
            {
                BackColor = STEPS_BACKGROUND,
                Dock = DockStyle.Right,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = PLAIN_TEXT,
                Text = "Open Solution",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _openSteps.FlatAppearance.BorderSize = 0;

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

            _stepInfo = new()
            {
                BackColor = INFO_BACKGROUND,
                Dock = DockStyle.Fill,
                ForeColor = INFO_TEXT,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                TextAlign = HorizontalAlignment.Left,
                WordWrap = false
            };
            _stepReturn = new()
            {
                BackColor = STEPS_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = PLAIN_TEXT,
                Text = "Return",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _stepReturn.FlatAppearance.BorderSize = 0;
            _stepReturn.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            InitializePage();
            Application.Run(_window);
        }

        private void InitializePage()
        {
            for (int i = 0; i < _game.BigValues.Count; i++)
                _bigValues.Add(CreateButton(i, true));

            for (int i = 0; i < _game.SmallValues.Count; i++)
                _smallValues.Add(CreateButton(i, false));

            _openSteps.Click += (o, e) =>
            {
                _window.Controls.Remove(_windowContainer);
                _window.Controls.Add(_stepContainer);

            };
            _stepReturn.Click += (o, e) =>
            {
                _window.Controls.Remove(_stepContainer);
                _window.Controls.Add(_windowContainer);
            };

            _spinnerTicker.Interval = SPINNER_TICK_INTERVAL;
            _spinnerTicker.Tick += (o, e) =>
            {
                _game.RandomizeGoal();
                _goalSpinner.Text = _game.State == ValueGenerator<T>.GenerationPhase.ERROR ? "ERROR" : _game.Goal?.AsString() ?? "GENERATION ERROR";
                _spinnerStop.Enabled = true;
            };
            _spinnerStop.Click += (o, e) =>
            {
                if (!_spinnerTicker.Enabled)
                {
                    _openSteps.Enabled = false;
                    _stepInfo.Clear();
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
                        _goalSpinner.Text = _game.State == ValueGenerator<T>.GenerationPhase.ERROR ? "ERROR" : _game.Goal?.AsString() ?? "GENERATION ERROR";
                    }

                    _spinnerTicker.Enabled = false;
                    _spinnerStop.BackColor = STOP_POST_BACKGROUND;
                    _spinnerStop.Text = "Resume Spinner";

                    var steps = _game.GetIntendedSolution();

                    for (int i = 0; i < steps.Count; i++)
                    {
                        _stepInfo.AppendText(steps[i].ToEquationString());
                        _stepInfo.AppendText(Environment.NewLine);
                    }
                    _stepInfo.AppendText(Environment.NewLine);
                    _stepInfo.AppendText(Equation<T>.ConvertStepsToEquation(steps).ConvertToString() + " = " + steps[steps.Count - 1].Result!.AsString());

                    _openSteps.Enabled = true;
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
                    _bigValues[i].Text = _game.BigValues[i].AsString();
                }

                for (int i = 0; i < _game.SmallValues.Count; i++)
                {
                    _smallValues[i].BackColor = BUTTON_BACKGROUND;
                    _smallValues[i].Enabled = true;
                    _smallValues[i].ForeColor = BUTTON_TEXT;
                    _smallValues[i].Text = _game.SmallValues[i].AsString();
                }


                _windowContainer.Controls.Add(_bigValueContainer, 2, 3);
                _windowContainer.SetRowSpan(_bigValueContainer, 1);
                _windowContainer.SetColumnSpan(_bigValueContainer, 2);

                _windowContainer.Controls.Add(_smallValueContainer, 1, 5);
                _windowContainer.SetRowSpan(_smallValueContainer, 1);
                _windowContainer.SetColumnSpan(_smallValueContainer, 4);

                _goalSpinner.Text = "";
                _spinnerTicker.Stop();
                _spinnerTicker.Enabled = false;
                _spinnerStop.Enabled = false;
                _openSteps.Enabled = false;
                _stepInfo.Clear();
            };

            _bigValueContainer = CreateButtonRows(_bigValues, MAX_BIG_ROW_BUTTON_COUNT);
            _smallValueContainer = CreateButtonRows(_smallValues, MAX_SMALL_ROW_BUTTON_COUNT);

            _windowContainer.Controls.Add(_bigValueContainer, 2, 3);
            _windowContainer.SetRowSpan(_bigValueContainer, 1);
            _windowContainer.SetColumnSpan(_bigValueContainer, 2);

            _windowContainer.Controls.Add(_smallValueContainer, 1, 5);
            _windowContainer.SetRowSpan(_smallValueContainer, 1);
            _windowContainer.SetColumnSpan(_smallValueContainer, 4);

            _windowContainer.Controls.Add(_openSteps, 3, 1);
            _windowContainer.SetRowSpan(_openSteps, 1);
            _windowContainer.SetColumnSpan(_openSteps, 1);

            _windowContainer.Controls.Add(_goalSpinner, 4, 1);
            _windowContainer.SetRowSpan(_goalSpinner, 1);
            _windowContainer.SetColumnSpan(_goalSpinner, 1);

            _windowContainer.Controls.Add(_reset, 1, 3);
            _windowContainer.SetRowSpan(_reset, 1);
            _windowContainer.SetColumnSpan(_reset, 1);

            _windowContainer.Controls.Add(_spinnerStop, 4, 3);
            _windowContainer.SetRowSpan(_spinnerStop, 1);
            _windowContainer.SetColumnSpan(_spinnerStop, 1);

            _stepContainer.Controls.Add(_stepInfo, 1, 1);
            _stepContainer.SetRowSpan(_stepInfo, 1);
            _stepContainer.SetColumnSpan(_stepInfo, 1);

            _stepContainer.Controls.Add(_stepReturn, 1, 2);
            _stepContainer.SetRowSpan(_stepReturn, 1);
            _stepContainer.SetColumnSpan(_stepReturn, 1);

            _window.Controls.Add(_windowContainer);
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
            if (isBig)
            {
                string? v = _game.BigValues[pos]?.AsString();
                if (v == null)
                    throw new NullReferenceException();

                output.Text = v;
            }
            else
            {
                string? v = _game.SmallValues[pos]?.AsString();
                if (v == null)
                    throw new NullReferenceException();

                output.Text = v;
            }

            output.Click += (o, e) =>
            {
                output.BackColor = CLICKED_BUTTON_BACKGROUND;
                output.ForeColor = CLICKED_BUTTON_TEXT;
                _game.ChooseNumber(pos, isBig);
                output.Enabled = false;

                if(_game.State == ValueGenerator<T>.GenerationPhase.RANDOMIZING)
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

            int extraRow = (entries.Count % rowAmt == 0 ? 0 : 1);
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
                for(int c = 0; c < rowAmt; c++)
                {
                    Button current = entries[rowAmt * r + c];
                    output.Controls.Add(current);
                    output.SetRow(current, r);
                    output.SetColumn(current, c);
                    output.SetRowSpan(current, 1);
                    output.SetColumnSpan(current, 1);
                }
            }

            if(extraRow != 0)
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
                for(int i = 0; i < entries.Count % rowAmt; i++)
                {
                    Button endVal = entries[(entries.Count / rowAmt) * rowAmt + i];
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

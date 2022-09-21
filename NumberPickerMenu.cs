namespace Countdown
{
    public class NumberPickerMenu<T>
    {
        public static readonly int SPINNER_TICK_INTERVAL = 200;
        public static readonly int MAX_BIG_ROW_BUTTON_COUNT = 4;
        public static readonly int MAX_SMALL_ROW_BUTTON_COUNT = 10;

        private readonly TableLayoutPanel _windowContainer;
        private readonly TableLayoutPanel _stepContainer;
        private readonly Form _window;

        private readonly List<Button> _bigNumbers;
        private readonly List<Button> _smallNumbers;

        private TableLayoutPanel _bigNumberContainer;
        private TableLayoutPanel _smallNumberContainer;

        private readonly Label _goalSpinner;
        private readonly System.Windows.Forms.Timer _spinnerTicker;

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
                BackColor = Color.FromArgb(255, 255, 240),
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
                BackColor = Color.FromArgb(255, 255, 240),
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

            _bigNumbers = new();
            _smallNumbers = new();
            _bigNumberContainer = new();
            _smallNumberContainer = new();

            _openSteps = new()
            {
                BackColor = Color.LightGoldenrodYellow,
                Dock = DockStyle.Right,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Black,
                Text = "Open Solution",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _openSteps.FlatAppearance.BorderSize = 0;

            _goalSpinner = new()
            {
                BackColor = Color.Black,
                Dock = DockStyle.Fill,
                Enabled = true,
                ForeColor = Color.Yellow,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _spinnerTicker = new();
            _spinnerStop = new()
            {
                BackColor = Color.Red,
                Dock = DockStyle.Fill,
                Enabled = false,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Black,
                Text = "STOP!",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _spinnerStop.FlatAppearance.BorderSize = 0;

            _stepInfo = new()
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                TextAlign = HorizontalAlignment.Left,
                WordWrap = false
            };
            _stepReturn = new()
            {
                BackColor = Color.LightGoldenrodYellow,
                Dock = DockStyle.Fill,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.Black,
                Text = "Return",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _stepReturn.FlatAppearance.BorderSize = 0;

            InitializePage();
            Application.Run(_window);
        }

        private void InitializePage()
        {
            //TODO: Randomize positions here
            for (int i = 0; i < _game.BigNumbers.Count; i++)
                _bigNumbers.Add(CreateButton(i, true));

            for (int i = 0; i < _game.SmallNumbers.Count; i++)
                _smallNumbers.Add(CreateButton(i, false));

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
                _goalSpinner.Text = _game.State == ValueGenerator<T>.GenerationPhase.ERROR ? "ERROR" : _game.Goal?.ToString() ?? "GENERATION ERROR";
                _spinnerStop.Enabled = true;
            };
            _spinnerStop.Click += (o, e) =>
            {
                _spinnerTicker.Stop();
                for (int i = 0; i < new Random().Next(0, 2); i++)
                {
                    _game.RandomizeGoal();
                    _goalSpinner.Text = _game.State == ValueGenerator<T>.GenerationPhase.ERROR ? "ERROR" : _game.Goal?.ToString() ?? "GENERATION ERROR";
                }

                _spinnerStop.Enabled = false;

                var steps = _game.GetIntendedSolution();

                for (int i = 0; i < steps.Count; i++)
                {
                    _stepInfo.AppendText(steps[i].ToString());
                    _stepInfo.AppendText(Environment.NewLine);
                }
               
                    _openSteps.Enabled = true;
            };

            _bigNumberContainer = CreateButtonRows(_bigNumbers, MAX_BIG_ROW_BUTTON_COUNT);
            _smallNumberContainer = CreateButtonRows(_smallNumbers, MAX_SMALL_ROW_BUTTON_COUNT);

            _windowContainer.Controls.Add(_bigNumberContainer, 2, 3);
            _windowContainer.SetRowSpan(_bigNumberContainer, 1);
            _windowContainer.SetColumnSpan(_bigNumberContainer, 2);

            _windowContainer.Controls.Add(_smallNumberContainer, 1, 5);
            _windowContainer.SetRowSpan(_smallNumberContainer, 1);
            _windowContainer.SetColumnSpan(_smallNumberContainer, 4);

            _windowContainer.Controls.Add(_openSteps, 3, 1);
            _windowContainer.SetRowSpan(_openSteps, 1);
            _windowContainer.SetColumnSpan(_openSteps, 1);

            _windowContainer.Controls.Add(_goalSpinner, 4, 1);
            _windowContainer.SetRowSpan(_goalSpinner, 1);
            _windowContainer.SetColumnSpan(_goalSpinner, 1);

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
                BackColor = Color.DarkBlue,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            output.FlatAppearance.BorderSize = 0;
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
                output.BackColor = ControlPaint.Light(output.BackColor, 0.5F);
                output.ForeColor = Color.DarkBlue;
                _game.ChooseNumber(pos, isBig);
                output.Enabled = false;

                if(_game.State == ValueGenerator<T>.GenerationPhase.RANDOMIZING)
                {
                    _bigNumbers.ForEach(b => b.Enabled = false);
                    _smallNumbers.ForEach(b => b.Enabled = false);
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
                BackColor = Color.LightBlue,
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
                    BackColor = Color.LightBlue,
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

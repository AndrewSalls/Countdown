using Countdown.ValueImplementations.Representation;

namespace Countdown.GameController
{
    public class SolutionPage<T> : TableLayoutPanel
    {
        private readonly ExpressionInput<T> _baseInput;
        private readonly Button _stepReturn;
        private readonly Button _calculate;
        private readonly Label _result;

        private readonly ValueGenerator<T> _game;
        private readonly ExpressionConverter<T> _converter;

        public SolutionPage(Form window, GamePage<T> mainPage, ValueGenerator<T> game, ExpressionConverter<T> converter)
        {
            _game = game;
            _converter = converter;

            RowCount = 5;
            ColumnCount = 4;
            BackColor = GamePage<T>.BACKGROUND;
            Dock = DockStyle.Fill;
            Enabled = true;
            Visible = true;

            RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

            _stepReturn = new()
            {
                BackColor = GamePage<T>.STEPS_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = GamePage<T>.PLAIN_TEXT,
                Text = "Return",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _stepReturn.FlatAppearance.BorderSize = 0;
            _stepReturn.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            _stepReturn.Click += (o, e) =>
            {
                window.Controls.Remove(this);
                window.Controls.Add(mainPage);
            };

            Controls.Add(_stepReturn, 1, 3);
            SetRowSpan(_stepReturn, 1);
            SetColumnSpan(_stepReturn, 2);

            _baseInput = new()
            {
                AutoScroll = true,
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Enabled = true,
                ForeColor = GamePage<T>.PLAIN_TEXT,
                Visible = true
            };

            Controls.Add(_baseInput, 1, 1);
            SetRowSpan(_baseInput, 1);
            SetColumnSpan(_baseInput, 2);

            _result = new()
            {
                BackColor = GamePage<T>.SPINNER_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                ForeColor = GamePage<T>.SPINNER_TEXT,
                Margin = new Padding(3),
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };

            Controls.Add(_result, 2, 2);
            SetRowSpan(_result, 1);
            SetColumnSpan(_result, 1);

            _calculate = new()
            {
                BackColor = GamePage<T>.STOP_POST_BACKGROUND,
                Dock = DockStyle.Fill,
                Enabled = true,
                FlatStyle = FlatStyle.Flat,
                ForeColor = GamePage<T>.PLAIN_TEXT,
                Text = "Calculate!",
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = true
            };
            _calculate.FlatAppearance.BorderSize = 0;
            _calculate.FlatAppearance.BorderColor = Color.FromArgb(0, 255, 255, 255);

            _calculate.Click += (o, e) =>
            {
                if(CanCalculate())
                {
                    T equalTo = Calculate();
                    mainPage.RenderOnControl(equalTo, _result, GamePage<T>.INFO_TEXT);
                    if (_game.Goal!.Equals(equalTo))
                        _result.ForeColor = Color.LawnGreen;
                    else
                        _result.ForeColor = ControlPaint.Light(Color.Crimson, 0.3f);
                }
            };

            Controls.Add(_calculate, 1, 2);
            SetRowSpan(_calculate, 1);
            SetColumnSpan(_calculate, 1);
        }

        private bool CanCalculate()
        {
            throw new NotImplementedException();
        }

        private T Calculate()
        {
            throw new NotImplementedException();
        }
    }
}

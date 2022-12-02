namespace Countdown.GameController
{
    public class StepPage<T> : TableLayoutPanel
    {
        private readonly Panel _panelEmbed;
        private readonly Button _stepReturn;

        public StepPage(Form window, TableLayoutPanel mainPage)
        {
            RowCount = 4;
            ColumnCount = 3;
            BackColor = GamePage<T>.BACKGROUND;
            Dock = DockStyle.Fill;
            Enabled = true;
            Visible = true;

            RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            RowStyles.Add(new RowStyle(SizeType.Percent, 70));
            RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            RowStyles.Add(new RowStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80));
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));

            _panelEmbed = new()
            {
                AutoScroll = true,
                Dock = DockStyle.Fill
            };

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

            Controls.Add(_panelEmbed, 1, 1);
            SetRowSpan(_panelEmbed, 1);
            SetColumnSpan(_panelEmbed, 1);

            Controls.Add(_stepReturn, 1, 2);
            SetRowSpan(_stepReturn, 1);
            SetColumnSpan(_stepReturn, 1);
        }

        public void Clear()
        {
            _panelEmbed.Controls.Clear();
        }

        public void Display(Control cRep)
        {
            _panelEmbed.Controls.Add(cRep);
            _panelEmbed.Controls[0].Dock = DockStyle.Fill;
        }
    }
}

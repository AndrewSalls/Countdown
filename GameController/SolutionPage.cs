namespace Countdown.GameController
{
    public class SolutionPage<T, U> : TableLayoutPanel
    {
        public SolutionPage()
        {
            RowCount = 4;
            ColumnCount = 3;
            BackColor = GamePage<T, U>.BACKGROUND;
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
        }
    }
}

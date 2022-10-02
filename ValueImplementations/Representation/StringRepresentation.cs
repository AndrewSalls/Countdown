using Countdown.ValueImplementations.Values;

namespace Countdown.ValueImplementations.Representation
{
    public class StringRepresentation<T> : IRepresentation<T, string>
    {
        private readonly ToRep _toRep;
        private readonly FromRep _fromRep;

        public StringRepresentation(ToRep toRep, FromRep fromRep)
        {
            _toRep = toRep;
            _fromRep = fromRep;
        }

        public override void AppendRepresentation(Control c, string rep)
        {
            if (c is TextBox box)
                box.AppendText(rep);
        }

        public override string CreateErrorRepresentation() => "ERROR";

        public override string CreateExpression(string left, SymbolRepresentation<string> symbol, string right) => symbol.ApplySymbol(left, right);

        public override bool IsParenthesized(string rep) => rep.StartsWith("(") && rep.EndsWith(")");

        public override string Parenthesize(string rep) => $"({rep})";

        public override string SetEqualTo(string exp, string equalTo) => $"{exp} = {equalTo}";

        public override void AppendLineBreak(Control c)
        {
            if (c is TextBox box)
                box.AppendText(Environment.NewLine);
        }

        public override Control CreateDisplayRepresentationBase()
        {
            return new TextBox()
            {
                BackColor = NumberPickerMenu<T, string>.INFO_BACKGROUND,
                Dock = DockStyle.Fill,
                ForeColor = NumberPickerMenu<T, string>.INFO_TEXT,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                TextAlign = HorizontalAlignment.Left,
                WordWrap = false
            };
        }

        public override string AsRepresentation(T value) => _toRep(value);

        protected override T FromRepresentation(string representation) => _fromRep(representation);

        public delegate string ToRep(T value);
        public delegate T FromRep(string value);
    }
}

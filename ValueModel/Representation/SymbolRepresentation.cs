namespace Countdown.ValueImplementations.Representation
{
    public class SymbolRepresentation
    {
        public Image Symbol { get; private set; }
        public Apply ApplySymbol { get; private set; }

        public SymbolRepresentation(Image symbol, Apply app)
        {
            Symbol = symbol;
            ApplySymbol = app;
        }

        public delegate ImageTreeNode Apply(ImageTreeNode left, Image symbol, ImageTreeNode right);
        public delegate bool OperatesOn(ImageTreeNode rep);
    }
}

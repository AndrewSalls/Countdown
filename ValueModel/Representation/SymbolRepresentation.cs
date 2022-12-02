namespace Countdown.ValueModel.Representation
{
    public class SymbolRepresentation
    {
        public string Name { get; private set; }
        public Representation Symbol { get; private set; }

        public SymbolRepresentation(string name, Representation symbol)
        {
            Name = name;
            Symbol = symbol;
        }

        public delegate Image Representation(Color color, int imageRowHeight);
    }
}

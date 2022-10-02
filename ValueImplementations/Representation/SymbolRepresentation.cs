namespace Countdown.ValueImplementations.Representation
{
    public class SymbolRepresentation<U>
    {
        public U Symbol { get; private set; }
        public Apply ApplySymbol { get; private set; }
        public OperatesOn IsOperator { get; private set; }

        public SymbolRepresentation(U symbol, Apply app, OperatesOn rec)
        {
            Symbol = symbol;
            ApplySymbol = app;
            IsOperator = rec;
        }

        public delegate U Apply(U left, U right);
        public delegate bool OperatesOn(U rep);
    }
}

using Countdown.ValueImplementations.Representation;

namespace Countdown.ValueImplementations.Values
{
    public abstract class IRepresentation<T, U>
    {
        public abstract U AsRepresentation(T value);
        protected abstract T FromRepresentation(U representation);

        public abstract U Parenthesize(U rep);
        public abstract U SetEqualTo(U exp, U equalTo);
        public abstract U CreateErrorRepresentation();
        public abstract U CreateExpression(U left, SymbolRepresentation<U> symbol, U right);

        public abstract bool IsParenthesized(U rep);

        public abstract Control CreateDisplayRepresentationBase();
        public abstract void AppendRepresentation(Control c, U rep);
        public abstract void AppendLineBreak(Control c);
    }
}

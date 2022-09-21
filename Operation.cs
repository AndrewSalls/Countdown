namespace Countdown
{
    public class Operation<T> where T : IStringRepresentable<T>
    {
        public string Symbol { get; private set; }
        public T? LeftValue { get; private set; }
        public T? RightValue { get; private set; }
        public T? Result { get; private set; }
        public int Priority { get; private set; }

        private readonly Evaluation eval;
        private readonly Verification verify;
        private readonly ToStringSymbol display;

        public Operation(string symbol, int priority, Evaluation eval, Verification verify, ToStringSymbol display)
        {
            this.Symbol = symbol;
            this.eval = eval;
            this.verify = verify;
            this.display = display;
            LeftValue = default;
            RightValue = default;
            Result = default;
            Priority = priority;
        }
        public Operation(string symbol, int priority, Evaluation eval, Verification verify) : this(symbol, priority, eval, verify, (a, b) => $"{a.AsString()} {symbol} {b.AsString()}")
        {
        }

        public Operation<T> FixEvaluation(T a, T b)
        {
            Operation<T> output = new(Symbol, Priority, eval, verify, display);
            if (IsEvaluable(a, b))
                output.Result = Evaluate(a, b);

            output.LeftValue = a;
            output.RightValue = b;

            return output;
        }

        public T Evaluate(T a, T b) => eval(a, b);
        public bool IsEvaluable(T a, T b) => verify(a, b);

        public delegate T Evaluation(T a, T b);
        public delegate bool Verification(T a, T b);

        public delegate string ToStringSymbol(T a, T b);

        public string ToEquationString()
        {
            if (LeftValue is null || RightValue is null || Result is null)
                throw new InvalidOperationException("Part of equation is null.");

            return display(LeftValue, RightValue) + " = " + Result;
        }

        public string ToExpressionString()
        {
            if (LeftValue is null || RightValue is null || Result is null)
                throw new InvalidOperationException("Part of expression is null.");

            return display(LeftValue, RightValue);
        }
    }
}

namespace Countdown
{
    public class Operation<T>
    {
        public string Symbol { get; private set; }
        public T? LeftValue { get; private set; }
        public T? RightValue { get; private set; }
        public T? Result { get; private set; }

        private readonly Evaluation eval;
        private readonly Verification verify;
        private readonly ToStringSymbol display;

        public Operation(string symbol, Evaluation eval, Verification verify, ToStringSymbol display)
        {
            this.Symbol = symbol;
            this.eval = eval;
            this.verify = verify;
            this.display = display;
            LeftValue = default;
            RightValue = default;
            Result = default;
        }
        public Operation(string symbol, Evaluation eval, Verification verify) : this(symbol, eval, verify, (a, b) => $"{a} {symbol} {b}")
        {
        }

        public Operation<T> FixEvaluation(T a, T b)
        {
            Operation<T> output = new(Symbol, eval, verify, display);
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

        public override string ToString()
        {
            if (LeftValue is null || RightValue is null || Result is null)
                return Symbol;

            return display(LeftValue, RightValue) + " = " + Result;
        }
    }
}

namespace Countdown
{
    public class Operation<T>
    {
        public string Symbol { get; private set; }

        private readonly Evaluation eval;
        private readonly Verification verify;

        public Operation(string symbol, Evaluation eval, Verification verify)
        {
            this.Symbol = symbol;
            this.eval = eval;
            this.verify = verify;
        }

        public T Evaluate(T a, T b) => eval(a, b);
        public bool IsEvaluable(T a, T b) => verify(a, b);

        public delegate T Evaluation(T a, T b);
        public delegate bool Verification(T a, T b);
    }
}

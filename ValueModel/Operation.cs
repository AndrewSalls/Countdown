using Countdown.ValueImplementations.Representation;

namespace Countdown.ValueImplementations
{
    public class Operation<T>
    {
        public int Priority { get; private set; }
        public bool IsAssociative { get; private set; }
        public bool IsCommutative { get; private set; }

        public readonly Evaluation eval;
        public readonly Verification verify;

        public Operation(int priority, bool isAssociative, bool isCommutative, Evaluation eval, Verification verify)
        {
            this.eval = eval;
            this.verify = verify;
            Priority = priority;
            IsAssociative = isAssociative;
            IsCommutative = isCommutative;
        }

        public T Evaluate(T a, T b) => eval(a, b);
        public bool IsEvaluable(T a, T b) => verify(a, b);

        public delegate T Evaluation(T a, T b);
        public delegate bool Verification(T a, T b);
    }
}

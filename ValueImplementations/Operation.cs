using Countdown.ValueImplementations.Values;

namespace Countdown.ValueImplementations
{
    public class Operation<T, U> where T : IRepresentable<T, U>
    {
        public string Symbol { get; private set; }
        public T? LeftValue { get; private set; }
        public T? RightValue { get; private set; }
        public T? Result { get; private set; }
        public int Priority { get; private set; }
        public bool IsAssociative { get; private set; }
        public bool IsCommutative { get; private set; }

        public readonly Evaluation eval;
        public readonly Verification verify;
        public readonly OperationRepresentation display;

        public Operation(string symbol, int priority, bool isAssociative, bool isCommutative, Evaluation eval, Verification verify, OperationRepresentation display)
        {
            Symbol = symbol;
            this.eval = eval;
            this.verify = verify;
            this.display = display;
            LeftValue = default;
            RightValue = default;
            Result = default;
            Priority = priority;
            IsAssociative = isAssociative;
            IsCommutative = isCommutative;
        }
        public Operation(string symbol, int priority, bool isAssociative, bool isCommutative, Evaluation eval, Verification verify) : this(symbol, priority, isAssociative, isCommutative, eval, verify, (a, b) => $"{a} {symbol} {b}")
        {
        }

        public Operation<T, U> FixEvaluation(T a, T b)
        {
            Operation<T, U> output = new(Symbol, Priority, IsAssociative, IsCommutative, eval, verify, display);
            if (IsEvaluable(a, b))
                output.Result = Evaluate(a, b);

            output.LeftValue = a;
            output.RightValue = b;

            return output;
        }

        public T Evaluate(T a, T b) => eval(a, b);
        public bool IsEvaluable(T a, T b) => verify(a, b);

        //Modify to properly handle adding equals sign
        public U ToEquation()
        {
            if (LeftValue is null || RightValue is null || Result is null)
                throw new InvalidOperationException("Part of equation is null.");

            return display(LeftValue.AsRepresentation(), RightValue.AsRepresentation()) + " = " + Result.AsRepresentation();
        }

        public U ToExpression()
        {
            if (LeftValue is null || RightValue is null)
                throw new InvalidOperationException("Part of expression is null.");

            return display(LeftValue.AsRepresentation(), RightValue.AsRepresentation());
        }

        public U ToExpression(T left, T right) => display(left.AsRepresentation(), right.AsRepresentation());

        public U ArbitraryExpression(U left, U right) => display(left, right);

        public delegate T Evaluation(T a, T b);
        public delegate bool Verification(T a, T b);
        public delegate U OperationRepresentation(U a, U b);
    }
}

﻿namespace Countdown
{
    public class Operation<T> where T : IStringRepresentable<T>
    {
        public string Symbol { get; private set; }
        public T? LeftValue { get; private set; }
        public T? RightValue { get; private set; }
        public T? Result { get; private set; }
        public int Priority { get; private set; }
        public bool IsAssociative { get; private set; }
        public bool IsCommutative { get; private set; }

        private readonly Evaluation eval;
        private readonly Verification verify;
        private readonly ToStringSymbol display;

        public Operation(string symbol, int priority, bool isAssociative, bool isCommutative, Evaluation eval, Verification verify, ToStringSymbol display)
        {
            this.Symbol = symbol;
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

        public Operation<T> FixEvaluation(T a, T b)
        {
            Operation<T> output = new(Symbol, Priority, IsAssociative, IsCommutative, eval, verify, display);
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

        public delegate string ToStringSymbol(string a, string b);

        public string ToEquationString()
        {
            if (LeftValue is null || RightValue is null || Result is null)
                throw new InvalidOperationException("Part of equation is null.");

            return display(LeftValue.AsString(), RightValue.AsString()) + " = " + Result.AsString();
        }

        public string ToExpressionString()
        {
            if (LeftValue is null || RightValue is null)
                throw new InvalidOperationException("Part of expression is null.");

            return display(LeftValue.AsString(), RightValue.AsString());
        }

        public string ToExpressionString(T left, T right) => display(left.AsString(), right.AsString());

        public string ArbitraryExpressionString(string left, string right) => display(left, right);
    }
}

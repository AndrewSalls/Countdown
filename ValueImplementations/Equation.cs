using Countdown.ValueImplementations.Values;

namespace Countdown.ValueImplementations
{
    public record EquationFactor<T> where T : IStringRepresentable<T>
    {
        public bool IsValue { get; private set; }
        public T Value { get { return _value!; } set { _value = value; _subEquation = null; IsValue = true; } }
        private T? _value;
        public Equation<T> SubEquation { get { return _subEquation!; } set { _subEquation = value; _value = default; IsValue = false; } }
        private Equation<T>? _subEquation;

        public EquationFactor(T val)
        {
            _value = val;
            _subEquation = null;
            IsValue = true;
        }
        public EquationFactor(Equation<T> sub)
        {
            _value = default;
            _subEquation = sub;
            IsValue = false;
        }
    }

    public class Equation<T> where T : IStringRepresentable<T>
    {
        //Contains a left and right value, which are either another equationfactor or a value of type T
        public Operation<T> Operation { get; private set; }
        public EquationFactor<T> Left { get; private set; }
        public EquationFactor<T> Right { get; private set; }

        public Equation(Operation<T> op, EquationFactor<T> left, EquationFactor<T> right)
        {
            Operation = op;
            Left = left;
            Right = right;
        }
        public Equation(Operation<T> op, T left, T right) : this(op, new EquationFactor<T>(left), new EquationFactor<T>(right))
        {
        }
        public Equation(Operation<T> op, EquationFactor<T> left, T right) : this(op, left, new EquationFactor<T>(right)) { }
        public Equation(Operation<T> op, T left, EquationFactor<T> right) : this(op, new EquationFactor<T>(left), right) { }

        public static Equation<T> ConvertStepsToEquation(IReadOnlyList<Operation<T>> steps)
        {
            List<Equation<T>> subSteps = new();

            foreach (Operation<T> step in steps)
            {
                int leftSubIndex = subSteps.FindIndex(e => e.Operation.Result!.IsEquivalentTo(step.LeftValue));
                int rightSubIndex = subSteps.FindLastIndex(e => e.Operation.Result!.IsEquivalentTo(step.RightValue));

                if (rightSubIndex == leftSubIndex)
                    rightSubIndex = -1;

                if (leftSubIndex > -1)
                {
                    if (rightSubIndex > -1)
                    {
                        subSteps.Add(new Equation<T>(step, new EquationFactor<T>(subSteps[leftSubIndex]), new EquationFactor<T>(subSteps[rightSubIndex])));
                        if (rightSubIndex < leftSubIndex)
                        {
                            subSteps.RemoveAt(leftSubIndex);
                            subSteps.RemoveAt(rightSubIndex);
                        }
                        else
                        {
                            subSteps.RemoveAt(rightSubIndex);
                            subSteps.RemoveAt(leftSubIndex);
                        }
                    }
                    else
                    {
                        subSteps.Add(new Equation<T>(step, new EquationFactor<T>(subSteps[leftSubIndex]), step.RightValue!));
                        subSteps.RemoveAt(leftSubIndex);
                    }
                }
                else
                {
                    if (rightSubIndex > -1)
                    {
                        subSteps.Add(new Equation<T>(step, step.LeftValue!, new EquationFactor<T>(subSteps[rightSubIndex])));
                        subSteps.RemoveAt(rightSubIndex);
                    }
                    else
                        subSteps.Add(new Equation<T>(step, step.LeftValue!, step.RightValue!));
                }
            }

            return subSteps.First();
        }

        public string ConvertToString()
        {
            string left, right;

            if (!Left.IsValue)
            {
                Equation<T> leftEq = Left.SubEquation;
                Operation<T> leftOp = leftEq.Operation;
                if (leftOp.Priority < Operation.Priority)
                    left = $"({leftEq.ConvertToString()})";
                else
                    left = leftEq.ConvertToString();
            }
            else
                left = Left.Value.AsString();

            if (!Right.IsValue)
            {
                Equation<T> rightEq = Right.SubEquation;
                Operation<T> rightOp = rightEq.Operation;
                if (rightOp.Priority < Operation.Priority)
                    right = $"({rightEq.ConvertToString()})";
                else if (rightOp.Priority == Operation.Priority && !Operation.IsAssociative)
                    right = $"({rightEq.ConvertToString()})";
                else
                    right = rightEq.ConvertToString();
            }
            else
                right = Right.Value.AsString();

            if (left.StartsWith("(") && !right.StartsWith("(") && Operation.IsCommutative)
                (left, right) = (right, left);

            return Operation.ArbitraryExpressionString(left, right);
        }
        public List<Operation<T>> ConvertFromEquation(string equation)
        {
            //Find any equations in parenthesis and convert those, and repeat until there are no operations remaining
            throw new NotImplementedException();
        }

        private List<Operation<T>> ConvertFromSimpleEquation(string equation)
        {
            //Converts equation where the order of operations does not matter
            throw new NotImplementedException();
        }
    }
}

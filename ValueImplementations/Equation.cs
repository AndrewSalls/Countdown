using Countdown.ValueImplementations.Values;

namespace Countdown.ValueImplementations
{
    public record EquationFactor<T, U> where T : IRepresentable<T, U>
    {
        public bool IsValue { get; private set; }
        public T Value { get { return _value!; } set { _value = value; _subEquation = null; IsValue = true; } }
        private T? _value;
        public Equation<T, U> SubEquation { get { return _subEquation!; } set { _subEquation = value; _value = default; IsValue = false; } }
        private Equation<T, U>? _subEquation;

        public EquationFactor(T val)
        {
            _value = val;
            _subEquation = null;
            IsValue = true;
        }
        public EquationFactor(Equation<T, U> sub)
        {
            _value = default;
            _subEquation = sub;
            IsValue = false;
        }
    }

    public class Equation<T, U> where T : IRepresentable<T, U>
    {
        //Contains a left and right value, which are either another equationfactor or a value of type T
        public Operation<T, U> Operation { get; private set; }
        public EquationFactor<T, U> Left { get; private set; }
        public EquationFactor<T, U> Right { get; private set; }

        public Equation(Operation<T, U> op, EquationFactor<T, U> left, EquationFactor<T, U> right)
        {
            Operation = op;
            Left = left;
            Right = right;
        }
        public Equation(Operation<T, U> op, T left, T right) : this(op, new EquationFactor<T, U>(left), new EquationFactor<T, U>(right))
        {
        }
        public Equation(Operation<T, U> op, EquationFactor<T, U> left, T right) : this(op, left, new EquationFactor<T, U>(right)) { }
        public Equation(Operation<T, U> op, T left, EquationFactor<T, U> right) : this(op, new EquationFactor<T, U>(left), right) { }

        public static Equation<T, U> ConvertStepsToEquation(IReadOnlyList<Operation<T, U>> steps)
        {
            List<Equation<T, U>> subSteps = new();

            foreach (Operation<T, U> step in steps)
            {
                int leftSubIndex = subSteps.FindIndex(e => e.Operation.Result!.IsEquivalentTo(step.LeftValue!));
                int rightSubIndex = subSteps.FindLastIndex(e => e.Operation.Result!.IsEquivalentTo(step.RightValue!));

                if (rightSubIndex == leftSubIndex)
                    rightSubIndex = -1;

                if (leftSubIndex > -1)
                {
                    if (rightSubIndex > -1)
                    {
                        subSteps.Add(new Equation<T, U>(step, new EquationFactor<T, U>(subSteps[leftSubIndex]), new EquationFactor<T, U>(subSteps[rightSubIndex])));
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
                        subSteps.Add(new Equation<T, U>(step, new EquationFactor<T, U>(subSteps[leftSubIndex]), step.RightValue!));
                        subSteps.RemoveAt(leftSubIndex);
                    }
                }
                else
                {
                    if (rightSubIndex > -1)
                    {
                        subSteps.Add(new Equation<T, U>(step, step.LeftValue!, new EquationFactor<T, U>(subSteps[rightSubIndex])));
                        subSteps.RemoveAt(rightSubIndex);
                    }
                    else
                        subSteps.Add(new Equation<T, U>(step, step.LeftValue!, step.RightValue!));
                }
            }

            return subSteps.First();
        }

        public U ConvertToRepresentation()
        {
            U left, right;

            if (!Left.IsValue)
            {
                Equation<T, U> leftEq = Left.SubEquation;
                Operation<T, U> leftOp = leftEq.Operation;
                if (leftOp.Priority < Operation.Priority)
                    left = $"({leftEq.ConvertToRepresentation()})";
                else
                    left = leftEq.ConvertToRepresentation();
            }
            else
                left = Left.Value.AsRepresentation();

            if (!Right.IsValue)
            {
                Equation<T, U> rightEq = Right.SubEquation;
                Operation<T, U> rightOp = rightEq.Operation;
                if (rightOp.Priority < Operation.Priority)
                    right = $"({rightEq.ConvertToRepresentation()})";
                else if (rightOp.Priority == Operation.Priority && !Operation.IsAssociative)
                    right = $"({rightEq.ConvertToRepresentation()})";
                else
                    right = rightEq.ConvertToRepresentation();
            }
            else
                right = Right.Value.AsRepresentation();

            if (left.StartsWith("(") && !right.StartsWith("(") && Operation.IsCommutative)
                (left, right) = (right, left);

            return Operation.ArbitraryExpression(left, right);
        }
        public List<Operation<T, U>> ConvertFromEquation(string equation)
        {
            //Find any equations in parenthesis and convert those, and repeat until there are no operations remaining
            throw new NotImplementedException();
        }

        private List<Operation<T, U>> ConvertFromSimpleEquation(string equation)
        {
            //Converts equation where the order of operations does not matter
            throw new NotImplementedException();
        }
    }
}

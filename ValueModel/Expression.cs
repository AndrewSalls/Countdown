namespace Countdown.ValueImplementations
{
    public class Expression<T>
    {
        public Operation<T> Operation { get; private set; }
        public ExpressionFactor<T> Left { get; private set; }
        public ExpressionFactor<T> Right { get; private set; }
        public T Result { get; private set; }

        public Expression(Operation<T> op, ExpressionFactor<T> left, ExpressionFactor<T> right)
        {
            Operation = op;
            Left = left;
            Right = right;

            if (op.IsEvaluable(left.Value, right.Value))
                Result = op.Evaluate(left.Value, right.Value);
            else
                throw new ArgumentException("Operation cannot use the values provided to it.");
        }
        public Expression(Operation<T> op, T left, T right) : this(op, new ExpressionFactor<T>(left), new ExpressionFactor<T>(right))
        {
        }
        public Expression(Operation<T> op, ExpressionFactor<T> left, T right) : this(op, left, new ExpressionFactor<T>(right)) { }
        public Expression(Operation<T> op, T left, ExpressionFactor<T> right) : this(op, new ExpressionFactor<T>(left), right) { }

        public static Expression<T> ConvertStepsToEquation(IReadOnlyList<Expression<T>> steps)
        {
            List<Expression<T>> subSteps = new();

            foreach (Expression<T> step in steps)
            {
                System.Diagnostics.Debug.WriteLine(subSteps.Count);
                int leftSubIndex = subSteps.FindIndex(e => e.Result!.Equals(step.Left.Value));
                int rightSubIndex = subSteps.FindLastIndex(e => e.Result!.Equals(step.Right.Value));

                if (rightSubIndex == leftSubIndex)
                    rightSubIndex = -1;

                if (leftSubIndex > -1)
                {
                    if (rightSubIndex > -1)
                    {
                        subSteps.Add(new Expression<T>(step.Operation, new ExpressionFactor<T>(subSteps[leftSubIndex]), new ExpressionFactor<T>(subSteps[rightSubIndex])));
                        if (rightSubIndex < leftSubIndex)
                            (rightSubIndex, leftSubIndex) = (leftSubIndex, rightSubIndex);

                        subSteps.RemoveAt(rightSubIndex);
                        subSteps.RemoveAt(leftSubIndex);
                    }
                    else
                    {
                        subSteps.Add(new Expression<T>(step.Operation, new ExpressionFactor<T>(subSteps[leftSubIndex]), step.Right!.Value));
                        subSteps.RemoveAt(leftSubIndex);
                    }
                }
                else
                {
                    if (rightSubIndex > -1)
                    {
                        subSteps.Add(new Expression<T>(step.Operation, step.Left!.Value, new ExpressionFactor<T>(subSteps[rightSubIndex])));
                        subSteps.RemoveAt(rightSubIndex);
                    }
                    else
                        subSteps.Add(step);
                }
            }

            return subSteps.First();
        }
    }
}

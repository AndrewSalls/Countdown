using Countdown.ValueImplementations.Values;

namespace Countdown.ValueImplementations.Representation
{
    public class ExpressionConverter<T, U>
    {
        public IRepresentation<T, U> Representer { get; private set; }
        private readonly Dictionary<Operation<T>, SymbolRepresentation<U>> _operationRepresentation;

        public ExpressionConverter(IRepresentation<T, U> converter, Dictionary<Operation<T>, SymbolRepresentation<U>> operationRepresentations)
        {
            Representer = converter;
            _operationRepresentation = operationRepresentations;
        }

        public U ConvertToRepresentation(Expression<T> exp)
        {
            U left, right;

            if(exp.Left.IsValue && exp.Right.IsValue)
            {
                return Representer.CreateExpression(Representer.AsRepresentation(exp.Left.Value), _operationRepresentation[exp.Operation], Representer.AsRepresentation(exp.Right.Value));
            }

            if (!exp.Left.IsValue)
            {
                Expression<T> leftEq = exp.Left.SubEquation;
                Operation<T> leftOp = leftEq.Operation;
                if (leftOp.Priority < exp.Operation.Priority)
                    left = Representer.Parenthesize(ConvertToRepresentation(leftEq));
                else
                    left = ConvertToRepresentation(leftEq);
            }
            else
                left = Representer.AsRepresentation(exp.Left.Value);

            if (!exp.Right.IsValue)
            {
                Expression<T> rightEq = exp.Right.SubEquation;
                Operation<T> rightOp = rightEq.Operation;
                if (rightOp.Priority < exp.Operation.Priority)
                    right = Representer.Parenthesize(ConvertToRepresentation(rightEq));
                else if (rightOp.Priority == exp.Operation.Priority && !exp.Operation.IsAssociative)
                    right = Representer.Parenthesize(ConvertToRepresentation(rightEq));
                else
                    right = ConvertToRepresentation(rightEq);
            }
            else
                right = Representer.AsRepresentation(exp.Right.Value);

            if (Representer.IsParenthesized(left) && !Representer.IsParenthesized(right) && exp.Operation.IsCommutative)
                (left, right) = (right, left);

            return Representer.CreateExpression(left, _operationRepresentation[exp.Operation], right);
        }

        public Expression<T> ConvertFromRepresentation(U expression)
        {
            //Find any equations in parenthesis and convert those, and repeat until there are no operations remaining
            throw new NotImplementedException();
        }

        private Expression<T> ConvertFromSimpleEquation(U expression)
        {
            //Converts equation where the order of operations does not matter
            throw new NotImplementedException();
        }

        public Control CreateDisplayableRepresentation(IReadOnlyList<Expression<T>> steps)
        {
            Control output = Representer.CreateDisplayRepresentationBase();

            for (int i = 0; i < steps.Count; i++)
            {
                Representer.AppendRepresentation(output,
                    Representer.SetEqualTo(
                        Representer.CreateExpression(Representer.AsRepresentation(steps[i].Left.Value),
                                                     _operationRepresentation[steps[i].Operation],
                                                     Representer.AsRepresentation(steps[i].Right.Value)),
                        Representer.AsRepresentation(steps[i].Result)));
                Representer.AppendLineBreak(output);
            }
            Representer.AppendLineBreak(output);
            Representer.AppendRepresentation(output, Representer.SetEqualTo(ConvertToRepresentation(Expression<T>.ConvertStepsToEquation(steps)), Representer.AsRepresentation(steps[steps.Count - 1].Result)));

            return output;
        }
    }
}

namespace Countdown.ValueImplementations.Representation
{
    public class ExpressionConverter<T>
    {
        public ImageRepresentation<T> Representer { get; private set; }
        private readonly Dictionary<Operation<T>, SymbolRepresentation> _operationRepresentation;

        public ExpressionConverter(ImageRepresentation<T> converter, Dictionary<Operation<T>, SymbolRepresentation> operationRepresentations)
        {
            Representer = converter;
            _operationRepresentation = operationRepresentations;
        }

        public ImageTreeNode ConvertToRepresentation(Expression<T> exp, Size bBox)
        {
            ImageTreeNode left, right;

            if(exp.Left.IsValue && exp.Right.IsValue)
            {
                return Representer.CreateExpression(Representer.AsRepresentation(exp.Left.Value, bBox), _operationRepresentation[exp.Operation], Representer.AsRepresentation(exp.Right.Value, bBox));
            }

            if (!exp.Left.IsValue)
            {
                Expression<T> leftEq = exp.Left.SubEquation;
                Operation<T> leftOp = leftEq.Operation;
                if (leftOp.Priority < exp.Operation.Priority)
                    left = Representer.Parenthesize(ConvertToRepresentation(leftEq, bBox));
                else
                    left = ConvertToRepresentation(leftEq, bBox);
            }
            else
                left = Representer.AsRepresentation(exp.Left.Value, bBox);

            if (!exp.Right.IsValue)
            {
                Expression<T> rightEq = exp.Right.SubEquation;
                Operation<T> rightOp = rightEq.Operation;
                if (rightOp.Priority < exp.Operation.Priority)
                    right = Representer.Parenthesize(ConvertToRepresentation(rightEq, bBox));
                else if (rightOp.Priority == exp.Operation.Priority && !exp.Operation.IsAssociative)
                    right = Representer.Parenthesize(ConvertToRepresentation(rightEq, bBox));
                else
                    right = ConvertToRepresentation(rightEq, bBox);
            }
            else
                right = Representer.AsRepresentation(exp.Right.Value, bBox);

            if (Representer.IsParenthesized(left) && !Representer.IsParenthesized(right) && exp.Operation.IsCommutative)
                (left, right) = (right, left);

            return Representer.CreateExpression(left, _operationRepresentation[exp.Operation], right);
        }

        public Expression<T> ConvertFromRepresentation(ImageTreeNode expression)
        {
            //Find any equations in parenthesis and convert those, and repeat until there are no operations remaining
            throw new NotImplementedException();
        }

        private Expression<T> ConvertFromSimpleEquation(ImageTreeNode expression)
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
                        Representer.CreateExpression(Representer.AsRepresentation(steps[i].Left.Value, output.Size),
                                                     _operationRepresentation[steps[i].Operation],
                                                     Representer.AsRepresentation(steps[i].Right.Value, output.Size)),
                        Representer.AsRepresentation(steps[i].Result, output.Size)));
            }
            Representer.AppendRepresentation(output, Representer.SetEqualTo(ConvertToRepresentation(Expression<T>.ConvertStepsToEquation(steps), output.Size), Representer.AsRepresentation(steps[steps.Count - 1].Result, output.Size)));

            return output;
        }
    }
}

using Countdown.ValueModel.Representation;

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

        public ImageTreeNode<T> ConvertToRepresentation(Expression<T> exp, int imageRowHeight, bool treatSimple = false)
        {
            ImageTreeNode<T> left, right;

            if(treatSimple || (exp.Left.IsValue && exp.Right.IsValue))
                return Representer.CreateExpression(new(exp.Left.Value, Representer), _operationRepresentation[exp.Operation], new(exp.Right.Value, Representer));

            if (!exp.Left.IsValue)
            {
                Expression<T> leftEq = exp.Left.SubEquation;
                Operation<T> leftOp = leftEq.Operation;
                if (leftOp.Priority < exp.Operation.Priority)
                    left = Representer.Parenthesize(ConvertToRepresentation(leftEq, imageRowHeight));
                else
                    left = ConvertToRepresentation(leftEq, imageRowHeight);
            }
            else
                left = new(exp.Left.Value, Representer);

            if (!exp.Right.IsValue)
            {
                Expression<T> rightEq = exp.Right.SubEquation;
                Operation<T> rightOp = rightEq.Operation;
                if (rightOp.Priority < exp.Operation.Priority)
                    right = Representer.Parenthesize(ConvertToRepresentation(rightEq, imageRowHeight));
                else if (rightOp.Priority == exp.Operation.Priority && !exp.Operation.IsAssociative)
                    right = Representer.Parenthesize(ConvertToRepresentation(rightEq, imageRowHeight));
                else
                    right = ConvertToRepresentation(rightEq, imageRowHeight);
            }
            else
                right = new(exp.Right.Value, Representer);

            if (Representer.IsParenthesized(left) && !Representer.IsParenthesized(right) && exp.Operation.IsCommutative)
                (left, right) = (right, left);

            return Representer.CreateExpression(left, _operationRepresentation[exp.Operation], right);
        }

        public Control CreateDisplayableRepresentation(IReadOnlyList<Expression<T>> steps, Color color, int imageRowHeight)
        {
            Control output = Representer.CreateDisplayRepresentationBase();

            for (int i = 0; i < steps.Count; i++)
            {
                Representer.AppendRepresentation(output, Representer.SetEqualTo(ConvertToRepresentation(steps[i], imageRowHeight, true),
                                                         new(steps[i].Result, Representer)), color, imageRowHeight);
            }
            Representer.AppendRepresentation(output, Representer.SetEqualTo(ConvertToRepresentation(Expression<T>.ConvertStepsToEquation(steps), imageRowHeight),
                                                                            new(steps[steps.Count - 1].Result, Representer)), color, imageRowHeight);

            return output;
        }
    }
}

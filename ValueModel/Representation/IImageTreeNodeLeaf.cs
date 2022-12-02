using Countdown.ValueImplementations.Representation;

namespace Countdown.ValueModel.Representation
{
    public interface IImageTreeNodeLeaf
    {
        bool IsEquivalent(object comparison);
        Image Render(Color color, int imageRowHeight);
    }

    public class ValueLeaf<T> : IImageTreeNodeLeaf
    {
        public T Value { get; private set; }
        private readonly ImageRepresentation<T> _rep;

        public ValueLeaf(T val, ImageRepresentation<T> rep)
        {
            Value = val;
            _rep = rep;
        }

        public bool IsEquivalent(object comparison) => comparison.Equals(Value);
        public Image Render(Color color, int imageRowHeight) => _rep.AsRepresentation(Value, color, imageRowHeight);
    }

    public class SymbolLeaf : IImageTreeNodeLeaf
    {
        public SymbolRepresentation Value { get; private set; }

        public SymbolLeaf(SymbolRepresentation val) => Value = val;

        public bool IsEquivalent(object comparison) => comparison is SymbolRepresentation representation && representation.Name.Equals(Value.Name);
        public Image Render(Color color, int imageRowHeight) => Value.Symbol(color, imageRowHeight);
    }
}

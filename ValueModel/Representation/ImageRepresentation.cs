using Countdown.GameController;
using Countdown.ValueModel.Representation;

namespace Countdown.ValueImplementations.Representation
{
    public class ImageRepresentation<T>
    {
        private readonly Representation _asRep;
        private readonly MaximizeScale _scaler;

        public ImageRepresentation(Representation caster, MaximizeScale scaler)
        {
            _asRep = caster;
            _scaler = scaler;
        }

        public Image AsRepresentation(T value, Color color, Size bBox, int minScale = -1) => _asRep(value, color, bBox, minScale);
        public Image AsRepresentation(T value, Color color, int imageRowHeight) => _asRep(value, color, new Size(), imageRowHeight);

        public ImageTreeNode<T> CreateLeftParenthesis() => new(new SymbolRepresentation("(", (c, mh) => ImageFactory.CreateImage("(", c, mh)));
        public ImageTreeNode<T> CreateRightParenthesis() => new(new SymbolRepresentation(")", (c, mh) => ImageFactory.CreateImage(")", c, mh)));
        public ImageTreeNode<T> CreateEqualsSign() => new(new SymbolRepresentation("=", (c, mh) => ImageFactory.CreateImage("=", c, mh)));

        public void AppendRepresentation(Control c, ImageTreeNode<T> rep, Color color, int imageRowHeight)
        {
            if (c.BackgroundImage != null)
                c.BackgroundImage = ImageFactory.CombineImagesVertical(c.BackgroundImage, rep.CombineAsImage(color, imageRowHeight));
            else
                c.BackgroundImage = ImageFactory.CombineImagesVertical(rep.CombineAsImage(color, imageRowHeight));
        }

        public Image CreateErrorRepresentation(Color color, Size bBox) => ImageFactory.CreateImage("ERROR", color, bBox);

        public ImageTreeNode<T> CreateExpression(ImageTreeNode<T> left, SymbolRepresentation symbol, ImageTreeNode<T> right) => new(left, new(symbol), right);

        public bool IsParenthesized(ImageTreeNode<T> rep) => !rep.IsLeaf && rep.Left!.IsLeaf && rep.Left.Leaf!.IsEquivalent("(") && rep.Right!.IsLeaf && rep.Right.Leaf!.IsEquivalent(")");

        public ImageTreeNode<T> Parenthesize(ImageTreeNode<T> rep) => new(CreateLeftParenthesis(), rep, CreateRightParenthesis());

        public ImageTreeNode<T> SetEqualTo(ImageTreeNode<T> exp, ImageTreeNode<T> equalTo) => new(exp, CreateEqualsSign(), equalTo);

        public Control CreateDisplayRepresentationBase()
        {
            return new Panel()
            {
                BackgroundImageLayout = ImageLayout.None,
                BackColor = GamePage<T>.INFO_BACKGROUND,
                Dock = DockStyle.Fill,
                Visible = true
            };
        }
        public int GetMaximalScaling(T value, Size container) => _scaler(value, container);

        public delegate Image Representation(T value, Color color, Size boundingBox, int minScale = -1);
        public delegate int MaximizeScale(T value, Size container);
    }
}
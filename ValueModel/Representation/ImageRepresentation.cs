using Countdown.GameController;
using Countdown.ValueModel.Representation;

namespace Countdown.ValueImplementations.Representation
{
    //Represents either an operation f(a, b) or parenthesis (a)
    public class ImageTreeNode
    {
        public bool IsImage { get { return Leaf != null; } }
        public ImageTreeNode? Left { get; private set; }
        public ImageTreeNode? Middle { get; private set; }
        public ImageTreeNode? Right { get; private set; }
        public Image? Leaf { get; private set; }

        public ImageTreeNode(ImageTreeNode? left, ImageTreeNode? middle, ImageTreeNode? right)
        {
            if (left == null && middle == null && right == null)
                throw new ArgumentNullException("Image node contains no base elements.");

            SetBranch(left, middle, right);
        }

        public ImageTreeNode(Image leaf)
        {
            SetLeaf(leaf);
        }

        public void SetLeaf(Image leaf)
        {
            if(!IsImage)
            {
                Left = null;
                Middle = null;
                Right = null;
                Leaf = leaf;
            }
        }

        public void SetBranch(ImageTreeNode? left, ImageTreeNode? middle, ImageTreeNode? right)
        {
            if (left == null && middle == null && right == null)
                throw new ArgumentNullException("Image node contains no base elements.");

            Left = left;
            Middle = middle;
            Right = right;
            Leaf = null;
        }

        public Image CombineAsImage()
        {
            if (IsImage)
                return Leaf!;
            else
            {
                Image l = Left!.CombineAsImage();
                Image m = Middle!.CombineAsImage();
                Image r = Right!.CombineAsImage();

                return ImageFactory.CombineImagesHorizontal(l, m, r);
            }
        }
        public static implicit operator Image(ImageTreeNode node)
        {
            return node.CombineAsImage();
        }

        public static implicit operator ImageTreeNode(Image img)
        {
            return new(img);
        }
    }

    public class ImageRepresentation<T>
    {
        public static readonly Image LEFT_PARENTHESIS = ImageFactory.CreateImage("(", GamePage<int>.PLAIN_TEXT, ImageFactory.DEFAULT_SIZE.bBox);
        public static readonly Image RIGHT_PARENTHESIS = ImageFactory.CreateImage(")", GamePage<int>.PLAIN_TEXT, ImageFactory.DEFAULT_SIZE.bBox);
        public static readonly Image EQUALS_SIGN = ImageFactory.CreateImage("=", GamePage<int>.PLAIN_TEXT, ImageFactory.DEFAULT_SIZE.bBox);

        public static Color RenderColor { get; set; }
        public static int RenderSize { get; set; }

        private readonly Representation _asRep;
        private readonly MaximizeScale _scaler;

        public ImageRepresentation(Representation caster, MaximizeScale scaler)
        {
            RenderColor = GamePage<int>.PLAIN_TEXT;
            RenderSize = ImageFactory.DEFAULT_SIZE.scale;
            _asRep = caster;
            _scaler = scaler;
        }

        public Image AsRepresentation(T value, Size bBox, int minScale = -1) => _asRep(value, bBox, minScale);

        public void AppendRepresentation(Control c, ImageTreeNode rep)
        {
            if (c.BackgroundImage != null)
                c.BackgroundImage = ImageFactory.CombineImagesVertical(c.BackgroundImage, rep.CombineAsImage());
            else
                c.BackgroundImage = ImageFactory.CombineImagesVertical(rep.CombineAsImage());
        }

        public Image CreateErrorRepresentation(Size bBox) => ImageFactory.CreateImage("ERROR", RenderColor, bBox);

        public ImageTreeNode CreateExpression(ImageTreeNode left, SymbolRepresentation symbol, ImageTreeNode right) => new(left, symbol.Symbol, right);

        public bool IsParenthesized(ImageTreeNode rep) => LEFT_PARENTHESIS.Equals(rep.Left) && RIGHT_PARENTHESIS.Equals(rep.Right);

        public ImageTreeNode Parenthesize(ImageTreeNode rep) => new(LEFT_PARENTHESIS, rep, RIGHT_PARENTHESIS);

        public ImageTreeNode SetEqualTo(ImageTreeNode exp, ImageTreeNode equalTo) => new(exp, EQUALS_SIGN, equalTo);

        public Control CreateDisplayRepresentationBase()
        {
            return new Panel()
            {
                AutoScroll = true,
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Visible = true
            };
        }
        public int GetMaximalScaling(T value, Size container)
        {
            return _scaler(value, container);
        }

        public delegate Image Representation(T value, Size boundingBox, int minScale = -1);
        public delegate int MaximizeScale(T value, Size container);
    }
}
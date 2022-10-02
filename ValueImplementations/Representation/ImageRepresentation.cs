using Countdown.ValueImplementations.Values;

namespace Countdown.ValueImplementations.Representation
{
    public class ImageTreeNode
    {
        public bool IsImage { get { return Left is null; } }
        public ImageTreeNode? Left { get; private set; }
        public ImageTreeNode? Middle { get; private set; }
        public ImageTreeNode? Right { get; private set; }
        public Image? Leaf { get; private set; }

        public ImageTreeNode(ImageTreeNode left, ImageTreeNode middle, ImageTreeNode right) => SetBranch(left, middle, right);

        public ImageTreeNode(Image leaf) => SetLeaf(leaf);

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

        public void SetBranch(ImageTreeNode left, ImageTreeNode middle, ImageTreeNode right)
        {
            Left = left;
            Middle = middle;
            Right = right;
            Leaf = null;
        }
    }

    public class ImageRepresentation<T> : IRepresentation<T, ImageTreeNode>
    {
        public override ImageTreeNode AsRepresentation(T value)
        {
            throw new NotImplementedException();
        }

        public override void AppendRepresentation(Control c, ImageTreeNode rep)
        {
            throw new NotImplementedException();
        }

        public override ImageTreeNode CreateErrorRepresentation()
        {
            throw new NotImplementedException();
        }

        public override ImageTreeNode CreateExpression(ImageTreeNode left, SymbolRepresentation<ImageTreeNode> symbol, ImageTreeNode right)
        {
            throw new NotImplementedException();
        }

        public override bool IsParenthesized(ImageTreeNode rep)
        {
            throw new NotImplementedException();
        }

        public override ImageTreeNode Parenthesize(ImageTreeNode rep)
        {
            throw new NotImplementedException();
        }

        public override ImageTreeNode SetEqualTo(ImageTreeNode exp, ImageTreeNode equalTo)
        {
            throw new NotImplementedException();
        }

        protected override T FromRepresentation(ImageTreeNode representation)
        {
            throw new NotImplementedException();
        }

        public override void AppendLineBreak(Control c)
        {
            throw new NotImplementedException();
        }

        public override Control CreateDisplayRepresentationBase()
        {
            throw new NotImplementedException();
        }
    }
}

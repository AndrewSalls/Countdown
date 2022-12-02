using Countdown.ValueImplementations.Representation;

namespace Countdown.ValueModel.Representation
{
    //Represents either an operation f(a, b) or parenthesis (a)
    public class ImageTreeNode<T>
    {
        public bool IsLeaf { get { return Leaf != null; } }
        public ImageTreeNode<T>? Left { get; private set; }
        public ImageTreeNode<T>? Middle { get; private set; }
        public ImageTreeNode<T>? Right { get; private set; }
        public IImageTreeNodeLeaf? Leaf { get; private set; }

        public ImageTreeNode(ImageTreeNode<T>? left, ImageTreeNode<T>? middle, ImageTreeNode<T>? right)
        {
            if (left == null && middle == null && right == null)
                throw new ArgumentNullException("Image node contains no base elements.");

            SetBranch(left, middle, right);
        }

        public ImageTreeNode(T actualValue, ImageRepresentation<T> renderer) => SetLeaf(actualValue, renderer);
        public ImageTreeNode(SymbolRepresentation actualRep) => SetLeaf(actualRep);

        public void SetLeaf(T actualValue, ImageRepresentation<T> renderer)
        {
            if (!IsLeaf)
            {
                Left = null;
                Middle = null;
                Right = null;
                Leaf = new ValueLeaf<T>(actualValue, renderer);
            }
        }
        public void SetLeaf(SymbolRepresentation actualRep)
        {
            if (!IsLeaf)
            {
                Left = null;
                Middle = null;
                Right = null;
                Leaf = new SymbolLeaf(actualRep);
            }
        }

        public void SetBranch(ImageTreeNode<T>? left, ImageTreeNode<T>? middle, ImageTreeNode<T>? right)
        {
            if (left == null && middle == null && right == null)
                throw new ArgumentNullException("Image node contains no base elements.");

            Left = left;
            Middle = middle;
            Right = right;
            Leaf = null;
        }

        public Image CombineAsImage(Color color, int imageRowHeight)
        {
            if (IsLeaf)
                return Leaf!.Render(color, imageRowHeight);
            else
            {
                Image l = Left!.CombineAsImage(color, imageRowHeight);
                Image m = Middle!.CombineAsImage(color, imageRowHeight);
                Image r = Right!.CombineAsImage(color, imageRowHeight);

                return ImageFactory.CombineImagesHorizontal(l, m, r);
            }
        }
    }
}

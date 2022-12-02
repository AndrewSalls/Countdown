using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Xml.Linq;

namespace Countdown.ValueModel.Representation
{
    public class ImageFactory
    {
        public static readonly (int scale, Size bBox) DEFAULT_SIZE;

        static ImageFactory()
        {
            DEFAULT_SIZE = (48, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 48, FontStyle.Regular, GraphicsUnit.Pixel)));
        }
        public static Image CreateImage(string input, Color color, Size boundingBox, int fixedScale = -1)
        {
            if (boundingBox.Width == 0 || boundingBox.Height == 0)
                throw new ArgumentException("Bounding box provided is too small.");

            Bitmap bmp = new(boundingBox.Width, boundingBox.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Font sized;

            if (fixedScale == -1)
            {
                (int scale, Size _size) = MaximizeTextFont(input, boundingBox);
                sized = new(new FontFamily("Consolas"), scale, FontStyle.Regular, GraphicsUnit.Pixel);
            }
            else
                sized = new(new FontFamily("Consolas"), fixedScale, FontStyle.Regular, GraphicsUnit.Pixel);

            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(input, sized, new SolidBrush(color), new RectangleF(0, 0, boundingBox.Width, boundingBox.Height), format);
            g.Flush();
            return bmp;
        }
        public static Image CreateImage(string input, Color color, int imageRowHeight)
        {
            (int scale, Size bBox) = MaximizeVerticalFont(input, imageRowHeight);
            Bitmap bmp = new(bBox.Width, bBox.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Font sized = new(new FontFamily("Consolas"), scale, FontStyle.Regular, GraphicsUnit.Pixel);

            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(input, sized, new SolidBrush(color), new RectangleF(0, 0, bBox.Width, bBox.Height), format);
            g.Flush();
            return bmp;
        }

        public static (int scale, Size bBox) MaximizeTextFont(string input, Size container)
        {
            int scale = 0;
            Size fontSize = TextRenderer.MeasureText(input, new(new FontFamily("Consolas"), 1, FontStyle.Regular, GraphicsUnit.Pixel));
            Size lastSize;
            do
            {
                lastSize = fontSize;
                scale += 4;
                fontSize = TextRenderer.MeasureText(input, new(new FontFamily("Consolas"), scale, FontStyle.Regular, GraphicsUnit.Pixel));
            }
            while (fontSize.Width < container.Width && fontSize.Height < container.Height);

            //System.Diagnostics.Debug.WriteLine($"{container} => {Math.Max(scale - 4, 1)} for input {input} with size {lastSize} as compared to {fontSize}");
            return (Math.Max(scale - 4, 1), lastSize);
        }
        public static (int scale, Size bBox) MaximizeVerticalFont(string input, int maxHeight)
        {
            int scale = 0;
            Size fontSize = TextRenderer.MeasureText(input, new(new FontFamily("Consolas"), 1, FontStyle.Regular, GraphicsUnit.Pixel));
            Size lastSize;
            do
            {
                lastSize = fontSize;
                scale += 4;
                fontSize = TextRenderer.MeasureText(input, new(new FontFamily("Consolas"), scale, FontStyle.Regular, GraphicsUnit.Pixel));
            }
            while (fontSize.Height < maxHeight);

            //System.Diagnostics.Debug.WriteLine($"{container} => {Math.Max(scale - 4, 1)} for input {input} with size {lastSize} as compared to {fontSize}");
            return (Math.Max(scale - 4, 1), lastSize);
        }

        public static Image CombineImagesHorizontal(params Image[] imgs)
        {
            Bitmap bmp = new(imgs.Sum(i => i.Width), imgs[0].Height);
            using Graphics g = Graphics.FromImage(bmp);

            int width = 0;
            for(int i = 0; i < imgs.Length; i++)
            {
                g.DrawImage(imgs[i], width, 0);
                width += imgs[i].Width;
            }

            return bmp;
        }
        public static Image CombineImagesVertical(params Image[] imgs)
        {
            Bitmap bmp = new(imgs.Max(i => i.Width), imgs.Sum(i => i.Height));
            using Graphics g = Graphics.FromImage(bmp);

            int height = 0;
            for (int i = 0; i < imgs.Length; i++)
            {
                g.DrawImage(imgs[i], 0, height);
                height += imgs[i].Height;
            }

            return bmp;
        }
    }
}

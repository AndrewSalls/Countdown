using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Countdown.ValueModel.Representation
{
    public class ImageFactory
    {
        public static readonly (int scale, Size bBox)[] FONT_SIZES;
        public static readonly (int scale, Size bBox) DEFAULT_SIZE;

        static ImageFactory()
        {
            using Graphics g = Graphics.FromImage(new Bitmap(1, 1));
            FONT_SIZES = new (int, Size)[]{
                (128, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 128, FontStyle.Regular, GraphicsUnit.Pixel))),
                (96, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 96, FontStyle.Regular, GraphicsUnit.Pixel))),
                (80, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 80, FontStyle.Regular, GraphicsUnit.Pixel))),
                (64, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 64, FontStyle.Regular, GraphicsUnit.Pixel))),
                (48, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 48, FontStyle.Regular, GraphicsUnit.Pixel))),
                (36, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 36, FontStyle.Regular, GraphicsUnit.Pixel))),
                (24, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 24, FontStyle.Regular, GraphicsUnit.Pixel))),
                (16, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 16, FontStyle.Regular, GraphicsUnit.Pixel))),
                (14, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 14, FontStyle.Regular, GraphicsUnit.Pixel))),
                (12, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 12, FontStyle.Regular, GraphicsUnit.Pixel))),
                (8, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 8, FontStyle.Regular, GraphicsUnit.Pixel))),
                (4, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 4, FontStyle.Regular, GraphicsUnit.Pixel))),
                (2, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 2, FontStyle.Regular, GraphicsUnit.Pixel))),
                (1, TextRenderer.MeasureText("e", new(new FontFamily("Consolas"), 1, FontStyle.Regular, GraphicsUnit.Pixel)))
            };
            DEFAULT_SIZE = FONT_SIZES[1];
        }
        public static Image CreateImage(string input, Color color, Size boundingBox)
        {
            //System.Diagnostics.Debug.WriteLine(FONT_SIZES.Select(f => $"{f.scale} -> {f.bBox.Width} {f.bBox.Height}").Aggregate((a, b) => a + "\n" + b));
            Bitmap bmp = new(boundingBox.Width, boundingBox.Height);
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            (int scale, Size bBox) = FONT_SIZES.Where(s => input.Length * s.bBox.Width < boundingBox.Width && s.bBox.Height < boundingBox.Height).FirstOrDefault(FONT_SIZES[^1]);

            Font sized = new(new FontFamily("Consolas"), scale, FontStyle.Regular, GraphicsUnit.Pixel);
            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(input, sized, new SolidBrush(color), new RectangleF(0, 0, boundingBox.Width, boundingBox.Height), format);
            g.Flush();
            return bmp;
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

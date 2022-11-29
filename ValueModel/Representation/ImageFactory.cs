using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Countdown.ValueModel.Representation
{
    public class ImageFactory
    {
        public const int DEFAULT_CHARACTER_SIZE = 64;
        public static readonly Font STRING_FONT = new(FontFamily.GenericMonospace, DEFAULT_CHARACTER_SIZE, FontStyle.Regular, GraphicsUnit.Pixel);

        public static Image CreateImage(string input, Color color, int fontSize)
        {     
            Bitmap bmp = new(fontSize, fontSize);
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Font sized = new(STRING_FONT.FontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            RectangleF bounds = new(new PointF(0, 0), g.MeasureString(input, sized));
            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(input, sized, new SolidBrush(color), bounds, format);
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

        public static void SetImage(Panel ctrl, Graphics g, Image img)
        {
            Size cs = ctrl.Size;
            if (img.Size != cs)
            {
                //Gets size of control relative to size of image
                float ratio = Math.Min(cs.Height / (float)img.Height, cs.Width / (float)img.Width);
                //If image is larger than control, scales image to fit in control
                if (ratio < 1)
                {
                    int calc(float f) => (int)(Math.Ceiling(f / ratio));
                    img = new Bitmap(img, calc(img.Width), calc(img.Height));
                }

            }

            g.DrawImageUnscaled(img, (cs.Width - img.Width) / 2, (cs.Height - img.Height) / 2);
        }
    }
}

using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Countdown.ValueModel.Representation
{
    public class ImageFactory
    {
        public const int CHARACTER_SIZE = 128;
        public static readonly Font STRING_FONT = new(FontFamily.GenericMonospace, CHARACTER_SIZE, FontStyle.Regular, GraphicsUnit.Pixel);

        public static Image CreateImage(string input, Color color, int size)
        {
            Bitmap bmp = new(size * input.Length, size);
            Graphics g = Graphics.FromImage(bmp);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            RectangleF bounds = new(new PointF(0, 0), g.MeasureString(input, STRING_FONT));
            StringFormat format = new()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(input, STRING_FONT, new SolidBrush(color), bounds, format);
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

        public static void SetImage(Control ctrl, Image img)
        {
            Size cs = ctrl.Size;
            if (img.Size != cs)
            {
                float ratio = Math.Max(cs.Height / (float)img.Height, cs.Width / (float)img.Width);
                if (ratio > 1)
                {
                    int calc(float f) => (int)Math.Ceiling(f * ratio);
                    img = new Bitmap(img, calc(img.Width), calc(img.Height));
                }

                Bitmap part = new Bitmap(cs.Width, cs.Height);
                using (Graphics g = Graphics.FromImage(part))
                {
                    g.DrawImageUnscaled(img, (cs.Width - img.Width) / 2, (cs.Height - img.Height) / 2);
                }
                img = part;
            }
            ctrl.BackgroundImageLayout = ImageLayout.Center;
            ctrl.BackgroundImage = img;
        }
    }
}

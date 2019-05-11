using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace TPR_ExampleView
{
    public static partial class Extensions
    {
        public static readonly List<string> ExtSupport = new List<string>()
        {
            ".BMP",
            ".DIB",
            ".JPEG",
            ".JPG",
            ".JPE",
            ".PNG",
            ".PBM",
            ".PGM",
            ".PPM",
            ".SR",
            ".RAS",
            ".TIFF",
            ".TIF"
        };
        public static bool PathIsImage(this string s) => ExtSupport.Contains(Path.GetExtension(s).ToUpper());
        public static string DescriptionAttr<T>(this T source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0) return attributes[0].Description;
            else return source.ToString();
        }
        public static void DrawRotatedText(this Graphics g, int x, int y, float angle, string text, Font font, Brush brush)
        {
            g.TranslateTransform(x, y); // Set rotation point
            g.RotateTransform(angle); // Rotate text
            g.TranslateTransform(-x, -y); // Reset translate transform
            SizeF size = g.MeasureString(text, font); // Get size of rotated text (bounding box)
            g.DrawString(text, font, brush, new PointF(x + size.Width / 2.0f, y - size.Height / 2.0f)); // Draw string centered in x, y (x + size.Width / 2.0f, y - size.Height ))
            g.ResetTransform(); // Only needed if you reuse the Graphics object for multiple calls to DrawString
        }
    }
}

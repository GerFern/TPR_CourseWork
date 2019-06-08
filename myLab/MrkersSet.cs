using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.UI;
using System.Runtime.InteropServices;
using Emgu.CV.Structure;

namespace myLab
{
    public partial class MarkerSet : Form
    {
        Bitmap bm;
        Random testR = new Random();
        Rectangle rectImage;
        public bool auto = false;
        IImage clone, ime;
        public Mat res;
        Point point = Point.Empty;
        Point t = Point.Empty;
        Stack<(Point, byte)> points = new Stack<(Point, byte)>();
        public MarkerSet(IImage im)
        {
            InitializeComponent();
            bm = im.Bitmap;
            clone = (IImage)im.Clone();
            imageBox1.Image = (IImage)im.Clone();
            // Image = (IImage)image.Clone();
            rectImage = new Rectangle(Point.Empty, clone.Size);
        }

        public static void SetValue(Mat mat, int row, int col, int value)
        {
            var target = new[] { value };
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
            //mat.Data.SetValue(value, row, col);
        }

        public Color retColor;
        public Point retPoint;
        private void Button1_Click(object sender, EventArgs e)
        {
            res = new Mat(bm.Height, bm.Width, Emgu.CV.CvEnum.DepthType.Cv32S, 1);

            // Mat temp = new Mat(res.Size, res.Depth, res.NumberOfChannels);
            // temp.SetTo(new MCvScalar(1)); 
            //  CvInvoke.Add(res, res, temp);

            Point point;
            byte b;
            while (points.Count > 0)
            {
                (point, b) = points.Pop();
                SetValue(res, point.X, point.Y, (byte)numericUpDown1.Value);
            }
            Close();
        }

        private void ImageBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (rectImage.Contains(retPoint))
            {
                button2.Enabled = true;
                points.Push((retPoint, (byte)numericUpDown1.Value));
                imageBox1.Image.Bitmap.SetPixel(retPoint.X, retPoint.Y, Color.Blue);
                if (retPoint.X > 5)
                    for (int i = 0; i < 5; i++)
                        imageBox1.Image.Bitmap.SetPixel(retPoint.X - i, retPoint.Y, Color.Blue);

                if (retPoint.X < imageBox1.Image.Bitmap.Width - 5)
                    for (int i = 0; i < 5; i++)
                        imageBox1.Image.Bitmap.SetPixel(retPoint.X + i, retPoint.Y, Color.Blue);

                if (retPoint.Y < imageBox1.Image.Bitmap.Height - 5)
                    for (int i = 0; i < 5; i++)

                        imageBox1.Image.Bitmap.SetPixel(retPoint.X, retPoint.Y + i, Color.Blue);

                if (retPoint.Y > 5)
                    for (int i = 0; i < 5; i++)
                        imageBox1.Image.Bitmap.SetPixel(retPoint.X, retPoint.Y - i, Color.Blue);
            }
            imageBox1.Refresh();
        }

        private void ImageBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int offsetX = (int)(e.Location.X / imageBox1.ZoomScale);
            int offsetY = (int)(e.Location.Y / imageBox1.ZoomScale);
            int horizontalScrollBarValue = imageBox1.HorizontalScrollBar.Visible ? (int)imageBox1.HorizontalScrollBar.Value : 0;
            int verticalScrollBarValue = imageBox1.VerticalScrollBar.Visible ? (int)imageBox1.VerticalScrollBar.Value : 0;
            Point temp = new Point(offsetX + horizontalScrollBarValue, offsetY + verticalScrollBarValue);
            if (rectImage.Contains(temp))
            {
                retPoint = temp;
                if (isDawn)
                {
                    ImageBox1_MouseClick(sender, e);
                }
            }

        }
        bool isDawn = false;

        private void ImageBox1_MouseDown(object sender, MouseEventArgs e)
        {
            isDawn = true;
        }

        private void ImageBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDawn = false;
        }

        private void ImageBox1_MouseLeave(object sender, EventArgs e)
        {
            isDawn = false;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Point t;
            Color cl;
            (t, _) = points.Pop();

            cl = clone.Bitmap.GetPixel(t.X, t.Y);
            imageBox1.Image.Bitmap.SetPixel(t.X, t.Y, cl);


            if (t.X > 5)
                for (int i = 0; i < 5; i++)
                    imageBox1.Image.Bitmap.SetPixel(t.X - i, t.Y, clone.Bitmap.GetPixel(t.X - i, t.Y));

            if (t.X < imageBox1.Image.Bitmap.Width - 5)
                for (int i = 0; i < 5; i++)
                    imageBox1.Image.Bitmap.SetPixel(t.X + i, t.Y, clone.Bitmap.GetPixel(t.X + i, t.Y));

            if (t.Y < imageBox1.Image.Bitmap.Height - 5)
                for (int i = 0; i < 5; i++)

                    imageBox1.Image.Bitmap.SetPixel(t.X, t.Y + i, clone.Bitmap.GetPixel(t.X, t.Y + i));

            if (t.Y > 5)
                for (int i = 0; i < 5; i++)
                    imageBox1.Image.Bitmap.SetPixel(t.X, t.Y - i, clone.Bitmap.GetPixel(t.X, t.Y - i));


            if (points.Count == 0)
            {
                button2.Enabled = false;
            }
        }
    }
}

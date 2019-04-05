using BaseLibrary;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestLibrary
{
    public partial class SelectCoord : ImageForm
    {
        Point point = Point.Empty;
        Point t = Point.Empty;
        Point Point
        {
            get => point;
            set
            {
                point = value;
                label1.Text = point.ToString();
            }
        }
        protected override void SetImage(IImage image)
        {
            imageBox1.Image = image;
        }
        public SelectCoord(IImage image)
        {
            InitializeComponent();
            Image = image;
        }

        private void ImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            int offsetX = (int)(e.Location.X / imageBox1.ZoomScale);
            int offsetY = (int)(e.Location.Y / imageBox1.ZoomScale);
            int horizontalScrollBarValue = imageBox1.HorizontalScrollBar.Visible ? (int)imageBox1.HorizontalScrollBar.Value : 0;
            int verticalScrollBarValue = imageBox1.VerticalScrollBar.Visible ? (int)imageBox1.VerticalScrollBar.Value : 0;
            t = new Point(offsetX + horizontalScrollBarValue, offsetY + verticalScrollBarValue);
            label2.Text = t.ToString();
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {
            Point = t;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OutputImage outputImage = Class1.FloodFill((Image<Bgr, byte>)Image, Point);
            BaseMethods.LoadOutputImage(outputImage);
            //CastToOutputImage(outputImage);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

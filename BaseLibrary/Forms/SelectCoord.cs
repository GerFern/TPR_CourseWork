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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseLibrary
{
    public partial class SelectCoord : Form
    {
        Rectangle rectImage;
        Point point = Point.Empty;
        Point t = Point.Empty;
        /// <summary>
        /// Выбранная координата
        /// </summary>
        public Point SelectedPoint
        {
            get => point;
            set
            {
                point = value;
                label1.Text = point.ToString();
            }
        }
        public SelectCoord(IImage image, bool cancelButton = true)
        {
            InitializeComponent();
            label1.Text = label2.Text = Point.Empty.ToString();
            imageBox1.Image = image;
            rectImage = new Rectangle(Point.Empty, image.Size);
            if(ControlBox = button2.Visible = cancelButton)
            {
                CancelButton = button2;
            }
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
            if (rectImage.Contains(t))
            {
                SelectedPoint = t;
            }
        }
    }
}

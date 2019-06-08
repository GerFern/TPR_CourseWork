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

namespace myLab
{
    public partial class GetPont : Form
    {
        Bitmap bm;
        Random testR = new Random();
        Rectangle rectImage;
        public bool auto = false;
        IImage backup; 
        public GetPont(IImage im)
        {
            InitializeComponent();
            bm = im.Bitmap;
            imageBox1.Image = im;
            backup = (IImage)im.Clone();
            // Image = (IImage)image.Clone();
            rectImage = new Rectangle(Point.Empty, backup.Size);

            pictureBox2.Image = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            pictureBox3.Image = new Bitmap(pictureBox3.Width, pictureBox3.Height);
        }


        public Color retColor,curColor;
        public Point retPoint,curPoint;
        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ImageBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (rectImage.Contains(curPoint))
            {
                retPoint = new Point(curPoint.X, curPoint.Y);
                retColor =pictureBox3.BackColor;
                pictureBox2.BackColor = bm.GetPixel(retPoint.X, retPoint.Y);
                label1.Text = "X:" + curPoint.X;
                label3.Text = "Y:" + curPoint.Y;
                label2.Text = "RGB:" + pictureBox3.BackColor.R + "." + pictureBox3.BackColor.G + "." + pictureBox3.BackColor.B;
            }
        }

        private void ImageBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ImageBox1_MouseClick(sender, e);
            Button1_Click(sender, e);
        }

        private void ImageBox1_MouseMove_1(object sender, MouseEventArgs e)
        {
            int offsetX = (int)(e.Location.X / imageBox1.ZoomScale);
            int offsetY = (int)(e.Location.Y / imageBox1.ZoomScale);
            int horizontalScrollBarValue = imageBox1.HorizontalScrollBar.Visible ? (int)imageBox1.HorizontalScrollBar.Value : 0;
            int verticalScrollBarValue = imageBox1.VerticalScrollBar.Visible ? (int)imageBox1.VerticalScrollBar.Value : 0;
            Point temp = new Point(offsetX + horizontalScrollBarValue, offsetY + verticalScrollBarValue);
            if (rectImage.Contains(temp))
            {
                curPoint = temp;
                pictureBox3.BackColor = bm.GetPixel(curPoint.X, curPoint.Y);
                label6.Text = "X:" + curPoint.X;
                label4.Text = "Y:" + curPoint.Y;
                label5.Text= "RGB:"+ pictureBox3.BackColor.R+"."+ pictureBox3.BackColor.G+"."+pictureBox3.BackColor.B;

            }
        }
    }
}

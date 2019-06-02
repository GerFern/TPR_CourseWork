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

namespace Temp
{
    public partial class GetPoint : Form
    {
        Bitmap bm;
        public GetPoint(Bitmap im)
        {
            InitializeComponent();
            bm = im;
            pictureBox1.Image = (Image)im;
            pictureBox2.Image = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            pictureBox3.Image = new Bitmap(pictureBox3.Width, pictureBox3.Height);
        }

        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
    
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            pictureBox3.BackColor = bm.GetPixel(e.X, e.Y);
            label6.Text = "X:" + e.X.ToString();
            label4.Text = "Y:" + e.Y.ToString();
            label5.Text = $"RGB:{pictureBox3.BackColor.R}.{pictureBox3.BackColor.G}.{pictureBox3.BackColor.B}";
        }

        private void Label3_Click(object sender, EventArgs e)
        {

        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
          
        }

        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            pictureBox2.BackColor = bm.GetPixel(e.X, e.Y);
            label1.Text = "X:" + e.X.ToString();
            label3.Text = "Y:" + e.Y.ToString();
            retColor = pictureBox2.BackColor;
            retPoint = new Point(e.X, e.Y);
            label2.Text = $"RGB:{pictureBox2.BackColor.R}.{pictureBox2.BackColor.G}.{pictureBox2.BackColor.B}";
        }
        public Color retColor;
        public Point retPoint;
        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

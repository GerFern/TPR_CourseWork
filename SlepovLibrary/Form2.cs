using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SlepovLibrary
{
    public partial class Form2 : BaseLibrary.ImageForm
    {
        bool auto;
        IImage backup;
        /// <summary>
        /// Установить изображение
        /// </summary>
        /// <param name="image"></param>
        protected override void SetImage(IImage image)
        {
            imageBox.Image = image;
        }
        public Form2(IImage input)
        {
            InitializeComponent();
            Image = input;
            backup = (IImage)input.Clone();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            if (auto)
            {
                Image?.Dispose();
                dynamic img = backup.Clone();
                CvInvoke.BilateralFilter(backup, img, (int)numericUpDown1.Value, (double)hScrollBar1.Value, (double)hScrollBar2.Value);
                Image = img;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (auto)
            {
                Image?.Dispose();
                dynamic img = backup.Clone();
                CvInvoke.BilateralFilter(backup, img, (int)numericUpDown1.Value, (double)hScrollBar1.Value, (double)hScrollBar2.Value);
                Image = img;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Image?.Dispose();
            dynamic img = backup.Clone();
            CvInvoke.BilateralFilter(backup, img, (int)numericUpDown1.Value, (double)hScrollBar1.Value, (double)hScrollBar2.Value);
            Image = img;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            auto = checkBox1.Checked;
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            backup?.Dispose();
        }
    }
}

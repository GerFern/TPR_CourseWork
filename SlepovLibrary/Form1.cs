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
    public partial class Form1 : BaseLibrary.ImageForm
    {
        IImage backup;
        /// <summary>
        /// Установить изображение
        /// </summary>
        /// <param name="image"></param>
        protected override void SetImage(IImage image)
        {
            imageBox.Image = image;
        }
        public Form1(IImage input)
        {
            InitializeComponent();
            backup = (IImage)input.Clone();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Image?.Dispose();
            Image<Emgu.CV.Structure.Bgr, byte> img = (Image<Emgu.CV.Structure.Bgr, byte>)backup.Clone();
            img._EqualizeHist();
            img._GammaCorrect(e.NewValue/10d);
            Image = img;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            backup?.Dispose();
        }
    }
}

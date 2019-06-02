using BaseLibrary;
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
    public partial class EqualizeHistForm : BaseForm
    {
        public IImage backup;
        public EqualizeHistForm(InputImage inputImage, System.Reflection.MethodInfo methodInfo) :base(inputImage, methodInfo)
        {
            InitializeComponent();
            backup = (IImage)inputImage.Image.Clone();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            dynamic img = backup.Clone();
            img._EqualizeHist();
            img._GammaCorrect(e.NewValue / 10d);
            OutputImage outputImage = new OutputImage { UpdateSelectedImage = img };
            BaseMethods.LoadOutputImage(outputImage);
        }
    }
}

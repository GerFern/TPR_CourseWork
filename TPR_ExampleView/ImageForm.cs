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
using TPR_ExampleView.Properties;

namespace TPR_ExampleView
{
    public partial class ImageForm : Form
    {
        string name;
        IImage backup;
        public Emgu.CV.UI.ImageBox ImageBox => imageBox;
        public string ImageName => name;
        public bool IsGeneral { get; private set; }
        
        public ImageForm()
        {
            InitializeComponent();
        }

        public ImageForm(IImage image, string name):this()
        {
            backup = (IImage)image.Clone();
            imageBox.Image = image;
            this.name = name;
            this.TopLevel = false;
        }

        public void MakeGeneral()
        {
            if (!IsGeneral)
            {
                try
                {
                    MenuMethod.SelectedForm.toolStripButton1.Image = Resources.пустой_чекбокс_32;
                }
                catch { }
                toolStripButton1.Image = Resources.отмеченный_чекбокс_32;
                MenuMethod.SelectedForm = this;
                MenuMethod.SelectedImage = this.imageBox.Image;
            }
        }
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            MakeGeneral();
        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            
        }

        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            toolStrip1.Hide();
        }

        private void ToolStripButton4_Click(object sender, EventArgs e)
        {
            if(this.Parent is TabForm tabForm)
            {
                tabForm.RestoreTabBeforeClosing = false;
                tabForm.Close();
            }
            else if(this.Parent is TabPage tabPage)
            {
                tabPage.Dispose();
            }
            if (this.IsGeneral)
            {
                MenuMethod.SelectedForm = null;
                MenuMethod.SelectedImage = null;
            }
            this.imageBox.Image.Dispose();
            backup.Dispose();
            this.Close();
        }

        private void ImageBox_Click(object sender, EventArgs e)
        {
            toolStrip1.Show();
        }

        private void ToolStripButton5_Click(object sender, EventArgs e)
        {
            imageBox.Image.Dispose();
            imageBox.Image = (IImage)backup.Clone();
        }
    }
}

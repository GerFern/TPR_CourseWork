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
using TPR_ExampleView.Properties;

namespace TPR_ExampleView
{
    public partial class ImageForm : BaseLibrary.ImageForm
    {
        IImage backup;
        public Emgu.CV.UI.ImageBox ImageBox => imageBox;
        public override bool AutoSelect => true;
        //public ImageForm()
        //{
        //    InitializeComponent();
        //}

        public ImageForm(IImage image, string text):base()
        {
            InitializeComponent();
            IsSelectedChanged += ImageForm_IsSelectedChanged;
            backup = (IImage)image.Clone();
            this.Image = image;
            this.Text = text;
        }
        public ImageForm(string path, bool register = true) : base(register)
        {
            InitializeComponent();
            //string name = System.IO.Path.GetFileName(path);
            FilePath = path;
            IsSelectedChanged += ImageForm_IsSelectedChanged;
            Worker.DoWork += new DoWorkEventHandler((Object obj, DoWorkEventArgs arg) =>
            {
                BackgroundWorkerImg worker = (BackgroundWorkerImg)obj;
                //(string path, string name) t = ((string, string))arg.Argument;
                IImage img = MenuMethod.CreateImage((string)arg.Argument);
                IImage image = img;
                backup = (IImage)image.Clone();
                this.Image = image;
                arg.Result = img;
            });
            this.Text = System.IO.Path.GetFileName(path);
            //this.NameForm = name;
        }

        private void ImageForm_IsSelectedChanged(object sender, EventArgsWithImageForm e)
        {
            this.toolStripButton1.Image = e.Selected ? Resources.отмеченный_чекбокс_32 : Resources.пустой_чекбокс_32;
        }


        protected override void SetImage(IImage image)
        {
            this.imageBox.Image = Image;
        }

        public override void UpdateImage(IImage image = null)
        {
            if (image != null) imageBox.Image = image;
            else imageBox.Update();
        }
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            MakeSelected();
        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = BaseMethods.GetSaveFileDialog();
            if(sfd.ShowDialog()==DialogResult.OK)
            {
                try
                {
                    Image.Save(sfd.FileName);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            toolStrip1.Hide();
        }

        private void ToolStripButton4_Click(object sender, EventArgs e)
        {
            //if(this.Parent is TabForm tabForm)
            //{
            //    tabForm.RestoreTabBeforeClosing = false;
            //    tabForm.Close();
            //}
            //else if(this.Parent is TabPage tabPage)
            //{
            //    tabPage.Dispose();
            //}
            //if (this.IsSelected)
            //{
            //    MenuMethod.SelectedForm = null;
            //    MenuMethod.SelectedImage = null;
            //}
            imageBox.Image?.Dispose();
            backup?.Dispose();
            this.Close();
        }

        private void ImageBox_Click(object sender, EventArgs e)
        {
            toolStrip1.Show();
        }

        private void ToolStripButton5_Click(object sender, EventArgs e)
        {
            imageBox.Image.Dispose();
            //SetImage((IImage)backup.Clone());
            Image = (IImage)backup.Clone();
            if (IsSelected)
            {
                IsSelected = false;
                IsSelected = true;
            }
        }


        private void ToolStripButton7_Click(object sender, EventArgs e)
        {
            double hDelta = (double)imageBox.Height / Image.Size.Height;
            double wDelta = (double)imageBox.Width / Image.Size.Width;
            if (hDelta < wDelta) imageBox.SetZoomScale(hDelta, Point.Empty);
            else imageBox.SetZoomScale(wDelta, Point.Empty);
        }

        private void ToolStripButton8_Click(object sender, EventArgs e)
        {
            imageBox.SetZoomScale(1, Point.Empty);
        }

        public Image<Bgr, Byte> Method(Image<Bgr, Byte> image) 
        {
            return image.Dilate(10);
        }
    }
}

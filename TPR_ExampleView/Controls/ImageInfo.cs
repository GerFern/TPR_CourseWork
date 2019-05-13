using BaseLibrary;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPR_ExampleView
{
    public partial class ImageInfo : UserControl
    {
        ImageList imageList;
        public ImageInfo()
        {
            InitializeComponent();
            lName.DoubleClick += ПереименоватьToolStripMenuItem_Click;
            ParentChanged += new EventHandler((o, e) => { if (Parent is ImageList imageList) this.imageList = imageList; });
        }
        public ImageInfo(int ID, BaseLibrary.ImageForm imageForm = null, string imgFilePath = null) : this()
        {
            IsFile = !string.IsNullOrWhiteSpace(imgFilePath);
            this.ID = ID;
            ImageForm = imageForm;
            ImgFilePath = imgFilePath;
            if (imageForm != null)
            {
                imageForm.FormClosed += new FormClosedEventHandler((o, e) => Image = null);
                imageForm.ImageChanged += ImageForm_ImageChanged;
                if (imageForm.Image == null)
                {
                    if (ImgFilePath != null && System.IO.File.Exists(ImgFilePath))
                        LoadBitmapFromStrPath();
                }
                else
                    Image = imageForm.Image;
                lName.Text = imageForm.Text;
            }
            else
            {
                LoadBitmapFromStrPath();
                lName.Text = System.IO.Path.GetFileName(ImgFilePath);
                Status = ImgStatus.UnloadedFile;
                Image = null;
            }

        }

        [DontCatchException]
        private void LoadBitmapFromStrPath()
        {
            try
            {
                {
                    using (Bitmap source = new Bitmap(ImgFilePath))
                        pictureBox1.Image = new Bitmap(source, new Size(64, 64));
                }
            }
            catch { }
        }

        public enum ImgStatus
        {
            [Description("Загружен(файл)")]
            LoadedFile,
            [Description("Выгружен(файл)")]
            UnloadedFile,
            [Description("Загружен")]
            Loaded,
            [Description("Выгружен")]
            Close
        }
        ImgStatus _imgStatus;
        public ImgStatus Status
        {
            get => _imgStatus;
            set
            {
                _imgStatus = value;
                if (lStat.InvokeRequired)
                    lStat.Invoke(new Action(() => lStat.Text = _imgStatus.DescriptionAttr()));
                else lStat.Text = _imgStatus.DescriptionAttr();
                if (_imgStatus == ImgStatus.Close) Close();
            }
        }

        public bool CanOpen => Status == ImgStatus.UnloadedFile && File.Exists(ImgFilePath);
        public bool FormExist => !(ImageForm == null || ImageForm.IsDisposed);
        
        public bool CanSelectImage => !(ImageForm == null || ImageForm.IsDisposed || ImageForm.IsSelected);
        public bool IsFile { get; }
        public Type TColor { get; private set; }
        public Type TDepth { get; private set; }
        public int ID { get; }
        public BaseLibrary.ImageForm ImageForm { get; private set; }
        public bool Checked
        {
            get => checkBox1.Checked;
            set => checkBox1.Checked = value;
        }
        public string ImgFilePath { get; set; }
        IImage _image;
        public IImage Image
        {
            get => _image;
            set
            {
                pictureBox1.SuspendLayout();
                if (value == null)
                {
                    //_image?.Dispose();
                    //pictureBox1.Image = null;
                    if (IsFile) Status = ImgStatus.UnloadedFile;
                    else Status = ImgStatus.Close;
                }
                else
                {
                    if (pictureBox1.Image != null) pictureBox1.Image.Dispose();
                    using (Bitmap source = value.Bitmap)
                        pictureBox1.Image = new Bitmap(source, new Size(48, 48));
                    if (IsFile) Status = ImgStatus.LoadedFile;
                    else Status = ImgStatus.Loaded;
                }
                pictureBox1.ResumeLayout();
                _image = value;

                if (lType.InvokeRequired)
                    lType.Invoke(new Action(() => LoadType()));
                else LoadType();
            }
        }

        public string ImgName => lName.Text;

        private void LoadType()
        {
            if (_image == null)
            {
                lType.Text = "null";
            }
            else
            {
                Type ImageType = _image.GetType();
                Type[] t = ImageType.GetGenericArguments();
                if (t.Length == 2)
                {
                    TColor = t[0];
                    TDepth = t[1];
                    lType.Text = $"{TColor.Name},{TDepth.Name}";
                }
                else
                {
                    TColor = TDepth = null;
                    lType.Text = ImageType.Name;
                }
            }
        }

        public bool LoadImage()
        {
            if (Image != null) return true;
            if (string.IsNullOrWhiteSpace(ImgFilePath)) return false;
            Image = new Image<Bgr, byte>(ImgFilePath);
            return true;
        }

        public void Close()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;

        private void T1ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

     

        private void ImageForm_ImageChanged(object sender, EventArgsImage e)
        {
            if (e.Image == null)
            {
                if (ImgFilePath != null && System.IO.File.Exists(ImgFilePath))
                {
                    using (Bitmap source = new Bitmap(ImgFilePath))
                        pictureBox1.Image = new Bitmap(source, new Size(64, 64));
                    Status = ImgStatus.UnloadedFile;
                }
                else Status = ImgStatus.Close;
            }
            else
            {
                Image = e.Image;
                if (IsFile) Status = ImgStatus.LoadedFile;
                else Status = ImgStatus.Loaded;
            }
        }

        private void ОткрытьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageForm == null || ImageForm.IsDisposed)
                if (ImgFilePath != null && File.Exists(ImgFilePath))
                {
                    ImageForm = new ImageForm(ImgFilePath, false);
                    ImageForm.Text = lName.Text;
                    Image = ImageForm.Image;
                    ImageForm.FormClosed += new FormClosedEventHandler((o, arg) => Image = null);
                    ImageForm.ImageChanged += ImageForm_ImageChanged;
                    ImageForm.ShowFormAsync(ImgFilePath);
                }
        }

        private void ВыбратьТекущимИзображениемToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(ImageForm != null && ImageForm.Image != null && ImageForm.Image.Ptr != IntPtr.Zero )
            {
                ImageForm.MakeSelected();
            }
        }

        private void УбратьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ЗакрытьФормуToolStripMenuItem_Click(object sender, EventArgs e)
        { 
            ImageForm?.Close();
        }

        private void ЗакрытьИУбратьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            ImageForm?.Close();
        }

        private void ПоказатьФормуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!(ImageForm == null || ImageForm.IsDisposed))
                ImageForm.Show();
        }

        private void ContextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            открытьФайлToolStripMenuItem.Enabled = CanOpen;
            закрытьФормуToolStripMenuItem.Enabled = FormExist;
            показатьФормуToolStripMenuItem.Enabled = FormExist;
            выбратьТекущимИзображениемToolStripMenuItem.Enabled = CanSelectImage;
            сохранитьИзображениеToolStripMenuItem.Enabled = !Image.IsDisposedOrNull();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = !checkBox1.Checked;
        }

        private void ПереименоватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form f = new Form { Text = "Название изображения", Size = new Size(250, 100), MinimizeBox = false, MaximizeBox = false })
            {
                TextBox tb = new TextBox { Dock = DockStyle.Fill, Parent = f, Text = lName.Text, Multiline = true, AcceptsReturn = true };
                f.ShowDialog();
                lName.Text = tb.Text;
                if (ImageForm != null) ImageForm.Text = tb.Text;
            }
        }

        private void СохранитьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!Image.IsDisposedOrNull())
                using (SaveFileDialog sfd = BaseMethods.GetSaveFileDialog())
                    if (sfd.ShowDialog() == DialogResult.OK)
                        Image.Save(sfd.FileName);
        }
    }
}

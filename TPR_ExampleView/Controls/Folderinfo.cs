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
    public partial class FolderInfo : UserControl
    {
        ImageList imageList;
        public FolderInfo()
        {
            InitializeComponent();
            ParentChanged += new EventHandler((o, e) => { if (Parent is ImageList imageList) this.imageList = imageList; });
        }
        public FolderInfo(int ID,  string folderDir) : this()
        {
            this.ID = ID;
            FolderDir = folderDir;
            ImgNames = GetImgNames();
            lPath.Text = folderDir;
            lCount.Text = ImgNames.Count.ToString();
        }

        public List<ImgName> ImgNames { get; private set; }

        List<ImgName> GetImgNames()
        {
            List<ImgName> list = new List<ImgName>();
            foreach (var item in Directory.GetFiles(FolderDir))
            {
                if (item.PathIsImage())
                    list.Add(new ImgName(item, null, Path.GetFileNameWithoutExtension(item)));
            } 
            return list;
        }

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
        public string FolderDir { get; set; }

        public void Close()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;

        private void T1ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

     


        private void УбратьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ContextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
        }

        private void ЗакрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Close();
        }

        private void ОбновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImgNames = GetImgNames();
            lCount.Text = ImgNames.Count.ToString();
        }
    }
}

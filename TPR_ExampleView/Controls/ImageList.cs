using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using System.IO;
using BaseLibrary;

namespace TPR_ExampleView
{
    public partial class ImageList : UserControl
    {
        public ImageList()
        {
            InitializeComponent();
            ImgItems = new ImageInfoCollection();
            FolderInfos = new FolderInfoCollection();
        }

        public void Add(FolderInfo folderInfo)
        {
            FolderInfos.Add(folderInfo);
            folderInfo.tableLayoutPanel1.Margin = new Padding(0, 1, 0, 1);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(folderInfo.tableLayoutPanel1);
            folderInfo.Closed += new EventHandler((o, e) =>
            {
                if (FolderInfos.Contains(folderInfo))
                {
                    FolderInfos.Remove(folderInfo);
                    tableLayoutPanel1.Controls.Remove(folderInfo.tableLayoutPanel1);
                    folderInfo.Dispose();
                }
            });
        }

        public void Add(ImageInfo imageInfo)
        {
            ImgItems.Add(imageInfo);
            imageInfo.tableLayoutPanel1.Margin = new Padding(0, 1, 0, 1);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(imageInfo.tableLayoutPanel1);
            imageInfo.Closed += new EventHandler((o, e) =>
            {
                if (ImgItems.Contains(imageInfo))
                {
                    ImgItems.Remove(imageInfo);
                    tableLayoutPanel1.Controls.Remove(imageInfo.tableLayoutPanel1);
                    imageInfo.Dispose();
                }
            });
        }

        public IEnumerable<IImage> CheckedImages => ImgItems.Where(a => a.Checked).Select(a => a.Image);

        public IEnumerable<ImgName> CheckedImgNames => ImgItems.Where(a => a.Checked).Select(a => new ImgName(a.ImgFilePath, a.Image, a.ImgName)).Concat(FolderInfos.CheckedImgNames); 

        public ImageInfoCollection ImgItems { get; }

        public FolderInfoCollection FolderInfos { get; }

        public class FolderInfoCollection : List<FolderInfo>
        {
            public List<ImgName> CheckedImgNames => this.Where(a => a.Checked).SelectMany(a => a.ImgNames).ToList();
        }

        public class ImageInfoCollection : List<ImageInfo>
        {
            public List<IImage> CheckedImages => this.Where(a => a.Checked).Select(a => a.Image).ToList();
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            bool b = (ImgItems.Count==0 || ImgItems.Where(a => a.Checked).Count() != ImgItems.Count) 
                && (FolderInfos.Count == 0 || FolderInfos.Where(a => a.Checked).Count() != FolderInfos.Count);

            foreach (var item in ImgItems)
                item.Checked = b;

            foreach (var item in FolderInfos)
                item.Checked = b;
            //if (CheckedImages.Count()==Items.Count)
            //{
            //    foreach (var item in Items)
            //        item.Checked = false;
            //}
            //else
            //    foreach (var item in Items)
            //        item.Checked = true;
        }

        private void ToolStripButton2_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            while(ImgItems.Count>0)
            {
                ImgItems.First().Close();
            }
            while (FolderInfos.Count > 0)
            {
                FolderInfos.First().Close();
            }
            //Items.ForEach(new Action<ImageInfo>(a => a.Close()));
            ResumeLayout();
            tableLayoutPanel1.ResumeLayout();
        }

        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            var forms = Application.OpenForms.OfType<BaseLibrary.ImageForm>();
            foreach (var item in forms)
            {
                if(!ImgItems.Select(a=>a.ImageForm).Contains(item))
                {
                    Add(new ImageInfo(1, item, item.FilePath));
                }
            }
            ResumeLayout();
            tableLayoutPanel1.ResumeLayout();
        }

        private void ToolStripButton4_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            //using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true })
            using (OpenFileDialog ofd = BaseMethods.GetOpenFileDialog(true))
                if (ofd.ShowDialog() == DialogResult.OK)
                    foreach (var item in ofd.FileNames)
                        Add(new ImageInfo(1, null, item));
            ResumeLayout();
            tableLayoutPanel1.ResumeLayout();
        }

        private void ToolStripButton5_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                if (fbd.ShowDialog() == DialogResult.OK)
                    if (Directory.Exists(fbd.SelectedPath))
                        Add(new FolderInfo(1, fbd.SelectedPath));
                        //foreach (var item in Directory.GetFiles(fbd.SelectedPath).Where(a => a.PathIsImage()))
                        //    Add(new ImageInfo(1, null, item));
            ResumeLayout();
            tableLayoutPanel1.ResumeLayout();
        }

        private void ToolStripButton6_Click(object sender, EventArgs e)
        {
            new Forms.FormSettings().ShowDialog();
        }
    }
}

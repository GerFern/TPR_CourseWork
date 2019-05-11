﻿using System;
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

namespace TPR_ExampleView
{
    public partial class ImageList : UserControl
    {
        public ImageList()
        {
            InitializeComponent();
            Items = new ImageInfoCollection();
        }

        public void Add(ImageInfo imageInfo)
        {
            Items.Add(imageInfo);
            imageInfo.tableLayoutPanel1.Margin = new Padding(0, 1, 0, 1);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(imageInfo.tableLayoutPanel1);
            imageInfo.Closed += new EventHandler((o, e) =>
            {
                if (Items.Contains(imageInfo))
                {
                    Items.Remove(imageInfo);
                    tableLayoutPanel1.Controls.Remove(imageInfo.tableLayoutPanel1);
                    imageInfo.Dispose();
                }
            });
        }

        public IEnumerable<IImage> CheckedImages => Items.Where(a => a.Checked).Select(a => a.Image);

        public ImageInfoCollection Items { get; }

        public class ImageInfoCollection : List<ImageInfo>
        {
            public List<IImage> CheckedImages => this.Where(a => a.Checked).Select(a => a.Image).ToList();
            public void Add(ImageInfo imageInfo)
            {
                base.Add(imageInfo);
            }
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            bool b = Items.Where(a => a.Checked).Count() != Items.Count;
            foreach (var item in Items)
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
            while(Items.Count>0)
            {
                Items.First().Close();
            }
            //Items.ForEach(new Action<ImageInfo>(a => a.Close()));
            ResumeLayout();
        }

        private void ToolStripButton3_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            var forms = Application.OpenForms.OfType<BaseLibrary.ImageForm>();
            foreach (var item in forms)
            {
                if(!Items.Select(a=>a.ImageForm).Contains(item))
                {
                    Add(new ImageInfo(1, item, item.FilePath));
                }
            }
            ResumeLayout();
        }

        private void ToolStripButton4_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true })
                if (ofd.ShowDialog() == DialogResult.OK)
                    foreach (var item in ofd.FileNames)
                        Add(new ImageInfo(1, null, item));
            ResumeLayout();
        }

        private void ToolStripButton5_Click(object sender, EventArgs e)
        {
            SuspendLayout();
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                if (fbd.ShowDialog() == DialogResult.OK)
                    if (Directory.Exists(fbd.SelectedPath))
                        foreach (var item in Directory.GetFiles(fbd.SelectedPath).Where(a => a.PathIsImage()))
                            Add(new ImageInfo(1, null, item));
            ResumeLayout();
        }
    }
}

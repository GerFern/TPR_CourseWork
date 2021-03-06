﻿using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageCollection
{
    public partial class UserControl1 : UserControl
    {
        public bool showAll;
        public bool ShowAll
        {
            get => showAll;
            set
            {
                showAll = value;
                LoadPreview(pathRoot, showAll, showDir);
            }
        }

        public bool showDir;
        public bool ShowDir
        {
            get => showDir;
            set
            {
                showDir = value;
                LoadPreview(pathRoot, showAll, showDir);
            }
        }

        public enum ViewMode
        {
            Horizontal,
            Vertical,
            Tree
        }

        private string pathRoot;
        private string path;
        public string PathRoot
        {
            get => pathRoot;
            set
            {
                SetPath(value);
            }
        }

        private ViewMode view;
        public ViewMode View
        {
            get => view;
            set
            {
                if (view != value)
                {
                    switch (view = value)
                    {
                        case ViewMode.Horizontal:
                            //flowLayoutPanel1.WrapContents = false;
                            flowLayoutPanel1.Visible = true;
                            treeView1.Visible = false;
                            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
                            break;
                        case ViewMode.Vertical:
                            //flowLayoutPanel1.WrapContents = true;
                            flowLayoutPanel1.Visible = true;
                            treeView1.Visible = false;
                            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
                            break;
                        case ViewMode.Tree:
                            flowLayoutPanel1.Visible = false;
                            treeView1.Visible = true;
                            break;
                    }

                }
            }
        }

        private int count;
        private List<TableLayoutPanel> previews = new List<TableLayoutPanel>();
        private Dictionary<string, Bitmap> bitmaps = new Dictionary<string, Bitmap>();
        public int Count => count;
        public UserControl1()
        {
            InitializeComponent();
            treeView1.Dock = flowLayoutPanel1.Dock = DockStyle.Fill;
            treeView1.Hide();
            View = ViewMode.Horizontal;
        }

        public void SetPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            this.path = this.pathRoot = path;
            PathChanged?.Invoke(this, new EventArgsWithFilePath(path, LoadPreview(path, showAll, showDir)));
            fileSystemWatcher1.Path = PathRoot;
        }

        private string LoadPreview(string path, bool showAll, bool showDir)
        {
            this.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            foreach (var item in previews)
            {
                item.SuspendLayout();
                //item.Image.Dispose();
                item.Dispose();
            }
            flowLayoutPanel1.Controls.Clear();
            previews.Clear();
            bitmaps.Clear();

            ShellContainer shellContainer = (ShellContainer)ShellObject.FromParsingName(path);
            if (/*path != pathRoot &&*/ shellContainer.Parent != null)
            {
                previews.Add(CreatePictureBoxWithLabel("..", shellContainer.Parent.ParsingName, new Bitmap(64, 64), true));
            }
            try
            {
                if (showDir)
                {
                    foreach (var item in shellContainer.OfType<ShellFolder>())
                    {
                        //ShellObject shellObject = ShellObject.FromParsingName(item);
                        Bitmap b;
                        ShellThumbnail st = item.Thumbnail;
                        b = st.LargeBitmap;
                        //try
                        //{
                        //    st.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
                        //    b = st.MediumBitmap;
                        //}
                        //catch
                        //{
                        //    st.FormatOption = ShellThumbnailFormatOption.IconOnly;
                        //    b = st.MediumBitmap;
                        //    b.MakeTransparent();
                        //}
                        bitmaps.Add(item.ParsingName, b);
                        TableLayoutPanel panel = CreatePictureBoxWithLabel(item.Name, item.ParsingName, b, true);

                        previews.Add(panel);
                    }
                }

                //var l = Directory.GetFiles(path);
                IEnumerable<ShellFile> l = shellContainer.OfType<ShellFile>();
                if (!showAll)
                {
                    l = l.Where(a => a.IsImage());
                }
                foreach (var item in l)
                {

                    string name = System.IO.Path.GetFileName(item.Path);
                    //ShellObject shellObject = ShellObject.FromParsingName(item);
                    //shellFolder.
                    //ShellFile shellFile = ShellFile.FromFilePath(item);
                    Bitmap b;
                    ShellThumbnail st = item.Thumbnail;
                    b = st.LargeBitmap;
                    //try
                    //{
                    //    st.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
                    //    b = st.MediumBitmap;
                    //}
                    //catch
                    //{
                    //    st.FormatOption = ShellThumbnailFormatOption.IconOnly;
                    //    b = st.MediumBitmap;
                    //    b.MakeTransparent();
                    //}
                    //b.MakeTransparent();
                    bitmaps.Add(item.ParsingName, b);
                    TableLayoutPanel panel = CreatePictureBoxWithLabel(name, item.ParsingName, b, false);
                    previews.Add(panel);
                    //previews.Add(new PictureBox { Size = new Size(64,64), Image = shellFile.Thumbnail.MediumIcon.ToBitmap(), SizeMode = PictureBoxSizeMode.Zoom });

                }
            
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                //previews.Clear();
                //if (shellContainer.Parent != null)
                //{
                //    previews.Add(CreatePictureBoxWithLabel("..", shellContainer.Parent.ParsingName, new Bitmap(64, 64), true));
                //}
                //else previews.
            }
            foreach (var item in previews)
            {
                flowLayoutPanel1.Controls.Add(item);
            }
            this.ResumeLayout();
            flowLayoutPanel1.ResumeLayout();
            return shellContainer.Name;
        }

        private void Panel_File_Click(string path)
        {
            FileSelect?.Invoke(this, new EventArgsWithFilePath(path));
        }

        private void Panel_Folder_Click(string path)
        {
            PathChanged?.Invoke(this, new EventArgsWithFilePath(path, LoadPreview(path, showAll, showDir)));
            try
            {
                fileSystemWatcher1.Changed -= FileSystemWatcher1_Changed;
                fileSystemWatcher1.Path = path;
                fileSystemWatcher1.Changed += FileSystemWatcher1_Changed;
            }
            catch{}
        }

        private TableLayoutPanel CreatePictureBoxWithLabel(string name, string path, Bitmap image, bool IsDir)
        {
            Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogButton c = new Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogButton();
            TableLayoutPanel panel = new TableLayoutPanel { Size = new Size(90, 90), ColumnCount = 1, RowCount = 2 };
            //panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            PictureBox pictureBox = new PictureBox {/* Image = image, */SizeMode = PictureBoxSizeMode.Zoom };
            pictureBox.Image = image;
            
            Label label = new Label { Text = name, TextAlign = ContentAlignment.MiddleCenter };
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            //panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            panel.Controls.Add(pictureBox, 0, 0);
            panel.Controls.Add(label, 0, 1);
            pictureBox.Dock = label.Dock = DockStyle.Fill;


            if (IsDir)
            {
                panel.Tag = new PanelClick(path, Panel_Folder_Click);
            }
            else
            {
                panel.Tag = new PanelClick(path, Panel_File_Click);
            }

            panel.DoubleClick += Panel_DoubleClick;
            pictureBox.DoubleClick += SubPanel_Click;
            label.DoubleClick += SubPanel_Click;

            return panel;
        }

        private void Panel_DoubleClick(object sender, EventArgs e)
        {
            ((PanelClick)((Control)sender).Tag).Click();
        }

        private void SubPanel_Click(object sender, EventArgs e)
        {
            ((PanelClick)((TableLayoutPanel)((Control)sender).Parent).Tag).Click();
            //var c = (TableLayoutPanel)((Control)sender).Parent;
            ////c.DoubleClick?.Invoke(c, e);
            ////c.Invoke(c.GetType().GetEvent("DoubleClick").);
            ////c.GetType().GetMethods();
            //var t = c.GetType().GetEvent("DoubleClick");
            //t.
            //c.DoubleClick.Invoke(c, e);
            //c.DoubleClick(c, e);
        }

        private void FileSystemWatcher1_Renamed(object sender, RenamedEventArgs e)
        {
            //LoadPreview(path);
            //SetPath(path);
        }

        private void FileSystemWatcher1_Changed(object sender, FileSystemEventArgs e)
        {
            LoadPreview(path, showAll, showDir);
        }

        public event EventHandler<EventArgsWithFilePath> PathChanged;

        public event EventHandler<EventArgsWithFilePath> FileSelect;
        public class EventArgsWithFilePath : EventArgs
        {
            public EventArgsWithFilePath(string path, string pathName = null)
            {
                Path = path ?? throw new ArgumentNullException(nameof(path));
                PathName = pathName;
            }

            //public EventArgsWithFilePath(string path) => Path = path ?? throw new ArgumentNullException(nameof(path));

            public string Path { get; }
            public string PathName { get; }
            //public EventArgsWithFilePath(string path) : base() => Path = path;
        }
        public class PanelClick
        {
            public delegate void ClickHander(string path);

            private ClickHander clickHander;
            private string path;
            public PanelClick(string path, ClickHander clickHander)
            {
                this.clickHander = clickHander;
                this.path = path;
            }
            public void Click()
            {
                clickHander(path);
            }
        }
    }

    public static class ShellExtension
    {
        public static bool IsImage(this ShellFile shellFile)
        {
            string ext = System.IO.Path.GetExtension(shellFile.Path).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp";
        }
    }


}

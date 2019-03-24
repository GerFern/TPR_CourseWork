using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExplorerImage;
using static ExplorerImage.UserControl1;
//using static ImageCollection.UserControl1;

namespace TPR_ExampleView
{
    public partial class Form1 : Form
    {
        UserControl1 userControl1 = new UserControl1();
        IImage source = null;
        string imagePath = null;
        public Form1()
        {
            InitializeComponent();
            elementHost1.Child = userControl1;
            userControl1.PathChanged += UserControl1_PathChanged;
            userControl1.FileSelect += UserControl1_FileSelect;
            userControl1.View = ViewMode.Horizontal;
            userControl1.showDir = userControl1.showAll = true;
            //userControl1.SetPath("D:\\");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            MenuMethod.imageBox = imageBox1;
            MenuMethod.textBox = textBox1;
            DLL_Init.AssemblyInSolution = "TestLibrary";
            DLL_Init.Init(menuStrip1);
        }

        private void открытьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    source.Dispose();
                    imageBox1.Image.Dispose();
                    imageBox1.Image = new Image<Bgr, byte>(ofd.FileName);
                    source = imageBox1.Image.Clone() as IImage;
                    восстановитьToolStripMenuItem.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void сохранитьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imageBox1.Image.Save(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void восстановитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                imageBox1.Image.Dispose();
                imageBox1.Image = source.Clone() as IImage;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void открытьПапкуСБиблиотекамиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(DLL_Init.path);
        }

        private void ВыбратьПапкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if(FBD.ShowDialog()==DialogResult.OK)
            {
                userControl1.SetPath(FBD.SelectedPath);
            }
        }

        void UserControl1_PathChanged(object sender, EventArgsWithFilePath e)
        {
            if (e.Path.Split(new char[] { '\\', '/' }).Last() == e.PathName) groupBox4.Text = $"Галерея - {e.Path}";
            else groupBox4.Text = $"Галерея - {e.Path} ({e.PathName})";
        }

        private void UserControl1_FileSelect(object sender, EventArgsWithFilePath e)
        {
            try
            { 
                if(source!=null) source.Dispose();
                if(imageBox1.Image!=null) imageBox1.Image.Dispose();
                imageBox1.Image = new Image<Bgr, byte>(e.Path);
                source = imageBox1.Image.Clone() as IImage;
                восстановитьToolStripMenuItem.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void СкрытьГалереюToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = ((ToolStripMenuItem)sender).Checked;
        }

        private void TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser explorerBrowser = new Microsoft.WindowsAPICodePack.Controls.WindowsForms.ExplorerBrowser();
            f.Controls.Add(explorerBrowser);

            explorerBrowser.Dock = DockStyle.Fill;
            explorerBrowser.Navigate((ShellObject)KnownFolders.Desktop);
            f.Show();
        }
    }
}

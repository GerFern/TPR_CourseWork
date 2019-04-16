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
using System.Runtime.InteropServices;
using Emgu.CV.UI;
using BaseLibrary;
using System.Threading;
using System.IO;
//using static ImageCollection.UserControl1;

namespace TPR_ExampleView
{
    public partial class Form1 : Form
    {
        IImage SelectedImage = MenuMethod.SelectedImage;
        TabDragger tabDragger;
        //UserControl1 userControl1 = new UserControl1();
        IImage source = null;
        string imagePath = null;

        public Form1()
        {
            InitializeComponent();
            MenuMethod.MainForm = this;
            tabDragger = new TabDragger(tabControl1, TabDragBehavior.TabDragOut);
            userControl1.FileSelect += UserControl1_FileSelect;
            userControl1.PathChanged += UserControl1_PathChanged;
            //userControl1.showAll = userControl1.showDir = true;
            отображатьФайлыToolStripMenuItem.Checked = userControl1.showAll = Properties.Settings.Default.ShowAll;
            отображатьПапкиToolStripMenuItem.Checked = userControl1.showDir = Properties.Settings.Default.ShowDir;
            скрытьГалереюToolStripMenuItem.Checked = splitContainer3.Panel1Collapsed = Properties.Settings.Default.HideExplorer;

            отображатьПапкиToolStripMenuItem.CheckedChanged += ОтображатьПапкиToolStripMenuItem_CheckedChanged;
            отображатьФайлыToolStripMenuItem.CheckedChanged += ОтображатьФайлыToolStripMenuItem_CheckedChanged;
            скрытьГалереюToolStripMenuItem.CheckedChanged += СкрытьГалереюToolStripMenuItem_CheckedChanged;

            try { userControl1.SetPath(Properties.Settings.Default.ExplorerPath); }
            catch { userControl1.SetPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)); }
            BaseLibrary.ImageForm.SetIsSelectedChangedMethod(new EventHandler<EventArgsWithImageForm>(MenuMethod.ChangeSelected));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MenuMethod.textBox = textBox1;
            //Инициализация для открытия форм
            BaseMethods.Init(tabControl1,
            //
            new OutputImageInvoker((OutputImage img) =>
            {
                this.Invoke(new MethodInvoker(() =>
                    {
                        if (img != null)
                        {
                            if (img.Image != null)
                                this.OpenImage(img.Image, img.Name);
                            MenuMethod.CreateImage(img.Image);
                            if (img.Info != null)
                                textBox1.Text = img.Info;
                            if (img.ImageForm != null)
                                img.ImageForm.ShowForm();
                            if (img.UpdateSelectedImage != null)
                                MenuMethod.SelectedForm.UpdateImage();
                        }
                    }));
                return null;
            }),
            new OutputImageInvoker((OutputImage img) =>
            {
                ImageForm imageForm = new ImageForm(img.Image, img.Name);
                if (img.Info != null) textBox1.Text = img.Info;
                return imageForm;
            }), WriteToOutput);
            //BaseMethods.On_Writing += WriteToOutput;
            DLL_Init.AssemblyInSolution = "TestLibrary";
            DLL_Init.Init(menuStrip1); 
        }

        void WriteToOutput(string s)
        {
            textBox1.Invoke(new Action(() => { textBox1.Text += s; }));
        }

        private void открытьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    OpenImage(ofd.FileName);
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
                    SelectedImage.Save(sfd.FileName);
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
                //imageBox1.Image.Dispose();
                //imageBox1.Image = source.Clone() as IImage;
            }
            catch (Exception ex)
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
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                userControl1.SetPath(FBD.SelectedPath);
            }
        }

        void UserControl1_PathChanged(object sender, EventArgsWithFilePath e)
        {
            if (e.Path.Split(new char[] { '\\', '/' }).Last() == e.PathName) groupBox4.Text = $"Галерея - {e.Path}";
            else groupBox4.Text = $"Галерея - {e.Path} ({e.PathName})";
            Properties.Settings.Default.ExplorerPath = e.Path;
        }

        private void UserControl1_FileSelect(object sender, EventArgsWithFilePath e)
        {
            //try
            //{
            OpenImage(e.Path);
            //}
            //catch (Exception ex)
            //{
            //    if (MessageBox.Show($"{ex.Message}{Environment.NewLine}Открыть файл программой по умолчанию?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //    {
            //        try
            //        {
            //            System.Diagnostics.Process.Start(e.Path);
            //        }
            //        catch (Exception ex2)
            //        { MessageBox.Show(ex2.Message); }
            //    }
            //}
        }

        private void OpenImage(string path)
        {
            ImageForm imageForm = new ImageForm(path);
            imageForm.ShowFormAsync(path);
        }

        public void OpenImage(IImage image, string text)
        {
            ImageForm imageForm = new ImageForm(image, text);
            imageForm.ShowForm();
        }

        public void OpenForm(BaseLibrary.ImageForm imageForm, string name, object arg)
        {
            TabPage tabPage = new TabPage(name);
            BackgroundWorker backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += new DoWorkEventHandler((Object obj, DoWorkEventArgs args) =>
            {

            }
            );
            backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((Object obj, RunWorkerCompletedEventArgs args) =>
            {

            }
            );
        }

        private void ImageBox_Click(object sender, EventArgs e)
        {
            MenuMethod.SelectedImage = ((ImageBox)sender).Image;
        }

        private void СкрытьГалереюToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.HideExplorer = splitContainer3.Panel1Collapsed = ((ToolStripMenuItem)sender).Checked;
        }

        private void ОтображатьПапкиToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowDir = userControl1.ShowDir = ((ToolStripMenuItem)sender).Checked;
        }

        private void ОтображатьФайлыToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShowAll = userControl1.ShowAll = ((ToolStripMenuItem)sender).Checked;
        }

        private void ДобавитьФормуДляИзображенийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabDragger.CreatePanelForm();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        public class ToolStripVerticalSeparator:ToolStripItem
        {
            //public override string Text { get => ""; set { } }
            protected override void OnPaint(PaintEventArgs e)
            {
                if (this.Owner != null)
                {
                    //ToolStripRenderer renderer 
                    Graphics g = e.Graphics;
                    g.DrawLine(new Pen(ForeColor), new Point(this.Size.Width / 2, 0), new Point(this.Size.Width / 2, this.Size.Height));
                }
            }
        }
    }
}

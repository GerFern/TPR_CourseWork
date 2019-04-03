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

            userControl1.SetPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            BaseLibrary.ImageForm.SetIsSelectedChangedMethod(new EventHandler<EventArgsWithImageForm>(MenuMethod.ChangeSelected));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //MenuMethod.ImageViewer

            //MenuMethod.imageBox = imageBox1;
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
            string name = System.IO.Path.GetFileName(path);
            TabPage tabPage = new TabPage(name)
            {
                UseWaitCursor = true
            };
            //IImage img = MenuMethod.CreateImage(path);
            //ImageForm imageForm = new ImageForm(img, name)
            //{
            //    //TopLevel = false,
            //    //Dock = DockStyle.Fill
            //};
            //imageForm.MakeSelected();
            //imageForm.Show();
            ImageForm imageForm = new ImageForm(path);
            //backgroundWorker.DoWork += new DoWorkEventHandler((Object obj, DoWorkEventArgs target) =>
            //{
            //    BackgroundWorkerImg worker = (BackgroundWorkerImg)obj;
            //    (string path, string name) t = ((string, string))target.Argument;
            //    IImage img = MenuMethod.CreateImage(t.path);
            //    worker.ImageForm = new ImageForm(img, t.name)
            //    {
            //        Dock = DockStyle.Fill
            //    };
            //});
            imageForm.Worker.TabPage = tabPage;
            imageForm.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((Object obj, RunWorkerCompletedEventArgs target) =>
            {
                BackgroundWorkerImg worker = (BackgroundWorkerImg)obj;
                worker.ImageForm.TopLevel = false;
                worker.ImageForm.Parent = worker.TabPage;
                worker.TabPage.UseWaitCursor = false;
                worker.ImageForm.Dock = DockStyle.Fill;
                //form.Dock = DockStyle.Fill;
                worker.ImageForm.Visible = true;
                worker.ImageForm.MakeSelected();
                //}));
                //tabPage.Controls.Add(imageForm);
            });
            tabControl1.TabPages.Add(tabPage);
            imageForm.Worker.RunWorkerAsync(path);
            //Thread thread = new Thread(new ParameterizedThreadStart((obj) => 
            //{
            //    (IImage, string, TabPage) t = ((IImage, string, TabPage))obj;
            //    ImageForm form = new ImageForm(t.Item1, t.Item2);
            //    //form.Dock = DockStyle.Fill;
            //    var r= t.Item3.BeginInvoke(new MethodInvoker (()=>
            //    {
            //        //t.Item3.Controls.Add(form);
            //        form.Parent = t.Item3;
            //        //form.Dock = DockStyle.Fill;
            //        form.Visible = true;
            //    }));
            //    t.Item3.EndInvoke(r);
            //    form.Dock = DockStyle.Fill;
            //    //MessageBox.Show("a2");
            //    //Application.Run();

            //    //while(true)
            //    //{

            //    //}
            //    //t.Item3.Controls.Add(form);
            //    //form.Load += new EventHandler((Object sender, EventArgs e) =>
            //    //{
            //    //    MessageBox.Show("AAAA");
            //    //    ((Form)sender).Visible = true;
            //    //});
            //    //Application.Run(form);
            //    //MessageBox.Show("Close");
            //    //form.Visible = true;

            //});
            //tabPage.Controls.Add(imageForm);
            //tabControl1.TabPages.Add(tabPage);
            //thread.ApartmentState = ApartmentState.MTA;
            //thread.SetApartmentState(ApartmentState.MTA);
            //thread.Start((img, name, tabPage));
        }

        public void OpenImage(IImage image, string name)
        {
            string n = name == null ? "" : name;
            TabPage tabPage = new TabPage(n);
            ImageForm imageForm = new ImageForm(image, n)
            {
                TopLevel = false,
                Dock = DockStyle.Fill
            };
            tabPage.Controls.Add(imageForm);
            tabControl1.TabPages.Add(tabPage);
            //imageForm.MakeGeneral();
            imageForm.Show();
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

       
    }
}

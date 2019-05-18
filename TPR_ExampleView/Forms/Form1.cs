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
        public bool AutoLoadImageForm { get; private set; }
        //UserControl1 userControl1 = new UserControl1();
        public Dictionary<int, InputImage.InitProgress> ProgressDict { get; } = new Dictionary<int, InputImage.InitProgress>();
        public bool InvokeMethodImmediately { get; set; }
        IImage source = null;
        string imagePath = null;
        int id_gen = 0;
        int oldWidthTabs;
        int tabFirstIndex;
        int tabSecondIndex;
        bool hideTabs;
        bool noHide;
        bool HideTabs
        {
            get => hideTabs;
            set
            {
                if (value)
                {
                    if (!hideTabs)
                    {
                        tabControl2.SelectedIndex = -1;
                        oldWidthTabs = splitContainer4.SplitterDistance;
                        splitContainer4.Panel1MinSize = 0;
                        splitContainer4.SplitterDistance = tabControl2.GetTabRect(0).Width + 2;
                        splitContainer4.IsSplitterFixed = true;
                        hideTabs = true;
                    }
                }
                else if (hideTabs)
                {
                    noHide = true;
                    tabFirstIndex = tabControl2.SelectedIndex;
                    splitContainer4.SplitterDistance = oldWidthTabs;
                    splitContainer4.Panel1MinSize = 100;
                    splitContainer4.IsSplitterFixed = false;
                    hideTabs = false;
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            tabPage3.Parent = null;
            MenuMethod.MainForm = this;
            tabDragger = new TabDragger(tabControl1, TabDragBehavior.TabDragOut);
            userControl1.FileSelect += UserControl1_FileSelect;
            userControl1.PathChanged += UserControl1_PathChanged;
            //userControl1.showAll = userControl1.showDir = true;
            отображатьФайлыToolStripMenuItem.Checked = userControl1.showAll = Properties.Settings.Default.ShowAll;
            отображатьПапкиToolStripMenuItem.Checked = userControl1.showDir = Properties.Settings.Default.ShowDir;
            скрытьГалереюToolStripMenuItem.Checked = splitContainer3.Panel1Collapsed = Properties.Settings.Default.HideExplorer;

            отображатьФайлыToolStripMenuItem.CheckedChanged += ОтображатьФайлыToolStripMenuItem_CheckedChanged;
            отображатьПапкиToolStripMenuItem.CheckedChanged += ОтображатьПапкиToolStripMenuItem_CheckedChanged;
            скрытьГалереюToolStripMenuItem.CheckedChanged += СкрытьГалереюToolStripMenuItem_CheckedChanged;

            InvokeMethodImmediately = true;
            выполнятьМетодыСразуToolStripMenuItem.Checked = true;
           
            BaseLibrary.ImageForm.SetIsSelectedChangedMethod(new EventHandler<EventArgsWithImageForm>(MenuMethod.ChangeSelected));
            BaseLibrary.BaseMethods.NewImageForm += new EventHandler<EventArgsNewImageForm>((_, args) =>
                imageList1.InvokeFix(() => { if(AutoLoadImageForm) imageList1.Add(new ImageInfo(1, args.ImageForm, args.ImageForm.FilePath)); }));
            HideTabs = true;
            tabControl2.SelectedIndex = -1;
      
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try { userControl1.SetPath(Properties.Settings.Default.ExplorerPath); }
            catch { userControl1.SetPath(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)); }
            MenuMethod.textBox = textBox1;

            BaseMethods.Init(
                tabControl1,
                LoadOutputImage,
                SelectedImageForm,
                WriteToOutput,
                new GetProgressBar((InputImage img) => ProgressDict.ContainsKey(img.ID) ? ProgressDict[img.ID] : null)
            );
            //BaseMethods.On_Writing += WriteToOutput;
            //DLL_Init.AssemblyInSolution = "myLab";
            DLL_Init.Init(menuStrip1);
            BaseMethods.loadSetting();
        }

        internal BaseLibrary.ImageForm SelectedImageForm() => MenuMethod.SelectedForm;
        internal ImageForm LoadOutputImage(OutputImage img)
        {
            this.InvokeFix(() =>
            {
                if (img != null)
                {
                    if (img.Image != null)
                        this.OpenImage(img.Image, img.Name);
                    //MenuMethod.CreateImage(img.Image);
                    if (img.Info != null)
                        textBox1.Text = img.Info;
                    if (img.ImageForm != null)
                        img.ImageForm.ShowForm();
                    if (img.UpdateSelectedImage != null)
                        MenuMethod.SelectedForm.UpdateImage();
                    if (img.GlobalForm != null)
                    {
                        var GlobalForm = img.GlobalForm;
                        if (Program.GlobalForm == null)
                        {
                            GlobalForm.ShowForm();
                        }
                        else
                        {
                            if (Program.GlobalForm.IsDisposed)
                            {
                                GlobalForm.ShowForm();
                            }
                            else Program.GlobalForm.CastToOtherForm(GlobalForm);
                        }
                        Program.GlobalForm = GlobalForm;
                    }
                    else if (img.GlobalImage != null)
                    {
                        if (Program.GlobalForm == null || Program.GlobalForm.IsDisposed)
                            (Program.GlobalForm = new ImageForm(img.GlobalImage, img.Name)).ShowForm();
                        else
                            Program.GlobalForm.Image = img.GlobalImage;
                    }
                }
            });
            return null;
        }

        internal void LoadOutputImage(OutputImage img, MenuMethod.InvParam invParam)
        {
            this.InvokeFix(() =>
            {
                if (img != null)
                {
                    if (img.Image != null)
                    {
                        if(invParam.ImageSetting.SaveToFile)
                        {
                            if (invParam.ImageSetting.ReplaceFile)
                            {
                                img.Image.Save(invParam.GetFilePath(out _));
                            }
                            else
                            {
                                int counter = 1;
                                int uIndex;
                                string file = invParam.GetFilePath(out uIndex);
                                if (File.Exists(file))
                                {
                                    string fileu;
                                    do
                                    {
                                        fileu = file.Insert(uIndex, counter.ToString());
                                        counter++;
                                    } while (File.Exists(fileu));
                                    img.Image.Save(fileu);
                                }
                                else img.Image.Save(file);
                            }
                            img.Image.Dispose();
                        }
                        else
                        this.OpenImage(img.Image, img.Name);
                    }
                    //MenuMethod.CreateImage(img.Image);
                    if (img.Info != null)
                        textBox1.Text = img.Info;
                    if (img.ImageForm != null)
                        img.ImageForm.ShowForm();
                    if (img.UpdateSelectedImage != null)
                        MenuMethod.SelectedForm.UpdateImage();
                    if (img.GlobalForm != null)
                    {
                        var GlobalForm = img.GlobalForm;
                        if (Program.GlobalForm == null)
                        {
                            GlobalForm.ShowForm();
                        }
                        else
                        {
                            if (Program.GlobalForm.IsDisposed)
                            {
                                GlobalForm.ShowForm();
                            }
                            else Program.GlobalForm.CastToOtherForm(GlobalForm);
                        }
                        Program.GlobalForm = GlobalForm;
                    }
                    else if(img.GlobalImage != null)
                    {
                        if (Program.GlobalForm == null || Program.GlobalForm.IsDisposed)
                            (Program.GlobalForm = new ImageForm(img.GlobalImage, img.Name)).ShowForm();
                        else
                            Program.GlobalForm.Image = img.GlobalImage;
                    }
                }
            });
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
            try
            {
                OpenImage(e.Path);
            }
            catch { }
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
            //imageForm.Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((object o, RunWorkerCompletedEventArgs e) =>
            //    imageList1.Add(new ImageInfo(1, imageForm, path)));
        }

        public IEnumerable<IImage> SelectedImages => imageList1.CheckedImages;


        public void SetExceptionError(Exception ex)
        {
            this.InvokeFix(() => exceptionList1.SetLastExceptionError(ex));
        }
        public void AddException(Exception exception, bool error) =>
            this.InvokeFix(() =>
            {
                if (!tabControl2.TabPages.Contains(tabPage3))
                {
                    tabControl2.TabPages.Add(tabPage3);
                    if (HideTabs) tabControl2.SelectedIndex = -1;
                }
                exceptionList1.Add(new ExceptionInfoControl(exception, error));
            });

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
            splitContainer3.Panel1Collapsed = Properties.Settings.Default.HideExplorer = ((ToolStripMenuItem)sender).Checked;
        }

        private void ОтображатьПапкиToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            userControl1.ShowDir = Properties.Settings.Default.ShowDir = ((ToolStripMenuItem)sender).Checked;
        }

        private void ОтображатьФайлыToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            userControl1.ShowAll = Properties.Settings.Default.ShowAll = ((ToolStripMenuItem)sender).Checked;
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
            bool qwerty = false;
            public override Rectangle Bounds => DesignMode ? new Rectangle(base.Bounds.Location, new Size(300, base.Bounds.Height)) : base.Bounds;
            protected override void OnPaint(PaintEventArgs e)
            {
                Graphics g = e.Graphics;
                if (this.DesignMode)
                {
                    //e.Graphics.Clip = new Region();
                    Random r = new Random();
                    if (this.qwerty)
                    {
                        qwerty = false;
                        this.Invalidate();
                        if (r.Next() % 100 == 0)
                        {
                            Thread.Sleep(4321);
                            //new Form1().Show();
                            for (int i = 0; i < 20; i++)
                            {
                                ControlPaint.FillReversibleRectangle(new Rectangle((r.Next() % 1500)-100, (r.Next() % 1000)-100, r.Next() % 400, r.Next() % 400), Color.FromArgb(r.Next()));
                            }
                        }
                    }
                    else qwerty = true;
                    using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Point(0, 0), new Point(0, this.Height),
                        Color.FromArgb(r.Next() % 255, r.Next() % 255, r.Next() % 255),
                        Color.FromArgb(r.Next() % 255, r.Next() % 255, r.Next() % 255)))
                        g.FillRectangle(brush, new Rectangle(Point.Empty, this.Size));
                }
                else
                {
                    using (Pen pen = new Pen(ForeColor))
                        g.DrawLine(pen, new Point(this.Size.Width / 2, 0), new Point(this.Size.Width / 2, this.Size.Height));
                }
            }
        }

        private void СкрыватьБоковоеМенюToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            if(((ToolStripMenuItem)sender).Checked)
            {
                HideTabs = true;
                tabControl2.Leave += TabControl2_LostFocus;
            }
            else
            {
                HideTabs = false;
                tabControl2.Leave -= TabControl2_LostFocus;
            }
        }

        private void TabControl2_LostFocus(object sender, EventArgs e)
        {
             HideTabs = true;
        }

        private void TabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabFirstIndex = tabSecondIndex;
            tabSecondIndex = tabControl2.SelectedIndex;
            if (tabControl2.SelectedIndex >= 0 && hideTabs)
            {
                HideTabs = false;
            }
        }

        private void TabControl2_Click(object sender, EventArgs e)
        {
            //if (e is MouseEventArgs meargs && meargs.Button == MouseButtons.Left && tabControl2.SelectedIndex >= 0 && tabControl2.GetTabRect(tabControl2.SelectedIndex).Contains(meargs.Location))
            //{
            //    HideTabs = true;
            //    tabControl2.Click -= TabControl2_Click;
            //    //tabControl2.Click += TabControl2_ClickAlt;
            //}
        }


        private void TabControl2_MouseDown(object sender, MouseEventArgs e)
        {
            if (HideTabs) tabFirstIndex = -1;
            //tabFirstIndex = tabControl2.SelectedIndex;
        }
        private void TabControl2_MouseUp(object sender, MouseEventArgs e)
        {
            if (noHide) noHide = false;
            else if (tabFirstIndex >= 0)
                for (int i = 0; i < tabControl2.TabCount; i++)
                {
                    if (tabControl2.GetTabRect(i).Contains(e.Location))
                    {
                        if (tabFirstIndex == i) HideTabs = true;
                        else tabFirstIndex = tabSecondIndex = i;
                        return;
                    }
                }
        }

        internal void CreateTask(string name, MyMethodInfo myMethodInfo, Thread thread, MenuMethod.InvParam invParam, out ProgressInfoControl progressInfoControl)
        {
            ProgressInfoControl pic = null;
            this.InvokeFix(()=>
            {
                //tableLayoutPanel1.SuspendLayout();
                //ProgressBar progressBar = new ProgressBar();
                invParam.TaskID = id_gen;
                pic = new ProgressInfoControl(name, invParam, thread);
                pic.tableLayoutPanel1.Dock = DockStyle.Fill;
                //pic.tableLayoutPanel1.AutoSize = true;
                //TableLayoutPanel tlp = new TableLayoutPanel();
                //tlp.RowCount = 2;
                //tlp.ColumnCount = 1;
                //tlp.Controls.Add(new Label { Text = DateTime.Now.ToString(), Dock = DockStyle.Fill }, 0, 0);
                //tlp.Controls.Add(progressBar, 0, 1);
                //if (id_gen != 0)
                //    tableLayoutPanel1.RowCount++;
                //else
                //    tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
                //for (int i = tableLayoutPanel1.RowCount - 2; i >= 0; i--)
                //{
                //    tableLayoutPanel1.SetRow(tableLayoutPanel1.GetControlFromPosition(0, i), i + 1);
                //}
                //tableLayoutPanel1.Controls.Add(tlp, 0, 0);
                //tableLayoutPanel1.ResumeLayout();
            });
            progressInfoControl = pic;
            id_gen++;
        }

        private void ОтладкаИсключенийToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            Program.debugException = ((ToolStripMenuItem)sender).Checked;
        }

        private void AddToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Forms.FormInvokeProgress().ShowDialog();
        }

        private void ExceptionList1_AllClear(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 2) HideTabs = true;
            tabControl2.TabPages.RemoveAt(2);
            tabPage3.Text = $"Исключения";
        }

        private void ExceptionList1_AddException(object sender, EventArgs e)
        {
            tabPage3.Text = $"Исключения ({exceptionList1.ExceptionCount})";
        }


        private void ВыполнятьМетодыСразуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.InvokeMethodImmediately = ((ToolStripMenuItem)sender).Checked;
        }

        private void РедактироватьDllloadconfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string Path = "dll_load.conf";
            using (Form form = new Form() { Text = Path })
            {
                string str = null;
                if (!File.Exists(Path))
                {
                    str = "TestLibrary";
                    File.WriteAllText(Path, str);
                }
                else str = File.ReadAllText(Path);
                TableLayoutPanel tlp = new TableLayoutPanel() { Dock = DockStyle.Fill, Parent = form };
                TextBox tb = new TextBox { Multiline = true, Dock = DockStyle.Fill, Text = str };
                FlowLayoutPanel flp = new FlowLayoutPanel();
                Button bOK = new Button { Text = "OK", DialogResult = DialogResult.OK };
                Button bCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel };
                //form.AcceptButton = bOK;
                form.CancelButton = bCancel;
                flp.Controls.Add(bOK);
                flp.Controls.Add(bCancel);
                tlp.Controls.Add(tb, 0, 0);
                tlp.Controls.Add(flp, 0, 1);
                tlp.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
                if (form.ShowDialog() == DialogResult.OK)
                {
                    str = tb.Text;
                    File.WriteAllText(Path, str);
                }
            }
        }

        private void АвтоматическиДобавлятьНовыеИзображенияToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            AutoLoadImageForm = ((ToolStripMenuItem)sender).Checked;
        }
    }
}

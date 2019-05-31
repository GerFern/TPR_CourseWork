using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Shell;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using BaseLibrary;
using System.Threading;

namespace ExplorerImage
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public EventWaitHandle waiter;
        public Thread threadFileLoader;
        public bool IsLoadNewNow = false;

        public bool showAll;
        public bool ShowAll
        {
            get => showAll;
            set
            {
                showAll = value;
                LoadPreview(path, showAll, showDir);
            }
        }

        public bool showDir;
        public bool ShowDir
        {
            get => showDir;
            set
            {
                showDir = value;
                LoadPreview(path, showAll, showDir);
            }
        }

        public enum ViewMode
        {
            Horizontal,
            Vertical
        }

        private string path;
        
        private ViewMode view;
        public ViewMode View
        {
            get => view;
            set
            {
                //if (view != value)
                //{
                switch (view = value)
                {
                    case ViewMode.Horizontal:
                        //flowLayoutPanel1.WrapContents = false;
                        panel.FlowDirection = FlowDirection.LeftToRight;
                        panel.Orientation = Orientation.Horizontal;

                        panel.ScrollOwner.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        panel.ScrollOwner.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

                        break;
                    case ViewMode.Vertical:
                        //flowLayoutPanel1.WrapContents = true;
                        panel.FlowDirection = FlowDirection.LeftToRight;
                        panel.Orientation = Orientation.Vertical;
                        panel.ScrollOwner.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                        panel.ScrollOwner.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                        break;
                }

                //}
            }
        }

        private int count;
        private Dictionary<string, BitmapSource> bitmaps = new Dictionary<string, BitmapSource>();
        public int Count => count;
        public UserControl1()
        {
            InitializeComponent();
            panel.ScrollOwner = scrool;
            View = ViewMode.Horizontal;
        }

        public void SetPath(string path)
        {
            //if (!Directory.Exists(path))
            //{
            //    return;
            //}

            string name = LoadPreview(path, showAll, showDir);
            this.path  = path;
            PathChanged?.Invoke(this, new EventArgsWithFilePath(path, name));
        }

        private string LoadPreview(string path, bool showAll, bool showDir, bool saveLoad = true)
        {
            IsLoadNewNow = false;
            if (threadFileLoader != null && threadFileLoader.IsAlive)
            {
                threadFileLoader.Abort();
                //threadFileLoader.Join();
            }
            ShellContainer shellContainer = (ShellContainer)ShellObject.FromParsingName(path);
            //this.SuspendLayout();
            //flowLayoutPanel1.SuspendLayout();

            //flowLayoutPanel1.Controls.Clear();
            bitmaps.Clear();
            panel.Children.Clear();


            if (ShowDir && shellContainer.Parent != null)
            {
                panel.Children.Add(CreatePictureBoxWithLabel("..", shellContainer.Parent.ParsingName, true));
            }
            try
            {
                IEnumerable<ShellObject> list = new List<ShellObject>(); //= shellContainer.AsEnumerable().Where(a=>a is ShellFile || a is ShellFolder);
                if (ShowDir) list = list.Concat(shellContainer.OfType<ShellFolder>());
                if (showAll) list = list.Concat(shellContainer.OfType<ShellFile>());
                else list = list.Concat(shellContainer.OfType<ShellFile>().Where(a => a.IsImage()));

                //if (showDir) foreach (var item in shellContainer.OfType<ShellFolder>()) Add(item);
                //if (showAll) foreach (var item in shellContainer.OfType<ShellFile>()) Add(item);
                //else foreach (var item in shellContainer.OfType<ShellFile>().Where(a=>a.IsImage())) Add(item);

                if(list.Count()>100&&saveLoad)
                {
                    Grid grid = new Grid();
                    grid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = new GridLength(64)
                    });
                    Label label = new Label();
                    label.Content = $"Двойной клик, чтобы загрузить {list.Count()} элементов";
                    label.MouseDoubleClick += new MouseButtonEventHandler((o, e) =>
                    {
                        if (e.ChangedButton == MouseButton.Left) LoadPreview(path, showAll, showDir, false);
                    });
                    grid.Children.Add(label);
                    panel.Children.Add(grid);
                    return shellContainer.Name;
                }


                IsLoadNewNow = true;
                threadFileLoader = new Thread(new ParameterizedThreadStart(LoadShells)) { Name = "ShellLoader", ApartmentState = ApartmentState.STA };
                threadFileLoader.Start(list);
                    //list = shellContainer.OfType<ShellFolder>();
                //if (!showAll && !showDir) list = list.Where(a => a.IsImage());
                //else
                //{
                //    if (!showDir) list = list.Where(a => a is ShellFile);
                //    if (!showAll) list = list.Where(a => a.IsImage() || a is ShellFolder);
                //}
                //foreach (var item in list)
                //{
                //    Add(item);
                //}
                #region old
                //if (showDir)
                //{
                //    foreach (var item in shellContainer.OfType<ShellFolder>())
                //    {
                //        //ShellObject shellObject = ShellObject.FromParsingName(item);
                //        BitmapSource b;
                //        ShellThumbnail st = item.Thumbnail;
                //        b = st.MediumBitmapSource;
                //        //try
                //        //{
                //        //    st.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
                //        //    b = st.MediumBitmap;
                //        //}
                //        //catch
                //        //{
                //        //    st.FormatOption = ShellThumbnailFormatOption.IconOnly;
                //        //    b = st.MediumBitmap;
                //        //    b.MakeTransparent();
                //        //}
                //        bitmaps.Add(item.ParsingName, b);
                //        Grid panel = CreatePictureBoxWithLabel(item.Name, item.ParsingName, true, b);

                //        previews.Add(panel);
                //    }
                //}

                ////var l = Directory.GetFiles(path);
                //IEnumerable<ShellFile> l = shellContainer.OfType<ShellFile>();
                //if (!showAll)
                //{
                //    l = l.Where(a => a.IsImage());
                //}
                //foreach (var item in l)
                //{

                //    string name = System.IO.Path.GetFileName(item.Path);
                //    //ShellObject shellObject = ShellObject.FromParsingName(item);
                //    //shellFolder.
                //    //ShellFile shellFile = ShellFile.FromFilePath(item);
                //    BitmapSource b;
                //    ShellThumbnail st = item.Thumbnail;
                //    b = st.MediumBitmapSource;
                //    //try
                //    //{
                //    //    st.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
                //    //    b = st.MediumBitmap;
                //    //}
                //    //catch
                //    //{
                //    //    st.FormatOption = ShellThumbnailFormatOption.IconOnly;
                //    //    b = st.MediumBitmap;
                //    //    b.MakeTransparent();
                //    //}
                //    //b.MakeTransparent();
                //    bitmaps.Add(item.ParsingName, b);
                //    Grid panel = CreatePictureBoxWithLabel(name, item.ParsingName, false, b);
                //    previews.Add(panel);
                //    //previews.Add(new PictureBox { Size = new Size(64,64), Image = shellFile.Thumbnail.MediumIcon.ToBitmap(), SizeMode = PictureBoxSizeMode.Zoom });

                //}
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //previews.Clear();
                //if (shellContainer.Parent != null)
                //{
                //    previews.Add(CreatePictureBoxWithLabel("..", shellContainer.Parent.ParsingName, new Bitmap(64, 64), true));
                //}
                //else previews.
            }
            //foreach (var item in previews)
            //{
            //    panel.Children.Add(item);
            //}
            //this.ResumeLayout();
            //flowLayoutPanel1.ResumeLayout();
            return shellContainer.Name;
        }

        public struct ShItem
        {
            public ShItem(BitmapSource thumbnail, string name, string parsingName, bool isFolder)
            {
                Thumbnail = thumbnail;
                Name = name;
                ParsingName = parsingName;
                IsFolder = isFolder;
            }

            public BitmapSource Thumbnail { get; set; }
            public string Name { get; set; }
            public string ParsingName { get; set; }
            public bool IsFolder { get; set; }
        }

        public void LoadShells(object obj)
        {
            IEnumerable<ShellObject> shellObjects = obj as IEnumerable<ShellObject>;
            if (obj == null) return;
            var enumerator = shellObjects.GetEnumerator();
            int counter = 0;
            //ShItem[] buffer = new ShItem[20];
            int buffersize = 25;
            ShellObject[] buffer = new ShellObject[1000];
            while (enumerator.MoveNext())
            {
                if (IsLoadNewNow)
                {
                    ShellObject shellObject = enumerator.Current;
                    buffer[counter] = shellObject;
                    //buffer[counter] = new ShItem(shellObject.Thumbnail.MediumBitmapSource, shellObject.Name, shellObject.ParsingName, shellObject is ShellFolder);
                    counter++;
                    if (counter == buffersize)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            for (int i = 0; i < counter; i++)
                            {
                                if (IsLoadNewNow)
                                {
                                    ShellObject shItem = buffer[i];
                                    panel.Children.Add(CreatePictureBoxWithLabel(shItem));
                                }
                                else break;
                            }
                        });
                        counter = 0;
                    }
                }
                else break;
            }
            Dispatcher.Invoke(() =>
            {
                for (int i = 0; i < counter; i++)
                {
                    if (IsLoadNewNow)
                    {
                        ShellObject shItem = buffer[i];
                        panel.Children.Add(CreatePictureBoxWithLabel(shItem));
                    }
                    else break;
                }
                IsLoadNewNow = false;
            });
            //Dispatcher.Invoke(() => IsLoadNewNow = false);
        }

        //private void Add(ShellObject item)
        //{
        //    BitmapSource b;
        //    ShellThumbnail st = item.Thumbnail;
        //    b = st.MediumBitmapSource;
        //    bitmaps.Add(item.ParsingName, b);
        //    Grid grid = CreatePictureBoxWithLabel(item.Name, item.ParsingName, item is ShellFolder, b);
        //    previews.Add(grid);
        //}

        private Grid CreateGrid(ShellObject item)
        {
            BitmapSource b;
            ShellThumbnail st = item.Thumbnail;
            b = st.MediumBitmapSource;
            bitmaps.Add(item.ParsingName, b);
            
            return CreatePictureBoxWithLabel(item.Name, item.ParsingName, item is ShellFolder, b);
        }


        private void Panel_File_Click(string path)
        {
            FileSelect?.Invoke(this, new EventArgsWithFilePath(path));
        }

        private void Panel_Folder_Click(string path)
        {
            try
            {
                PathChanged?.Invoke(this, new EventArgsWithFilePath(path, LoadPreview(path, showAll, showDir)));
                this.path = path;
                //fileSystemWatcher1.Changed -= FileSystemWatcher1_Changed;
                //fileSystemWatcher1.Path = path;
                //fileSystemWatcher1.Changed += FileSystemWatcher1_Changed;
            }
            catch (Exception ex){ MessageBox.Show(ex.Message); }
        }

        private Grid CreatePictureBoxWithLabel(ShItem shItem)
        {
            return CreatePictureBoxWithLabel(shItem.Name, shItem.ParsingName, shItem.IsFolder, shItem.Thumbnail);
        }

        private Grid CreatePictureBoxWithLabel(ShellObject shItem)
        {
            return CreatePictureBoxWithLabel(shItem.Name, shItem.ParsingName, shItem is ShellFolder, shItem.Thumbnail.MediumBitmapSource);
        }

        private Grid CreatePictureBoxWithLabel(string name, string path,  bool IsDir, BitmapSource image = null)
        {
            Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogButton c = new Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogButton();
            Grid panel = new Grid{ Height = 90, Width = 64 };
            panel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(64) });
            panel.RowDefinitions.Add(new RowDefinition());
            //panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            
            TextBlock label = new TextBlock { Text = name, TextAlignment = TextAlignment.Center, TextWrapping = TextWrapping.Wrap };
            //panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));
            ////panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
            //panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            panel.Children.Add(label);
            Grid.SetRow(label, 1);
            //panel.Controls.Add(label, 0, 1);
            //pictureBox.Dock = label.Dock = DockStyle.Fill;


            if (IsDir)
            {
                panel.Tag = new PanelClick(path, Panel_Folder_Click);
            }
            else
            {
                panel.Tag = new PanelClick(path, Panel_File_Click);
            }

            panel.MouseLeftButtonDown += Panel_DoubleClick;
            label.MouseLeftButtonDown += SubPanel_Click;
            Image pictureBox = new Image {/* Image = image, */};
            if (image != null)
            {
                pictureBox.Source = image;
                
            }
            else
            {
                PixelFormat pf = PixelFormats.Bgra32;
                int width = 200;
                int height = 200;
                int rawStride = (width * pf.BitsPerPixel + 7) / 8;
                byte[] rawImage = new byte[rawStride * height];

                // Initialize the image with data.
                //Random value = new Random();
                //value.NextBytes(rawImage);

                // Create a BitmapSource.
                BitmapSource bitmap = BitmapSource.Create(width, height,
                    96, 96, pf, null,
                    rawImage, rawStride);
                
                pictureBox.Source = bitmap;
            }
            panel.Children.Add(pictureBox);
            Grid.SetRow(pictureBox, 0);
            pictureBox.MouseLeftButtonDown += SubPanel_Click;
            return panel;
        }

        private static void Panel_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount>=2)
            ((PanelClick)((FrameworkElement)sender).Tag).Click();
        }

        private static void SubPanel_Click(object sender, MouseButtonEventArgs e)
        {
            //if (e.ClickCount == 2)
            //    ((PanelClick)((Grid)((FrameworkElement)sender).Parent).Tag).Click();
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
        public static bool IsImage(this ShellObject SO)
        {
            return SO.ParsingName.PathIsImage();
            //string ext = System.IO.Path.GetExtension(SO.ParsingName).ToLower();
            //return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp";
        }
    }
}

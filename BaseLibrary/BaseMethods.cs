using Emgu.CV;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BaseLibrary
{
    public static class BaseMethods
    {
        private static bool _init = false;
        private static TabControl tabControl;
        internal static OutputImageInvoker _loadOutputImage;
        internal static GetImageForm _selectedForm;
        internal static OutputImageInvoker _createFormFromOutputImage;
        internal static GetProgressBar _getProgressBar;
        public static Dictionary<string, string> settings;

       
        /// <summary>
        /// Добовляйте уникальный префикс
        /// </summary>
        /// <param name="param"></param>
        public static string getSetting(object sender, string param, string value)
        {
            return settings[sender.GetType().FullName];
        }

        public static void deleteSetting(object sender, string param)
        {
            settings.Remove(sender.GetType().FullName);
        }

        public static void saveSetting(object sender, string param)
        {
            if (!Directory.Exists("Params"))
                Directory.CreateDirectory("Params");
            StreamWriter sr = new StreamWriter("setts.ixi");
            foreach (var item in settings)
            {
                sr.WriteLine(item.Key + ":::" + item.Value);
            }
            sr.Close();
        }
        public static void loadSetting()
        {
            if (!Directory.Exists("Params"))
                Directory.CreateDirectory("Params");
            else
            {
                StreamReader sw = new StreamReader("setts.ixi");
                string s;
                while ((s = sw.ReadLine())!=null)
                {
                    int c= s.LastIndexOf(":::");
                    settings[s.Substring(0, c)] = s.Substring(c + 3);
                }
                sw.Close();
            }
        }

        /// <summary>
        /// Добовляйте уникальный префикс
        /// </summary>
        /// <param name="param"></param>
        public static void addSetting(object sender, string param, string value)
        {
            settings.Add(sender.GetType().FullName + param, value);
        }

        public static OpenFileDialog GetOpenFileDialog(bool multiselect = false)
        {
            var ofd = new OpenFileDialog();
            ofd.Multiselect = multiselect;
            ofd.Filter = Extensions.GetFilterOpenFileDialog(true);
            return ofd;
        }

        public static SaveFileDialog GetSaveFileDialog()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = Extensions.GetFilterSaveFileDialog();
            return sfd;
        }

        /// <summary>
        /// Выполняет делегат в главном потоке
        /// </summary>
        /// <param name="method">Делегат, содержащий метод, который необходимо вызвать в главном потоке</param>
        /// <returns></returns>
        public static object Invoke(Delegate method)
        {
            return tabControl.Invoke(method);
        }
        /// <summary>
        /// Выполняет делегат в главном потоке, с указанным списком аргументов
        /// </summary>
        /// <param name="method">Делегат, содержащий метод, который необходимо вызвать в главном потоке</param>
        /// <param name="args">Массив объектов, передаваемых в качестве аргументов указаному методу</param>
        /// <returns></returns>
        public static object Invoke(Delegate method, params object[] args)
        {
            return tabControl.Invoke(method, args);
        }
        /// <summary>
        /// Не нужно использовать. Требуется для инициализации некоторых методов
        /// </summary>
        /// <param name="load"><see cref="LoadOutputImage(OutputImage)"/></param>
        public static void Init(TabControl tabControl, OutputImageInvoker load, GetImageForm selectedForm, MesWrites writes, GetProgressBar progress)
        {
            if (!_init)
            {
                _init = true;
                BaseMethods.tabControl = tabControl;
                _loadOutputImage = load;
                _selectedForm = selectedForm;
                On_Writing = writes;
                _getProgressBar = progress;
            }
        }

        /// <summary>
        /// Загружает <see cref="OutputImage"/> объект на главную форму
        /// </summary>
        /// <param name="outputImage"></param>
        public static void LoadOutputImage(OutputImage outputImage)
        {
            _loadOutputImage?.Invoke(outputImage);
        }

        public static ImageForm SelectedImageForm => _selectedForm.Invoke();
        public static IImage SelectedImage => 
            SelectedImageForm == null
                ? null 
                : SelectedImageForm.Image.IsDisposedOrNull()
                    ? null
                    : SelectedImageForm.Image;

        public static ImageForm CreateFormFromOutputImage(OutputImage outputImage)
        {
            ImageForm imageForm = _createFormFromOutputImage?.Invoke(outputImage);
            if (imageForm == null) return null;
            if (imageForm.NeedNewFormRegister)
                NewImageForm?.Invoke(imageForm, new EventArgsNewImageForm(imageForm));
            return imageForm;
        }

        /// <summary>
        /// Показывает форму в главной форме
        /// </summary>
        /// <param name="dockStyle">Заполнение формы</param>
        /// <param name="formBorderStyle">Границы формы</param>
        public static void ShowForm(ImageForm imageForm,
                                    FormBorderStyle formBorderStyle = FormBorderStyle.None,
                                    DockStyle dockStyle = DockStyle.Fill)
        {
            if (imageForm.NeedNewFormRegister)
                NewImageForm?.Invoke(imageForm, new EventArgsNewImageForm(imageForm));
            tabControl.Invoke(new MethodInvoker(() =>
            {
                imageForm.TextChanged += new EventHandler((Object obj, EventArgs arg) =>
                {
                    ImageForm form = (ImageForm)obj;
                    if (form.Parent != null)
                        form.Parent.Text = form.Text;
                });
                TabPage tabPage = new TabPage(imageForm.Text);
                tabControl.Controls.Add(tabPage);
                imageForm.TopLevel = false;
                imageForm.FormBorderStyle = formBorderStyle;
                imageForm.Dock = dockStyle;
                imageForm.Parent = tabPage;
                imageForm.Visible = true;
                //if (tabControl.TabPages.Contains(tabPage))
                //    tabControl.SelectedTab = tabPage;
                imageForm.Show();
            }));
        }

        /// <summary>
        /// Вызывает imageForm.Worker.RunWorkerAsync(workerArgument), позволяя выполнять вычислительные операции без зависания главной формы. После завершения, показывает форму
        /// </summary>
        /// <param name="workerArgument">Аргументы для Worker</param>
        /// <param name="dockStyle">Заполнение формы</param>
        /// <param name="formBorderStyle">Границы формы</param>
        public static void ShowFormAsync(ImageForm imageForm,
                                         Object workerArgument,
                                         FormBorderStyle formBorderStyle = FormBorderStyle.None,
                                         DockStyle dockStyle = DockStyle.Fill)
        {
            if (imageForm.NeedNewFormRegister)
                NewImageForm?.Invoke(imageForm, new EventArgsNewImageForm(imageForm));
            tabControl.Invoke(new MethodInvoker(() =>
            {
                imageForm.TextChanged += new EventHandler((Object obj, EventArgs arg) =>
                {
                    ImageForm form = (ImageForm)obj;
                    if (form.Parent != null)
                        form.Parent.Text = form.Text;
                });
                TabPage tabPage = new TabPage(imageForm.Text);
                tabControl.Controls.Add(tabPage);
                tabPage.UseWaitCursor = true;
                imageForm.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler((Object obj, System.ComponentModel.RunWorkerCompletedEventArgs arg) =>
                {
                    BackgroundWorkerImg worker = (BackgroundWorkerImg)obj;
                    worker.ImageForm.TopLevel = false;
                    worker.ImageForm.Parent = worker.TabPage;
                    worker.TabPage.UseWaitCursor = false;
                    worker.ImageForm.Dock = dockStyle;
                    worker.ImageForm.FormBorderStyle = formBorderStyle;
                    worker.ImageForm.Visible = true;
                    if (worker.ImageForm.AutoSelect) worker.ImageForm.MakeSelected();
                });
                imageForm.Worker.TabPage = tabPage;
                imageForm.Worker.RunWorkerAsync(workerArgument);
                imageForm.Worker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler((o, e) =>
                {
                    imageForm.Show();
                });
            }));
        }
        //static недоступен делегату
        public delegate void MesWrites(string s);
        public static event MesWrites On_Writing;

        public static Point GetCoord(IImage image)
        {
            SelectCoord selectCoord = new SelectCoord(image, false);
            selectCoord.ShowDialog();
            return selectCoord.SelectedPoint;
        }

        public static void WriteLog(string s)
        {
            On_Writing(s);
        }

        public static event EventHandler<EventArgsNewImageForm> NewImageForm;


        //public static void LoadOutputImage(OutputImage outputImage)
        //{
        //    tabControl.Invoke(new MethodInvoker(() => {
        //        if (outputImage != null)
        //        {

        //            if (outputImage.Image != null)
        //                MainForm.OpenImage(outputImage.Image, outputImage.Name);
        //            MenuMethod.CreateImage(outputImage.Image);
        //            //imageBox.Image = outputImage.Image;
        //            if (outputImage.Info != null)
        //                textBox.Text = outputImage.Info;
        //        }
        //    }));
        //}

    }

    public delegate ImageForm GetImageForm();
    public delegate ImageForm OutputImageInvoker(OutputImage outputImage);
    public delegate InputImage.InitProgress GetProgressBar(InputImage inputImage);

    public class EventArgsNewImageForm : EventArgs
    {
        public EventArgsNewImageForm(ImageForm imageForm) => ImageForm = imageForm;
        public ImageForm ImageForm { get; }
    }
}

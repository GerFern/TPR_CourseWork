using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseLibrary
{
    /// <summary>
    /// Отмечает, что в данном классе имеются статические методы, которые нужно использовать для обработки изображений
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ImgClass : Attribute
    {
        public string StudentName { get; }
        /// <summary>
        /// Отмечает, что в данном классе имеются статические методы, которые нужно использовать для обработки изображений
        /// </summary>
        /// <param name="StudentName">ФИО студента, который выполнил задание</param>
        public ImgClass(string StudentName)
        {
            this.StudentName = StudentName;
        }
    }

    /// <summary>
    /// Отмечает, что данный метод участвует в обработке изображения и должен быть встроен в программу
    /// <para/>Метод должен возвращать <see cref="BaseLibrary.OutputImage"/>
    /// <para/>Первый параметр должен быть производным от <see cref="Emgu.CV.IImage"/>.
    /// Рекомендуемый тип <see cref="Emgu.CV.Image{TColor, TDepth}"/>, где TColor - <seealso cref="Emgu.CV.Structure.Bgr"/> или <seealso cref="Emgu.CV.Structure.Gray"/>, а TDepth - <seealso cref="byte"/>
    /// <para/>Следующие параметры можно определять как угодно
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ImgMethod: Attribute
    {
        public string[] Hierarchy { get; }
        /// <summary>
        /// Отмечает, что данный метод участвует в обработке изображения и должен быть встроен в программу
        /// <para/> 
        /// <code>
        /// Пример: [<see cref="ImgMethod"/>("FIO","Фильтр","ч/б")]
        /// <para/>
        /// ImageHandler(IImage image) { ... }
        /// </code>
        /// </summary>
        /// <param name="Hierarchy">Иерархия вкладок в которой будет встроена кнопка вызова метода, где последний элемент - название фильтра</param>
        /// <example>
        /// <code>
        /// [ImageProcessing("Фильтр","Категория 1","Название фильтра")]
        /// public ImageOutput(IImage input) {}
        /// </code>
        /// </example>
        public ImgMethod(params string[] Hierarchy)
        {
            this.Hierarchy = Hierarchy;
        }
        protected ImgMethod() { }
    }

    /// <summary>
    /// Отмечает, что для данного метода будет автоматически сконструирована простая форма выбора параметров.
    /// Если нужно несколько параметров, то необходимо определить несколько таких атрибутов
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class AutoForm : Attribute
    {
        /// <summary>
        /// Тип параметра
        /// </summary>
        public Type Type { get; }
        /// <summary>
        /// Текст параметра
        /// </summary>
        public string LabelText { get; }
        /// <summary>
        /// Индекс параметра
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// Высота элемента <see cref="System.Windows.Forms.TextBox"/> для ввода
        /// </summary>
        public int TextBoxHeigth { get; }
        /// <summary>
        /// Многстрочное тесктовое поле
        /// </summary>
        public bool IsMultiline { get; }
        /// <summary>
        /// Отмечает, что для данного метода будет автоматически сконструирована простая форма выбора параметров.
        /// Если нужно несколько параметров, то необходимо определить несколько таких атрибутов
        /// </summary>
        /// <param name="Index">Индекс параметра метода, начинающийся с 1</param>
        /// <param name="Type">Тип параметра. Поддерживаются <see cref="int"/>, <see cref="float"/>, <see cref="double"/>. Получить через <see langword="typeof"/></param>
        /// <param name="LabelText">Название параметра, который будет отображаться в форме.</param>
        /// <param name="IsMultiline">Многстрочное тесктовое поле/></param>
        /// <param name="TextBoxHeigth">Высота текстового поля, если свойство <paramref name="IsMultiline"/> == <see langword="true"/></param>
        public AutoForm(int Index, Type Type, string LabelText, bool IsMultiline = false, int TextBoxHeigth = 26)
        {
            this.Index = Index;
            this.Type = Type;
            this.LabelText = LabelText;
            this.IsMultiline = IsMultiline;
            this.TextBoxHeigth = TextBoxHeigth;
        }
    }

    /// <summary>
    ///  Отмечает, что для данного метода использоваться особая форма
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CustomForm : Attribute
    {
        /// <summary>
        /// <see cref="Type"/> формы
        /// </summary>
        public Type FormType { get; }
        /// <summary>
        ///  Отмечает, что для данного метода будет использоваться особая форма 
        /// </summary>
        /// <param name="baseAttribute">Базовый атрибут с необходимыми параметрами</param>
        /// <param name="FormType"><see cref="Type"/> формы. Получить через <see langword="typeof"/>(MyFormClass)</param>
        public CustomForm(Type FormType)
        {
            this.FormType = FormType;
        }
    }

    public class InputImage
    {
        public int ID { get; }
        public IImage Image { get; }
        public Type ImageType { get; }
        public Type TColor { get; }
        public Type TDepth { get; }
        public ProgressInfo Progress { get; }

        public InputImage(IImage image, int ID)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            this.ID = ID;
            ImageType = image.GetType();
            Type[] t = ImageType.GetGenericArguments();
            try
            {
                TColor = t[0];
                TDepth = t[1];
            }
            catch { }
            Progress = new ProgressInfo(this);
        }

        public class ProgressInfo
        {
            //public void Show();
            internal InputImage _inputImage;
            /// <summary>
            /// Возвращает или задает наибольшее значение диапозона элемента управления <see cref="ProgressBar"/>
            /// </summary>
            public int Maximum
            {
                get => progressBar.Maximum;
                set => progressBar.Invoke(new Action(() => { progressBar.Maximum = value; }));
            }

            /// <summary>
            /// Возвращает или задает наименьшее значение диапозона элемента управления <see cref="ProgressBar"/>
            /// </summary>
            public int Minimum
            {
                get => progressBar.Minimum;
                set => progressBar.Invoke(new Action(() => { progressBar.Minimum = value; }));
            }

            /// <summary>
            /// Возвращает или задает текущее положение индикатора выполнения
            /// </summary>
            public int Value
            {
                get => progressBar.Value;
                set => progressBar.Invoke(new Action(() => { progressBar.Value = value; }));
            }

            /// <summary>
            /// Возвращает или задает интервал, в котором вызов <see cref="PerformStep()"/> метод увеличивает текущее положение индикатора хода выполнения
            /// </summary>
            public int Step
            {
                get => progressBar.Step;
                set => progressBar.Invoke(new Action(() => { progressBar.Step = value; }));
            }

            /// <summary>
            /// Возвращает или задает способ отображения выполнения в индикаторе выполнения
            /// </summary>
            public ProgressBarStyle Style
            {
                get => progressBar.Style;
                set => progressBar.Invoke(new Action(() => progressBar.Style = value));
            }

            /// <summary>
            /// Увеличивает текущую позицию индикатора хода выполнения на объем <see cref="Step"/> свойство
            /// </summary>
            public void PerformStep()
            {
                progressBar.Invoke(new Action(() => progressBar.PerformStep()));
            }

            /// <summary>
            /// Инициализация прогресса
            /// </summary>
            /// <param name="step">Интервал, в котором вызов <see cref="PerformStep()"/> метод увеличивает текущее положение индикатора хода выполнения</param>
            /// <param name="maximum">Наибольшее значение диапозона элемента управления <see cref="ProgressBar"/>
            public void Run(int step, int maximum)
            {
                if (progressBar == null)
                {
                    progressBar = BaseMethods._getProgressBar(_inputImage);
                    Step = step;
                    Maximum = maximum;
                }
            }

            /// <summary>
            /// Завершение выполнения операции
            /// </summary>
            public void Finish()
            {
                if (progressBar != null)
                {
                    //Будут доработки, но метод уже можно (даже желательно) использовать
                    Value = Maximum;
                }
            }
            ProgressBar progressBar;
            internal ProgressInfo(InputImage inputImage)
            {
                _inputImage = inputImage;
            }
        }
    }

    /// <summary>
    /// Обработанное изображение и информация
    /// </summary>
    public class OutputImage
    {
        /// <summary>
        /// Выходная информация. Если <see langword="null"></see>, то информация не требуется
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// Обработанное изображение. Если <see langword="null"></see>, то изменения в изображении не требуются
        /// <para>Рекомендуемый тип <see cref="Emgu.CV.Image{TColor, TDepth}"/>, где TColor - <seealso cref="Emgu.CV.Structure.Bgr"/> или <seealso cref="Emgu.CV.Structure.Gray"/>, а TDepth - <seealso cref="byte"/></para>
        /// </summary>
        public Emgu.CV.IImage Image { get; set; }
        /// <summary>
        /// Название нового изображения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Добавить форму, производную от <see cref="BaseLibrary.ImageForm"/>
        /// </summary>
        public ImageForm ImageForm { get; set; }
        /// <summary>
        /// Предназначено для обновления выбранного изображения
        /// </summary>
        public ImageForm UpdateSelectedImage { get; set; }
    }

    public delegate ImageForm OutputImageInvoker(OutputImage outputImage);
    public delegate ProgressBar GetProgressBar(InputImage inputImage);
    public static class BaseMethods
    {
        private static bool _init = false;
        private static TabControl tabControl;
        private static OutputImageInvoker _loadOutputImage;
        private static OutputImageInvoker _createFormFromOutputImage;
        internal static GetProgressBar _getProgressBar;
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
        public static void Init(TabControl tabControl, OutputImageInvoker load, OutputImageInvoker create, MesWrites writes, GetProgressBar progress)
        {
            if (!_init)
            {
                _init = true;
                BaseMethods.tabControl = tabControl;
                _loadOutputImage = load;
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
            BaseMethods.Invoke(new Action(()=> _loadOutputImage?.Invoke(outputImage)));
        }

        public static ImageForm CreateFormFromOutputImage(OutputImage outputImage)
        {
            return _createFormFromOutputImage == null 
                ? null
                : _createFormFromOutputImage.Invoke(outputImage);
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
            }));
        }
        //static недоступен делегату
        public delegate void MesWrites(string s);
        public static event MesWrites On_Writing;

        static void WriteMessege(string s)
        {
            On_Writing(s);
        }

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
}

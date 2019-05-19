using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseLibrary
{
    public partial class BaseForm : Form
    {
        [Obsolete]
        /// <summary>
        /// Результат
        /// </summary>
        public OutputImage OutputImage { get => outputImage; }
        public InputImage InputImage { get; }
        public bool IsInvoked { get; private set; }
        private OutputImage outputImage;
        public MethodInfo MethodInfo { get; }
        private BaseForm()
        {
            InitializeComponent();
            BackgroundThread = new Thread(InvokePreview);
        }
        /// <summary>
        /// Входное изображение для обработки
        /// </summary>
        public IImage Image { get; }
        /// <summary>
        /// Здесь будут храниться параметры для функции. Элемент с индексом 0 должен соответсвовать интерфейсу <see cref="Emgu.CV.IImage"/> или быть элементом класса <see cref="InputImage"/>
        /// </summary>
        public object[] Vs { get; }
        /// <summary>
        /// Необходимо вызвать конструктор базовой формы в конструкторе производной формы
        /// </summary>
        /// <param name="methodInfo">Метаданные метода</param>
        /// <param name="image">Входное изображение</param>
        public BaseForm(IImage image, MethodInfo methodInfo) : this()
        {
            MethodInfo = methodInfo;
            Vs = new object[methodInfo.GetParameters().Length];
            Vs[0] = Image = image;
        }
        public BaseForm(InputImage inputImage, MethodInfo methodInfo) : this()
        {
            MethodInfo = methodInfo;
            Vs = new object[methodInfo.GetParameters().Length];
            Vs[0] = InputImage = inputImage;
        }

        [Obsolete("Лучше не использовать", false)]
        /// <summary>
        /// Применение метода. В последних версиях возможно откажусь от этого
        /// </summary>
        public virtual bool Accept()
        {
            try
            {
                if (MethodInfo.GetParameters()[0].ParameterType == typeof(InputImage))
                    outputImage = (OutputImage)MethodInfo.Invoke(null, Vs);
                IsInvoked = true;
                return true;
            }
            catch (System.Reflection.TargetInvocationException ex)//Обработка исключений в MethodInfo
            {
                MessageBox.Show(ex.InnerException.Message, "Ошибка");
                return false;
            }
            catch (Exception ex)//Базовый Exception лучше использовать в последнюю очередь
            {
                MessageBox.Show(ex.Message, "Ошибка");
                return false;
            }
        }

        Thread BackgroundThread;

        /// <summary>
        /// Вызвать метод для предпоказа в глобальной форме
        /// </summary>
        /// <param name="otherThread">Вызывать ли метод в отдельном потоке. Если поток уже запущен и до сих пор работает, то будет возвращено <see langword="false"/></param>
        public virtual bool Preview(bool otherThread)
        {
            if (otherThread)
            {
                ThreadState threadState = BackgroundThread.ThreadState;
                if (threadState.HasFlag(ThreadState.Stopped)
                    || threadState.HasFlag(ThreadState.Aborted))
                    BackgroundThread = new Thread(InvokePreview) { Name = $"BackgroundInvoker({MethodInfo.Name})" };
                if (threadState.HasFlag(ThreadState.Unstarted))
                {
                    BackgroundThread.Start(Vs);
                    return true;
                }
                return false;
            }
            else
            {
                InvokePreview(Vs);
                return true;
            }
        }

        
        private void InvokePreview(object vs)
        {
            if (vs is object[] Vs)
            {
                object result = MethodInfo.Invoke(null, Vs);
                if (result is OutputImage outputImage)
                {
                    outputImage.GlobalImage = outputImage.Image;
                    outputImage.Image = null;
                    outputImage.GlobalForm = outputImage.ImageForm;
                    outputImage.ImageForm = null;
                    BaseMethods.LoadOutputImage(outputImage);
                }
            }
        }
    }
}

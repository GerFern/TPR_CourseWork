using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
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
        public bool IsInvoked { get; private set; }
        private OutputImage outputImage;
        public MethodInfo MethodInfo { get; }
        /// <summary>
        /// Нужно только для дизайнера форм. Не использовать
        /// </summary>
        private BaseForm() { }
        /// <summary>
        /// Входное изображение для обработки
        /// </summary>
        public IImage image;
        /// <summary>
        /// Здесь будут храниться параметры для функции. Элемент с индексом 0 должен соответсвовать интерфейсу <see cref="Emgu.CV.IImage"/> или быть элементом класса <see cref="InputImage"/>
        /// </summary>
        public object[] Vs { get; }
        /// <summary>
        /// Необходимо вызвать конструктор базовой формы в конструкторе производной формы
        /// </summary>
        /// <param name="methodInfo">Метаданные метода</param>
        /// <param name="ParametersCount">Общее число параметров метода</param>
        /// <param name="image">Входное изображение</param>
        public BaseForm(IImage image, MethodInfo methodInfo)
        {
            InitializeComponent();
            MethodInfo = methodInfo;
            Vs = new object[methodInfo.GetParameters().Length];
            Vs[0] = image;
        }
        public BaseForm(InputImage inputImage, MethodInfo methodInfo)
        {
            InitializeComponent();
            MethodInfo = methodInfo;
            Vs = new object[methodInfo.GetParameters().Length];
            Vs[0] = inputImage;
        }

        [Obsolete("Можно не использовать", false)]
        /// <summary>
        /// Применение метода. В последних версиях возможно откажусь от этого
        /// </summary>
        public virtual bool Accept()
        {
            try
            {
                if(MethodInfo.GetParameters()[0].ParameterType == typeof(InputImage))
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
    }
}

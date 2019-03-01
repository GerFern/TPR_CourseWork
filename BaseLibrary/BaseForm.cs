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
        /// <summary>
        /// Результат
        /// </summary>
        public OutputImage OutputImage { get => outputImage; }
        private OutputImage outputImage;
        MethodInfo MethodInfo { get; }
        /// <summary>
        /// Нужно только для дизайнера форм. Не использовать
        /// </summary>
        private BaseForm() { }
        /// <summary>
        /// Входное изображение для обработки
        /// </summary>
        public IImage image;
        /// <summary>
        /// Здесь будут храниться параметры для функции. Элемент с индексом 0 должен быть объектом класса <see cref="Emgu.CV.IImage"/> или его производным
        /// </summary>
        public object[] Vs { get; }
        /// <summary>
        /// Необходимо вызвать конструктор базовой формы в конструкторе производной формы
        /// </summary>
        /// <param name="methodInfo">Метаданные метода</param>
        /// <param name="ParametersCount">Общее число параметров метода</param>
        /// <param name="image">Входное изображение</param>
        protected BaseForm(IImage image, MethodInfo methodInfo)
        {
            InitializeComponent();
            MethodInfo = methodInfo;
            Vs = new object[methodInfo.GetParameters().Length];
            Vs[0] = image;
        }

        /// <summary>
        /// Применение метода
        /// </summary>
        public virtual bool Accept()
        {
            try
            {
                outputImage = (OutputImage)MethodInfo.Invoke(null, Vs);
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

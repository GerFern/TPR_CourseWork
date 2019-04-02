using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    }
}

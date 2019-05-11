﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseLibrary
{
    public abstract class TPRAttribute : Attribute { }
    /// <summary>
    /// Отмечает, что в данном классе имеются статические методы, которые нужно использовать для обработки изображений
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ImgClassAttribute : TPRAttribute
    {
        public string[] Authors { get; }
        /// <summary>
        /// Отмечает, что в данном классе имеются статические методы, которые нужно использовать для обработки изображений
        /// </summary>
        /// <param name="authors">ФИО разработчиков, которые написали код</param>
        public ImgClassAttribute(params string[] authors)
        {
            this.Authors = authors;
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
    public class ImgMethod : TPRAttribute
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

    public abstract class TPRFormAttribute : TPRAttribute
    {
        /// <summary>
        /// Текст параметра
        /// </summary>
        public string LabelText { get; }
        /// <summary>
        /// Индекс параметра
        /// </summary>
        public int Index { get; }

        protected TPRFormAttribute(string labelText, int index)
        {
            LabelText = labelText;
            Index = index;
        }
    }

    /// <summary>
    /// Отмечает, что для данного метода будет автоматически сконструирована простая форма выбора параметров.
    /// Если нужно несколько параметров, то необходимо определить несколько таких атрибутов
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class AutoFormAttribute : TPRFormAttribute
    {
        /// <summary>
        /// Тип параметра
        /// </summary>
        public Type Type { get; }
        
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
        /// <param name="index">Индекс параметра метода, начинающийся с 1</param>
        /// <param name="type">Тип параметра. Поддерживаются <see cref="int"/>, <see cref="float"/>, <see cref="double"/>. Получить через <see langword="typeof"/></param>
        /// <param name="labelText">Название параметра, который будет отображаться в форме.</param>
        /// <param name="isMultiline">Многстрочное тесктовое поле/></param>
        /// <param name="textBoxHeigth">Высота текстового поля, если свойство <paramref name="isMultiline"/> == <see langword="true"/></param>
        public AutoFormAttribute(int index, Type type, string labelText, bool isMultiline = false, int textBoxHeigth = 26) : base(labelText, index)
        {
            this.Type = type;
            this.IsMultiline = isMultiline;
            this.TextBoxHeigth = textBoxHeigth;
        }
    }

    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ControlFormAttribute : TPRFormAttribute
    {
        public ControlFormAttribute(Type controlType, string propertyValue, int index, string labelText) : base(labelText, index)
        {
            if (string.IsNullOrEmpty(propertyValue))
                throw new ArgumentNullException(nameof(propertyValue));
            if (string.IsNullOrEmpty(labelText))
                throw new ArgumentNullException(nameof(labelText));

            ControlType = controlType ?? throw new ArgumentNullException(nameof(controlType));
            Property = propertyValue;
        }

        public Type ControlType { get; }
        public string Property { get; }
    }
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ControlPropertyAttribute : TPRAttribute
    {
        public ControlPropertyAttribute(string propertyName, string propertyValue, int paramIndex)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            PropertyValue = propertyValue ?? throw new ArgumentNullException(nameof(propertyValue));
            ParamIndex = paramIndex;
        }

        public string PropertyName { get; }
        public string PropertyValue { get; }
        public int ParamIndex { get; }
    }

    /// <summary>
    ///  Отмечает, что для данного метода использоваться особая форма
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class CustomFormAttribute : TPRAttribute
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
        public CustomFormAttribute(Type FormType)
        {
            this.FormType = FormType;
        }
    }

 
}
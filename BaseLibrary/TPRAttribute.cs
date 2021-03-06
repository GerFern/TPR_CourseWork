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
    public class ImgMethodAttribute : TPRAttribute
    {
        public string[] Hierarchy { get; }
        /// <summary>
        /// Отмечает, что данный метод участвует в обработке изображения и должен быть встроен в программу
        /// <para/> 
        /// <code>
        /// Пример: [<see cref="ImgMethodAttribute"/>("FIO","Фильтр","ч/б")]
        /// <para/>
        /// ImageHandler(IImage image) { ... }
        /// </code>
        /// </summary>
        /// <param name="hierarchy">Иерархия вкладок в которой будет встроена кнопка вызова метода</param>
        /// <example>
        /// <code>
        /// [ImageProcessing("Фильтр","Категория 1","Название фильтра")]
        /// public ImageOutput(IImage input) {}
        /// </code>
        /// </example>
        public ImgMethodAttribute(params string[] hierarchy)
        {
            this.Hierarchy = hierarchy;
        }
        protected ImgMethodAttribute() { }
    }

    /// <summary>
    /// Задает имя методу
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class MethodNameAttribute : TPRAttribute
    {
        public string Name { get; }
        /// <summary>
        /// Задает имя методу
        /// </summary>
        /// <param name="name">Новое имя для метода</param>
        public MethodNameAttribute(string name) => Name = name ?? throw new ArgumentNullException(nameof(name));
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
    /// Если нужно несколько параметров, то необходимо определить несколько таких атрибутов.
    /// Поле для ввода параметра будет текстовым
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
        /// Если нужно несколько параметров, то необходимо определить несколько таких атрибутов.
        /// Поле для ввода параметра будет текстовым
        /// </summary>
        /// <param name="index">Индекс параметра метода</param>
        /// <param name="type">Тип параметра. Поддерживаются базовые типы, такие как <see cref="int"/>, <see cref="float"/>, <see cref="double"/>, <see cref="string"/> и другие. Получить через <see langword="typeof"/></param>
        /// <param name="labelText">Название параметра, который будет отображаться в форме.</param>
        /// <param name="isMultiline">Является ли тесктовое поле многострочным/></param>
        /// <param name="textBoxHeigth">Высота текстового поля, если свойство <paramref name="isMultiline"/> == <see langword="true"/></param>
        public AutoFormAttribute(int index, Type type, string labelText, bool isMultiline = false, int textBoxHeigth = 26) : base(labelText, index)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.IsMultiline = isMultiline;
            this.TextBoxHeigth = textBoxHeigth;
        }
    }

    /// <summary>
    /// Отмечает, что для данного метода будет автоматически сконструирована простая форма выбора параметров.
    /// Если нужно несколько параметров, то необходимо определить несколько таких атрибутов.
    /// Тип поля для ввода указывается вручную
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ControlFormAttribute : TPRFormAttribute
    {
        /// <summary>
        /// Отмечает, что для данного метода будет автоматически сконструирована простая форма выбора параметров.
        /// Если нужно несколько параметров, то необходимо определить несколько таких атрибутов.
        /// Тип поля для ввода указывается вручную
        /// </summary>
        /// <param name="index">Индекс параметра метода</param>
        /// <param name="controlType">Тип элемента управления, производный от <see cref="Control"/>, который будет управлять параметром</param>
        /// <param name="propertyValue">Свойство элемента управления, которое будет связано с параметром метода</param>
        /// <param name="labelText">Надпись над элементом управления. Если <see langword="null"/>, то надпись не будет показана</param>
        public ControlFormAttribute(int index, Type controlType, string propertyValue, string labelText) : base(labelText, index)
        {
            if (string.IsNullOrEmpty(propertyValue))
                throw new ArgumentNullException(nameof(propertyValue));
            ControlType = controlType ?? throw new ArgumentNullException(nameof(controlType));
            Property = propertyValue;
        }

        /// <summary>
        /// Тип элемента управления
        /// </summary>
        public Type ControlType { get; }
        /// <summary>
        /// Основное свойство
        /// </summary>
        public string Property { get; }
    }

    /// <summary>
    /// Позволяет задать свойство для элемента управления
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ControlPropertyAttribute : TPRAttribute
    {
        /// <summary>
        /// Позволяет задать свойство для элемента управления
        /// </summary>
        /// <param name="paramIndex">Индекс параметра метода, к которому привязан элемент управления</param>
        /// <param name="propertyName">Название свойства</param>
        /// <param name="propertyValue">Значение свойства</param>
        public ControlPropertyAttribute(int paramIndex, string propertyName, string propertyValue)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            PropertyValue = propertyValue ?? throw new ArgumentNullException(nameof(propertyValue));
            ParamIndex = paramIndex;
        }

        public ControlPropertyAttribute(int paramIndex, string propertyName, object propertyObject)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
            PropertyObject = propertyObject ?? throw new ArgumentNullException(nameof(propertyObject));
            ParamIndex = paramIndex;
        }

        /// <summary>
        /// Название свойства
        /// </summary>
        public string PropertyName { get; }
        /// <summary>
        /// Значение свойства
        /// </summary>
        public string PropertyValue { get; }

        public object PropertyObject { get; }

        /// <summary>
        /// Индекс параметра
        /// </summary>
        public int ParamIndex { get; }
    }

    /// <summary>
    /// Отмечает, что для данного метода использоваться особая форма
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
        /// <param name="formType"><see cref="Type"/> формы. Получить через <see langword="typeof"/>(MyFormClass)</param>
        public CustomFormAttribute(Type formType)
        {
            FormType = formType;
        }
    }

    /// <summary>
    /// Отмечает, что для данного метода не обязательно иметь изображение
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ImgCanBeDisposedOrNullAttribute : TPRAttribute { }
    /// <summary>
    /// Отмечает, что не следует обращать внимание на вызываемые исключения в этом методе
    /// </summary>
    public sealed class DontCatchExceptionAttribute : TPRAttribute { }

    /// <summary>
    /// Отмечает, что данное свойство или поле будет сохраняться при использовании методов 
    /// <see cref="Extensions.SaveSetting{T}(T, string)"/> и 
    /// <see cref="Extensions.LoadSettings{T}(T, string)"/>
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public sealed class SaveParamAttribute : TPRAttribute { }
}

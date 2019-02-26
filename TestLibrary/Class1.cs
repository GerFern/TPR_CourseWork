using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary;
using Emgu.CV;
using Emgu.CV.Structure;

namespace TestLibrary
{
    [ImgClass("Лялин М.С.")]//Тут можно указать свое имя, а также пометить, что в этом классе есть методы для обработки изображений
    public static class Class1
    {
        /// <summary>
        /// Преобразование в оттенки серого
        /// </summary>
        /// <param name="image">Оригинальное изображение</param>
        /// <returns>Обработаное изображение</returns>
        [ImgMethod("Фильтр", "Конвертация", "Оттенки серого")]//Указывается иерархия вкладок в меню программы
        public static OutputImage TestGray(Image<Bgr, byte> image)
        {
            Image<Gray, byte> grayImage = image.Convert<Gray, byte>();
            return new OutputImage { Image = grayImage };
        }

        /// <summary>
        /// Медианная филтрация изображения
        /// </summary>
        /// <param name="image">Оригинальное изображение</param>
        /// <param name="size">Размер фильтрации (число должно быть положительным и нечетным)</param>
        /// <returns>Обработаное изображение</returns>
        [ImgMethod("Фильтр", "Медианая фильтрация")]
        [AutoForm(1, typeof(int), "Размер фильтрации")]//Автоматическое построение формы
        public static OutputImage TestMedian(IImage image, int size)
        {
            dynamic img = image;//Простой обход проверки на тип, чтобы не определять универсальные параметры Image используя к рефлексию
            return new OutputImage { Image = img.SmoothMedian(size), Info = $"Параметр медианной фильтрации - {size}" };
        }

        /// <summary>
        /// Тестовое сообщение
        /// </summary>
        /// <param name="image">Оригинальное изображение</param>
        /// <param name="s1">Текст 1</param>
        /// <param name="s2">Текст 2</param>
        /// <returns></returns>
        [ImgMethod("Анализ", "Тестовое сообщение")]
        [AutoForm(1, typeof(string), "Текст 1")]
        [AutoForm(2, typeof(string), "Текст 2", true, 100)]//Форма с несколькими параметрами
        public static OutputImage TestMessage(IImage image, string s1, string s2)
        {
            return new OutputImage { Info = $"Здесь могла быть любая информация для вывода{Environment.NewLine}{s1}{Environment.NewLine}{s2}" };
        }

        [ImgMethod("Фильтр", "Гауссовое размытие")]
        [CustomForm(typeof(Form1))]//Своя форма для метода
        public static OutputImage GausForm(IImage image, string s1, string s2, int kernelSize)
        {
            dynamic img = image;
            return new OutputImage
            {
                Info = $"{s1}{Environment.NewLine}{s2}",
                Image = img.SmoothGaussian(kernelSize)
            };
        }
    }
}

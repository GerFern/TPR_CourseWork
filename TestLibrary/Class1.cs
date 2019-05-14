using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseLibrary;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace TestLibrary
{
    [ImgClass("Лялин М.С.")]//Тут можно указать свое имя, а также пометить, что в этом классе есть методы для обработки изображений
    public static class Class1
    {
        [ImgMethod("Test", "Debug", "Exception")]
        [ImgCanBeDisposedOrNull]
        public static OutputImage TestException(InputImage inputImage)
        {
            inputImage.Progress.Run(1, 2);
            try
            {
                throw new Exception("AAAA");
            }
            catch { }
            inputImage.Progress.PerformStep();
            //Thread.Sleep(10000);
            int zero = 0;
            int t = int.MaxValue / zero;
            inputImage.Progress.Finish();
            return null;
        }
        /// <summary>
        /// Преобразование в оттенки серого
        /// </summary>
        /// <param name="image">Оригинальное изображение</param>
        /// <returns>Обработаное изображение</returns>
        [ImgMethod("Test", "Фильтр", "Конвертация", "Оттенки серого")]//Указывается иерархия вкладок в меню программы
        public static OutputImage TestGray(Image<Bgr, byte> image)
        {
            Image<Gray, byte> grayImage = image.Convert<Gray, byte>();
            return new OutputImage { Image = grayImage };
        }

        [ImgMethod("Test", "Debug", "for")]
        public static OutputImage TestFor(InputImage inputImage)
        {
            inputImage.Progress.Run(1, 100000);
            for (int i = 0; i < 100000; i++)
            {
                for (int j = 0; j < 100000; j++)
                {

                }
                inputImage.Progress.PerformStep();
            }
            inputImage.Progress.Finish();
            return null;
        }
        /// <summary>
        /// Медианная филтрация изображения
        /// </summary>
        /// <param name="image">Оригинальное изображение</param>
        /// <param name="size">Размер фильтрации (число должно быть положительным и нечетным)</param>
        /// <returns>Обработаное изображение</returns>
        [ImgMethod("Test", "Фильтр", "Медианая фильтрация")]
        [ControlForm(1, typeof(NumericUpDown), "Value", "Размер фильтрации")]//Автоматическое построение формы
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
        [ImgMethod("Test", "Анализ", "Тестовое сообщение")]
        //[AutoForm(1, typeof(string), "Текст 1")]
        [ControlForm(1, typeof(TextBox), "Text", "Текст_")]
        [AutoForm(2, typeof(string), "Текст 2", true, 100)]//Форма с несколькими параметрами
        [BaseLibrary.ImgCanBeDisposedOrNull]
        public static OutputImage TestMessage(IImage image, string s1, string s2)
        {
            if(new Random().Next(2)==1)
            Thread.Sleep(1000);
            return new OutputImage { Info = $"Здесь могла быть любая информация для вывода{Environment.NewLine}{s1}{Environment.NewLine}{s2}" };
        }

        [ImgMethod("Test", "Фильтр", "Гауссовое размытие")]
        [CustomForm(typeof(Form1))]//Своя форма для метода
        public static OutputImage GausForm(InputImage inputImage, int iteration, int iterTime, int kernelSize, bool new_img)
        {
            dynamic img = inputImage.Image;
            inputImage.Progress.Run(1, iteration);
            for (int i = 0; i < iteration; i++)
            {
                Thread.Sleep(iterTime);
                inputImage.Progress.PerformStep();
            }
            inputImage.Progress.Finish();
            img = img.SmoothGaussian(kernelSize);
            if (new_img)
                return new OutputImage { UpdateSelectedImage = img };
            else
                return new OutputImage { Image = img };
        }

        [ImgMethod("Test", "Debug", "Point")]
        //[CustomForm(typeof(Debug))]
        public static OutputImage Debug(IImage image)
        {
            Point point = BaseMethods.GetCoord(image);
            MessageBox.Show(point.ToString());
            return null;
        }

        /// <summary>
        /// Открытие формы
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [ImgMethod("Test", "Формы", "FloodFill (с координатами)")]
        public static OutputImage TestCoord(IImage image)
        {
            SelectCoord form = new SelectCoord(image);
            //Можно двумя способами
            //form.ShowForm();
            //return null;
            return new OutputImage { ImageForm = form };
        }

        public static OutputImage FloodFill(IImage image, Point point, int t1, int t2, int t3)
        {
            dynamic input = image;
            var res = (IImage)image.Clone();
            Mat outputMask = new Mat(input.Height + 2, input.Width + 2, DepthType.Cv8U, 1);
            CvInvoke.FloodFill(res, outputMask, point, new MCvScalar(t1), out _, new MCvScalar(t2), new MCvScalar(t3));
            return new OutputImage { Image = res, Name = "FloodFill" };
        }

        [MethodName("Проверка прогресс бара 1")]
        [ImgMethod("Test", "Прогресс", "Без формы")]
        [ImgCanBeDisposedOrNull]

        public static OutputImage ProgressTest(InputImage inputImage)
        {
            //MessageBox.Show("Hello");
            bool cancel = false;
            inputImage.Progress.Run(1, 20, true);
            for (int i = 0; i < 20; i++)
            {
                if (inputImage.Progress.Cancel)
                {
                    cancel = true;
                    break;
                }
                Thread.Sleep(1000);
                inputImage.Progress.PerformStep();
            }
            inputImage.Progress.Finish(cancel);
            return new OutputImage();
        }
        [MethodName("Проверка прогресс бара 2")]
        [ImgMethod("Test", "Прогресс", "Параметризированная форма")]
        //[AutoForm(1, typeof(int), "Итераций")]
        //[AutoForm(2, typeof(int), "Перерывы между итерациями")]
        [ControlForm(1, typeof(NumericUpDown), "Value", "Итераций")]
        [ControlProperty(1, "Maximum", "5000")]
        [ControlForm(2, typeof(NumericUpDown), "Value", "Перерывы между итерациями")]
        [ControlProperty(2, "Maximum", "5000")]
        [ControlForm(3, typeof(CheckBox), "Checked", null)]
        [ControlProperty(3, "Text", "Копировать изображение")]
        [ImgCanBeDisposedOrNull]
        public static OutputImage ProgressTest(InputImage inputImage, int iteration, int timeIter, bool copy)
        {
            inputImage.Progress.Run(1, iteration, true);
            bool cancel = false;
            for (int i = 0; i < iteration; i++)
            {
                if (inputImage.Progress.Cancel)
                {
                    cancel = true;
                    break;
                }
                Thread.Sleep(timeIter);
                inputImage.Progress.PerformStep();
            }
            inputImage.Progress.Finish(cancel);
            return copy ? new OutputImage { Image = inputImage.Image } : new OutputImage();
        }
    }
}

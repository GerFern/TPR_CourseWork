using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary;
using Emgu.CV;
using Emgu.CV.Structure;

namespace SlepovLibrary
{
    [BaseLibrary.ImgClass("Слепов А.Ю.")]//Тут можно указать свое имя, а также пометить, что в этом классе есть методы для обработки изображений
    public static class Class1
    {
        [ImgMethod("Коррекция", "Контраст")]
        public static OutputImage Contrast(IImage input)
        {
            Form1 form = new Form1(input);
            return new OutputImage { ImageForm = form };
        }

        [ImgMethod("Коррекция", "Контраст [параметры]")]
        [AutoForm(1, typeof(double), "GammaCorrect")]
        public static OutputImage Contrast(IImage input, double gamma)
        {
            //Form1 form = new Form1(input);
            dynamic img = input.Clone();
            img._EqualizeHist();
            img._GammaCorrect(gamma);
            return new OutputImage { Name = "Констраст", Image = img };
        }

        [ImgMethod("Коррекция", "Контраст (новый)")]
        [CustomForm(typeof(EqualizeHistForm))]
        public static OutputImage ContrastNew(InputImage input)
        {
            return new OutputImage { };
        }

        [ImgMethod("Коррекция", "Двойное сглаживание")]
        public static OutputImage QWERTY(IImage input)
        {
            Form2 form = new Form2((IImage)input.Clone());
            return new OutputImage { ImageForm = form };
        }

        [ImgMethod("Коррекция", "Двойное сглаживание [параметры]")]
        [AutoForm(1, typeof(int), "Diametr")]
        [AutoForm(2, typeof(int), "SigmaColor")]
        [AutoForm(3, typeof(int), "SigmaSpace")]
        public static OutputImage QWERTYP(IImage input, int t1, double t2, double t3)
        {
            var dest = (IImage)input.Clone();
            CvInvoke.BilateralFilter(input, dest, t1, t2, t3);
            return new OutputImage { Name = "Двойное сглаживание", Image = dest };
        }

        [ImgMethod("Фильтр", "Гауссовое размытие")]
        
        public static OutputImage GausForm(InputImage image)
        {
            var src = image.CreateConverted<Gray, byte>();
            var dst = new Image<Gray, byte>(src.Size);
            CvInvoke.EqualizeHist(src, dst);
            return new OutputImage { Name = "Гауссовое размытие", Image = dst };
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Windows.Forms;

namespace X_rayLib
{
    [ImgClass("Патрикеев")]
    public static class XRayExpl
    {
        [ImgMethod("Патрикеев", "Эквализация гистограммы")]
        public static OutputImage Equalizing(InputImage image)
        {
            var resultImage = GetEqualizingImage(image.Image);

            return GetResult("Эквализация гистограммы", resultImage);
        }

        [ImgMethod("Патрикеев", "Пространственная фильтрация фильтром Лапласа")]
        public static OutputImage Laplace(InputImage image)
        {
            var resultImage = GetLaplaceImage(image.Image);

            return GetResult("Пространственная фильтрация фильтром Лапласа", resultImage);
        }

        [ImgMethod("Патрикеев", "Рассчитать качество изображения")]
        public static OutputImage Calculation(InputImage image)
        {
            return GetResult("Качество  исходного изображения", image.CreateConverted<Gray, byte>());
        }

        private static OutputImage GetResult(string name, IImage image)
        {
            float Q = GetCalculation(image);
            OutputImage result = new OutputImage
            {
                Image = image,
                Info = $"Качество изображения: {Q} 'больше = лучше'",
                Name = $"{name}({Q})"
            };

            return result;
        }



        private static Image<Gray, byte> GetEqualizingImage(IImage image)
        {
            var result = InputImage.Convert<Gray, byte>(image);

            result._EqualizeHist();

            return result;
        }

        private static Image<Gray, float> GetLaplaceImage(IImage image)
        {
            var t = InputImage.Convert<Gray, byte>(image);
            Image<Gray, float> result = t.Laplace(5);
            t.Dispose();
            return result;
        }

        private static float GetCalculation(dynamic image)
        {
            byte[] imageBytes = image.Bytes;

            // Нормирующий коэффициент
            const int K = 100;

            // Среднеарифметическое значение яркостей
            float LQ = (float)image.GetAverage().Intensity;

            // Резкость изображения

            // Максимальная яркость
            const int MAX = 255;

            float Q = K * LQ;

            //float LQ = image.
            return Q;
        }

        
    }





    /*private static Image<Gray, byte> GetLaplaceImage(Image<Bgr, byte> image)
        {
            // Получаем байты изображения
            byte[] imageBytes = image.Bytes;
            byte[] resultBytes = imageBytes;

            // Устанавливаем границы
            byte startPointer = imageBytes[0];
            byte endPointer = imageBytes[imageBytes.Count()];

            // Задаем маску Лапласа
            sbyte[] mask = { -1, -1, -1, -1, 8, -1, -1, -1, -1 };

            // Определение нужного байта
            /*int GetPosition(int x, int y, int start)
            {
                if(start + x * 2 + y * 2 < 0)
                {

                }
            }
            

            while(startPointer <= endPointer)
            {
                // Рассчет первого пиксела
                sbyte pix1 = 
            }



            var result = new Image<Gray, byte>()
            
            return result;
        }*/
}

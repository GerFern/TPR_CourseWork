using BaseLibrary;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing; 

namespace myLab
{
    [ImgClass("Абакумов А.В.")]//Тут можно указать свое имя, а также пометить, что в этом классе есть методы для обработки изображений
    public static class Class1
    {
        /// <summary>
        /// Преобразование в оттенки серого
        /// </summary>
        /// <param name="image">Оригинальное изображение</param>
        /// <returns>Обработаное изображение</returns>
        [ImgMethod("Сегментация", "Водораздел(отсечение фона)")]//Указывается иерархия вкладок в меню программы
        public static OutputImage TestGray(Image<Bgr, byte> image)
        {
            IImage backup = image.Clone();
            Image<Gray, byte> grayFrame = new Image<Gray, byte>(backup.Bitmap);
            CvInvoke.Threshold(grayFrame, grayFrame, 40, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

            IImage bw = new Image<Gray, float>(grayFrame.Size);
            CvInvoke.DistanceTransform(grayFrame, bw, null, Emgu.CV.CvEnum.DistType.L2, 3);

            CvInvoke.Normalize(bw, bw, 0, 255.0f, Emgu.CV.CvEnum.NormType.MinMax);
            CvInvoke.Threshold(bw, bw, 122, 255, Emgu.CV.CvEnum.ThresholdType.Binary);

            Mat hierarchy = new Mat();//Можно null
            Mat dist = new Mat();
            (bw as Image<Gray, float>).Mat.ConvertTo(dist, Emgu.CV.CvEnum.DepthType.Cv8U);
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();

            CvInvoke.FindContours(grayFrame, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
            int ncomp = contours.Size;

            Mat markers = Mat.Zeros(bw.Size.Height, bw.Size.Width, Emgu.CV.CvEnum.DepthType.Cv32S, 1);
            for (int i = 0; i < ncomp; i++)
                CvInvoke.DrawContours(markers, contours, i, new MCvScalar(i + 1), -1);
            MCvScalar nvc = new MCvScalar(255, 255, 255);//1,1,1
            CvInvoke.Circle(markers, new Point(5, 5), 3, nvc, -1);

            CvInvoke.Watershed(backup, markers);

            List<Color> colors = new List<Color>();

            Random rand = new Random();
            for (int i = 0; i < ncomp; i++)
            {
                int b = rand.Next(256);
                int g = rand.Next(256);
                int r = rand.Next(256);
                colors.Add(Color.FromArgb(r, g, b));
            }
            // Create the result image
            Image<Bgr, byte> dst = new Image<Bgr, byte>(markers.Size);
            dynamic tttt = backup;
            for (int i = 0; i < ncomp; i++)
            {
                //dst.Draw(contours[i].ToArray(), new Bgr(colors[i]), 9);
                tttt.Draw(contours[i].ToArray(), new Bgr(colors[i]));
            }
            return new OutputImage { Image = tttt };
        }

        /// <summary>
        /// Открытие формы
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [ImgMethod("Сегментация", "Сегментация с настройками")]//Указывается иерархия вкладок в меню программы
       // [CustomForm(typeof(Segment))]//Своя форма для метода
        public static OutputImage segmentFunction(IImage image)
        {
           myLab.Segment form = new Segment(image);
            //Можно двумя способами
            form.ShowDialog();
            //return null;
            // return new OutputImage { ImageForm = form };
            return new OutputImage { Image = null};
        }

    }
}

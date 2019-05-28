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

namespace OzhPakKu
{
    [ImgClass("Ожегина_Пакина_Кузнецов")]
    public static class OPK_Class
    {
        public static Image<Bgr, byte> image_orig = null;
        public static byte[,] Iimage_matr; // матрица полутонового изображения
        public static double[,] Iobr_image; // матрица обработанного изображения
        public static int Iw_b, Ih_b;
        [ImgMethod("ОПК.Фильтрация", "Фильтр Габора")]  //писать перед методом
        public static OutputImage OPK_Method(Image<Bgr, byte> image)   ////очистка фона
        {
            dynamic img = image;
            Image<Gray, float> grayImage = image.Convert<Gray, float>(); 
            double psi = 0;
            double gamma = 1;
            double theta = Math.PI / 5;
            double sigma = 3;
            double lambda = 6;
            int width = 3;

            GaborKernel kernel = new GaborKernel(width, lambda, theta, psi, sigma, gamma);
            Image<Gray, float> trans = Convolution(grayImage, kernel);
            return new OutputImage
            {
                Image = trans,
                Info = "Применен фильтр Габора"
            };
        }
        public class GaborKernel
        {
            public int width;
            public Matrix<float> real;
            public Matrix<float> imaginary;
            public GaborKernel(int _width, double lambda, double theta, double psi, double sigma, double gamma) // constructor
            {
                width = _width;
                imaginary = new Matrix<float>(width, width);
                real = new Matrix<float>(width, width);
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int x = i - width / 2;
                        int y = j - width / 2;

                        double x_prime = x * Math.Cos(theta) + y * Math.Sin(theta);  //xcos(~O)+ysin(~O)
                        double y_prime = -x * Math.Sin(theta) + y * Math.Cos(theta);//-xsin(~O)+ycos(~O)

                        double a = Math.Exp(-(x_prime * x_prime + gamma * gamma * y_prime * y_prime) / (2 * sigma * sigma));
                        //exp(-x`^2+u^2+y^2)
                        // ------------
                        //     2Q^2
                        double re = Math.Cos(2 * Math.PI * x_prime / lambda + psi); //cos(2pix`/lam +psi)
                        double im = Math.Sin(2 * Math.PI * x_prime / lambda + psi); //sin(2pix`/lam +psi)

                        double real_part = a * re;
                        double imaginary_part = a * im;

                        real.Data[i, j] = (float)real_part;

                        imaginary.Data[i, j] = (float)imaginary_part;

                    }
                }
                real.Save("rl.bmp"); imaginary.Save("im.bmp");
            }
        }
            public static Image<Gray, float> Convolution(Image<Gray, float> src, GaborKernel kernel)
            {
                Point center = new Point(kernel.width / 2 + 1, kernel.width / 2 + 1);
                ConvolutionKernelF kernel_f;

                kernel_f = new ConvolutionKernelF(kernel.real, center);
                Image<Gray, float> temp1 = src.Convolution(kernel_f);

                kernel_f = new ConvolutionKernelF(kernel.imaginary, center);
                Image<Gray, float> temp2 = src.Convolution(kernel_f);

                temp1 = temp1.Pow(2);
                temp2 = temp2.Pow(2);
                temp1 = temp1.Add(temp2);
                return temp1.Pow(0.5);
            }
            [ImgMethod("ОПК.Фильтрация", "Фильтр Собеля")]  //писать перед методом
        public static OutputImage MYRmen_Method2(Image<Bgr, byte> image)   ////очистка фона
        {
            dynamic img = image;
            img.Save("obr.bmp");
            Bitmap obr = new Bitmap("obr.bmp");
            //Image<Gray, byte> grayImage = img.Convert<Gray, byte>();
            //Image<Gray, byte> SMimg = grayImage.SmoothMedian(3);
            //img = SMimg.Sobel(0,0,3) ;
            //img = img.Sobel(0,0,3);
            Bitmap ish_bitmap = obr;
            Iw_b = ish_bitmap.Width;  //Ширина изображения
            Ih_b = ish_bitmap.Height; //Высота изображения
            Iimage_matr = new byte[Iw_b, Ih_b];  //матрица изображения 
            Iobr_image = new double[Iw_b, Ih_b];  //матрица изображения 
            IMatrColor(ish_bitmap);
            int n = 3;
            byte[,] n_bit = new byte[Iw_b, Ih_b];
            //медианная фильтрация для полутоновых изображений
            byte[] matr_3 = new byte[n * n]; int k2 = 0;
            for (int i = 0; i < Ih_b; i++)
            {  for (int j = 0; j < Iw_b; j++)
                {  n_bit[j, i] = Iimage_matr[j, i];     }
            }
            for (int i = n / 2; i < Ih_b - n / 2; i++)
            {  for (int j = n / 2; j < Iw_b - n / 2; j++)
                {   for (int k = i - n / 2; k <= i + n / 2; k++)
                    {   for (int l = j - n / 2; l <= j + n / 2; l++)
                        {  matr_3[k2++] = Iimage_matr[l, k];   }
                    }
                    matr_3[4] = (byte)Math.Sqrt(Math.Pow(((matr_3[6] + 2 * matr_3[7] + matr_3[8]) - (matr_3[0] + 2 * matr_3[1] + matr_3[2])), 2)
                        + Math.Pow(((matr_3[2] + 2 * matr_3[5] + matr_3[8]) - (matr_3[0] + 2 * matr_3[3] + matr_3[6])), 2));
                    k2 = 0;
                    if ((matr_3[4] > 30) && (matr_3[4] < 155))
                        matr_3[4] += 100;
                    n_bit[j, i] = matr_3[4];
                }
            }
            obr = new Bitmap(Iw_b, Ih_b);
            for (int i = 1; i < Ih_b - 1; i++)
            {  for (int j = 1; j < Iw_b - 1; j++)
                {   Color c = Color.FromArgb(n_bit[j, i], n_bit[j, i], n_bit[j, i]);
                    obr.SetPixel(j, i, c);//заносим преобразорванный цвет в текущую точку
                }
            }
            obr.Save("obr2.bmp");

            Image<Bgr, byte> _image = new Image<Bgr, byte>("obr2.bmp");
            image.Data = _image.Data;
            return new OutputImage
            {
                Image = new Image<Bgr, Byte>(obr) ,
                Info = "Применен алгоритм Собеля"
            };
        }
        public static void IMatrColor(Bitmap ish_bitmap)
        { //Заполнение массива яркостями каждой точки
            for (int x = 0; x < Iw_b; x++)
            {
                for (int y = 0; y < Ih_b; y++)
                {
                    System.Drawing.Color c = ish_bitmap.GetPixel(x, y);//получаем цвет указанной точки
                    int r = Convert.ToInt32(c.R);
                    int b = Convert.ToInt32(c.B);
                    int g = Convert.ToInt32(c.G);
                    int brit = Convert.ToInt32(0.299 * r + 0.587 * g + 0.114 * b); //Перевод из RGB в полутон
                    Iimage_matr[x, y] = Convert.ToByte(brit);
                }
            }
        }
      
        [ImgMethod("ОПК.Фильтрация", "Фильтр Превитта")]  //писать перед методом
        public static OutputImage MYRmen_Method3(Image<Bgr, byte> image)   ////очистка фона
        {
            dynamic img = image;
            img.Save("obr.bmp");
            Bitmap obr = new Bitmap("obr.bmp");
            Bitmap ish_bitmap = obr;
            //obr = new Bitmap((Bitmap)img);
            Iw_b = ish_bitmap.Width;  //Ширина изображения
            Ih_b = ish_bitmap.Height; //Высота изображения
            Iimage_matr = new byte[Iw_b, Ih_b];  //матрица изображения 
            Iobr_image = new double[Iw_b, Ih_b];  //матрица изображения 
            IMatrColor(ish_bitmap);

            int n = 3;
            byte[,] n_bit = new byte[Iw_b, Ih_b];
            for (int x = 0; x < Iw_b; x++)
            {   for (int y = 0; y < Ih_b; y++)
                {   System.Drawing.Color c = ish_bitmap.GetPixel(x, y);//получаем цвет указанной точки
                    int r = Convert.ToInt32(c.R);
                    int b = Convert.ToInt32(c.B);
                    int g = Convert.ToInt32(c.G);
                    int brit = Convert.ToInt32(0.299 * r + 0.587 * g + 0.114 * b); //Перевод из RGB в полутон       
                    Iimage_matr[x, y] = Convert.ToByte(brit);
                }
            }
            //медианная фильтрация для полутоновых изображений
            byte[] matr_3 = new byte[n * n]; int k2 = 0;
            for (int i = 0; i < Ih_b; i++)
            {   for (int j = 0; j < Iw_b; j++)
                {   n_bit[j, i] = Iimage_matr[j, i];      }
            }
            for (int i = n / 2; i < Ih_b - n / 2; i++)
            {   for (int j = n / 2; j < Iw_b - n / 2; j++)
                {   for (int k = i - n / 2; k <= i + n / 2; k++)
                    {   for (int l = j - n / 2; l <= j + n / 2; l++)
                        {     matr_3[k2++] = Iimage_matr[l, k];        }
                    }
                    matr_3[4] = (byte)Math.Sqrt(Math.Pow(((matr_3[6] + matr_3[7] + matr_3[8]) - (matr_3[0] + matr_3[1] + matr_3[2])), 2)
                        + Math.Pow(((matr_3[2] + matr_3[5] + matr_3[8]) - (matr_3[0] + matr_3[3] + matr_3[6])), 2));
                    k2 = 0;
                    if ((matr_3[4] > 30) && (matr_3[4] < 155))
                        matr_3[4] += 100;
                    n_bit[j, i] = matr_3[4];
                }
            }
            obr = new Bitmap(Iw_b, Ih_b);
            for (int i = 1; i < Ih_b - 1; i++)
            {   for (int j = 1; j < Iw_b - 1; j++)
                {
                    Color c = Color.FromArgb(n_bit[j, i], n_bit[j, i], n_bit[j, i]);
                    obr.SetPixel(j, i, c);//заносим преобразорванный цвет в текущую точку
                }
            }

            obr.Save("obr2.bmp");
            
            Image<Bgr, byte> _image = new Image<Bgr, byte>("obr2.bmp");
            //image.Data = _image.Data;
            return new OutputImage
            {
                Image = _image,
                Info = "Применен фильтр Превитта"
            };
        }

        //[ImgMethod("Методы МУРовцев", "form")]
        //public static OutputImage Start_form(Image<Bgr, byte> image)
        //{
        //    try
        //    {
        //        Bitmap img = image.Bitmap;
        //        Bitmap img_orig = image_orig.Bitmap;
        //        //Form1 form = new Form1(img_orig, img);
        //        //form.Show();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show("примените к изображению очистку фона и фурье преобразование");
        //    }
        //    return new OutputImage
        //    {
        //        Image = image,
        //        Info = ""
        //    };
        //}

       
    }
   
}

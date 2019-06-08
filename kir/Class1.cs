using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BaseLibrary;
using Emgu.CV;
using Emgu.CV.Structure;

namespace kir
{
    [ImgClass("Копейкин К.")]
    public class Class1
    {

        public int GetValue(Mat mat, int row, int col)
        {
            var value = new int[1];
            Marshal.Copy(mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, value, 0, 1);
            return value[0];
        }

        public static void SetValue(Mat mat, int row, int col, int value)
        {
            var target = new[] { value };
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 1);
            //mat.Data.SetValue(value, row, col);
        }
        public static void SetValue(Mat mat, int row, int col, int v1, int v2, int v3)
        {
            var target = new[] { v1, v2, v3 };
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 3);
            // mat.Data.SetValue(new int[] { v1, v2, v3 }, row, col);
        }
        static int n = 0;
        static int N = 0;
        static int[] m;
        static int y = 0;


        static byte convert(byte x)
        {
            y = y + (x - m[n]);
            m[n] = x;
            n = (n + 1) % N;
            return (byte)(y / N);
        }


        [ImgMethod("Улучшение качества", "Метод ближайшиъх вершин")]//Указывается иерархия вкладок в меню программы
        [AutoForm(1, typeof(int), "Размер буфера")]
        public static OutputImage cleser(InputImage input, int N)
        {
            Image<Gray, byte> img = (input.Image.Clone() as Image<Bgr, byte>).Convert<Gray, byte>();

            Image<Gray, byte> res = new Image<Gray, byte>(img.Size);
            m = new int[N];
            Class1.N = N;
            for (int i = 0; i < res.Size.Height; i++)
                for (int j = 0; j < res.Size.Width - 1; j++)
                {
                    res.Data[i, j, 0] = convert(img.Data[i, j, 0]);//отмечаем на изображении области
                }

            return new OutputImage { Image = res };
        }

        [ImgMethod("Улучшение качества", "Метод локальных констант")]//Указывается иерархия вкладок в меню программы
        [AutoForm(1, typeof(int), "Размер маски (>1 не кратно 2)")]
        [AutoForm(2, typeof(int), "Режим констран(1-4)")]
        [AutoForm(3, typeof(float), "Контсанта(для реимjd №3 и №4)")]
        public static OutputImage localConst(InputImage input, int N, int mode, float k)
        {
            Image<Gray, byte> img = (input.Image.Clone() as Image<Bgr, byte>).Convert<Gray, byte>();
            Image<Gray, byte> res = new Image<Gray, byte>(img.Size);

            if (N < 1 || N % 2 == 0)
            {
                BaseMethods.WriteLog("Неправильно задан размер маски");
                return new OutputImage { Image = img };
            }
            int hN = N / 2;
            for (int i = hN; i < img.Size.Height - hN; i++)
                for (int j = hN; j < img.Size.Width - hN; j++)
                {
                    int sum = 0;
                    byte Amin = 255;
                    byte Amax = 0;
                    float avg;
                    byte a = img.Data[i, j, 0];

                    for (int i1 = 0; i1 < N; i1++)
                        for (int j1 = 0; j1 < N; j1++)
                        {
                            byte cur = img.Data[i + i1 - hN, j + j1 - hN, 0];
                            if (cur > Amax)
                                Amax = cur;
                            if (cur < Amin)
                                Amin = cur;
                            sum += cur;
                        }

                    avg = sum / (float)(N * N);
                    float Ymin = Amin / 255f;
                    float Ymax = Amax / 255f;
                    float y = a / 255f;

                    int newV;
                    double y5 = Math.Pow(y, 5);

                    switch (mode)
                    {
                        case 1:
                            newV = (int)(Amin + (Amax - Amin) * Math.Pow((y - Ymax) / (Ymax - Ymin), 5));
                            break;
                        case 2:
                            newV = (int)(Amin * y5 + Amax * (1 - y5));
                            break;
                        case 3:
                            float ya2 = (y - a) * k;
                            ya2 *= ya2;
                            newV = a + (int)((Amin - Amax) * Math.Pow(1 - Math.Exp(-ya2 / (2 * 0.14 * 0.14)), 5));
                            break;
                        case 4:
                            newV = (int)(Amin + k * avg * y5);
                            break;
                        default:
                            newV = a;
                            break;
                    }
                    if (newV > 255)
                        newV = 255;
                    else if (newV < 0)
                        newV = 0;

                    res.Data[i, j, 0] = (byte)newV;
                }
            return new OutputImage { Image = res };
        }

        [ImgMethod("Улучшение качества", "Матричное преобразование", "Фильтр повышения резкости")]
        public static OutputImage pov(InputImage input)
        {
            double[,] matr = new double[,]{
                                         {-1,-1,-1},
                                         {-1,16,-1},
                                         {-1,-1,-1}
                                         };
            return mart(input, matr, 3, 8);
        }


        [ImgMethod("Улучшение качества", "Матричное преобразование", "Фильтр высоких частот")]
        public static OutputImage High(InputImage input)
        {
            double[,] matr = new double[,]{
                                         {-1,-1,-1},
                                         {-1, 9,-1},
                                         {-1,-1,-1}
                                         };
            return mart(input, matr, 3, 1);

        }

        [ImgMethod("Улучшение качества", "Матричное преобразование", "Фильтр четкости")]
        public static OutputImage chet(InputImage input)
        {
            double[,] matr = new double[,]{
                                         {-0.5,-0.75,-0.5},
                                         {-0.75, 6 ,-0.75},
                                         {-0.5,-0.75, -0.5}
                                         };
            return mart(input, matr, 3, 1);
        }

        public static OutputImage mart(InputImage input, double[,] matr, int n, int u=1)
        {
            Image<Gray, byte> img = (input.Image.Clone() as Image<Bgr, byte>).Convert<Gray, byte>();
            Image<Gray, byte> res = new Image<Gray, byte>(img.Size);
            N = n;
            int hN = N / 2;
            for (int i = hN; i < img.Size.Height - hN; i++)
                for (int j = hN; j < img.Size.Width - hN; j++)
                { 
                    double a = 0;

                    for (int i1 = 0; i1 < N; i1++)
                        for (int j1 = 0; j1 < N; j1++)
                        {
                            byte cur = img.Data[i + i1 - hN, j + j1 - hN, 0];
                            a += cur * matr[i1, j1];
                        }

                    int newV = (int)(a / u);
                    if (newV > 255)
                        newV = 255;
                    else if (newV < 0)
                        newV = 0; 

                    res.Data[i, j, 0] = (byte)newV;
                }
            return new OutputImage { Image = res };
        }
    }
}
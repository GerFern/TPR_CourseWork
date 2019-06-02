using BaseLibrary;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Emgu.CV.CvInvoke;
using static System.Math;

namespace Temp
{
    [ImgClass("___")]
    public static class AAAA
    {
        private const string V = "Wavelet";
        public static IImage ImgCompare1, ImgCompare2;

        public enum fType
        {
            [System.ComponentModel.Description("Без фильтра")]
            NONE = 0,   // Без фильтра
            [System.ComponentModel.Description("Жесткий")]
            HARD = 1,   // Жесткий
            [System.ComponentModel.Description("Мягкий")]
            SOFT = 2,   // Мягкий
            [System.ComponentModel.Description("Фильтр Гаррота")]
            GARROT = 3  // Фильтр Гаррота
        }

        /// <summary>
        /// Определяем знак числа
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        static float Sgn(float x)
        {
            float res = 0;
            if (x == 0)
            {
                res = 0;
            }
            if (x > 0)
            {
                res = 1;
            }
            if (x < 0)
            {
                res = -1;
            }
            return res;
        }
        /// <summary>
        /// Мягкое ослабление коэффициентов
        /// </summary>
        /// <param name="d"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        static float soft_shrink(float d, float T)
        {
            float res;
            if (Abs(d) > T)
            {
                res = Sgn(d) * (Abs(d) - T);
            }
            else
            {
                res = 0;
            }

            return res;
        }
        /// <summary>
        /// Жесткое ослабление коэффициентов
        /// </summary>
        /// <param name="d"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        static float hard_shrink(float d, float T)
        {
            float res;
            if (Abs(d) > T)
            {
                res = d;
            }
            else
            {
                res = 0;
            }

            return res;
        }
        /// <summary>
        /// Ослабление коэффициентов по Гарроту
        /// </summary>
        /// <param name="d"></param>
        /// <param name="T"></param>
        /// <returns></returns>
        static float Garrot_shrink(float d, float T)
        {
            float res;
            if (Abs(d) > T)
            {
                res = d - ((T * T) / d);
            }
            else
            {
                res = 0;
            }

            return res;
        }

        [ImgMethod(V, "SetCompare1")]
        public static OutputImage SetCompare1(IImage image)
        {
            ImgCompare1 = image;
            return null;
        }

        [ImgMethod(V, "SetCompare2")]
        public static OutputImage SetCompare2(IImage image)
        {
            ImgCompare2 = image;
            return null;
        }

        [ImgCanBeDisposedOrNull]
        [ImgMethod(V, "GetPSNR")]
        public static OutputImage GetPSNR(IImage image)
        {
            IImage img1 = InputImage.ConverctDepth<byte>(ImgCompare1);
            IImage img2 = InputImage.ConverctDepth<byte>(ImgCompare2);
            MessageBox.Show(PSNR(img1, img2).ToString(), "PSNR");
            img1.Dispose();
            img2.Dispose();
            return null;
        }

        [ImgMethod(V, "GetGrayFloat")]
        public static OutputImage GetGrayFloat(InputImage inputImage)
        {
            return new OutputImage { Image = inputImage.CreateConverted<Gray, Byte>() };
        }

        [ImgMethod(V, "Normalization")]
        public static OutputImage Normalization(InputImage inputImage)
        {
            double m = 0, M = 0;
            Point pm = Point.Empty, pM = Point.Empty;
            dynamic dst = inputImage.DynamicImage.Clone();
            MinMaxLoc(dst, ref m, ref M, ref pm, ref pM);
            //Matrix<float> mDst = new Matrix<float>(dst.Rows, dst.Cols, dst.Ptr);
            if ((M - m) > 0) { dst = dst * (255.0 / (M - m)) - m / (M - m); }
            return new OutputImage { Image = dst };
        }


        [ImgMethod(V, "HaarWavelet")]
        [ControlForm(1, typeof(NumericUpDown), "Value", "NIter")]
        [ControlForm(2, typeof(NumericUpDown), "Value", "mc")]
        [ControlForm(3, typeof(NumericUpDown), "Value", "mh")]
        [ControlForm(4, typeof(NumericUpDown), "Value", "mv")]
        [ControlForm(5, typeof(NumericUpDown), "Value", "md")]
        [ControlProperty(2, "DecimalPlaces", "5")]
        [ControlProperty(3, "DecimalPlaces", "5")]
        [ControlProperty(4, "DecimalPlaces", "5")]
        [ControlProperty(5, "DecimalPlaces", "5")]
        public static OutputImage HaarWavelet(InputImage inputImage, int NIter, float mc, float mh, float mv, float md)
        {
            var src = inputImage.CreateConverted<Gray, float>();
            var dst = new Image<Gray, float>(src.Size);
            CvHaarWavelet(src, dst, NIter, mc, mh, mv, md);
            src.Dispose();
            return new OutputImage { Image = dst };
        }

        [ImgMethod(V, "InvHaarWavelet")]
        [ControlForm(1, typeof(NumericUpDown), "Value", "NIter")]
        [ControlForm(2, typeof(EnumComboBox<fType>), "EnumValue", "Фильтр")]
        [ControlForm(3, typeof(NumericUpDown), "Value", "Интенсивность фильтра")]
        [ControlProperty(3, "Maximum", "100000")]

        public static OutputImage InvHaarWavelet(InputImage inputImage, int NIter, fType ft, int t)
        {
            var src = inputImage.CreateConverted<Gray, float>();
            var dst = new Image<Gray, float>(src.Size);
            CvInvHaarWavelet(src, dst, NIter, ft, t);
            src.Dispose();
            return new OutputImage { Image = dst };
        }


        [ImgMethod(V, "HaarWaveletByte")]
        [ControlForm(1, typeof(NumericUpDown), "Value", "NIter")]
        public static OutputImage HaarWaveletByte(InputImage inputImage, int NIter)
        {
            var src = inputImage.CreateConverted<Gray, byte>();
            var dst = new Image<Gray, byte>(src.Size);
            CvHaarWaveletByte(src, dst, NIter);
            src.Dispose();
            return new OutputImage { Image = dst };
        }

        [ImgMethod(V, "InvHaarWaveletByte")]
        [ControlForm(1, typeof(NumericUpDown), "Value", "NIter")]
        [ControlForm(2, typeof(EnumComboBox<fType>), "EnumValue", "Фильтр")]
        [ControlForm(3, typeof(NumericUpDown), "Value", "Интенсивность фильтра")]
        [ControlProperty(3, "Maximum", "100000")]

        public static OutputImage InvHaarWaveletByte(InputImage inputImage, int NIter, fType ft, int t)
        {
            var src = inputImage.CreateConverted<Gray, byte>();
            var dst = new Image<Gray, byte>(src.Size);
            CvInvHaarWaveletByte(src, dst, NIter, ft, t);
            src.Dispose();
            return new OutputImage { Image = dst };
        }


        /// <summary>
        /// Вейвлет-преобразование
        /// </summary>
        /// <param name="src">Исходное изображение</param>
        /// <param name="dst">Назначаемое изображение</param>
        /// <param name="NIter">Количество итераций</param>
        static void CvHaarWavelet(Image<Gray, float> src, Image<Gray, float> dst, int NIter, float mc, float mh, float mv, float md)
        {
            float c, dh, dv, dd;
            int width = src.Cols;
            int height = src.Rows;
            float[,,] dataSrc = (float[,,])src.ManagedArray;
            float[,,] dataDst = (float[,,])dst.ManagedArray;
            //float[,] dataSrc = (float[,])src.Data; // Проверить!!!
            //float[,] dataDst = (float[,])dst.Data; // Проверить!!!
            for (int k = 0; k < NIter; k++)
            {
                for (int y = 0; y < (height >> (k + 1)); y++)
                {
                    for (int x = 0; x < (width >> (k + 1)); x++)
                    {
                        c = (dataSrc[2 * y, 2 * x, 0] + dataSrc[2 * y, 2 * x + 1, 0] + dataSrc[2 * y + 1, 2 * x, 0] + dataSrc[2 * y + 1, 2 * x + 1, 0]) * mc;
                        dataDst[y, x, 0] = c;

                        dh = (dataSrc[2 * y, 2 * x, 0] + dataSrc[2 * y + 1, 2 * x, 0] - dataSrc[2 * y, 2 * x + 1, 0] - dataSrc[2 * y + 1, 2 * x + 1, 0]) * mh;
                        dataDst[y, x + (width >> (k + 1)), 0] = dh;

                        dv = (dataSrc[2 * y, 2 * x, 0] + dataSrc[2 * y, 2 * x + 1, 0] - dataSrc[2 * y + 1, 2 * x, 0] - dataSrc[2 * y + 1, 2 * x + 1, 0]) * mv;
                        dataDst[y + (height >> (k + 1)), x, 0] = dv;

                        dd = (dataSrc[2 * y, 2 * x, 0] - dataSrc[2 * y, 2 * x + 1, 0] - dataSrc[2 * y + 1, 2 * x, 0] + dataSrc[2 * y + 1, 2 * x + 1, 0]) * md;
                        dataDst[y + (height >> (k + 1)), x + (width >> (k + 1)), 0] = dd;
                    }
                }
                //dst.Data = dataDst;
                //dataDst.CopyTo(dataSrc,0);
                dst.CopyTo(src);

            }
            double min = 0, max = 1;
            Point pmin = Point.Empty, pmax = Point.Empty;
            MinMaxLoc(src, ref min, ref max, ref pmin, ref pmax);
            //MessageBox.Show($"min {min} max {max} pmin {pmin} pmax {pmax}");
            //dst.Data = dataDst;
            //src.Data = dataSrc;
        }
        /// <summary>
        /// Обратное вейвлет-преобразование
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <param name="NIter">Количество итераций</param>
        /// <param name="SHRINKAGE_TYPE"></param>
        /// <param name="SHRINKAGE_T"></param>
        static void CvInvHaarWavelet(Image<Gray, float> src, Image<Gray, float> dst, int NIter, fType SHRINKAGE_TYPE = 0, float SHRINKAGE_T = 50)
        {
            float c, dh, dv, dd;
            //assert(src.type() == CV_32FC1);
            //assert(dst.type() == CV_32FC1);
            int width = src.Cols;
            int height = src.Rows;
            float[,,] dataSrc = src.Data; // Проверить!!!
            float[,,] dataDst = dst.Data; // Проверить!!!
            //--------------------------------
            // NIter - Количество итераций преобразования
            //--------------------------------
            for (int k = NIter; k > 0; k--)
            {
                for (int y = 0; y < (height >> k); y++)
                {
                    for (int x = 0; x < (width >> k); x++)
                    {
                        c = dataSrc[y, x, 0];
                        dh = dataSrc[y, x + (width >> k), 0];
                        dv = dataSrc[y + (height >> k), x, 0];
                        dd = dataSrc[y + (height >> k), x + (width >> k), 0];

                        // Ослабляем коэффициенты (shrinkage)
                        switch (SHRINKAGE_TYPE)
                        {
                            case fType.HARD:
                                dh = hard_shrink(dh, SHRINKAGE_T);
                                dv = hard_shrink(dv, SHRINKAGE_T);
                                dd = hard_shrink(dd, SHRINKAGE_T);
                                break;
                            case fType.SOFT:
                                dh = soft_shrink(dh, SHRINKAGE_T);
                                dv = soft_shrink(dv, SHRINKAGE_T);
                                dd = soft_shrink(dd, SHRINKAGE_T);
                                break;
                            case fType.GARROT:
                                dh = Garrot_shrink(dh, SHRINKAGE_T);
                                dv = Garrot_shrink(dv, SHRINKAGE_T);
                                dd = Garrot_shrink(dd, SHRINKAGE_T);
                                break;
                        }

                        //-------------------
                        dataDst[y * 2, x * 2, 0] = 0.5f * (c + dh + dv + dd);
                        dataDst[y * 2, x * 2 + 1, 0] = 0.5f * (c - dh + dv - dd);
                        dataDst[y * 2 + 1, x * 2, 0] = 0.5f * (c + dh - dv - dd);
                        dataDst[y * 2 + 1, x * 2 + 1, 0] = 0.5f * (c - dh - dv + dd);
                    }
                }
                Mat C = new Mat(src.Mat, new Rectangle(0, 0, width >> (k - 1), height >> (k - 1)));
                Mat D = new Mat(dst.Mat, new Rectangle(0, 0, width >> (k - 1), height >> (k - 1)));
                D.CopyTo(C);
            }
        }
        //--------------------------------
        //
        //--------------------------------
        //static int  process(VideoCapture capture)
        //{
        //    int n = 0;
        //    const int NIter = 4;
        //    string filename;
        //    string window_name = "video | q or esc to quit";
        //    //cout << "press space to save a picture. q or esc to quit" << endl;
        //    CvInvoke.NamedWindow(window_name, NamedWindowType.KeepRatio); //resizable window;
        //    Mat frame =
        //    capture.QueryFrame();
        //    //capture >> frame;

        //    Mat GrayFrame = new Mat(frame.Rows, frame.Cols, DepthType.Cv8U, 1);
        //    Mat Src = new Mat(frame.Rows, frame.Cols, DepthType.Cv32F, 1);
        //    Mat Dst = new Mat(frame.Rows, frame.Cols, DepthType.Cv32F, 1);
        //    Mat Temp = new Mat(frame.Rows, frame.Cols, DepthType.Cv32F, 1);
        //    Mat Filtered = new Mat(frame.Rows, frame.Cols, DepthType.Cv32F, 1);
        //    for (; ; )
        //    {
        //        //Dst = 0;
        //        //capture >> frame;
        //        frame = capture.QueryFrame();
        //        if (frame.IsEmpty) continue;
        //        CvInvoke.CvtColor(frame, GrayFrame, ColorConversion.Bgr2Gray);
        //        GrayFrame.ConvertTo(Src, DepthType.Cv32F);
        //        cvHaarWavelet(Src, Dst, NIter);

        //        Dst.CopyTo(Temp);

        //        cvInvHaarWavelet(Temp, Filtered, NIter, fType.GARROT, 30);

        //        CvInvoke.Imshow(window_name, frame);

        //        double M = 0, m = 0;
        //        Point pm = Point.Empty, pM = Point.Empty;
        //        //----------------------------------------------------
        //        // Приводим к диапазону 0-1, чтобы было видно картинки
        //        //----------------------------------------------------
        //        CvInvoke.MinMaxLoc(Dst, ref m, ref M, ref pm, ref pM);
        //        Matrix<float> mDst = new Matrix<float>(Dst.Rows, Dst.Cols, Dst.Ptr);
        //        if ((M - m) > 0) { mDst = mDst * (1.0 / (M - m)) - m / (M - m); }
        //        CvInvoke.Imshow("Coeff", mDst.Mat);

        //        CvInvoke.MinMaxLoc(Filtered, ref m, ref M, ref pm, ref pM);
        //        mDst = new Matrix<float>(Dst.Rows, Dst.Cols, Filtered.Ptr);
        //        if ((M - m) > 0) { mDst = mDst * (1.0 / (M - m)) - m / (M - m); }
        //        CvInvoke.Imshow("Filtered", mDst.Mat);

        //        char key = (char)CvInvoke.WaitKey(5);
        //        switch (key)
        //        {
        //            case 'q':
        //            case 'Q':
        //            case '\x0027': //escape key
        //                return 0;
        //            case ' ': //Save an image
        //                //sprintf(filename, "filename%.3d.jpg", n++);
        //                Imwrite($"image{n++}", frame);
        //                //cout << "Saved " << filename << endl;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    return 0;
        //}



        /// <summary>
        /// Вейвлет-преобразование
        /// </summary>
        /// <param name="src">Исходное изображение</param>
        /// <param name="dst">Назначаемое изображение</param>
        /// <param name="NIter">Количество итераций</param>
        static void CvHaarWaveletByte(Image<Gray, byte> src, Image<Gray, byte> dst, int NIter)
        {
            byte dh, dv, dd;
            byte c;
            //assert(src.type() == CV_32FC1);
            //assert(dst.type() == CV_32FC1);
            int width = src.Cols;
            int height = src.Rows;
            byte[,,] dataSrc = src.Data;
            byte[,,] dataDst = dst.Data;
            //float[,] dataSrc = (float[,])src.Data; // Проверить!!!
            //float[,] dataDst = (float[,])dst.Data; // Проверить!!!
            for (int k = 0; k < NIter; k++)
            {
                for (int y = 0; y < (height >> (k + 1)); y++)
                {
                    for (int x = 0; x < (width >> (k + 1)); x++)
                    {
                        c = (byte) ((dataSrc[2 * y, 2 * x, 0] + dataSrc[2 * y, 2 * x + 1, 0] + dataSrc[2 * y + 1, 2 * x, 0] + dataSrc[2 * y + 1, 2 * x + 1, 0]) * 0.5f) ;
                        dataDst[y, x, 0] = c;

                        dh = (byte) ((dataSrc[2 * y, 2 * x, 0] + dataSrc[2 * y + 1, 2 * x, 0] - dataSrc[2 * y, 2 * x + 1, 0] - dataSrc[2 * y + 1, 2 * x + 1, 0]) * 0.5f);
                        dataDst[y, x + (width >> (k + 1)), 0] = dh;

                        dv = (byte) ((dataSrc[2 * y, 2 * x, 0] + dataSrc[2 * y, 2 * x + 1, 0] - dataSrc[2 * y + 1, 2 * x, 0] - dataSrc[2 * y + 1, 2 * x + 1, 0]) * 0.5f);
                        dataDst[y + (height >> (k + 1)), x, 0] = dv;

                        dd = (byte) ((dataSrc[2 * y, 2 * x, 0] - dataSrc[2 * y, 2 * x + 1, 0] - dataSrc[2 * y + 1, 2 * x, 0] + dataSrc[2 * y + 1, 2 * x + 1, 0]) * 0.5f);
                        dataDst[y + (height >> (k + 1)), x + (width >> (k + 1)), 0] = dd;
                    }
                }
                //dst.Data = dataDst;
                //dataDst.CopyTo(dataSrc,0);
                dst.CopyTo(src);

            }
            //dst.Data = dataDst;
            //src.Data = dataSrc;
        }

        static void CvInvHaarWaveletByte(Image<Gray, byte> src, Image<Gray, byte> dst, int NIter, fType SHRINKAGE_TYPE = 0, float SHRINKAGE_T = 50)
        {
            float c, dh, dv, dd;
            //assert(src.type() == CV_32FC1);
            //assert(dst.type() == CV_32FC1);
            int width = src.Cols;
            int height = src.Rows;
            byte[,,] dataSrc = src.Data; // Проверить!!!
            byte[,,] dataDst = dst.Data; // Проверить!!!
            //--------------------------------
            // NIter - Количество итераций преобразования
            //--------------------------------
            for (int k = NIter; k > 0; k--)
            {
                for (int y = 0; y < (height >> k); y++)
                {
                    for (int x = 0; x < (width >> k); x++)
                    {
                        c = dataSrc[y, x, 0];
                        dh = dataSrc[y, x + (width >> k), 0];
                        dv = dataSrc[y + (height >> k), x, 0];
                        dd = dataSrc[y + (height >> k), x + (width >> k), 0];

                        // Ослабляем коэффициенты (shrinkage)
                        switch (SHRINKAGE_TYPE)
                        {
                            case fType.HARD:
                                dh = hard_shrink(dh, SHRINKAGE_T);
                                dv = hard_shrink(dv, SHRINKAGE_T);
                                dd = hard_shrink(dd, SHRINKAGE_T);
                                break;
                            case fType.SOFT:
                                dh = soft_shrink(dh, SHRINKAGE_T);
                                dv = soft_shrink(dv, SHRINKAGE_T);
                                dd = soft_shrink(dd, SHRINKAGE_T);
                                break;
                            case fType.GARROT:
                                dh = Garrot_shrink(dh, SHRINKAGE_T);
                                dv = Garrot_shrink(dv, SHRINKAGE_T);
                                dd = Garrot_shrink(dd, SHRINKAGE_T);
                                break;
                        }

                        //-------------------
                        dataDst[y * 2, x * 2, 0] = (byte)(0.5f * (c + dh + dv + dd));
                        dataDst[y * 2, x * 2 + 1, 0] = (byte)(0.5f * (c - dh + dv - dd));
                        dataDst[y * 2 + 1, x * 2, 0] = (byte)(0.5f * (c + dh - dv - dd));
                        dataDst[y * 2 + 1, x * 2 + 1, 0] = (byte)(0.5f * (c - dh - dv + dd));
                    }
                }
                Mat C = new Mat(src.Mat, new Rectangle(0, 0, width >> (k - 1), height >> (k - 1)));
                Mat D = new Mat(dst.Mat, new Rectangle(0, 0, width >> (k - 1), height >> (k - 1)));
                D.CopyTo(C);
            }
        }

        [ImgMethod(V, "Proc")]
        public static OutputImage Process(IImage img)
        {
            int n = 0;
            const int NIter = 4;
            string filename;
            string window_name = "video | q or esc to quit";
            //cout << "press space to save a picture. q or esc to quit" << endl;
            //CvInvoke.NamedWindow(window_name, NamedWindowType.KeepRatio); //resizable window;
            dynamic t = img;
            //Mat frame = t.Mat;
            //capture >> frame;
            //Image<Gray, float> gray = t.Convert<Gray, float>();
            //return new OutputImage { Image = gray };
            Image<Gray, float> src = t.Convert<Gray, float>();
            int width = src.Width;
            int height = src.Height;
            Image<Gray, float> dst = new Image<Gray, float>(width, height);
            Image<Gray, float> tmp = new Image<Gray, float>(width, height);
            Image<Gray, float> filtered = new Image<Gray, float>(width, height);

            //Mat GrayFrame = new Mat(frame.Rows, frame.Cols, DepthType.Cv8U, 1);
            //Mat Src = new Mat(frame.Rows, frame.Cols, DepthType.Cv32F, 1);
            //Mat Dst = new Mat(frame.Rows, frame.Cols, DepthType.Cv32F, 1);
            //Mat Temp = new Mat(frame.Rows, frame.Cols, DepthType.Cv32F, 1);
            //Mat Filtered = new Mat(frame.Rows, frame.Cols, DepthType.Cv32F, 1);
            //CvInvoke.CvtColor(frame, GrayFrame, ColorConversion.Bgr2Gray);
            //GrayFrame.ConvertTo(Src, DepthType.Cv32F);
            CvHaarWavelet(src, dst, NIter, 0.5f, 0.5f, 0.5f, 0.5f);

            dst.CopyTo(tmp);

            BaseMethods.LoadOutputImage(new OutputImage { Name = "tmp", Image = tmp });
            CvInvHaarWavelet(tmp, filtered, NIter, fType.GARROT, 30);
            BaseMethods.LoadOutputImage(new OutputImage { Name = "filtered", Image = filtered });

            //CvInvoke.Imshow(window_name, frame);

            double M = 0, m = 0;
            Point pm = Point.Empty, pM = Point.Empty;
            //----------------------------------------------------
            // Приводим к диапазону 0-1, чтобы было видно картинки
            //----------------------------------------------------
            CvInvoke.MinMaxLoc(dst, ref m, ref M, ref pm, ref pM);
            //Matrix<float> mDst = new Matrix<float>(dst.Rows, dst.Cols, dst.Ptr);
            if ((M - m) > 0) { dst = dst * (1.0 / (M - m)) - m / (M - m); }
            //CvInvoke.Imshow("Coeff", dst.Mat);
            BaseMethods.LoadOutputImage(new OutputImage { Name = "coeff", Image = dst });

            CvInvoke.MinMaxLoc(filtered, ref m, ref M, ref pm, ref pM);
            //mDst = new Matrix<float>(Dst.Rows, Dst.Cols, Filtered.Ptr);
            if ((M - m) > 0) { filtered = filtered * (1.0 / (M - m)) - m / (M - m); }
            //CvInvoke.Imshow("Filtered", dst.Mat);

            return new OutputImage { Name = "filteredII", Image = dst };
            //char key = (char)CvInvoke.WaitKey(5);
            //switch (key)
            //{
            //    case 'q':
            //    case 'Q':
            //    case '\x0027': //escape key
            //        return 0;
            //    case ' ': //Save an image
            //        //sprintf(filename, "filename%.3d.jpg", n++);
            //        Imwrite($"image{n++}", frame);
            //        //cout << "Saved " << filename << endl;
            //        break;
            //    default:
            //        break;
            //}
        }


        //int main(int ac, char** av)
        //{
        //    VideoCapture capture(0);
        //    if (!capture.isOpened())
        //    {
        //        return 1;
        //    }
        //    return process(capture);
        //}

        //static double Var(Image<Gray, float> img, double mu)
        //{       //variance
        //    int x, y;
        //    double ret = 0;
        //    int width = img.Size.Width;
        //    int height = img.Size.Height;
        //    float c;
        //    float[,,] data = img.Data;
        //    for (x = 0; x <= width; x++)
        //    {
        //        for (y = 0; y <= height; y++)
        //        {
        //            c = data[y, x, 0];
        //            ret += Pow(Abs(c - mu), 2);
        //        }
        //    }
        //    return ret;
        //}


    }



}
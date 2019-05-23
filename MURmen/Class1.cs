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

namespace MYRmen
{
    [ImgClass("МУРовцы")]
    public static class MYRmen_Class
    {
        public static Image<Bgr, byte> image_orig = null;
        public static bool Flag = true;
        [ImgMethod("Методы МУРовцев", "очистка фона 8*8")]  //писать перед методом
        public static OutputImage MYRmen_Method(Image<Bgr, byte> image)   ////очистка фона
        {
            if (Flag)
            {
                image_orig = new Image<Bgr, byte>(image.Size);
                image_orig.Data = image.Data;
                Flag = false;
            }
            byte[,,] data = image.Data;

            ulong[,] longData = new ulong[image.Rows, image.Cols];

            for (int y = 0; y < image.Rows; y++)
            {
                for (int x = 0; x < image.Cols; x++)
                {
                    longData[y, x] = (ulong)(data[y, x, 0]);                     //0-B 1-G 2-R
                }
            }
            ulong Counter_e = 0;
            ulong Counter_e2 = 0;
            int s = image.Rows / 8;
            int s2 = s / 2;
            ulong L = 0;
            for (int i = 0; i < image.Rows; i++)
            {
                for (int j = 0; j < image.Cols; j++)
                {

                    int x1 = i - s2;                //x1..x2  по y1   x1 y1..y2    x1..x2 y2    x2 y1..y2
                    int x2 = i + s2;
                    int y1 = j - s2;
                    int y2 = j + s2;            //(a+b)*2 = P    ====L

                    if (x1 < 0)
                        x1 = 0;
                    if (x2 >= image.Cols)
                        x2 = image.Cols - 1;
                    if (y1 < 0)
                        y1 = 0;
                    if (y2 >= image.Rows)
                        y2 = image.Rows - 1;
                    ulong ni = 0;
                    L = (ulong)(((x2 - x1) + (y2 - y1)) * 2);//периметр квадрата

                    for (int x = x1; x < x2; x++)
                    {
                        ni += longData[x, y1];//1 сторона
                        ni += longData[x, y2];
                    }
                    for (int y = y1 + 1; y < y2 - 1; y++)
                    {
                        ni += longData[x1, y];//1 сторона
                        ni += longData[x2, y];
                    }
                    ulong n_apostrof = ni / L;// ???цвет фона???
                    if (longData[i, j] < n_apostrof)
                    {
                        data[i, j, 0] = data[i, j, 1] = data[i, j, 2] = 0;
                    }
                    else
                        data[i, j, 0] = data[i, j, 1] = data[i, j, 2] = (byte)(longData[i, j] - n_apostrof);
                }
            }
            image.Save("8_8.bmp");
            image.Data = data;
            return new OutputImage
            {
                Image = image,
                Info = "Применена очистка фона 8х8"
            };
        }
        [ImgMethod("Методы МУРовцев", "очистка фона 16*16")]  //писать перед методом
        public static OutputImage MYRmen_Method2(Image<Bgr, byte> image)   ////очистка фона
        {
            if (Flag)
            {
                image_orig = new Image<Bgr, byte>(image.Size);
                image_orig.Data = image.Data;
                Flag = false;
            }
            byte[,,] data = image.Data;

            ulong[,] longData = new ulong[image.Rows, image.Cols];

            for (int y = 0; y < image.Rows; y++)
            {
                for (int x = 0; x < image.Cols; x++)
                {
                    longData[y, x] = (ulong)(data[y, x, 0]);                     //0-B 1-G 2-R
                }
            }
            ulong Counter_e = 0;
            ulong Counter_e2 = 0;
            int s = image.Rows / 16;
            int s2 = s / 2;
            ulong L = 0;
            for (int i = 0; i < image.Rows; i++)
            {
                for (int j = 0; j < image.Cols; j++)
                {

                    int x1 = i - s2;                //x1..x2  по y1   x1 y1..y2    x1..x2 y2    x2 y1..y2
                    int x2 = i + s2;
                    int y1 = j - s2;
                    int y2 = j + s2;            //(a+b)*2 = P    ====L

                    if (x1 < 0)
                        x1 = 0;
                    if (x2 >= image.Cols)
                        x2 = image.Cols - 1;
                    if (y1 < 0)
                        y1 = 0;
                    if (y2 >= image.Rows)
                        y2 = image.Rows - 1;
                    ulong ni = 0;
                    L = (ulong)(((x2 - x1) + (y2 - y1)) * 2);//периметр квадрата

                    for (int x = x1; x < x2; x++)
                    {
                        ni += longData[x, y1];//1 сторона
                        ni += longData[x, y2];
                    }
                    for (int y = y1 + 1; y < y2 - 1; y++)
                    {
                        ni += longData[x1, y];//1 сторона
                        ni += longData[x2, y];
                    }
                    ulong n_apostrof = ni / L;// ???цвет фона???
                    if (longData[i, j] < n_apostrof)
                    {
                        data[i, j, 0] = data[i, j, 1] = data[i, j, 2] = 0;
                    }
                    else
                        data[i, j, 0] = data[i, j, 1] = data[i, j, 2] = (byte)(longData[i, j] - n_apostrof);
                }
            }
            image.Save("16_16.bmp");
            image.Data = data;
            return new OutputImage
            {
                Image = image,
                Info = "Применена очистка фона 16х16"
            };
        }

        [ImgMethod("Методы МУРовцев", "фурье")]  //писать перед методом
        public static OutputImage MYRmen_Method3(Image<Bgr, byte> image)   ////очистка фона
        {

            byte[,,] data = image.Data;
            //  Flag2 = true;
            Flag = true;
            ulong[,] longData = new ulong[image.Rows, image.Cols];
            Image<Gray, float> _image = image.Convert<Gray, float>();
            Image<Gray, float> dft = new Image<Gray, float>(image.Size);
            Image<Gray, float> indft = new Image<Gray, float>(image.Size);
            CvInvoke.Dft(_image, dft, DxtType.Forward, _image.Rows);
            dft.Save("temp1.bmp");
            longData = new ulong[dft.Rows, dft.Cols];
            CvInvoke.Dft(dft, indft, Emgu.CV.CvEnum.DxtType.Inverse, _image.Rows);
            indft.Save("temp2.bmp");
            Image<Bgr, byte> _image5 = new Image<Bgr, byte>("temp2.bmp");
            image.Data = _image5.Data;
            return new OutputImage
            {
                Image = image,
                Info = "Применено прямое и обратное преобразование по фурье"
            };
        }

        [ImgMethod("Методы МУРовцев", "form")]
        public static OutputImage Start_form(Image<Bgr, byte> image)
        {
            Form1 form = null;
            try
            {
                Bitmap img = image.Bitmap;
                Bitmap img_orig = image_orig.Bitmap;
                form = new Form1(img_orig, img);
                form.ShowDialog();
                form.get_image();
            }
            catch (Exception e)
            {
                MessageBox.Show("примените к изображению очистку фона и фурье преобразование");
            }
            return new OutputImage
            {
                Image = form.get_image(),
                Info = ""
            };
        }


    }
}


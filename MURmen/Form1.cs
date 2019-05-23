using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseLibrary;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace MYRmen
{
    public partial class Form1 : Form
    {
        PictureBox pic = new PictureBox();
        Image<Bgr, byte> image_const;
        Image<Bgr, byte> image_orig_const;
        Image<Bgr, byte> image;
        Image<Bgr, byte> image_orig;
        Image<Bgr, byte> image_orig_obrez;

        public Form1(Bitmap orig_image,Bitmap img )
        {
            InitializeComponent();
            image_const = new Image<Bgr, byte>(img.Size);
            image_orig_obrez = new Image<Bgr, byte>(orig_image.Size);
            image_orig_obrez.Bitmap = orig_image;//оригинальная картинка
            image_const.Bitmap = img;//обработанная картинка
            image_orig_const = new Image<Bgr, byte>(orig_image.Size);
            image_orig_const.Bitmap = orig_image;//оригинальная картинка 
            label1.Text= trackBar1.Value.ToString()+"*"+trackBar1.Value.ToString();
            label2.Text = (trackBar2.Value *0.1).ToString();
            trackBar1.MouseWheel += new MouseEventHandler(DoNothing_MouseWheel);
            trackBar2.MouseWheel += new MouseEventHandler(DoNothing_MouseWheel);    
        }
        public Image<Bgr, byte> get_image() { return image_orig; }
        private void DoNothing_MouseWheel(object sender, EventArgs e)
        {
            HandledMouseEventArgs ee = (HandledMouseEventArgs)e;
            ee.Handled = true;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {    
            label1.Text = trackBar1.Value.ToString() + "*" + trackBar1.Value.ToString();
            label2.Text = (trackBar2.Value * 0.1).ToString();
            image = new Image<Bgr, byte>(image_const.Size);
            image.Bitmap = image_const.Bitmap;//обработанная картинка
            image_orig = new Image<Bgr, byte>(image_orig_const.Size);
            image_orig.Bitmap = image_orig_const.Bitmap;//оригинальная картинка
            ulong N_sigma = 0;
            ulong Counter_sigma = 0;
            ulong S_sigma = 0;
            byte[,,] data = image.Data;
            int kol_colors = 0;
            int _kol_colors = 0;
            int kol = 0;
            List<float> ves = new List<float>();
            int shetchic = 0;
            shetchic = trackBar1.Value;
            double shetchic2 = 0;
            shetchic2 = trackBar2.Value*0.1;
            int d = image.Rows / shetchic;
            int j_counter = 0;
            int i_counter = 0;

            while (i_counter < shetchic)
            {
                while (j_counter < shetchic)//внутренний while проход по квадратам 8*8 по горизонтали
                {
                    for (int i = i_counter * d; i < i_counter * d + d; i++)
                    {
                        for (int j = j_counter * d; j < d * j_counter + d; j++)
                        {
                            if (image.Data[i, j, 0] > 128)
                                kol_colors++;
                        }
                    }
                    j_counter++;
                }
                j_counter = 0;
                i_counter++;
            }
            i_counter = 0;
            j_counter = 0;

            while (i_counter < shetchic)
            {
                while (j_counter < shetchic)//внутренний while проход по квадратам 8*8 по горизонтали
                {
                    for (int i = i_counter * d; i < i_counter * d + d; i++)
                    {
                        for (int j = j_counter * d; j < d * j_counter + d; j++)
                        {
                            if (image.Data[i, j, 0] > 127)
                            { _kol_colors++; }
                        }
                    }
                    ves.Add(((float)_kol_colors) / kol_colors);//вес ячейки
                    j_counter++;
                    _kol_colors = 0;
                }
                j_counter = 0;
                i_counter++;
            }
            int ves_counter = 0;

            ves_counter = 0;
            i_counter = 0;
            j_counter = 0;

            while (i_counter < shetchic)//разлиновывание новой пикчи квадратами 8*8
            {
                while (j_counter < shetchic)//внутренний while проход по квадратам 8*8 по горизонтали
                {
                    for (int i = i_counter * d; i < i_counter * d + d; i++)//разлиновывание картинки квадратами 1го уровня
                    {
                        for (int j = j_counter * d; j < d * j_counter + d; j++)
                        {
                            if (i_counter != 0)
                            {
                               
                                image_orig.Data[i_counter * d, j, 0] = image_orig.Data[i_counter * d, j, 1] = image_orig.Data[i_counter * d, j, 2] = 40;
                            }
                        }
                        if (j_counter != 0)
                        {
                           
                            image_orig.Data[i, j_counter * d, 0] = image_orig.Data[i, j_counter * d, 1] = image_orig.Data[i, j_counter * d, 2] = 40;
                        }
                    }
                    if (ves[ves_counter] > ves.Average() * shetchic2)
                    {
                        List<float> ves_2lvl = new List<float>();
                        int D_2lvl = d / 2;// сторона квадратика маленького 2 уровня
                        _kol_colors = 0;
                        int j_counter2 = 0;
                        int i_counter2 = 0;
                        int i_old = i_counter * d;
                        int j_old = j_counter * d;

                        while (i_counter2 < 2)
                        {
                            while (j_counter2 < 2)//внутренний while проход по квадратам 2 уровня
                            {
                                for (int i2 = i_old + i_counter2 * D_2lvl; i2 < i_old + i_counter2 * D_2lvl + D_2lvl; i2++)
                                {
                                    for (int j2 = j_old + j_counter2 * D_2lvl; j2 < j_old + D_2lvl * j_counter2 + D_2lvl; j2++)
                                    {
                                        if (image.Data[i2, j2, 0] > 127)//подсчет веса клетки 2го уровня
                                            _kol_colors++;
                                        //разлиновывание клеток 2 уровня
                                        {
                                            
                                            image_orig.Data[i_old + i_counter2 * D_2lvl, j2, 0] = image_orig.Data[i_old + i_counter2 * D_2lvl, j2, 1] = image_orig.Data[i_old + i_counter2 * D_2lvl, j2, 2] = 40;
                                        }
                                    }
                                    {
                                       
                                        image_orig.Data[i2, j_old + j_counter2 * D_2lvl, 0] = image_orig.Data[i2, j_old + j_counter2 * D_2lvl, 1] = image_orig.Data[i2, j_old + j_counter2 * D_2lvl, 2] = 40;
                                    }
                                }
                                ves_2lvl.Add(((float)_kol_colors) / ves[ves_counter]);//вес ячейки 2 уровня
                                j_counter2++;
                            }
                            j_counter2 = 0;
                            i_counter2++;
                        }
                        int ves_2lvl_counter = 0;
                        i_counter2 = 0;
                        j_counter2 = 0;
                        while (i_counter2 < 2)
                        {
                            while (j_counter2 < 2)//внутренний while проход по квадратам 8*8 по горизонтали
                            {
                                if (ves_2lvl[ves_2lvl_counter] > ves_2lvl.Average() * shetchic2)
                                {
                                    //3 lvl
                                    List<float> ves_3lvl = new List<float>();
                                    int D_3lvl = D_2lvl / 2;// сторона квадратика маленького 3 уровня
                                    _kol_colors = 0;
                                    int j_counter3 = 0;
                                    int i_counter3 = 0;
                                    int i_old2 = i_old + i_counter2 * D_2lvl;
                                    int j_old2 = j_old + j_counter2 * D_2lvl;
                                    while (i_counter3 < 2)
                                    {
                                        while (j_counter3 < 2)//внутренний while проход по квадратам 3 уровня
                                        {
                                            for (int i3 = i_old2 + i_counter3 * D_3lvl; i3 < i_old2 + i_counter3 * D_3lvl + D_3lvl; i3++)
                                            {
                                                for (int j3 = j_old2 + j_counter3 * D_3lvl; j3 < j_old2 + j_counter3 * D_3lvl + D_3lvl; j3++)
                                                {
                                                    if (image.Data[i3, j3, 0] > 128)
                                                        _kol_colors++;

                                                    image_orig.Data[i_old2 + i_counter3 * D_3lvl, j3, 0] =
                                                          image_orig.Data[i_old2 + i_counter3 * D_3lvl, j3, 1] =
                                                          image_orig.Data[i_old2 + i_counter3 * D_3lvl, j3, 2] = 40;
                                                }
                                                image_orig.Data[i3, j_old2 + j_counter3 * D_3lvl, 0]
                                                    = image_orig.Data[i3, j_old2 + j_counter3 * D_3lvl, 1]
                                                    = image_orig.Data[i3, j_old2 + j_counter3 * D_3lvl, 2] = 40;
                                            }
                                            ves_3lvl.Add(((float)_kol_colors) / ves_2lvl[ves_2lvl_counter]);//вес ячейки 3 уровня
                                            j_counter3++;
                                        }
                                        j_counter3 = 0;
                                        i_counter3++;
                                    }
                                    i_counter3 = 0;
                                    j_counter3 = 0;
                                    int ves_3lvl_counter = 0;
                                    while (i_counter3 < 2)
                                    {
                                        while (j_counter3 < 2)//внутренний while проход по квадратам 3 уровня
                                        {
                                            if (ves_3lvl[ves_3lvl_counter] > ves_3lvl.Average() * shetchic2)
                                            {

                                                List<float> ves_4lvl = new List<float>();
                                                int D_4lvl = D_3lvl / 2;// сторона квадратика маленького 3 уровня
                                                _kol_colors = 0;
                                                int j_counter4 = 0;
                                                int i_counter4 = 0;
                                                int i_old3 = i_old2 + i_counter3 * D_3lvl;
                                                int j_old3 = j_old2 + j_counter3 * D_3lvl;
                                                while (i_counter4 < 2)
                                                {
                                                    while (j_counter4 < 2)//внутренний while проход по квадратам 3 уровня
                                                    {
                                                        for (int i4 = i_old3 + i_counter4 * D_4lvl; i4 < i_old3 + i_counter4 * D_4lvl + D_4lvl; i4++)
                                                        {
                                                            for (int j4 = j_old3 + j_counter4 * D_4lvl; j4 < j_old3 + j_counter4 * D_4lvl + D_4lvl; j4++)
                                                            {
                                                                if (image.Data[i4, j4, 0] > 128)
                                                                    _kol_colors++;

                                                                image_orig.Data[i_old3 + i_counter4 * D_4lvl, j4, 0] =
                                                                      image_orig.Data[i_old3 + i_counter4 * D_4lvl, j4, 1] =
                                                                      image_orig.Data[i_old3 + i_counter4 * D_4lvl, j4, 2] = 40;

                                                            }

                
                                                            image_orig.Data[i4, j_old3 + j_counter4 * D_4lvl, 0]
                                                                = image_orig.Data[i4, j_old3 + j_counter4 * D_4lvl, 1]
                                                                = image_orig.Data[i4, j_old3 + j_counter4 * D_4lvl, 2] = 40;

                                                        }
                                                        ves_4lvl.Add(((float)_kol_colors) / ves_3lvl[ves_3lvl_counter]);//вес ячейки 3 уровня
                                                        j_counter4++;
                                                    }
                                                    j_counter4 = 0;
                                                    i_counter4++;
                                                }
                                                int ves_4lvl_counter = 0;
                                                i_counter4 = 0;
                                                j_counter4 = 0;
                                                while (i_counter4 < 2)
                                                {
                                                    while (j_counter4 < 2)//внутренний while проход по квадратам 3 уровня
                                                    {
                                                        if (ves_4lvl[ves_4lvl_counter] > ves_4lvl.Average() * shetchic2)
                                                        {
                                                            for (int i4 = i_old3 + i_counter4 * D_4lvl; i4 < i_old3 + i_counter4 * D_4lvl + D_4lvl; i4++)
                                                            {
                                                                for (int j4 = j_old3 + j_counter4 * D_4lvl; j4 < j_old3 + j_counter4 * D_4lvl + D_4lvl; j4++)
                                                                {
                                                                    image_orig.Data[i4, j4, 2] = 0;
                                                                }

                                                            }
                                                        }
                                                        ves_4lvl_counter++;
                                                        j_counter4++;
                                                    }
                                                    j_counter4 = 0;
                                                    i_counter4++;
                                                }
                                            } 
                                            j_counter3++;
                                            ves_3lvl_counter++;
                                        }
                                        j_counter3 = 0;
                                        i_counter3++;
                                    }
                                } 
                                ves_2lvl_counter++;
                                j_counter2++;
                            }
                            j_counter2 = 0;
                            i_counter2++;
                        }
                    }
                    ves_counter++;
                    //квадрат завершен 
                    j_counter++;
                }
                j_counter = 0;
                i_counter++;
            }
            pictureBox1.Image = image_orig.Bitmap;
            image_orig.Save("pic_end.bmp");
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString() + "*" + trackBar1.Value.ToString();
            label2.Text = (trackBar2.Value * 0.1).ToString();
            image = new Image<Bgr, byte>(image_const.Size);
            image.Bitmap = image_const.Bitmap;//обработанная картинка
            image_orig = new Image<Bgr, byte>(image_orig_const.Size);
            image_orig.Bitmap = image_orig_const.Bitmap;//оригинальная картинка
            ulong N_sigma = 0;
            ulong Counter_sigma = 0;
            ulong S_sigma = 0;
            byte[,,] data = image.Data;
            int kol_colors = 0;
            int _kol_colors = 0;
            int kol = 0;
            List<float> ves = new List<float>();
            int shetchic = 0;
            shetchic = trackBar1.Value;
            double shetchic2 = 0;
            shetchic2 = trackBar2.Value *0.1;
            int d = image.Rows / shetchic;
            int j_counter = 0;
            int i_counter = 0;

            while (i_counter < shetchic)
            {
                while (j_counter < shetchic)//внутренний while проход по квадратам 8*8 по горизонтали
                {
                    for (int i = i_counter * d; i < i_counter * d + d; i++)
                    {
                        for (int j = j_counter * d; j < d * j_counter + d; j++)
                        {
                            if (image.Data[i, j, 0] > 128)
                                kol_colors++;
                        }
                    }
                    j_counter++;
                }
                j_counter = 0;
                i_counter++;
            }
            i_counter = 0;
            j_counter = 0;

            while (i_counter < shetchic)
            {
                while (j_counter < shetchic)//внутренний while проход по квадратам 8*8 по горизонтали
                {
                    for (int i = i_counter * d; i < i_counter * d + d; i++)
                    {
                        for (int j = j_counter * d; j < d * j_counter + d; j++)
                        {
                            if (image.Data[i, j, 0] > 127)
                            { _kol_colors++; }
                        }
                    }
                    ves.Add(((float)_kol_colors) / kol_colors);//вес ячейки
                    j_counter++;
                    _kol_colors = 0;
                }
                j_counter = 0;
                i_counter++;
            }
            int ves_counter = 0;

            ves_counter = 0;
            i_counter = 0;
            j_counter = 0;

            while (i_counter < shetchic)//разлиновывание новой пикчи квадратами 8*8
            {
                while (j_counter < shetchic)//внутренний while проход по квадратам 8*8 по горизонтали
                {
                    for (int i = i_counter * d; i < i_counter * d + d; i++)//разлиновывание картинки квадратами 1го уровня
                    {
                        for (int j = j_counter * d; j < d * j_counter + d; j++)
                        {
                            if (i_counter != 0)
                            {
                                
                                image_orig.Data[i_counter * d, j, 0] = image_orig.Data[i_counter * d, j, 1] = image_orig.Data[i_counter * d, j, 2] = 40;
                            }
                        }
                        if (j_counter != 0)
                        {
                          
                            image_orig.Data[i, j_counter * d, 0] = image_orig.Data[i, j_counter * d, 1] = image_orig.Data[i, j_counter * d, 2] = 40;
                        }
                    }
                    //новый проход по квадруту для выделения пикселей если масса ячейки >20%
                    if (ves[ves_counter] > ves.Average() * shetchic2)
                    {
                        List<float> ves_2lvl = new List<float>();
                        int D_2lvl = d / 2;// сторона квадратика маленького 2 уровня
                        _kol_colors = 0;
                        int j_counter2 = 0;
                        int i_counter2 = 0;
                        int i_old = i_counter * d;
                        int j_old = j_counter * d;

                        while (i_counter2 < 2)
                        {
                            while (j_counter2 < 2)//внутренний while проход по квадратам 2 уровня
                            {
                                for (int i2 = i_old + i_counter2 * D_2lvl; i2 < i_old + i_counter2 * D_2lvl + D_2lvl; i2++)
                                {
                                    for (int j2 = j_old + j_counter2 * D_2lvl; j2 < j_old + D_2lvl * j_counter2 + D_2lvl; j2++)
                                    {
                                        if (image.Data[i2, j2, 0] > 127)//подсчет веса клетки 2го уровня
                                            _kol_colors++;
                                        //разлиновывание клеток 2 уровня
                                        {
                                            
                                            image_orig.Data[i_old + i_counter2 * D_2lvl, j2, 0] = image_orig.Data[i_old + i_counter2 * D_2lvl, j2, 1] = image_orig.Data[i_old + i_counter2 * D_2lvl, j2, 2] = 40;
                                        }
                                    }
                                    {
                                       
                                        image_orig.Data[i2, j_old + j_counter2 * D_2lvl, 0] = image_orig.Data[i2, j_old + j_counter2 * D_2lvl, 1] = image_orig.Data[i2, j_old + j_counter2 * D_2lvl, 2] = 40;
                                    }
                                }
                                ves_2lvl.Add(((float)_kol_colors) / ves[ves_counter]);//вес ячейки 2 уровня
                                j_counter2++;
                            }
                            j_counter2 = 0;
                            i_counter2++;
                        }
                        int ves_2lvl_counter = 0;
                        i_counter2 = 0;
                        j_counter2 = 0;
                        while (i_counter2 < 2)
                        {
                            while (j_counter2 < 2)//внутренний while проход по квадратам 8*8 по горизонтали
                            {
                                if (ves_2lvl[ves_2lvl_counter] > ves_2lvl.Average() * shetchic2)
                                {
                                    //3 lvl
                                    List<float> ves_3lvl = new List<float>();
                                    int D_3lvl = D_2lvl / 2;// сторона квадратика маленького 3 уровня
                                    _kol_colors = 0;
                                    int j_counter3 = 0;
                                    int i_counter3 = 0;
                                    int i_old2 = i_old + i_counter2 * D_2lvl;
                                    int j_old2 = j_old + j_counter2 * D_2lvl;
                                    while (i_counter3 < 2)
                                    {
                                        while (j_counter3 < 2)//внутренний while проход по квадратам 3 уровня
                                        {
                                            for (int i3 = i_old2 + i_counter3 * D_3lvl; i3 < i_old2 + i_counter3 * D_3lvl + D_3lvl; i3++)
                                            {
                                                for (int j3 = j_old2 + j_counter3 * D_3lvl; j3 < j_old2 + j_counter3 * D_3lvl + D_3lvl; j3++)
                                                {
                                                    if (image.Data[i3, j3, 0] > 128)
                                                        _kol_colors++;
                                                    image_orig.Data[i_old2 + i_counter3 * D_3lvl, j3, 0] =
                                                          image_orig.Data[i_old2 + i_counter3 * D_3lvl, j3, 1] =
                                                          image_orig.Data[i_old2 + i_counter3 * D_3lvl, j3, 2] = 40;
                                                }
                                                image_orig.Data[i3, j_old2 + j_counter3 * D_3lvl, 0]
                                                    = image_orig.Data[i3, j_old2 + j_counter3 * D_3lvl, 1]
                                                    = image_orig.Data[i3, j_old2 + j_counter3 * D_3lvl, 2] = 40;
                                            }
                                            ves_3lvl.Add(((float)_kol_colors) / ves_2lvl[ves_2lvl_counter]);//вес ячейки 3 уровня
                                            j_counter3++;
                                        }
                                        j_counter3 = 0;
                                        i_counter3++;
                                    }
                                    i_counter3 = 0;
                                    j_counter3 = 0;
                                    int ves_3lvl_counter = 0;
                                    while (i_counter3 < 2)
                                    {
                                        while (j_counter3 < 2)//внутренний while проход по квадратам 3 уровня
                                        {
                                            if (ves_3lvl[ves_3lvl_counter] > ves_3lvl.Average() * shetchic2)
                                            {

                                                List<float> ves_4lvl = new List<float>();
                                                int D_4lvl = D_3lvl / 2;// сторона квадратика маленького 3 уровня
                                                _kol_colors = 0;
                                                int j_counter4 = 0;
                                                int i_counter4 = 0;
                                                int i_old3 = i_old2 + i_counter3 * D_3lvl;
                                                int j_old3 = j_old2 + j_counter3 * D_3lvl;
                                                while (i_counter4 < 2)
                                                {
                                                    while (j_counter4 < 2)//внутренний while проход по квадратам 3 уровня
                                                    {
                                                        for (int i4 = i_old3 + i_counter4 * D_4lvl; i4 < i_old3 + i_counter4 * D_4lvl + D_4lvl; i4++)
                                                        {
                                                            for (int j4 = j_old3 + j_counter4 * D_4lvl; j4 < j_old3 + j_counter4 * D_4lvl + D_4lvl; j4++)
                                                            {
                                                                if (image.Data[i4, j4, 0] > 128)
                                                                    _kol_colors++;
                                                                image_orig.Data[i_old3 + i_counter4 * D_4lvl, j4, 0] =
                                                                      image_orig.Data[i_old3 + i_counter4 * D_4lvl, j4, 1] =
                                                                      image_orig.Data[i_old3 + i_counter4 * D_4lvl, j4, 2] = 40;

                                                            }
                                                            image_orig.Data[i4, j_old3 + j_counter4 * D_4lvl, 0]
                                                                = image_orig.Data[i4, j_old3 + j_counter4 * D_4lvl, 1]
                                                                = image_orig.Data[i4, j_old3 + j_counter4 * D_4lvl, 2] = 40;

                                                        }
                                                        ves_4lvl.Add(((float)_kol_colors) / ves_3lvl[ves_3lvl_counter]);//вес ячейки 3 уровня
                                                        j_counter4++;
                                                    }
                                                    j_counter4 = 0;
                                                    i_counter4++;
                                                }
                                                int ves_4lvl_counter = 0;
                                                i_counter4 = 0;
                                                j_counter4 = 0;
                                                while (i_counter4 < 2)
                                                {
                                                    while (j_counter4 < 2)//внутренний while проход по квадратам 3 уровня
                                                    {
                                                        if (ves_4lvl[ves_4lvl_counter] > ves_4lvl.Average() * shetchic2)
                                                        {
                                                            for (int i4 = i_old3 + i_counter4 * D_4lvl; i4 < i_old3 + i_counter4 * D_4lvl + D_4lvl; i4++)
                                                            {
                                                                for (int j4 = j_old3 + j_counter4 * D_4lvl; j4 < j_old3 + j_counter4 * D_4lvl + D_4lvl; j4++)
                                                                {
                                                                    image_orig.Data[i4, j4, 2] = 0;
                                                                }

                                                            }
                                                        }

                                                        ves_4lvl_counter++;
                                                        j_counter4++;
                                                    }
                                                    j_counter4 = 0;
                                                    i_counter4++;
                                                }
                                            }
                                            j_counter3++;
                                            ves_3lvl_counter++;
                                        }
                                        j_counter3 = 0;
                                        i_counter3++;
                                    }
                                }
                                ves_2lvl_counter++;
                                j_counter2++;
                            }
                            j_counter2 = 0;
                            i_counter2++;
                        }
                    }
                    ves_counter++;
                    //квадрат завершен 
                    j_counter++;
                }
                j_counter = 0;
                i_counter++;
            }
            pictureBox1.Image = image_orig.Bitmap;
            image_orig.Save("pic_end.bmp");
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using BaseLibrary;
using System.Reflection;
using System.Runtime.InteropServices;

namespace myLab
{
    public partial class Segment : Form
    {
        IImage img, result;

        #region blabla
        [SaveParamAttribute]
        public bool isWat { get; set; }
        [SaveParamAttribute]
        public bool isMen { get; set; }
        [SaveParamAttribute]
        public bool isFlood { get; set; }
        [SaveParamAttribute]
        public int mv { get; set; }
        [SaveParamAttribute]
        public int dv { get; set; }
        [SaveParamAttribute]
        public int k { get; set; }
        [SaveParamAttribute]
        public int distmask { get; set; }
        [SaveParamAttribute]
        public int bint { get; set; }
        [SaveParamAttribute]
        public int Property1 { get; set; }
        [SaveParamAttribute]
        public int ld { get; set; }
        [SaveParamAttribute]
        public int ud { get; set; }
        [SaveParamAttribute]

        public string point { get; set; }
        [SaveParamAttribute]

        public string color { get; set; }
        [SaveParamAttribute]

        public int pr { get; set; }
        [SaveParamAttribute]

        public int cr { get; set; }
        [SaveParamAttribute]

        public int pl { get; set; }
        //  [SaveParamAttribute]

        // public int brig { get; set; }
        // [SaveParamAttribute]
        // public double contr { get; set; }
        [SaveParamAttribute]
        public string term { get; set; }
        #endregion

        public IImage OutputImg { get => result; set { result = value; sRes(); } }

        public Segment(IImage image)//: base(image,info)
        {
            InitializeComponent();
            this.img = image;
            fSP0.Maximum = image.Bitmap.Width;
            fSP1.Maximum = image.Bitmap.Height;
            this.LoadSettings("segments");
            if (isFlood)
            {
                string[] cols = color.Split(':');
                colorDialog1.Color = Color.FromArgb(int.Parse(cols[0]), int.Parse(cols[1]), int.Parse(cols[2]));
                pictureBox1.BackColor = colorDialog1.Color;
                cols = point.Split(':');
                fSP0.Value = (int)int.Parse(cols[0]);
                fSP1.Value = (int)int.Parse(cols[1]);

                fLD0.Value = (int)ld;
                fUD0.Value = (int)ud;
            }


            if (isMen)
            {
                //  mC.Value = (int)(contr * 100);//вычсляем кооф контрастности
                //  mB.Value = (int)brig;
                mCR.Value = cr;
                mPL.Value = (int)pl;
                mSR.Value = (int)pr;
                string[] temp = term.Split(':');
                mT1.Value = int.Parse(temp[0]);
                mT2.Value = int.Parse(temp[1]);
            }

            if (isWat)
            {
                wP1MI.Value = mv;
                wP1DI.Value = dv;
                wP1NM.Value = k;
                wP1BT.Value = bint;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Point fillPoint = new Point((int)fSP0.Value, (int)fSP1.Value);
            point = fSP0.Value + ":" + fSP1.Value;
            color = colorDialog1.Color.R + ":" + colorDialog1.Color.G + ":" + colorDialog1.Color.B;
            ld = (int)fLD0.Value;
            ud = (int)fUD0.Value;
            //neighbors = 8
            dynamic input = img.Clone();
            var res = (IImage)img.Clone();
            CvInvoke.ConvertScaleAbs(res, res, nuv(Fcontr) / 100f, nuv(Fbrigh));//image = cv2.convertScaleAbs(image, alpha=1.2, beta=0)

            //получаем маску для заливки
            Mat outputMask = new Mat(input.Height + 2, input.Width + 2, DepthType.Cv8U, 1);//mask = np.zeros((h + 2, w + 2), np.uint8) 
            //заливаем области с начаьной точкой fillPoint
            CvInvoke.FloodFill
                (
                res,
                outputMask,
                fillPoint,
                new MCvScalar(colorDialog1.Color.B, colorDialog1.Color.G, colorDialog1.Color.R),
                out _,
                new MCvScalar(ld, ld, ld),
                new MCvScalar(ud, ud, ud)
                );//_, image, mask, rect =cv2.floodFill(image, mask, startPoint, fillColor, loDiff, upDiff)
            OutputImg = res;
            //   return new OutputImage { Image = res, Name = "FloodFill" };
        }


        private void Button2_Click(object sender, EventArgs e)
        {

            var new_ing = (IImage)img.Clone();//копия исходного изображения
            double contr = ((int)mC.Value) / 100.0;//вычсляем кооф контрастности
            int brig = (int)mB.Value;
            cr = (int)mCR.Value;
            pl = (int)mPL.Value;
            pr = (int)mSR.Value;
            term = (int)mT1.Value + ":" + (int)mT2.Value;
            //изменяем яркость и контрастность
            CvInvoke.ConvertScaleAbs(new_ing, new_ing, contr, brig);//image = cv2.convertScaleAbs(image, alpha=1.2, beta=0)
            var imageSegment = (IImage)img.Clone();
            //применяем алгоритм MeanShift
            CvInvoke.PyrMeanShiftFiltering(new_ing, imageSegment, pr, cr, pl, new MCvTermCriteria((int)mT1.Value, (int)mT2.Value));//imageSegment=cv2.pyrMeanShiftFiltering(image, spatialRadius, colorRadius, pyramidLevels)
            OutputImg = (IImage)imageSegment.Clone();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackColor = colorDialog1.Color;
            }
        }
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

        private void Button4_Click(object sender, EventArgs e)
        {
            using (GetPont gp = new GetPont(img))
            {
                gp.ShowDialog();
                if (gp.retPoint != null)
                {
                    fSP0.Value = gp.retPoint.X;
                    fSP1.Value = gp.retPoint.Y;
                }
            }
        }
        int nuv(NumericUpDown nd)
        {
            return (int)nd.Value;
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            Mat markers = new Mat();
            Image<Bgr, byte> res = (Image<Bgr, byte>)img.Clone();
            CvInvoke.ConvertScaleAbs(res, res, nuv(Wcontr) / 100f, nuv(Wbrig));//image = cv2.convertScaleAbs(image, alpha=1.2, beta=0)
            try
            {
                if (radioButton1.Checked)
                {
                    Point p = new Point(0, 0);
                    mv = (int)wP1MI.Value;
                    dv = (int)wP1DI.Value;
                    k = (int)wP1NM.Value;
                    bint = (int)wP1BT.Value;
                    MCvScalar m = new MCvScalar((int)wP1S.Value);
                    distmask = (int)wP1DTM.Value;

                    //создаем матрицу для чбизображения
                    Mat gray = new Mat();
                    //перевоим наше изображениев полутон
                    CvInvoke.CvtColor(res, gray, ColorConversion.Bgr2Gray);//gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

                    //создаем матрицу для бинарзоанного иображения
                    Mat thresh = new Mat();
                    //бинаризуем с порогом 
                    CvInvoke.Threshold(gray, thresh, bint, 255, bint == 0 ? (ThresholdType)9 : ThresholdType.BinaryInv);//ret, thresh = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY_INV + cv2.THRESH_OTSU)

                    //# noise removal
                    Mat opening = new Mat();
                    //убираем шум
                    Mat kernel = new Mat(new Size(k, k), DepthType.Cv8U, 1);//kernel = np.ones((3, 3), np.uint8)
                    CvInvoke.MorphologyEx(thresh, opening, MorphOp.Open, kernel, p, mv, BorderType.Default, m);//opening = cv2.morphologyEx(thresh, cv2.MORPH_OPEN, kernel, iterations=2)

                    //
                    Mat sure_bg = new Mat();
                    //с помощью дилатации отделяем фон
                    CvInvoke.Dilate(opening, sure_bg, kernel, p, dv, BorderType.Default, m);//sure_bg = cv2.dilate(opening, kernel, iterations=3)

                    //# Finding sure foreground area
                    Mat dist_transform = new Mat();

                    //подготавлваемся к нахождению конецной области переднего плана
                    //Применяем аглоритм Distance Transform для нахождений впадин
                    CvInvoke.DistanceTransform(opening, dist_transform, null, DistType.L2, distmask); //dist_transform = cv2.distanceTransform(opening, cv2.DIST_L2, 5)

                    //нормаизуем 
                    CvInvoke.Normalize(dist_transform, dist_transform, 0, 1.0, NormType.MinMax);
                    dist_transform.ConvertTo(dist_transform, DepthType.Cv8U, 255, 0);


                    Mat sure_fg = new Mat();
                    //убираем все ненужное с помощью бинаризации
                    CvInvoke.Threshold(dist_transform/*opening*/, sure_fg, 0.7 * dist_transform.GetData().Max(), 255, ThresholdType.Binary);//ret, sure_fg = cv2.threshold(dist_transform, 0.7 * dist_transform.max(), 255, 0)
                                                                                                                                            //преобразование к типу byte
                    sure_fg.ConvertTo(sure_fg, DepthType.Cv8U);//sure_fg = np.uint8(sure_fg)
                    Mat unknown = new Mat();
                    //находим разницу между передним и задним планами
                    CvInvoke.Subtract(sure_bg, sure_fg, unknown);//unknown = cv2.subtract(sure_bg, sure_fg)
                                                                 //# Marker labelling
                                                                 //находим апроксимацию вершин
                    CvInvoke.ConnectedComponents(sure_bg, markers);//ret, markers = cv2.connectedComponents(sure_fg)
                                                                   //Add one to all labels so that sure background is not 0, but 1
                    Gray grayVal = new Gray(1);

                    Mat temp = new Mat(markers.Size, markers.Depth, markers.NumberOfChannels);
                    temp.SetTo(new MCvScalar(1));
                    //увеличиваем все показатели markers на 1
                    CvInvoke.Add(markers, markers, temp);//markers = markers + 1

                    //Now, mark the region of unknown with zero
                    for (int i = 0; i < markers.Height; i++)
                        for (int j = 0; j < markers.Width; j++)
                        {
                            if (GetValue(unknown, i, j) == 255)//проверяем вершины - если 255
                            {
                                //Убираем маркер
                                SetValue(markers, i, j, 0);  //markers[unknown == 255] = 0
                            }
                        }
                }
                else
                {
                    markers = forms.res;
                    forms.Dispose();
                }
                //используем водоразделя для полученныхх вершин
                CvInvoke.Watershed(res, markers);//markers = cv2.watershed(img, markers)

                for (int i = 0; i < res.Size.Height; i++)
                    for (int j = 0; j < res.Size.Width - 1; j++)
                    {
                        if (GetValue(markers, i, j) == -1)
                        {
                            //markers[unknown == 255] = 0
                            res.Data[i, j, 0] = 255;//отмечаем на изображении области
                            res.Data[i, j, 1] = 0;
                            res.Data[i, j, 2] = 0;
                            res.Data[i, j + 1, 0] = 255;//отмечаем на изображении области
                            res.Data[i, j + 1, 1] = 0;
                            res.Data[i, j + 1, 2] = 0;
                        }
                    }
                OutputImg = res;
            }
            catch (Emgu.CV.Util.CvException ex)
            {

                MessageBox.Show("Некорректные параметры");
            }
        }



        private void Button5_Click(object sender, EventArgs e)
        {
            Point p = new Point(0, 0);
            mv = (int)wP1MI.Value;
            dv = (int)wP1DI.Value;
            k = (int)wP1NM.Value;
            bint = (int)wP1BT.Value;
            MCvScalar m = new MCvScalar((int)wP1S.Value);
            distmask = (int)wP1DTM.Value;
            isWat = true;
            this.SaveSetting("segments");
        }

        private void TabPage1_Click(object sender, EventArgs e)
        {
            this.SaveSetting("segments");
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            //contr = ((int)mC.Value) / 100.0;//вычсляем кооф контрастности
            //brig = (int)mB.Value;
            cr = (int)mCR.Value;
            pl = (int)mPL.Value;
            pr = (int)mSR.Value;
            term = (int)mT1.Value + ":" + (int)mT2.Value;
            isMen = true;
            this.SaveSetting("segments");
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            isFlood = true;
            Point fillPoint = new Point((int)fSP0.Value, (int)fSP1.Value);
            point = fSP0.Value + ":" + fSP1.Value;
            color = colorDialog1.Color.R + ":" + colorDialog1.Color.G + ":" + colorDialog1.Color.B;
            ld = (int)fLD0.Value;
            ud = (int)fUD0.Value;
            this.SaveSetting("segments");
        }
        MarkerSet forms;
        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox4.Enabled = radioButton1.Checked;
            button8.Enabled = radioButton2.Checked;
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            forms = new MarkerSet(img);
            forms.ShowDialog();
        }

        private void Segment_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (forms != null)
                forms.Close();
        }

        void sRes()
        {
            BaseMethods.LoadOutputImage(new OutputImage { Image = (IImage)OutputImg.Clone() });
        }
    }
}

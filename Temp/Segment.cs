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

namespace Temp
{
    public partial class Segment : Form
    {
        IImage img, result;

        public IImage OutputImg { get => result; set { result = value; Close(); } }

        public Segment(IImage image)//: base(image,info)
        {
            InitializeComponent();
            this.img = image;
            fSP0.Maximum = image.Bitmap.Width;
            fSP1.Maximum = image.Bitmap.Height;
        }

        private void TabPage3_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //loDiff = (2, 2, 2)
            // upDiff = (2, 2, 2)
            Point fillPoint = new Point((int)fSP0.Value, (int)fSP1.Value);
            //neighbors = 8
            dynamic input = img.Clone();
            var res = (IImage)img.Clone();
            Mat outputMask = new Mat(input.Height + 2, input.Width + 2, DepthType.Cv8U, 1);//mask = np.zeros((h + 2, w + 2), np.uint8) 
            CvInvoke.FloodFill(res, outputMask, fillPoint, new MCvScalar(colorDialog1.Color.R/255f, colorDialog1.Color.G/255f, colorDialog1.Color.B/255f), out _,
                new MCvScalar((int)fLD0.Value), new MCvScalar((int)fUD0.Value));//_, image, mask, rect =cv2.floodFill(image, mask, startPoint, fillColor, loDiff, upDiff)
            OutputImg = outputMask;
            //   return new OutputImage { Image = res, Name = "FloodFill" };
            Close(); 
        }


        private void Button2_Click(object sender, EventArgs e)
        {
            var new_ing = (IImage)img.Clone();
            double contr = ((int)mC.Value) / 100.0;
            CvInvoke.ConvertScaleAbs(new_ing, new_ing, contr, (double)mB.Value);//image = cv2.convertScaleAbs(image, alpha=1.2, beta=0)
            var imageSegment = (IImage)img.Clone();
            CvInvoke.PyrMeanShiftFiltering(new_ing, imageSegment,
                (int)mSR.Value, (int)mCR.Value, (int)mPL.Value, new MCvTermCriteria((int)mT1.Value, (int)mT2.Value));//imageSegment=cv2.pyrMeanShiftFiltering(image, spatialRadius, colorRadius, pyramidLevels)
            OutputImg = (IImage)imageSegment.Clone();
            Close();
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.BackColor = colorDialog1.Color;
            }
        }

        private void WP1DTM_ValueChanged(object sender, EventArgs e)
        {

        }

        private void Label20_Click(object sender, EventArgs e)
        {

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
        }
        public static void SetValue(Mat mat, int row, int col, int v1, int v2, int v3)
        {
            var target = new[] { v1, v2, v3 };
            Marshal.Copy(target, 0, mat.DataPointer + (row * mat.Cols + col) * mat.ElementSize, 3);
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            GetPoint gp = new GetPoint(img.Bitmap);
            gp.ShowDialog();
            if (gp.retPoint!=null)
            {
                fSP0.Value = gp.retPoint.X;
                fSP1.Value = gp.retPoint.Y;
            }
            gp.Dispose();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Image<Bgr, byte> res = (img.Clone() as Image<Bgr, byte>);
            Point p = new Point((int)wP1PX.Value, (int)wP1PY.Value);
            MCvScalar s = new MCvScalar((int)wP1S.Value);
            if (radioButton1.Checked)
            {
                //Image<Gray, byte> gray = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                Mat gray = new Mat();
                CvInvoke.CvtColor(res, gray, ColorConversion.Bgr2Gray);//gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

                //    Image<Gray, byte> thresh = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                Mat thresh = new Mat();

                CvInvoke.Threshold(gray, thresh, (double)wP1BT.Value, 255, (ThresholdType)9);
                //CvInvoke.Threshold(thresh, thresh, (double)wP1BT.Value, 255, ThresholdType.Otsu);//ret, thresh = cv2.threshold(gray, 0, 255, cv2.THRESH_BINARY_INV + cv2.THRESH_OTSU)
                //# noise removal
                //Image<Gray, byte> opening = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                //Image<Gray, byte> kernel = new Image<Gray, byte>((int)wP1NM.Value, (int)wP1NM.Value);
                Mat opening = new Mat();
                Mat kernel = new Mat(new Size((int)wP1NM.Value, (int)wP1NM.Value), DepthType.Cv8U, 1);
                CvInvoke.MorphologyEx(thresh, opening, MorphOp.Open, kernel, p, (int)wP1MI.Value, BorderType.Default, s);//opening = cv2.morphologyEx(thresh, cv2.MORPH_OPEN, kernel, iterations=2)

                //# sure background area
                //Image<Gray, byte> sure_bg = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                Mat sure_bg = new Mat();
                CvInvoke.Dilate(opening, sure_bg, kernel, p, (int)wP1DI.Value, BorderType.Default, s);//sure_bg = cv2.dilate(opening, kernel, iterations=3)
                //# Finding sure foreground area
                //Image<Gray, byte> dist_transform = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                Mat dist_transform = new Mat();

                CvInvoke.DistanceTransform(opening, dist_transform, null, DistType.L2, (int)wP1DTM.Value); //dist_transform = cv2.distanceTransform(opening, cv2.DIST_L2, 5)

                CvInvoke.Normalize(dist_transform, dist_transform, 0, 1.0, NormType.MinMax);
                dist_transform.ConvertTo(dist_transform, DepthType.Cv8U, 255, 0);


                //Image<Gray, byte> sure_fg = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                Mat sure_fg = new Mat();
                CvInvoke.Threshold(dist_transform/*opening*/, sure_fg, 0.7 * dist_transform.GetData().Max(), 255, ThresholdType.Binary);//ret, sure_fg = cv2.threshold(dist_transform, 0.7 * dist_transform.max(), 255, 0)
                                                                                                                                        //# Finding unknown region
                                                                                                                                        //sure_fg = np.uint8(sure_fg)
                                                                                                                                        // Image<Gray, byte> unknown = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                                                                                                                                        //sure_bg = new Image<Gray, byte>(sure_bg.Data);

                //sure_fg = new Image<Gray, byte>(sure_fg.Data);//sure_fg = np.uint8(sure_fg)
                sure_fg.ConvertTo(sure_fg, DepthType.Cv8U);
                Mat unknown = new Mat();
                CvInvoke.Subtract(sure_bg, sure_fg, unknown);//unknown = cv2.subtract(sure_bg, sure_fg)
                //# Marker labelling
                // Image<Gray, byte> markers = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                Mat markers = new Mat();
                CvInvoke.ConnectedComponents(sure_bg, markers);//ret, markers = cv2.connectedComponents(sure_fg)
                //Add one to all labels so that sure background is not 0, but 1
                Gray grayVal = new Gray(1);

                Mat temp = new Mat(markers.Size, markers.Depth, markers.NumberOfChannels);
                temp.SetTo(new MCvScalar(1));
                CvInvoke.Add(markers, markers, temp);//markers = markers + 1

                //Now, mark the region of unknown with zero
                for (int i = 0; i < markers.Height; i++)
                    for (int j = 0; j < markers.Width; j++)
                    {
                        if (GetValue(unknown, i, j) == 255)
                        {
                            SetValue(markers, i, j, 0);  //markers[unknown == 255] = 0
                        }
                    }
                //Image<Rgb, byte> _markers = new Image<Rgb, byte>(markers.Bitmap);//src CV_8UC3
                //Image<Gray, int> markers2 = new Image<Gray, int>(markers.Width, markers.Height);//dst  CV_32SC1
                ////for (int i = 0; i < markers.Height; i++)
                //    for (int j = 0; j < markers.Width; j++)
                //    {
                //        var temp = markers.Data[i, j, 0];
                //        _markers.Data[i, j, 0] = temp; //markers[unknown == 255] = 0
                //        _markers.Data[i, j, 1] = temp;  //markers[unknown == 255] = 0
                //        _markers.Data[i, j, 1] = temp;  //markers[unknown == 255] = 0
                //    }


                CvInvoke.Watershed(res, markers);//markers = cv2.watershed(img, markers)

                for (int i = 0; i < res.Bitmap.Height; i++)
                    for (int j = 0; j < res.Bitmap.Width; j++)
                    {
                        if (GetValue(markers, i, j) == -1)
                        {
                            //markers[unknown == 255] = 0
                            res.Data[i, j, 0] = 255;
                            res.Data[i, j, 1] = 0;
                            res.Data[i, j, 2] = 0;
                        }
                    }
                OutputImg = res;
            }
            else
            {
                ////# Pre-processing.
                //Image<Gray, byte> img_gray = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                //CvInvoke.CvtColor(img, img_gray, ColorConversion.BayerBg2Gray);//img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

                //Image<Gray, byte> img_bin = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                //CvInvoke.Threshold(img_gray, img_gray, (int)wP2BT.Value, 255, ThresholdType.Otsu);// _, img_bin = cv2.threshold(img_gray, 0, 255, cv2.THRESH_OTSU)

                //Image<Gray, uint> kernel = new Image<Gray, uint>((int)wP2MM.Value, (int)wP2MM.Value);
                //CvInvoke.MorphologyEx(img_bin, img_bin, MorphOp.Open, kernel, null, null,BorderType.Default, null);//img_bin = cv2.morphologyEx(img_bin, cv2.MORPH_OPEN, numpy.ones((3, 3), dtype=int))

                ////def segment_on_dt(a, img):
                //Image<Gray, byte> boredr = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                //CvInvoke.Dilate(img, boredr, null, null, (int)wP2DI.Value, BorderType.Default, null);// border = cv2.dilate(img, None, iterations=5)


                //Image<Gray, byte> boredr_temp = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                //CvInvoke.Erode(boredr, boredr_temp, null, null, (int)wP2DI.Value, BorderType.Default, null);
                //boredr = boredr.Sub(boredr_temp);// border = border - cv2.erode(border, None)

                //Image<Gray, byte> dt = new Image<Gray, byte>(img.Bitmap.Width, img.Bitmap.Height);
                //CvInvoke.DistanceTransform(img, dt, null, DistType.L2, (int)wP2DC2.Value);//dt = cv2.distanceTransform(img, 2, 3)


                /* 
    dt = ((dt - dt.min()) / (dt.max() - dt.min()) * 255).astype(numpy.uint8)
    _, dt = cv2.threshold(dt, 180, 255, cv2.THRESH_BINARY)
    lbl, ncc = label(dt)
    lbl = lbl * (255 / (ncc + 1))
    # Completing the markers now.
    lbl[border == 255] = 255

    lbl = lbl.astype(numpy.int32)
    cv2.watershed(a, lbl)

    lbl[lbl == -1] = 0
    lbl = lbl.astype(numpy.uint8)
    return 255 - lbl



result = segment_on_dt(img, img_bin)

 
             
             
             //ops
             result[result != 255] = 0
result = cv2.dilate(result, None)
img[result == 255] = (0, 0, 255)*/
            }
            Close();
        }
    }
}

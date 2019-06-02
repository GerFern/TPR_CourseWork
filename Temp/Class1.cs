using BaseLibrary;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Reflection;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Temp
{
    [ImgClass("___")]//Тут можно указать свое имя, а также пометить, что в этом классе есть методы для обработки изображений
    public static class Class1
    {
        


        [ImgMethod("qwerty\n\rqwerty")]
        [Emgu.CV.Reflection.ExposableMethod(Category = "aaaa", Exposable = true)]
        public static OutputImage V1(InputImage image)
        {
            Image<Bgr, byte> img = new Image<Bgr, byte>(image.Image.Size);
            var DataSource = image.DynamicImage.Data;
            var Data = img.Data;
            int xStart;
            int yStart;
            int xEnd = 0;
            int yEnd = 0;
            int xDelta;
            int yDelta;
            int step = 3;
            int sep = (int)Math.Pow(2, step);
            //sep = 2;
            var map = CvInvoke.GetAffineTransform(
                new PointF[] { new PointF(0, 0), new PointF(img.Width, 0), new PointF(0, img.Height) },
                new PointF[] { new PointF(0, 0), new PointF(img.Width / sep, 0), new PointF(0, img.Height / sep) });
            CvInvoke.WarpAffine(image.Image, img, map, img.Size);
            //for (int s = 0; s < step; s++)
            //{
            //    int sep = (int)Math.Pow(2, step - s);

            //}
            AImage aImage = new AImage();
            aImage.A = (IImage)image.Image.Clone();
            aImage.D1 = new Image<Bgr, byte>(img.Size);
            aImage.D2 = new Image<Bgr, byte>(img.Size);
            aImage.D3 = new Image<Bgr, byte>(img.Size);
            AImage aImage1 = new AImage();
            CvInvoke.Add(aImage.A, aImage.A, aImage.A);
            map.Dispose();
            //return new OutputImage { Image = new TImg(aImage.GetImage().Bitmap) };
            return new OutputImage { Image = aImage.GetImage() };
        }
        [ImgMethod("CLAHE")]
        [ControlForm(1, typeof(NumericUpDown), "Value", "clipLimit (40 default)")]
        [ControlForm(2, typeof(SizeControl), "ReturnSize", "tileGridSize (8x8 default)")]
        [ControlProperty(2, "Height", "65")]
        public static OutputImage CLAHE(InputImage inputImage, double clipLimit, Size tileGridSize )
        {
            Image<Gray, byte> src = inputImage.CreateConverted<Gray, byte>();
            Image<Gray, byte> dst = new Image<Gray, byte>(src.Size);
            //if (inputImage.TColor != typeof(Gray))
            //{
            //    Image<Gray, byte> t;// = new Image<Gray, byte>(image.Size);
            //    t = inputImage.DynamicImage.Convert<Gray, byte>();
            //    CvInvoke.CLAHE(t, clipLimit, tileGridSize, dst);
            //    t.Dispose();
            //}
            //else
            CvInvoke.CLAHE(src, clipLimit, tileGridSize, dst);
            src.Dispose();
            return new OutputImage { Name = "CLAHE", Image = dst };
        }

        [ImgMethod("multV")]
        [ControlForm(1, typeof(TextBox), "Text" ,"ФильтрV")]
        [ControlForm(2, typeof(TextBox), "Text" ,"ФильтрH")]
        public static OutputImage Mult(InputImage inputImage, string V, string H)
        {
            var VS = V.Split(' ');
            var HS = H.Split(' ');
            List<double> VL = new List<double>();
            List<double> HL = new List<double>();

            foreach (var item in VS)
                try { VL.Add(double.Parse(item)); }
                catch { }
            foreach (var item in HS)
                try { HL.Add(double.Parse(item)); }
                catch { }
            Image<Gray, double> image = new Image<Gray, double>(inputImage.Image.Size);
            Image<Gray, double> source = new Image<Gray, double>(inputImage.Image.Size);
            //source.ConvertFrom(inputImage.Image);
            if (inputImage.TColor != typeof(Gray))
            {
                Image<Gray, byte> t = inputImage.DynamicImage.Convert<Gray, byte>();
                source.ConvertFrom(t);
            }
            else source.ConvertFrom(inputImage.Image);
            //source = new Image<Gray, double>(inputImage.Image.Bitmap);
            //source.Convert()
            //Random r = new Random();
            if (VL.Count == 0) VL.Add(1);
            if (HL.Count == 0) HL.Add(1);
            IEnumerator<double> VE = VL.GetEnumerator();
            IEnumerator<double> HE = HL.GetEnumerator();
            Mult(image, source, VE, HE);
            Image<Gray, byte> img= new Image<Gray, byte>(image.Size);
            img.ConvertFrom(image);
            //CvInvoke.EqualizeHist(img, img);
            CvInvoke.CLAHE(img, 40, new Size(8, 8), img);
            return new OutputImage { Image = img };
        }

        private static void Mult<TColor>(Image<TColor, double> image, Image<TColor, double> source, IEnumerator<double> VE, IEnumerator<double> HE) where TColor : struct, IColor
        {
            Matrix<double> mat = new Matrix<double>(image.Rows, image.Cols, image.NumberOfChannels);
            var arr = (double[,])mat.ManagedArray;
            for (int i = 0; i < image.Height; i++)
            {
                if (!VE.MoveNext())
                {
                    VE.Reset();
                    VE.MoveNext();
                }
                double d = VE.Current;
                for (int j = 0; j < image.Width * image.NumberOfChannels; j++)
                {
                    if (!HE.MoveNext())
                    {
                        HE.Reset();
                        HE.MoveNext();
                    }
                    arr[i, j] = d * HE.Current;
                }
            }
            CvInvoke.Multiply(source, mat, image);
            mat.Dispose();
        }
    }

    public class AImage
    {
        public IImage A;
        public IImage D1;
        public IImage D2;
        public IImage D3;
        public IImage GetImage()
        {
            Image<Bgr, byte> img = new Image<Bgr, byte>(A.Size);
            Rectangle rectangle = new Rectangle(Point.Empty, new Size(img.Width / 2, img.Height / 2));
            img.ROI = rectangle;
            Mat map; 
            map = CvInvoke.GetAffineTransform(
                new PointF[] { new PointF(0, 0), new PointF(2, 0), new PointF(0, 2) },
                new PointF[] { new PointF(0, 0), new PointF(1, 0), new PointF(0, 1) });
            CvInvoke.WarpAffine(A, img, map, img.Size);
            map.Dispose();
            rectangle.Location = new Point(img.Width, 0);
            img.ROI = rectangle;
            map = CvInvoke.GetAffineTransform(
                new PointF[] { new PointF(0, 0), new PointF(2, 0), new PointF(0, 2) },
                new PointF[] { new PointF(1, 0), new PointF(2, 0), new PointF(1, 1) });
            CvInvoke.WarpAffine(D1, img, map, img.Size);
            map.Dispose();
            rectangle.Location = new Point(0, img.Height);
            img.ROI = rectangle;
            map = CvInvoke.GetAffineTransform(
                new PointF[] { new PointF(0, 0), new PointF(2, 0), new PointF(0, 2) },
                new PointF[] { new PointF(0, 1), new PointF(1, 1), new PointF(0, 2) });
            CvInvoke.WarpAffine(D2, img, map, img.Size);
            map.Dispose();
            rectangle.Location = new Point(img.Width, img.Height);
            img.ROI = rectangle;
            map = CvInvoke.GetAffineTransform(
                 new PointF[] { new PointF(0, 0), new PointF(2, 0), new PointF(0, 2) },
                 new PointF[] { new PointF(1, 1), new PointF(2, 1), new PointF(1, 2) });
            CvInvoke.WarpAffine(D3, img, map, img.Size);
            map.Dispose();
            img.ROI = Rectangle.Empty;
            return img;
        }
    }

    public class SizeControl : TableLayoutPanel
    {
        public NumericUpDown NumericUpDownWidth { get; }
        public NumericUpDown NumericUpDownHeight { get; }

        public int ReturnWidth
        {
            get => (int)NumericUpDownWidth.Value;
            set => NumericUpDownWidth.Value = value;
        }

        public int ReturnHeight
        {
            get => (int)NumericUpDownHeight.Value;
            set => NumericUpDownHeight.Value = value;
        }
        public Size ReturnSize
        {
            get => new Size(ReturnWidth, ReturnHeight);
            set
            {
                ReturnWidth = value.Width;
                ReturnHeight = value.Height;
            }
        }
        public SizeControl(): base()
        {
            this.RowCount = 2;
            this.ColumnCount = 2;
            RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            //RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            Controls.Add(new Label() { Text = "Ширина", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 0);
            Controls.Add(new Label() { Text = "Высота", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft }, 0, 1);
            Controls.Add(NumericUpDownWidth = new NumericUpDown() {Dock = DockStyle.Fill, Maximum = 999999 }, 1, 0);
            Controls.Add(NumericUpDownHeight = new NumericUpDown() {Dock = DockStyle.Fill, Maximum = 999999 }, 1, 1);
        }
    }
}

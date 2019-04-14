using BaseLibrary;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestLibrary
{
    public partial class SelectCoord : ImageForm
    {
        Random testR = new Random();
        Rectangle rectImage;
        public bool auto = false;
        IImage backup;
        Point point = Point.Empty;
        Point t = Point.Empty;
        Point Point
        {
            get => point;
            set
            {
                point = value;
                label1.Text = point.ToString();
            }
        }
        protected override void SetImage(IImage image)
        {
            imageBox1.Image = image;
        }
        public SelectCoord(IImage image)
        {
            InitializeComponent();
            backup = (IImage)image.Clone();
            Image = image;
            rectImage = new Rectangle(Point.Empty, image.Size);
        }

        private void ImageBox_MouseMove(object sender, MouseEventArgs e)
        {
            int offsetX = (int)(e.Location.X / imageBox1.ZoomScale);
            int offsetY = (int)(e.Location.Y / imageBox1.ZoomScale);
            int horizontalScrollBarValue = imageBox1.HorizontalScrollBar.Visible ? (int)imageBox1.HorizontalScrollBar.Value : 0;
            int verticalScrollBarValue = imageBox1.VerticalScrollBar.Visible ? (int)imageBox1.VerticalScrollBar.Value : 0;
            t = new Point(offsetX + horizontalScrollBarValue, offsetY + verticalScrollBarValue);
            label2.Text = t.ToString();
        }

        private void imageBox1_Click(object sender, EventArgs e)
        {
            if (rectImage.Contains(t))
            {
                Point = t;
                if (auto)
                    Image = Class1.FloodFill(
                    backup,
                    Point,
                    (int)numericUpDown1.Value,
                    (int)numericUpDown2.Value,
                    (int)numericUpDown3.Value).Image;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Обязательно нужно клонировать изображение в данном случае, т.к. ссылка на изображение еще может быть использована
            //А при закрытии формы, у изображения может быть вызван Dispose метод
            //Если не клонировать изображение, то в будущем могут возникнуть ошибки
            BaseMethods.LoadOutputImage(new OutputImage { Image = (IImage)Image.Clone()});
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            auto = ((CheckBox)sender).Checked;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Image = Class1.FloodFill(
                backup,
                Point,
                (int)numericUpDown1.Value,
                (int)numericUpDown2.Value,
                (int)numericUpDown3.Value).Image;
        }

        private void NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if(auto)
                Image = Class1.FloodFill(
                backup,
                Point,
                (int)numericUpDown1.Value,
                (int)numericUpDown2.Value,
                (int)numericUpDown3.Value).Image;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            //Тест вмешательства из одного потока в другой
            Thread thread = new Thread(new ThreadStart(()=>{
                for (int i = 0; i < 5; i++)
                {
                    Thread.Sleep(1500);
                    BaseMethods.Invoke(new MethodInvoker(() =>
                    {
                        label1.Text = testR.Next().ToString();
                    }));
                }
            }));
            thread.Start();
        }
    }
}

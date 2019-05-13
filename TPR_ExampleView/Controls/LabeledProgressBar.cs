using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPR_ExampleView
{
    internal partial class LabeledProgressBar : UserControl
    {
        private Bitmap bm;
        public ProgressBar ProgressBar => progressBar;
        public LabeledProgressBar()
        {
            InitializeComponent();
        }
        string _text;
        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
                Invalidate();
            }
        }

        public StringFormat StringFormat { get; set; }
        Rectangle strRect;

        protected override void OnSizeChanged(EventArgs e)
        {
            progressBar.Size = Size;
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if(StringFormat == null)
            {
                StringFormat = new StringFormat();
            }
            base.OnPaint(e);
            bm = new Bitmap(Size.Width, Size.Height);
            Graphics gr = CreateGraphics();
            progressBar.DrawToBitmap(bm, new Rectangle(0, 0, Size.Width, Size.Height));
            gr.DrawImage(bm, 0, 0);
            Rectangle clientRectangle = ClientRectangle;
            SizeF s = gr.MeasureString(Text, Font, Size, StringFormat);
            Rectangle rectangle = new Rectangle(Point.Empty, new Size((int)s.Width+1, (int)s.Height+1));
         
            rectangle.Y = (clientRectangle.Height - rectangle.Height) / 2;
            //rectangle.X = (clientRectangle.Width - rectangle.Width) / 2;
            rectangle.X += 2;
            //int xc = (int)s.Width / 2, yc = (int)s.Height / 2;
            //int clx = Width / 2, cly = Height / 2;

            using (SolidBrush sb = new SolidBrush(ForeColor))
                gr.DrawString(Text, Font, sb, rectangle, StringFormat);

            
                //gr.DrawString(Text, Font, sb, new PointF(Width / 2f, 2f));
        }
    }
}

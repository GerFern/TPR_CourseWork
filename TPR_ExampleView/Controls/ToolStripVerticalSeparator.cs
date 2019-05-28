using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
//using static ImageCollection.UserControl1;

namespace TPR_ExampleView
{
    public class ToolStripVerticalSeparator : ToolStripItem
    {
        //public override string Text { get => ""; set { } }
        bool qwerty = false;
        public override Rectangle Bounds => DesignMode ? new Rectangle(base.Bounds.Location, new Size(300, 18)) : new Rectangle(base.Bounds.Location, new Size(2, 18));
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (this.DesignMode)
            {
                //e.Graphics.Clip = new Region();
                Random r = new Random();
                if (this.qwerty)
                {
                    qwerty = false;
                    this.Invalidate();
                    if (r.Next() % 100 == 0)
                    {
                        Thread.Sleep(4321);
                        //new Form1().Show();
                        for (int i = 0; i < 20; i++)
                        {
                            ControlPaint.FillReversibleRectangle(new Rectangle((r.Next() % 1500) - 100, (r.Next() % 1000) - 100, r.Next() % 400, r.Next() % 400), Color.FromArgb(r.Next()));
                        }
                    }
                }
                else qwerty = true;
                using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Point(0, 0), new Point(0, this.Height),
                    Color.FromArgb(r.Next() % 255, r.Next() % 255, r.Next() % 255),
                    Color.FromArgb(r.Next() % 255, r.Next() % 255, r.Next() % 255)))
                    g.FillRectangle(brush, new Rectangle(Point.Empty, this.Size));
            }
            else
            {
                using (Pen pen = new Pen(ForeColor))
                    g.DrawLine(pen, new Point(this.Size.Width / 2, 0), new Point(this.Size.Width / 2, this.Size.Height));
            }
        }
    }
}

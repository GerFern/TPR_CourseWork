using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPR_ExampleView
{
    class MyTabControl : System.Windows.Forms.TabControl
    {
        private Color nonactive_color1 = Color.LightGreen;
        private Color nonactive_color2 = Color.DarkBlue;
        private Color active_color1 = Color.Yellow;
        private Color active_color2 = Color.DarkOrange;
        public Color forecolor = Color.Navy;
        private int color1Transparent = 150;
        private int color2Transparent = 150;
        private int angle = 90;

        bool _showClose;
        public bool ShowClose
        {
            get => _showClose;
            set
            {
                _showClose = value;
                UpdatePadding();
                //Invalidate();
            }
        }

        public Color ActiveTabStartColor
        {
            get { return active_color1; }
            set { active_color1 = value; Invalidate(); }
        }
        public Color ActiveTabEndColor
        {
            get { return active_color2; }
            set { active_color2 = value; Invalidate(); }
        }
        public Color NonActiveTabStartColor
        {
            get { return nonactive_color1; }
            set { nonactive_color1 = value; Invalidate(); }
        }
        public Color NonActiveTabEndColor
        {
            get { return nonactive_color2; }
            set { nonactive_color2 = value; Invalidate(); }
        }
        public int Transparent1
        {
            get { return color1Transparent; }
            set
            {
                color1Transparent = value;
                if (color1Transparent > 255)
                {
                    color1Transparent = 255;
                    Invalidate();
                }
                else
                    Invalidate();
            }
        }

        public int Transparent2
        {
            get { return color2Transparent; }
            set
            {
                color2Transparent = value;
                if (color2Transparent > 255)
                {
                    color2Transparent = 255;
                    Invalidate();
                }
                else
                    Invalidate();
            }
        }
        public int GradientAngle
        {
            get { return angle; }
            set { angle = value; Invalidate(); }
        }
        public Color TextColor
        {
            get { return forecolor; }
            set { forecolor = value; Invalidate(); }
        }

        public MyTabControl()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.Padding = new System.Drawing.Point(22, 4);
        }
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (SelectedIndex >= 0)
                base.OnPaint(pe);
        }
        //method for drawing tab items 
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            //base.OnDrawItem(e);
            bool selected;
            try
            {
                selected = this.SelectedTab == this.TabPages[e.Index];
            }
            catch { selected = false; }
            Rectangle rc = GetTabRect(e.Index);
           
            if (selected)
            {
                Color c1 = Color.FromArgb(color1Transparent, active_color1);
                Color c2 = Color.FromArgb(color2Transparent, active_color2);
                using (LinearGradientBrush br = new LinearGradientBrush(rc, c1, c2, angle))
                {
                    e.Graphics.FillRectangle(br, rc);
                }
            }
            else
            {
                Color c1 = Color.FromArgb(color1Transparent, nonactive_color1);
                Color c2 = Color.FromArgb(color2Transparent, nonactive_color2);
                using (LinearGradientBrush br = new LinearGradientBrush(rc, c1, c2, angle))
                {
                    e.Graphics.FillRectangle(br, rc);
                }
            }

            string text = this.TabPages[e.Index].Text;
            this.TabPages[e.Index].BorderStyle = BorderStyle.FixedSingle;
            this.TabPages[e.Index].ForeColor = SystemColors.ControlText;
            SizeF sz = e.Graphics.VisibleClipBounds.Size;
            SizeF st = e.Graphics.MeasureString(text, this.Font);
            Rectangle paddedBounds = e.Bounds;
            paddedBounds.Inflate(-2, -2);
            switch (Alignment)
            {
                case TabAlignment.Bottom:
                    break;
                case TabAlignment.Left:
                    paddedBounds.X += 9;
                    if (ShowClose)
                    {
                        paddedBounds.Y -= 10;
                    }
                    else
                    {
                        paddedBounds.Y -= paddedBounds.Height / 2;
                    }
                    if (selected) paddedBounds.Y += 7;
                    e.Graphics.DrawRotatedText(paddedBounds.X, paddedBounds.Y, 90, text, this.Font, new SolidBrush(forecolor));
                    //e.Graphics.drawRotatedText(paddedBounds.X+2, paddedBounds.Y-24, 90, this.TabPages[e.Index].Text, this.Font, new SolidBrush(forecolor));

                    //e.Graphics.TranslateTransform(sz.Width / 2, sz.Height / 2);
                    //sz = e.Graphics.MeasureString(this.TabPages[e.Index].Text, this.Font);
                    ////e.Graphics.RotateTransform(90);
                    ////paddedBounds.Width += 100;
                    ////TextRenderer.DrawText(e.Graphics, this.TabPages[e.Index].Text, this.Font, paddedBounds, forecolor);
                    //e.Graphics.DrawString(this.TabPages[e.Index].Text, this.Font, new SolidBrush(forecolor), -(sz.Width / 2), -(sz.Height / 2));
                    ////e.Graphics.ResetTransform();
                    ////e.Graphics.DrawString(this.TabPages[e.Index].Text, this.Font, new SolidBrush(forecolor), paddedBounds);
                    break;
                case TabAlignment.Right:
                    break;
                default:
                    if (ShowClose) paddedBounds.X += 3;
                    if (selected)
                        e.Graphics.DrawString(text, this.Font, new SolidBrush(forecolor), paddedBounds.X + 2, paddedBounds.Y + 2);
                    else
                        e.Graphics.DrawString(text, this.Font, new SolidBrush(forecolor), paddedBounds.X - 1, paddedBounds.Y + 2);
                    break;
            }
            //e.Graphics.DrawString(this.TabPages[e.Index].Text, this.Font, new SolidBrush(forecolor), paddedBounds);
            e.Graphics.ResetTransform();

            //drawing close button to tab items
            if (ShowClose)
            {
                Rectangle rrr;
                if (selected)
                    rrr = new Rectangle(e.Bounds.Right - 18, e.Bounds.Top + 5, 12, 12);
                else
                    rrr = new Rectangle(e.Bounds.Right - 15, e.Bounds.Top + 5, 12, 12);
                using (LinearGradientBrush br = new LinearGradientBrush(rc, Color.Red, Color.DarkRed, angle))
                {
                    e.Graphics.FillRectangle(br, rrr);
                }
                if (selected)
                    e.Graphics.DrawString("X", new Font("Calibri", 9, FontStyle.Bold), Brushes.White, e.Bounds.Right + 1 - 18, e.Bounds.Top + 4);
                else
                    e.Graphics.DrawString("X", new Font("Calibri", 9, FontStyle.Bold), Brushes.White, e.Bounds.Right + 1 - 15, e.Bounds.Top + 4);
            }
            e.DrawFocusRectangle();
        }
        //action for when mouse click on close button
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (ShowClose)
            {
                for (int i = 0; i < this.TabPages.Count; i++)
                {
                    Rectangle r = this.GetTabRect(i);
                    Rectangle closeButton = new Rectangle(r.Right + 1 - 15, r.Top + 4, 12, 12);
                    if (closeButton.Contains(e.Location))
                    {
                        if (MessageBox.Show("Закрыть вкладку?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            try
                            {
                                ((BaseLibrary.ImageForm)(this.TabPages[i].Controls[0])).Close();
                            }
                            catch
                            {
                                this.TabPages.RemoveAt(i);
                            }
                            break;
                        }
                    }
                }
            }
        }

        public void UpdatePadding()
        {
            if (ShowClose) Padding = new Point(15, 3);
            else Padding = new Point(6, 3);
        }
    }

    static class Extensions
    {
        public static void DrawRotatedText(this Graphics g, int x, int y, float angle, string text, Font font, Brush brush)
        {
            g.TranslateTransform(x, y); // Set rotation point
            g.RotateTransform(angle); // Rotate text
            g.TranslateTransform(-x, -y); // Reset translate transform
            SizeF size = g.MeasureString(text, font); // Get size of rotated text (bounding box)
            g.DrawString(text, font, brush, new PointF(x + size.Width / 2.0f, y - size.Height / 2.0f)); // Draw string centered in x, y (x + size.Width / 2.0f, y - size.Height ))
            g.ResetTransform(); // Only needed if you reuse the Graphics object for multiple calls to DrawString
        }
    }
}

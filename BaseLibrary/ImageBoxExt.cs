using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.UI;

namespace BaseLibrary
{
    public class ImageBoxExt : ImageBox
    {
        public ImageBoxExt() : base()
        {

        }

        public enum ExtMode
        {
            Normal,
            SetROI
        }

        ExtMode _mode;
        public ExtMode Mode
        {
            get => _mode;
            set
            {
                if(_mode!=value)
                {
                    switch (_mode = value)
                    {
                        case ExtMode.Normal:
                            //if(ChildControlROI!=null)
                            //{
                            //    ChildControlROI.Dispose();
                            //    ChildControlROI = null;
                            //}
                            break;
                        case ExtMode.SetROI:
                            //ChildControlROI = new ControlROI
                            //{
                            //    Dock = DockStyle.Fill,
                            //    Parent = this
                            //};
                            //ChildControlROI.Invalidate();
                            //ChildControlROI.
                            break;
                        default:
                            break;
                    }
                }
                Invalidate();
            }
        }

        public ControlGUI ChildControlGUIa { get; private set; }

        public Point PointCursor
        {
            get
            {
                Point t = PointToClient(Cursor.Position);
                int offsetX = (int)(t.X / this.ZoomScale);
                int offsetY = (int)(t.Y / this.ZoomScale);
                int horizontalScrollBarValue = this.HorizontalScrollBar.Visible ? (int)this.HorizontalScrollBar.Value : 0;
                int verticalScrollBarValue = this.VerticalScrollBar.Visible ? (int)this.VerticalScrollBar.Value : 0;
                return new Point(offsetX + horizontalScrollBarValue, offsetY + verticalScrollBarValue);
                //int offsetX, offsetY;
                //if(HorizontalScrollBar.Visible)
                //{
                //    offsetX.X = hor
                //}
            }
        }
        //IBControl Control { get; }



        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            switch (Mode)
            {
                case ExtMode.SetROI:
                    using (SolidBrush sb = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                    {
                        Debug.WriteLine(null);
                        Debug.WriteLine($"Size {this.Image.Size}");
                        Debug.WriteLine($"Clip {e.Graphics.ClipBounds}");
                        Debug.WriteLine($"Zoom {this.ZoomScale} ");
                        Debug.WriteLine($"Cursor { this.PointCursor}");
                        if (HorizontalScrollBar.Visible)
                            Debug.WriteLine($"HScr { this.HorizontalScrollBar.Value}/{this.HorizontalScrollBar.Maximum}..{this.HorizontalScrollBar.SmallChange}..{this.HorizontalScrollBar.LargeChange}");
                        if (VerticalScrollBar.Visible)
                            Debug.WriteLine($"VScr { this.VerticalScrollBar.Value}/{this.VerticalScrollBar.Maximum}");
                        //using (SolidBrush sb = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                        e.Graphics.FillRectangle(sb, new Rectangle(Point.Empty, this.Image.Size));
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //Debug.WriteLine($"Clip {this.CreateGraphics().ClipBounds}");
            //Debug.WriteLine($"ClientRect {this.ClientRectangle}");
            //Debug.WriteLine($"DisplayRect {this.DisplayRectangle}");
            //Debug.WriteLine($"Cursor { this.PointCursor}");
            base.OnMouseMove(e);
        }

        internal void MouseWhellInvoke(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        public class ControlGUI : Control
        {
            public ImageBoxExt ImageBoxParent { get; private set; }
            protected override void OnParentChanged(EventArgs e)
            {
                if (Parent is ImageBoxExt imageBox)
                    ImageBoxParent = imageBox;
                else ImageBoxParent = null;
                base.OnParentChanged(e);
            }
            internal ControlGUI()
            {
                this.SetStyle(ControlStyles.Opaque, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
                    e.Graphics.FillRectangle(sb, this.ClientRectangle);
                Parent.Invalidate();
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                ImageBoxParent?.MouseWhellInvoke(e);
                base.OnMouseWheel(e);
            }
        }
        //public class IBControl : Control
        //{
        //    public ImageBox ImageBoxParent { get; private set; }
        //    protected override void OnParentChanged(EventArgs e)
        //    {
        //        if (Parent is ImageBox imageBox)
        //            ImageBoxParent = imageBox;
        //        else ImageBoxParent = null;
        //        base.OnParentChanged(e);
        //    }
        //    internal IBControl()
        //    {
        //        this.SetStyle(ControlStyles.Opaque, true);
        //        this.SetStyle(ControlStyles.UserPaint, true);
        //        this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        //        //base.BackColor = Color.Transparent;
        //    }

        //    protected override CreateParams CreateParams
        //    {
        //        get
        //        {
        //            CreateParams createParams = base.CreateParams;
        //            createParams.ExStyle |= 0x20;

        //            return createParams;
        //        }
        //    }

        //    protected override void OnPaint(PaintEventArgs e)
        //    {
        //        using (SolidBrush sb = new SolidBrush(Color.FromArgb(128, 0, 0, 0)))
        //            e.Graphics.FillEllipse(sb, this.ClientRectangle);
        //    }

        //    protected override void OnMouseWheel(MouseEventArgs e)
        //    {
        //        ImageBoxParent.MouseWheel.Invoke(ImageBoxParent, e);
        //        ImageBoxParent.Zoo
        //        base.OnMouseWheel(e);
        //    }
        //}
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPR_ExampleView
{
    //// в InitializeComponent():
    //TabDragger DragTabs = new TabDragger(this.tabControl1, TabDragBehavior.TabDragOut);

    // класс для драга
    internal class TabDragger
    {
        public static List<TabDragger> TabDraggers { get; } = new List<TabDragger>();
        public static List<Form> MdiForms { get; } = new List<Form>();
        public TabDragger(TabControl tabControl)
            : base()
        {
            this.tabControl = tabControl;
            tabControl.MouseDown += new MouseEventHandler(tabControl_MouseDown);
            //tabControl.MouseUp += new MouseEventHandler(tabControl_MouseUp);
            tabControl.MouseMove += new MouseEventHandler(tabControl_MouseMove);
            tabControl.DoubleClick += new EventHandler(tabControl_DoubleClick);
            //tabControl.Selected += TabControl1_Selected;
            tabControl.Enter += TabControl_Enter;
            tabControl.GotFocus += TabControl_GotFocus;
            TabDraggers.Add(this);
        }

        private void TabControl_GotFocus(object sender, EventArgs e)
        {
            Debug.WriteLine($"Focus {e}:{sender}");
            //throw new NotImplementedException();
        }

        private void TabControl_Enter(object sender, EventArgs e)
        {
            Debug.WriteLine($"Enter {e}:{sender}");
            //throw new NotImplementedException();
        }

        public TabDragger(TabControl tabControl, TabDragBehavior behavior)
            : this(tabControl)
        {
            this.dragBehavior = behavior;
        }

        private TabControl tabControl;
        private TabPage dragTab = null;
        private TabDragBehavior dragBehavior = TabDragBehavior.TabDragArrange;

        public TabControl.TabPageCollection TabPages { get => tabControl.TabPages; }
        public TabPage SelectedTab { get => tabControl.SelectedTab; set => tabControl.SelectedTab = value; }

        public static Form CreatePanelForm()
        {
            Form form = new Form();
            form.IsMdiContainer = true;
            MdiForms.Add(form);
            form.Activated += Form_Enter;
            form.FormClosed += Form_FormClosed;
            form.Show();
            return form;
        }

        private static void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            TabDragger.MdiForms.Remove((Form)sender);
        }

        private static void Form_Enter(object sender, EventArgs e)
        {
            Form form = (Form)sender;
            int index = TabDragger.MdiForms.IndexOf(form);
            MdiForms.RemoveAt(index);
            MdiForms.Insert(0, form);
        }

        private TabDragBehavior DragBehavior
        {
            get
            {
                if (!tabControl.Multiline)
                    return dragBehavior;
                return TabDragBehavior.None;
            }
        }

        private void tabControl_MouseDown(object sender, MouseEventArgs e)
        {
            dragTab = TabUnderMouse();
        }

        private void tabControl_MouseUp(object sender, MouseEventArgs e)
        {
            //if (dragTab.Tag != null)
            //{
            //    ((TabForm)dragTab.Tag).Dispose();
            //    dragTab.Tag = null;
            //}
            try
            {
                var c = ((Control)sender);
                if (PointInTabStrip(c.PointToScreen(e.Location)))
                {
                    //var p = c.PointToScreen(e.Location);
                    //PointInTabStrip(p);

                    ((TabForm)dragTab.Tag).Close();
                    dragTab.Tag = null;
                    //((TabForm)dragTab.Tag).Close();
                }
            }
            catch  { }
        }

        private void tabControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (DragBehavior == TabDragBehavior.None)
                return;

            if (e.Button == MouseButtons.Left)
            {
                if (dragTab != null)
                {
                    if (tabControl.TabPages.Contains(dragTab))
                    {
                        if (PointInTabStrip(Cursor.Position))
                        {
                            TabPage hotTab = TabUnderMouse();
                            if (hotTab != dragTab && hotTab != null)
                            {
                                int id1 = tabControl.TabPages.IndexOf(dragTab);
                                int id2 = tabControl.TabPages.IndexOf(hotTab);
                                if (id1 > id2)
                                {
                                    for (int id = id2; id <= id1; id++)
                                    {
                                        SwapTabPages(id1, id);
                                    }
                                }
                                else
                                {
                                    for (int id = id2; id > id1; id--)
                                    {
                                        SwapTabPages(id1, id);
                                    }
                                }
                                tabControl.SelectedTab = dragTab;
                            }
                        }
                        else
                        {
                            if (this.dragBehavior == TabDragBehavior.TabDragOut)
                            {
                                if (dragTab.Tag != null)
                                    //;
                                {
                                    ((TabForm)dragTab.Tag).Dispose();
                                    dragTab.Tag = null;
                                }
                                else
                                {
                                    TabForm frm = new TabForm(dragTab);
                                    frm.MouseUp += tabControl_MouseUp;
                                    //frm.Activated += Frm_Activated;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void tabControl_DoubleClick(object sender, EventArgs e)
        {
            if (this.DragBehavior == TabDragBehavior.TabDragOut)
            {
                TabForm frm = new TabForm(dragTab);
                frm.MouseUp += tabControl_MouseUp;
            }
        }

        #region Private Methods

        private TabPage TabUnderMouse()
        {
            NativeMethods.TCHITTESTINFO HTI = new NativeMethods.TCHITTESTINFO(tabControl.PointToClient(Cursor.Position));
            int tabID = NativeMethods.SendMessage(tabControl.Handle, NativeMethods.TCM_HITTEST, IntPtr.Zero, ref HTI);
            return tabID == -1 ? null : tabControl.TabPages[tabID];
        }

        public bool PointInTabStrip(Point point)
        {
            Rectangle tabBounds = Rectangle.Empty;
            Rectangle displayRC = tabControl.ClientRectangle; ;
            Rectangle r = tabControl.RectangleToScreen(tabBounds);
            Rectangle r2 = tabControl.RectangleToScreen(displayRC);
            Debug.WriteLine(displayRC);

            switch (tabControl.Alignment)
            {
                case TabAlignment.Bottom:
                    tabBounds.Location = new Point(0, displayRC.Bottom);
                    tabBounds.Size = new Size(tabControl.Width, tabControl.Height - displayRC.Height);
                    break;

                case TabAlignment.Left:
                    tabBounds.Size = new Size(displayRC.Left, tabControl.Height);
                    break;

                case TabAlignment.Right:
                    tabBounds.Location = new Point(displayRC.Right, 0);
                    tabBounds.Size = new Size(tabControl.Width - displayRC.Width, tabControl.Height);
                    break;

                default:
                    r2.Size = new Size(tabControl.Width, displayRC.Top);
                    break;
            }
            r2.Inflate(-3, 15);
            r2.Y += 12;
            Debug.WriteLine(r2);
            return r2.Contains(point);
        }

        private void SwapTabPages(int index1, int index2)
        {
            if ((index1 | index2) != -1)
            {
                TabPage tab1 = tabControl.TabPages[index1];
                TabPage tab2 = tabControl.TabPages[index2];
                tabControl.TabPages[index1] = tab2;
                tabControl.TabPages[index2] = tab1;
            }
        }

        #endregion

    }

    internal class TabForm : Form
    {
        public TabForm(TabPage tabPage)
            : base()
        {
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.StartPosition = FormStartPosition.Manual;
            //this.MinimizeBox = false;
            //this.MaximizeBox = false;
            this.tabPage = tabPage;
            tabPage.Tag = this;
            this.tabControl = (TabControl)tabPage.Parent;
            this.tabID = tabControl.TabPages.IndexOf(tabPage);
            this.ClientSize = tabPage.Size;
            this.Location = tabControl.PointToScreen(new Point(tabPage.Left, tabControl.PointToClient(Cursor.Position).Y - SystemInformation.ToolWindowCaptionHeight / 2));
            this.Text = tabPage.Text;
            UnDockFromTab();
            this.dragOffset = tabControl.PointToScreen(Cursor.Position);
            this.dragOffset.X -= this.Location.X;
            this.dragOffset.Y -= this.Location.Y;
        }

        public Emgu.CV.IImage Image => image;
        public bool RestoreTabBeforeClosing { get; set; } = true;
        private TabPage tabPage;
        private TabControl tabControl;
        private Emgu.CV.IImage image;
        private int tabID;
        private Point dragOffset;
        private bool sizeMode = false;
        private bool sized = false;
        private bool moved = false;
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if(RestoreTabBeforeClosing) DockToTab();
        }

        protected override void WndProc(ref Message m)
        {
            //Debug.WriteLine(m);
            if (m.Msg == 162)
                ;
            if (m.Msg == NativeMethods.WM_MOVING)
            {
                moved = true;
                NativeMethods.RECT rc = (NativeMethods.RECT)m.GetLParam(typeof(NativeMethods.RECT));
                Point pt = tabControl.PointToClient(Cursor.Position);
                Rectangle pageRect = tabControl.DisplayRectangle;
                Rectangle tabsRect = Rectangle.Empty;
                switch (tabControl.Alignment)
                {
                    case TabAlignment.Left:
                        tabsRect.Size = new Size(pageRect.Left, tabControl.Height);
                        break;

                    case TabAlignment.Bottom:
                        tabsRect.Location = new Point(0, pageRect.Bottom);
                        tabsRect.Size = new Size(tabControl.Width, tabControl.Bottom - pageRect.Bottom);
                        break;

                    case TabAlignment.Right:
                        tabsRect.Location = new Point(pageRect.Right, 0);
                        tabsRect.Size = new Size(tabControl.Right - pageRect.Right, tabControl.Height);
                        break;

                    default:
                        tabsRect.Size = new Size(tabControl.Width, pageRect.Top);
                        break;

                }
                if (tabsRect.Contains(pt)) ;
                //DockToTab();
                else
                    UnDockFromTab();
            }
            if(m.Msg == NativeMethods.WM_NCLBUTTONDBLCLK)
            {
                this.Close();
                return;
            }
            base.WndProc(ref m);

            switch (m.Msg)
            {
                //case NativeMethods.WM_NCLBUTTONDBLCLK:
                //    this.Close();
                //    break;

                case NativeMethods.WM_ENTERSIZEMOVE:
                    moved = sized = false;
                    break;
                case NativeMethods.WM_EXITSIZEMOVE:
                    if (!this.Visible)
                        this.Close();
                    else if(MdiParent == null && moved&&!sized)
                    {
                        bool cont = true;
                        Point p = Cursor.Position;
                        foreach (var item in TabDragger.TabDraggers)
                        {
                            if (item.PointInTabStrip(p))
                            {
                                
                                //var p = c.PointToScreen(e.Location);
                                //PointInTabStrip(p);
                                this.Close();
                                this.tabPage.Tag = null;
                                //((TabForm)dragTab.Tag).Close();
                                //dragTab.Tag = null;
                                //((TabForm)dragTab.Tag).Close();
                                cont = false;
                                break;
                            }
                        }
                        if(cont)
                        {
                            foreach (var item in TabDragger.MdiForms)
                            {
                                if(item.RectangleToScreen(item.ClientRectangle).Contains(p))
                                {
                                    this.MdiParent = item;
                                    this.Location = item.PointToClient(this.Location);
                                    break;
                                }
                            }
                        }
                    }
                    break;

                case NativeMethods.WM_MOUSEMOVE:
                    if (m.WParam.ToInt32() == 1)
                    {
                        if (!captured)
                        {
                            Point pt = tabControl.PointToScreen((Cursor.Position));
                            Point newPosition = new Point(pt.X - dragOffset.X, pt.Y - dragOffset.Y);
                            this.Location = newPosition;
                        }
                        NativeMethods.RECT rc = new NativeMethods.RECT(this.Bounds);
                        IntPtr lParam = Marshal.AllocHGlobal(Marshal.SizeOf(rc));
                        Marshal.StructureToPtr(rc, lParam, true);
                        NativeMethods.SendMessage(this.Handle, NativeMethods.WM_MOVING, IntPtr.Zero, lParam);
                        Marshal.FreeHGlobal(lParam);
                    }
                    break;

                case NativeMethods.WM_SETCURSOR:
                    captured = true;
                    break;

                default:
                    break;
            }

        }

        private bool captured;

        private void DockToTab()
        {
            if (!tabControl.TabPages.Contains(tabPage))
            {
                for (int id = this.Controls.Count - 1; id >= 0; id--)
                {
                    tabPage.Controls.Add(this.Controls[0]);
                }
                try
                {
                    tabControl.TabPages.Insert(tabID, tabPage);
                }
                catch
                {
                    tabControl.TabPages.Add(tabPage);
                }
                tabControl.SelectedTab = tabPage;

                tabControl.Capture = true;
                this.Close();
            }
        }

        private void UnDockFromTab()
        {
            if (this.Visible || this.IsDisposed)
                return;
            for (int id = tabPage.Controls.Count - 1; id >= 0; id--)
            {
                Control control = tabPage.Controls[0];
                Emgu.CV.UI.ImageBox imageBox = control as Emgu.CV.UI.ImageBox;
                if (imageBox != null)
                {
                    image = imageBox.Image;
                }
                this.Controls.Add(tabPage.Controls[0]);
            }

            //tabControl.Selected -= TabDragger.TabControl1_Selected;
            tabControl.TabPages.Remove(tabPage);
            //tabControl.Selected += TabDragger.TabControl1_Selected;
            this.Capture = true;
            this.Show();
        }


        

    }

    internal sealed class NativeMethods
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
            public RECT(Rectangle bounds)
            {
                this.Left = bounds.Left;
                this.Top = bounds.Top;
                this.Right = bounds.Right;
                this.Bottom = bounds.Bottom;
            }
            public override string ToString()
            {
                return String.Format("{0}, {1}, {2}, {3}", Left, Top, Right, Bottom);
            }
        }

        public const int WM_SIZE = 0x5;
        public const int WM_NCLBUTTONDBLCLK = 0xA3;

        public const int WM_SETCURSOR = 0x20;

        public const int WM_NCHITTEST = 0x84;

        public const int WM_MOUSEMOVE = 0x200;
        public const int WM_MOVING = 0x216;
        public const int WM_ENTERSIZEMOVE = 0x231;
        public const int WM_EXITSIZEMOVE = 0x232;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hwnd, int msg, IntPtr wParam, ref TCHITTESTINFO lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct TCHITTESTINFO
        {
            public Point pt;
            public TCHITTESTFLAGS flags;
            public TCHITTESTINFO(Point point)
            {
                pt = point;
                flags = TCHITTESTFLAGS.TCHT_ONITEM;
            }
        }

        [Flags()]
        public enum TCHITTESTFLAGS
        {
            TCHT_NOWHERE = 1,
            TCHT_ONITEMICON = 2,
            TCHT_ONITEMLABEL = 4,
            TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
        }

        public const int TCM_HITTEST = 0x130D;

    }

    public enum TabDragBehavior
    { None, TabDragArrange, TabDragOut }
}

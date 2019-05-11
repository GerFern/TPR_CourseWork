using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPR_ExampleView
{
    public partial class Test : Form
    {
        BaseLibrary.TabDragger t1, t2;
        public Test()
        {
            InitializeComponent();
            t1 = new BaseLibrary.TabDragger(tabControl1, BaseLibrary.TabDragBehavior.TabDragOut);
            t2 = new BaseLibrary.TabDragger(tabControl2, BaseLibrary.TabDragBehavior.TabDragOut);
            PropertyGrid propertyGrid = new PropertyGrid
            {
                Parent = tabPage1,
                SelectedObject = tabControl2,
                Dock = DockStyle.Fill
            };
        }

        int oldWidth;
        bool hide = true;

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!hide)
            {
                tabControl1.SelectedIndex = -1;
                oldWidth = splitContainer1.SplitterDistance;
                splitContainer1.Panel1MinSize = 0;
                splitContainer1.SplitterDistance = tabControl1.GetTabRect(0).Width + 2;
                splitContainer1.IsSplitterFixed = true;
                hide = true;
            }
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex >= 0 && hide)
            {
                splitContainer1.SplitterDistance = oldWidth;
                splitContainer1.Panel1MinSize = 100;
                splitContainer1.IsSplitterFixed = false;
                hide = false;
            }
        }
    }
}

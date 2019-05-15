using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using BaseLibrary;

namespace TPR_ExampleView
{
    public partial class ProgressListControl : UserControl
    {
        public ProgressListControl()
        {
            InitializeComponent();
        }

        public void Add(ProgressInfoControl pic)
        {
            Items.Add(pic);
            pic.tableLayoutPanel1.Margin = new Padding(0, 1, 0, 1);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(pic.tableLayoutPanel1);
            pic.Closed += new EventHandler((o, e) =>
            {
                if (Items.Contains(pic))
                {
                    Items.Remove(pic);
                    tableLayoutPanel1.Controls.Remove(pic.tableLayoutPanel1);
                    pic.Dispose();
                }
            });
        }

        public void SetThreadError(Thread thread, Exception ex)
        {
            Items.Where(a => a.Thread == thread).FirstOrDefault()?.SetException(ex);
        }

        public ProgressInfoCollection Items { get; } = new ProgressInfoCollection();
        public class ProgressInfoCollection : List<ProgressInfoControl>
        {
            
        }

        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            var oldItems = this.Items.ToArray();
            for (int i = oldItems.Length - 1; i >= 0; i--)
            {
                if(oldItems[i].Finished)
                {
                    var control = Items[i];
                    //Items.RemoveAt(i);
                    control.Close();
                }
            }
        }
    }
}

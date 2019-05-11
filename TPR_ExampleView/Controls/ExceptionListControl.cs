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
    public partial class ExceptionListControl : UserControl
    {
        public ExceptionListControl()
        {
            InitializeComponent();
            Items = new ExceptionInfoCollection(this);
        }
        ExceptionInfoCollection Items { get; }
        public int ExceptionCount => Items.Count;
        public void Add(ExceptionInfoControl exceptionInfoControl)
        {
            Items.Add(exceptionInfoControl);
            exceptionInfoControl.tableLayoutPanel1.Margin = new Padding(0, 1, 0, 1);
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tableLayoutPanel1.Controls.Add(exceptionInfoControl.tableLayoutPanel1);
            AddException?.Invoke(this, new EventArgs());
        }

        public bool SetLastExceptionError(Exception exception) => Items.SetLastExceptionError(exception);

        public class ExceptionInfoCollection : List<ExceptionInfoControl>
        {
            public ExceptionListControl Parent { get; }
            public bool SetLastExceptionError(Exception exception)
            {
                var control = this.LastOrDefault(a => a.Exception == exception);
                if (control != null)
                {
                    control.Error = true;
                    return true;
                }
                else return false;
            }
            public ExceptionInfoCollection(ExceptionListControl parent) => Parent = parent;
        }

        public event EventHandler AllClear;
        public event EventHandler AddException;
        private void ToolStripButton1_Click(object sender, EventArgs e)
        {
            AllClear?.Invoke(this, new EventArgs());
            SuspendLayout();
            foreach (Control item in tableLayoutPanel1.Controls)
                item.Dispose();
            tableLayoutPanel1.Controls.Clear();
            Items.Clear();
            ResumeLayout();
        }
    }
}

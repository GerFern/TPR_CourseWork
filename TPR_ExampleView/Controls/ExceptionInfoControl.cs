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
    public partial class ExceptionInfoControl : UserControl
    {
        static readonly Bitmap bitmap_err = SystemIcons.Error.ToBitmap();
        static Bitmap bitmap_info = SystemIcons.Information.ToBitmap();
        public Exception Exception { get; }
        bool _error;
        public bool Error
        {
            get => _error;
            set
            {
                if (_error = value) pictureBox1.BackgroundImage = bitmap_err;
                else pictureBox1.BackgroundImage = bitmap_info;
            }
        }
        public ExceptionInfoControl()
        {
            InitializeComponent();
        }

        public ExceptionInfoControl(Exception exception, bool error) : this()
        {
            Exception = exception;
            Error = error;
            lType.Text = exception.GetType().Name;
            toolTip1.SetToolTip(lType, lType.Text);
            lMessage.Text = exception.Message;
        }

        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(Exception.StackTrace, Exception.GetType().Name);
        }

        private void ToolTip1_Popup(object sender, PopupEventArgs e)
        {
            //this.
            //e.AssociatedControl.Text
        }
    }
}

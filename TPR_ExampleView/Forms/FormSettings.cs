using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPR_ExampleView.Forms
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (System.IO.Directory.Exists(textBox1.Text)) fbd.SelectedPath = textBox1.Text;
            fbd.SelectedPath = textBox1.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
                textBox1.Text = fbd.SelectedPath;
        }

        private void Label2_DoubleClick(object sender, EventArgs e)
        {
            Form f = new Form() { Text = "Подсказка", Size = new Size(600,400) };
            new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font(Font.FontFamily, 14),
                Text = new ComponentResourceManager(typeof(FormSettings)).GetString("label2.ToolTip"),
                TextAlign = ContentAlignment.MiddleLeft,
                Parent = f
            };
            f.Show();
        }
    }
}

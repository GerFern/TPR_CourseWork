using Emgu.CV;
using Emgu.CV.Structure;
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
    public partial class Form1 : Form
    {
        IImage source = null;
        string imagePath = null;
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            MenuMethod.imageBox = imageBox1;
            MenuMethod.textBox = textBox1;
            DLL_Init.AssemblyInSolution = "TestLibrary";
            DLL_Init.Init(menuStrip1);
        }

        private void открытьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imageBox1.Image = new Image<Bgr, byte>(ofd.FileName);
                    source = imageBox1.Image.Clone() as IImage;
                    восстановитьToolStripMenuItem.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void сохранитьИзображениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imageBox1.Image.Save(sfd.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void восстановитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                imageBox1.Image.Dispose();
                imageBox1.Image = source.Clone() as IImage;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void открытьПапкуСБиблиотекамиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(DLL_Init.path);
        }

        private void ВыбратьПапкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if(FBD.ShowDialog()==DialogResult.OK)
            {
                
            }
        }
    }
}

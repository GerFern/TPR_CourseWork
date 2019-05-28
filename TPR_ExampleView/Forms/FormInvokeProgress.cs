using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseLibrary;
using System.Threading;

namespace TPR_ExampleView.Forms
{
    public partial class FormInvokeProgress : Form
    {
        public FormInvokeProgress()
        {
            InitializeComponent();
            int limit = Environment.ProcessorCount;
            numericUpDown1.Maximum = limit;
            label1.Text = $"Предел потоков (максимум {limit})";
        }

        bool AllFinished => plc.Items.All(a => a.Finished);
        bool AutoStart { get; }
        IEnumerator<ProgressInfoControl> Enumerator { get; set; }
        ProgressInfoControl curPic;
        int active = 0;
        internal FormInvokeProgress(bool autoStart, MenuMethod.InvParam invParam, params ImgName[] imgs) : this()
        {
            AutoStart = autoStart;
            if(AutoStart)
                numericUpDown1.Value = Environment.ProcessorCount;
            numericUpDown1.ValueChanged += NumericUpDown1_ValueChanged;
            foreach (var item in imgs)
            {
                var localInvParam = (MenuMethod.InvParam)invParam.Clone();
                if (item.Image.IsDisposedOrNull())
                {
                    localInvParam.ImgFileString = item.ImgPath;
                }
                else
                {
                    localInvParam.Image = item.Image;
                }
                ProgressInfoControl pic;
                /*localInvParam.TaskID = */Program.mainForm.CreateTask(
                    item.Name,
                    invParam.MethodInfo,
                    new Thread(new ParameterizedThreadStart(MenuMethod.InvMethod)) { Name = item.Name },
                    localInvParam,
                    out pic);
                pic.ThreadStarted += new EventHandler((o, e) => this.InvokeFix(() => { active++; Next(); }));
                pic.ThreadFinished += new EventHandler((o, e) => this.InvokeFix(() => { active--; Next(); }));
                plc.Add(pic);

            }
            
            Enumerator = plc.Items.GetEnumerator();

            if (AutoStart)
                this.HandleCreated += new EventHandler((o, e) => Next());
        }

        private void Next()
        {
            if (Enumerator != null)
            {
                if (numericUpDown1.Value > active)
                {
                    if (Enumerator.MoveNext())
                    {
                        curPic = Enumerator.Current;
                        //if (curPic.Started)
                        //{
                            
                        //}
                        //else
                        //{
                        //    curPic.ThreadStart();
                        //}
                        curPic.ThreadStart();
                    }
                    else Enumerator = null;
                }
            }
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Next();
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!AllFinished)
                if (MessageBox.Show("Завершить работу?", "", MessageBoxButtons.YesNo) == DialogResult.No)
                    e.Cancel = false;
            base.OnClosing(e);
        }
    }
}

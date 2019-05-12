using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BaseLibrary;
using System.Threading;

namespace TPR_ExampleView
{
    public partial class ProgressInfoControl : UserControl
    {
        public BaseLibrary.InputImage.ProgressInfo ProgressInfo { get; }
        DateTime startTime;
        DateTime endTime;
        Thread Thread { get; }
        public ProgressBar ProgressBar { get; }
        int _max;
        int _value;
        int Max
        {
            get => _max;
            set
            {
                _max = value;
                if (lProgress.InvokeRequired)
                    lProgress.Invoke(new Action(() =>
                        lProgress.Text = $"Выполняется ({_value}/{_max})"));
            }
        }
        int Value
        {
            get => _value;
            set
            {
                _value = value;
                if (lProgress.InvokeRequired)
                    lProgress.Invoke(new Action(() =>
                        lProgress.Text = $"Выполняется ({_value}/{_max})"));
            }
        }
        public ProgressInfoControl(InputImage.ProgressInfo progressInfo, Thread thread)
        {
            InitializeComponent();
            ProgressInfo = progressInfo;
            Thread = thread;
            startTime = DateTime.Now;
            lName.Text = progressInfo.MethodName;
            lTimeStart.Text = $"Начало: {startTime.ToLongTimeString()}";
            progressInfo.ProgressMaximumChanged += new EventHandler((o, e) =>
            {
                Max = ProgressInfo.Maximum;
            });
            progressInfo.ProgressValueChanged += new EventHandler((o, e) =>
            {
                Value = ProgressInfo.Value;
            });
            progressInfo.Finished += new EventHandler<BaseLibrary.CancelEventArgs>((o, e) =>
            {
                timer1.Stop();
                endTime = DateTime.Now;
                if (InvokeRequired)
                {
                    lDuratuin.Invoke(new Action(() => { FinishM(e); }));
                }
                else FinishM(e);
            });
            ProgressBar = Program.mainForm.ProgressDict[progressInfo.ID].ProgressBar;
            ProgressBar.Dock = DockStyle.Fill;
            if (tableLayoutPanel1.InvokeRequired||ProgressBar.InvokeRequired)
                ProgressBar.Invoke(new Action(() => { tableLayoutPanel1.Controls.Add(ProgressBar, 0, 4); }));
            else
            tableLayoutPanel1.Controls.Add(ProgressBar, 0, 4);
            tableLayoutPanel1.SetColumnSpan(ProgressBar, 3);
            //if(!progressInfo.CancelSupport)
            //{
            //    tableLayoutPanel1.Controls.Remove(button1);
            //}
            timer1.Start();
            //Progress = progress;
        }

        private void FinishM(BaseLibrary.CancelEventArgs e)
        {
            lDuratuin.Text = $"Прошло {(endTime - startTime).TotalSeconds.ToString("F")} c.";
            if (e.Cancel) lProgress.Text = $"Отменено ({_value}/{_max})";
            else lProgress.Text = $"Выполнено ({_value}/{_max})";
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {

            lDuratuin.Text = $"Прошло {(DateTime.Now-startTime).TotalSeconds.ToString("F")} c.";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ProgressInfo.CancelExecute();
        }

        private void BAbort_Click(object sender, EventArgs e)
        {
            if (Thread.ThreadState.HasFlag(ThreadState.Suspended)) Thread.Resume();
            Thread.Abort();
        }

        private void BPause_Click(object sender, EventArgs e)
        {
            if (Thread.ThreadState.HasFlag(ThreadState.Unstarted)) Thread.Start();
            else
            if (Thread.ThreadState.HasFlag(ThreadState.Suspended)) Thread.Resume();
            else Thread.Suspend();
            MessageBox.Show(
            Thread.ThreadState.ToString()
            );
        }
    }
}

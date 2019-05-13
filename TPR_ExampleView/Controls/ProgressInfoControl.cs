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
using System.Runtime.InteropServices;

namespace TPR_ExampleView
{
    public partial class ProgressInfoControl : UserControl
    {
        public BaseLibrary.InputImage.ProgressInfo ProgressInfo { get; private set; }
        MenuMethod.InvParam InvParam { get; }
        public bool Started { get; private set; }
        DateTime startTime;
        //DateTime resumeTime;
        //TimeSpan deltaTime;
        DateTime endTime;

        BaseLibrary.Timer Timer = new BaseLibrary.Timer();
        TimeSpan TimeSpent => Timer.TimeSpent;
        internal Thread Thread { get; }
        public ProgressBar ProgressBar { get; private set; }
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
                        lProgress.Text = $"{ThreadStatus.DescriptionAttr()} ({_value}/{_max})"));
            }
        }

        Exception Exception { get; set; }

        internal void SetException(Exception ex)
        {
            Exception = ex;
            if (tableLayoutPanel2.InvokeRequired)
                tableLayoutPanel2.Invoke(new Action(() =>
                {
                    LinkLabel linkLabel = new LinkLabel() { Text = "Стек вызовов...", Dock = DockStyle.Bottom };
                    linkLabel.Click += new EventHandler((o, e) => MessageBox.Show(Exception.StackTrace, Exception.GetType().Name));
                    tableLayoutPanel2.Controls.Add(linkLabel, 0, 1);
                }));
            ThreadStatus = Status.Exception;
        }

        int Value
        {
            get => _value;
            set
            {
                _value = value;
                if (lProgress.InvokeRequired)
                    lProgress.Invoke(new Action(() =>
                        lProgress.Text = $"{ThreadStatus.DescriptionAttr()} ({_value}/{_max})"));
            }
        }
        Status _status;
        Status ThreadStatus
        {
            get => _status;
            set
            {
                _status = value;
                switch (value)
                {
                    case Status.Unstarted:
                        bPause.BackgroundImage = Properties.Resources.воспроизведение_32;
                        break;
                    case Status.Started:
                        bPause.BackgroundImage = Properties.Resources.пауза_32;
                        ProgressBar.SetState(1);
                        timer1.Start();
                        break;
                    case Status.Paused:
                        timer1.Stop();
                        bPause.BackgroundImage = Properties.Resources.воспроизведение_32;
                        ProgressBar.SetState(3);
                        break;
                    case Status.Aborted:
                        timer1.Stop();
                        break;
                    case Status.Cancel:
                        timer1.Stop();
                        break;
                    case Status.Finished:
                        ProgressBar.InvokeFix(() => { ProgressBar.Style = ProgressBarStyle.Blocks; ProgressBar.Value = ProgressBar.Maximum; });
                        break;
                    case Status.Exception:
                        //ProgressBar.InvokeFix(()=>ForeColor = Color.Red);
                        ProgressBar.SetState(2);
                        break;
                    default:
                        break;
                }
                lProgress.InvokeFix(() => lProgress.Text = $"{value.DescriptionAttr()} ({Value}/{Max})");
            }
        }
        public void Start(InputImage.ProgressInfo progressInfo)
        {
            ProgressInfo = progressInfo;
            Max = ProgressInfo.Maximum;
            Value = ProgressInfo.Value;
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

           
            //MessageBox.Show(Thread.ThreadState.ToString());

        }

        public void ThreadStart()
        {
            if (Started) return;
            Started = true;
            startTime = DateTime.Now;
            ProgressBar.Style = ProgressBarStyle.Marquee;
            ThreadStatus = Status.Started;
            Timer.Start();
            Thread.Start(InvParam);
            lTimeStart.InvokeFix(() =>
            {
                lTimeStart.Text = $"Начало: {startTime.ToLongTimeString()}";
                timer1.Start();
            });
            new Thread(() =>
            {
                ThreadStarted?.Invoke(this, EventArgs.Empty);
            })
            { Name = "ThreadStartInvoker" }.Start();
            new Thread(() =>
            {
                Thread.Join();
                timer1.Stop();
                if (ThreadStatus != Status.Finished && ThreadStatus != Status.Cancel && ThreadStatus != Status.Exception)
                {
                    if (Thread.ThreadState.HasFlag(ThreadState.Stopped))
                        ThreadStatus = Status.Finished;
                    if (Thread.ThreadState.HasFlag(ThreadState.Aborted))
                        ThreadStatus = Status.Aborted;
                }
                TimeText();
                ThreadFinished?.Invoke(this, EventArgs.Empty);
            })
            { Name = "ThreadWaitHandler" }.Start();
        }

        internal ProgressInfoControl(string name, MenuMethod.InvParam invParam, Thread thread, ProgressBar progressBar)
        {
            InitializeComponent();
            const string empty = "___";
            lTimeStart.Text = empty;
            lName.Text = empty;
            lDuratuin.Text = empty;
            lTimeStart.Text = empty;
            InvParam = invParam;
            InvParam.ProgressInfoControl = this;
            Thread = thread;
            if (Thread.ThreadState.HasFlag(ThreadState.Unstarted)) ThreadStatus = Status.Unstarted;
            else ThreadStatus = Status.Started;
            lName.Text = name;

            ProgressBar = progressBar;
            ProgressBar.Dock = DockStyle.Fill;
            if (tableLayoutPanel1.InvokeRequired||ProgressBar.InvokeRequired)
                ProgressBar.Invoke(new Action(() => { tableLayoutPanel1.Controls.Add(ProgressBar, 0, 4); }));
            else
            tableLayoutPanel1.Controls.Add(ProgressBar, 0, 4);
            //tableLayoutPanel1.SetColumnSpan(ProgressBar, 3);
            //if(!progressInfo.CancelSupport)
            //{
            //    tableLayoutPanel1.Controls.Remove(button1);
            //}
            //Progress = progress;
        }

        private void FinishM(BaseLibrary.CancelEventArgs e)
        {
            TimeText();
            if (e.Cancel) ThreadStatus = Status.Cancel;
            else ThreadStatus = Status.Finished;
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            TimeText();
        }

        private void TimeText()
        {
            lDuratuin.InvokeFix(() => lDuratuin.Text = $"Прошло {TimeSpent.TotalSeconds.ToString("F")} c.");
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ProgressInfo?.CancelExecute();
        }

#pragma warning disable CS0618 // Тип или член устарел
        private void BAbort_Click(object sender, EventArgs e)
        {
            if (Thread.ThreadState.HasFlag(ThreadState.Suspended)) Thread.Resume();
            Thread.Abort();
        }
        private void BPause_Click(object sender, EventArgs e)
        {
            if (Thread.ThreadState.HasFlag(ThreadState.Stopped)) return;
            if (Thread.ThreadState.HasFlag(ThreadState.Aborted)) return;
            if (Thread.ThreadState.HasFlag(ThreadState.SuspendRequested)) return;
            if (Thread.ThreadState.HasFlag(ThreadState.Unstarted))
                ThreadStart();
            else
                try
                {
                    if (Thread.ThreadState.HasFlag(ThreadState.Suspended))
                    {
                        Thread.Resume();
                        Timer.Resume();
                        ThreadStatus = Status.Started;
                        timer1.Start();
                    }
                    else
                    {
                        Thread.Suspend();
                        Timer.Suspend();
                        ThreadStatus = Status.Paused;
                        timer1.Stop();
                        TimeText();
                    }
                }
                catch { }
        }
#pragma warning restore CS0618 // Тип или член устарел

        public enum Status
        {
            [Description("Не запущен")]
            Unstarted,
            [Description("Выполняется")]
            Started,
            [Description("Приостановлен")]
            Paused,
            [Description("Прерван")]
            Aborted,
            [Description("Отменен")]
            Cancel,
            [Description("Завершен")]
            Finished,
            [Description("Ошибка")]
            Exception
        }

        private void ЗакрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void Close()
        {
            Closed?.Invoke(this, new EventArgs());
        }

        public event EventHandler Closed;
        public event EventHandler ThreadFinished;
        public event EventHandler ThreadStarted;
    }
    
}

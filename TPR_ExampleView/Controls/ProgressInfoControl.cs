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
        public bool MinimalStile
        {
            get => _minimalStile;
            set 
            {
                if (_minimalStile != value)
                {
                    _minimalStile = value;
                    //this.InvokeFix(() =>
                    //{
                    if (ProgressBar != null)
                    {
                        if (value)
                        {
                            //ProgressBar.InvokeFix(() =>
                            //ProgressBar.Parent = null);
                            //ProgressBar.InvokeFix(() =>
                            //ProgressBar.Size = labeledProgressBar1.Size);
                            //ProgressBar.Size = labeledProgressBar1.Size;
                           
                        }
                        else
                        {
                            //ProgressBar.InvokeFix(() =>
                            //MaxPanel.Controls.Add(ProgressBar, 0, 4));
                            //ProgressBar.Parent = null;
                            //MaxPanel.Controls.Add(ProgressBar, 0, 4);
                           
                           
                            //ProgressBar.InvokeFix(() =>
                            //MaxPanel.Parent = Panel);
                        }
                    }
                    //});
                }
            }
        }

        BaseLibrary.Timer Timer = new BaseLibrary.Timer();
        TimeSpan TimeSpent => Timer.TimeSpent;
        internal Thread Thread { get; }
        public ProgressBar ProgressBar => progressBar1;
        int _max;
        int _value;
        //Thread updateHadler;
        //EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
        int Max
        {
            get => ProgressInfo != null ? ProgressInfo.Maximum : 100;
            set
            {
                _max = value;
                //ProgressUpdate();
            }
        }
        int Value
        {
            get => ProgressInfo != null ? ProgressInfo.Value : 0;
            set
            {
                _value = value;
                //ProgressUpdate();

            }
        }
        private void ProgressUpdate()
        {
            lProgress.InvokeFix(() =>
            {
                lProgress.Text = $"{ThreadStatus.DescriptionAttr()} ({Value}/{Max})";
            });
        }

        Exception Exception { get; set; }

        internal void SetException(Exception ex)
        {
            Exception = ex;
            if (tableLayoutPanel2.InvokeRequired)
                tableLayoutPanel2.Invoke(new Action(() =>
                {
                    LinkLabel linkLabel = new LinkLabel() { Text = "Стек вызовов...", Dock = DockStyle.Bottom };
                    linkLabel.Click += new EventHandler((o, e) => MessageBox.Show(Exception.Message + Environment.NewLine + Exception.StackTrace, Exception.GetType().Name));
                    tableLayoutPanel2.RowCount = 2;
                    tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 16));
                    tableLayoutPanel2.Controls.Add(linkLabel, 0, 1);
                    tableLayoutPanel1.Height += 16;
                }));
            ThreadStatus = Status.Exception;
        }

        public bool Finished => _status == Status.Finished ||
                                _status == Status.Cancel ||
                                _status == Status.Aborted ||
                                _status == Status.Exception;

        Status _status;
        private bool _minimalStile;
        private TimeSpan _oldTimeSpent;
        private int _oldValue;
        private int _oldMax;
        EventWaitHandle waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset);
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
                        ProgressBar.SetState(Extensions.ProgressBarState.Green/*, waitHandle*/);
                        timer1.Start();
                        break;
                    case Status.Paused:
                        timer1.Stop();
                        bPause.BackgroundImage = Properties.Resources.воспроизведение_32;
                        ProgressBar.SetState(Extensions.ProgressBarState.Yellow/*, waitHandle*/);
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
                        ProgressBar.SetState(Extensions.ProgressBarState.Red/*, waitHandle*/);
                        break;
                    default:
                        break;
                }
                
                ProgressUpdate();
            }
        }
        public void Start(InputImage.ProgressInfo progressInfo)
        {
            ProgressInfo = progressInfo;
            //Max = ProgressInfo.Maximum;
            //Value = ProgressInfo.Value;
            progressInfo.Started += new EventHandler((o, e) => ProgressBar.Style = ProgressBarStyle.Continuous);
            if (progressInfo.IsRun) ProgressBar.InvokeFix(() => ProgressBar.Style = ProgressBarStyle.Continuous);
            //progressInfo.ProgressMaximumChanged += new EventHandler((o, e) =>
            //{
            //    Max = progressInfo.Maximum;
            //});
            //progressInfo.ProgressValueChanged += new EventHandler((o, e) =>
            //{
            //    Value = progressInfo.Value;
            //});
            progressInfo.Finished += new EventHandler<BaseLibrary.CancelEventArgs>((o, e) =>
            {
                timer1.Stop();
                endTime = DateTime.Now;
                lDuratuin.InvokeFix(() => { FinishM(e); });
                lProgress.InvokeFix(() => ProgressUpdate());
                ProgressBar.InvokeFix(() => { ProgressBar.Maximum = Max; ProgressBar.Value = Value; });
            });


            //MessageBox.Show(Thread.ThreadState.ToString());

        }

        public void ThreadStart()
        {
            if (Started) return;
            Started = true;
            startTime = DateTime.Now;
            //ProgressBar.Style = ProgressBarStyle.Marquee;
            ThreadStatus = Status.Started;
            Timer.Start();
            //Thread.Priority = ThreadPriority.Highest;
            Thread.Start(InvParam);
            //updateHadler.Start();
            lTimeStart.InvokeFix(() =>
            {
                lTimeStart.Text = $"Начало: {startTime.ToLongTimeString()}";
                timer1.Start();
            });
            new Thread(() =>
            {
                ThreadStarted?.Invoke(this, EventArgs.Empty);
            })
            { Name = "ThreadStartInvoker", IsBackground = true }.Start();
            new Thread(() =>
            {
                Thread.Join();
                timer1.Stop();
                //this.InvokeFix(() =>
                ProgressBar.InvokeFix(() => { ProgressBar.Maximum = Max; ProgressBar.Value = Value; });
                //{
                if (ThreadStatus != Status.Finished && ThreadStatus != Status.Cancel && ThreadStatus != Status.Exception)
                {
                    if (Thread.ThreadState.HasFlag(ThreadState.Stopped))
                        ThreadStatus = Status.Finished;
                    if (Thread.ThreadState.HasFlag(ThreadState.Aborted))
                        ThreadStatus = Status.Aborted;
                }
                tableLayoutPanel1.InvokeFix(() =>
                {
                    
                    tableLayoutPanel3.Parent = null;
                    tableLayoutPanel3.Dispose();
                    tableLayoutPanel1.RowCount = 5;
                    tableLayoutPanel1.Height -= 24;
                    //HeigthUpdate();
                });
                //TimeText();
                //});
                //this.InvokeFix(() => TimeText());
                ThreadFinished?.Invoke(this, EventArgs.Empty);
            })
            { Name = "ThreadWaitHandler", Priority = ThreadPriority.AboveNormal , IsBackground = true }.Start();
        }

        internal ProgressInfoControl()
        {
            InitializeComponent();
        }

        internal ProgressInfoControl(string name, MenuMethod.InvParam invParam, Thread thread) : this()
        {
            //ProgressBar = progressBar = new ProgressBar();
            InputImage.InitProgress initProgress = new InputImage.InitProgress(progressBar1);
            initProgress.Init += new EventHandler((o, e) =>
            {
                if (o is InputImage.InitProgress ip)
                {
                    Start(ip.ProgressInfo);
                }
            });
            Program.mainForm.ProgressDict.Add(invParam.TaskID, initProgress);
            //updateHadler = new Thread(() =>
            //{
            //    Thread.Sleep(1000);
            //    while (true)
            //    {
            //        waitHandle.WaitOne();
            //        Thread.Sleep(100);
            //        Thread.Yield();
            //        lProgress.InvokeFix(() =>
            //                          lProgress.Text = $"{ThreadStatus.DescriptionAttr()} ({_value}/{_max})");
            //        Thread.Sleep(100);
            //        if (Finished) break;
            //    }
            //})
            //{ Name = "UpdateProgressHandler", IsBackground = true };
            const string empty = "___";
            lTimeStart.Text = empty;
            lDuratuin.Text = empty;
            lTimeStart.Text = empty;
            InvParam = invParam;
            InvParam.ProgressInfoControl = this;
            Thread = thread;
            if (Thread.ThreadState.HasFlag(ThreadState.Unstarted)) ThreadStatus = Status.Unstarted;
            else ThreadStatus = Status.Started;
            toolTip1.SetToolTip(lName, name);
            lName.Text = name;

            //if (tableLayoutPanel1.InvokeRequired || ProgressBar.InvokeRequired)
            //    ProgressBar.Invoke(new Action(() => { tableLayoutPanel1.Controls.Add(ProgressBar, 0, 4); }));
            //else
            //    tableLayoutPanel1.Controls.Add(ProgressBar, 0, 4);
            //this.Closed += new EventHandler((o, e) => { updateHadler.Abort(); waitHandle.Close(); });
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
            this.InvokeFix(() =>
            {
                var TimeSpent = this.TimeSpent;
                if (_oldTimeSpent != TimeSpent)
                {
                    _oldTimeSpent = TimeSpent;
                    lDuratuin.InvokeFix(() => lDuratuin.Text = $"Прошло {TimeSpent.TotalSeconds.ToString("F")} c.");
                }
                bool val_changed = false;
                int max = Max, val = Value;
                if (_oldMax != max)
                {
                    ProgressBar.Maximum = max;
                    val_changed = true;
                }
                if (_oldValue != val)
                {
                    ProgressBar.Value = val;
                    val_changed = true;
                }
                if (val_changed)
                {
                    _oldValue = val;
                    _oldMax = max;
                    ProgressUpdate();
                }
            });
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            ProgressInfo?.CancelExecute();
        }

#pragma warning disable CS0618 // Тип или член устарел

        public void Abort()
        {
            if (Thread.ThreadState.HasFlag(ThreadState.Suspended)) Thread.Resume();
            Thread.Abort();
        }
        private void BAbort_Click(object sender, EventArgs e)
        {
            Abort();
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

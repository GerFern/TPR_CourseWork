using BaseLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPR_ExampleView.Forms
{
    public partial class FormSimpleInvokeProgress : Form
    {
        public FormSimpleInvokeProgress()
        {
            InitializeComponent();
        }

        public bool Suspended = false;
        public List<TaskItem> Tasks { get; } = new List<TaskItem>();
        public List<TaskItem> ActiveTasks { get; } = new List<TaskItem>();
        IEnumerator<TaskItem> Enumerator { get; set; }
        int active = 0;
        int finished = 0;
        int oldFinished = 0;
        int countTask;
        public class TaskItem
        {
            public ImgName ImgName { get; set; }
            internal MenuMethod.InvParam InvParam { get; set; }
            public Thread Thread { get; set; }
            public bool Started { get; private set; }
            public DateTime StartTime { get; private set; }
            public event EventHandler ThreadStarted;
            public event EventHandler ThreadFinished;

            public void Suspend()
            {
                ThreadState threadState = Thread.ThreadState;
                if (threadState.HasFlag(ThreadState.Stopped)) return;
                if (threadState.HasFlag(ThreadState.Suspended)) return;
                if (threadState.HasFlag(ThreadState.Aborted)) return;
                if (threadState.HasFlag(ThreadState.SuspendRequested)) return;
                else
                    try
                    {
                        Thread.Suspend();
                    }
                    catch { }
            }

            public void Resume()
            {
                ThreadState threadState = Thread.ThreadState;
                if (threadState.HasFlag(ThreadState.Stopped)) return;
                if (threadState.HasFlag(ThreadState.Aborted)) return;
                if (threadState.HasFlag(ThreadState.SuspendRequested)) return;
                else
                    try
                    {
                        if (threadState.HasFlag(ThreadState.Suspended))
                            Thread.Resume();
                    }
                    catch { }
            }

            public void ThreadStart()
            {
                if (Started) return;
                Started = true;
                StartTime = DateTime.Now;
                //Thread.Priority = ThreadPriority.Highest;
                Thread.Start(InvParam);
                //updateHadler.Start();
                new Thread(() =>
                {
                    ThreadStarted?.Invoke(this, EventArgs.Empty);
                })
                { Name = "ThreadStartInvoker", IsBackground = true }.Start();
                new Thread(() =>
                {
                    Thread.Join();
                    ThreadFinished?.Invoke(this, EventArgs.Empty);
                })
                { Name = "ThreadWaitHandler", Priority = ThreadPriority.AboveNormal, IsBackground = true }.Start();
            }
        }
        //public List<>
        internal FormSimpleInvokeProgress(bool autoStart, MenuMethod.InvParam invParam, params ImgName[] imgs) : this()
        {
            AutoStart = autoStart;
            label1.Text = $"Предел потоков (максимум {Environment.ProcessorCount})";
            if (AutoStart)
                numericUpDown1.Value = Environment.ProcessorCount;
            numericUpDown1.Maximum = Environment.ProcessorCount;
            numericUpDown1.ValueChanged += NumericUpDown1_ValueChanged;
            foreach (var item in imgs)
            {
                if(invParam.Vs!=null)
                {
                    invParam.Vs[0] = null;
                }
                var localInvParam = (MenuMethod.InvParam)invParam.Clone();
                if (item.Image.IsDisposedOrNull())
                {
                    localInvParam.ImgFileString = item.ImgPath;
                }
                else
                {
                    localInvParam.Image = item.Image;
                }
                localInvParam.ImageName = item.Name;
                TaskItem taskItem = new TaskItem
                {
                    ImgName = item,
                    InvParam = localInvParam,
                    Thread = new Thread(new ParameterizedThreadStart(MenuMethod.InvMethod)) { Name = item.Name }
                };
                Tasks.Add(taskItem);
                taskItem.ThreadStarted += new EventHandler((o, e) => this.InvokeFix(() => { ActiveTasks.Add(taskItem); active++; Next(); }));
                taskItem.ThreadFinished += new EventHandler((o, e) => this.InvokeFix(() => { ActiveTasks.Remove(taskItem); active--; finished++; Next(); }));
            }

            countTask = Tasks.Count;
            label3.Text = $"Выполнено {finished} из {countTask}";
            progressBar1.Maximum = countTask;
            Enumerator = Tasks.GetEnumerator();
            if (AutoStart)
            {
                this.HandleCreated += new EventHandler((o, e) =>
                {
                    timer1.Start();
                    Next();
                });
                //new Thread(() =>
                //{
                //    Thread.Sleep(1000);
                //    plc.InvokeFix(()=>Next());
                //})
                //{ Name = "Activator" }.Start();
            }
        }

        public bool AutoStart { get; private set; }
        public TaskItem CurTask { get; private set; }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Next();
        }

        private void Next()
        {
            if (Enumerator != null)
            {
                if (numericUpDown1.Value > active)
                {
                    if (Enumerator.MoveNext())
                    {
                        CurTask = Enumerator.Current;
                        CurTask.ThreadStart();
                    }
                    else Enumerator = null;
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (oldFinished != finished)
            {
                oldFinished = finished;
                label3.Text = $"Выполнено {finished} из {countTask}";
                progressBar1.Value = finished;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (Suspended)
            {
                Suspended = false;
                foreach (var item in ActiveTasks)
                {
                    item.Resume();
                }
                Next();
                button2.Text = "Приостановить все";
            }
            else
            {
                Suspended = true;
                foreach (var item in ActiveTasks)
                {
                    item.Suspend();
                }
                button2.Text = "Взобновить";
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (finished < countTask)
            {
                if (MessageBox.Show("Завершить выполнение задач?", "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    Suspended = true;
                    foreach (var item in ActiveTasks)
                    {
                        item.Thread.Abort();
                    }
                    Close();
                }
            }
            else Close();
        }
    }
}

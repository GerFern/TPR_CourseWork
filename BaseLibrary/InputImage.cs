using Emgu.CV;
using System;
using System.Windows.Forms;

namespace BaseLibrary
{
    public class InputImage
    {
        int _id = -1;
        public int ID
        {
            get => _id; set
            {
                if (_id < 0 && value >= 0)
                {
                    _id = value;
                    Progress = new ProgressInfo(this, BaseMethods._getProgressBar.Invoke(this));
                }
            }
        }
        public string MethodName { get; }
        public IImage Image { get; }
        public Type ImageType { get; }
        public Type TColor { get; }
        public Type TDepth { get; }
        public ProgressInfo Progress { get; private set; }
        public static IImage Convert<TColor, TDepth>(IImage image) where TColor : struct, IColor where TDepth : new()
        {
            dynamic t = image;
            return t.Convert<TColor, TDepth>();
        }

        public InputImage(IImage image, int ID, string methodName)
        {
            Image = image;
            this.ID = ID;
            MethodName = methodName;
            if (image != null)
            {
                ImageType = image.GetType();
                Type[] t = ImageType.GetGenericArguments();
                if (t.Length == 2)
                {
                    TColor = t[0];
                    TDepth = t[1];
                }
            }
            //if (ID >= 0)
            //    Progress = new ProgressInfo(this, BaseMethods._getProgressBar.Invoke(this));
        }

        public class ProgressInfo
        {
            //public void Show();
            public string MethodName => _inputImage.MethodName;
            internal InputImage _inputImage;
            private readonly InitProgress _initProgress;
            public int ID { get => _inputImage.ID; }

            /// <summary>
            /// Можно ли отменять выполнение метода
            /// </summary>
            public bool CancelSupport { get; private set; }
            /// <summary>
            /// Требуется ли прервать выполнения метода
            /// </summary>
            public bool Cancel { get; private set; }
            /// <summary>
            /// Отменить выполнение метода извне (в пользовательских библиотеках это использовать не нужно)
            /// </summary>
            public void CancelExecute()
            {
                if (CancelSupport) Cancel = true;
            }
            /// <summary>
            /// Возвращает или задает наибольшее значение диапозона элемента управления <see cref="System.Windows.Forms.ProgressBar"/>
            /// </summary>
            public int Maximum
            {
                get => ProgressBar.Maximum;
                set
                {
                    ProgressBar.InvokeFix(() => ProgressBar.Maximum = value);
                    ProgressMaximumChanged?.Invoke(this, new EventArgs());
                }
            }

            /// <summary>
            /// Возвращает или задает наименьшее значение диапозона элемента управления <see cref="System.Windows.Forms.ProgressBar"/>
            /// </summary>
            public int Minimum
            {
                get => ProgressBar.Minimum;
                set
                {
                    ProgressBar.InvokeFix(() => ProgressBar.Minimum = value);
                    ProgressMinimumChanged?.Invoke(this, new EventArgs());
                }
            }

            /// <summary>
            /// Возвращает или задает текущее положение индикатора выполнения
            /// </summary>
            public int Value
            {
                get => ProgressBar.Value;
                set
                {
                    ProgressBar.InvokeFix(() => ProgressBar.Value = value);
                    ProgressValueChanged?.Invoke(this, new EventArgs());
                }
            }

            /// <summary>
            /// Возвращает или задает интервал, в котором вызов <see cref="PerformStep()"/> метод увеличивает текущее положение индикатора хода выполнения
            /// </summary>
            public int Step
            {
                get => ProgressBar.Step;
                set => ProgressBar.InvokeFix(() => ProgressBar.Step = value);
            }

            /// <summary>
            /// Возвращает или задает способ отображения выполнения в индикаторе выполнения
            /// </summary>
            public ProgressBarStyle Style
            {
                get => ProgressBar.Style;
                set
                {
                    if (ProgressBar.InvokeRequired)
                        ProgressBar.Invoke(new Action(() => ProgressBar.Style = value));
                    else
                    ProgressBar.Style = value;
                }
            }
            internal ProgressBar ProgressBar { get; set; }

            /// <summary>
            /// Увеличивает текущую позицию индикатора хода выполнения на объем <see cref="Step"/> свойство
            /// </summary>
            public void PerformStep()
            {
                ProgressBar.InvokeFix(() => ProgressBar.PerformStep());
                ProgressValueChanged?.Invoke(this, new EventArgs());
            }

            /// <summary>
            /// Инициализация прогресса
            /// </summary>
            /// <param name="step">Интервал, в котором вызов <see cref="PerformStep()"/> метод увеличивает текущее положение индикатора хода выполнения</param>
            /// <param name="maximum">Наибольшее значение диапозона элемента управления <see cref="System.Windows.Forms.ProgressBar"/>
            public void Run(int step, int maximum, bool canCancel = false)
            {
                CancelSupport = canCancel;
                if (ProgressBar == null)
                {
                    ProgressBar = _initProgress.ProgressBar;
                    ProgressBar.InvokeFix(() => ProgressBar.Style = ProgressBarStyle.Continuous);
                    Step = step;
                    Maximum = maximum;
                }
                Started?.Invoke(this, new EventArgs());
                _initProgress.DoInit();
            }

            /// <summary>
            /// Завершение выполнения операции
            /// </summary>
            public void Finish(bool cancel = false)
            {
                if (ProgressBar != null)
                {
                    if (cancel)
                    {
                        if (ProgressBar.InvokeRequired)
                            ProgressBar.Invoke(new Action(() => { ProgressBar.Enabled = false; }));
                        else
                            ProgressBar.Enabled = false;
                    }
                    else
                        Value = Maximum;
                }
                Finished?.Invoke(this, new CancelEventArgs(cancel));
            }

            internal ProgressInfo(InputImage inputImage, InitProgress initProgress)
            {
                _inputImage = inputImage;
                _initProgress = initProgress;
                initProgress.ProgressInfo = this;
            }

           
            /// <summary>
            /// Выполнение метода было завершено
            /// </summary>
            public event EventHandler<CancelEventArgs> Finished;
            /// <summary>
            /// Начало выполнения метода
            /// </summary>
            public event EventHandler Started;
            /// <summary>
            /// Текущее значение прогресса изменилось
            /// </summary>
            public event EventHandler ProgressValueChanged;
            /// <summary>
            /// Минимальное значение прогресса изменилось
            /// </summary>
            public event EventHandler ProgressMinimumChanged;
            /// <summary>
            /// Максимальное значение прогресса изменилось
            /// </summary>
            public event EventHandler ProgressMaximumChanged;

        }
        public class InitProgress
        {
            public InitProgress(ProgressBar progressBar)
            {
                ProgressBar = progressBar;
            }

            public ProgressBar ProgressBar { get; }
            public ProgressInfo ProgressInfo { get; internal set; }
            public event EventHandler Init;
            internal void DoInit() => Init?.Invoke(this, new EventArgs());
        }
    }

    public class CancelEventArgs : EventArgs
    {
        public bool Cancel { get; }
        public CancelEventArgs(bool cancel) => Cancel = cancel;
    }
}

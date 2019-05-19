using Emgu.CV;
using System;
using System.Windows.Forms;

namespace BaseLibrary
{
    /// <summary>
    /// Входное изображение
    /// </summary>
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

        /// <summary>
        /// Название метода
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Входное изображение
        /// </summary>
        public IImage Image { get; }

        /// <summary>
        /// Тип изображения
        /// </summary>
        public Type ImageType { get; }

        /// <summary>
        /// Тип TColor. Например <see cref="Emgu.CV.Structure.Bgr"/>, <see cref="Emgu.CV.Structure.Gray"/>
        /// </summary>
        public Type TColor { get; }

        /// <summary>
        /// Тип TDepth. Например <see cref="byte"/>, <see cref="float"/>
        /// </summary>
        public Type TDepth { get; }
        public ProgressInfo Progress { get; private set; }

        /// <summary>
        /// Создать новое изображение, конвертированный в другой тип
        /// </summary>
        /// <typeparam name="TColor"></typeparam>
        /// <typeparam name="TDepth"></typeparam>
        /// <param name="image"></param>
        /// <returns></returns>
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
            /// <summary>
            /// Является ли запущеным
            /// </summary>
            public bool IsRun { get; private set; }
            /// <summary>
            /// Название метода
            /// </summary>
            public string MethodName => _inputImage.MethodName;
            internal InputImage _inputImage;
            private readonly InitProgress _initProgress;
            /// <summary>
            /// Идентификатор прогресса
            /// </summary>
            public int ID { get => _inputImage.ID; }

            /// <summary>
            /// Можно ли отменять выполнение метода (Это не защитит метод от принудительного прерывания потока)
            /// </summary>
            public bool CancelSupport { get; private set; }
            /// <summary>
            /// Требуется ли прервать выполнения метода
            /// </summary>
            public bool Cancel { get; private set; }
            /// <summary>
            /// Отменить выполнение метода извне. Выставляет свойство <see cref="Cancel"/> в <see langword="true"/>. В пользовательских библиотеках это использовать не имеет смысла
            /// </summary>
            public void CancelExecute()
            {
                if (CancelSupport) Cancel = true;
            }
            int _maximum;
            /// <summary>
            /// Возвращает или задает наибольшее значение диапозона элемента управления <see cref="ProgressBar"/>
            /// </summary>
            public int Maximum
            {
                get => _maximum;
                set
                {
                    _maximum = value;
                    //ProgressBar.InvokeFix(() => ProgressBar.Maximum = value);
                    ProgressMaximumChanged?.Invoke(this, new EventArgs());
                }
            }

            //int _minimum;
            ///// <summary>
            ///// Возвращает или задает наименьшее значение диапозона элемента управления <see cref="System.Windows.Forms.ProgressBar"/>
            ///// </summary>
            //public int Minimum
            //{
            //    get => _maximum;
            //    set
            //    {
            //        _minimum = value;
            //        //ProgressBar.InvokeFix(() => ProgressBar.Minimum = value);
            //        //ProgressMinimumChanged?.Invoke(this, new EventArgs());
            //    }
            //}

            int _value;

            /// <summary>
            /// Возвращает или задает текущее положение индикатора выполнения
            /// </summary>
            public int Value
            {
                get => _value;
                set
                {
                    _value = value;
                    //ProgressBar.InvokeFix(() => ProgressBar.Value = value);
                    ProgressValueChanged?.Invoke(this, new EventArgs());
                }
            }

            int _step;

            /// <summary>
            /// Возвращает или задает интервал, в котором вызов <see cref="PerformStep()"/> метод увеличивает текущее положение индикатора хода выполнения
            /// </summary>
            public int Step
            {
                get => _step;
                set => _step = value;
            }

            //internal ProgressBar ProgressBar { get; set; }

            /// <summary>
            /// Увеличивает текущую позицию индикатора хода выполнения на объем <see cref="Step"/> свойство
            /// </summary>
            public void PerformStep()
            {
                //try
                //{
                //System.Threading.Thread.Yield();
                Value += _step;
                //ProgressBar.InvokeFix(() => ProgressBar.PerformStep());
                //ProgressValueChanged?.Invoke(this, new EventArgs());
                //}
                //catch(Exception ex) { System.Diagnostics.Debugger.Break(); }
            }

            /// <summary>
            /// Инициализация прогресса
            /// </summary>
            /// <param name="step">Интервал, в котором вызов <see cref="PerformStep()"/> метод увеличивает текущее положение индикатора хода выполнения</param>
            /// <param name="maximum">Наибольшее значение диапозона элемента управления <see cref="System.Windows.Forms.ProgressBar"/></param>
            /// <param name="canCancel">
            /// Проверяет ли метод свойство <see cref="Cancel"/> для отмены.
            /// Это не защитит метод от принудительного прерывания потока.
            /// Для защиты от прерывания потока обрабатывайте исключение <see cref="System.Threading.ThreadAbortException"/>
            /// </param>
            public void Run(int step, int maximum, bool canCancel = false)
            {
                CancelSupport = canCancel;
                //if (ProgressBar == null)
                //{
                    //ProgressBar = _initProgress.ProgressBar;
                    //ProgressBar.InvokeFix(() => ProgressBar.Style = ProgressBarStyle.Continuous);
                    Step = step;
                    Maximum = maximum;
                //}
                Started?.Invoke(this, new EventArgs());
                IsRun = true;
                _initProgress.DoInit();
            }

            /// <summary>
            /// Завершение выполнения операции
            /// </summary>
            public void Finish(bool cancel = false)
            {
                //if (ProgressBar != null)
                //{
                    if (cancel)
                    {
                        //if (ProgressBar.InvokeRequired)
                        //    ProgressBar.Invoke(new Action(() => { ProgressBar.Enabled = false; }));
                        //else
                        //    ProgressBar.Enabled = false;
                    }
                    else
                        Value = Maximum;
                //}
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

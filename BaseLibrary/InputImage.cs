using Emgu.CV;
using System;
using System.Windows.Forms;

namespace BaseLibrary
{
    public class InputImage
    {
        public int ID { get; }
        public IImage Image { get; }
        public Type ImageType { get; }
        public Type TColor { get; }
        public Type TDepth { get; }
        public ProgressInfo Progress { get; }
        public static IImage Convert<TColor, TDepth>(IImage image) where TColor : struct, IColor where TDepth : new()
        {
            dynamic t = image;
            return t.Convert<TColor, TDepth>();
        }

        public InputImage(IImage image, int ID)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
            this.ID = ID;
            ImageType = image.GetType();
            Type[] t = ImageType.GetGenericArguments();
            if (t.Length == 2)
            {
                TColor = t[0];
                TDepth = t[1];
            }
            Progress = new ProgressInfo(this);
        }

        public class ProgressInfo
        {
            //public void Show();
            internal InputImage _inputImage;
            /// <summary>
            /// Возвращает или задает наибольшее значение диапозона элемента управления <see cref="ProgressBar"/>
            /// </summary>
            public int Maximum
            {
                get => progressBar.Maximum;
                set => progressBar.Invoke(new Action(() => { progressBar.Maximum = value; }));
            }

            /// <summary>
            /// Возвращает или задает наименьшее значение диапозона элемента управления <see cref="ProgressBar"/>
            /// </summary>
            public int Minimum
            {
                get => progressBar.Minimum;
                set => progressBar.Invoke(new Action(() => { progressBar.Minimum = value; }));
            }

            /// <summary>
            /// Возвращает или задает текущее положение индикатора выполнения
            /// </summary>
            public int Value
            {
                get => progressBar.Value;
                set => progressBar.Invoke(new Action(() => { progressBar.Value = value; }));
            }

            /// <summary>
            /// Возвращает или задает интервал, в котором вызов <see cref="PerformStep()"/> метод увеличивает текущее положение индикатора хода выполнения
            /// </summary>
            public int Step
            {
                get => progressBar.Step;
                set => progressBar.Invoke(new Action(() => { progressBar.Step = value; }));
            }

            /// <summary>
            /// Возвращает или задает способ отображения выполнения в индикаторе выполнения
            /// </summary>
            public ProgressBarStyle Style
            {
                get => progressBar.Style;
                set => progressBar.Invoke(new Action(() => progressBar.Style = value));
            }

            /// <summary>
            /// Увеличивает текущую позицию индикатора хода выполнения на объем <see cref="Step"/> свойство
            /// </summary>
            public void PerformStep()
            {
                progressBar.Invoke(new Action(() => progressBar.PerformStep()));
            }

            /// <summary>
            /// Инициализация прогресса
            /// </summary>
            /// <param name="step">Интервал, в котором вызов <see cref="PerformStep()"/> метод увеличивает текущее положение индикатора хода выполнения</param>
            /// <param name="maximum">Наибольшее значение диапозона элемента управления <see cref="ProgressBar"/>
            public void Run(int step, int maximum)
            {
                if (progressBar == null)
                {
                    progressBar = BaseMethods._getProgressBar(_inputImage);
                    Step = step;
                    Maximum = maximum;
                }
            }

            /// <summary>
            /// Завершение выполнения операции
            /// </summary>
            public void Finish()
            {
                if (progressBar != null)
                {
                    //Будут доработки, но метод уже можно (даже желательно) использовать
                    Value = Maximum;
                }
            }
            ProgressBar progressBar;
            internal ProgressInfo(InputImage inputImage)
            {
                _inputImage = inputImage;
            }
        }
    }
}

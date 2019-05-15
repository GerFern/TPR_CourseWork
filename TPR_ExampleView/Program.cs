using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TPR_ExampleView
{
    static class Program
    {
        public static Form1 mainForm;
        public static bool catchException = true;
        public static bool debugException = false;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Environment.ProcessorCount.ToString();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.ThreadException +=
            //    new System.Threading.ThreadExceptionEventHandler((object o, System.Threading.ThreadExceptionEventArgs e) =>
            //        { MessageBox.Show(e.Exception.Message, "Необработанное исключение"); mainForm.SetExceptionError(e.Exception); Debugger.Launch(); Debugger.Break(); });

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            mainForm = new Form1();
            Application.Run(mainForm);
        }

        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            StackFrame stackFrame = new StackFrame();
            StackTrace stackTrace = new StackTrace();
            if (!stackTrace.GetFrames().Any(a => a.GetMethod().GetCustomAttribute<BaseLibrary.DontCatchException>() != null))
            {
                if (catchException)
                {
                    if (mainForm.InvokeRequired)
                        mainForm.Invoke(new Action(() => Message(e)));
                    else Message(e);

                    //Уведомление об ошибке
                    if (e.Exception is System.Threading.ThreadAbortException) return;
                    if (!(e.Exception is TargetInvocationException || mainForm == null || mainForm.IsDisposed))
                        mainForm.AddException(e.Exception, false);
                }
            }
        }

        private static void Message(System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            if (debugException && MessageBox.Show(mainForm, $"{e.Exception.ToString()}{Environment.NewLine}Приостановть выполнение кода?", "Исключение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (Debugger.IsAttached)
                    Debugger.Break();
                //Выберите в отладчике поток с нужным именем
                //Проверьте стек вызовов. Код остановлен до перехода в catch
                ///Чтобы не показывать сообщение об исключениях, пометьте метод атрибутом <see cref="BaseLibrary.DontCatchException"/>
                else Debugger.Launch();
            }
        }
    }
}

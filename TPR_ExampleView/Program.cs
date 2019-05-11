using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace TPR_ExampleView
{
    static class Program
    {
        static Form1 form;
        public static bool debugException = false;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException +=
                new System.Threading.ThreadExceptionEventHandler((object o, System.Threading.ThreadExceptionEventArgs e) => 
                    { MessageBox.Show(e.Exception.Message, "Необработанное исключение"); form.SetExceptionError(e.Exception); Debugger.Launch(); Debugger.Break(); });

            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            form = new Form1();
            Application.Run(form);
        }

        [DebuggerNonUserCode]
        private static void CurrentDomain_FirstChanceException(object sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            if (debugException)
            {
                if (MessageBox.Show($"{e.Exception.ToString()}{Environment.NewLine}Приостановть выполнение кода?", "Исключение", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();
                    //Проверьте стек вызовов. Код остановлен до перехода в catch
                    else Debugger.Launch();
                }
            }

            //Уведомление об ошибке
            if (!(e.Exception is TargetInvocationException||form==null||form.IsDisposed))
                form.AddException(e.Exception, false);
        }
    }
}

using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseLibrary
{
    public partial class Form1 : BaseForm
    {
        public Form1(IImage image, System.Reflection.MethodInfo methodInfo) : base(image, methodInfo)
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Массив Vs - Массив параметров для метода
            //Для данной формы используется метод GausForm(IImage image, string s1, string s2, int kernelSize)
            //Vs[0] автоматически определяется изображением
            //Остальные параметры нужно заполнять самому в событии FormClosing, можно подругому, если придумаете как
            //Главное чтобы были заполнены параметры и можно было вызвать метод Accept(),
            //в котором будет произведено выполнение пользовательского метода GausForm(Vs[0], Vs[1], Vs[2], Vs[3])
            //Если произойдет ошибка, метод Accept() вернет false, иначе true

            //Метод Accept можно перегрузить по своему желанию, например для добавления своих обработчиков исключений,
            //ну или чего душа пожелает

            //Если была нажата кнопка OK
            if (DialogResult == DialogResult.OK)
            {
                if (radioButton1.Checked) Vs[1] = "A";
                else if (radioButton2.Checked) Vs[1] = "B";
                else if (radioButton3.Checked) Vs[1] = "C";
                else if (radioButton4.Checked) Vs[1] = "D";
                Vs[2] = textBox1.Text;
                Vs[3] = (int)numericUpDown1.Value;
                
                //Необходимо вызвать метод Accept(), чтобы сохранить изменения
                //В случае возникновения ошибок закрытие формы будет отменено
                e.Cancel = !base.Accept();
            }
        }

        //Исходный код метода Accept, который можно переопределить
        //protected override bool Accept()
        //{
        //    try
        //    {
        //        outputImage = (OutputImage)MethodInfo.Invoke(null, Vs);
        //        return true;
        //    }
        //    catch (System.Reflection.TargetInvocationException ex)//Обработка исключений в MethodInfo
        //    {
        //        MessageBox.Show(ex.InnerException.Message, "Ошибка");
        //        return false;
        //    }
        //    catch (Exception ex)//Базовый Exception лучше использовать в последнюю очередь
        //    {
        //        MessageBox.Show(ex.Message, "Ошибка");
        //        return false;
        //    }
        //}
    }
}

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
        public Form1(InputImage inputImage, System.Reflection.MethodInfo methodInfo) : base(inputImage, methodInfo)
        {
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Массив Vs - Массив параметров для метода
            //Для данной формы используется метод GausForm(InputImage, int, int, int)
            //Vs[0] автоматически определяется изображением и его лучше не перезаписывать
            //Остальные параметры нужно заполнять самому в событии FormClosing, можно подругому, если придумаете как
            //Главное чтобы были заполнены параметры
            //Метод выполнится, если форма вернет ShowDialog.OK и не был вызван базовый Accept (Свойство IsInvoked будет равно false)
            
            //Метод Accept можно перегрузить по своему желанию, например для добавления своих обработчиков исключений,
            //ну или чего душа пожелает

            //Если была нажата кнопка OK
            if (DialogResult == DialogResult.OK)
            {
                Vs[1] = (int)numericUpDown2.Value;
                Vs[2] = (int)numericUpDown3.Value;
                Vs[3] = (int)numericUpDown1.Value;
                
                //Необходимо вызвать метод Accept(), чтобы сохранить изменения
                //В случае возникновения ошибок закрытие формы будет отменено

                //Уже нет в этом необходимости
                //Однако можно продолжить использование, чтобы прерывать закрытие формы во время ошибок
                //Но в данном случае невозможно будет применить к нескольким изображениям

                //e.Cancel = !base.Accept();
            }
        }
    }
}

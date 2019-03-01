using BaseLibrary;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestLibrary
{
    public partial class Debug : BaseForm
    {
        //IImage img;
        public MethodInfo[] methods;
        public ParameterInfo[] parameters;
        public ParameterInfo ret;
        public MethodInfo method;
        public object[] vs;
        public Control[] controls;
        public Debug(IImage img, MethodInfo methodInfo) : base(img,methodInfo)
        {
            InitializeComponent();
            //this.img = img;
            imageBox1.Image = img;
            PropertyGrid propertyGrid = new PropertyGrid()
            {
                Parent = tabPage1,
                SelectedObject = imageBox1.Image,
                Dock = DockStyle.Fill
            };
            Type type = img.GetType();
            methods = type.GetMethods();
            for (int i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                tableLayoutPanel2.Controls.Add(new Label { Text = method.Name }, 0, i);
                Button button = new Button
                {
                    Tag = method,
                };
                tableLayoutPanel2.Controls.Add(button, 1, i);
                button.Click += Button_Click;
            }
            tableLayoutPanel2.RowCount = methods.Length;
            tableLayoutPanel2.AutoSize = true;
            tableLayoutPanel2.AutoScroll = true;
        }

        private void Button_Click(object sender, EventArgs e)
        {
            method = (MethodInfo)((Button)sender).Tag;
            ret = method.ReturnParameter;
            parameters = method.GetParameters();
            vs = new object[parameters.Length];
            controls = new Control[parameters.Length];
            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.ColumnCount = 2;
            tlp.RowCount = parameters.Length + 1;
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameterInfo = parameters[i];
                tlp.Controls.Add(new Label { Text = parameterInfo.Name }, 0, i);
                Control control = (TypeDescriptor.GetEditor(parameterInfo.ParameterType, typeof(System.Drawing.Design.UITypeEditor))) as Control;
                if (control == null)
                {
                    TypeConverter tc = TypeDescriptor.GetConverter(parameterInfo.ParameterType);
                    control = new TextBox();
                    control.Enabled = tc.CanConvertFrom(typeof(string));
                    if (!control.Enabled)
                    {
                        control.Text = (vs[i] = Activator.CreateInstance(parameterInfo.ParameterType)).ToString();
                    }
                    tlp.Controls.Add(control, 1, i);
                }
                controls[i] = control;
            }
            FlowLayoutPanel flp = new FlowLayoutPanel();
            Button BOK = new Button { Text = "OK", DialogResult = DialogResult.OK,Parent = flp };
            Button BCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Parent = flp };
            tlp.Controls.Add(flp, 0, tlp.RowCount - 1);
            tlp.SetColumnSpan(flp, 2);
            tlp.Dock = DockStyle.Fill;
            Form form = new Form();
            form.Tag = this;
            tlp.Parent = form;
            form.FormClosing += Form_FormClosing;
            form.ShowDialog();
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
            {
                for (int i = 0; i < vs.Length; i++)
                {
                    if (controls[i].Enabled)
                    {
                        var t = TypeDescriptor.GetConverter(parameters[i].ParameterType);
                        vs[i] = t.ConvertFromString(controls[i].Text);
                    }
                }
                dynamic result = method.Invoke(imageBox1.Image, vs);
                Type typeResult = result.GetType();
                if (result is IImage)
                {
                    if (MessageBox.Show("Заменить изображение?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        imageBox1.Image = result;
                    }
                }
                else MessageBox.Show(result.ToString());
                imageBox1.Update();
            }

        }

        private void Debug_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.OK)
                e.Cancel = !Accept();
        }
    }
}

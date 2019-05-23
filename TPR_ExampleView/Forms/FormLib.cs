using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TPR_ExampleView.Forms
{
    public partial class FormLib : Form
    {
        private const string strNull = "Null";

        AssemblyItem AssemblyItem { get; }
        AssemblyItem.ClassItem CurrentClass => comboBox1.SelectedValue == null ? null : (AssemblyItem.ClassItem)comboBox1.SelectedValue;
        MyMethodInfo CurrentMethodInfo => listBox1.SelectedValue == null ? null : (MyMethodInfo)listBox1.SelectedValue;

        private FormLib()
        {
            InitializeComponent();
        }

        internal FormLib(AssemblyItem assemblyItem) : this()
        {
            AssemblyItem = assemblyItem ?? throw new ArgumentNullException(nameof(assemblyItem));
            this.Text = AssemblyItem.Assembly.GetName().Name;
        }

        private void FormLib_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = AssemblyItem.Classes;
            lFullName.Text = AssemblyItem.Assembly.FullName;
        }

        private void ComboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            listBox1.DataSource = CurrentClass?.Methods;
            if (CurrentClass != null)
            {
                var authors = CurrentClass.Authors;
                if (authors != null && authors.Length > 0)
                {
                    string t = String.Empty;
                    foreach (var item in authors.Take(authors.Length - 1))
                        t += item + ", ";
                    t += authors.Last();
                    lAuthros.Text = t;
                }
                else lAuthros.Text = strNull;
            }
            else lAuthros.Text = strNull;
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CurrentMethodInfo != null)
            {
                var hierarchy = CurrentMethodInfo.Hierarchy;
                string t = string.Empty;
                if (hierarchy != null && hierarchy.Length > 0)
                {
                    foreach (var item in hierarchy.Take(hierarchy.Length - 1))
                        t += item + ", ";
                    t += hierarchy.Last();
                    lHierarchy.Text = t;
                }
                else lHierarchy.Text = strNull;
                if (CurrentMethodInfo.CustomForm != null)
                    lType.Text = "С собственной формой";
                else if (CurrentMethodInfo.IsAutoForm)
                    lType.Text = "С автоматической формой";
                else
                    lType.Text = "Без формы";
                var vs = CurrentMethodInfo.MethodInfo.GetParameters();
                if (vs.Length > 0)
                {
                    t = string.Empty;
                    foreach (var item in vs.Take(vs.Length - 1))
                        t += $"{item.ParameterType.Name} {item.Name}, ";
                    t += $"{vs.Last().ParameterType.Name} {vs.Last().Name}";
                }
                else t = "Empty";
                lParam.Text = t;
            }
            else
            {
                lHierarchy.Text = strNull;
                lType.Text = strNull;
                lParam.Text = strNull;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            MenuMethod.RunMethodInfo(CurrentMethodInfo);
        }
    }
}

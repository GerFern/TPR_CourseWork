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
    public partial class FormSettings : Form
    {
        bool multiImage;
        bool saveNewImageToFile;
        string dir;
        string mask;
        int extIndex;
        public FormSettings()
        {
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (System.IO.Directory.Exists(textBox1.Text)) fbd.SelectedPath = textBox1.Text;
            fbd.SelectedPath = textBox1.Text;
            if (fbd.ShowDialog() == DialogResult.OK)
                textBox1.Text = fbd.SelectedPath;
        }

        private void Label2_DoubleClick(object sender, EventArgs e)
        {
            Form f = new Form() { Text = "Подсказка", Size = new Size(600, 400) };
            new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font(Font.FontFamily, 14),
                Text = new ComponentResourceManager(typeof(FormSettings)).GetString("label2.ToolTip"),
                TextAlign = ContentAlignment.MiddleLeft,
                Parent = f
            };
            f.Show();
        }

        public static string FormatMask(string mask, string imageName, string mName, string bName, string aName, DateTime startAll, DateTime startThis, DateTime endThis, int nTask, string dateTimeFormat)
        {
            //%f - Имя изображения
            //%m - Название метода (оригинальное название)
            //%n - Название метода (кнопка меню)
            //%N - Название метода (через атрибут)
            //%t - Дата начала выполнения всех задач
            //%d - Дата начала выполнения задачи
            //%D - Дата конца выполнения задачи
            //%i - Номер задачи
            //%u - Инкрементное значение, для избежания совпадений имен. Используется, если выключен параметр перезаписывать существующие файлы
            string fres = string.Empty;
            string res = string.Empty;
            bool uSeparator = false;
            CharEnumerator vs = mask.GetEnumerator();
            while (vs.MoveNext())
            {
                char c = vs.Current;
                if (c == '%')
                {
                    if (vs.MoveNext())
                    {
                        switch (vs.Current)
                        {
                            case 'f':
                                res += imageName;
                                break;
                            case 'm':
                                res += mName;
                                break;
                            case 'n':
                                res += bName;
                                break;
                            case 'N':
                                res += aName;
                                break;
                            case 't':
                                res += startAll.ToString(dateTimeFormat);
                                break;
                            case 'd':
                                res += startThis.ToString(dateTimeFormat);
                                break;
                            case 'D':
                                res += endThis.ToString(dateTimeFormat);
                                break;
                            case 'i':
                                res += nTask.ToString();
                                break;
                            case 'u':
                                if (!uSeparator)
                                {
                                    fres = res;
                                    res = String.Empty;
                                    uSeparator = true;
                                }
                                break;
                            default:
                                res += c + vs.Current;
                                break;
                        }
                    }
                }
                else res += c;
            }
            return res;
        }

        public static string FormatMask(string mask, string imageName, string startThis, string endThis, string nTask, out int uSeparatorIndex)
        {
            //%f - Имя изображения
            //%m - Название метода (оригинальное название)
            //%n - Название метода (кнопка меню)
            //%N - Название метода (через атрибут)
            //%t - Дата начала выполнения всех задач
            //%d - Дата начала выполнения задачи
            //%D - Дата конца выполнения задачи
            //%i - Номер задачи
            //%u - Инкрементное значение, для избежания совпадений имен. Используется, если выключен параметр перезаписывать существующие файлы
            string res = string.Empty;
            bool uSeparator = false;
            uSeparatorIndex = -1;
            CharEnumerator vs = mask.GetEnumerator();
            int index = 0;
            while (vs.MoveNext())
            {
                char c = vs.Current;
                if (c == '%')
                {
                    if (vs.MoveNext())
                    {
                        switch (vs.Current)
                        {
                            case 'f':
                                res += imageName;
                                index += imageName.Length;
                                break;
                            case 'd':
                                res += startThis;
                                index += startThis.Length;
                                break;
                            case 'D':
                                res += endThis;
                                index += endThis.Length;
                                break;
                            case 'i':
                                res += nTask.ToString();
                                index += nTask.Length;
                                break;
                            case 'u':
                                if (!uSeparator)
                                {
                                    uSeparatorIndex = index;
                                }
                                break;
                            default:
                                res += c + vs.Current;
                                index++;
                                break;
                        }
                    }
                    else res += c;
                }
                else
                {
                    res += c;
                    index++;
                }
            }
            return res;
        }

        public static string FormatMask(string mask, string uStr, int uIndex)
        {
            return mask.Insert(uIndex, uStr);
        }

        public static string FormatMask(string mask, string mName, string bName, string aName, string startAll)
        {
            string res = string.Empty;
            CharEnumerator vs = mask.GetEnumerator();
            while (vs.MoveNext())
            {
                char c = vs.Current;
                if (c == '%')
                {
                    if (vs.MoveNext())
                    {
                        switch (vs.Current)
                        {
                            case 'm':
                                res += mName;
                                break;
                            case 'n':
                                res += bName;
                                break;
                            case 'N':
                                res += aName;
                                break;
                            case 't':
                                res += startAll;
                                break;
                            default:
                                res += c + vs.Current;
                                break;
                        }
                    }
                }
                else res += c;
            }
            return res;
        }
        private void FormSettings_Load(object sender, EventArgs e)
        {
            rbSaveFileImageOff.Tag = rbMultiImageOff.Tag = false;
            rbSaveFileImageOn.Tag = rbMultiImageOn.Tag = true;
            if (Properties.Settings.Default.MultiImage)
            {
                rbMultiImageOn.Checked = true;
                multiImage = true;
            }
            else
                rbMultiImageOff.Checked = true;
            if (Properties.Settings.Default.SaveNewImageToFile)
            {
                rbSaveFileImageOn.Checked = true;
                saveNewImageToFile = true;
            }
            else
                rbSaveFileImageOff.Checked = true;
            //rbMultiImageOff.CheckedChanged += RbMultiImage_CheckedChanged;
            //rbMultiImageOn.CheckedChanged += RbMultiImage_CheckedChanged;
            //rbSaveFileImageOff.CheckedChanged += RbSaveFileImage_CheckedChanged;
            //rbSaveFileImageOn.CheckedChanged += RbSaveFileImage_CheckedChanged;
            textBox1.Text = Properties.Settings.Default.SaveImageDir;
            textBox2.Text = Properties.Settings.Default.SaveFileNameMask;
            comboBox1.DataSource = BaseLibrary.Extensions.GetExtList(true, true);
            comboBox1.SelectedIndex = Properties.Settings.Default.SaveFileFormatIndex;
            checkBox1.Checked = Properties.Settings.Default.SaveFileReplace;
        }

        private void FormSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.MultiImage = rbMultiImageOn.Checked;
            Properties.Settings.Default.SaveNewImageToFile = rbSaveFileImageOn.Checked;
            Properties.Settings.Default.SaveImageDir = textBox1.Text;
            Properties.Settings.Default.SaveFileNameMask = textBox2.Text;
            Properties.Settings.Default.SaveFileFormatIndex = (byte)comboBox1.SelectedIndex;
            Properties.Settings.Default.SaveFileReplace = checkBox1.Checked;
        }
    }
}

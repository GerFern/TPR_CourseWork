//#define test
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BaseLibrary;
using System.Windows.Forms;
using Emgu.CV;
using Newtonsoft.Json.Linq;
using System.Drawing;

namespace TPR_ExampleView
{
    internal class FormP : Form
    {
        public object[] vs;
        MyMethodInfo methodInfo;
        public IImage Image { get; }
        Button OK = new Button { Text = "OK", DialogResult = System.Windows.Forms.DialogResult.OK };
        Button Cancel = new Button { Text = "Отмена", DialogResult = System.Windows.Forms.DialogResult.Cancel };
        Control[] controls;
        string[] propertyStr;
        PropertyInfo[] propertyInfos;
        int[] indeces;
        ParameterInfo[] parameters;
        FlowLayoutPanel flp = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Fill
        };
        TableLayoutPanel tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
        };
        void SaveParams()
        {
            JObject jRoot;
            string name = methodInfo.MethodInfo.Name;
            if (!Directory.Exists("Params"))
                Directory.CreateDirectory("Params");
            string asm = $"Params\\{methodInfo.Module.Name}.json";
            if (File.Exists(asm))
                jRoot = JObject.Parse(File.ReadAllText(asm));
            else
                jRoot = new JObject();
            Dictionary<String, String> vs = new Dictionary<string, string>();
            JObject jDict = new JObject();
            for (int i = 0; i < controls.Length; i++)
            {
                jDict[parameters[i].Name] = Newtonsoft.Json.JsonConvert.SerializeObject(propertyInfos[i].GetValue(controls[i]));
            }
            //foreach (Control item in controls)
            //{
            //    jDict[((ParameterInfo)item.Tag).Name] = item.Text;
            //    //vs.Add(((ParameterInfo)item.Tag).Name, item.Text);
            //}
            //Newtonsoft.Json.Linq.JObject jDict = new Newtonsoft.Json.Linq.JObject(vs);
            jDict.ToString();
            jRoot[name] = jDict;
            File.WriteAllText(asm, jRoot.ToString());
        }
        void LoadParams()
        {
            JObject jRoot;
            JObject jDict;
            string name = methodInfo.MethodInfo.Name;
            if (!Directory.Exists("Params"))
                Directory.CreateDirectory("Params");
            string asm = $"Params\\{methodInfo.Module.Name}.json";
            if (File.Exists(asm))
            {
                jRoot = JObject.Parse(File.ReadAllText(asm));
                jDict = (JObject)jRoot[methodInfo.MethodInfo.Name];
                //try
                //{
                if (jDict != null)
                {
                    foreach (var item in jDict)
                    {
                        try
                        {
                            int index = -1;
                            for (int i = 0; i < parameters.Length; i++)
                            {
                                if (parameters[i].Name == item.Key)
                                {
                                    index = i;
                                    break;
                                }
                            }
                            if (index >= 0)
                            {
                                //object nVal = ((IConvertible)item.Value).ToType(propertyInfos[index].PropertyType, System.Globalization.CultureInfo.CurrentCulture);
                                Type retType = propertyInfos[index].PropertyType;
                                object nVal = Newtonsoft.Json.JsonConvert.DeserializeObject(item.Value.ToString(), retType);
                                propertyInfos[index].SetValue(controls[index], nVal);
                            }
                        }
                        catch (Exception ex)
                        { }
                        //if (control != null)
                        //{
                        //    control.Text = item.Value.ToObject<string>();
                        //}
                    }
                }
                //}
                //catch { }
            }
        }
        public FormP(MyMethodInfo methodInfo, IImage image) : base()
        {
            PropertyInfo propertyText = typeof(TextBox).GetProperty("Text");
            int heigth = 80;
            this.AcceptButton = this.OK;
            this.CancelButton = this.Cancel;
            this.Image = image;
            this.methodInfo = methodInfo;
            this.SuspendLayout();
            tlp.Parent = this;
            tlp.RowCount = methodInfo.AutoAndControlForms.Length * 2 + 1;
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            controls = new Control[methodInfo.AutoAndControlForms.Length];
            propertyInfos = new PropertyInfo[methodInfo.AutoAndControlForms.Length];
            propertyStr = new string[methodInfo.AutoAndControlForms.Length];
            indeces = new int[methodInfo.AutoAndControlForms.Length];
            parameters = new ParameterInfo[methodInfo.AutoAndControlForms.Length];
            var unsortParams = methodInfo.MethodInfo.GetParameters();
            for (int i = 0; i < methodInfo.AutoAndControlForms.Length; i++)
            {
                TPRFormAttribute attribute = methodInfo.AutoAndControlForms[i];
                RowStyle rowStyle = new RowStyle(SizeType.Absolute, 26);
                if(attribute is AutoFormAttribute autoForm)
                {
                    TextBox tb;
                    controls[i] = tb = new TextBox { Dock = DockStyle.Fill};
                    propertyStr[i] = "Text";
                    propertyInfos[i] = propertyText;
                    indeces[i] = autoForm.Index;
                    parameters[i] = unsortParams[autoForm.Index];
                    if(autoForm.IsMultiline)
                    {
                        tb.Multiline = true;
                        rowStyle.Height = tb.Height = autoForm.TextBoxHeigth;
                    }
                    tlp.Controls.Add(new Label { Text = autoForm.LabelText, Dock = DockStyle.Bottom }, 0, i * 2);
                    tlp.Controls.Add(tb, 0, i * 2 + 1);
                }
                else if(attribute is ControlFormAttribute controlForm)
                {
                    Control control;
                    controls[i] = control = (Control)Activator.CreateInstance(controlForm.ControlType);
                    propertyInfos[i] = controlForm.ControlType.GetProperty(propertyStr[i] = controlForm.Property);
                    indeces[i] = controlForm.Index;
                    parameters[i] = unsortParams[controlForm.Index];
                    control.Dock = DockStyle.Fill;
                    if (methodInfo.DictControlProperties.ContainsKey(controlForm.Index))
                        foreach (var item in methodInfo.DictControlProperties[controlForm.Index])
                        {
                            if (item.PropertyName == "Height") rowStyle.Height = float.Parse(item.PropertyValue);
                            //try
                            //{
                            PropertyInfo property = controlForm.ControlType.GetProperty(item.PropertyName);
                            Type propertyType = property.PropertyType;
                            object nVal = ((IConvertible)item.PropertyValue).ToType(propertyType, System.Globalization.CultureInfo.CurrentCulture);
                            property.SetValue(control, nVal);
                            //}
                            //catch
                            //{

                            //}
                        }
                    tlp.Controls.Add(new Label { Text = controlForm.LabelText, Dock = DockStyle.Bottom }, 0, i * 2);
                    tlp.Controls.Add(control, 0, i * 2 + 1);
                }
                if (string.IsNullOrWhiteSpace(attribute.LabelText))
                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
                else
                {
                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
                    heigth += 26;
                }
                tlp.RowStyles.Add(rowStyle);
                heigth += (int)rowStyle.Height;
            }
            flp.Controls.Add(OK);
            flp.Controls.Add(Cancel);
            flp.Dock = DockStyle.Fill;
            tlp.Controls.Add(flp, 0, tlp.RowCount - 1);
            LoadParams();
            this.Height = heigth;
            this.ResumeLayout();
            FormClosing += delegate (object sender, FormClosingEventArgs e)
            {
                if (DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    vs = new object[methodInfo.MethodInfo.GetParameters().Length];
                    vs[0] = methodInfo.IsInputImage ? new InputImage(image, -1, methodInfo.MethodName) : (object)image;
                    for (int i = 0; i < controls.Length; i++)
                    {
                        object val = null;
                        TPRFormAttribute pFormAttribute = methodInfo.AutoAndControlForms[i];
                        try
                        {
                            val = propertyInfos[i].GetValue(controls[i]);
                            vs[pFormAttribute.Index] = Convert.ChangeType(val, unsortParams[pFormAttribute.Index].ParameterType);
                            //vs[i + 1] = ((IConvertible)controls[i].Text).ToType(Types[i], System.Globalization.CultureInfo.CurrentCulture);
                        }
                        catch (Exception ex)
                        {
                            if (val is string str && str.Contains("."))
                            {
                                str.Replace('.', ',');
                                try
                                {
                                    vs[pFormAttribute.Index] = Convert.ChangeType(str, unsortParams[pFormAttribute.Index].ParameterType);
                                    continue;
                                }
                                catch { }
                            }
                            e.Cancel = true;
                            MessageBox.Show($"Неверный параметр {pFormAttribute.LabelText} ({i})\r\n{ex.Message}");
                        }
                    }
                }
                SaveParams();
            };


            //var formAuto = methodInfo.AutoForms;
            //Types = new Type[formAuto.Length];
            //textBoxes = new System.Windows.Forms.TextBox[formAuto.Length];
            //this.image = image;
            //this.methodInfo = methodInfo;
            //flp.Controls.Add(OK);
            //flp.Controls.Add(Cancel);
            //tlp.RowCount = (formAuto.Length * 2 + 1);
            //int l = tlp.RowCount - 1;
            //this.Height = 80;
            //for (int i = 0; i < formAuto.Length; i++)
            //{
            //    AutoFormAttribute item = formAuto.ElementAt(i);
            //    Types[i] = item.Type;
            //    tlp.Controls.Add(new System.Windows.Forms.Label { Text = item.LabelText, Dock = System.Windows.Forms.DockStyle.Bottom }, 0, i * 2);
            //    tlp.Controls.Add(textBoxes[i] = new System.Windows.Forms.TextBox { Dock = System.Windows.Forms.DockStyle.Fill, Tag = methodInfo.MethodInfo.GetParameters()[item.Index] }, 0, i * 2 + 1);
            //    tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26));
            //    int h;
            //    if (item.IsMultiline) h = item.TextBoxHeigth;
            //    else h = 26;
            //    tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, h));
            //    Height += 26 + h;
            //}
            //tlp.Controls.Add(flp, 0, l);
            //Controls.Add(tlp);
            //LoadParams();
            //FormClosing += delegate (object sender, System.Windows.Forms.FormClosingEventArgs e)
            //{
            //    if (DialogResult == System.Windows.Forms.DialogResult.OK)
            //    {
            //        vs = new object[textBoxes.Length + 1];
            //        vs[0] = methodInfo.IsInputImage ? new InputImage(image, MenuMethod.MainForm.CreateTask(methodInfo)) : (object)image;
            //        for (int i = 0; i < Types.Length; i++)
            //        {
            //            try
            //            {
            //                vs[i + 1] = ((IConvertible)textBoxes[i].Text).ToType(Types[i], System.Globalization.CultureInfo.CurrentCulture);
            //            }
            //            catch
            //            {
            //                e.Cancel = true;
            //                System.Windows.Forms.MessageBox.Show("Неверный параметр " + i);
            //            }
            //        }
            //    }
            //    SaveParams();
            //};
        }
    }
}

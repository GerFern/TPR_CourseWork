//#define test
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using BaseLibrary;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace TPR_ExampleView
{
    internal class AssemblyItem
    {
        static Type TypeDefOutputImage = typeof(BaseLibrary.OutputImage);
        static Type TypeDefInputImage = typeof(BaseLibrary.InputImage);
        static Type TypeDefIImage = typeof(Emgu.CV.IImage);
        public class ClassItem
        {
            public Type TypeClass { get; }
            public List<MyMethodInfo> Methods { get; }
            public ClassItem(Type TypeClass)
            {
                this.TypeClass = TypeClass;
                Methods = new List<MyMethodInfo>();
            }
        }
        public Assembly Assembly { get; }
        public List<ClassItem> Classes { get; }
        public AssemblyItem(Assembly assembly)
        {
            Assembly = assembly;
            Classes = new List<ClassItem>();
            foreach (var item in assembly.ExportedTypes)
            {
                ImgClassAttribute imgClass = item.GetCustomAttribute<ImgClassAttribute>();
                if (imgClass != null)
                {
                    ClassItem classItem = new ClassItem(item);
                    Classes.Add(classItem);
                    foreach (var item2 in item.GetMethods())
                    {
                        ImgMethod imgMethod = item2.GetCustomAttribute<ImgMethod>();
                        if (imgMethod != null)
                        {
                            if (item2.ReturnType == TypeDefOutputImage)
                            {
                                ParameterInfo[] parameters = item2.GetParameters();
                                ParameterInfo p = parameters.First();
                                Type t = p.ParameterType;
                                if (t == TypeDefIImage || t.GetInterfaces().Contains(TypeDefIImage))
                                {
                                    MyMethodInfo mmi = new MyMethodInfo(item2, false);
                                    classItem.Methods.Add(mmi);
                                    if (imgMethod.Hierarchy.Length >= 1)
                                    {
                                        MenuMethod.Add(mmi);
                                    }
                                }
                                else if (t == TypeDefInputImage)
                                {
                                    MyMethodInfo mmi = new MyMethodInfo(item2, true);
                                    classItem.Methods.Add(mmi);
                                    if (imgMethod.Hierarchy.Length >= 1)
                                    {
                                        MenuMethod.Add(mmi);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Класс инициализации меню методов. Каждый объект представляет собой кнопку меню
    /// </summary>
    internal class MenuMethod
    {
        /// <summary>
        /// Главная форма
        /// </summary>
        public static Form1 MainForm { get; set; }
        /// <summary>
        /// Защита от повтора вызова <see cref="ChangeSelected"/>
        /// </summary>
        static bool repeatResist = false;
        /// <summary>
        /// Выбранная форма
        /// </summary>
        public static BaseLibrary.ImageForm SelectedForm { get; set; }
        /// <summary>
        /// Выбранное изображение
        /// </summary>
        public static IImage SelectedImage { get; set; }
        /// <summary>
        /// Словарь изображений
        /// </summary>
        public static Dictionary<string, Emgu.CV.UI.ImageViewer> images = new Dictionary<string, Emgu.CV.UI.ImageViewer>();
        //public static Emgu.CV.UI.ImageBox imageBox;
        public static TextBox textBox { get; set; }
        //public static IImage image;
        public static Dictionary<string, MenuMethod> Buttons { get; } = new Dictionary<string, MenuMethod>();
        /// <summary>
        /// Меню
        /// </summary>
        public static MenuStrip Menu { get; set; }
        public ToolStripMenuItem MenuItem { get; }
        public MenuMethod Parent { get; }
        /// <summary>
        /// Подкнопки меню
        /// </summary>
        public Dictionary<string, MenuMethod> SubButtons { get; } = new Dictionary<string, MenuMethod>();

        /// <summary>
        /// Список методов, привязанных к кнопке
        /// </summary>
        public List<MyMethodInfo> Methods { get; } = new List<MyMethodInfo>();

        public static void ChangeSelected(object sender, BaseLibrary.EventArgsWithImageForm e)
        {
            if (!repeatResist)
            {
                repeatResist = true;
                if (SelectedForm != null)
                {
                    BaseLibrary.ImageForm t = SelectedForm;
                    SelectedForm = null;
                    t.IsSelected = false;
                    SelectedForm = null;
                    SelectedImage = null;
                }
                if (e.Selected)
                {
                    SelectedForm = e.Form;
                    SelectedImage = e.Image;
                    BaseLibrary.ImageForm.selected = SelectedForm;
                }
                repeatResist = false;
            }
        }
        public static IImage CreateImage(string path, string caption = null)
        {
            IImage t = new Image<Bgr, byte>(path);
            return t;
            //return caption == null ? new ImageViewer(t) : new ImageViewer(t, caption);
            //return new ImageBox { Image = t };
        }
        public static ImageViewer CreateImage(IImage img, string caption = null)
        {
            return caption == null ? new ImageViewer(img) : new ImageViewer(img, caption);
        }
        internal static void Add(MyMethodInfo methodInfo)
        {
            MenuMethod current;
            if (Buttons.ContainsKey(methodInfo.Hierarchy[0]))
            {
                current = Buttons[methodInfo.Hierarchy[0]];//.Methods.Add(methodInfo);
            }
            else
            {
                Buttons.Add(methodInfo.Hierarchy[0], current = new MenuMethod(methodInfo.Hierarchy[0], null));
            }
            foreach (var item in methodInfo.Hierarchy.Skip(1))
            {
                if (current.SubButtons.ContainsKey(item))
                {
                    current = current.SubButtons[item];
                }
                else
                {
                    current.SubButtons.Add(item, current = new MenuMethod(item, current));
                }
            }
            current.Methods.Add(methodInfo);
        }
        public MenuMethod(string Text, MenuMethod parent)
        {
            Parent = parent;
            MenuItem = new ToolStripMenuItem();
            MenuItem.Text = Text;
            MenuItem.Tag = this;//Для связывания со списком методов
            MenuItem.Click += MenuItem_Click;
            if (parent == null)
            {
                Menu.Items.Add(MenuItem);
            }
            else
                parent.MenuItem.DropDownItems.Add(MenuItem);
        }

        /// <summary>
        /// Вызов метода
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedImage == null) return;
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem.DropDownItems.Count > 0) return;
            MenuMethod butTag = menuItem.Tag as MenuMethod;
            MyMethodInfo methodInfo = null;
            //Определение метода
            if (butTag.Methods.Count == 1)
            {
                methodInfo = butTag.Methods.First();
            }
            if (butTag.Methods.Count > 1)
            {
                using (Form f = new Form())
                {
                    TableLayoutPanel tlp = new TableLayoutPanel { RowCount = 1, ColumnCount = 3, Parent = f, Dock = DockStyle.Fill };
                    ComboBox comboBox = new ComboBox();

                }
                //Если есть неоднозначности в выборе метода
                //Доделать с выбором через форму определенного метода
            }


            if (methodInfo != null)
            {
                OutputImage outputImage = null;
                CustomFormAttribute formCustom = methodInfo.CustomForm;
                Thread thread = new Thread(new ParameterizedThreadStart(InvMethod));
                if (formCustom != null)
                {
                    Type formType = formCustom.FormType;
                    BaseForm form;
                    if (methodInfo.IsInputImage) form = Activator.CreateInstance(formType, new InputImage(SelectedImage, MainForm.CreateTask(methodInfo)), methodInfo.MethodInfo) as BaseForm;
                    else form = Activator.CreateInstance(formType, SelectedImage, methodInfo.MethodInfo) as BaseForm;
                    using (form)
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                            thread.Start(new InvParam { TypeInvoke = TypeInvoke.CustomForm, BaseForm = form });
                    }
                }
                else
                {
                    if (methodInfo.IsAutoForm)
                        using (FormP form = new FormP(methodInfo, SelectedImage))
                        {
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                thread.Start(new InvParam { TypeInvoke = TypeInvoke.ParameterizedForm, Vs = form.vs, MethodInfo = methodInfo });
                            }
                        }
                    else
                        thread.Start(new InvParam { TypeInvoke = TypeInvoke.WithoutForm, MethodInfo = methodInfo });
                }
                SelectedForm.UpdateImage();
                //BaseLibrary.BaseMethods.LoadOutputImage(outputImage);
            }
        }

        private class InvParam
        {
            public MyMethodInfo MethodInfo { get; set; }
            public Form Form { get; set; }
            public BaseForm BaseForm { get; set; }
            public FormP FormP { get; set; }
            public object[] Vs { get; set; }
            public TypeInvoke TypeInvoke { get; set; }
        }
        private enum TypeInvoke
        {
            CustomForm, ParameterizedForm, WithoutForm
        }
        private static void InvMethod(object param)
        {
            if (param is InvParam invParam)
            {
                OutputImage outputImage = null;
#if !test
                try
                {
#endif
                    switch (invParam.TypeInvoke)
                    {
                        case TypeInvoke.CustomForm:
                            BaseForm form = invParam.BaseForm;
                            //if(form.Vs[0] is InputImage inputImage)
                            //{
                            //}
                            if (form.IsInvoked)
#pragma warning disable CS0612 // Тип или член устарел
                                outputImage = form.OutputImage;
#pragma warning restore CS0612 // Тип или член устарел
                            else
                                outputImage = form.MethodInfo.Invoke(null, form.Vs) as OutputImage;
                            break;
                        case TypeInvoke.ParameterizedForm:
                            outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, invParam.Vs) as OutputImage;

                            break;
                        case TypeInvoke.WithoutForm:
                            if (invParam.MethodInfo.IsInputImage)
                                outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, new object[] { new InputImage(SelectedImage, MainForm.CreateTask(invParam.MethodInfo)) }) as OutputImage;

                            //    MainForm.Invoke(new Action(()=>
                            //   { outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, new object[] { new InputImage(SelectedImage, MainForm.CreateTask(invParam.MethodInfo)) }) as OutputImage; }
                            //));
                            else outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, new object[] { SelectedImage }) as OutputImage;
                            break;
                        default:
                            break;
                    }
                    SelectedForm.Invoke(new MethodInvoker(() => SelectedForm.Update()));
#if !test
                }
                catch (TargetInvocationException ex)
                {
                    MainForm.SetExceptionError(ex.InnerException);
                    //System.Windows.Forms.MessageBox.Show(ex.InnerException.Message);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
#endif
                if (outputImage != null)
                {
                    BaseLibrary.BaseMethods.LoadOutputImage(outputImage);
                    //    MainForm.Invoke(new MethodInvoker(() =>
                    //    {
                    //        BaseLibrary.BaseMethods.LoadOutputImage(outputImage);
                    //    }));
                }
            }
        }
        public void Clear()
        {
            foreach (var item in Buttons.Values)
                Menu.Items.Remove(item.MenuItem);
            Buttons.Clear();
        }
    }

    internal class MyMethodInfo
    {
        public bool IsAutoForm;
        public bool IsInputImage;
        public MethodInfo MethodInfo { get; }
        public string[] Hierarchy { get => ImgMethod.Hierarchy; }
        public CustomFormAttribute CustomForm { get; }
        public ControlFormAttribute[] ControlForms { get; }
        public ControlPropertyAttribute[] ControlProperties { get; }
        public Dictionary<int, List<ControlPropertyAttribute>> DictControlProperties { get; }
        public AutoFormAttribute[] AutoForms { get; }
        public TPRFormAttribute[] AutoAndControlForms { get; }
        public ImgMethod ImgMethod { get; }
        public Assembly Assembly { get => MethodInfo.Module.Assembly; }
        public Module Module { get => MethodInfo.Module; }
        public MyMethodInfo(MethodInfo methodInfo, bool isInputImage)
        {
            MethodInfo = methodInfo;
            ImgMethod = methodInfo.GetCustomAttribute<ImgMethod>();
            CustomForm = methodInfo.GetCustomAttribute<CustomFormAttribute>();
            AutoForms = methodInfo.GetCustomAttributes<AutoFormAttribute>().ToArray();
            ControlForms = methodInfo.GetCustomAttributes<ControlFormAttribute>().ToArray();
            ControlProperties = methodInfo.GetCustomAttributes<ControlPropertyAttribute>().ToArray();
            IsInputImage = isInputImage;
            IsAutoForm = AutoForms.Length > 0 || ControlForms.Length > 0;
            DictControlProperties = new Dictionary<int, List<ControlPropertyAttribute>>();
            if(IsAutoForm)
            {
                Type typeAutoForm = typeof(AutoFormAttribute);
                Type typeControlForm = typeof(ControlFormAttribute);
                AutoAndControlForms = new TPRFormAttribute[AutoForms.Length + ControlForms.Length];
                IEnumerator<CustomAttributeData> e = methodInfo.CustomAttributes.GetEnumerator();
                for (int i = 0; i < AutoAndControlForms.Length; i++)
                {
                    e.MoveNext();
                    while (!(e.Current.AttributeType == typeAutoForm || e.Current.AttributeType == typeControlForm))
                    {
                        e.MoveNext();
                    }
                    AutoAndControlForms[i] = (TPRFormAttribute)e.Current.Constructor.Invoke(e.Current.ConstructorArguments.Select(a=>a.Value).ToArray());
                }
                foreach (var item in ControlProperties)
                {
                    if(!DictControlProperties.ContainsKey(item.ParamIndex))
                        DictControlProperties.Add(item.ParamIndex, new List<ControlPropertyAttribute>());
                    DictControlProperties[item.ParamIndex].Add(item);
                }
            }
        }
    }

    internal class FormP : System.Windows.Forms.Form
    {
        public object[] vs;
        MyMethodInfo methodInfo;
        IImage image;
        Type[] Types;
        Button OK = new System.Windows.Forms.Button { Text = "OK", DialogResult = System.Windows.Forms.DialogResult.OK };
        Button Cancel = new System.Windows.Forms.Button { Text = "Отмена", DialogResult = System.Windows.Forms.DialogResult.Cancel };
        Control[] controls;
        string[] propertyStr;
        PropertyInfo[] propertyInfos;
        int[] indeces;
        ParameterInfo[] parameters;
        FlowLayoutPanel flp = new System.Windows.Forms.FlowLayoutPanel
        {
            FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft,
            Dock = System.Windows.Forms.DockStyle.Fill
        };
        TableLayoutPanel tlp = new System.Windows.Forms.TableLayoutPanel
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
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
                jDict[parameters[i].Name] = Convert.ToString(propertyInfos[i].GetValue(controls[i]));
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
                            object nVal = ((IConvertible)item.Value).ToType(propertyInfos[index].PropertyType, System.Globalization.CultureInfo.CurrentCulture);
                            propertyInfos[index].SetValue(controls[index], nVal);
                        }
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
            this.image = image;
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
                Attribute attribute = methodInfo.AutoAndControlForms[i];
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
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
                tlp.RowStyles.Add(rowStyle);
                heigth += (int)rowStyle.Height + 26;
            }
            flp.Controls.Add(OK);
            flp.Controls.Add(Cancel);
            flp.Dock = DockStyle.Fill;
            tlp.Controls.Add(flp, 0, tlp.RowCount - 1);
            LoadParams();
            this.Height = heigth;
            this.ResumeLayout();
            FormClosing += delegate (object sender, System.Windows.Forms.FormClosingEventArgs e)
            {
                if (DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    vs = new object[methodInfo.MethodInfo.GetParameters().Length];
                    vs[0] = methodInfo.IsInputImage ? new InputImage(image, MenuMethod.MainForm.CreateTask(methodInfo)) : (object)image;
                    for (int i = 0; i < controls.Length; i++)
                    {
                        TPRFormAttribute pFormAttribute = methodInfo.AutoAndControlForms[i];
                        try
                        {
                            object p = propertyInfos[i].GetValue(controls[i]);
                            vs[pFormAttribute.Index] = Convert.ChangeType(p, unsortParams[pFormAttribute.Index].ParameterType);
                            //vs[i + 1] = ((IConvertible)controls[i].Text).ToType(Types[i], System.Globalization.CultureInfo.CurrentCulture);
                        }
                        catch (Exception ex)
                        {
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

    internal static class DLL_Init
    {
        /// <summary>
        /// Название библиотеки, находящемся в решении, без помещения в папку с библиотеками. Полезно для отладки<para/>Можно не использовать
        /// </summary>
        public static string path = "DLL";
        public static string Log { get; set; }
        public static List<AssemblyItem> assemblies = new List<AssemblyItem>();
        public static void Init(MenuStrip menu)
        {
            MenuMethod.Menu = menu;
            try
            {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            List<string> list = new List<string>();
            if (!File.Exists("dll_load.conf"))
            {
                //  File.Create("dll_load.conf");
                StreamWriter sw = new StreamWriter("dll_load.conf");
                sw.WriteLine("TestLibrary");
                sw.Close();
                list.Add("TestLibrary");
            }
            else
            {
                StreamReader sr = new StreamReader("dll_load.conf");
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    list.Add(line);

                }
                sr.Close();
            }
            foreach(var item in list)
            {
                try
                {
                    assemblies.Add(new AssemblyItem(Assembly.Load(item)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось загрузить {item}{Environment.NewLine}{ex.Message}");
                }
            }
            foreach (var item in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    assemblies.Add(new AssemblyItem(Assembly.LoadFile(Environment.CurrentDirectory + "\\" + item)));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            foreach (var item in assemblies)
            {
                var menuTool = (menu.Items[1] as ToolStripMenuItem).DropDownItems.Add(item.Assembly.Modules.First().Name);
                menuTool.Enabled = false;
            }
        }
    }

    internal class IImageInfo
    {
        IImage Image { get; set; }
        string Text { get; set; }
        string FileImgPath { get; set; }
        bool SelectedMult { get; set; }
        bool IsLoaded { get; set; }
        bool IsDisposed { get; set; }

    }
}

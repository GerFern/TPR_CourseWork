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

namespace TPR_ExampleView
{
    internal class AssemblyItem
    {
        static Type TypeDefOutputImage = typeof(BaseLibrary.OutputImage);
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
                ImgClass imgClass = item.GetCustomAttribute<ImgClass>();
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
                                if (t == TypeDefIImage || t.GetInterfaces().Contains(typeof(Emgu.CV.IImage)))
                                {
                                    MyMethodInfo mmi = new MyMethodInfo(item2);
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
                    TableLayoutPanel tlp = new TableLayoutPanel { RowCount=1, ColumnCount = 3, Parent = f, Dock = DockStyle.Fill};
                    ComboBox comboBox = new ComboBox();
                    
                }
                //Если есть неоднозначности в выборе метода
                //Доделать с выбором через форму определенного метода
            }


            if (methodInfo != null)
            {
                OutputImage outputImage = null;
                CustomForm formCustom = methodInfo.CustomForm;
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InvMethod));
                if (formCustom != null)
                {
                    Type formType = formCustom.FormType;
                    BaseForm form = Activator.CreateInstance(formType, SelectedImage, methodInfo.MethodInfo) as BaseForm;
                    thread.Start(new InvParam { TypeInvoke = TypeInvoke.a, BaseForm = form });
                }
                else
                {
                    var formAuto = methodInfo.AutoForms;
                    if (formAuto.Count() > 0)
                        using (FormP form = new FormP(formAuto, methodInfo, SelectedImage))
                        {
                            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                thread.Start(new InvParam { TypeInvoke = TypeInvoke.b, FormP = form });
                            }
                        }
                    else
                        thread.Start(new InvParam { TypeInvoke = TypeInvoke.c, MethodInfo = methodInfo.MethodInfo });
                }
                SelectedForm.UpdateImage();
                BaseLibrary.BaseMethods.LoadOutputImage(outputImage);
            }
        }

        private class InvParam
        {
            public MethodInfo MethodInfo { get; set; }
            public Form Form { get; set; }
            public BaseForm BaseForm { get; set; }
            public FormP FormP { get; set; }
            public TypeInvoke TypeInvoke { get; set; }
        }
        private enum TypeInvoke
        {
            a,b,c
        }
        private static void InvMethod(object param)
        {
            if (param is InvParam invParam)
            {
                OutputImage outputImage = null;
                try
                {
                    switch (invParam.TypeInvoke)
                    {
                        case TypeInvoke.a:
                            if (invParam.BaseForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                outputImage = invParam.BaseForm.OutputImage;
                            }
                            break;
                        case TypeInvoke.b:
                            if (invParam.FormP.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                outputImage = invParam.MethodInfo.Invoke(null, invParam.FormP.vs) as OutputImage;
                            break;
                        case TypeInvoke.c:
                            outputImage = invParam.MethodInfo.Invoke(null, new object[] { SelectedImage }) as OutputImage;
                            break;
                        default:
                            break;
                    }

                    



                }
                catch (TargetInvocationException ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.InnerException.Message);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
                MainForm.Invoke(new MethodInvoker(()=>{
                    BaseLibrary.BaseMethods.LoadOutputImage(outputImage);
                }));
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
        public MethodInfo MethodInfo { get; }
        public string[] Hierarchy { get => ImgMethod.Hierarchy; }
        public CustomForm CustomForm { get; }
        public AutoForm[] AutoForms { get; }
        public ImgMethod ImgMethod { get; }
        public Assembly Assembly { get => MethodInfo.Module.Assembly; }
        public Module Module { get => MethodInfo.Module; }
        public MyMethodInfo(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
            ImgMethod = methodInfo.GetCustomAttribute<ImgMethod>();
            CustomForm = methodInfo.GetCustomAttribute<CustomForm>();
            AutoForms = methodInfo.GetCustomAttributes<AutoForm>().ToArray();
        }
    }

    internal class FormP : System.Windows.Forms.Form
    {
        public object[] vs;
        MyMethodInfo methodInfo;
        IImage image;
        Type[] Types;
        System.Windows.Forms.Button OK = new System.Windows.Forms.Button { Text = "OK", DialogResult = System.Windows.Forms.DialogResult.OK };
        System.Windows.Forms.Button Cancel = new System.Windows.Forms.Button { Text = "Отмена", DialogResult = System.Windows.Forms.DialogResult.Cancel };
        System.Windows.Forms.TextBox[] textBoxes;
        System.Windows.Forms.FlowLayoutPanel flp = new System.Windows.Forms.FlowLayoutPanel
        {
            FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft,
            Dock = System.Windows.Forms.DockStyle.Fill
        };
        System.Windows.Forms.TableLayoutPanel tlp = new System.Windows.Forms.TableLayoutPanel
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
            foreach (Control item in textBoxes)
            {
                jDict[((ParameterInfo)item.Tag).Name] = item.Text;
                //vs.Add(((ParameterInfo)item.Tag).Name, item.Text);
            }
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
                foreach (var item in jDict)
                {
                    TextBox textBox = textBoxes.Where(a => ((ParameterInfo)a.Tag).Name == item.Key).FirstOrDefault();
                    if(textBox!=null)
                    {
                        textBox.Text = item.Value.ToObject<string>();
                    }
                }
            }
        }
        public FormP(AutoForm[] formAuto, MyMethodInfo methodInfo, IImage image) : base()
        {
            this.AcceptButton = this.OK;
            this.CancelButton = this.Cancel;
            Types = new Type[formAuto.Length];
            textBoxes = new System.Windows.Forms.TextBox[formAuto.Length];
            this.image = image;
            this.methodInfo = methodInfo;
            flp.Controls.Add(OK);
            flp.Controls.Add(Cancel);
            tlp.RowCount = (formAuto.Length * 2 + 1);
            int l = tlp.RowCount - 1;
            this.Height = 80;
            for (int i = 0; i < formAuto.Length; i++)
            {
                AutoForm item = formAuto.ElementAt(i);
                Types[i] = item.Type;
                tlp.Controls.Add(new System.Windows.Forms.Label { Text = item.LabelText, Dock = System.Windows.Forms.DockStyle.Bottom }, 0, i * 2);
                tlp.Controls.Add(textBoxes[i] = new System.Windows.Forms.TextBox { Dock = System.Windows.Forms.DockStyle.Fill, Tag = methodInfo.MethodInfo.GetParameters()[item.Index] }, 0, i * 2 + 1);
                tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26));
                int h;
                if (item.IsMultiline) h = item.TextBoxHeigth;
                else h = 26;
                tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, h));
                Height += 26 + h;
            }
            tlp.Controls.Add(flp, 0, l);
            Controls.Add(tlp);
            LoadParams();
            FormClosing += delegate (object sender, System.Windows.Forms.FormClosingEventArgs e)
            {
                if (DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    vs = new object[textBoxes.Length + 1];
                    vs[0] = image;
                    for (int i = 0; i < Types.Length; i++)
                    {
                        try
                        {
                            vs[i + 1] = ((IConvertible)textBoxes[i].Text).ToType(Types[i], System.Globalization.CultureInfo.CurrentCulture);
                        }
                        catch
                        {
                            e.Cancel = true;
                            System.Windows.Forms.MessageBox.Show("Неверный параметр " + i);
                        }
                    }
                }
                SaveParams();
            };
        }
    }

    internal static class DLL_Init
    {
        /// <summary>
        /// Название библиотеки, находящемся в решении, без помещения в папку с библиотеками. Полезно для отладки<para/>Можно не использовать
        /// </summary>
        public static string AssemblyInSolution;
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {
                if (!string.IsNullOrEmpty(AssemblyInSolution)) assemblies.Add(new AssemblyItem(Assembly.Load(AssemblyInSolution)));
            }
            catch
            {
                MessageBox.Show($"Не удалось загрузить {AssemblyInSolution}");
            }
            foreach (var item in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    assemblies.Add(new AssemblyItem(Assembly.LoadFile(Environment.CurrentDirectory +"\\"+ item)));
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
}

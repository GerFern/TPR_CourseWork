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

namespace TPR_ExampleView
{
    public class AssemblyItem
    {
        static Type TypeDefOutputImage = typeof(BaseLibrary.OutputImage);
        static Type TypeDefIImage = typeof(Emgu.CV.IImage);
        public class ClassItem
        {
            public Type TypeClass { get; }
            public List<MethodInfo> Methods { get; }
            public ClassItem(Type TypeClass)
            {
                this.TypeClass = TypeClass;
                Methods = new List<MethodInfo>();
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
                                    classItem.Methods.Add(item2);
                                    if (imgMethod.Hierarchy.Length >= 1)
                                    {
                                        MenuMethod.Add(imgMethod.Hierarchy, item2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public class MenuMethod
    {
        internal static Form1 MainForm { get; set; }
        static bool repeatResist = false;
        public static BaseLibrary.ImageForm SelectedForm { get; set; }
        public static IImage SelectedImage;
        public static Dictionary<string, Emgu.CV.UI.ImageViewer> images = new Dictionary<string, Emgu.CV.UI.ImageViewer>();
        //public static Emgu.CV.UI.ImageBox imageBox;
        public static TextBox textBox;
        //public static IImage image;
        public static Dictionary<string, MenuMethod> Buttons { get; } = new Dictionary<string, MenuMethod>();
        public static MenuStrip Menu { get; set; }
        public ToolStripMenuItem MenuItem { get; }
        public MenuMethod Parent { get; }
        public Dictionary<string, MenuMethod> SubButtons { get; } = new Dictionary<string, MenuMethod>();

        public List<MethodInfo> Methods { get; } = new List<MethodInfo>();

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
        public static void Add(string[] hierarchy, MethodInfo methodInfo)
        {
            MenuMethod current;
            if (Buttons.ContainsKey(hierarchy[0]))
            {
                current = Buttons[hierarchy[0]];//.Methods.Add(methodInfo);
            }
            else
            {
                Buttons.Add(hierarchy[0], current = new MenuMethod(hierarchy[0], null));
            }
            foreach (var item in hierarchy.Skip(1))
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
            MethodInfo methodInfo = null;
            //Определение метода
            if (butTag.Methods.Count == 1)
            {
                methodInfo = butTag.Methods.First();
            }
            if (butTag.Methods.Count > 1)
            {
                //Если есть неоднозначности в выборе метода
                //Доделать с выбором через форму определенного метода
            }


            if (methodInfo != null)
            {
                OutputImage outputImage = null;
                CustomForm formCustom = methodInfo.GetCustomAttribute<CustomForm>();
                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(InvMethod));
                if (formCustom != null)
                {
                    Type formType = formCustom.FormType;
                    BaseForm form = Activator.CreateInstance(formType, SelectedImage, methodInfo) as BaseForm;
                    thread.Start(new InvParam { TypeInvoke = TypeInvoke.a, BaseForm = form });
                }
                else
                {
                    var formAuto = methodInfo.GetCustomAttributes<AutoForm>().ToArray();
                    if (formAuto.Count() > 0)
                        using (FormP form = new FormP(formAuto, methodInfo, SelectedImage))
                        {
                            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                thread.Start(new InvParam { TypeInvoke = TypeInvoke.b, FormP = form });
                            }
                        }
                    else
                        thread.Start(new InvParam { TypeInvoke = TypeInvoke.c, MethodInfo = methodInfo });
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
                            if (invParam.Form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

    public class FormP : System.Windows.Forms.Form
    {
        public object[] vs;
        MethodInfo methodInfo;
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
        //flp.Controls.Add(OK);
        //                flp.Controls.Add(Cancel);
        System.Windows.Forms.TableLayoutPanel tlp = new System.Windows.Forms.TableLayoutPanel
        {
            Dock = System.Windows.Forms.DockStyle.Fill,
            ColumnCount = 1,
            //RowCount = (formAuto.Count() * 2) + 1
        };
        public FormP(AutoForm[] formAuto, MethodInfo methodInfo, IImage image) : base()
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
                tlp.Controls.Add(textBoxes[i] = new System.Windows.Forms.TextBox { Dock = System.Windows.Forms.DockStyle.Fill }, 0, i * 2 + 1);
                tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26));
                int h;
                if (item.IsMultiline) h = item.TextBoxHeigth;
                else h = 26;
                tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, h));
                Height += 26 + h;
            }
            tlp.Controls.Add(flp, 0, l);
            Controls.Add(tlp);
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
            };
        }
    }

    public static class DLL_Init
    {
        /// <summary>
        /// Название библиотеки, находящемся в решении, без помещения в папку с библиотеками. Полезно для отладки<para/>Можно не использовать
        /// </summary>
        public static string AssemblyInSolution ;
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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

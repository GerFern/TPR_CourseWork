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
        public static Emgu.CV.UI.ImageBox imageBox;
        public static TextBox textBox;
        //public static IImage image;
        public static Dictionary<string, MenuMethod> Buttons { get; } = new Dictionary<string, MenuMethod>();
        public static MenuStrip Menu { get; set; }
        public ToolStripMenuItem MenuItem { get; }
        public MenuMethod Parent { get; }
        public Dictionary<string, MenuMethod> SubButtons { get; } = new Dictionary<string, MenuMethod>();
        public List<MethodInfo> Methods { get; } = new List<MethodInfo>();
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

        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (imageBox.Image == null) return;
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem.DropDownItems.Count > 0) return;
            MenuMethod butTag = menuItem.Tag as MenuMethod;
            MethodInfo methodInfo = null;
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
                if (formCustom != null)
                {
                    Type formType = formCustom.FormType;
                    BaseForm form = Activator.CreateInstance(formType, imageBox.Image, methodInfo) as BaseForm;
                    try
                    {
                        if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            outputImage = form.OutputImage;
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
                }
                else
                {
                    var formAuto = methodInfo.GetCustomAttributes<AutoForm>().ToArray();
                    if (formAuto.Count() > 0)
                        using (FormP form = new FormP(formAuto, methodInfo, imageBox.Image))
                        {
                            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                try
                                {
                                    outputImage = methodInfo.Invoke(null, form.vs) as OutputImage;
                                }
                                catch (TargetInvocationException ex)
                                {
                                    System.Windows.Forms.MessageBox.Show(ex.InnerException.Message);
                                }
                                catch (Exception ex)
                                {
                                    System.Windows.Forms.MessageBox.Show(ex.Message);
                                }
                            }
                        }
                    else
                        try
                        {
                            outputImage = methodInfo.Invoke(null, new object[] { imageBox.Image }) as OutputImage;
                        }
                        catch (TargetInvocationException ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.InnerException.Message);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message);
                        }
                }
                if (outputImage != null)
                {
                    if (outputImage.Image != null)
                        imageBox.Image = outputImage.Image;
                    if (outputImage.Info != null)
                        textBox.Text = outputImage.Info;
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

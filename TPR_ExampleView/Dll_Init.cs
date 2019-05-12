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
using System.ComponentModel;

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
                        ImgMethodAttribute imgMethod = item2.GetCustomAttribute<ImgMethodAttribute>();
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
                string dll = string.Empty;
                string path = Directory.GetCurrentDirectory();
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                try
                {
                    FileInfo[] files = directoryInfo.GetFiles(item + ".dll");
                    if (files.Length > 0)
                        dll = files.First().FullName;
                    else
                    {
                        directoryInfo = directoryInfo.Parent.Parent.Parent;
                        var dirs = directoryInfo.GetDirectories(item);
                        directoryInfo = dirs.FirstOrDefault();
                        if (directoryInfo != null)
                        {
                            directoryInfo = directoryInfo.GetDirectories("bin").FirstOrDefault();
                            if (directoryInfo != null)
                            {
                                var dllDir = directoryInfo.GetDirectories("Debug").FirstOrDefault();
                                if (dllDir == null)
                                    dllDir = directoryInfo.GetDirectories("Release").FirstOrDefault();
                                if (dllDir != null)
                                {
                                    files = dllDir.GetFiles(item + ".dll");
                                    if (files.Length > 0)
                                        dll = files.First().FullName;
                                }
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(dll))
                        assemblies.Add(new AssemblyItem(Assembly.LoadFile(dll)));
                    else
                    {
                        assemblies.Add(new AssemblyItem(Assembly.Load(item)));
                    }
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
}

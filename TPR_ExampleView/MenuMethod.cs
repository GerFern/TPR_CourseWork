//#define test
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BaseLibrary;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Threading;

namespace TPR_ExampleView
{
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
            //if (SelectedImage == null) return;

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
                using (Form f = new Form
                {
                    Text = "Выберите метод",
                    Size = new System.Drawing.Size(250, 100),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    StartPosition = FormStartPosition.CenterParent
                })
                {
                    TableLayoutPanel tlp = new TableLayoutPanel { RowCount = 2, ColumnCount = 1, Parent = f, Dock = DockStyle.Fill };
                    FlowLayoutPanel flp = new FlowLayoutPanel { Anchor = AnchorStyles.Left | AnchorStyles.Right };
                    Button bOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Parent = flp };
                    Button bCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Parent = flp };
                    ComboBox comboBox = new ComboBox
                    {
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        DataSource = butTag.Methods,
                        DisplayMember = "FullName"
                    };
                    f.AcceptButton = bOK;
                    f.CancelButton = bCancel;

                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
                    tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

                    tlp.Controls.Add(comboBox, 0, 0);
                    tlp.Controls.Add(flp, 0, 1);
                    if (f.ShowDialog() == DialogResult.OK)
                        methodInfo = (MyMethodInfo)comboBox.SelectedItem;
                }
                //Если есть неоднозначности в выборе метода
                //Доделать с выбором через форму определенного метода
            }

            RunMethodInfo(methodInfo);
        }

        public static void RunMethodInfo(MyMethodInfo methodInfo)
        {
            if (methodInfo != null)
            {
                if (!methodInfo.CanBeDisposedOrNull)
                {
                    //const string err = "В ходе выполнения метода могут произойти ошибки. " +
                    //   "Чтобы не показывать такое сообщение пометьте метод атрибутом [ImgCanBeDisposedOrNull]. " +
                    //   "Проверяйте IImage.IsDisposedOrNull(). Продолжить?";
                    if (SelectedImage == null)
                    {
                        MessageBox.Show("Входное изображение не выбрано", "");
                        return;
                    }
                    else if (SelectedImage.IsDisposedOrNull())
                    {
                        MessageBox.Show("Входное изображение было удалено. Это могло произойти из-за того, " +
                            "что несколько форм ссылалось на это изображение и одна из них была закрыта или по другим причинам, " +
                            "что привело к вызову метода IImage.Dispose()", "");
                        return;
                    }
                }
                InvokeMethodInfo(methodInfo);
                //BaseLibrary.BaseMethods.LoadOutputImage(outputImage);
            }
        }

        private static void InvokeMethodInfo(MyMethodInfo methodInfo)
        {
            //OutputImage outputImage = null;
            CustomFormAttribute formCustom = methodInfo.CustomForm;
            InvParam invParam = null;
            
            if (formCustom != null)
            {
                Type formType = formCustom.FormType;
                BaseForm form;
                if (methodInfo.IsInputImage) form = Activator.CreateInstance(formType, new InputImage(SelectedImage, -1, methodInfo.MethodName), methodInfo.MethodInfo) as BaseForm;
                else form = Activator.CreateInstance(formType, SelectedImage, methodInfo.MethodInfo) as BaseForm;
                using (form)
                {
                    form.LoadSettings(methodInfo.MethodName);
                    if (form.ShowDialog() == DialogResult.OK)
                        invParam = new InvParam { TypeInvoke = TypeInvoke.CustomForm, BaseForm = form, MethodInfo = methodInfo };
                        //thread.Start(new InvParam { TypeInvoke = TypeInvoke.CustomForm, BaseForm = form });
                    form.SaveSetting(methodInfo.MethodName);
                }
            }
            else
            {
                if (methodInfo.IsAutoForm)
                    using (FormP form = new FormP(methodInfo, SelectedImage))
                    {
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            invParam = new InvParam { TypeInvoke = TypeInvoke.ParameterizedForm, Vs = form.vs, MethodInfo = methodInfo };
                            //thread.Start(new InvParam { TypeInvoke = TypeInvoke.ParameterizedForm, Vs = form.vs, MethodInfo = methodInfo });
                        }
                    }
                else
                    invParam = new InvParam { TypeInvoke = TypeInvoke.WithoutForm, MethodInfo = methodInfo, Vs = new object[1] };
                    //thread.Start(new InvParam { TypeInvoke = TypeInvoke.WithoutForm, MethodInfo = methodInfo });
            }
            if (invParam != null)
            {
                if (Properties.Settings.Default.MultiImage)
                {
                    var ImageSetting = invParam.ImageSetting;
                    if (Properties.Settings.Default.SaveNewImageToFile)
                    {
                        ImageSetting.SaveToFile = true;
                        ImageSetting.RootDirectory = Properties.Settings.Default.SaveImageDir;
                        ImageSetting.MaskFile = Properties.Settings.Default.SaveFileNameMask;
                        ImageSetting.ReplaceFile = Properties.Settings.Default.SaveFileReplace;
                        ImageSetting.ExtFile = Extensions.ExtToNameSupport.Keys.ToList()[Properties.Settings.Default.SaveFileFormatIndex];
                        ImageSetting.ProcessMask(methodInfo.MethodInfo.Name, methodInfo.Hierarchy.LastOrDefault(), methodInfo.MethodName, DateTime.Now.ToString());
                    }
                    else ImageSetting.SaveToFile = false;
                    new Thread(() =>
                    {
                        var f = new Forms.FormInvokeProgress(MainForm.InvokeMethodImmediately, invParam, MainForm.imageList1.CheckedImgNames.ToArray());
                        f.ShowDialog();
                    }) { Name = "ProgressForm", Priority = ThreadPriority.Highest }.Start();
                }
                else
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(InvMethod)) { Name = methodInfo.MethodName };
                    ProgressInfoControl pic;
                    invParam.Image = SelectedImage;
                    /*invParam.TaskID =*/ MainForm.CreateTask(methodInfo.MethodName, methodInfo, thread, invParam, out pic);
                    MainForm.progressListControl.InvokeFix(() => MainForm.progressListControl.Add(pic));
                    //MainForm.tableLayoutPanel1.Invoke(new Action(() =>
                    //{
                    //    MainForm.tableLayoutPanel1.Controls.Add(
                    //    new ProgressInfoControl(ip.ProgressInfo, thread));
                    //}));
                    if (MainForm.InvokeMethodImmediately)
                        pic.ThreadStart();
                }
            }
            //SelectedForm?.UpdateImage();
        }

        /// <summary>
        /// Общие настройки
        /// </summary>
        internal class OverallLoadImageSetting
        {
            public bool SaveToFile { get; set; }
            public string RootDirectory { get; set; }
            public string MaskFile { get; set; }
            public string ProcessedMask { get; set; }
            public bool ReplaceFile { get; set; }
            public string ExtFile { get; set; }

            public void ProcessMask(string mName, string bName, string aName, string dateTimeStart)
            {
                string res = string.Empty;
                CharEnumerator vs = MaskFile.GetEnumerator();
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
                                    res += dateTimeStart;
                                    break;
                                default:
                                    res += c.ToString() + vs.Current;
                                    break;
                            }
                        }
                    }
                    else res += c;
                }
                ProcessedMask = res;
            }

        }


        /// <summary>
        /// Настройки выполнения методов
        /// </summary>
        internal class InvParam : ICloneable
        {
            public ProgressInfoControl ProgressInfoControl { get; set; }
            public OverallLoadImageSetting ImageSetting { get; set; } = new OverallLoadImageSetting();
            public bool NeedDisposeImage { get; set; }
            public string ImgFileString { get; set; }
            public IImage Image { get; set; }
            public MyMethodInfo MethodInfo { get; set; }
            public Form Form { get; set; }
            public BaseForm BaseForm { get; set; }
            public FormP FormP { get; set; }
            public object[] Vs { get; set; }
            public TypeInvoke TypeInvoke { get; set; }
            public int TaskID { get; set; }
            /// <summary>
            /// Не копируется
            /// </summary>
            public int LocalID { get; set; }
            public DateTime DateTimeStart { get; set; }
            public DateTime DateTimeEnd { get; set; }
            public string ImageName { get; set; }
            //public InputImage InputImage => new InputImage(Image, TaskID, MethodInfo.MethodName);

            public string GetFilePath(out int uSeparatorIndex)
            {
                string mask = ImageSetting.ProcessedMask;

                //%f - Имя изображения
                //%m - Название метода (оригинальное название)
                //%n - Название метода (кнопка меню)
                //%N - Название метода (через атрибут)
                //%t - Дата начала выполнения всех задач
                //%d - Дата начала выполнения задачи
                //%D - Дата конца выполнения задачи
                //%i - Номер задачи
                //%u - Инкрементное значение, для избежания совпадений имен. Используется, если выключен параметр перезаписывать существующие файлы
                string startTime = DateTimeStart.ToString();
                string endTime = DateTimeEnd.ToString();
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
                                    if (ImageName != null)
                                    {
                                        res += ImageName;
                                        index += ImageName.Length;
                                    }
                                    break;
                                case 'd':
                                    res += startTime;
                                    index += startTime.Length;
                                    break;
                                case 'D':
                                    res += endTime;
                                    index += endTime.Length;
                                    break;
                                case 'i':
                                    res += LocalID.ToString();
                                    index += LocalID.ToString().Length;
                                    break;
                                case 'u':
                                    if (!uSeparator)
                                    {
                                        uSeparatorIndex = index;
                                        uSeparator = true;
                                    }
                                    break;
                                default:
                                    res += c + vs.Current;
                                    index += 2;
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
                if(uSeparatorIndex >= 0)
                {
                    uSeparatorIndex += ImageSetting.RootDirectory.Length + 1;
                }
                return ImageSetting.RootDirectory + '\\' + res + ImageSetting.ExtFile;
            }

            

            public object Clone()
            {
                InvParam invParam = new InvParam();
                invParam.BaseForm = BaseForm;
                invParam.Form = Form;
                invParam.FormP = FormP;
                invParam.ImageSetting = ImageSetting;
                invParam.ImgFileString = ImgFileString;
                invParam.MethodInfo = MethodInfo;
                invParam.TaskID = TaskID;
                invParam.TypeInvoke = TypeInvoke;
                if (Vs != null)
                {
                    invParam.Vs = new object[Vs.Length];
                    Vs.CopyTo(invParam.Vs, 0);
                }
                return invParam;
            }

            //public InvParam Clone() => new InvParam
            //{
            //    ProgressInfoControl = progressInfoControl,
            //    ImgFileString = imgFileString,
            //    Image = image,
            //    MethodInfo = methodInfo,
            //    Form = form,
            //    BaseForm = baseForm,
            //    FormP = formP,
            //    Vs = vs,
            //    TypeInvoke = typeInvoke,
            //    TaskID = taskID
            //};
        }
        internal enum TypeInvoke
        {
            CustomForm, ParameterizedForm, WithoutForm
        }

        /// <summary>
        /// Выполнения метода
        /// </summary>
        /// <param name="param"></param>
        internal static void InvMethod(object param)
        {
            if (param is InvParam invParam)
            {
                OutputImage outputImage = null;
#if !test
                try
                {
#endif
                    if (invParam.Image.IsDisposedOrNull())
                    {
                        try
                        {
                            if (invParam.ImgFileString.PathIsImage())
                            {
                                invParam.Image = new Image<Bgr, byte>(invParam.ImgFileString);
                                invParam.NeedDisposeImage = true;
                            }
                        }
                        catch { }
                    }
                    if (invParam.Vs == null) invParam.Vs = new object[1];
                    if (invParam.MethodInfo.IsInputImage)
                    {
                        invParam.Vs[0] = new InputImage(invParam.Image, invParam.TaskID, invParam.MethodInfo.MethodName);
                    }
                    else if (invParam.Vs[0] == null) invParam.Vs[0] = invParam.Image;
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
                                if (form.Vs[0] is InputImage inpImgForm)
                                    inpImgForm.ID = invParam.TaskID;
                            outputImage = form.MethodInfo.Invoke(null, form.Vs) as OutputImage;
                            break;
                        case TypeInvoke.ParameterizedForm:
                            if (invParam.Vs[0] is InputImage inpImgPForm)
                                inpImgPForm.ID = invParam.TaskID;
                            outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, invParam.Vs) as OutputImage;
                            break;
                        case TypeInvoke.WithoutForm:
                            outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, invParam.Vs) as OutputImage;

                            //if (invParam.MethodInfo.IsInputImage)
                            //    outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, new object[] { invParam.InputImage }) as OutputImage;

                            ////    MainForm.Invoke(new Action(()=>
                            ////   { outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, new object[] { new InputImage(SelectedImage, MainForm.CreateTask(invParam.MethodInfo)) }) as OutputImage; }
                            ////));
                            //else outputImage = invParam.MethodInfo.MethodInfo.Invoke(null, new object[] { invParam.V }) as OutputImage;
                            break;
                        default:
                            break;
                    }
                    //SelectedForm?.InvokeFix(() => SelectedForm.Update());
#if !test
                }
                catch (TargetInvocationException ex)
                {
                    invParam.ProgressInfoControl.SetException(ex.InnerException);
                    MainForm.SetExceptionError(ex.InnerException);
                    //System.Windows.Forms.MessageBox.Show(ex.InnerException.Message);
                }
                catch (System.Threading.ThreadAbortException ex)
                {
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (invParam.NeedDisposeImage) invParam.Image?.Dispose();
                }
#endif
                if (outputImage != null)
                {
                    MainForm.LoadOutputImage(outputImage, invParam);
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
}

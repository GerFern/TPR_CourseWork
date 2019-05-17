using Emgu.CV;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaseLibrary
{
    public partial class ImageForm : Form
    {
        /// <summary>
        /// Путь к файлу, если имеется
        /// </summary>
        public string FilePath { get; protected set; }
        /// <summary>
        /// Цвет, который показывается при перетаскивании формы на панель вкладок
        /// </summary>
        [System.ComponentModel.DefaultValue(typeof(Color), "DeepSkyBlue")]
        public virtual Color OverColor
        {
            get; set;
        } = Color.DeepSkyBlue;
        public BackgroundWorkerImg Worker { get; set; }
        IImage _image;
        private bool _cast = false;
        private ImageForm _castForm;
        /// <summary>
        /// Нужно ли регистрировать данную форму во вкладке с изображениями. 
        /// Если <see langword="true"/>, то будет вызван <see cref="BaseMethods.NewImageForm"/>.
        /// Значение устанавливается в конструкторе
        /// </summary>
        public bool NeedNewFormRegister { get; }
        /// <summary>
        /// Основное изображение
        /// </summary>
        public IImage Image
        {
            get => _image;
            set
            {
                SetImage(_image = value);
                ImageChanged?.Invoke(this, new EventArgsImage(Image));
            }
        }
        public static ImageForm selected;
        /// <summary>
        /// Является ли изображение выделенным
        /// </summary>
        public bool IsSelected
        {
            get => Object.ReferenceEquals(this, selected);
            set
            {
                if (value != IsSelected)
                {
                    if (Image == null) throw new NullReferenceException();
                    //_isSelected = value;
                    if (value)
                    {
                        _isSelectedChanged?.Invoke(this, new EventArgsWithImageForm(this, Image));
                        IsSelectedChanged?.Invoke(this, new EventArgsWithImageForm(this, Image));
                    }
                    else
                    {
                        _isSelectedChanged?.Invoke(this, new EventArgsWithImageForm());
                        IsSelectedChanged?.Invoke(this, new EventArgsWithImageForm());
                    }
                }
            }
        }
        /// <summary>
        /// Если основное изображение изменено
        /// </summary>
        public event EventHandler<EventArgsImage> ImageChanged;
        /// <summary>
        /// Если основное изображение было выделено или наоборот
        /// </summary>
        public event EventHandler<EventArgsWithImageForm> IsSelectedChanged;
        /// <summary>
        /// Если название формы изменено
        /// </summary>
        public event EventHandler NameFormChanged;

        //Показать форму
        public new void Show()
        {
            if (this.Parent is TabForm tabForm)
            {
                if (tabForm.WindowState == FormWindowState.Minimized) tabForm.WindowState = FormWindowState.Normal;
                tabForm.Select();
            }
            else if (this.Parent is TabPage tabPage)
            {
                if (tabPage.Parent is TabControl tabControl)
                    tabControl.SelectedTab = tabPage;
            }
            else ShowForm();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_cast)
            {
                Control parent = this.Parent;
                parent.Controls.Clear();
                parent.Controls.Add(_castForm);
                return;
            }
            else if (_replaceForm!=null)
            {
                Control parent = this.Parent;
                parent.Controls.Clear();
                parent.Controls.Add(_replaceForm);
                return;
            }
            else if (this.Parent is TabForm tabForm)
            {
                tabForm.RestoreTabBeforeClosing = false;
                tabForm.Close();
            }
            else if (this.Parent is TabPage tabPage)
            {
                tabPage.Dispose();
            }
            if (this.IsSelected)
            {
                _isSelectedChanged?.Invoke(this, new EventArgsWithImageForm());
            }
            if (Image != null && Image.Ptr != IntPtr.Zero) Image.Dispose();
            base.OnClosed(e);
        }

        static EventHandler<EventArgsWithImageForm> _isSelectedChanged;
        private ImageForm _replaceForm;

        /// <summary>
        /// Использовать не нужно
        /// </summary>
        /// <param name="eventHandler"></param>
        public static void SetIsSelectedChangedMethod(EventHandler<EventArgsWithImageForm> eventHandler)
        {
            if (_isSelectedChanged == null)
                _isSelectedChanged = eventHandler;
        }

        /// <summary>
        /// Превращает текущую форму в простую форму для отображения изображения. Вызывает метод Close(), так что текущая форма перестанет существовать
        /// </summary>
        /// <param name="outputImage"></param>
        public void CastToOutputImage(OutputImage outputImage)
        {
            if (outputImage != null)
            {
                _castForm = BaseMethods.CreateFormFromOutputImage(outputImage);
                if(_castForm!=null)
                {
                    _cast = true;
                    Close();
                }
            }
        }

        /// <summary>
        /// Заменяет эту форму другой
        /// </summary>
        /// <param name="imageForm"></param>
        public void CastToOtherForm(ImageForm imageForm)
        {
            if (imageForm!=null)
            {
                _replaceForm = imageForm;
                Close();
            }
        }


        /// <summary>
        /// Загружает <see cref="OutputImage"/> объект на главную форму
        /// </summary>
        /// <param name="outputImage"></param>
        public static void LoadOutputImage(OutputImage outputImage) => BaseMethods.LoadOutputImage(outputImage);
        /// <summary>
        /// Пометить изображение как выделенное
        /// </summary>
        public void MakeSelected()
        {
            if (!IsSelected) IsSelected = true;
        }

        protected virtual void SetImage(IImage image) { }

        /// <summary>
        /// Обновить изображение
        /// </summary>
        public virtual void UpdateImage(IImage image = null) { }
        /// <summary>
        /// Можно переопределить для возврата <see langword="true"/> или с каким-то условием. Тогда при открытии формы изображение будет автоматически становится выделенным
        /// </summary>
        public virtual bool AutoSelect => false;

        /// <summary>
        /// В конструкторе следует определить события
        /// </summary>
        public ImageForm()
        {
            InitializeComponent();
            NeedNewFormRegister = true;
            Worker = new BackgroundWorkerImg(this);
        }

        public ImageForm(bool NeedNewFormRegister)
        {
            InitializeComponent();
            this.NeedNewFormRegister = NeedNewFormRegister;
            Worker = new BackgroundWorkerImg(this);
        }

        /// <summary>
        /// Вызывает imageForm.Worker.RunWorkerAsync(workerArgument), позволяя выполнять вычислительные операции без зависания главной формы. После завершения, показывает форму
        /// Для этого должны быть определены события <code>Worker.DoWork</code>
        /// </summary>
        /// <param name="workerArgument">Аргументы для Worker</param>
        /// <param name="dockStyle">Заполнение формы</param>
        /// <param name="formBorderStyle">Границы формы</param>
        public void ShowFormAsync(object argument,
                                  FormBorderStyle formBorderStyle = FormBorderStyle.None,
                                  DockStyle dockStyle = DockStyle.Fill)
        {
            BaseMethods.ShowFormAsync(this, argument, formBorderStyle, dockStyle);
        }

        /// <summary>
        /// Показывает форму в главной форме
        /// </summary>
        /// <param name="dockStyle">Заполнение формы</param>
        /// <param name="formBorderStyle">Границы формы</param>
        public void ShowForm(FormBorderStyle formBorderStyle = FormBorderStyle.None,
                             DockStyle dockStyle = DockStyle.Fill)
        {
            BaseMethods.ShowForm(this, formBorderStyle, dockStyle);
        }
    }

    public class EventArgsImage : EventArgs
    {
        /// <summary>
        /// Изображение
        /// </summary>
        public IImage Image { get; }
        public EventArgsImage(IImage image) => Image = image;
    }

    /// <summary>
    /// Выбрано новое изображение
    /// </summary>
    public class EventArgsWithImageForm : EventArgs
    {
        /// <summary>
        /// Форма, к которому принадлежит выделенное изображение (при <see cref="Selected"/> == <see langword="null"/> значение будет <see langword="null"/>)
        /// </summary>
        public ImageForm Form { get; }
        /// <summary>
        /// Изображение (при <see cref="Selected"/> == <see langword="null"/> значение будет <see langword="null"/>)
        /// </summary>
        public IImage Image { get; }
        /// <summary>
        /// Выделено ли
        /// </summary>
        public bool Selected { get; }
        public EventArgsWithImageForm()
        {
            Selected = false;
        }
        public EventArgsWithImageForm(ImageForm imageForm, IImage image)
        {
            Selected = true;
            Form = imageForm;
            Image = image;
        }
    }


    public class BackgroundWorkerImg : BackgroundWorker
    {
        public ImageForm ImageForm { get; set; }
        public TabPage TabPage { get; set; }

        public BackgroundWorkerImg() : base() { }
        public BackgroundWorkerImg(ImageForm imageForm) : base()
        {
            ImageForm = imageForm;
        }
    }
}

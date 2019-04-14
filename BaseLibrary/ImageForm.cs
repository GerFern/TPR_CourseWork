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
        /// Основное изображение
        /// </summary>
        public IImage Image
        {
            get => _image;
            set
            {
                SetImage(_image = value);
                ImageChanged?.Invoke(this, new EventArgs());
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
        public event EventHandler ImageChanged;
        /// <summary>
        /// Если основное изображение было выделено или наоборот
        /// </summary>
        public event EventHandler<EventArgsWithImageForm> IsSelectedChanged;
        /// <summary>
        /// Если название формы изменено
        /// </summary>
        public event EventHandler NameFormChanged;

        static EventHandler<EventArgsWithImageForm> _isSelectedChanged;
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
        public virtual void UpdateImage() { }
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
        private void ImageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if( _cast)
            {
                Control parent = this.Parent;
                parent.Controls.Clear();
                parent.Controls.Add(_castForm);
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
        }
    }

    public class EventArgsWithImageForm : EventArgs
    {
        public ImageForm Form { get; }
        public IImage Image { get; }
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

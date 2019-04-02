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
        IImage _image;
        bool _isSelected;
        string _nameForm;

        public IImage Image
        {
            get => _image;
            set
            {
                _image = value;
                ImageChanged?.Invoke(this, new EventArgs());
            }
        }
        public virtual bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (Image == null) throw new NullReferenceException();
                _isSelected = value;
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
        public virtual string NameForm
        {
            get => _nameForm;
            set
            {
                _nameForm = value;
                if (this.Parent is TabForm tabForm)
                {
                    tabForm.Text = value;
                }
                else if (this.Parent is TabPage tabPage)
                {
                    tabPage.Text = value;
                }
                NameFormChanged?.Invoke(this, new EventArgs());
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
       
        public virtual void UpdateImage() { }
        /// <summary>
        /// Можно переопределить для возврата true. Тогда при открытии формы изображение будет автоматически становится выделенным
        /// </summary>
        public virtual bool AutoSelect => false;

        /// <summary>
        /// В конструкторе следует определить события
        /// </summary>
        public ImageForm()
        {
            InitializeComponent();
            this.TopLevel = false;
        }

        private void ImageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (this.Parent is TabForm tabForm)
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
            this.Close();
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
}

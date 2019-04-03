﻿using Emgu.CV;
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
        public BackgroundWorkerImg Worker { get; set; }
        IImage _image;
        string _nameForm;

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
        public string NameForm
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

        public void MakeSelected()
        {
            if (!IsSelected) IsSelected = true;
        }
        public virtual void SetImage(IImage image) { }

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
            Worker = new BackgroundWorkerImg(this);
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

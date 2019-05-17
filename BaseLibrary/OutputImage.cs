using Emgu.CV;

namespace BaseLibrary
{
    /// <summary>
    /// Обработанное изображение и информация
    /// </summary>
    public class OutputImage
    {
        /// <summary>
        /// Выходная информация. Если <see langword="null"></see>, то информация не требуется
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// Обработанное изображение. Если <see langword="null"></see>, то изменения в изображении не требуются
        /// <para>Рекомендуемый тип <see cref="Emgu.CV.Image{TColor, TDepth}"/>, где TColor - <seealso cref="Emgu.CV.Structure.Bgr"/> или <seealso cref="Emgu.CV.Structure.Gray"/>, а TDepth - <seealso cref="byte"/></para>
        /// </summary>
        public Emgu.CV.IImage Image { get; set; }
        /// <summary>
        /// Установить новое глобальное изображение, которое будет отображаться в отдельной форме. Подойдет в качестве предпросмотра
        /// </summary>
        public Emgu.CV.IImage GlobalImage { get; set; }
        /// <summary>
        /// Установить новую глобальную форму. Подойдет в качестве предпросмотра
        /// </summary>
        public ImageForm GlobalForm { get; set; }
        /// <summary>
        /// Название нового изображения
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Добавить форму, производную от <see cref="BaseLibrary.ImageForm"/>
        /// </summary>
        public ImageForm ImageForm { get; set; }
        /// <summary>
        /// Предназначено для обновления выбранного изображения
        /// </summary>
        public IImage UpdateSelectedImage { get; set; }
    }
}

using Emgu.CV;

namespace TPR_ExampleView
{
        public struct ImgName
        {
            public ImgName(string imgPath, IImage image, string name)
            {
                ImgPath = imgPath;
                Image = image;
                Name = name;
            }

            public string ImgPath { get; set; }
            public IImage Image { get; set; }
            public string Name { get; set; }
        }
}

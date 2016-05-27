using System;
using System.IO;
using System.Reflection;

namespace Base64Sample.ViewModels
{
    public class ImageViewModel : BaseViewModel
    {
        public ImageViewModel()
        {
            LoadImage();
        }

        public static string ImageProperty = "Image";
        private string _image;
        public string Image
        { 
            get  
            {
                return _image; 
            }
            set 
            {
                _image = value; 
                RaisePropertyChanged(() => Image); 
            }
        }  

        private async void LoadImage()
        {
            using (Stream resource = typeof(ImageViewModel).GetTypeInfo().Assembly.GetManifestResourceStream("Base64Sample.Resources.image.txt"))
            {
                if (resource == null)
                {
                    throw new ArgumentException("No such resource", "resourceName");
                }
                using (var reader = new StreamReader(resource))
                {
                    Image = await reader.ReadToEndAsync();
                }
            }
        }
    }
}

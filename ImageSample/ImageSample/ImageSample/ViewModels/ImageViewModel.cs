using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarouselSample.ViewModels;
using ImageSample.Model;
using ImageSample.Services;
using Xamarin.Forms;

namespace ImageSample.ViewModels
{
    public class ImageViewModel : BaseViewModel
    {
        #region Private fields

        private readonly IImageService _imageService;

        #endregion
        #region Constructors

        public ImageViewModel()
        {
            _imageService = DependencyService.Get<IImageService>();
        }

        #endregion

        #region Properties

        public static string ImageProperty = "Image";
        private IImage _newImage;
        public IImage NewImage
        { 
            get  
            {
                return _newImage; 
            }
            set 
            {
                _newImage = value;
                _image = null;
                OnPropertyChanged(() => NewImage);
                OnPropertyChanged(() => Image);
            }
        }

        ImageSource _image;

        public ImageSource Image
        {
            get
            {
                if (_image != null)
                    return _image;
                if (NewImage != null && NewImage.RawImage != null)
                {
                    Stream stream = new MemoryStream(NewImage.RawImage);
                    _image = ImageSource.FromStream(() => stream);
                }
                return _image;
            }
        }

        #endregion

        #region Commands

        private Command _pickCameraImageCommand;

        public Command PickCameraImageCommand
            => _pickCameraImageCommand ?? (_pickCameraImageCommand = _pickCameraImageCommand ?? new Command(PickCameraImage));

        private Command _pickLibraryImageCommand;
        public Command PickLibraryImageCommand => _pickLibraryImageCommand ?? (_pickLibraryImageCommand = _pickLibraryImageCommand ?? new Command(PickLibraryImage));

        #endregion

        #region Utility methods

        private async void PickCameraImage()
        {
            try
            {
                var image = await _imageService.GetImageFromCamera();
                NewImage = image;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private async void PickLibraryImage()
        {
            try
            {
                var image = await _imageService.GetImageFromLibrary();
                NewImage = image;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        #endregion

    }
}

using System.IO;
using System.Threading.Tasks;
using ImageSample.iOS.Services;
using ImageSample.iOS.Toolbox;
using ImageSample.Model;
using ImageSample.Services;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using ObjCRuntime;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;

[assembly: Xamarin.Forms.Dependency(typeof(ImageService))]
namespace ImageSample.iOS.Services
{
    public class ImageService : IImageService
    {
        #region Constructors

        public ImageService()
        {
        }

        #endregion

        #region IImageService implementation

        public async Task<IImage> GetImageFromLibrary()
        {
            var _mediaPicker = DependencyService.Get<IMediaPicker>();
            var imageFile = await _mediaPicker.SelectPhotoAsync(new CameraMediaStorageOptions());
            var result = await getImageFromMediaFile(imageFile);
            return result;
        }

        public async Task<IImage> GetImageFromCamera()
        {
            var _mediaPicker = DependencyService.Get<IMediaPicker>();
            var imageFile = await _mediaPicker.TakePhotoAsync(new CameraMediaStorageOptions
            {
                DefaultCamera = CameraDevice.Rear
            });
            var result = await getImageFromMediaFile(imageFile);
            return result;
        }

        #endregion

        #region Utility methods

        public Task<IImage> getImageFromMediaFile(MediaFile file)
        {
            return Task<IImage>.Factory.StartNew(() =>
            {
                var rawImage = getScaledAndRotatedImage(file, Constants.MaxPixelDimensionOfImages);

                //Initialize picked image
                var image = new PickedImage
                {
                    Name = Path.GetFileName(file.Path),
                    RawImage = rawImage
                };

                return image;
            });
        }

        public byte[] getScaledAndRotatedImage(MediaFile file, float maxPixelDimension)
        {
            var rawImage = new byte[file.Source.Length];
            file.Source.Read(rawImage, 0, rawImage.Length);

            using (var original = rawImage.GetUIImage())
            using (var result = original.GetScaledAndRotatedImage(maxPixelDimension))
            {
                var fileName = file.Path.ToLower();
                if (fileName.EndsWith("png"))
                {
                    return result.GetRawBytes(ImageType.Png);
                }
                return result.GetRawBytes(ImageType.Jpeg);
            }
        }

        #endregion
    }
}
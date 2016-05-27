using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using ImageSample.Droid.Services;
using ImageSample.Model;
using ImageSample.Services;
using MvvmCross.Plugins.Messenger;
using ImageSample.Droid.Toolbox;
using ImageSample.Droid.Views;
using MvvmCross.Platform;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;
using Path = System.IO.Path;

[assembly: Xamarin.Forms.Dependency(typeof(ImageService))]
namespace ImageSample.Droid.Services
{
    public class ImageService : IImageService
    {
        #region Private fields and properties

        private MvxSubscriptionToken _tokenCamera;
        private MvxSubscriptionToken _tokenLibrary;

        private TaskCompletionSource<IImage> _selectImageCompletion;
        private readonly Guid _receiverID = Guid.NewGuid();

        #endregion

        #region Constructors

        public ImageService()
        {
            var messenger = DependencyService.Get<IMvxMessenger>();
            _tokenCamera = messenger.Subscribe<CameraImageSelectedMessage>(cameraImageSelected);
            _tokenLibrary = messenger.Subscribe<LibraryImageSelectedMessage>(libraryImageSelected);
        }

        #endregion

        #region IImageService

        public Task<IImage> GetImageFromLibrary()
        {
            _selectImageCompletion = new TaskCompletionSource<IImage>();
            var activity = Xamarin.Forms.Forms.Context as MainActivity;
            var intent = PhotoGaleryActivity.GetIntentForReceiver(activity, _receiverID);
            activity.StartActivity(intent);
            return _selectImageCompletion.Task;
        }

        public Task<IImage> GetImageFromCamera()
        {
            _selectImageCompletion = new TaskCompletionSource<IImage>();
            var activity = Xamarin.Forms.Forms.Context as MainActivity;
            var intent = CameraActivity.GetIntentForReceiver(activity, _receiverID);
            activity.StartActivity(intent);
            return _selectImageCompletion.Task;
        }

        #endregion

        #region Messages

        private async void cameraImageSelected(CameraImageSelectedMessage message)
        {
            if (message.ReceiverID == _receiverID)
            {
                var result = await Task<IImage>.Factory.StartNew(() =>
                {
                    IImage image = null;
                    if (message.Image != null)
                    {
                        image = new PickedImage
                        {
                            Name = Guid.NewGuid() + ".jpg",
                            RawImage = message.Image.GetRawBytes(Bitmap.CompressFormat.Jpeg)
                        };
                    }
                    return image;
                });

                _selectImageCompletion.TrySetResult(result);
            }
        }

        private async void libraryImageSelected(LibraryImageSelectedMessage message)
        {
            if (message.ReceiverID == _receiverID)
            {
                var result = await Task<IImage>.Factory.StartNew(() =>
                {
                    IImage image = null;
                    try
                    {
                        if (message.Path != null)
                        {
                            var path = message.Path;
                            var file = new MediaFile(path, () => File.OpenRead(path));
                            var rawImage = getScaledAndRotatedImage(file, Constants.MaxPixelDimensionOfImages);

                            //Initialize picked image
                            image = new PickedImage
                            {
                                Name = Path.GetFileName(path),
                                RawImage = rawImage
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    return image;
                });

                _selectImageCompletion.TrySetResult(result);
            }
        }

        #endregion

        #region Utility methods

        private byte[] getScaledAndRotatedImage(MediaFile file, float maxPixelDimension)
        {
            var rawImage = new byte[file.Source.Length];
            file.Source.Read(rawImage, 0, rawImage.Length);

            using (var original = rawImage.GetBitmap())
            using (var result = original.GetScaledAndRotatedBitmap(file.Exif.Orientation, maxPixelDimension))
            {
                var format = Bitmap.CompressFormat.Jpeg;
                var fileName = file.Path.ToLower();
                if (fileName.EndsWith("png"))
                {
                    format = Bitmap.CompressFormat.Png;
                }
                return result.GetRawBytes(format);
            }
        }

        #endregion
    }
}
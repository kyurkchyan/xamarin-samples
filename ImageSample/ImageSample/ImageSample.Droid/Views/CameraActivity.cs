using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Hardware;
using Android.OS;
using Android.Views;
using ImageSample.Droid.Services;
using ImageSample.Droid.Toolbox;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using Xamarin.Forms;
using Button = Android.Widget.Button;
using Camera = Android.Hardware.Camera;

namespace ImageSample.Droid.Views
{
    [Activity(Label = "CameraActivity",
        Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class CameraActivity : Activity, TextureView.ISurfaceTextureListener
    {
        #region Private fields and properties

        private Camera camera;
        private Button takePhotoButton;
        private Button toggleFlashButton;
        private Button switchCameraButton;

        private CameraFacing cameraType;
        private TextureView textureView;
        private SurfaceTexture surfaceTexture;

        private bool _flashOn;

        private static readonly string KEY_RECEIVER_ID = "RECEIVER_ID";
        private Guid _receiverID;
        private Bitmap _selectedImage;

        #endregion

        #region Constructors

        #endregion

        #region View lifecycle management

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (Intent != null)
            {
                var receiverID = Intent.GetStringExtra(KEY_RECEIVER_ID);
                if (receiverID != null)
                    _receiverID = new Guid(receiverID);
            }

            SetContentView(Resource.Layout.activity_camera);

            try
            {
                cameraType = CameraFacing.Back;

                textureView = FindViewById<TextureView>(Resource.Id.textureView);
                textureView.SurfaceTextureListener = this;

                takePhotoButton = FindViewById<Button>(Resource.Id.takePhotoButton);
                takePhotoButton.Click += TakePhotoButtonTapped;

                switchCameraButton = FindViewById<Button>(Resource.Id.switchCameraButton);
                switchCameraButton.Click += SwitchCameraButtonTapped;

                toggleFlashButton = FindViewById<Button>(Resource.Id.toggleFlashButton);
                toggleFlashButton.Click += ToggleFlashButtonTapped;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            publishSelectedImage();
        }

        #endregion

        #region ISurface

        public void OnSurfaceTextureAvailable(SurfaceTexture surface, int width, int height)
        {
            camera = Camera.Open((int) cameraType);
            //textureView.LayoutParameters = new FrameLayout.LayoutParams(width, height);
            surfaceTexture = surface;

            camera.SetPreviewTexture(surface);
            PrepareAndStartCamera();
        }

        public bool OnSurfaceTextureDestroyed(SurfaceTexture surface)
        {
            camera.StopPreview();
            camera.Release();

            return true;
        }

        public void OnSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height)
        {
            PrepareAndStartCamera();
        }

        public void OnSurfaceTextureUpdated(SurfaceTexture surface)
        {
        }

        #endregion

        #region Public API

        public static Intent GetIntentForReceiver(Context context, Guid receiver)
        {
            var intent = new Intent(context, typeof (CameraActivity));
            intent.PutExtra(KEY_RECEIVER_ID, receiver.ToString());
            return intent;
        }

        #endregion

        #region Utility methods

        private void PrepareAndStartCamera()
        {
            camera.StopPreview();

            var display = WindowManager.DefaultDisplay;
            if (display.Rotation == SurfaceOrientation.Rotation0)
            {
                camera.SetDisplayOrientation(90);
            }

            if (display.Rotation == SurfaceOrientation.Rotation270)
            {
                camera.SetDisplayOrientation(180);
            }

            camera.StartPreview();
        }

        private void SwitchCameraButtonTapped(object sender, EventArgs e)
        {
            if (cameraType == CameraFacing.Front)
            {
                cameraType = CameraFacing.Back;

                camera.StopPreview();
                camera.Release();
                camera = Camera.Open((int) cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
            else
            {
                cameraType = CameraFacing.Front;

                camera.StopPreview();
                camera.Release();
                camera = Camera.Open((int) cameraType);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        private void ToggleFlashButtonTapped(object sender, EventArgs e)
        {
            _flashOn = !_flashOn;
            if (_flashOn)
            {
                if (cameraType == CameraFacing.Back)
                {
                    toggleFlashButton.SetBackgroundResource(Resource.Drawable.FlashButton);
                    cameraType = CameraFacing.Back;

                    camera.StopPreview();
                    camera.Release();
                    camera = Camera.Open((int) cameraType);
                    var parameters = camera.GetParameters();
                    parameters.FlashMode = Camera.Parameters.FlashModeTorch;
                    camera.SetParameters(parameters);
                    camera.SetPreviewTexture(surfaceTexture);
                    PrepareAndStartCamera();
                }
            }
            else
            {
                toggleFlashButton.SetBackgroundResource(Resource.Drawable.NoFlashButton);
                camera.StopPreview();
                camera.Release();

                camera = Camera.Open((int) cameraType);
                var parameters = camera.GetParameters();
                parameters.FlashMode = Camera.Parameters.FlashModeOff;
                camera.SetParameters(parameters);
                camera.SetPreviewTexture(surfaceTexture);
                PrepareAndStartCamera();
            }
        }

        private async void TakePhotoButtonTapped(object sender, EventArgs e)
        {
            camera.StopPreview();

            var image = textureView.Bitmap;
            _selectedImage = image.GetScaledAndRotatedBitmap(SurfaceOrientation.Rotation0,
                Constants.MaxPixelDimensionOfImages);
            Finish();
            publishSelectedImage();
        }

        public void publishSelectedImage()
        {
            var messenger = DependencyService.Get<IMvxMessenger>();
            messenger.Publish(new CameraImageSelectedMessage(this, _receiverID, _selectedImage));
        }

        #endregion
    }
}
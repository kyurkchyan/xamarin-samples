using System;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ImageSample.Droid.Services;
using MvvmCross.Platform;
using MvvmCross.Plugins.Messenger;
using Nito.AsyncEx;
using Xamarin.Forms;
using View = Android.Views.View;

namespace ImageSample.Droid.Views
{
    [Activity(Label = "PhotoGaleryActivity", Icon = "@drawable/icon", 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class PhotoGaleryActivity : Activity
    {
        #region Private fields and properties

        private RecyclerView _recyclerView;
        private GridLayoutManager _layoutManager;
        private PhotoGaleryAdapter _adapter;
        private PhotoGalery _galery;
        private static readonly string KEY_RECEIVER_ID = "RECEIVER_ID";
        private Guid _receiverID = Guid.Empty;
        private string _selectedImagePath;

        #endregion

        #region Lifecycle management

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (Intent != null)
            {
                _receiverID = new Guid(Intent.GetStringExtra(KEY_RECEIVER_ID));
            }

            // Create your application here
            SetContentView(Resource.Layout.activity_photo_galery);

            _layoutManager = new GridAutofitLayoutManager(this, 150);
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recycler_view_photo_galery);
            _recyclerView.SetLayoutManager(_layoutManager);

            _galery = new PhotoGalery(this);
            await _galery.LoadImages();
            _adapter = new PhotoGaleryAdapter(_galery, imageSelected);
            _recyclerView.SetAdapter(_adapter);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            publishSelectedImage();
        }

        #endregion

        #region Public API

        public static Intent GetIntentForReceiver(Context context, Guid receiver)
        {
            var intent = new Intent(context, typeof (PhotoGaleryActivity));
            intent.PutExtra(KEY_RECEIVER_ID, receiver.ToString());
            return intent;
        }

        #endregion

        #region Utility methods

        public void imageSelected(string path)
        {
            _selectedImagePath = path;
            Finish();
            publishSelectedImage();
        }

        public void publishSelectedImage()
        {
            var messenger = DependencyService.Get<IMvxMessenger>();
            messenger.Publish(new LibraryImageSelectedMessage(this, _receiverID, _selectedImagePath));
        }

        #endregion

        public class PhotoGaleryAdapter : RecyclerView.Adapter
        {
            private readonly PhotoGalery _galery;
            private readonly Action<string> _selectedAction;

            public PhotoGaleryAdapter(PhotoGalery galery, Action<string> selectedAction)
            {
                _galery = galery;
                _selectedAction = selectedAction;
            }

            #region implemented abstract members of Adapter

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                var view = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.grid_cell_photo_galery_item, null);
                var holder = new ImageViewHolder(view);
                return holder;
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                var cell = (ImageViewHolder) holder;
                cell.ImageView.Id = position;
                cell.FetchImage = token => _galery.GetThumbnailAtIndex(position, token);
                cell.SelectedAction = () => selectImageAtIndex(position);
                try
                {
                    cell.LoadBitmap();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override int ItemCount
            {
                get { return _galery.Count; }
            }

            #endregion

            #region Utility methods

            private void selectImageAtIndex(int index)
            {
                var path = _galery.GetImagePathAtIndex(index);
                _selectedAction?.Invoke(path);
            }

            #endregion

            class ImageViewHolder : RecyclerView.ViewHolder
            {
                #region Private fields and properties

                static int count = 0;
                private CancellationTokenSource _cancellationToken;
                private readonly AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();

                #endregion

                #region Constructors

                public ImageViewHolder(View parent)
                    : base(parent)
                {
                    ImageView = parent.FindViewById<ImageView>(Resource.Id.thumbImage);
                    parent.Clickable = true;
                    parent.Click += clicked;
                }

                #endregion

                #region Disposable

                protected override void Dispose(bool disposing)
                {
                    if (disposing)
                    {
                        ItemView.Click -= clicked;
                    }
                    base.Dispose(disposing);
                }

                #endregion

                #region Properties

                public ImageView ImageView { get; }
                public Func<CancellationToken, Task<Bitmap>> FetchImage { get; set; }
                public Action SelectedAction { get; set; }

                #endregion

                #region Public API

                public async void LoadBitmap()
                {
                    //If we had already running task on this view, cancel it.
                    if (_cancellationToken != null)
                    {
                        _cancellationToken.Cancel();
                    }

                    //Create new cancellation token
                    _cancellationToken = new CancellationTokenSource();

                    //Set the image to null
                    setImage(null, _cancellationToken.Token);

                    Bitmap image = null;
                    try
                    {
                        image = await FetchImage(_cancellationToken.Token);
                        //If the task is cancelled dispose the image and return
                        if (_cancellationToken.Token.IsCancellationRequested && image != null)
                        {
                            image.Recycle();
                        }
                        else
                        {
                            //Set new image
                            setImage(image, _cancellationToken.Token);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                #endregion

                #region Utility methods

                private void setImage(Bitmap bitmap, CancellationToken token)
                {
                    using (var writerLock = _locker.WriterLock(token))
                    {
                        if (ImageView == null)
                            return;
                        if (ImageView.Drawable != null)
                        {
                            var old = ((BitmapDrawable) ImageView.Drawable).Bitmap;
                            if (old != null && !old.IsRecycled)
                                old.Recycle();
                        }
                        ImageView.SetImageBitmap(bitmap);
                    }
                }

                void clicked(object sender, EventArgs e)
                {
                    SelectedAction?.Invoke();
                }

                #endregion
            }
        }
    }
}
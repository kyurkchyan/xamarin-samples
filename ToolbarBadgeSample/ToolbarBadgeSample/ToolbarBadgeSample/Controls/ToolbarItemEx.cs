using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NGraphics;
using Nito.AsyncEx;
using PCLStorage;
using ToolbarBadgeSample.Services;
using ToolbarBadgeSample.Services.Contracts;
using ToolbarBadgeSample.Toolbox;
using Xamarin.Forms;
using FormsColor = Xamarin.Forms.Color;
using FormsSize = Xamarin.Forms.Size;
using Color = NGraphics.Color;
using Font = NGraphics.Font;
using Path = System.IO.Path;
using Point = NGraphics.Point;
using Size = NGraphics.Size;
using TextAlignment = NGraphics.TextAlignment;

namespace ToolbarBadgeSample.Controls
{
    public class ToolbarItemEx : ToolbarItem
    {
        #region Private fields

        //Constants
        private static readonly string CacheFolderName = "toolbar";
        private const int BatchOperationTimeout = 500;

        //Fields
        public static IFolder _cacheFolder;
        private readonly ICanvasService _canvasService;
        private Mode _mode;
        private static readonly AsyncReaderWriterLock _locker = new AsyncReaderWriterLock();
        private bool _isUpdatingBadge;
        private CancellationTokenSource _cancellationToken;

        //Enum
        private enum Mode
        {
            None,
            File,
            Svg
        }


        #endregion

        #region Constructors

        static ToolbarItemEx()
        {
            InitializeCache();
        }

        public ToolbarItemEx()
        {
            _canvasService = DependencyService.Get<ICanvasService>();
        }

        #endregion

        #region Properties

        #region FileIcon

        public static readonly BindableProperty FileIconProperty = BindableProperty.Create(nameof(FileIcon), typeof(FileImageSource), typeof(ToolbarItemEx),
            defaultValue: null, propertyChanged: OnFileIconChanged);

        public FileImageSource FileIcon
        {
            get { return (FileImageSource)GetValue(FileIconProperty); }
            set { SetValue(FileIconProperty, value); }
        }

        #endregion

        #region SvgIcon

        public static readonly BindableProperty SvgIconProperty = BindableProperty.Create(nameof(SvgIcon),
            typeof(string), typeof(ToolbarItemEx),
            defaultValue: null, propertyChanged: OnSvgIconChanged);

        public string SvgIcon
        {
            get { return (string)GetValue(SvgIconProperty); }
            set { SetValue(SvgIconProperty, value); }
        }

        #endregion

        #region SvgWidth

        public static readonly BindableProperty SvgWidthProperty = BindableProperty.Create(nameof(SvgWidth), typeof(float), typeof(ToolbarItemEx),
            defaultValue: -1f, propertyChanged: OnPropertyChanged);

        public float SvgWidth
        {
            get { return (float)GetValue(SvgWidthProperty); }
            set { SetValue(SvgWidthProperty, value); }
        }

        #endregion

        #region SvgHeight

        public static readonly BindableProperty SvgHeightProperty = BindableProperty.Create(nameof(SvgHeight), typeof(float), typeof(ToolbarItemEx),
            defaultValue: -1f, propertyChanged: OnPropertyChanged);

        public float SvgHeight
        {
            get { return (float)GetValue(SvgHeightProperty); }
            set { SetValue(SvgHeightProperty, value); }
        }

        #endregion

        #region BadgeCount

        public static readonly BindableProperty BadgeCountProperty = BindableProperty.Create(nameof(BadgeCount), typeof(int), typeof(ToolbarItemEx),
            defaultValue: 0, propertyChanged: OnPropertyChanged);

        public int BadgeCount
        {
            get { return (int)GetValue(BadgeCountProperty); }
            set { SetValue(BadgeCountProperty, value); }
        }

        #endregion

        #region IsBadgeVisible

        public static readonly BindableProperty IsBadgeVisibleProperty = BindableProperty.Create(nameof(IsBadgeVisible), typeof(bool), typeof(ToolbarItemEx),
            defaultValue: false, propertyChanged: OnPropertyChanged);

        public bool IsBadgeVisible
        {
            get { return (bool)GetValue(IsBadgeVisibleProperty); }
            set { SetValue(IsBadgeVisibleProperty, value); }
        }

        #endregion

        #region BadgeTextColor

        public static readonly BindableProperty BadgeTextColorProperty = BindableProperty.Create(nameof(BadgeTextColor), typeof(FormsColor), typeof(ToolbarItemEx),
            defaultValue: FormsColor.White, propertyChanged: OnPropertyChanged);

        public FormsColor BadgeTextColor
        {
            get { return (FormsColor)GetValue(BadgeTextColorProperty); }
            set { SetValue(BadgeTextColorProperty, value); }
        }

        #endregion

        #region BadgeColor

        public static readonly BindableProperty BadgeColorProperty = BindableProperty.Create(nameof(BadgeColor), typeof(FormsColor), typeof(ToolbarItemEx),
            defaultValue: FormsColor.Red, propertyChanged: OnPropertyChanged);

        public FormsColor BadgeColor
        {
            get { return (FormsColor)GetValue(BadgeColorProperty); }
            set { SetValue(BadgeColorProperty, value); }
        }

        #endregion

        #region BadgeFontSize

        public static readonly BindableProperty BadgeFontSizeProperty = BindableProperty.Create(nameof(BadgeFontSize), typeof(float), typeof(ToolbarItemEx),
            defaultValue: 10f, propertyChanged: OnPropertyChanged);

        public float BadgeFontSize
        {
            get { return (float)GetValue(BadgeFontSizeProperty); }
            set { SetValue(BadgeFontSizeProperty, value); }
        }

        #endregion

        #region BadgeRadius

        public static readonly BindableProperty BadgeRadiusProperty = BindableProperty.Create(nameof(BadgeRadius), typeof(float), typeof(ToolbarItemEx),
            defaultValue: 5f, propertyChanged: OnPropertyChanged);

        public float BadgeRadius
        {
            get { return (float)GetValue(BadgeRadiusProperty); }
            set { SetValue(BadgeRadiusProperty, value); }
        }

        #endregion

        #region BadgeInsets

        public static readonly BindableProperty BadgeInsetsProperty = BindableProperty.Create(nameof(BadgeInsets), typeof(FormsSize), typeof(ToolbarItemEx),
            defaultValue: new FormsSize(10, 10), propertyChanged: OnPropertyChanged);

        public FormsSize BadgeInsets
        {
            get { return (FormsSize)GetValue(BadgeInsetsProperty); }
            set { SetValue(BadgeInsetsProperty, value); }
        }

        #endregion

        #region BadgePadding

        public static readonly BindableProperty BadgePaddingProperty = BindableProperty.Create(nameof(BadgePadding),
            typeof(Thickness), typeof(ToolbarItemEx),
            defaultValue: new Thickness(5), propertyChanged: OnPropertyChanged);

        private static IDeviceService _deviceService;

        public Thickness BadgePadding
        {
            get { return (Thickness)GetValue(BadgePaddingProperty); }
            set { SetValue(BadgePaddingProperty, value); }
        }

        #endregion


        #endregion

        #region Utility methods

        public static async void InitializeCache()
        {
            IList<IFolder> previouseCache = null;
            using (var writerLock = await _locker.WriterLockAsync())
            {
                _deviceService = DependencyService.Get<IDeviceService>();
                var deviceCache = await _deviceService.GetCacheFolder();
                var cacheRootFolder =
                    await deviceCache.CreateFolderAsync(CacheFolderName, CreationCollisionOption.OpenIfExists);
                previouseCache = await cacheRootFolder.GetFoldersAsync();
                _cacheFolder = await cacheRootFolder.CreateFolderAsync(Path.GetRandomFileName(),
                            CreationCollisionOption.ReplaceExisting);
            }

            //Clear previous cache every time application launches
            //foreach (var folder in previouseCache)
            //{
            //    await folder.DeleteAsync();
            //}
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var item = (ToolbarItemEx)bindable;
            item.UpdateBadge();
        }

        private static void OnFileIconChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var item = (ToolbarItemEx)bindable;
            item._mode = Mode.File;
            item.UpdateBadge();
        }

        private static void OnSvgIconChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var item = (ToolbarItemEx)bindable;
            item._mode = Mode.Svg;
            item.UpdateBadge();
        }

        private async void UpdateBadge()
        {
            _cancellationToken?.Cancel();

            _cancellationToken = new CancellationTokenSource();

            try
            {
                using (var writerLock = await _locker.WriterLockAsync(_cancellationToken.Token))
                {
                    string icon = null;
                    if (!IsBadgeVisible && _mode == Mode.File)
                    {
                        icon = FileIcon?.File;
                    }
                    else
                    {
                        var cachedBadge = await GetCachedBadge(_cancellationToken.Token);
                        _cancellationToken.Token.ThrowIfCancellationRequested();
                        if (cachedBadge != null)
                        {
                            icon = cachedBadge;
                        }
                        else
                        {
                            icon = await CreateBadge(_cancellationToken.Token);
                            _cancellationToken.Token.ThrowIfCancellationRequested();
                        }
                    }

                    SetupIcon(icon);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private void SetupIcon(string icon)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                Icon = icon;

                var page = ParentView.GetParentPage() as NavigationPage;
                var toolbarItems = page?.CurrentPage?.ToolbarItems;
                //var copy = new List<ToolbarItem>();
                //copy.AddRange(toolbarItems);
                //page.CurrentPage.ToolbarItems.Clear();
                //foreach (var toolbarItem in copy)
                //{
                //    page.CurrentPage.ToolbarItems.Add(toolbarItem);
                //}
                var index = toolbarItems?.IndexOf(this);
                if (index >= 0)
                {
                    page.CurrentPage.ToolbarItems.RemoveAt(index.Value);
                    page.CurrentPage.ToolbarItems.Insert(index.Value, this);
                }
            });
        }


        private async Task<string> GetCachedBadge(CancellationToken token)
        {
            string path = null;
            try
            {
                var filename = GetBadgeCacheFileName();
                if (string.IsNullOrEmpty(filename))
                    return null;
                var badge = await _cacheFolder.GetFileAsync(filename, token);
                path = badge?.Path;
            }
            catch
            {
                // ignored
            }
            return path;
        }

        private async Task<string> CreateBadge(CancellationToken token)
        {
            //Get the badge icon
            IImage badgeIcon = null;
            if (_mode == Mode.File)
                badgeIcon = await _canvasService.GetImage(FileIcon, token);
            else if (_mode == Mode.Svg)
                badgeIcon = await _canvasService.GetSvgImage(SvgIcon, SvgWidth, SvgHeight, token);

            if (badgeIcon == null)
                return null;

            token.ThrowIfCancellationRequested();

            IImageCanvas canvas = null;
            if (IsBadgeVisible)
            {
                //InitializeCache frames
                var font = new Font(null, BadgeFontSize);
                var metrics = _canvasService.MeasureText(BadgeCount.ToString(), font);
                var textSize = metrics.Size;
                var badgeSize = new Size(BadgePadding.Left + BadgePadding.Right + textSize.Width,
                    BadgePadding.Top + BadgePadding.Bottom + textSize.Height);
                var canvasSize = new Size(badgeIcon.Size.Width + badgeSize.Width - BadgeInsets.Width,
                    badgeIcon.Size.Height + badgeSize.Height - BadgeInsets.Height);

                //Create canvas
                canvas = _canvasService.GetCanvas(canvasSize);
                token.ThrowIfCancellationRequested();

                //Draw icon
                var imageFrame = new Rect(0, badgeSize.Height - BadgeInsets.Height, badgeIcon.Size.Width,
                    badgeIcon.Size.Height);
                canvas.DrawImage(badgeIcon, imageFrame);
                token.ThrowIfCancellationRequested();

                //Draw the badge rounded rect
                var badgeFrame = new Rect(badgeIcon.Size.Width - BadgeInsets.Width, 0, badgeSize.Width,
                    badgeSize.Height);
                var color = GetColor(BadgeColor);
                var badgeImage = _canvasService.GetRoundedImage(badgeFrame.Size, color, BadgeRadius);
                canvas.DrawImage(badgeImage, badgeFrame);
                token.ThrowIfCancellationRequested();

                //Draw badge text
                var textFrame = new Rect(badgeFrame.X + BadgePadding.Left,
                    badgeFrame.Y + BadgePadding.Top + metrics.Ascent, textSize.Width, textSize.Height);
                canvas.DrawText(BadgeCount.ToString(), textFrame, font, TextAlignment.Center,
                    GetColor(BadgeTextColor));
            }
            else
            {
                //Create canvas
                canvas = _canvasService.GetCanvas(badgeIcon.Size);
                token.ThrowIfCancellationRequested();

                //Draw icon
                var imageFrame = new Rect(0, 0, badgeIcon.Size.Width, badgeIcon.Size.Height);
                canvas.DrawImage(badgeIcon, imageFrame);
            }

            token.ThrowIfCancellationRequested();

            //Get the result image
            var image = canvas.GetImage();

            //Save the image to cache
            var filename = GetBadgeCacheFileName();
            var path = PortablePath.Combine(_cacheFolder.Path, filename);
            await _canvasService.SaveImage(image, _cacheFolder.Path, filename, token);

            return path;
        }

        private string GetBadgeCacheFileName()
        {
            string imageName = null;
            if (_mode == Mode.Svg && !string.IsNullOrEmpty(SvgIcon))
            {
                imageName = $"{SvgIcon}_{SvgWidth}x{SvgHeight}";
            }
            else if (_mode == Mode.File && !string.IsNullOrEmpty(FileIcon?.File))
            {
                imageName = FileIcon?.File;
            }

            if (imageName == null)
                return null;
            var filename = imageName;
            if (IsBadgeVisible)
            {
                var badgeColor = GetColorString(BadgeColor);
                var textColor = GetColorString(BadgeTextColor);
                filename = $"{imageName}_{BadgeCount}_{badgeColor}_{textColor}_" +
                           $"{BadgeFontSize}_{BadgeInsets.Width}x{BadgeInsets.Height}_{BadgeRadius}" +
                           $"{BadgePadding.Left}.{BadgePadding.Top}.{BadgePadding.Right}.{BadgePadding.Bottom}";
            }

            //var hash = $"{filename.GetHashCode():X}@{_deviceService.Scale}x.png";
            var hash = $"{filename.GetHashCode():X}.png";
            return hash;
        }

        private string GetColorString(FormsColor c)
        {
            return $"{(byte)(c.A * byte.MaxValue):X}" +
                   $"{(byte)(c.R * byte.MaxValue):X}" +
                   $"{(byte)(c.G * byte.MaxValue):X}" +
                   $"{(byte)(c.B * byte.MaxValue):X}";
        }

        private Color GetColor(FormsColor c)
        {
            return new Color(c.R, c.G, c.B, c.A);
        }


        #endregion
    }
}

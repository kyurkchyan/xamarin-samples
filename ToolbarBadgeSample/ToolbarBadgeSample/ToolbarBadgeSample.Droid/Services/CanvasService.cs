using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using NGraphics;
using PCLStorage;
using ToolbarBadgeSample.Controls;
using ToolbarBadgeSample.Droid.Services;
using ToolbarBadgeSample.Services.Contracts;
using ToolbarBadgeSample.Toolbox;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Android.Graphics.Color;
using File = Java.IO.File;
using Font = NGraphics.Font;
using Point = NGraphics.Point;
using Rect = NGraphics.Rect;
using Size = NGraphics.Size;

[assembly: Dependency(typeof(CanvasService))]

namespace ToolbarBadgeSample.Droid.Services
{
    public class CanvasService : ICanvasService
    {
        #region Private fields

        private readonly AndroidPlatform _platform;
        private readonly float _scale;

        #endregion


        #region Constructors

        public CanvasService()
        {
            _platform = new AndroidPlatform();
            var metrics = Forms.Context.Resources.DisplayMetrics;
            _scale = ((float) metrics.DensityDpi/(int) DisplayMetricsDensity.Default);
        }

        #endregion

        #region ICanvasService implementation

        public async Task<IImage> GetImage(FileImageSource file, CancellationToken token)
        {
            var handler = new FileImageSourceHandler();
            var bitmap = await handler.LoadImageAsync(file, Forms.Context, token);

            if (bitmap == null)
                return null;

            var image = new BitmapImage(bitmap, _scale);

            return image;
        }

        public async Task<IImage> GetSvgImage(string path, float width, float height, CancellationToken token)
        {
            return await  Task<IImage>.Factory.StartNew(() =>
            {
                var assembly = typeof(ToolbarItemEx).Assembly;
                var fullPath = $"ToolbarBadgeSample.Resources.Images.{path}";
                using (var resource = assembly.GetManifestResourceStream(fullPath))
                {
                    if (resource == null)
                    {
                        throw new Exception(
                            $"Error retrieving {path} make sure Build Action is Embedded Resource. Full path {fullPath}");
                    }
                    var svg = new SvgReader(new StreamReader(resource));
                    var outputSize = svg.Graphic.Size;
                    if (width > 0 && height > 0)
                    {
                        outputSize = new Size(width, height);
                        var scale = svg.Graphic.Size.ScaleThatFits(outputSize);
                        svg.Graphic.ViewBox = new Rect(Point.Zero, svg.Graphic.Size / scale);
                    }

                    var imageCanvas = _platform.CreateImageCanvas(outputSize, _scale);
                    svg.Graphic.Draw(imageCanvas);
                    return imageCanvas.GetImage();
                }
            }, token);
        }

        public IImageCanvas GetCanvas(Size size)
        {
            return _platform.CreateImageCanvas(size);
        }

        public TextMetrics MeasureText(string text, Font font)
        {
            return _platform.MeasureText(text, font);
        }

        public Task SaveImage(IImage image, string path, string name, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
            {
                var filename = PortablePath.Combine(path, name);
                using (var output = new FileStream(filename, FileMode.Create))
                {
                    var bitmapImage = (BitmapImage)image;
                    bitmapImage.Bitmap.Compress(Bitmap.CompressFormat.Png, 100, output);
                }
            }, token);
        }

        public IImage GetRoundedImage(Size size, NGraphics.Color color, float radius)
        {
            var paint = new Paint();
            paint.AntiAlias = true;
            paint.SetARGB(color.A, color.R, color.G, color.B);
            paint.SetStyle(Paint.Style.Fill);
            var conf = Bitmap.Config.Argb8888;
            var bitmap = Bitmap.CreateBitmap(Forms.Context.Resources.DisplayMetrics, (int)size.Width, (int)size.Height, conf);
            var canvas = new Canvas(bitmap);
            var rect = new RectF(0, 0, (float)size.Width, (float)size.Height);
            canvas.DrawRoundRect(rect, radius, radius, paint);
            var image = new BitmapImage(bitmap, _scale);
            return image;
        }

        #endregion


    }
}
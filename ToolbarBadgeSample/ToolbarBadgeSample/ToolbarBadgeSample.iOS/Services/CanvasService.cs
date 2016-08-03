using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreGraphics;
using NGraphics;
using PCLStorage;
using ToolbarBadgeSample.Controls;
using ToolbarBadgeSample.iOS.Services;
using ToolbarBadgeSample.Services;
using ToolbarBadgeSample.Services.Contracts;
using ToolbarBadgeSample.Toolbox;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using FileAccess = System.IO.FileAccess;
using Font = NGraphics.Font;
using Path = System.IO.Path;
using Point = NGraphics.Point;
using Size = NGraphics.Size;

[assembly: Dependency(typeof(CanvasService))]

namespace ToolbarBadgeSample.iOS.Services
{
    public class CanvasService : ICanvasService
    {
        #region Private fields

		private readonly ApplePlatform _platform;
        private nfloat _scale = UIScreen.MainScreen.Scale;

        #endregion


        #region Constructors

        public CanvasService()
        {
			_platform = new ApplePlatform();
            
        }

        #endregion

        #region ICanvasService implementation

        public async Task<IImage> GetImage(FileImageSource file, CancellationToken token)
        {
            var handler = new FileImageSourceHandler();
            var nativeImage = await handler.LoadImageAsync(file, token);

            if (nativeImage == null)
                return null;

			var image = new CGImageImage(nativeImage.CGImage, _scale);

            return image;
        }

        public Task<IImage> GetSvgImage(string path, float width, float height, CancellationToken token)
        {
            return Task<IImage>.Factory.StartNew(() =>
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
                        outputSize = new Size(width, height)*_scale;
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

		public IImage GetRoundedImage(Size size, NGraphics.Color color, float radius)
		{
		    size *= _scale;
            UIGraphics.BeginImageContextWithOptions(new CGSize(size.Width, size.Height), false, 0);
            IImage image = null;
            using (var context = UIGraphics.GetCurrentContext())
            {
                context.SetAllowsAntialiasing(true);
                context.SetShouldAntialias(true);

                context.SetFillColor(UIColor.FromRGBA(color.R, color.G, color.B, color.A).CGColor);
                context.AddPath(CGPath.FromRoundedRect(new CGRect(0, 0, size.Width, size.Height), radius, radius));
                context.FillPath();
            }
            var nativeImage = UIGraphics.GetImageFromCurrentImageContext();
            image = new CGImageImage(nativeImage.CGImage, _scale);
            return image;

        }

        public  async Task SaveImage(IImage image, string path, string name, CancellationToken token)
        {
            var folder = await FileSystem.Current.GetFolderFromPathAsync(path, token);
            var badge = await folder.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting, token);
            using (var stream = await badge.OpenAsync(PCLStorage.FileAccess.ReadAndWrite, token))
            {
                image.SaveAsPng(stream);
            }
        }

        #endregion


    }
}
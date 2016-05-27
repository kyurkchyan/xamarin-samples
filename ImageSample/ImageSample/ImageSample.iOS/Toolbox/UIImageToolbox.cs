using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;

namespace ImageSample.iOS.Toolbox
{
    public enum ImageType
    {
        Jpeg = 0,
        Png = 1
    }

    public static class UIImageToolbox
    {
        /// <summary>
        /// Loades image from path.
        /// </summary>
        /// <returns>The image with specified path if found, returns null if not found.</returns>
        /// <param name="path">Image path.</param>
        public static async Task<UIImage> GetImageFromPath(this string path)
        {
//			try {
//				using(var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
//				{
//					var rawImage = new byte[stream.Length];
//					await stream.ReadAsync(rawImage,0,(int)stream.Length);
//					return rawImage.GetUIImage();
//				}
//			} catch (Exception ex) {
//				Console.WriteLine (ex);
//				return null;
//			}

            try
            {
//				var image = await Task<UIImage>.Run(delegate {
//					return new UIImage();//UIImage.FromFile(path);
//				});
                return UIImage.FromFile(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets <c>UIImage</c> instance from raw byte array
        /// </summary>
        /// <returns><c>UIImage</c> representing the raw byte array if successful, otherwise returns <c>null</c> </returns>
        /// <param name="rawImage">Raw image byte array.</param>
        public static UIImage GetUIImage(this byte[] rawImage)
        {
            try
            {
                var image = UIImage.LoadFromData(NSData.FromArray(rawImage));
                return image;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets the raw byte representation of <c>UIImage</c> instance
        /// </summary>
        /// <returns>If conversation was successful returns the raw byte representation of <c>UIImage</c> instance, otherwise returns <c>null</c></returns>
        /// <param name="image"><c>UIImage</c> instance.</param>
        /// <param name="imageType">Image type. Default is <c>ImageType.Jpeg</c></param>
        public static byte[] GetRawBytes(this UIImage image, ImageType imageType = ImageType.Jpeg)
        {
            try
            {
                using (var imageData = image.GetImageData(imageType))
                {
                    var rawBytes = new byte[imageData.Length];
                    Marshal.Copy(imageData.Bytes, rawBytes, 0, Convert.ToInt32(imageData.Length));
                    return rawBytes;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// Gets the NSData representation of <c>UIImage</c> instance
        /// </summary>
        /// <returns>If conversation was successful returns the NSData representation of <c>UIImage</c> instance, otherwise returns <c>null</c></returns>
        /// <param name="image"><c>UIImage</c> instance.</param>
        /// <param name="imageType">Image type. Default is <c>ImageType.Jpeg</c></param>
        public static NSData GetImageData(this UIImage image, ImageType imageType = ImageType.Jpeg)
        {
            try
            {
                switch (imageType)
                {
                    case ImageType.Png:
                        return image.AsPNG();
                    case ImageType.Jpeg:
                    default:
                        return image.AsJPEG();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static UIImage ImageWithColor(UIColor color)
        {
            var rect = new CGRect(0.0f, 0.0f, 1.0f, 1.0f);
            UIGraphics.BeginImageContext(rect.Size);
            var context = UIGraphics.GetCurrentContext();

            context.SetFillColor(color.CGColor);
            context.FillRect(rect);

            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;
        }


        public const double RadToDeg = 180.0/Math.PI;
        public const double DegToRad = Math.PI/180.0;

        public static UIImage ScaleImage(this UIImage owner, CGSize size, float screenScale = 0f)
        {
            if (null == owner)
            {
                throw new NullReferenceException("Image object is null!");
            } //end if

            UIImage toReturn = null;
            UIGraphics.BeginImageContextWithOptions(size, false, screenScale);
            using (var context = UIGraphics.GetCurrentContext())
            {
                context.TranslateCTM(0f, size.Height);
                context.ScaleCTM(1f, -1f);
                context.DrawImage(new CGRect(0f, 0f, size.Width, size.Height), owner.CGImage);

                toReturn = UIGraphics.GetImageFromCurrentImageContext();
            } //end using context
            UIGraphics.EndImageContext();

            return toReturn;
        } //end static UIImage ScaleImage


        public static UIImage RotateToCorrectOrientation(this UIImage owner)
        {
            UIImage toReturn = null;
            var degAngle = 0f;

            switch (owner.Orientation)
            {
                case UIImageOrientation.Down:

                    degAngle = 180;

                    break;

                case UIImageOrientation.Left:

                    degAngle = -90f;

                    break;

                case UIImageOrientation.Right:

                    degAngle = 90f;

                    break;

                default:

                    return owner;
            } //end switch

            UIGraphics.BeginImageContextWithOptions(owner.Size, true, 1f);
            using (var context = UIGraphics.GetCurrentContext())
            {
                owner.Draw(new CGPoint(0f, 0f));
                context.RotateCTM(degAngle*(float) DegToRad);

                owner.Dispose();
                toReturn = UIGraphics.GetImageFromCurrentImageContext();
            } //end using context
            UIGraphics.EndImageContext();

            return toReturn;
        }


        public static UIImage CreateThumbnail(this UIImage img, int maxSize, float screenScale = 0f)
        {
            var imgSize = img.Size;
            UIImage scaledImage = null;

            nfloat ratio = 0f;
            if (imgSize.Width > imgSize.Height)
            {
                ratio = imgSize.Width/imgSize.Height;
                scaledImage = img.ScaleImage(new CGSize(maxSize*ratio, maxSize), screenScale);
            }
            else
            {
                ratio = imgSize.Height/imgSize.Width;
                scaledImage = img.ScaleImage(new CGSize(maxSize, maxSize*ratio), screenScale);
            } //end if else

            using (var cImage = scaledImage.CGImage)
            {
                var cropRect = new CGRect(scaledImage.Size.Width/2f - maxSize/2f,
                    scaledImage.Size.Height/2f - maxSize/2f,
                    maxSize,
                    maxSize);
                using (var croppedImage = cImage.WithImageInRect(cropRect))
                {
                    return new UIImage(croppedImage);
                } //end using croppedImage
            } //end using cImage
        } //end UIImage CreateThumbnail

        public static UIImage GetScaledAndRotatedImage(this UIImage image, float maxPixelDimension)
        {
            var imageSize = image.Size;
            var newImageSize = CGSize.Empty;
            if (maxPixelDimension > 0)
            {
                if (imageSize.Width > imageSize.Height)
                {
                    var ratio = imageSize.Width/maxPixelDimension;
                    newImageSize = new CGSize(maxPixelDimension, imageSize.Height/ratio);
                }
                else
                {
                    var ratio = imageSize.Height/maxPixelDimension;
                    newImageSize = new CGSize(imageSize.Width/ratio, maxPixelDimension);
                }
            }

            var rotatedImage = image.RotateToCorrectOrientation();
            if (newImageSize != CGSize.Empty)
            {
                var result = rotatedImage.ScaleImage(newImageSize, 1);
                rotatedImage.Dispose();
                return result;
            }
            return rotatedImage;
        }
    }
}
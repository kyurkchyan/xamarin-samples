using System.Threading;
using System.Threading.Tasks;
using NGraphics;
using Xamarin.Forms;
using Font = NGraphics.Font;
using Size = NGraphics.Size;
using Color = NGraphics.Color;
namespace ToolbarBadgeSample.Services.Contracts
{
    public interface ICanvasService
    {
        Task<IImage> GetImage(FileImageSource file, CancellationToken token);
        Task<IImage> GetSvgImage(string path, float width, float height, CancellationToken token);
		IImage GetRoundedImage(Size size, Color color, float radius);

        IImageCanvas GetCanvas(Size size);
        TextMetrics MeasureText(string text, Font font);
        Task SaveImage(IImage image, string path, string name, CancellationToken token);
    }
}

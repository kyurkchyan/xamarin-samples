using System.Threading.Tasks;
using ImageSample.Model;

namespace ImageSample.Services
{
    public interface IImageService
    {
        Task<IImage> GetImageFromLibrary();
        Task<IImage> GetImageFromCamera();
    }
}
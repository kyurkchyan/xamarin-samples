using System.Threading.Tasks;
using PCLStorage;

namespace ToolbarBadgeSample.Services.Contracts
{
    public interface IDeviceService
    {
        IFolder CacheFolder { get; }
        Task<IFolder> GetCacheFolder();
        float Scale { get; }
    }
}

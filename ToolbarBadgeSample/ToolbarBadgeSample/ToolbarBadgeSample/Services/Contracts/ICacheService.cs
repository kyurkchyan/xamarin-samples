using System.Threading.Tasks;
using PCLStorage;

namespace ToolbarBadgeSample.Services.Contracts
{
    public interface ICacheService
    {
        Task<byte[]> GetCachedItem(IFolder folder, string cacheId);
        Task StoreItem(IFolder folder, byte[] item, string cacheId);
    }
}

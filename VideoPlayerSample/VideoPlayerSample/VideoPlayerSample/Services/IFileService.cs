using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace VideoPlayerSample.Services
{
    public interface IFileService
    {
        Task<IFolder> GetCacheFolder();
        Task CopyResource(string resourceName, string targetName, IFolder targetFolder);
    }
}

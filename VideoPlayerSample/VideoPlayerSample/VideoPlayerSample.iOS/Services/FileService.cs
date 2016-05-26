using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using VideoPlayerSample.iOS.Services;
using VideoPlayerSample.Services;
using FileAccess = PCLStorage.FileAccess;

[assembly: Xamarin.Forms.Dependency(typeof(FileService))]
namespace VideoPlayerSample.iOS.Services
{
    public class FileService : IFileService
    {
        public async Task CopyResource(string resourceName, string targetName, IFolder targetFolder)
        {
            using (Stream resource = typeof(IFileService).GetTypeInfo().Assembly.GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    throw new ArgumentException("No such resource", "resourceName");
                }

                var file = await targetFolder.CreateFileAsync(targetName, CreationCollisionOption.ReplaceExisting);

                using (var output = await file.OpenAsync(FileAccess.ReadAndWrite))
                {
                    await resource.CopyToAsync(output);
                }
            }
        }

        public Task<IFolder> GetCacheFolder()
        {
            return FileSystem.Current.LocalStorage.GetFolderAsync("Caches");
        }
    }
}

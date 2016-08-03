using System;
using System.IO;
using System.Threading.Tasks;
using PCLStorage;
using ToolbarBadgeSample.iOS.Services;
using ToolbarBadgeSample.Services.Contracts;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(DeviceService))]
namespace ToolbarBadgeSample.iOS.Services
{
    public class DeviceService : IDeviceService
    {
        private IFolder _cacheFolder;
        public IFolder CacheFolder => _cacheFolder ?? (_cacheFolder = GetCacheFolder().Result);

        public async Task<IFolder> GetCacheFolder()
        {
            if (_cacheFolder != null)
                return _cacheFolder;

            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Directory.GetParent(documents).FullName;
            var cache = Path.Combine(path, "Library", "Caches");

            _cacheFolder = await FileSystem.Current.GetFolderFromPathAsync(cache).ConfigureAwait(false);
            return _cacheFolder;
        }


        private float? _scale;
        public float Scale
        {
            get
            {
                if (_scale == null)
                {
                    _scale = (float)UIScreen.MainScreen.Scale;
                }
                return _scale.Value;
            }
        }

        public void MakeFileReadable(string path)
        {
            
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using FontSample.Droid.Services;
using FontSample.Model;
using FontSample.Services;
using PCLStorage;

[assembly: Xamarin.Forms.Dependency(typeof(FontService))]
namespace FontSample.Droid.Services
{
    public class FontService : BaseFontService
    {
        protected override Task<IFolder> GetCacheFolder()
        {
            return FileSystem.Current.LocalStorage.GetFolderAsync("../cache");
        }

        protected override async Task InstallFonts(IEnumerable<CustomFont> fonts)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FontSample.Model;
using Newtonsoft.Json;
using PCLStorage;

namespace FontSample.Services
{
    public abstract class BaseFontService : IFontService
    {
        #region Private fields

        private const string FontFolderName = "Fonts";
        private const string MetadataFileName = ".metadata";

        #endregion

        #region IFontService implementation

        public Dictionary<string, string> Fonts { get; private set; }
        public IFolder FontFolder { get; private set; }

        public async Task LoadAvailableFonts()
        {
            //Install fonts before trying to get them
            await InstallFonts();
            var metadata = await FontFolder.GetFileAsync(MetadataFileName);
            if (metadata == null)
                return;
            var json = await metadata.ReadAllTextAsync();
            var fonts = JsonConvert.DeserializeObject<CustomFont[]>(json);

            Fonts = fonts.ToDictionary(f => f.Name, f => f.File);
        }

        #endregion


        #region Protected API

        //We will cache all the fonts in caches folder, which is device specific, 
        //That's why we use abstract property which we'll implement separately for iOS and android
        //and inject into PCL using dependency injection
        protected abstract Task<IFolder> GetCacheFolder();

        //For iOS we need to register custom fonts with iOS runtime. 
        //Check FontService implementation for more details.
        protected abstract void InstallFonts(IEnumerable<CustomFont> fonts);

        #endregion


        #region Utility methods

        private async Task InstallFonts()
        {
            var cache = await GetCacheFolder();
            FontFolder = await cache.CreateFolderAsync(FontFolderName, CreationCollisionOption.OpenIfExists);

            //If metadata exits, it means we've already downloaded all necessary fonts
            var metadataExists = await FontFolder.CheckExistsAsync(MetadataFileName);
            if (metadataExists == ExistenceCheckResult.FileExists)
                return;

            //Otherwise we need to download and install fonts in our caches folder
            var fonts = await DownloadFonts();

            //Create metadata file which will hold what fonts we have
            //We'll use this metadata file later to get list of available fonts
            var json = JsonConvert.SerializeObject(fonts);
            var metadata = await FontFolder.CreateFileAsync(MetadataFileName, CreationCollisionOption.ReplaceExisting);
            await metadata.WriteAllTextAsync(json);

            //Now we need to create files for each font in our fonts folder
            foreach (var font in fonts)
            {
                var fontFile = await FontFolder.CreateFileAsync(font.File, CreationCollisionOption.ReplaceExisting);
                using (var stream = await fontFile.OpenAsync(FileAccess.ReadAndWrite))
                {
                    await stream.WriteAsync(font.Data, 0, font.Data.Length);
                }
            }

            //Install fonts
            InstallFonts(fonts);
        }

        //For simplicity sake I've added all my fonts to assembly resources
        //In your app you can make actual web request to download the font file
        private async Task<List<CustomFont>> DownloadFonts()
        {
            var assembly = typeof (IFontService).GetTypeInfo().Assembly;
            //var fontNames = new[] { "AvenirLTPro-Heavy.ttf", "AvenirLTPro-Light.ttf", "AvenirLTPro-Medium.ttf", "AvenirLTPro-Roman.ttf" };
            var fontNames = assembly.GetManifestResourceNames().Where(r => r.EndsWith(".ttf"));
            List<CustomFont> fonts = new List<CustomFont>();
            foreach (var fontName in fontNames)
            {
                using (var resource = assembly.GetManifestResourceStream(fontName))
                {
                    var data = new byte[resource.Length];
                    using (var stream = new MemoryStream(data))
                    {
                        await resource.CopyToAsync(stream);
                    }
                    var name = fontName.Replace(".ttf", "").Split('.').Last();
                    fonts.Add(new CustomFont()
                    {
                        Name = name,
                        File = name+".ttf",
                        Data = data
                    });
                }
            }
            return fonts;
        }

        #endregion

    }
}

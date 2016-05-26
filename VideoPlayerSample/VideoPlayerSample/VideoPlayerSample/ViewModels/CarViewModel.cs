using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using PCLStorage;
using VideoPlayerSample.Services;
using Xamarin.Forms;

namespace VideoPlayerSample.ViewModels
{
    public class CarViewModel : BaseViewModel
    {
        private readonly IFileService _fileService;

        public CarViewModel()
        {
            _fileService = DependencyService.Get<IFileService>();
            Load();
        }


        private string _videoPath;

        public string VideoPath
        {
            get
            {
                return _videoPath;
            }
            set
            {
                _videoPath = value;
                RaisePropertyChanged(() => VideoPath);
            }
        }

        private async void Load()
        {
            var cache = await _fileService.GetCacheFolder();
            var fileName = "bmw.mp4";
            var alreadyCopied = await cache.CheckExistsAsync(fileName);
            if (alreadyCopied != ExistenceCheckResult.FileExists)
            {
                await _fileService.CopyResource("VideoPlayerSample.Resources."+fileName, fileName, cache);
            }
            VideoPath = PortablePath.Combine(cache.Path, fileName);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using PCLStorage;
using ToolbarBadgeSample.Droid.Services;
using ToolbarBadgeSample.Services.Contracts;
using Xamarin.Forms;
using Environment = Android.OS.Environment;

[assembly: Dependency(typeof(DeviceService))]
namespace ToolbarBadgeSample.Droid.Services
{
    public class DeviceService : IDeviceService
    {
        private IFolder _cacheFolder;
        public IFolder CacheFolder => _cacheFolder ?? (_cacheFolder = GetCacheFolder().Result);

        public async Task<IFolder> GetCacheFolder()
        {
            var folder = await FileSystem.Current.GetFolderFromPathAsync(Environment.ExternalStorageDirectory.Path);
            return folder;

            _cacheFolder = _cacheFolder ??
                           (_cacheFolder = await FileSystem.Current.GetFolderFromPathAsync(Forms.Context.CacheDir.Path).ConfigureAwait(false));

            return _cacheFolder;
        }

        private float? _scale;
        public float Scale  
        {
            get
            {
                if(_scale == null)
                {
                    var metrics = Forms.Context.Resources.DisplayMetrics;
                    _scale = ((float)metrics.DensityDpi / (int)DisplayMetricsDensity.Default);
                }
                return _scale.Value;
            }
        }
    }
}
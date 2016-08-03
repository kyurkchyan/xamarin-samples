using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using ToolbarBadgeSample.Controls;
using ToolbarBadgeSample.Services.Contracts;
using Xamarin.Forms;

namespace ToolbarBadgeSample.Views
{
    public partial class ToolbarPage : ContentPage
    {
        public ToolbarPage()
        {
            InitializeComponent();

          //  test();
        }

        private async void test()
        {
            
            await Task.Delay(3000);

            ToolbarItems.Clear();

            await Task.Delay(2000);

            var folder = ToolbarItemEx._cacheFolder;
            var files = await folder.GetFilesAsync();
            foreach (var file in files)
            {
                var toolbaritem = new ToolbarItem("", file.Path, () => { });
                ToolbarItems.Add(toolbaritem);
                //_item.Icon = file.Path;
                //await Task.Delay(2000);
            }

            var cache = DependencyService.Get<IDeviceService>().CacheFolder;
            var path = PortablePath.Combine(cache.Path, "toolbar/wfh8r7cd.9a9/events1.png");
            ToolbarItems.Add(new ToolbarItem("",path, () => { }));
            path = PortablePath.Combine(cache.Path, "toolbar/wfh8r7cd.9a9/messages1.png");
            ToolbarItems.Add(new ToolbarItem("", path, () => { }));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontSample.Services;
using Xamarin.Forms;

namespace FontSample.ViewModels
{
    public class FontDemoViewModel : BaseViewModel
    {
        #region Private fields

        private readonly IFontService _fontService;

        #endregion

        #region Constructors

        public FontDemoViewModel()
        {
            _fontService = DependencyService.Get<IFontService>();
            Load();
        }

        #endregion

        #region Properties

        public static string FontsProperty = "Fonts";
        private List<string> _fonts;
        public List<string> Fonts
        { 
            get  
            {
                return _fonts; 
            }
            set 
            {
                _fonts = value; 
                RaisePropertyChanged(() => Fonts); 
            }
        }  

        public static string FontProperty = "Font";
        private string _font;
        public string Font
        { 
            get  
            {
                return _font; 
            }
            set 
            {
                _font = value; 
                RaisePropertyChanged(() => Font); 
            }
        }  
        #endregion

        #region Utility methods

        private async void Load()
        {
            await _fontService.LoadAvailableFonts();
            Fonts = _fontService.Fonts.Keys.ToList();
            Font = Fonts.FirstOrDefault();
        }

        #endregion
    }
}

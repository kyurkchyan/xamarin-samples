using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontSample.Model;
using PCLStorage;

namespace FontSample.Services
{
    public interface IFontService
    {
        //Directory where we'll keep custom fonts
        IFolder FontFolder { get; }
        //Contains mapping between font name and font file
        Dictionary<string, string> Fonts { get; }
        //Will download and install custom fonts and instantiate the Fonts mapping property
        Task LoadAvailableFonts();
    }
}

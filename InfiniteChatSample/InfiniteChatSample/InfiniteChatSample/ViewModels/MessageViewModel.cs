using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfiniteChatSample.ViewModels
{
    public class MessageViewModel : BaseViewModel
    {
        public bool IsFromCurrentUser { get; set; }
        public string Content { get; set; }
        public string ProfilePicture { get; set; }
    }
}

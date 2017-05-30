using InfiniteChatSample.ViewModels;
using Xamarin.Forms;

namespace InfiniteChatSample
{
    public class ChatDataTemplateSelector : DataTemplateSelector
    {
        private readonly DataTemplate _thisUserDataTemplate;
        private readonly DataTemplate _otherUserDataTemplate;

        public ChatDataTemplateSelector()
        {
            _thisUserDataTemplate = new DataTemplate(typeof(ThisUserMessageCell));
            _otherUserDataTemplate = new DataTemplate(typeof(OtherUserMessageCell));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var message = item as MessageViewModel;
            if(message == null) return null;
            if (message.IsFromCurrentUser)
            {
                return _thisUserDataTemplate;
            }
            else
            {
                return _otherUserDataTemplate;
            }
        }
    }
}
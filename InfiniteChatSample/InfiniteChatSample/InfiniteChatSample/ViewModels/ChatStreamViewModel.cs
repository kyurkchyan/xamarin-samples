using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InfiniteChatSample.ViewModels
{
    public class ChatStreamViewModel : BaseViewModel
    {
        private const string ThisUserImage = "http://www.american.edu/uploads/profiles/large/chris_palmer_profile_11.jpg";
        private const string OtherUserImage = "https://blog.linkedin.com/content/dam/blog/en-us/corporate/blog/2014/07/Anais_Saint-Jude_L4388_SQ.jpg.jpeg";
        private const string LoremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut dapibus, orci ac tempor hendrerit, metus odio mattis leo, a accumsan tellus diam id ligula. Aliquam erat volutpat. Proin vulputate urna in ipsum egestas, eget imperdiet quam pretium. Etiam sagittis tincidunt massa nec egestas. Fusce vehicula felis nisi, at tempus nisl accumsan quis. Mauris fringilla nisi eros, non bibendum velit auctor sit amet. Maecenas tristique non nulla id condimentum.";
        private readonly Random _indexRandom;
        private readonly Random _lengthRandom;

        public ChatStreamViewModel()
        {
            Messages = new ObservableCollection<MessageViewModel>();
            _indexRandom = new Random();
            _lengthRandom = new Random();
        }

        public ObservableCollection<MessageViewModel> Messages { get; }

        private bool _isLoading;
        public bool IsLoading
        { 
            get  
            {
                return _isLoading; 
            }
            set 
            {
                _isLoading = value; 
                RaisePropertyChanged();
            }
        }  

        public event EventHandler DidCompleteInitialLoad;
        public event EventHandler DidInsertNextPage;
        public event EventHandler WillInsertNextPage;

        public async void Load()
        {
            IsLoading = true;
            //Mock some messages
            var newMessages = await Task<MessageViewModel[]>.Factory.StartNew(() => GetMockMessages());

            //Simulate backend loading timeout
            await Task.Delay(1000);

            var previousCount = Messages.Count;
            int index = 0;

            //Notify that we're going to change messages, so that UI can store current list position and later restore it
            WillInsertNextPage?.Invoke(this, EventArgs.Empty);
            foreach(var message in newMessages)
            {
                Messages.Insert(index++, message);
            }

            if(previousCount == 0)
            {
                //Notify the UI, that this is first load, and the chat page should scroll to bottom to display the last message
                DidCompleteInitialLoad?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                //Notify the UI that we completed inserting new items, and that chat page should try to restore previous scroll position
                DidInsertNextPage?.Invoke(this, EventArgs.Empty);
            }
            IsLoading = false;
        }

        private MessageViewModel[] GetMockMessages()
        {
            return Enumerable.Range(Messages.Count, 20).Reverse().Select(i =>
                                                                         {
                                                                             var startIndex = _indexRandom.Next(LoremIpsum.Length - 10);
                                                                             var length = _lengthRandom.Next(5, LoremIpsum.Length - startIndex);
                                                                             return new MessageViewModel
                                                                                    {
                                                                                        IsFromCurrentUser = i % 2 == 0,
                                                                                        Content = LoremIpsum.Substring(startIndex, length),
                                                                                        ProfilePicture = i % 2 == 0 ? ThisUserImage : OtherUserImage
                                                                                    };
                                                                         }).ToArray();
        }
    }
}
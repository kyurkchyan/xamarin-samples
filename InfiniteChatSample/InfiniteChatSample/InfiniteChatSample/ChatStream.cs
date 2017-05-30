using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfiniteChatSample.ViewModels;
using Xamarin.Forms;

namespace InfiniteChatSample
{
    public class ChatStream : ListView
    {
        private ChatStreamViewModel _chatStream;
        private int? _topmostItemIndex;
        private int? _previousCount;
        private bool _didScrollToEnd;

        public ChatStream() : base(ListViewCachingStrategy.RecycleElement)
        {
            HasUnevenRows = true;
            SeparatorVisibility = SeparatorVisibility.None;
            ItemSelected += OnItemSelected;
            ItemAppearing += OnItemAppearing;
        }

        public Func<int?> GetFirstVisibleItemIndex { get; set; }
        public Func<int?> GetLastVisibleItemIndex { get; set; }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var chatStream = BindingContext as ChatStreamViewModel;
            if (chatStream != _chatStream && _chatStream != null)
            {
                _chatStream.DidInsertNextPage -= ChatStreamOnDidInsertNextPage;
                _chatStream.DidCompleteInitialLoad -= ChatStreamOnDidCompleteInitialLoad;
                _chatStream.WillInsertNextPage -= ChatStreamOnWillInsertItems;
            }

            if (chatStream != null && _chatStream != chatStream)
            {
                _chatStream = chatStream;
                ItemsSource = _chatStream.Messages;
                _chatStream.DidInsertNextPage += ChatStreamOnDidInsertNextPage;
                _chatStream.DidCompleteInitialLoad += ChatStreamOnDidCompleteInitialLoad;
                _chatStream.WillInsertNextPage += ChatStreamOnWillInsertItems;
            }
        }

        private void ChatStreamOnWillInsertItems(object sender, EventArgs eventArgs)
        {
            _previousCount = _chatStream?.Messages?.Count();
            //Capture what's the currently visible top most item, so we can restore the scroll 
            //position to this after new elements will be added from the top.
            _topmostItemIndex = GetFirstVisibleItemIndex?.Invoke();
        }

        private void ChatStreamOnDidCompleteInitialLoad(object sender, EventArgs eventArgs)
        {
            _didScrollToEnd = false;
            var lastMessage = _chatStream?.Messages?.LastOrDefault();
            ScrollToMessage(lastMessage, ScrollToPosition.End);
        }

        private void ChatStreamOnDidInsertNextPage(object sender, EventArgs eventArgs)
        {
            _topmostItemIndex = _topmostItemIndex ?? 0;
            var newCount = _chatStream.Messages?.Count() ?? 0;
            var oldCount = _previousCount ?? 0;
            if (newCount == oldCount) return;

            var diff = newCount - oldCount;
            var newIndex = diff + _topmostItemIndex;

            var message = _chatStream.Messages.ElementAt(newIndex.Value);
            ScrollToMessage(message, ScrollToPosition.Start);
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs selectedItemChangedEventArgs)
        {
            SelectedItem = null;
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs eventArgs)
        {
            var message = eventArgs.Item as MessageViewModel;
            var index = _chatStream.Messages.IndexOf(message);
            if (index == 0 && _didScrollToEnd && !_chatStream.IsLoading)
            {
                _chatStream.Load();
            }
            if (index == _chatStream.Messages.Count() - 1)
            {
                _didScrollToEnd = true;
            }
        }

        private void ScrollToMessage(MessageViewModel message, ScrollToPosition position, bool animated = false)
        {
            if (message != null)
            {
                ScrollTo(message, position, animated);
            }
            _topmostItemIndex = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabSample.Toolbox;
using Xamarin.Forms;

namespace TabSample.Controls
{
    public class ScrollTabView : BaseTabView
    {
        #region Private fields

        private readonly StackLayout _titleContainer;
        private readonly ScrollViewEx _headerScroll;
        private readonly ContentView _contentContainer;
         
        #endregion

        #region Constructors

        public ScrollTabView()
            : base()
        {
            var rootContainer = new StackLayout()
            {
                Padding = 0,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            _headerScroll = new ScrollViewEx()
            {
                Orientation = ScrollOrientation.Horizontal,
                ShowHorizontalScrollIndicator = false,
                BackgroundColor = Color.White,
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            _headerScroll.SizeChanged += HeaderScrollOnSizeChanged;
            var spacingContainer = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.Transparent,
                Spacing = 0,
                Padding = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            var leftSpacer = new ContentView();
            leftSpacer.SetBinding(ContentView.WidthRequestProperty, new Binding("Width", source:_headerScroll));

            var rightSpacer = new ContentView();
            rightSpacer.SetBinding(ContentView.WidthRequestProperty, new Binding("Width", source: _headerScroll));

            _titleContainer = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.Transparent,
                Spacing = 50,
                Padding = 0
            };

            spacingContainer.Children.Add(leftSpacer);
            spacingContainer.Children.Add(_titleContainer);
            spacingContainer.Children.Add(rightSpacer);

            _contentContainer = new ContentView()
            {
                BackgroundColor = Color.FromHex("#efefef"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            _headerScroll.Content = spacingContainer;
            
            rootContainer.Children.Add(_headerScroll);
            rootContainer.Children.Add(_contentContainer);
            Content = rootContainer;
        }

        #endregion

        #region Properties

        #region ItemWidth

        public static readonly BindableProperty TitleSpacingProperty = BindableProperty.Create(nameof(TitleSpacing), typeof (float), typeof (ScrollTabView),
            propertyChanged:TitleSpacingChanged,
            defaultValue: 50f);

        public float TitleSpacing
        {
            get { return (float) GetValue(TitleSpacingProperty); }
            set { SetValue(TitleSpacingProperty, value); }
        }

        #endregion

        #region HeaderHeight

        public static readonly BindableProperty HeaderHeightProperty = BindableProperty.Create(nameof(HeaderHeight), typeof (float), typeof (ScrollTabView),
            defaultValue: 50f, propertyChanged: OnHeaderHeightChanged);

        public float HeaderHeight
        {
            get { return (float) GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        #endregion

        #endregion

        #region Abstract API implementation

        protected override void SetupTitles()
        {
            ClearTitles();
            if(Tabs?.Any() != true)
                return;
            foreach (var tabItem in Tabs)
            {
                var button = CreateTitle(tabItem);
                _titleContainer.Children.Add(button);
            }
        }

        protected override void SelectTitle(ITabItem tab)
        {
            ScrollToTab(tab);
        }

        protected override void InsertTitle(ITabItem tab, int index)
        {
            var title = CreateTitle(tab);
            _titleContainer.Children.Insert(index, title);
            ScrollToTab(CurrentTab);
        }

        protected override void RemoveTitle(ITabItem tab, int index)
        {
            if(index >= 0)
                _titleContainer.Children.RemoveAt(index);
            ScrollToTab(CurrentTab);
        }

        protected override void SetupContent()
        {
            _contentContainer.Content = null;
        }

        protected override void SelectContent(ITabItem tab, View content)
        {
            _contentContainer.Content = content;
        }

        protected override void RemoveContent(ITabItem tab, View content, int index)
        {
            _contentContainer.Content = null;
        }

        #endregion

        #region Utility methods

        private static void TitleSpacingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = (ScrollTabView)bindable;
            tabView._titleContainer.Spacing = tabView.TitleSpacing;
            tabView.ScrollToTab(tabView.CurrentTab);
        }

        private static void OnHeaderHeightChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabView = (ScrollTabView)bindable;
            tabView._headerScroll.HeightRequest = tabView.HeaderHeight;
        }

        private void HeaderScrollOnSizeChanged(object sender, EventArgs eventArgs)
        {
            ScrollToTab(CurrentTab);
        }

        private void ScrollToTab(ITabItem tab)
        {
            if (Tabs?.Any() != true)
                return;
            var index = Tabs.IndexOf(tab);
            var title = _titleContainer.Children[index];
            var rect = title.Bounds;
            _headerScroll.ScrollToAsync(_titleContainer.Bounds.X + rect.X + rect.Width/2 - _headerScroll.Width/2, _headerScroll.ScrollY, false);
        }

        private ViewButton CreateTitle(ITabItem tabItem)
        {
            var title = (View)TitleTemplate.CreateContent();
            title.BindingContext = tabItem;
            var button = new ViewButton()
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            button.Content = title;
            button.Clicked += ViewButtonOnClicked;
            return button;
        }

        private void ClearTitles()
        {
            foreach (var viewButton in _titleContainer.Children.Cast<ViewButton>())
            {
                viewButton.Clicked -= ViewButtonOnClicked;
            }
            _titleContainer.Children.Clear();
        }

        private void ViewButtonOnClicked(object sender, EventArgs eventArgs)
        {
            if (_titleContainer?.Children?.Any() != true)
                return;
            var index = _titleContainer.Children.IndexOf((ViewButton) sender);
            CurrentTab = Tabs.ElementAt(index);
        }

        #endregion
    }
}

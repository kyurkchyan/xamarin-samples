using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabSample.Toolbox;
using Xamarin.Forms;

namespace TabSample.Controls
{
    public partial class HeaderTabView
    {
        #region Private fields

        private readonly Image _tabImage;

        #endregion

        #region Constructors

        public HeaderTabView()
            : base()
        {
            InitializeComponent();
            _titleContainer.SizeChanged += OnSizeChanged;
            _header.SizeChanged += OnSizeChanged;
        }

        #endregion

        #region Properties

        #region ItemWidth

        public static readonly BindableProperty TitleSpacingProperty = BindableProperty.Create(nameof(TitleSpacing), typeof(float), typeof(HeaderTabView),
            propertyChanged: TitleSpacingChanged,
            defaultValue: 50f);

        public float TitleSpacing
        {
            get { return (float)GetValue(TitleSpacingProperty); }
            set { SetValue(TitleSpacingProperty, value); }
        }

        #endregion

        #region HeaderHeight

        public static readonly BindableProperty HeaderHeightProperty = BindableProperty.Create(nameof(HeaderHeight), typeof(float), typeof(HeaderTabView),
            defaultValue: 50f, propertyChanged: OnHeaderHeightChanged);

        public float HeaderHeight
        {
            get { return (float)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        #endregion

        #endregion

        #region Abstract API implementation

        protected override void SetupTitles()
        {
            ClearTitles();
            if (Tabs?.Any() != true)
                return;
            foreach (var tabItem in Tabs)
            {
                var button = CreateTitle(tabItem);
                _titleContainer.Children.Add(button);
            }
        }

        protected override void SelectTitle(ITabItem tab)
        {
            UpdateTabLayout(tab);
        }

        protected override void InsertTitle(ITabItem tab, int index)
        {
            var title = CreateTitle(tab);
            _titleContainer.Children.Insert(index, title);
            UpdateTabLayout(CurrentTab);
        }

        protected override void RemoveTitle(ITabItem tab, int index)
        {
            if (index >= 0)
                _titleContainer.Children.RemoveAt(index);
            UpdateTabLayout(CurrentTab);
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
            if (Content == _contentContainer.Content)
                _contentContainer.Content = null;
        }

        #endregion

        #region Utility methods

        private static void TitleSpacingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = (HeaderTabView)bindable;
            tabView._titleContainer.Spacing = tabView.TitleSpacing;
            tabView.UpdateTabLayout(tabView.CurrentTab);
        }

        private static void OnHeaderHeightChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var tabView = (HeaderTabView)bindable;
            tabView._header.HeightRequest = tabView.HeaderHeight;
        }

        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            UpdateTabLayout(CurrentTab);
        }

        private void UpdateTabLayout(ITabItem tab)
        {
            if (Tabs?.Any() != true || tab == null)
                return;
            var index = Tabs.IndexOf(tab);
            if (index < 0)
                return;
            var title = _titleContainer.Children[index];
            var rect = title.Bounds;
            var x = _header.Width / 2 - rect.X - rect.Width / 2;
            var y = _header.Height / 2 - _titleContainer.Height / 2;
            var frame = new Rectangle(x, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize);
            AbsoluteLayout.SetLayoutBounds(_titleContainer, frame);
        }

        private ViewButton CreateTitle(ITabItem tabItem)
        {
            View title = null;
            var selector = TitleTemplate as DataTemplateSelector;
            if (selector != null)
            {
                var template = selector.SelectTemplate(tabItem, this);
                title = template?.CreateContent() as View;
            }
            else
            {
                title = (View)TitleTemplate.CreateContent();
            }
            if (title == null)
                throw new InvalidOperationException(
                    "Could not instantiate title. Please make sure that your TitleTemplate is configured correctly.");

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
            var index = _titleContainer.Children.IndexOf((ViewButton)sender);
            CurrentTab = Tabs.ElementAt(index);
        }

        #endregion
    }
}

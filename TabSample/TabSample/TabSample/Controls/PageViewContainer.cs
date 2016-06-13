using System;
using Xamarin.Forms;

namespace TabSample.Controls
{
    //specialized class for showing a page within a page
    [ContentProperty("Content")]
    public class PageViewContainer : View
    {
        #region Constructors

        public PageViewContainer()
        {
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (Content != null)
                Content.BindingContext = BindingContext;
        }

        #endregion


        #region Properties

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(nameof(Content), typeof (Page),
            typeof (PageViewContainer), defaultValue: null,
            propertyChanged: PageChanged);

        public Page Content
        {
            get { return (Page) GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        #endregion


        #region Utility methods

        private static void PageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var container = (PageViewContainer) bindable;
            var page = (Page) newValue;
            page.BindingContext = container.BindingContext;
        }

        #endregion
    }
}
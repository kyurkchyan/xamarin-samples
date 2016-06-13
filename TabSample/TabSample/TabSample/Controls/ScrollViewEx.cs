using System;
using Xamarin.Forms;

namespace TabSample.Controls
{
    public class ScrollViewEx : ScrollView
    {

        #region ShowHorizontalScrollIndicator

        public static readonly BindableProperty ShowHorizontalScrollIndicatorProperty =
            BindableProperty.Create(nameof(ShowHorizontalScrollIndicator), typeof(bool), typeof(ScrollViewEx), defaultValue: true);

        public bool ShowHorizontalScrollIndicator
        {
            get { return (bool)GetValue(ShowHorizontalScrollIndicatorProperty); }
            set { SetValue(ShowHorizontalScrollIndicatorProperty, value); }
        }

        #endregion


        #region ShowVerticalScrollIndicator

        public static readonly BindableProperty ShowVerticalScrollIndicatorProperty =
            BindableProperty.Create(nameof(ShowVerticalScrollIndicator), typeof(bool), typeof(ScrollViewEx), defaultValue: true);

        public bool ShowVerticalScrollIndicator
        {
            get { return (bool)GetValue(ShowVerticalScrollIndicatorProperty); }
            set { SetValue(ShowVerticalScrollIndicatorProperty, value); }
        }

        #endregion

        #region BounceEnabled

        public static readonly BindableProperty BounceEnabledProperty = BindableProperty.Create(nameof(BounceEnabled), typeof(bool), typeof(ScrollViewEx), defaultValue: true);

        public bool BounceEnabled
        {
            get { return (bool)GetValue(BounceEnabledProperty); }
            set { SetValue(BounceEnabledProperty, value); }
        }

        #endregion
    }
}

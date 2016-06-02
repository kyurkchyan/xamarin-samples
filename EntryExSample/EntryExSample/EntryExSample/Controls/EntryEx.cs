using Xamarin.Forms;

namespace EntryExSample.Controls
{
    public class EntryEx : Entry
    {
        #region Constructors

        public EntryEx()
            : base()
        {

        }

        #endregion

        #region Properties

        public static BindableProperty BorderColorProperty = BindableProperty.Create<EntryEx, Color>(o => o.BorderColor, Color.Transparent);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }


        public static BindableProperty BorderWidthProperty = BindableProperty.Create<EntryEx, float>(o => o.BorderWidth, 0);

        public float BorderWidth
        {
            get { return (float)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }

        public static BindableProperty BorderRadiusProperty = BindableProperty.Create<EntryEx, float>(o => o.BorderRadius, 0);

        public float BorderRadius
        {
            get { return (float)GetValue(BorderRadiusProperty); }
            set { SetValue(BorderRadiusProperty, value); }
        }

        public static BindableProperty LeftPaddingProperty = BindableProperty.Create<EntryEx, int>(o => o.LeftPadding, 5);

        public int LeftPadding
        {
            get { return (int) GetValue(LeftPaddingProperty); }
            set { SetValue(LeftPaddingProperty, value); }
        }

        public static BindableProperty RightPaddingProperty = BindableProperty.Create < EntryEx, int>(o => o.RightPadding, 5);

        public int RightPadding
        {
            get { return (int) GetValue(RightPaddingProperty); }
            set { SetValue(RightPaddingProperty, value); }
        }

        #endregion
    }
}

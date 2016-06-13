using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace TabSample.Controls
{
    public class ViewButton : ContentView
    {
        #region Constructors

        public ViewButton()
        {
            BackgroundColor = Color.Transparent;
        }

        #endregion

        #region Properties

        public event EventHandler<EventArgs> Clicked;

        #endregion

        #region Public API

        public void InvokeClicked(object sender, EventArgs args)
        {
            Clicked?.Invoke(sender, args);
        }

        #endregion

        #region Commands

        public static BindableProperty CommandProperty = BindableProperty.Create<ViewButton, ICommand>(o => o.Command, null);

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion
    }
}

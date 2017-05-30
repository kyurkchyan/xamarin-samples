using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfiniteChatSample.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InfiniteChatSample
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        private readonly ChatStreamViewModel _viewModel;

        public ChatPage()
        {
            InitializeComponent();
            _viewModel = new ChatStreamViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.Load();
        }
    }
}

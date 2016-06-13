using TabSample.Controls;

namespace TabSample.ViewModels
{
	public class CarViewModel : BaseViewModel, ITabItem
	{
        public CarViewModel(string name, string imageUrl)
        {
            Name = name;
            ImageUrl = imageUrl;
        }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public static string IsSelectedProperty = "IsSelected";
        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }
    }
}


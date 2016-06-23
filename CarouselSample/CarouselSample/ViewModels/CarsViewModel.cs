using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarouselSample.Toolbox;

namespace CarouselSample.ViewModels
{
    public class CarsViewModel : BaseViewModel
    {
        private List<CarViewModel> _items;

        public CarsViewModel()
        {
            var imageUrls = new[] {
                "http://media.caranddriver.com/images/media/51/2016-10best-cars-lead-photo-664005-s-original.jpg",
                "http://i2.cdn.turner.com/cnnnext/dam/assets/150918170501-frankfurt-motor-show-concept-cars-4-super-169.jpg",
                "https://www.enterprise.com/content/dam/global-vehicle-images/cars/FORD_FOCU_2012-1.png",
                "https://media.ed.edmunds-media.com/honda/civic/2016/oem/2016_honda_civic_sedan_touring_fq_oem_4_717.jpg",
                "https://www.enterprise.ca/content/dam/global-vehicle-images/cars/CHRY_200_2015.png",
                "http://pop.h-cdn.co/assets/cm/15/05/54cb1d27a519c_-_analog-sports-cars-01-1013-de.jpg"
            };
            int i = 0;
            _items = imageUrls.Select(url => new CarViewModel("Car " + ++i, url)).ToList();
            
            Test();
        }

        public static string CarsProperty = "Cars";
        private ObservableCollection<CarViewModel> _cars;
        public ObservableCollection<CarViewModel> Cars
        { 
            get  
            {
                return _cars; 
            }
            set 
            {
                _cars = value; 
                RaisePropertyChanged(() => Cars); 
            }
        }  

        public static string SelectedItemProperty = "SelectedItem";
        private CarViewModel _selectedItem;

        public CarViewModel SelectedItem
        { 
            get  
            {
                return _selectedItem; 
            }
            set 
            {
                _selectedItem = value;
                RaisePropertyChanged(() => SelectedItem); 
            }
        }

        public static string TestActionProperty = "TestAction";
        private string _testAction;
        public string TestAction
        { 
            get  
            {
                return _testAction; 
            }
            set 
            {
                _testAction = value; 
                RaisePropertyChanged(() => TestAction); 
            }
        }  

        private async void Test()
        {
            TestAction = "Testing collection loading.";
            await Task.Delay(2000);

            var cars = new ObservableCollection<CarViewModel>();
            cars.AddRange(_items);
            Cars = cars;
            SelectedItem = Cars.FirstOrDefault();

            TestAction = "Testing item removing at index 2.";
            await Task.Delay(2000);
            Cars.RemoveAt(2);

            TestAction = "Testing item inserting at index 2.";
            await Task.Delay(2000);
            Cars.Insert(2, _items[2]);

            TestAction = "Testing item appending at end";
            await Task.Delay(5000);
            Cars.Add(new CarViewModel("Car 6 added", _items[2].ImageUrl));

            TestAction = "Testing item replacing at index 0";
            await Task.Delay(5000);
            Cars[0] = new CarViewModel("Car 0 replaced", _items.Last().ImageUrl);

            TestAction = "Testing completed";
        }
    }
}

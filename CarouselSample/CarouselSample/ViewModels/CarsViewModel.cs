using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarouselSample.ViewModels
{
    public class CarsViewModel : BaseViewModel
    {
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
            Cars = imageUrls.Select(url => new CarViewModel("Car " + ++i, url)).ToList();
            SelectedItem = Cars.FirstOrDefault();
        }

        public static string CarsProperty = "Cars";
        private List<CarViewModel> _cars;
        public List<CarViewModel> Cars
        { 
            get  
            {
                return _cars; 
            }
            set 
            {
                _cars = value; 
                OnPropertyChanged(() => Cars); 
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
                OnPropertyChanged(() => SelectedItem); 
            }
        }  
    }
}

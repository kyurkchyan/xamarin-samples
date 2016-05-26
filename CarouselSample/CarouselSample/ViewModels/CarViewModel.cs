using System;
namespace CarouselSample.ViewModels
{
	public class CarViewModel
	{
        public CarViewModel(string name, string imageUrl)
        {
            Name = name;
            ImageUrl = imageUrl;
        }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
	}
}


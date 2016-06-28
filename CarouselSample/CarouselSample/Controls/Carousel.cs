using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarouselSample.Views;
using Xamarin.Forms;

namespace CarouselSample.Controls
{
    public class Carousel : View
    {
        #region Constructors

        public Carousel()
        {
        }

        #endregion

        #region Properties

        #region ItemTemplate

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(nameof(ItemTemplate),
            typeof(DataTemplate), typeof(Carousel),
            defaultValue: null);

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        #endregion

        #region Items

        public static readonly BindableProperty ItemsProperty = BindableProperty.Create(nameof(Items), typeof(IList),
            typeof(Carousel),
            defaultValue: default(IList));

        public IList Items
        {
            get { return (IList)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        #endregion

        #region Current

        public static readonly BindableProperty CurrentProperty = BindableProperty.Create(nameof(Current), typeof(object), typeof(Carousel),
            defaultValue: null, defaultBindingMode: BindingMode.TwoWay);

        public object Current
        {
            get { return (object)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        #endregion


        #endregion
    }
}

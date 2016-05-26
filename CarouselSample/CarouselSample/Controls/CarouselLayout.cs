using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace CarouselSample.Controls
{
    public class CarouselLayout : ScrollView
    {
        public enum IndicatorStyleEnum
        {
            None,
            Dots,
            Tabs
        }

        #region Private fields and properties

        private readonly StackLayout _stack;
        private Timer _timer;
        private int _selectedIndex;
        private bool _layingOutChildren;

        #endregion

        #region Constructors

        public CarouselLayout()
        {
            Orientation = ScrollOrientation.Horizontal;

            _stack = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Spacing = 0
            };

            Content = _stack;
        }

        #endregion

        #region Public Properties

        public IndicatorStyleEnum IndicatorStyle { get; set; }

        public IList<View> Children
        {
            get { return _stack.Children; }
        }

        public DataTemplate ItemTemplate { get; set; }

        #region Selected Index

        public static readonly BindableProperty SelectedIndexProperty =
            BindableProperty.Create<CarouselLayout, int>(
                carousel => carousel.SelectedIndex,
                0,
                BindingMode.TwoWay,
                propertyChanged: (bindable, oldValue, newValue) => { ((CarouselLayout) bindable).UpdateSelectedItem(); }
                );

        public int SelectedIndex
        {
            get { return (int) GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        #endregion

        #region Selected item

        public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create<CarouselLayout, object>(
                view => view.SelectedItem,
                null,
                BindingMode.TwoWay,
                propertyChanged:
                    (bindable, oldValue, newValue) => { ((CarouselLayout) bindable).UpdateSelectedIndex(); }
                );

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        #endregion

        #region Items source

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create<CarouselLayout, IList>(
                view => view.ItemsSource,
                null,
                propertyChanging:
                    (bindableObject, oldValue, newValue) =>
                    {
                        ((CarouselLayout) bindableObject).ItemsSourceChanging();
                    },
                propertyChanged:
                    (bindableObject, oldValue, newValue) =>
                    {
                        ((CarouselLayout) bindableObject).ItemsSourceChanged();
                    }
                );

        public IList ItemsSource
        {
            get { return (IList) GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        #endregion

        #endregion

        #region Parent override

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            base.LayoutChildren(x, y, width, height);
            if (_layingOutChildren)
                return;

            _layingOutChildren = true;
            foreach (var child in Children)
                child.WidthRequest = width;
            _layingOutChildren = false;
        }

        #endregion

        #region Utility methods

        void UpdateSelectedItem()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            _timer = new Timer(state =>
            {
                _timer.Dispose();
                _timer = null;
                SelectedItem = SelectedIndex > -1 ? Children[SelectedIndex].BindingContext : null;
            }, null, 300);
        }

        void ItemsSourceChanging()
        {
            if (ItemsSource == null)
                return;
            _selectedIndex = ItemsSource.IndexOf(SelectedItem);
        }

        void ItemsSourceChanged()
        {
            _stack?.Children?.Clear();
            if(ItemsSource == null)
                return;

            foreach (var item in ItemsSource)
            {
                var view = (View) ItemTemplate.CreateContent();
                var bindableObject = view as BindableObject;
                if (bindableObject != null)
                    bindableObject.BindingContext = item;
                _stack.Children.Add(view);
            }

            if (_selectedIndex >= 0)
                SelectedIndex = _selectedIndex;
        }


        void UpdateSelectedIndex()
        {
            if (SelectedItem == BindingContext)
                return;
            if(Children == null)
                return;
            
            SelectedIndex = Children
                .Select(c => c.BindingContext)
                .ToList()
                .IndexOf(SelectedItem);
        }

        #endregion
    }
}
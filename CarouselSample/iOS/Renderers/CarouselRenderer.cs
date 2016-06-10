using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Carousels;
using CarouselSample.Controls;
using CarouselSample.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Carousel), typeof(CarouselRenderer))]
namespace CarouselSample.iOS.Renderers
{
    public class CarouselRenderer : ViewRenderer<Carousel, iCarousel>
    {
        #region Private fields

        private nfloat _previousWidth;
        private nfloat _previousHeight;
        private ObservedCollection _observedCollection;

        #endregion

        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<Carousel> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                var carousel = new iCarousel(e.NewElement.Bounds.ToRectangleF());
                carousel.Type = iCarouselType.Linear;
                carousel.PagingEnabled = true;
                carousel.DataSource = new CarouselDataSource(e.NewElement);
                carousel.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
                carousel.CurrentItemIndexChanged += OnCurrentItemIndexChanged;
                SetNativeControl(carousel);
            }

            if (e.OldElement != null)
            {
                Control.CurrentItemIndexChanged -= OnCurrentItemIndexChanged;
                DisconnectCollectionEvents();
            }

            if (e.NewElement != null)
            {
                SetupCollection();
                UpdateItems();
                UpdateCurrent();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if(Element == null || Control == null)
                return;
            if (string.IsNullOrEmpty(e.PropertyName) ||
                e.PropertyName == Carousel.ItemsProperty.PropertyName)
            {
                SetupCollection();
                UpdateItems();
            }
            else if (e.PropertyName == Carousel.ItemsProperty.PropertyName ||
                e.PropertyName == Carousel.MaxWidthProperty.PropertyName ||
                e.PropertyName == Carousel.ItemTemplateProperty.PropertyName)
            {
                UpdateItems();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (_previousWidth != Control.Bounds.Width ||
                _previousHeight != Control.Bounds.Height)
            {
                Control.ReloadData();
                _previousWidth = Control.Bounds.Width;
                _previousHeight = Control.Bounds.Height;
            }
        }

        #endregion

        #region Utility methods

        private void SetupCollection()
        {
            DisconnectCollectionEvents();

            var items = Element.Items as INotifyCollectionChanged;
            if (items == null) return;
           
            _observedCollection = new ObservedCollection(items);
            ConnectCollectionEvents();
        }

        private void UpdateItems()
        {
            Control.ReloadData();
        }

        private void UpdateCurrent()
        {
            if (Element.Current != null && Element.Items != null)
            {
                var index = Element.Items.IndexOf(Element.Current);
                if(index >= 0)
                    Control.ScrollToItemAt(index, false);
            }
        }

        private void OnCurrentItemIndexChanged(object sender, EventArgs eventArgs)
        {
            if(!(Element?.Items?.Count > 0))
                return;

            Element.Current = Element.Items[(int)Control.CurrentItemIndex];
        }

        #region Observed collection

        private void ConnectCollectionEvents()
        {
            if (_observedCollection == null) return;

            _observedCollection.OnCleared += CollectionOnOnCleared;
            _observedCollection.OnItemAdded += CollectionOnOnItemAdded;
            _observedCollection.OnItemMoved += CollectionOnOnItemMoved;
            _observedCollection.OnItemRemoved += CollectionOnOnItemRemoved;
            _observedCollection.OnItemReplaced += CollectionOnOnItemReplaced;
        }

        private void DisconnectCollectionEvents()
        {
            if (_observedCollection == null) return;

            _observedCollection.OnCleared -= CollectionOnOnCleared;
            _observedCollection.OnItemAdded -= CollectionOnOnItemAdded;
            _observedCollection.OnItemMoved -= CollectionOnOnItemMoved;
            _observedCollection.OnItemRemoved -= CollectionOnOnItemRemoved;
            _observedCollection.OnItemReplaced -= CollectionOnOnItemReplaced;
        }

        private void CollectionOnOnItemReplaced(INotifyCollectionChanged aSender, int aIndex, object aOldItem, object aNewItem)
        {
            Control.ReloadItemAt(aIndex, true);
        }

        private void CollectionOnOnItemRemoved(INotifyCollectionChanged aSender, int aIndex, object aItem)
        {
            Control.RemoveItemAt(aIndex, true);
        }

        private void CollectionOnOnItemMoved(INotifyCollectionChanged aSender, int aOldIndex, int aNewIndex, object aItem)
        {
            Control.RemoveItemAt(aOldIndex, true);
            Control.InsertItemAt(aNewIndex, true);
        }

        private void CollectionOnOnItemAdded(INotifyCollectionChanged aSender, int aIndex, object aItem)
        {
            Control.InsertItemAt(aIndex, true);
        }

        private void CollectionOnOnCleared(INotifyCollectionChanged aSender)
        {
            Control.ReloadData();
        }

        #endregion

        #endregion
    }
}

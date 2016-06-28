using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using CarouselSample.Controls;
using CarouselSample.iOS.Renderers;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Carousel), typeof(CarouselRenderer))]
namespace CarouselSample.iOS.Renderers
{
    public class CarouselRenderer : ViewRenderer<Carousel, CarouselCollectionView>
    {
        #region Private fields

        private nfloat _previousWidth;
        private nfloat _previousHeight;
        private ObservedCollection _observedCollection;
        private CarouselSource _source;

        #endregion

        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<Carousel> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                var carousel = new CarouselCollectionView(e.NewElement.Bounds.ToRectangleF())
                {
                    AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight
                };
                carousel.Source = _source = new CarouselSource(e.NewElement, carousel);

                SetNativeControl(carousel);
            }

            if (e.OldElement != null && Control != null)
            {
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
            if (Element == null || Control == null)
                return;
            if (string.IsNullOrEmpty(e.PropertyName) ||
                e.PropertyName == Carousel.ItemsProperty.PropertyName)
            {
                SetupCollection();
                UpdateItems();
                UpdateCurrent();
            }
            else if (e.PropertyName == Carousel.ItemTemplateProperty.PropertyName)
            {
                UpdateItems();
            }
            else if (e.PropertyName == Carousel.CurrentProperty.PropertyName)
            {
                UpdateCurrent();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            UpdateFrame();
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
            if (Element.Current == null)
                return;
            var index = Element?.Items?.IndexOf(Element.Current);
            if (index >= 0 && index != _source.CurrentIndex)
                Control.ScrollToItem(NSIndexPath.FromRowSection(index.Value, 0), UICollectionViewScrollPosition.Left, false);
        }

        private void UpdateFrame()
        {
            if (_previousWidth != Control.Bounds.Width ||
                    _previousHeight != Control.Bounds.Height)
            {
                Control.CellSize = new CGSize(Control.Bounds.Width, Control.Bounds.Height);
                _previousWidth = Control.Bounds.Width;
                _previousHeight = Control.Bounds.Height;
            }
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
            Control.ReloadItems(new[] { NSIndexPath.FromRowSection(aIndex, 0) });
            if (aOldItem == Element.Current)
                Element.Current = aNewItem;
        }

        private void CollectionOnOnItemRemoved(INotifyCollectionChanged aSender, int aIndex, object aItem)
        {
            Control.DeleteItems(new[] { NSIndexPath.FromRowSection(aIndex, 0) });
            if (aItem == Element.Current)
            {
                var count = Element.Items?.Count ?? 0;
                var index = Math.Min(aIndex, count - 1);
                if (index < count)
                    Element.Current = Element.Items[index];
            }
        }

        private void CollectionOnOnItemMoved(INotifyCollectionChanged aSender, int aOldIndex, int aNewIndex, object aItem)
        {
            Control.PerformBatchUpdates(() =>
            {
                Control.DeleteItems(new[] { NSIndexPath.FromRowSection(aOldIndex, 0) });
                Control.InsertItems(new[] { NSIndexPath.FromRowSection(aNewIndex, 0) });
            }, null);
            UpdateCurrent();
        }

        private void CollectionOnOnItemAdded(INotifyCollectionChanged aSender, int aIndex, object aItem)
        {
            Control.InsertItems(new[] { NSIndexPath.FromRowSection(aIndex, 0) });
        }

        private void CollectionOnOnCleared(INotifyCollectionChanged aSender)
        {
            UpdateItems();
        }

        #endregion

        #endregion
    }
}

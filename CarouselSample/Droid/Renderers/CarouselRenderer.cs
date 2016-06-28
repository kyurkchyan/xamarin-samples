using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Android.Animation;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CarouselSample.Controls;
using CarouselSample.Droid.Renderers;
using CarouselSample.Droid.Toolbox;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Carousel), typeof(CarouselRenderer))]
namespace CarouselSample.Droid.Renderers
{
    public class CarouselRenderer : ViewRenderer<Carousel, SnappyRecyclerView>
    {
        #region Private fields

        private ObservedCollection _observedCollection;
        private CarouselAdapter _adapter;
        private SnappyLinearLayoutManager _layoutManager;

        #endregion

        #region Parent override

        protected override void OnElementChanged(ElementChangedEventArgs<Carousel> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                var element = e.NewElement;
                CreateCarousel(element);
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
                UpdateAdapter();
            }
            else if (e.PropertyName == Carousel.CurrentProperty.PropertyName && sender != this)
            {
                UpdateCurrent();
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            if (oldw != w || oldh != h)
            {
                UpdateAdapter();
            }
        }

        #endregion

        #region Utility methods

        private void CreateCarousel(Carousel element)
        {
            //Create carousel
            var carousel = new SnappyRecyclerView(Xamarin.Forms.Forms.Context, SelectItemAction);

            //Setup layout options
            var frame = element.Bounds;
            var width = (int)Context.ToPixels(frame.Width);
            var height = (int)Context.ToPixels(frame.Height);
            carousel.LayoutParameters = new LayoutParams(width, height);

            //Set the custom adapter
            _adapter = new CarouselAdapter(element);
            carousel.SetAdapter(_adapter);

            //Set the custom layout manager
            _layoutManager = new SnappyLinearLayoutManager(Context);
            carousel.SetLayoutManager(_layoutManager);

            SetNativeControl(carousel);
        }

        private void SelectItemAction(int index)
        {
            if (!(index >= 0 && index < Element?.Items?.Count))
                return;
            var item = Element.Items[index];
            Element.Current = item;
            UpdateCurrent();
        }

        private void SetupCollection()
        {
            DisconnectCollectionEvents();

            var items = Element.Items as INotifyCollectionChanged;
            if (items == null) return;
            _observedCollection = new ObservedCollection(items);
            ConnectCollectionEvents();
        }

        private void UpdateAdapter()
        {
            Context.InvokeOnMainThread(() =>
            {
                _adapter = new CarouselAdapter(Element);
                Control.SetAdapter(_adapter);
                UpdateCurrent(false);
            });
        }


        private void UpdateItems()
        {
            _adapter.NotifyDataSetChanged();
        }

        private void UpdateCurrent(bool animated = true)
        {
            if(Element.Current == null)
                return;
            var index = Element?.Items?.IndexOf(Element.Current);
            if (index >= 0)
                ScrollToItemAt(index.Value, animated);
        }

        private void ScrollToItemAt(int index, bool animated = true)
        {
            if (!(index >= 0 && index < (Element?.Items?.Count ?? -1)))
                return;

            Context.InvokeOnMainThread(() =>
            {
                if (animated)
                {
                    Control.SmoothScrollToPosition(index);
                }
                else
                {
                    _layoutManager.ScrollToPosition(index);
                }
            });
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
            _adapter.NotifyItemChanged(aIndex);
            if (aOldItem == Element.Current)
                Element.Current = aNewItem;
        }

        private void CollectionOnOnItemRemoved(INotifyCollectionChanged aSender, int aIndex, object aItem)
        {
            _adapter.NotifyItemRemoved(aIndex);
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
            _adapter.NotifyItemRemoved(aOldIndex);
            _adapter.NotifyItemInserted(aNewIndex);
            UpdateCurrent(false);
        }

        private void CollectionOnOnItemAdded(INotifyCollectionChanged aSender, int aIndex, object aItem)
        {
            _adapter.NotifyItemInserted(aIndex);
        }

        private void CollectionOnOnCleared(INotifyCollectionChanged aSender)
        {
            _adapter.NotifyDataSetChanged();
        }

        #endregion

        #endregion
    }
}

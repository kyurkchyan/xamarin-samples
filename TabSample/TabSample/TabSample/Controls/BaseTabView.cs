using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabSample.Toolbox;
using Xamarin.Forms;

namespace TabSample.Controls
{
    public interface ITabItem
    {
        bool IsSelected { get; set; }
    }

    public abstract class BaseTabView : ContentView
    {
        #region Private fields

        private ObservedCollection _observedCollection;
        private Dictionary<ITabItem, View> _tabViews;

        #endregion

        #region Constructors

        public BaseTabView()
            : base()
        {
            
        }

        #endregion

        #region Properties

        #region Tabs

        public static readonly BindableProperty TabsProperty = BindableProperty.Create(nameof(Tabs), typeof (IEnumerable<ITabItem>), typeof (BaseTabView),
            propertyChanged:OnTabsChanged,
            defaultValue: default(IEnumerable<ITabItem>));

        public IEnumerable<ITabItem> Tabs
        {
            get { return (IEnumerable<ITabItem>) GetValue(TabsProperty); }
            set { SetValue(TabsProperty, value); }
        }

        #endregion

        #region CurrentTab

        public static readonly BindableProperty CurrentTabProperty = BindableProperty.Create(nameof(CurrentTab), typeof (ITabItem), typeof (BaseTabView),
            propertyChanged:OnCurrentTabChanged,
            defaultValue: default(ITabItem));

        public ITabItem CurrentTab
        {
            get { return (ITabItem) GetValue(CurrentTabProperty); }
            set { SetValue(CurrentTabProperty, value); }
        } 

        #endregion

        #region ContentTemplate

        public static readonly BindableProperty ContentTemplateProperty = BindableProperty.Create(nameof(ContentTemplate), typeof (DataTemplate), typeof (BaseTabView),
            propertyChanged:OnContentTemplateChanged,
            defaultValue: null);

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate) GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        #endregion

        #region TitleTemplate

        public static readonly BindableProperty TitleTemplateProperty = BindableProperty.Create(nameof(TitleTemplate), typeof(DataTemplate), typeof(BaseTabView),
            propertyChanged: OnTitleTemplateChanged,
            defaultValue: null);

        public DataTemplate TitleTemplate
        {
            get { return (DataTemplate)GetValue(TitleTemplateProperty); }
            set { SetValue(TitleTemplateProperty, value); }
        }

        #endregion

        #endregion


        #region Protected API

        #region Titles

        protected abstract void SetupTitles();
        protected abstract void SelectTitle(ITabItem tab);
        protected abstract void InsertTitle(ITabItem tab, int index);
        protected abstract void RemoveTitle(ITabItem tab, int index);

        #endregion


        #region Content

        protected abstract void SetupContent();
        protected abstract void SelectContent(ITabItem tab, View content);
        protected abstract void RemoveContent(ITabItem tab, View content, int index);

        #endregion


        #endregion

        #region Utility methods

        private static void OnTabsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = (BaseTabView) bindable;
            tabView.Initialize();
        }

        private static void OnCurrentTabChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = (BaseTabView)bindable;
            if (oldValue != null)
            {
                var oldTab = (ITabItem) oldValue;
                oldTab.IsSelected = false;
            }

            if(tabView.CurrentTab != null)
                tabView.CurrentTab.IsSelected = true;

            tabView.SelectTab(tabView.CurrentTab);
        }

        private static void OnContentTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = (BaseTabView)bindable;
            
            tabView._tabViews = new Dictionary<ITabItem, View>();
            tabView.SetupContent();

            var current = tabView.CurrentTab ?? tabView.Tabs?.FirstOrDefault();
            tabView.SelectTab(current);
        }

        private static void OnTitleTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var tabView = (BaseTabView)bindable;
            tabView.SetupTitles();
            var current = tabView.CurrentTab ?? tabView.Tabs?.FirstOrDefault();
            tabView.SelectTitle(current);
        }

        private void Initialize()
        {
            _tabViews = new Dictionary<ITabItem, View>();
            SetupCollection();
            SetupContent();
            SetupTitles();
            if (Tabs?.Any() == true)
                CurrentTab = Tabs.First();
        }

        private void SetupCollection()
        {
            DisconnectCollectionEvents();
            _observedCollection = null;

            var items = Tabs as INotifyCollectionChanged;
            if (items == null) return;

            _observedCollection = new ObservedCollection(items);
            ConnectCollectionEvents();
        }

        private View GetOrCreateConent(ITabItem tab)
        {
            if (!(Tabs?.Any() == true && ContentTemplate != null))
                return null;
            View element = null;
            if (!(_tabViews.TryGetValue(tab, out element)))
            {
                var selector = ContentTemplate as DataTemplateSelector;
                if (selector != null)
                {
                    var template = selector.SelectTemplate(tab, this);
                    element = template?.CreateContent() as View;
                }
                else
                {
                    element = (View)ContentTemplate.CreateContent();
                }
                if (element == null)
                    throw new InvalidOperationException(
                        "Could not instantiate content. Please make sure that your ContentTemplate is configured correctly.");
                _tabViews[tab] = element;
                element.BindingContext = tab;
            }
            return element;
        }

        private void SelectTab(ITabItem tab)
        {
            if (tab != null)
            {
                SelectTitle(tab);
                SelectContent(tab);
            }
        }

        protected virtual void SelectContent(ITabItem tab)
        {
            var content = GetOrCreateConent(tab);
            SelectContent(tab, content);
        }

        private void InsertTab(ITabItem tab, int index)
        {
            InsertTitle(tab, index);
            if (tab.IsSelected)
                CurrentTab = Tabs?.FirstOrDefault();
        }

        private void RemoveTab(ITabItem tab, int index)
        {
            RemoveTitle(tab, index);
            View content = null;
            if (_tabViews.TryGetValue(tab, out content))
            {
                RemoveContent(tab, content, index);
                _tabViews.Remove(tab);
            }
            if (tab.IsSelected)
                CurrentTab = Tabs?.FirstOrDefault();
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
            var oldTab = (ITabItem) aOldItem;
            var newTab = (ITabItem) aNewItem;
            RemoveTab(oldTab, aIndex);
            InsertTab(newTab, aIndex);
        }

        private void CollectionOnOnItemRemoved(INotifyCollectionChanged aSender, int aIndex, object aItem)
        {
            RemoveTab((ITabItem)aItem, aIndex);
        }

        private void CollectionOnOnItemMoved(INotifyCollectionChanged aSender, int aOldIndex, int aNewIndex, object aItem)
        {
            var tab = (ITabItem) aItem;
            RemoveTab(tab, aOldIndex);
            InsertTab(tab, aNewIndex);
        }

        private void CollectionOnOnItemAdded(INotifyCollectionChanged aSender, int aIndex, object aItem)
        {
            InsertTab((ITabItem)aItem, aIndex);
        }

        private void CollectionOnOnCleared(INotifyCollectionChanged aSender)
        {
            Initialize();
        }

        #endregion

        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Specialized;

namespace TabSample.Controls
{
    public class ObservedCollection
    {
        private readonly INotifyCollectionChanged _source;

        public ObservedCollection(INotifyCollectionChanged aSource)
        {
            _source = aSource;
            _source.CollectionChanged += Source_CollectionChanged;
        }

        public delegate void ObservedCollectionAddDelegate(INotifyCollectionChanged aSender, int aIndex, object aItem);

        public event ObservedCollectionAddDelegate OnItemAdded;

        public delegate void ObservedCollectionMoveDelegate(
            INotifyCollectionChanged aSender, int aOldIndex, int aNewIndex, object aItem);

        public event ObservedCollectionMoveDelegate OnItemMoved;

        public delegate void ObservedCollectionRemoveDelegate(INotifyCollectionChanged aSender, int aIndex, object aItem);

        public event ObservedCollectionRemoveDelegate OnItemRemoved;

        public delegate void ObservedCollectionReplaceDelegate(
            INotifyCollectionChanged aSender, int aIndex, object aOldItem, object aNewItem);

        public event ObservedCollectionReplaceDelegate OnItemReplaced;

        public delegate void ObservedCollectionResetDelegate(INotifyCollectionChanged aSender);

        public event ObservedCollectionResetDelegate OnCleared;

        protected void CheckOneOrNone(IList aList)
        {
            if (aList.Count > 1)
            {
                throw new Exception("Holy cow. Someone changed ObservableCollection - update ObservedCollection.");
            }
        }

        void Source_CollectionChanged(object aSender, NotifyCollectionChangedEventArgs aArgs)
        {
            switch (aArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (OnItemAdded != null)
                    {
                        CheckOneOrNone(aArgs.NewItems);
                        OnItemAdded(_source, aArgs.NewStartingIndex, (object) aArgs.NewItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Move:
                    if (OnItemMoved != null)
                    {
                        CheckOneOrNone(aArgs.NewItems);
                        OnItemMoved(_source, aArgs.OldStartingIndex, aArgs.NewStartingIndex, (object) aArgs.NewItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (OnItemRemoved != null)
                    {
                        CheckOneOrNone(aArgs.OldItems);
                        OnItemRemoved(_source, aArgs.OldStartingIndex, (object) aArgs.OldItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                    if (OnItemReplaced != null)
                    {
                        CheckOneOrNone(aArgs.NewItems);
                        OnItemReplaced(_source, aArgs.OldStartingIndex, (object) aArgs.OldItems[0], (object) aArgs.NewItems[0]);
                    }
                    break;

                case NotifyCollectionChangedAction.Reset:
                    if (OnCleared != null)
                    {
                        OnCleared(_source);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
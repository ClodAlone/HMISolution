using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using log4net;
using Utilities;

namespace AlarmWindow
{
    /// <summary>
    /// ref. https://stackoverflow.com/questions/13203795/binding-multiple-observablecollections-to-one-observablecollection
    /// </summary>
    internal class CompositeCollection<T> : ObservableCollection<T>
    {
        private readonly List<SafeObservableCollection<T>> _viewModelsCollection;
        private readonly object _syncLock;
        protected SemaphoreSlim Semaphore = new SemaphoreSlim(1);
        protected static ILog Log = LogManager.GetLogger(Properties.Resources.SessionName);

        public CompositeCollection(object syncLock)
        {
            _viewModelsCollection = new List<SafeObservableCollection<T>>();
            _syncLock = syncLock;
        }

        public void Add(SafeObservableCollection<T> viewModel)
        {
            AddRange(new[] { viewModel });
        }

        public void AddRange(IEnumerable<SafeObservableCollection<T>> viewModels)
        {
            lock (_syncLock)
            {
                _viewModelsCollection.AddRange(viewModels);
                foreach (var collection in viewModels)
                {
                    AddItems(collection);
                    collection.CollectionChanged += OnParentCollectionChanged;
                }
            }
        }

        protected virtual void AddItems(IEnumerable<T> items)
        {
            try
            {
                Semaphore.Wait();
                foreach (var me in items.ToList())
                    Add(me);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        protected virtual void RemoveItems(IEnumerable<T> items)
        {
            try
            {
                Semaphore.Wait();
                foreach (var me in items.ToList())
                    Remove(me);
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        protected override void ClearItems()
        {
            lock (_syncLock)
            {
                base.ClearItems();
                foreach (var collection in _viewModelsCollection)
                    collection.CollectionChanged -= OnParentCollectionChanged;
                _viewModelsCollection.Clear();
            }
        }

        private void OnParentCollectionChanged(object source, NotifyCollectionChangedEventArgs args)
        {
            lock (_syncLock)
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AddItems(args.NewItems.Cast<T>());
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        RemoveItems(args.OldItems.Cast<T>());
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        base.ClearItems();
                        foreach (var collection in _viewModelsCollection)
                            AddItems(collection);
                        break;
                }
            }
        }
    }
}
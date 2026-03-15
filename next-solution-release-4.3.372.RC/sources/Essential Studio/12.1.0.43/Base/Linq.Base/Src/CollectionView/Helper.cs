#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if !SILVERLIGHT
namespace Syncfusion.Windows.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;
    using Syncfusion.Linq;
    using System.Linq.Expressions;
    using System.Xml;

    /// <summary>
    /// ICollectionViewAdv helper extensions
    /// </summary>
    public static class CollectionViewHelper
    {
        public static IEnumerable ToTypedSource(this IEnumerable itemsSource)
        {
            return ToTypedSource(itemsSource, null);
        }

        /// <summary>
        /// Converts the IEnumerable data source as a strongly typed generic source.
        /// </summary>
        /// <param name="itemsSource">The items source.</param>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public static IEnumerable ToTypedSource(this IEnumerable itemsSource, Type sourceType)
        {
            if (itemsSource != null)
            {
                if (itemsSource.IsGenericType())
                {
                    return itemsSource;
                }
                else
                {
                    Type type = null;
                    if (sourceType != null)
                    {
                        type = sourceType;
                    }
                    else
                    {
                        var enumerator = itemsSource.GetEnumerator();
                        if (enumerator.MoveNext())
                        {
                            type = enumerator.Current.GetType();
                        }
                        else if (itemsSource is System.Xml.XmlElement)
                        {
                            IList source = new List<XmlNode>();
                            CreateXmlSourceListWrapper((itemsSource as XmlElement), ref source);
                            return source as IEnumerable;
                        }
                        else
                        {
                            return null;
                        }
                    }

                    if (type != null)
                    {

                        IList source = null;
                        if (itemsSource is IBindingList)
                        {
                            IBindingList bindingList = itemsSource as IBindingList;
                            CreateSourceListWrapper(itemsSource, type, ref source);
                            var listEvents = source as IListEvents;
                            listEvents.WireListChangedHandler(bindingList);
                        }
                        else if (itemsSource is IListSource)
                        {
                            var ilistsource = itemsSource as IListSource;
                            itemsSource = ilistsource.GetList();
                            CreateSourceListWrapper(itemsSource, type, ref source);
                        }
                        else if (itemsSource is IList)
                        {
                            CreateSourceListWrapper(itemsSource, type, ref source);
                            if (itemsSource is INotifyCollectionChanged)
                            {
                                var notifyCollectionChanged = itemsSource as INotifyCollectionChanged;
                                var listEvents = source as IListEvents;
                                listEvents.WireCollectionChangedHandler(notifyCollectionChanged);
                            }
                        }
                        else if (itemsSource is ICollectionView)
                        {
                            var collectionview = itemsSource as ICollectionView;
                            itemsSource = collectionview.SourceCollection;
                            CreateSourceListWrapper(itemsSource, type, ref source);
                            if (itemsSource is INotifyCollectionChanged)
                            {
                                var notifyCollectionChanged = itemsSource as INotifyCollectionChanged;
                                var listEvents = source as IListEvents;
                                listEvents.WireCollectionChangedHandler(notifyCollectionChanged);
                            }
                        }
                        else if (itemsSource is IEnumerable)
                        {
                            CreateSourceListWrapper(itemsSource, type, ref source);
                        }

                        return source as IEnumerable;

                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Determines whether [is generic type] [the specified items source].
        /// </summary>
        /// <param name="itemsSource">The items source.</param>
        /// <returns>
        /// 	<c>true</c> if [is generic type] [the specified items source]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsGenericType(this IEnumerable itemsSource)
        {
            Type type = FindGenericType(typeof(IEnumerable<>), itemsSource.GetType());
            if (type == null)
            {
                return false;
            }
            return true;
        }

        internal static Type FindGenericType(Type definition, Type type)
        {
            while ((type != null) && (type != typeof(object)))
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type type2 in type.GetInterfaces())
                    {
                        Type type3 = FindGenericType(definition, type2);
                        if (type3 != null)
                        {
                            return type3;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        private static void CreateSourceListWrapper(IEnumerable itemsSource, Type type, ref IList source)
        {
            var enumerator = itemsSource.GetEnumerator();
            source = CreateGeneric(typeof(SourceListWrapper<>), type);
            while (enumerator.MoveNext())
            {
                source.Add(enumerator.Current);
            }
        }

        private static void CreateXmlSourceListWrapper(XmlNode itemsSource, ref IList source)
        {
            XmlNode xmlData = itemsSource as XmlNode;
            if (xmlData != null)
            {
                source.Add(xmlData);
                CreateXmlSourceListWrapper(xmlData.NextSibling, ref source);
            }
        }

        private static IList CreateGeneric(Type generic, Type innerType, params object[] args)
        {
            System.Type specificType = generic.MakeGenericType(new System.Type[] { innerType });
            return (IList)Activator.CreateInstance(specificType, args);
        }
    }

    /// <summary>
    /// Exposes events for flat IBindingList or INotifyCollectionChanged implemented data sources. Used by SourceListWrapper 
    /// for wrapping up in a generic collection.
    /// </summary>
    public interface IListEvents
    {
        /// <summary>
        /// Occurs when collection is changed.
        /// </summary>
        event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Wires the collection changed handler.
        /// </summary>
        /// <param name="notifyCollectionChangedSource">The notify collection changed source.</param>
        void WireCollectionChangedHandler(INotifyCollectionChanged notifyCollectionChangedSource);

        /// <summary>
        /// Wires the list changed handler.
        /// </summary>
        /// <param name="listSource">The list source.</param>
        void WireListChangedHandler(IBindingList listSource);
    }

    /// <summary>
    /// A generic wrapper for normal IBindingList / INotifyCollectionChanged interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SourceListWrapper<T> : List<T>, IListEvents
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceListWrapper&lt;T&gt;"/> class.
        /// </summary>
        public SourceListWrapper()
        {
        }

        #region IListEvents Members

        /// <summary>
        /// Occurs when collection is changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private INotifyCollectionChanged notifyCollectionChangedSource;
        /// <summary>
        /// Wires the collection changed handler.
        /// </summary>
        /// <param name="notifyCollectionChangedSource">The notify collection changed source.</param>
        public void WireCollectionChangedHandler(INotifyCollectionChanged notifyCollectionChangedSource)
        {
            this.notifyCollectionChangedSource = notifyCollectionChangedSource;
            notifyCollectionChangedSource.CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollectionChanged);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.UpdateSourceListWrapper(sender, e);
                this.CollectionChanged(this, e);
            }
        }

        private IBindingList originalBindingList;
        /// <summary>
        /// Wires the list changed handler.
        /// </summary>
        /// <param name="listSource">The list source.</param>
        public void WireListChangedHandler(IBindingList listSource)
        {
            this.originalBindingList = listSource;
            listSource.ListChanged += new ListChangedEventHandler(this.OnListSourceListChanged);
        }

        private void OnListSourceListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyCollectionChangedEventArgs arg = null;
            List<T> newItems = new List<T>();
            IList source = null;
            if (e.ListChangedType != ListChangedType.Reset)
            {
                source = sender as IList;
            }
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    newItems.Add((T)source[e.NewIndex]);
                    arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems, e.NewIndex);
                    break;

                case ListChangedType.ItemDeleted:
                    //newItems.Add((T)source[e.NewIndex]);
                    arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, newItems, e.NewIndex);
                    break;

                case ListChangedType.ItemChanged:
                    //arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,(T)source[e.NewIndex],(T)source[e.OldIndex], e.NewIndex);
                    arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, (T)source[e.NewIndex], (T)source[e.NewIndex], e.NewIndex);
                    break;

                case ListChangedType.ItemMoved:
                    newItems.Add((T)source[e.NewIndex]);
                    arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItems, e.NewIndex, e.OldIndex);
                    break;

                case ListChangedType.Reset:
                    arg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
            }
            this.UpdateSourceListWrapper(sender, arg);
            if(this.CollectionChanged!=null)
            this.CollectionChanged(this, arg);
        }

        private void UpdateSourceListWrapper(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Move:
                    this.RemoveAt(e.OldStartingIndex);
                    this.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    this.RemoveAt(e.OldStartingIndex);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    this.RemoveAt(e.OldStartingIndex);
                    this.Insert(e.NewStartingIndex, (T)e.NewItems[0]);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Clear();
                    if (this.originalBindingList != null)
                    {
                        var enumerator = this.originalBindingList.GetEnumerator();
                        this.ResetItems(enumerator);
                    }
                    else if (this.notifyCollectionChangedSource != null)
                    {
                        var enumerable = this.notifyCollectionChangedSource as IEnumerable<T>;
                        if (enumerable != null)
                        {
                            var enumerator = enumerable.GetEnumerator();
                            this.ResetItems(enumerator);
                        }
                    }
                    break;
            }
        }

        private void ResetItems(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
            {
                this.Add((T)enumerator.Current);
            }
        }

    }
        #endregion
}
#endif
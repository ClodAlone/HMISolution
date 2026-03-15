// <copyright file="DataTemplatesDictionary.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a collection that is used for storing date/template pairs.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DataTemplatesDictionary : Hashtable, INotifyCollectionChanged
    {
        #region Class overrides

        /// <summary>
        /// Adds new entry to hash table
        /// </summary>
        /// <param name="key">String representation of date parameter.</param>
        /// <param name="value">The <see cref="DataTemplateItem"/> object.</param>
        /// <exception cref="NotSupportedException">The key must be valid a string representation
        /// of <see cref="DateTime"/> value.</exception>
        /// <exception cref="NotSupportedException">Collection is used for <see cref="DataTemplateItem"/> items only.</exception>
        /// <exception cref="ArgumentException">Template value of <see cref="DataTemplateItem"/> parameter cannot be null.</exception>
        public override void Add(object key, object value)
        {
            string strKey = key as string;

            if (null == strKey)
            {
                throw new NotSupportedException("The key must be valid a string representation of DateTime value.");
            }

            DataTemplateItem item = value as DataTemplateItem;

            if (null == item)
            {
                throw new NotSupportedException("Collection is used for DataTemplateItem items only.");
            }

            DateTime dtKey;

            if (DateTime.TryParse(strKey, out dtKey))
            {
                DataTemplate template = item.Template as DataTemplate;

                if (template == null)
                {
                    throw new ArgumentException("Template value must be of DataTemplate type only.");
                }

                item.Date = dtKey;
                base.Add(dtKey, item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
            else
            {
                throw new ArgumentException("The key must be valid a string representation of DateTime value.");
            }
        }

        /// <summary>
        /// Removes entry by the key.
        /// </summary>
        /// <param name="key">String representation of the date parameter.</param>
        public override void Remove(object key)
        {
            DataTemplateItem item = (DataTemplateItem)base[key];
            base.Remove(key);
            NotifyCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        }

        /// <summary>
        /// Removes all elements from the hash table
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            NotifyCollectionChanged(NotifyCollectionChangedAction.Reset, null);
        }

        /// <summary>
        /// Implements indexer logic.
        /// </summary>
        /// <param name="key">String representation of the date parameter.</param>
        /// <returns>
        /// The <see cref="System.Windows.DataTemplate"/> composition of
        /// the entry.
        /// </returns>
        public override object this[object key]
        {
            get
            {
                if (base.ContainsKey(key))
                {
                    DataTemplateItem entry = (DataTemplateItem)base[key];
                    return entry.Template;
                }

                return null;
            }

            set
            {
                DataTemplate template = value as DataTemplate;
                if (template != null)
                {
                    ((DataTemplateItem)base[key]).Template = template;
                    DataTemplateItem item = (DataTemplateItem)base[key];
                    NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
                }
            }
        }

        #endregion Class overrides

        #region INotifyCollectionChanged Members

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Invoked when the collection changes.
        /// </summary>
        /// <param name="action">Describes the action caused by the event.</param>
        /// <param name="item">The item affected by the change.</param>
        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, DataTemplateItem item)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item));
            }
        }

        #endregion INotifyCollectionChanged Members
    }
}
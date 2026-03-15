// <copyright file="StyleDictionary.cs" company="Syncfusion">
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
    /// Represents a collection that is used for storing date/style pairs.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class StylesDictionary : Hashtable, INotifyCollectionChanged
    {
        #region Class overrides

        /// <summary>
        /// Adds new entry to the hash table
        /// </summary>
        /// <param name="key">String representation of the date parameter.</param>
        /// <param name="value">The <see cref="StyleItem"/> object.</param>
        /// <exception cref="NotSupportedException">The key must be valid a string representation
        /// of <see cref="DateTime"/> value.</exception>
        /// <exception cref="NotSupportedException">Collection is used for <see cref="StyleItem"/> items only.</exception>
        /// <exception cref="ArgumentException">Style value of <see cref="StyleItem"/> parameter cannot be null.</exception>
        public override void Add(object key, object value)
        {
            string strKey = key as string;

            if (null == strKey)
            {
                throw new NotSupportedException("The key must be valid a string representation of DateTime value.");
            }

            StyleItem item = value as StyleItem;

            if (null == item)
            {
                throw new NotSupportedException("Collection is used for StyleItem items only.");
            }

            DateTime dtKey;

            if (DateTime.TryParse(strKey, out dtKey))
            {
                Style style = item.Style as Style;

                if (style == null)
                {
                    throw new ArgumentException("Style value must be of Style type only.");
                }

                item.Date = dtKey;
                base.Add(dtKey, item);
                NotifyCollectionChanged(NotifyCollectionChangedAction.Add, item);
            }
            else
            {
                throw new ArgumentException(" The key must be valid a string representation of DateTime value.");
            }
        }

        /// <summary>
        /// Removes entry by the key.
        /// </summary>
        /// <param name="key">String representation of the date parameter.</param>
        public override void Remove(object key)
        {
            StyleItem item = (StyleItem)base[key];
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
        /// The <see cref="System.Windows.Style"/> composition of the entry.
        /// </returns>
        public override object this[object key]
        {
            get
            {
                if (base.ContainsKey(key))
                {
                    StyleItem entry = (StyleItem)base[key];
                    return entry.Style;
                }

                return null;
            }

            set
            {
                Style style = value as Style;
                if (style != null)
                {
                    ((StyleItem)base[key]).Style = style;
                    StyleItem item = (StyleItem)base[key];
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
        private void NotifyCollectionChanged(NotifyCollectionChangedAction action, StyleItem item)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item));
            }
        }

        #endregion INotifyCollectionChanged Members
    }
}
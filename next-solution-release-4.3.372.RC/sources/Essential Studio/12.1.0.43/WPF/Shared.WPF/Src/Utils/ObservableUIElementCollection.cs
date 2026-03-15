// <copyright file="ObservableUIElementCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Collections.Specialized;
using System.Windows;
using System.Collections;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents an ordered collection of <see cref="UIElement"/> instances with implemented <see cref="INotifyCollectionChanged"/> interface.   
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ObservableUIElementCollection : UIElementCollection, INotifyCollectionChanged
    {
        #region Events

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableUIElementCollection"/> class.
        /// </summary>
        /// <param name="visualParent">The <see cref="T:System.Windows.UIElement"/> parent of the collection.</param>
        /// <param name="logicalParent">The logical parent of the elements in the collection.</param>
        public ObservableUIElementCollection(UIElement visualParent, FrameworkElement logicalParent)
            : base(visualParent, logicalParent)
        {
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Adds the specified element to the <see cref="T:System.Windows.Controls.UIElementCollection"/>.
        /// </summary>
        /// <param name="element">The <see cref="T:System.Windows.UIElement"/> to add.</param>
        /// <returns>The index position of the added element.</returns>
        public override int Add(UIElement element)
        {
            int result = base.Add(element);
            int index = IndexOf(element);
            UIElement[] array = { element };
            OnCollectionChanged(NotifyCollectionChangedAction.Add, array, null, index);
            return result;
        }

        /// <summary>
        /// Removes all elements from a <see cref="T:System.Windows.Controls.UIElementCollection"/>.
        /// </summary>
        public override void Clear()
        {
            IList list = this;
            base.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset, null, list, -1);
        }

        /// <summary>
        /// Inserts an element into a <see cref="T:System.Windows.Controls.UIElementCollection"/> at the specified index position.
        /// </summary>
        /// <param name="index">The index position where you want to insert the element.</param>
        /// <param name="element">The element to insert into the <see cref="T:System.Windows.Controls.UIElementCollection"/>.</param>
        public override void Insert(int index, UIElement element)
        {
            base.Insert(index, element);
            UIElement[] array = { element };
            OnCollectionChanged(NotifyCollectionChangedAction.Add, array, null, index);
        }

        /// <summary>
        /// Removes the specified element from a <see cref="T:System.Windows.Controls.UIElementCollection"/>.
        /// </summary>
        /// <param name="element">The element to remove from the collection.</param>
        public override void Remove(UIElement element)
        {
            base.Remove(element);
            int index = IndexOf(element);
            UIElement[] array = { element };
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, null, array, index);
        }

        /// <summary>
        /// Removes the <see cref="T:System.Windows.UIElement"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the <see cref="T:System.Windows.UIElement"/> that you want to remove.</param>
        public override void RemoveAt(int index)
        {
            UIElement[] array = { this[index] };
            base.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, null, array, index);
        }

        /// <summary>
        /// Removes a range of elements from a <see cref="T:System.Windows.Controls.UIElementCollection"/>.
        /// </summary>
        /// <param name="index">The index position of the element where removal begins.</param>
        /// <param name="count">The number of elements to remove.</param>
        public override void RemoveRange(int index, int count)
        {
            ArrayList array = new ArrayList();
            int cnt = index + count;

            if (Count >= cnt)
            {
                for (int i = index; i < cnt; ++i)
                {
                    array.Add(this[i]);
                }
            }

            base.RemoveRange(index, count);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, null, array, index);
        }
        #endregion

        #region Implementation

        /// <summary>
        /// Invoked when the collection was changed.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="added">The added.</param>
        /// <param name="removed">The removed.</param>
        /// <param name="index">The index.</param>
        private void OnCollectionChanged(
            NotifyCollectionChangedAction action,
            IList added,
            IList removed,
            int index)
        {
            if (this.CollectionChanged != null)
            {
                NotifyCollectionChangedEventArgs e = new NotifyCollectionChangedEventArgs(action, added, removed, index);
                CollectionChanged(this, e);
            }
        }

        #endregion
    }
}

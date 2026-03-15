#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Specifies the handler of <see cref="ChartToolBarButtonCollection.AddButton"/>
    /// and <see cref="ChartToolBarButtonCollection.RemoveButton"/> events.
    /// </summary>
    [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
    public delegate void ButtonCollectionEventHandler(object sender, Button button);

    /// <summary>
    /// Collection of <see cref="Button"/> instances.
    /// </summary>
    [Obsolete, EditorBrowsable(EditorBrowsableState.Never)]
    public class ChartToolBarButtonCollection : CollectionBase
    {
        #region Events
        /// <summary>
        /// Event will be raised when a button is added this collection.
        /// </summary>
        public event ButtonCollectionEventHandler AddButton;

        /// <summary>
        /// Event will be raised when a button is removed from this collection.
        /// </summary>
        public event ButtonCollectionEventHandler RemoveButton;

        /// <summary>
        /// Event will be raised when the collection is cleared.
        /// </summary>
        public event EventHandler ClearButtons;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the item with the specified index.
        /// </summary>
        public Button this[int index]
        {
            get
            {
                return List[index] as Button;
            }

            set
            {
                List[index] = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the ChartToolBarButtonCollection class.
        /// </summary>
        [Obsolete]
        public ChartToolBarButtonCollection()
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Removes the specified button from the collection.
        /// </summary>
        /// <param name="value">The Element to remove from the collection.</param>
        public void Remove(Button value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Indicates if this collection contains the specified button.
        /// </summary>
        /// <param name="value">The Element to locate in the Collections.</param>
        /// <returns>true if the Element is found in the Collection list; otherwise, false</returns>
        public bool Contains(Button value)
        {
            return List.Contains(value);
        }

        /// <summary>
        /// Returns the index of this button.
        /// </summary>
        /// <param name="value">The Element to locate in the Collections list.</param>
        /// <returns>The index of value if found in the list; otherwise, -1.</returns>
        public int IndexOf(Button value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        /// Adds this button in the collection.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The position into which the new element was inserted.</returns>
        public int Add(Button value)
        {
            return List.Add(value);
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"></see> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert value.</param>
        /// <param name="value">The new value of the element at index.</param>
        protected override void OnInsertComplete(int index, object value)
        {
            if (AddButton != null)
            {
                AddButton(this, value as Button);
            }

            base.OnInsertComplete(index, value);
        }

        /// <summary>
        /// Performs additional custom processes when removing an element from the <see cref="T:System.Collections.CollectionBase"></see> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which value can be found.</param>
        /// <param name="value">The value of the element to remove from index.</param>
        protected override void OnRemove(int index, object value)
        {
            if (RemoveButton != null)
            {
                RemoveButton(this, value as Button);
            }

            base.OnRemove(index, value);
        }

        /// <summary>
        /// Performs additional custom processes after clearing the contents of the <see cref="T:System.Collections.CollectionBase"></see> instance.
        /// </summary>
        protected override void OnClearComplete()
        {
            if (ClearButtons != null)
            {
                ClearButtons(this, new EventArgs());
            }

            base.OnClearComplete();
        }
        #endregion
    }
}

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

#region file using directives

using System;
using System.Collections;
using System.Diagnostics;

using Syncfusion.Documentation;

#endregion

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    ///		Collection of ChartStripLines. A strip line is a band that is drawn on the background of the chart, to highlight areas of interest.
    /// <seealso cref="ChartStripLine"/>
    /// </summary>
    [Serializable]
    public class ChartStripLineCollection : CollectionBase
    {
        #region Class properties
        /// <summary>
        /// Returns the strip line stored in the specified index.
        /// </summary>
        public ChartStripLine this[int index]
        {
            get
            {
                return this.List[index] as ChartStripLine;
            }
        }

        #endregion

        #region Class events

        /// <summary>
        ///     Event that is raised when this collection is changed.
        /// </summary>
        public event EventHandler Changed;

        #endregion

        #region Class Initialize/Finalize methods


        /// <summary>
        /// Initializes a new instance of the <see cref="ChartStripLineCollection"/> class.
        /// </summary>
        public ChartStripLineCollection()
        {
        }
        #endregion

        #region Class Public Methods
        /// <summary>
        ///     Looks up the collection and returns the index value of the specified strip line.
        /// </summary>
        /// <param name="stripLine" type="Syncfusion.Windows.Forms.Chart.ChartStripLine">
        ///     <para>
        ///     An instance of the strip line that is to be looked up in the collection.
        ///     </para>
        /// </param>
        /// <returns>
        ///     The index value if the look up was successful; -1 otherwise.
        /// </returns>
        public int IndexOf(ChartStripLine stripLine)
        {
            return InnerList.IndexOf(stripLine);
        }

        /// <summary>
        ///     Adds the specified strip line to this collection.
        /// </summary>
        /// <param name="stripLine" type="Syncfusion.Windows.Forms.Chart.ChartStripLine">
        ///     <para>
        ///     An instance of the strip line that is to be added to the collection.
        ///     </para>
        /// </param>
        public void Add(ChartStripLine stripLine)
        {
            this.List.Add(stripLine);
        }

        /// <summary>
        ///     Inserts the specified strip line at the specified index value.
        /// </summary>
        /// <param name="index" type="int">
        ///     <para>
        ///     Index value where the instance of the specified strip line is to be inserted.
        ///     </para>
        /// </param>
        /// <param name="stripLine" type="Syncfusion.Windows.Forms.Chart.ChartStripLine">
        ///     <para>
        ///    An instance of the stripline that is to be inserted at the specified index value.
        ///     </para>
        /// </param>
        public void Insert(int index, ChartStripLine stripLine)
        {
            this.List.Insert(index, stripLine);
        }

        /// <summary>
        ///     Removes the specified strip line from this collection.
        /// </summary>
        /// <param name="stripLine" type="Syncfusion.Windows.Forms.Chart.ChartStripLine">
        ///     <para>
        ///      Strip line that is to be removed.
        ///     </para>
        /// </param>
        public void Remove(ChartStripLine stripLine)
        {
            this.List.Remove(stripLine);
        }

        #endregion

        #region Class overrides
        /// <summary>
        /// Performs additional custom processes when clearing the contents of the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        protected override void OnClear()
        {
            foreach (ChartStripLine csl in this)
            {
                csl.Changed -= new EventHandler(OnStripLineChaged);
            }

            base.OnClear();
        }

        /// <summary>
        /// Performs additional custom processes after clearing the contents of the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        protected override void OnClearComplete()
        {
            BroadcastChange();
            base.OnClearComplete();
        }

        /// <summary>
        /// Performs additional custom processes before inserting a new element into the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">The new value of the element at <paramref name="index"/>.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        protected override void OnInsert(int index, object value)
        {
            base.OnInsert(index, value);
        }

        /// <summary>
        /// Performs additional custom processes after inserting a new element into the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert <paramref name="value"/>.</param>
        /// <param name="value">The new value of the element at <paramref name="index"/>.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        protected override void OnInsertComplete(int index, object value)
        {
            ChartStripLine stripLine = (ChartStripLine)value;
            stripLine.Changed += new EventHandler(OnStripLineChaged);
            BroadcastChange();
            base.OnInsertComplete(index, value);
        }

        /// <summary>
        /// Performs additional custom processes when removing an element from the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> can be found.</param>
        /// <param name="value">The value of the element to remove from <paramref name="index"/>.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        protected override void OnRemove(int index, object value)
        {
            ChartStripLine stripLine = (ChartStripLine)value;
            stripLine.Changed -= new EventHandler(OnStripLineChaged);
            base.OnRemove(index, value);
        }

        /// <summary>
        /// Performs additional custom processes after removing an element from the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="value"/> can be found.</param>
        /// <param name="value">The value of the element to remove from <paramref name="index"/>.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        protected override void OnRemoveComplete(int index, object value)
        {
            BroadcastChange();
            base.OnRemoveComplete(index, value);
        }

        /// <summary>
        /// Called when [set].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        /// <param name="newValue">The new value.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        protected override void OnSet(int index, object value, object newValue)
        {
            ChartStripLine stripLine = (ChartStripLine)value;
            stripLine.Changed -= new EventHandler(OnStripLineChaged);
            base.OnSet(index, value, newValue);
        }

        /// <summary>
        /// Performs additional custom processes after setting a value in the <see cref="T:System.Collections.CollectionBase"/> instance.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="oldValue"/> can be found.</param>
        /// <param name="oldValue">The value to replace with <paramref name="newValue"/>.</param>
        /// <param name="newValue">The new value of the element at <paramref name="index"/>.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            ChartStripLine stripLine = (ChartStripLine)newValue;
            stripLine.Changed += new EventHandler(OnStripLineChaged);
            BroadcastChange();
            base.OnSetComplete(index, oldValue, newValue);
        }

        #endregion

        #region Class event raisers
        /// <summary>
        /// Broadcasts the change.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        private void BroadcastChange()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [strip line chaged].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnStripLineChaged(object sender, EventArgs e)
        {
            BroadcastChange();
        }
        #endregion
    }
}
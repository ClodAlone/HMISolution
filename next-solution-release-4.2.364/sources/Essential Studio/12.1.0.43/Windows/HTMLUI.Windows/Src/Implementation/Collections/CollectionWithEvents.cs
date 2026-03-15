#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

#region file using directives
using System;
using System.Collections;

using Syncfusion.Windows.Forms.HTMLUI;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Description for EventBaseCollection.
    /// </summary>
    public class EventBaseCollection : CollectionBase
    {
        #region Class members

        /// <summary>
        /// Indicates whether the class must skip all event-raising code.
        /// </summary>
        private bool m_bSkipEvents;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets a value indicating whether a collection work in silent mode without raising any event
        /// to user or in normal mode.
        /// </summary>
        public bool QuietMode
        {
            get
            {
                return m_bSkipEvents;
            }
            set
            {
                m_bSkipEvents = value;
            }
        }
        #endregion

        #region Class Events
        /// <summary>
        /// Raised on any change in the collection.
        /// </summary>
        public event EventHandler OnChanged;

        /// <summary>
        /// Raised by <see cref="OnClear"/> method.
        /// </summary>
        public event CollectionEventHandler Clearing;

        /// <summary>
        /// Raised by <see cref="OnClearComplete"/> method.
        /// </summary>
        public event CollectionEventHandler Cleared;

        /// <summary>
        /// Raised by <see cref="OnInsert"/> method.
        /// </summary>
        public event CollectionEventHandler Inserting;

        /// <summary>
        /// Raised by <see cref="OnInsertComplete"/> method.
        /// </summary>
        public event CollectionEventHandler Inserted;

        /// <summary>
        /// Raised by <see cref="OnRemove"/> method.
        /// </summary>
        public event CollectionEventHandler Removing;

        /// <summary>
        /// Raised by <see cref="OnRemoveComplete"/> method.
        /// </summary>
        public event CollectionEventHandler Removed;

        /// <summary>
        /// Raised by <see cref="OnSet"/> method.
        /// </summary>
        public event CollectionEventHandler Setting;

        /// <summary>
        /// Raised by <see cref="OnSetComplete"/> method.
        /// </summary>
        public event CollectionEventHandler Set;

        #endregion

        #region Class Event catchers

        /// <summary>
        /// Overridden. Runs when Clear event raises.
        /// </summary>
        protected override void OnClear()
        {
            CollectionEventArgs ev = CollectionEventArgs.Empty;

            if (Clearing != null && !m_bSkipEvents)
            {
                Clearing(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnClear();
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Runs when ClearComplete event raises.
        /// </summary>
        protected override void OnClearComplete()
        {
            CollectionEventArgs ev = CollectionEventArgs.Empty;

            if (Cleared != null && !m_bSkipEvents)
            {
                Cleared(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnClearComplete();
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Runs when Insert event raises.
        /// </summary>
        /// <param name="index">Index in the collection.</param>
        /// <param name="value">Value for inserting into the collection.</param>
        protected override void OnInsert(int index, object value)
        {
            CollectionEventArgs ev = new CollectionEventArgs(index, value);

            if (Inserting != null && !m_bSkipEvents)
            {
                Inserting(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnInsert(index, value);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Runs when InsertComplete event raises.
        /// </summary>
        /// <param name="index">Index in the collection.</param>
        /// <param name="value">Value for inserting into the collection.</param>
        protected override void OnInsertComplete(int index, object value)
        {
            CollectionEventArgs ev = new CollectionEventArgs(index, value);

            if (Inserted != null && !m_bSkipEvents)
            {
                Inserted(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnInsertComplete(index, value);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Runs when Remove event raises.
        /// </summary>
        /// <param name="index">Index in the collection.</param>
        /// <param name="value">Value for inserting into the collection.</param>
        protected override void OnRemove(int index, object value)
        {
            CollectionEventArgs ev = new CollectionEventArgs(index, value);

            if (Removing != null && !m_bSkipEvents)
            {
                Removing(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnRemove(index, value);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Runs when RemoveComplete event raises.
        /// </summary>
        /// <param name="index">Index in the collection.</param>
        /// <param name="value">Value to remove from the collection.</param>
        protected override void OnRemoveComplete(int index, object value)
        {
            CollectionEventArgs ev = new CollectionEventArgs(index, value);

            if (Removed != null && !m_bSkipEvents)
            {
                Removed(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnRemoveComplete(index, value);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Runs when Set event raises.
        /// </summary>
        /// <param name="index">Index in the collection.</param>
        /// <param name="oldValue">Old value of the object.</param>
        /// <param name="newValue">New value of the object.</param>
        protected override void OnSet(int index, object oldValue, object newValue)
        {
            CollectionEventArgs ev = new CollectionEventArgs(index, newValue, oldValue);

            if (Setting != null && !m_bSkipEvents)
            {
                Setting(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnSet(index, oldValue, newValue);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Runs when SetComplete event raises.
        /// </summary>
        /// <param name="index">Index in the collection.</param>
        /// <param name="oldValue">Old value of the object.</param>
        /// <param name="newValue">New value of the object.</param>
        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            CollectionEventArgs ev = new CollectionEventArgs(index, newValue, oldValue);

            if (Set != null && !m_bSkipEvents)
            {
                Set(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnSetComplete(index, oldValue, newValue);
                RaiseOnChangedEvent();
            }
        }
        #endregion

        #region Class Event Raisers
        /// <summary>
        /// Raises the OnChanged event.
        /// </summary>
        protected void RaiseOnChangedEvent()
        {
            if (OnChanged != null && !m_bSkipEvents)
            {
                OnChanged(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
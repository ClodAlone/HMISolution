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
    /// Collection throws an event on any context change.
    /// </summary>
    public class EventBaseDictionary : DictionaryBase
    {
        #region Class members
        /// <summary>
        /// Indicates whether to skip all event raising.
        /// </summary>
        private bool m_bSkipEvents;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets or sets a value indicating whether event will be raised.
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

        #region Class events

        /// <summary>
        /// Raised on any change in the collection.
        /// </summary>
        public event EventHandler OnChanged;

        /// <summary>
        /// Raised by <see cref="OnClear"/> method.
        /// </summary>
        public event DictionaryEventHandler Clearing;

        /// <summary>
        /// Raised by <see cref="OnClearComplete"/> method.
        /// </summary>
        public event DictionaryEventHandler Cleared;

        /// <summary>
        /// Raised by <see cref="OnGet"/> method.
        /// </summary>
        public event DictionaryEventHandler Get;

        /// <summary>
        /// Raised by <see cref="OnSet"/> method.
        /// </summary>
        public event DictionaryEventHandler Setting;

        /// <summary>
        /// Raised by <see cref="OnSetComplete"/> method.
        /// </summary>
        public event DictionaryEventHandler Set;

        /// <summary>
        /// Raised by <see cref="OnInsert"/> method.
        /// </summary>
        public event DictionaryEventHandler Inserting;

        /// <summary>
        /// Raised by <see cref="OnInsertComplete"/> method.
        /// </summary>
        public event DictionaryEventHandler Inserted;

        /// <summary>
        /// Raised by <see cref="OnRemove"/> method.
        /// </summary>
        public event DictionaryEventHandler Removing;

        /// <summary>
        /// Raised by <see cref="OnRemoveComplete"/> method.
        /// </summary>
        public event DictionaryEventHandler Removed;
        #endregion

        #region Class Event catchers

        /// <summary>
        /// Overridden. Raised when clear event occurs.
        /// </summary>
        protected override void OnClear()
        {
            DictionaryEventArgs ev = DictionaryEventArgs.Empty;

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
        /// Overridden. Raised when ClearComplete event occurs.
        /// </summary>
        protected override void OnClearComplete()
        {
            DictionaryEventArgs ev = DictionaryEventArgs.Empty;

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
        /// Overridden. Raised when Get event occurs.
        /// </summary>
        /// <param name="key">Key for getting value.</param>
        /// <param name="currentValue">Current value corresponding to this key.</param>
        /// <returns>Element with such a key and value.</returns>
        protected override object OnGet(object key, object currentValue)
        {
            DictionaryEventArgs ev = new DictionaryEventArgs(key, currentValue);

            if (Get != null && !m_bSkipEvents)
            {
                Get(this, ev);
            }

            if (!ev.Cancel)
            {
                object obj = base.OnGet(key, currentValue);
                RaiseOnChangedEvent();
                return obj;
            }

            return null;
        }

        /// <summary>
        /// Overridden. Raised when Set event occurs.
        /// </summary>
        /// <param name="key">Key for getting value.</param>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected override void OnSet(object key, object oldValue, object newValue)
        {
            DictionaryEventArgs ev = new DictionaryEventArgs(key, newValue, oldValue);

            if (Setting != null && !m_bSkipEvents)
            {
                Setting(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnSet(key, oldValue, newValue);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Raised when SetComplete event occurs.
        /// </summary>
        /// <param name="key">Key of the object.</param>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>
        protected override void OnSetComplete(object key, object oldValue, object newValue)
        {
            DictionaryEventArgs ev = new DictionaryEventArgs(key, newValue, oldValue);

            if (Set != null && !m_bSkipEvents)
            {
                Set(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnSetComplete(key, oldValue, newValue);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Raised when Insert event occurs.
        /// </summary>
        /// <param name="key">Key of the object.</param>
        /// <param name="value">Value of the object.</param>
        protected override void OnInsert(object key, object value)
        {
            DictionaryEventArgs ev = new DictionaryEventArgs(key, value);

            if (Inserting != null && !m_bSkipEvents)
            {
                Inserting(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnInsert(key, value);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Raised when InsertComplete event occurs.
        /// </summary>
        /// <param name="key">Key of the object.</param>
        /// <param name="value">Value of the object.</param>
        protected override void OnInsertComplete(object key, object value)
        {
            DictionaryEventArgs ev = new DictionaryEventArgs(key, value);

            if (Inserted != null && !m_bSkipEvents)
            {
                Inserted(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnInsertComplete(key, value);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Raised when Remove event occurs.
        /// </summary>
        /// <param name="key">Key of the object.</param>
        /// <param name="value">Value of the object.</param>
        protected override void OnRemove(object key, object value)
        {
            DictionaryEventArgs ev = new DictionaryEventArgs(key, value);

            if (Removing != null && !m_bSkipEvents)
            {
                Removing(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnRemove(key, value);
                RaiseOnChangedEvent();
            }
        }

        /// <summary>
        /// Overridden. Raised when RemoveComplete event occurs.
        /// </summary>
        /// <param name="key">Key of the object.</param>
        /// <param name="value">Value of the object.</param>
        protected override void OnRemoveComplete(object key, object value)
        {
            DictionaryEventArgs ev = new DictionaryEventArgs(key, value);

            if (Removed != null && !m_bSkipEvents)
            {
                Removed(this, ev);
            }

            if (!ev.Cancel)
            {
                base.OnRemoveComplete(key, value);
                RaiseOnChangedEvent();
            }
        }
        #endregion

        #region Class Event Raisers
        /// <summary>
        /// Raises the OnChanged Event.
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
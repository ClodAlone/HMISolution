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
using System.ComponentModel;
using System.ComponentModel.Design;

using Syncfusion.Windows.Forms.HTMLUI;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// This class must be inherited by the developer for each element
    /// where the HTML element supports events.
    /// </summary>
    public abstract class HTMLEventImpl
    : IHTMLEvent, IDisposable
    {
        #region Class members
        /// <summary>
        /// List of events which were attached to the element.
        /// </summary> 
        protected ArrayList m_events;

        /// <summary>
        /// Unique name of the event.
        /// </summary>
        private string m_strName;

        /// <summary>
        /// Parent of the current event.
        /// </summary>
        private IHTMLElement m_parent;

        /// <summary>
        /// Indicates whether the Dispose method was called.
        /// </summary>
        private bool m_bDisposed;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the unique name of the event.
        /// </summary>
        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                if (value != m_strName)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_strName, value);
                    m_strName = value;
                    OnNameChanged(args);
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent element of the current event.
        /// </summary>
        public IHTMLElement Parent
        {
            get
            {
                return m_parent;
            }
            set
            {
                if (value != m_parent)
                {
                    ValueChangedEventArgs args = new ValueChangedEventArgs(m_parent, value);
                    m_parent = value;
                    OnParentChanged(args);
                }
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Utility event. Helps developer to catch any changes of class property.
        /// Name value.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler NameChanged;

        /// <summary>
        /// Utility event. Helps developer to catch any changes of class property.
        /// Parent value.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler ParentChanged;

        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Initializes a new instance of the HTMLEventImpl class
        /// </summary>
        /// <param name="parent">Parent element.</param>
        private HTMLEventImpl(IHTMLElement parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            m_parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the HTMLEventImpl class
        /// </summary>
        /// <param name="parent">Parent of the event.</param>
        /// <param name="name">Name of the event.</param>
        protected HTMLEventImpl(IHTMLElement parent, string name)
            : this(parent)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("Event name can not be empty.");

            m_strName = name;
        }

        /// <summary>
        /// On dispose, detaches all events from the HTML element.
        /// </summary>
        public void Dispose()
        {
            if (!m_bDisposed)
            {
                if (m_events != null && m_events.Count > 0)
                {
                    for (int i = 0; i < m_events.Count; i++)
                    {
                        this.DetachEvent((EventHandler)m_events[i]);
                    }
                }

                m_bDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Finalizes an instance of the HTMLEventImpl class
        /// </summary>
        ~HTMLEventImpl()
        {
            Dispose();
        }
        #endregion

        #region Class event raisers
        /// <summary>
        /// Raises NameChanged event when property value changes.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseNameChanged(ValueChangedEventArgs args)
        {
            if (NameChanged != null)
            {
                NameChanged(this, args);
            }
        }

        /// <summary>
        /// Raises ParentChanged event when property value changes.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected void RaiseParentChanged(ValueChangedEventArgs args)
        {
            if (ParentChanged != null)
            {
                ParentChanged(this, args);
            }
        }
        #endregion

        #region Class overrides
        /// <summary>
        /// Called by property set part on value change. This is best
        /// place for custom logic.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnNameChanged(ValueChangedEventArgs args)
        {
            RaiseNameChanged(args);
        }

        /// <summary>
        /// Called by property set part on value change. This is best
        /// place for custom logic.
        /// </summary>
        /// <param name="args">ValueChangedEventArgs instance</param>
        protected virtual void OnParentChanged(ValueChangedEventArgs args)
        {
            RaiseParentChanged(args);
        }
        #endregion

        #region Class methods
        /// <summary>
        /// Attaches user method to the current event.
        /// </summary>
        /// <param name="handler">User delegate on method which must catch the event raising.</param>
        public void AttachEvent(EventHandler handler)
        {
            AttachEventInternal(handler);
            m_events.Add(handler);
        }

        /// <summary>
        /// Detaches user method to the current event.
        /// </summary>
        /// <param name="handler">User delegate on method which must catch the event raising.</param>
        public void DetachEvent(EventHandler handler)
        {
            DetachEventInternal(handler);
            m_events.Remove(handler);
        }
        #endregion

        #region Class utility methods
        /// <summary>
        /// Attaches user event.
        /// </summary>
        /// <param name="handler">Delegate to the user code.</param>
        public abstract void AttachEventInternal(EventHandler handler);

        /// <summary>
        /// Detaches user event.
        /// </summary>
        /// <param name="handler">Delegate to the user code.</param>
        public abstract void DetachEventInternal(EventHandler handler);

        /// <summary>
        /// Raises event for user.
        /// </summary>
        /// <param name="e">Reference on input parameters.</param>
        public abstract void RaiseEvent(EventArgs e);
        #endregion
    }
}
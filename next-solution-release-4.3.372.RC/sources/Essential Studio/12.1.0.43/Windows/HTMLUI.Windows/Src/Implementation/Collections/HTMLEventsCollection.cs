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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;

using Syncfusion.Windows.Forms.HTMLUI;
using Syncfusion.Windows.Forms.HTMLUI.Implementation;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Implementation of events collection.
    /// </summary>
    public class HTMLEventsCollection
    : EventBaseCollection, IHTMLEventsCollection
    {
        #region Class members
        /// <summary>
        /// Parent of the current events collection.
        /// </summary>
        private IHTMLElement m_parent;

        /// <summary>
        /// Map of event name-to-event.
        /// </summary>
        private IDictionary m_dict = CollectionsUtil.CreateCaseInsensitiveHashtable();
        #endregion

        #region Class properties

        /// <summary>
        /// Gets or sets the parent element of the collection.
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

        /// <summary>
        /// Returns the event with the specified name. If event is not supported by the element, then it will
        /// return NULL.
        /// </summary>
        /// <param name="name">A string variable</param>
        public IHTMLEvent this[string name]
        {
            get
            {
                if (name == null)
                    throw new ArgumentNullException("name");

                if (name.Length == 0)
                    throw new ArgumentException("name can not be empty");

                if (!((BaseElement)m_parent).IsEventSupported(name))
                    return null;

                if (!m_dict.Contains(name))
                {
                    List.Add(((BaseElement)m_parent).CreateEvent(name));
                }

                return (IHTMLEvent)m_dict[name];
            }
        }
        #endregion

        #region Class events

        /// <summary>
        /// Utility event. Raised when parent property changes.
        /// </summary>
        [Category("Property Changed")]
        public event ValueChangedEventHandler ParentChanged;
        #endregion

        #region Class initialize/finalize methods
        /// <summary>
        /// Prevents a default instance of the HTMLEventsCollection class from being created
        /// </summary>
        private HTMLEventsCollection()
        {
            this.Cleared += new CollectionEventHandler(HTMLEventsCollection_Cleared);
            this.Inserted += new CollectionEventHandler(HTMLEventsCollection_Inserted);
            this.Removed += new CollectionEventHandler(HTMLEventsCollection_Removed);
            this.Set += new CollectionEventHandler(HTMLEventsCollection_Set);
        }

        /// <summary>
        /// Initializes a new instance of the HTMLEventsCollection class
        /// </summary>
        /// <param name="parent">Element which is the parent of the collection.</param>
        public HTMLEventsCollection(IHTMLElement parent)
            : this()
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            m_parent = parent;
        }
        #endregion

        #region Class Public Methods

        /// <summary>
        /// Indicates whether event with the specified name is supported.
        /// </summary>
        /// <param name="name">Name of the event.</param>
        /// <returns>True if such an event is supported.</returns>
        public bool Contains(string name)
        {
            return ((BaseElement)m_parent).IsEventSupported(name);
        }

        /// <summary>
        /// Overridden. Copies to the specified array.
        /// </summary>
        /// <param name="array">Destination array of events.</param>
        /// <param name="index">Start index.</param>
        public void CopyTo(IHTMLEventsCollection[] array, int index)
        {
            ((ICollection)this).CopyTo(array, index);
        }
        #endregion

        #region Class event raisers

        /// <summary>
        /// Method raises the ParentChanged event.
        /// </summary>
        /// <param name="args">A ValueChangedEventArgs instance</param>
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
        /// Method called by parent property set part. This is the best place to
        /// write any logic dependent on the parent property.
        /// </summary>
        /// <param name="args">A ValueChangedEventArgs instance</param>
        protected virtual void OnParentChanged(ValueChangedEventArgs args)
        {
            RaiseParentChanged(args);
        }
        #endregion

        #region Class utility methods

        /// <summary>
        /// Overloaded. Attaches user delegate to the specified event. If event does not exist in the collection
        /// and the element supports such an event, then it will create a new instance
        /// of the event class to which the delegate will be attached.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handler">Delegate to attach.</param>
        public void AttachEvent(string name, EventHandler handler)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name can not be empty");

            if (handler == null)
                throw new ArgumentNullException("handler");

            IHTMLEvent evnt = this[name];

            if (evnt != null)
            {
                evnt.AttachEvent(handler);
            }
        }

        /// <summary>
        /// Attaches user delegate to the specified event. If event does not exist in the collection
        /// and the element supports such an event, then it will create a new instance
        /// of the event class to which the delegate will be attached.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handlers">Array of the delegates.</param>
        public void AttachEvent(string name, EventHandler[] handlers)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name can not be empty");

            if (handlers == null)
                throw new ArgumentNullException("handlers");

            IHTMLEvent evnt = this[name];

            if (evnt != null)
            {
                EventHandler handler = null;
                for (int i = 0, len = handlers.Length; i < len; i++)
                {
                    handler = handlers[i];
                    evnt.AttachEvent(handler);
                }
            }
        }

        /// <summary>
        /// Overloaded. Detaches delegate of the specified event.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handler">Delegate to detach.</param>
        public void DetachEvent(string name, EventHandler handler)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name can not be empty");

            if (handler == null)
                throw new ArgumentNullException("handler");

            IHTMLEvent evnt = this[name];

            if (evnt != null)
            {
                evnt.DetachEvent(handler);
            }
        }

        /// <summary>
        /// Detaches delegates of the specified event.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handlers">Array of delegates.</param>
        public void DetachEvent(string name, EventHandler[] handlers)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (name.Length == 0)
                throw new ArgumentException("name can not be empty");

            if (handlers == null)
                throw new ArgumentNullException("handlers");

            IHTMLEvent evnt = this[name];

            if (evnt != null)
            {
                EventHandler handler = null;
                for (int i = 0, len = handlers.Length; i < len; i++)
                {
                    handler = handlers[i];
                    evnt.DetachEvent(handler);
                }
            }
        }
        #endregion

        #region Keep in sync methods

        /// <summary>
        /// On collection clear, remove name access hash values.
        /// </summary>
        /// <param name="sender">This collection.</param>
        /// <param name="e">Will be empty for clear operation.</param>
        private void HTMLEventsCollection_Cleared(object sender, CollectionEventArgs e)
        {
            m_dict.Clear();
        }

        /// <summary>
        /// On item insert, check if it is unique and add it's name into fast access by name hash.
        /// </summary>
        /// <param name="sender">This collection.</param>
        /// <param name="e">Value which will be inserted into the collection.</param>
        private void HTMLEventsCollection_Inserted(object sender, CollectionEventArgs e)
        {
            IHTMLEvent evnt = (IHTMLEvent)e.Value;

            if (m_dict.Contains(evnt.Name))
                throw new ArgumentException("Collection can not contain too many events with the same name. Name is: " + evnt.Name);

            m_dict.Add(evnt.Name, evnt);
        }

        /// <summary>
        /// On item remove, also remove event name from fast access by name hash.
        /// </summary>
        /// <param name="sender">This collection.</param>
        /// <param name="e">Value which will be removed from the collection.</param>
        private void HTMLEventsCollection_Removed(object sender, CollectionEventArgs e)
        {
            IHTMLEvent evnt = (IHTMLEvent)e.Value;

            m_dict.Remove(evnt.Name);
        }

        /// <summary>
        /// On item replace, update fast access by name hash.
        /// </summary>
        /// <param name="sender">This collection.</param>
        /// <param name="e">Old and new values.</param>
        private void HTMLEventsCollection_Set(object sender, CollectionEventArgs e)
        {
            IHTMLEvent evnt = (IHTMLEvent)e.Value;
            IHTMLEvent evntOld = (IHTMLEvent)e.OldValue;

            m_dict.Remove(evntOld.Name);
            m_dict.Add(evnt.Name, evnt);
        }
        #endregion
    }
}
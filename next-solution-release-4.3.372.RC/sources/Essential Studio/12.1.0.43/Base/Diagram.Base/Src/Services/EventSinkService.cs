#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Event key.
    /// </summary>
    public sealed class EventKey
        : object 
    { 
    }

    /// <summary>
    /// Even sink service.
    /// </summary>
    public abstract class EventSink
        : Service
    {
        #region Class EventKeys
        private static readonly EventKey m_keyPropertyChanging;
        private static readonly EventKey m_keyPropertyChanged;
        private static readonly EventKey m_keySelectionChanging;
        private static readonly EventKey m_keySelectionChanged;
        private static readonly EventKey m_keyCancelled;
        #endregion

        #region Class members
        private object m_syncRoot;
        private Hashtable m_hashEvents;
        private Hashtable m_hashControllerHandlers;
        #endregion

        #region Class initalize/finalize methods
        /// <summary>
        /// Initializes static members of the <see cref="EventSink"/> class.
        /// </summary>
        static EventSink()
        {
            m_keyPropertyChanged = new EventKey();
            m_keyPropertyChanging = new EventKey();
            m_keySelectionChanging = new EventKey();
            m_keySelectionChanged = new EventKey();
            m_keyCancelled = new EventKey();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSink"/> class.
        /// </summary>
        public EventSink()
        {
            m_syncRoot = new object();
        }
        #endregion

        #region Class properties
        /// <summary>
        /// Gets the handlers.
        /// </summary>
        /// <value>The handlers.</value>
        protected Hashtable Handlers
        {
            get
            {
                if (m_hashEvents == null)
                {
                    m_hashEvents = new Hashtable();
                }

                return m_hashEvents;
            }
        }

        /// <summary>
        /// Gets the high priority handlers.
        /// </summary>
        /// <value>The high priority handlers.</value>
        protected Hashtable HighPriorityHandlers
        {
            get
            {
                if (m_hashControllerHandlers == null)
                {
                    m_hashControllerHandlers = new Hashtable();
                }

                return m_hashControllerHandlers;
            }
        }
        #endregion

        #region Class events
        /// <summary>
        /// Occurs when property changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging
        {
            add { AddHandler(m_keyPropertyChanging, value); }
            remove { RemoveHandler(m_keyPropertyChanging, value); }
        }

        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { AddHandler(m_keyPropertyChanged, value); }
            remove { RemoveHandler(m_keyPropertyChanged, value); }
        }

        /// <summary>
        /// Occurs when node collection changing.
        /// </summary>
        public virtual event CollectionExEventHandler NodeCollectionChanging
        {
            add { AddHandler(m_keySelectionChanging, value); }
            remove { RemoveHandler(m_keySelectionChanging, value); }
        }

        /// <summary>
        /// Occurs when node collection changed.
        /// </summary>
        public virtual event CollectionExEventHandler NodeCollectionChanged
        {
            add { AddHandler(m_keySelectionChanged, value); }
            remove { RemoveHandler(m_keySelectionChanged, value); }
        }

        /// <summary>
        /// Occurs when cancel the eventsink.
        /// </summary>
        public virtual event CancelCollectionChangedEventHandler CancelCollectionChanged
        {
            add { AddHandler(m_keyCancelled, value); }
            remove { RemoveHandler(m_keyCancelled, value); }
        }
        #endregion

        #region Class public methods
        /// <summary>
        /// Raises the nodes changing event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        /// <returns>true, if raise nodes changing event.</returns>
        public virtual bool RaiseNodesChangingEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keySelectionChanging] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            // if service status is started -- execute generic handlers
            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanging event
                handler = this.Handlers[m_keySelectionChanging] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the nodes changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public virtual void RaiseNodesChangedEvent(CollectionExEventArgs evtArgs)
        {
            CollectionExEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keySelectionChanged] as CollectionExEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keySelectionChanged] as CollectionExEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the cancel collection changed event.
        /// </summary>
        /// <param name="evtArgs">The <see cref="Syncfusion.Windows.Forms.Diagram.CollectionExEventArgs"/> instance containing the event data.</param>
        public virtual void RaiseCancelCollectionChangedEvent(CollectionExEventArgs evtArgs)
        {
            CancelCollectionChangedEventHandler handler;

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyCancelled] as CancelCollectionChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise NodesChanged event
                handler = this.Handlers[m_keyCancelled] as CancelCollectionChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }

        /// <summary>
        /// Raises the property changing event.
        /// </summary>
        /// <param name="propertyContainer">The property container.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>true, raise property changing event.</returns>
        public bool RaisePropertyChangingEvent(IPropertyContainer propertyContainer, string strPropertyName, object newValue)
        {
            PropertyChangingEventHandler handler;

            // Create PropertyChangingEvtArgs
            PropertyChangingEventArgs evtArgs = new PropertyChangingEventArgs(propertyContainer, strPropertyName, newValue);

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyPropertyChanging] as PropertyChangingEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise PropertyChanging event
                handler = this.Handlers[m_keyPropertyChanging] as PropertyChangingEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }

            return !evtArgs.Cancel;
        }

        /// <summary>
        /// Raises the property changed event.
        /// </summary>
        /// <param name="nodeAffected">The node affected.</param>
        /// <param name="strPropertyName">Name of the property.</param>
        public void RaisePropertyChangedEvent(IPropertyContainer nodeAffected, string strPropertyName)
        {
            PropertyChangedEventHandler handler;

            // Create EvtArgs
            PropertyChangedEventArgs evtArgs = new PropertyChangedEventArgs(nodeAffected, strPropertyName);

            // execute high priority handlers anyway
            handler = this.HighPriorityHandlers[m_keyPropertyChanged] as PropertyChangedEventHandler;

            if (handler != null)
            {
                handler(evtArgs);
            }

            if (this.ServiceStatus == Diagram.ServiceStatus.Started || this.ServiceStatus == Diagram.ServiceStatus.Resumed)
            {
                // Raise PropertyChanging event
                handler = this.Handlers[m_keyPropertyChanged] as PropertyChangedEventHandler;

                if (handler != null)
                {
                    handler(evtArgs);
                }
            }
        }
        #endregion

        #region Class helper methods
        /// <summary>
        /// Adds the handler.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="dNewHandler">The delegate.</param>
        protected void AddHandler(EventKey key, Delegate dNewHandler)
        {
            // ensure thread safety
            lock (m_syncRoot)
            {
                // if adding hadler has EventHandlerPriorityAttribute add it to HighPriority
                if (IsHighPriorityHandler(dNewHandler))
                {
                    AddHandler(this.HighPriorityHandlers, key, dNewHandler);
                }
                else
                {
                    AddHandler(this.Handlers, key, dNewHandler);
                }
            }
        }

        /// <summary>
        /// Removes the handler.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="dHandler">The delegate.</param>
        protected void RemoveHandler(EventKey key, Delegate dHandler)
        {
            // ensure thread safety
            lock (m_syncRoot)
            {
                if (IsHighPriorityHandler(key, dHandler))
                {
                    RemoveHandler(this.HighPriorityHandlers, key, dHandler);
                }
                else
                {
                    RemoveHandler(this.Handlers, key, dHandler);
                }
            }
        }

        /// <summary>
        /// Adds the handler.
        /// </summary>
        /// <param name="hashHandlers">The hash handlers.</param>
        /// <param name="key">The key.</param>
        /// <param name="dNewHandler">The new delegate.</param>
        private void AddHandler(Hashtable hashHandlers, EventKey key, Delegate dNewHandler)
        {
            if (hashHandlers.ContainsKey(key))
            {
                Delegate delCurHandlers = (Delegate)hashHandlers[key];

                if (delCurHandlers != null)
                {
                    hashHandlers[key] = Delegate.Combine(delCurHandlers, dNewHandler);
                }
            }
            else
            {
                hashHandlers[key] = dNewHandler;
            }
        }

        /// <summary>
        /// Removes the handler.
        /// </summary>
        /// <param name="hashHandlers">The hash handlers.</param>
        /// <param name="key">The key.</param>
        /// <param name="dHandler">The delegate.</param>
        private void RemoveHandler(Hashtable hashHandlers, EventKey key, Delegate dHandler)
        {
            Delegate dCurHandlers = hashHandlers[key] as Delegate;

            if (dCurHandlers != null)
            {
                dCurHandlers = Delegate.Remove(dCurHandlers, dHandler);

                if (dCurHandlers == null)
                {
                    hashHandlers.Remove(key);
                }
                else
                {
                    // update hash value
                    hashHandlers[key] = dCurHandlers;
                }
            }
        }

        /// <summary>
        /// Determines whether it is high priority handler.
        /// </summary>
        /// <param name="dlgtHandler">The delegate.</param>
        /// <returns>
        /// <c>true</c> if it is high priority handler; otherwise, <c>false</c>.
        /// </returns>
        private bool IsHighPriorityHandler(Delegate dlgtHandler)
        {
            bool bSuccess = false;

            Attribute[] arrAttributes = Attribute.GetCustomAttributes(dlgtHandler.Method);
            EventHandlerPriorityAttribute attrTemp;

            foreach (Attribute attrCur in arrAttributes)
            {
                attrTemp = attrCur as EventHandlerPriorityAttribute;

                if (attrTemp != null && attrTemp.ExecuteRegardlessOfServiceState)
                {
                    bSuccess = true;
                    break;
                }
            }

            return bSuccess;
        }

        /// <summary>
        /// Determines whether it is high priority handler.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="dNewHandler">The delegate.</param>
        /// <returns>
        /// <c>true</c> if it is high priority handler; otherwise, <c>false</c>.
        /// </returns>
        private bool IsHighPriorityHandler(EventKey key, Delegate dNewHandler)
        {
            bool bSuccess = false;

            if (this.HighPriorityHandlers.ContainsKey(key))
            {
                Delegate dlgt = (Delegate)this.HighPriorityHandlers[key];
                Delegate[] delegates = dlgt.GetInvocationList();

                foreach (Delegate dlgtCur in delegates)
                {
                    if (dlgtCur == dNewHandler)
                    {
                        bSuccess = true;
                        break;
                    }
                }
            }

            return bSuccess;
        }
        #endregion
    }
}

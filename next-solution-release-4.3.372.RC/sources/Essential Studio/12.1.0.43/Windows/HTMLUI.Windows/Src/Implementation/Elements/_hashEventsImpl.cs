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
using System.Diagnostics;
using System.Reflection;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Class that supports connection events for their owners (HTML elements).
    /// </summary>
    public sealed class HashElementEvents : HTMLEventImpl
    {
        #region Class members

        /// <summary>
        /// Holds all events.
        /// </summary>
        private Hashtable m_eventsHash;
        #endregion

        #region Class Properties
        /// <summary>
        /// Gets the event information
        /// </summary>
        public EventInfo Event
        {
            get
            {
                return ((ElementEventAttribute)m_eventsHash[this.Name]).Event;
            }
        }

        /// <summary>
        /// Gets the method information
        /// </summary>
        public MethodInfo RaiserMethod
        {
            get
            {
                if (m_eventsHash == null || !m_eventsHash.Contains(this.Name))
                {
                    if (this.Parent.Events.Contains(this.Name))
                    {
                        object sender = this.Parent.Events[this.Name];
                        if (sender is HashElementEvents == false) return null;

                        return ((HashElementEvents)sender).RaiserMethod;
                    }

                    return null;
                }

                return ((ElementEventAttribute)m_eventsHash[this.Name]).RaiserMethod;
            }
        }
        #endregion

        #region Class Initialize/Finalize methods

        /// <summary>
        /// Initializes a new instance of the HashElementEvents class
        /// </summary>
        /// <param name="parent">Parent element object.</param>
        /// <param name="events">Supported events.</param>
        /// <param name="name">Name of the event.</param>
        public HashElementEvents(IHTMLElement parent, Hashtable events, string name)
            : base(parent, name)
        {
            m_eventsHash = (Hashtable)events.Clone();
        }

        #endregion

        #region Class Overrides
        /// <summary>
        /// Overridden. Attaches events to the object.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        public override void AttachEventInternal(EventHandler handler)
        {
            Event.AddEventHandler(this.Parent, handler);
        }

        /// <summary>
        /// Overridden. Detaches event from the object.
        /// </summary>
        /// <param name="handler">Event handler.</param>
        public override void DetachEventInternal(EventHandler handler)
        {
            Event.RemoveEventHandler(this.Parent, handler);
        }

        /// <summary>
        /// Overridden. Overloaded. Raises event on the object.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        public override void RaiseEvent(EventArgs e)
        {
            if (this.RaiserMethod != null)
            {
#if DEBUG
                try
                {
#endif
                    RaiserMethod.Invoke(this.Parent, new object[] { e });
#if DEBUG
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");
                    throw;
                }
#endif
            }
        }

        /// <summary>
        /// Raises event on the object.
        /// </summary>
        /// <param name="parent">Parent of the event.</param>
        /// <param name="e">Event parameters.</param>
        public void RaiseEvent(object parent, EventArgs e)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (this.RaiserMethod != null)
            {
                RaiserMethod.Invoke(parent, new object[] { e });
            }
        }
        #endregion
    }
}

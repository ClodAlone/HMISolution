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
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Collection that stores and provides access to the events of the HTML element.
    /// </summary>
    public interface IHTMLEventsCollection
    : IHTMLCollection
    {
        #region Interface Properties
        /// <summary>
        /// Returns the event interface with the specified name. Name is case insensitive.
        /// </summary>
        /// <param name="name">Name value</param>
        IHTMLEvent this[string name]
        { 
            get;
        }
        #endregion

        #region Interface Helper Methods
        /// <summary>
        /// Attaches user delegate to the specified event. If event does not exists in the collection
        /// and element supports such an event then it will create a new instance
        /// of the event class to which the delegate will be attached.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handler">Delegete to attach.</param>
        void AttachEvent(string name, EventHandler handler);

        /// <summary>
        /// Attaches user delegates to the specified event. If event does not exists in the collection
        /// and element supports such an event then it will create a new instance
        /// of the event class to which the delegate will be attached.
        /// </summary>
        /// <param name="name">Event name</param>
        /// <param name="handlers">Array of delegates.</param>
        void AttachEvent(string name, EventHandler[] handlers);

        /// <summary>
        /// Detaches delegete of the specified event.
        /// </summary>
        /// <param name="name">Event name.</param>
        /// <param name="handler">Delegate to detach.</param>
        void DetachEvent(string name, EventHandler handler);

        /// <summary>
        /// Detach delegetes of specified event.
        /// </summary>
        /// <param name="name">event name</param>
        /// <param name="handlers">array of delegates</param>
        void DetachEvent(string name, EventHandler[] handlers);

        /// <summary>
        /// Indicates whether the event with the specified name is supported by the element.
        /// </summary>
        /// <param name="name">Name of  the event.</param>
        /// <returns>TRUE if element exists in collection.</returns>
        bool Contains(string name);
        #endregion
    }
}
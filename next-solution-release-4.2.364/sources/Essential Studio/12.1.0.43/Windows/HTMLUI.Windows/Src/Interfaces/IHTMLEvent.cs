#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Interface which publishes the event of the HTML element.
    /// </summary>
    public interface IHTMLEvent
    {
        /// <summary>
        /// Gets the Parent element of the current event.
        /// </summary>
        IHTMLElement Parent 
        { 
            get; 
        }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        string Name 
        { 
            get;
        }

        /// <summary>
        /// Attaches user method to the current event.
        /// </summary>
        /// <param name="handler">User delegate on method which must catch event raising.</param>
        void AttachEvent(EventHandler handler);

        /// <summary>
        /// Detaches user method to the current event.
        /// </summary>
        /// <param name="handler">User delegate on method which must catch event raising.</param>
        void DetachEvent(EventHandler handler);
    }
}
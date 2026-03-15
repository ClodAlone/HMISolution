#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Interface declaring base functionality for custom control wrappers.
    /// </summary>
    public interface ICustomControlBase
    {
        /// <summary>
        /// Gets the custom control object.
        /// </summary>
        Control CustomControl 
        { 
            get;
        }

        /// <summary>
        /// Gets the parent tag element for the custom control.
        /// </summary>
        IHTMLElement Parent 
        { 
            get; 
        }

        /// <summary>
        /// Gets or sets the default size of the control.
        /// </summary>
        Size DefaultSize 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Sets the location of the control if location of the custom
        /// tag has been changed.
        /// </summary>
        void SetLocation();

        /// <summary>
        /// Creates an instance of the control and configures it.
        /// </summary>
        void InitializeControl();

        /// <summary>
        /// Disposes control and releases all its resources.
        /// </summary>
        void DisposeControl();

        /// <summary>
        /// Attaches events to the control.
        /// </summary>
        void AttachEvents();

        /// <summary>
        /// Detaches events from the element.
        /// </summary>
        void DetachEvents();

        /// <summary>
        /// Raised when initialization has finished.
        /// </summary>
        void OnInitEndCallback();
    }
}

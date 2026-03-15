#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
////  Use of this code is subject to the terms of our license.
////  A copy of the current license can be obtained at any time by e-mailing
////  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#region file using directives
using System;

using Syncfusion.Windows.Forms.HTMLUI;
#endregion

namespace Syncfusion.Windows.Forms.HTMLUI
{
    /// <summary>
    /// Interface that publishes the logic which specify allowed actions with any
    /// HTML element.
    /// </summary>
    public interface IHTMLAttribute
    {
        /// <summary>
        /// Event raised after the attribute value changes. To event handlers
        /// send new and old value of attribute.
        /// </summary>
        event ValueChangedEventHandler ValueChanged;

        /// <summary>
        /// Gets the name of the attribute.
        /// </summary>
        string Name 
        { 
            get;
        }

        /// <summary>
        /// Gets or sets the current value of the attribute as variant. NULL if attribute value
        /// was never set before.
        /// </summary>
        string Value
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets the parent of the attribute.
        /// </summary>
        IHTMLElement Parent 
        { 
            get; 
        }

        /// <summary>
        /// Gets a value indicating whether the property runtime must not be serialized to text.
        /// </summary>
        bool IsRuntimeAttribute 
        { 
            get; 
        }
       }
}
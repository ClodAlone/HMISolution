#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.
#if WINDOWS_PHONE
namespace Syncfusion.WP.Controls.Navigation
#else
#if SILVERLIGHT
namespace Syncfusion.Tools.Controls.Layout
#else
namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
#endif
{
    /// <summary>
    /// Determines the action the AccordionItem will perform.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    internal enum AccordionAction
    {
        /// <summary>
        /// No action will be performed.
        /// </summary>
        None,

        /// <summary>
        /// A collapse will be performed.
        /// </summary>
        Collapse,
        
        /// <summary>
        /// An expand will be performed.
        /// </summary>
        Expand,
        
        /// <summary>
        /// A resize will be performed.
        /// </summary>
        Resize
    }
}
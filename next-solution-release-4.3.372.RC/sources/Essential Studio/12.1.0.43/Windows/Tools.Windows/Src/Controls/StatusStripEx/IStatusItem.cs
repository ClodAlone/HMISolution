#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

#if !( SyncfusionFramework1_0 || SyncfusionFramework1_1 )
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Forms.Tools
{
    public interface IStatusItem
    {
        /// <summary>
        /// Gets or sets a value indicating whether the component is shown in StatusStrip.
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the component is the last item in the group.
        /// </summary>
        bool EndOfGroup { get; set; }

        /// <summary>
        /// Gets the text that is to be displayed on the item. 
        /// </summary>
        string Text { get; }
    }

    public interface IStatusItem2 : IStatusItem
    {
        /// <summary>
        /// Gets or sets the text that is to be displayed in context menu. 
        /// </summary>
        string StatusString { get; set; }
    }
}
#endif

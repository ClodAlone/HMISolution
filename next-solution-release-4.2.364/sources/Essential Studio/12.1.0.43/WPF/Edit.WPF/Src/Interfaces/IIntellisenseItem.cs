#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
using System.Windows.Media;

namespace Syncfusion.Windows.Edit
{
#if SyncfusionFramework4_0

    using System.ComponentModel;

    [DesignTimeVisible(false)]
#endif
    /// <summary>
    ///
    /// </summary>
    public interface IIntellisenseItem
    {
        /// <summary>
        ///
        /// </summary>
        string Text { get; set; }

        /// <summary>
        ///
        /// </summary>
        ImageSource Icon { get; set; }

        /// <summary>
        ///
        /// </summary>
        IEnumerable<IIntellisenseItem> NestedItems { get; set; }
    }
}
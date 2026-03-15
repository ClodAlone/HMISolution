#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IGridDataFilterAction : IDisposable
    {
        /// <summary>
        /// Implement this method in the filter pane for focusing filtering controls. This would be called when the popup is opened.
        /// </summary>
        void Invoke();

        /// <summary>
        /// Set the DataContext of the FilteringPane with the wrapper instance value;
        /// </summary>
        /// <param name="wrapperInstance"></param>
        void SetDataContext(GridDataFilterWrapper wrapperInstance);
    }
}

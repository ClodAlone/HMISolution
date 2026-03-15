#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Primitives
{
    /// <summary>
    ///  Defines how the LoopingSelector communicates with data source.
    /// </summary>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public interface ILoopingSelectorDataSource
    {
        /// <summary>
        ///  Get the next datum, relative to an existing datum.
        /// </summary>
        /// <param name="relativeTo"></param>
        object GetNext(object relativeTo);

        /// <summary>
        ///  Get the previous datum, relative to an existing datum.
        /// </summary>
        /// <param name="relativeTo"></param>
        object GetPrevious(object relativeTo);

        /// <summary>
        /// Gets or sets the current selected item.
        /// </summary>
        object SelectedItem { get; set; }

        /// <summary>
        /// Occurs when the selected item changed.
        /// </summary>
        /// <seealso
        /// cref="P:Syncfusion.UI.Xaml.Primitives.ILoopingSelectorDataSource.SelectedItem"/>
        event EventHandler<SelectionChangedEventArgs> SelectionChanged;
    }
}

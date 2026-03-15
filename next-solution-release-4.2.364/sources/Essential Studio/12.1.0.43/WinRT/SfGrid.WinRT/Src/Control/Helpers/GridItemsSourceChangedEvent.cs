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
using Syncfusion.UI.Xaml.Grid;

namespace Syncfusion.UI.Xaml.Grid
{
    /// <summary>
    /// Delegate for ItemsSourceChanged Event.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="args">An <see cref="T:Syncfusion.UI.Xaml.Grid.GridItemsSourceChangedEventArgs">GridItemsSourceChangedEventArgs</see> that contains the event data.</param>
    /// <remarks></remarks>
    public delegate void GridItemsSourceChangedEventHandler(object sender,GridItemsSourceChangedEventArgs e);
    /// <summary>
    /// Class which contains the arguments for ItemsSourceChanged Event.
    /// </summary>
    /// <remarks></remarks>
    public class GridItemsSourceChangedEventArgs:GridEventArgs
    {
        public GridItemsSourceChangedEventArgs(object originalSource,object oldItemsSource, object newItemsSource)
            : base(originalSource)
        {
            this.OldItemsSource = oldItemsSource;
            this.NewItemsSource = newItemsSource;
        }
        /// <summary>
        /// Property Which gets old ItemsSource.
        /// </summary>
        public object OldItemsSource { get; internal set; }
        /// <summary>
        /// Property Which gets new ItemsSource.
        /// </summary>
        public object NewItemsSource { get; internal set; }
    }
}

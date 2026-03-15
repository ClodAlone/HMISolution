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

#if !SILVERLIGHT
namespace Syncfusion.Windows.Controls.PivotGrid
#else
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// The Event ItemSourceChanged will fire whenever the item source of PivotGrid is changed
    /// </summary>
    public delegate void ItemSourceChanged(object sender,ItemsSourceChangedEventArgs e);
   
    /// <summary>
    /// Represents the method that will handle the event on selection change.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void SelectionChanged(object sender, PivotGridSelectionChangedEventArgs e);

    /// <summary>
    /// Represents the method that will handle the event before expanding pivot item group.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void Expanding(object sender,ExpandingEventArgs e);

    /// <summary>
    /// Represents the method that will handle the event after expanding pivot item group.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void Expanded(object sender,ExpandedEventArgs e);

    /// <summary>
    /// Represents the method that will handle the event before collapsing pivot item group.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void Collapsing(object sender,CollapsingEventArgs e);
 
    /// <summary>
    /// Represents the method that will handle the event after collapsing pivot item group.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void Collapsed(object sender,CollapsedEventArgs e);
  
    /// <summary>
    /// Represents the method that will handle the event on clicking the hyperlink enabled cell.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void HyperlinkCellClick(object sender,HyperlinkCellClickEventArgs e);

    /// <summary>
    /// Represents the method that will handle the event before refreshing the pivot grid.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void DataRefreshing(object sender,DataRefreshingArgs e);

    /// <summary>
    /// Represents the method that will handle the event after refreshing the pivot grid.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void DataRefreshed(object sender,DataRefreshedArgs e);
    
    /// <summary>
    /// Represents the method that will handle the event when the grouping bar is loaded.
    /// </summary>
    /// <param name="sender">The object where the event handler is attached.</param>
    /// <param name="e">The event data.</param>
    public delegate void GroupingBarLoaded(object sender,EventArgs e);

#if SILVERLIGHT
    public delegate void CommandExecuteChanged(object sender, CommandEventArgs e);

    public delegate void ArrangeOverrideExecuted(object sender,ArrangeOverrideEventArgs e);

    public delegate void ContainerPrepared(object sender,ItemContainerPrepared e);

    public delegate void CommandExpanderChanged(object sender, EventArgs e);
#endif

}

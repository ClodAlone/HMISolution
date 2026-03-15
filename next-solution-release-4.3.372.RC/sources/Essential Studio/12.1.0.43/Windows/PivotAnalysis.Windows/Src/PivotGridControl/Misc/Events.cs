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

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    /// <summary>
    /// Represents the method that will handle ItemSourceChanged event
    /// </summary>
    public delegate void ItemSourceChangedEventHandler(object sender, ItemSourceChangedEventArgs e);

    /// <summary>
    ///  Represents the method that will handle SelectionChanged event
    /// </summary>
    public delegate void SelectionChanged(object sender, PivotGridSelectionChangedEventArgs e);

    /// <summary>
    ///  Represents the method that will handle Expanding event
    /// </summary>
    public delegate void Expanding(object sender,ExpandingEventArgs e);

    /// <summary>
    ///  Represents the method that will handle Expanded event
    /// </summary>
    public delegate void Expanded(object sender,ExpandedEventArgs e);

    /// <summary>
    ///  Represents the method that will handle Collapsing event
    /// </summary>
    public delegate void Collapsing(object sender,CollapsingEventArgs e);

    /// <summary>
    ///  Represents the method that will handle Collapsed event
    /// </summary>
    public delegate void Collapsed(object sender,CollapsedEventArgs e);

    /// <summary>
    ///  Represents the method that will handle HyperlinkCellClick event
    /// </summary>
    public delegate void HyperlinkCellClick(object sender,HyperlinkCellClickEventArgs e);

    /// <summary>
    ///  Represents the method that will handle DataRefreshing event
    /// </summary>
    public delegate void DataRefreshing(object sender,DataRefreshingEventArgs e);

    /// <summary>
    ///  Represents the method that will handle DataRefreshed event
    /// </summary>
    public delegate void DataRefreshed(object sender,DataRefreshedEventArgs e);

    /// <summary>
    ///  Represents the method that will handle GroupingBarLoaded event
    /// </summary>
    public delegate void GroupingBarLoaded(object sender,EventArgs e);

}

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
using System.Collections.ObjectModel;
#if SILVERLIGHT
namespace Syncfusion.PivotAnalysis.Base.Silverlight
#else
namespace Syncfusion.PivotAnalysis.Base
#endif
{
    /// <summary>
    /// Interface for Pivot Grid
    /// </summary>
    public interface IPivotControl
    {
        /// <summary>
        /// Obtains the Number of Pivot Columns
        /// </summary>
        ObservableCollection<PivotItem> PivotColumns
        {
            get;
        }
        /// <summary>
        /// Obtains the Number of Pivot Fileds
        /// </summary>
        ObservableCollection<PivotItem> PivotFields
        {
            get;
        }
        /// <summary>
        /// Obtains the Number of Pivot Rows
        /// </summary>
        ObservableCollection<PivotItem> PivotRows
        {
            get;
        }
        /// <summary>
        /// Obtains the Filter Expressions
        /// </summary>
        ObservableCollection<FilterExpression> Filters
        {
            get;
        }
        /// <summary>
        /// Gets the Calculation to be performed in PivotGrid
        /// </summary>
        ObservableCollection<PivotComputationInfo> PivotCalculations
        {
            get;
        }
        /// <summary>
        /// Used to Peform refreshing operations.
        /// </summary>
        bool DeferLayoutUpdate { get; set; }
        /// <summary>
        /// To show Rows in columns and vice-versa
        /// </summary>
        bool ShowCalculationsAsColumns { get; set; }
        /// <summary>
        /// PivotEngine
        /// </summary>
        PivotEngine PivotEngine { get; set; }
        /// <summary>
        /// Source of the Items in pivot Grid
        /// </summary>
        object ItemSource { get; set; }
        /// <summary>
        /// is used to specify whether the background has to be changed or not.
        /// </summary>
        event EventHandler  ShowDisabledGroupBackgroundPropertyChanged;
    }
}

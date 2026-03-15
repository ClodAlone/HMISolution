#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.XlsIO
{
    /// <summary>
    /// Represents pivot field object.
    /// </summary>
    public interface IPivotField
    {
        /// <summary>
        /// Returns pivot field name. Read-only.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Gets/sets field axis.
        /// </summary>
        PivotAxisTypes Axis { get; set; }
        /// <summary>
        /// Gets/sets the Filter value of Page fields
        /// </summary>
       // string FilterValue { get; set; }
        /// <summary>
        /// Gets/sets number format.
        /// </summary>
        string NumberFormat { get; set; }
        /// <summary>
        /// Gets or sets type of field subtotals.
        /// </summary>
        PivotSubtotalTypes Subtotals { get; set; }
        /// <summary>
        /// User can drag field to row area.
        /// </summary>
        bool CanDragToRow { get; set; }
        /// <summary>
        /// User can drag field to column area.
        /// </summary>
        bool CanDragToColumn { get; set; }
        /// <summary>
        /// User can drag field to page area.
        /// </summary>
        bool CanDragToPage { get; set; }
        /// <summary>
        /// User can remove field from view.
        ///</summary>
        bool CanDragOff { get; set; }
        /// <summary>
        /// True if a blank row is inserted after the specified row field in a PivotTable report.
        /// </summary>
        bool ShowBlankRow { get; set; } 
        /// <summary>
        /// True if the specified field can be dragged to the data position. The default value is True.
        /// </summary>
        bool CanDragToData { get; set; }
        /// <summary>
        /// Indicates whether this field is formula field
        /// </summary>
        bool IsFormulaField { get; }
        /// <summary>
        /// Specifies the formula for the calculated field
        /// </summary>
        string Formula { get; set; }
        /// <summary>
        /// Pivot Filter collections
        /// </summary>
        IPivotFilters PivotFilters { get; }
        /// <summary>
        /// Pivot field items
        /// </summary>
        IPivotFieldItems Items { get; }
        /// <summary>
        /// Gets/Sets the position of the field (first,second,third and so on) among all the fields in its Axis (Row,Column,Page,Data).
        /// </summary>
        int Position { get; set; }
        /// <summary>
        /// Indicates whether excluded or included items should be tracked when manual filtering is applied to pivot field.
        /// </summary>
        bool IncludeNewItemsInFilter { get; set; }
    }
}

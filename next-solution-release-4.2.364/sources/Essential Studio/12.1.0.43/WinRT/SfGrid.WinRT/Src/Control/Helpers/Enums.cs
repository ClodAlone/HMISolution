#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.UI.Xaml.Grid
{
    [ClassReference(IsReviewed = false)]
    public enum RowRegion
    {
        Header,
        Footer,
        Body
    }

    /// <summary>
    /// Used to specify how columns are auto generated. 
    /// </summary>
    public enum AutoGenerateColumnsMode 
    {
        /// <summary>
        /// Creates columns for all fields in a datasource, Retain the columns added by the user.
        /// </summary>
        Reset,
        /// <summary>
        /// Creates columns for all fields in a datasource if the grid doesn't contain any columns else retain old colmns.
        /// </summary>
        RetainOld,
        /// <summary>
        /// Creates columns for all fields in a datasource, and removing all old columns. 
        /// </summary>
        ResetAll,
        /// <summary>
        /// Keep the columns that are defined in DataGrid.Columns
        /// </summary>
        None
    }

    public enum ClipBoardAction
    {
        Copy,
        Cut,
        Paste
    }

    public enum GridValidationMode
    {
#if WPF || SILVERLIGHT
        InEdit,
#endif
        InView,
        None,
    }

    public enum GridCopyPasteOption
    {
        IncludeHeaders,

        CopyData,

        PasteData,

        CutData
    }

    [ClassReference(IsReviewed = false)]
    public enum SortClickAction
    {
        SingleClick,
        DoubleClick
    }

    [ClassReference(IsReviewed = false)]
    public enum ActivationTrigger
    {
        Mouse,
        Touch,
        Pen,
        Keyboard,
        Program
    }

    [ClassReference(IsReviewed = false)]
    public enum EditTrigger
    {
        OnTap,
        OnDoubleTap
    }

    #region Selection

    /// <summary>
    /// Selection mode enum.
    /// </summary>
    /// <remarks></remarks>
    [ClassReference(IsReviewed = false)]
    public enum GridSelectionMode
    {
        /// <summary>
        /// No items can be selected.
        /// </summary>
        /// <remarks></remarks>
        None,

        /// <summary>
        /// Single selection is maintained. By clicking the corresponding row selection will happened.
        /// If you click on the other rows old selection will be cleared.
        /// </summary>
        /// <remarks></remarks>
        Single,

        /// <summary>
        /// Multiple items can be selected.
        /// </summary>
        /// <remarks></remarks>
        Multiple,

        /// <summary>
        /// Multiple items can be selected, by using the SHIFT, CTRL, and
        /// arrow keys to make selections
        /// </summary>
        /// <remarks></remarks>
        Extended
    }

    [ClassReference(IsReviewed = false)]
    public enum FilterMode
    {
        CheckboxFilter,
        AdvancedFilter,
        Both
    }

    [ClassReference(IsReviewed = false)]
    public enum FilteredFrom
    {
        CheckboxFilter,
        AdvancedFilter,
        None
    }

    [ClassReference(IsReviewed = false)]
    public enum AdvancedFilterType
    {
        TextFilter,
        NumberFilter,
        DateFilter
    }

    [ClassReference(IsReviewed = false)]
    public enum PointerOperation
    {
        Pressed,
        Released,
        Tapped,
        DoubleTapped,
#if !WP && !WinRT
        Wheel
#endif
    }

    public enum GridOperation
    {
        Sorting,
        Filtering,
        Grouping,
        Paste,
        Paging,
        AddNewRow,
        TableSummary,
        FilterPopupOpening
    }

    #endregion
    /// <summary>
    /// ColumnSizer Enum
    /// </summary>
    /// <remarks></remarks>
    [ClassReference(IsReviewed = false)]
    public enum GridLengthUnitType
    {
        //// Summary:
        //// No Sizing
        None = 0,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells and the column header.
        Auto,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells and the column header with Last column fill.
        AutoWithLastColumnFill,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the cells.
        SizeToCells,
        ////
        //// Summary:
        ////     The unit of measure is based on the size of the column header.
        SizeToHeader,
        ////
        //// Summary:
        ////     The unit of measure is a weighted proportion of the available space.
        Star,

    }

    /// <summary>
    /// Bound for Height and Width Enum
    /// </summary>
    /// <remarks></remarks>
    [ClassReference(IsReviewed = false)]
    public enum GridQueryBounds
    {
        /// <summary>
        /// Queries height of cell.
        /// </summary>
        Height,

        /// <summary>
        /// Queries width of cell.
        /// </summary>
        Width
    }

    [ClassReference(IsReviewed = false)]
    public enum ExpressionError
    {
        None,
        MissingRightQuote,
        MismatchedParentheses,
        CannotCompareDifferentTypes,
        UnknownOperator,
        NotAValidFormula,
        ExceptionRaised
    }

    /// <summary>
    /// RowType enum
    /// </summary>
    /// <remarks></remarks>
    [ClassReference(IsReviewed = false)]
    public enum RowType
    {
        CaptionRow,
        CaptionCoveredRow,
        SummaryRow,
        SummaryCoveredRow,
        TableSummaryRow,
        TableSummaryCoveredRow,
        DefaultRow
    }

    [ClassReference(IsReviewed = false)]
    public enum IndentColumnType
    {
        BeforeExpander,
        AfterExpander,
        InExpanderExpanded,
        InExpanderCollapsed,
        InHeader,
        InLastGroupRow,
        InSummaryRow,
        InTableSummaryRow,
        InDataRow
    }

    [ClassReference(IsReviewed = false)]
    public enum SelectionReason
    {
        KeyPressed,
        PointerPressed,
        PointerReleased,
        SelectedItemsChanged,
        CollectionChanged
    }

    [ClassReference(IsReviewed = false)]
    public enum CollectionChangedReason
    {
        RecordCollectionChanged,
        SourceCollectionChanged,
        SelectedItemsCollection,
        ColumnsCollection,
        DataReorder
    }

    [ClassReference(IsReviewed = false)]
    public enum NavigationMode
    {
        Cell,
        Row
    }

    public enum MoveDirection
    {
        Right,
        Left
    }

#if WPF

    public enum ContextMenuType
    {
        RecordCell,
        Header,
        GroupDropAreaItem,
        GroupSummary,
        GroupCaption,
        TableSummary,
        GroupDropArea
    }

#endif

    public enum AddNewRowPosition
    {
        None,
        Top,
        Bottom
    }


#if !WP
    public enum PrintScaleOptions
    {
        NoScaling,
        FitViewonOnePage,
        FitAllColumnsonOnePage,
        FitAllRowsonOnePage
    }

    public enum EditorSelectionBehavior
    {
        SelectAll,
        MoveLast
    }
#endif
    
    [ClassReference(IsReviewed = false)]
    public enum GridRegion
    {
        GroupDropArea,
        Header,
        Grid,
#if !WP
        ColumnChooser,
#endif
        None
    }

    [ClassReference(IsReviewed = false)]
    public enum TableSummaryRowPosition
    {
        Bottom,
        Top
    }

    [ClassReference(IsReviewed = false)]
    public enum TableSummaryRowType
    {
        HeaderSummaryRow,
        FooterSummaryRow,
        LastFooterSummaryRow
    }
}

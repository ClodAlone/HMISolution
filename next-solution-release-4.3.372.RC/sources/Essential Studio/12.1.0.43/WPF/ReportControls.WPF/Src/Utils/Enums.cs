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

namespace Syncfusion.RDL.Internal
{
    internal enum ImageFormats
    {
        Bmp,
        Jpeg,
        Gif,
        Png,
        Emf
    }

    internal enum LayoutUpdateMode
    {
        All,
        Header,
        Footer,
        Body
    }

    internal enum UpdateSection
    {
        Header,
        Footer,
        Body,
        Container
    }

    internal enum DataObjectType
    {
        Default,
        Image,
        List
    }
   
    internal enum ExpressionError
    {
        None,
        MissingRightQuote,
        MismatchedParentheses,
        CannotCompareDifferentTypes,
        UnknownOperator,
        NotAValidFormula,
        ExceptionRaised
    }

    #region TablixCellType enumeration

    /// <summary>
    /// Enumerates the possible tablix cell types.
    /// </summary>
    [Flags]
    internal enum TablixCellType
    {
        /// <summary>
        /// Cell holds a summary value.
        /// </summary>
        ValueCell = 1,
        /// <summary>
        /// Cell is a row/column header that holds an expander.
        /// </summary>
        ExpanderCell = 2,
        /// <summary>
        /// Cell is a non-expander row or column header.
        /// </summary>
        HeaderCell = 4,
        /// <summary>
        /// Cell is the top left portion of the tablix table.
        /// </summary>
        TopLeftCell = 8,
        /// <summary>
        /// Cell is a row/column header that marks a total row or column.
        /// </summary>
        TotalCell = 16,
        /// <summary>
        /// Cell is a header cell holding a calculation name.
        /// </summary>
        CalculationHeaderCell = 32,
        /// <summary>
        /// Cell is row header
        /// </summary>
        RowHeaderCell = 64,
        /// <summary>
        /// Cell is column header
        /// </summary>
        ColumnHeaderCell = 128,
        /// <summary>
        /// Cell is a grand total cell.
        /// </summary>
        GrandTotalCell = 256 //,
        //MarkA = 512,
        // MarkB = 1024,
        //MarkC = 2048,
        // MarkD = 4096,
        // MarkE = 8192
    }

    #endregion

    internal enum FieldTypes
    {
        Property,
        Expression,
        Unbound
    }
}

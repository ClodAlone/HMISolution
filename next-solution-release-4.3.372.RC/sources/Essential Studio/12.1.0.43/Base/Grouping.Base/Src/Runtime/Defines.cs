//-------------------------------------------------------------------------------------------------
// <copyright file="Defines.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Defines various kinds of elements:
    /// </summary>
    public enum DisplayElementKind
    {
        /// <summary>
        /// Represents None.
        /// </summary>
        None,

        /// <summary>
        /// A Caption bar or section.
        /// </summary>
        Caption,

        /// <summary>
        /// A Record or Record Row.
        /// </summary>
        Record,

        /// <summary>
        /// An AddNew record.
        /// </summary>
        AddNewRecord,

        /// <summary>
        /// A Filter bar.
        /// </summary>
        FilterBar,

        /// <summary>
        /// A Column header row or section.
        /// </summary>
        ColumnHeader,

        /// <summary>
        /// A stacked header row or section.
        /// </summary>
        StackedHeader,

        /// <summary>
        /// A Nested table.
        /// </summary>
        NestedTable,

        /// <summary>
        /// A Summary row or section.
        /// </summary>
        Summary,

        /// <summary>
        /// A Table element.
        /// </summary>
        Table,

        /// <summary>
        /// A Group header section.
        /// </summary>
        GroupHeader,

        /// <summary>
        /// A Group footer section.
        /// </summary>
        GroupFooter,

        /// <summary>
        /// A Record preview row.
        /// </summary>
        RecordPreview,

        /// <summary>
        /// A Group preview row.
        /// </summary>
        GroupPreview,

        /// <summary>
        /// An Empty section.
        /// </summary>
        Empty
    }
}
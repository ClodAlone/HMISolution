//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableCellStyleEvents.cs" company="syncfusion">
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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Grouping;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Represents a method that handles an event with <see cref="GridTableCellStyleInfoChangedEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void GridTableCellStyleInfoChangedEventHandler(object sender, GridTableCellStyleInfoChangedEventArgs e);

    /// <summary>
    /// Provides information about <see cref="GridTableCellAppearance.Changed"/> and <see cref="GridTableCellAppearance.Changing"/>
    /// events raised by <see cref="GridTableCellAppearance"/> which occur when a style property of a <see cref="GridTableCellStyleInfo"/>
    /// is changed.
    /// </summary>
    public sealed class GridTableCellStyleInfoChangedEventArgs : SyncfusionEventArgs
    {
        GridTableCellAppearance tableCellAppearance;
        GridTableCellType tableCellType;
        EventArgs inner;

        /// <overload>
        /// Initializes a new object with the <see cref="GridTableCellAppearance"/> that the style object belongs to and the <see cref="GridTableCellType"/> that uniquely identifies the <see cref="GridTableCellStyleInfo"/>.
        /// </overload>
        /// <summary>
        /// Initializes a new object with the <see cref="GridTableCellAppearance"/> that the style object belongs to and the <see cref="GridTableCellType"/> that uniquely identifies the <see cref="GridTableCellStyleInfo"/>.
        /// </summary>
        /// <param name="tableCellAppearance">The parent <see cref="GridTableCellAppearance"/> that owns the <see cref="GridTableCellStyleInfo"/> object.</param>
        /// <param name="tableCellType">The <see cref="GridTableCellType"/> that uniquely identifies the <see cref="GridTableCellStyleInfo"/>
        /// with the <see cref="GridTableCellAppearance"/> that owns it.</param>
        public GridTableCellStyleInfoChangedEventArgs(GridTableCellAppearance tableCellAppearance, GridTableCellType tableCellType)
        {
            this.tableCellAppearance = tableCellAppearance;
            this.tableCellType = tableCellType;
        }

        /// <summary>
        /// Initializes a new object with the <see cref="GridTableCellAppearance"/> that the style object belongs to and the <see cref="GridTableCellType"/> that uniquely identifies the <see cref="GridTableCellStyleInfo"/>.
        /// </summary>
        /// <param name="tableCellAppearance">The parent <see cref="GridTableCellAppearance"/> that owns the <see cref="GridTableCellStyleInfo"/> object.</param>
        /// <param name="tableCellType">The <see cref="GridTableCellType"/> that uniquely identifies the <see cref="GridTableCellStyleInfo"/>
        /// with the <see cref="GridTableCellAppearance"/> that owns it.</param>
        /// <param name="inner">An inner eventargs.</param>
        public GridTableCellStyleInfoChangedEventArgs(GridTableCellAppearance tableCellAppearance, GridTableCellType tableCellType, EventArgs inner)
        {
            this.tableCellAppearance = tableCellAppearance;
            this.tableCellType = tableCellType;
            this.inner = inner;
        }

        /// <summary>
        /// The parent <see cref="GridTableCellAppearance"/> that owns the <see cref="GridTableCellStyleInfo"/> object.
        /// </summary>
        [TraceProperty(true)]
        public GridTableCellAppearance TableCellAppearance
        {
            get
            {
                return tableCellAppearance;
            }
        }

        /// <summary>
        /// The <see cref="GridTableCellType"/> that uniquely identifies the <see cref="GridTableCellStyleInfo"/>
        /// with the <see cref="GridTableCellAppearance"/> that owns it.
        /// </summary>
        [TraceProperty(true)]
        public GridTableCellType TableCellType
        {
            get
            {
                return tableCellType;
            }
        }

        /// <summary>
        /// An inner eventargs.
        /// </summary>
        [TraceProperty(true)]
        public EventArgs Inner
        {
            get
            {
                return this.inner;
            }
        }
    }

    ////eva GridTableCellStyleInfo SyncfusionHandled GridTableCellStyleInfoIdentity tableCellIdentity GridTableCellStyleInfo style StyleInfoProperty sip

    /// <summary>
    /// Represents a method that handles an event with <see cref="GridTableCellStyleInfoEventArgs"/>.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void GridTableCellStyleInfoEventHandler(object sender, GridTableCellStyleInfoEventArgs e);

    /// <summary>
    /// Provides information about <see cref="GridEngine.QueryCellStyleInfo"/>
    /// events raised by <see cref="GridEngine"/> and <see cref="GridTableDescriptor"/> which occur
    /// for each cell before a grid
    /// starts painting and lets users customize the display of cells.
    /// </summary>
    public sealed class GridTableCellStyleInfoEventArgs : SyncfusionHandledEventArgs
    {
        GridTableCellStyleInfoIdentity tableCellIdentity;
        GridTableCellStyleInfo style;
        StyleInfoProperty sip;

        /// <summary>
        /// Initializes the object with GridTableCellStyleInfoIdentity, GridTableCellStyleInfo and the StyleInfoProperty that was modified.
        /// </summary>
        /// <param name="tableCellIdentity">The identity object.</param>
        /// <param name="style">The style object.</param>
        /// <param name="sip">Specifies the property that is changed.</param>
        public GridTableCellStyleInfoEventArgs(GridTableCellStyleInfoIdentity tableCellIdentity, GridTableCellStyleInfo style, StyleInfoProperty sip)
        {
            this.tableCellIdentity = tableCellIdentity;
            this.style = style;
            this.sip = sip;
        }

        /// <summary>
        /// The identity object.
        /// </summary>
        [TraceProperty(true)]
        public GridTableCellStyleInfoIdentity TableCellIdentity
        {
            get
            {
                return tableCellIdentity;
            }
        }

        /// <summary>
        /// The style object.
        /// </summary>
        [TraceProperty(true)]
        public GridTableCellStyleInfo Style
        {
            get
            {
                return style;
            }
        }

        /// <summary>
        /// Specifies the property that is changed.
        /// </summary>
        [TraceProperty(true)]
        public StyleInfoProperty Sip
        {
            get
            {
                return sip;
            }
        }
    }
}

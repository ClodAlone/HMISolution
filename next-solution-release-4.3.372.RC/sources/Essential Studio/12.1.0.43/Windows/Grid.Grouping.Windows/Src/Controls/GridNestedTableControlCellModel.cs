//-------------------------------------------------------------------------------------------------
// <copyright file="GridNestedTableControlCellModel.cs" company="syncfusion">
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
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// GridNestedTableControlCellModel draws a TableControl. It has a GridTableModel with a table.
    /// Within the table, the FilteredChildTable will be set before the cell is drawn or accessed.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class. A nested table control cell model
    /// is identified through its parent relations name with an "RT" prefix.
    /// </remarks>
    /// <example>
    /// <code lang="C#">
    ///             string cellType = "RT" + relatedTable.TableDescriptor.Name;
    ///             GridNestedTableControlCellModel cm = this.CellModels[cellType] as GridNestedTableControlCellModel;
    /// </code>
    /// <code lang="VB">
    /// <para/>
    ///         Dim cellType As String = "RT" + relatedTable.TableDescriptor.Name
    ///         Dim cm As GridNestedTableControlCellModel = Me.CellModels(cellType)
    /// </code>
    /// </example>
    public class GridNestedTableControlCellModel : GridStaticCellModel
    {
        private GridTableModel tableModel;
        private GridTable table;

        /// <summary>
        /// Initializes a new <see cref="GridNestedTableControlCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridNestedTableControlCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ForceCoveredCellFullBounds = false;
        }

        /// <overload>
        /// Initializes a new <see cref="GridNestedTableControlCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridNestedTableControlCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> and <see cref="GridTable"/>
        /// this cell model belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <param name="table">The <see cref="GridTable"/>.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class. A nested table control cell model
        /// is identified through its parent relations name with an "RT" prefix.
        /// </remarks>
        /// <example>
        /// <code lang="C#">
        ///             string cellType = "RT" + relatedTable.TableDescriptor.Name;
        ///             GridNestedTableControlCellModel cm = this.CellModels[cellType] as GridNestedTableControlCellModel;
        /// </code>
        /// <code lang="VB">
        /// <para/>
        ///         Dim cellType As String = "RT" + relatedTable.TableDescriptor.Name
        ///         Dim cm As GridNestedTableControlCellModel = Me.CellModels(cellType)
        /// </code>
        /// </example>
        public GridNestedTableControlCellModel(GridModel grid, GridTable table)
            : base(grid)
        {
            AllowFloating = false;
            this.table = table;
            ForceCoveredCellFullBounds = false;
            this.tableId = ++tableCounter;
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(this.Grid.Table.ToString(), table.ToString(), tableId);
            }
        }

        /// <override/>
        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            string isdisposed = IsDisposed ? ", Disposed" : string.Empty;
            return GetType().Name + " { " + this.table.ToString() + "(" + this.tableId + isdisposed + ") }";
        }

        static int tableCounter = 0;
        int tableId = 0;

#if ASPNET
#else
        /// <override/>
        /// <summary>
        /// Creates a cell renderer for this cell model.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>returns the Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridNestedTableControlCellRenderer(control, this);
        }
#endif
        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (Engine.VerboseEnsureObjectLifeTime)
            {
                TraceUtil.TraceCurrentMethodInfo(this.tableId);
            }

            if (disposing)
            {
                if (this.tableModel != null)
                {
                    this.tableModel.QueryCellModel -= new GridQueryCellModelEventHandler(tableModel_QueryCellModel);
                    this.tableModel.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Returns the <see cref="GridTableModel"/> that is used by the
        /// <see cref="GridNestedTableControlCellRenderer.Control"/> (a <see cref="GridNestedTableControl"/>) of a
        /// <see cref="GridNestedTableControlCellRenderer"/>
        /// of this model.
        /// </summary>
        public GridTableModel RelatedTableModel
        {
            get
            {
                if (this.tableModel == null)
                {
                    this.tableModel = new GridTableModel();
                    this.tableModel.Table = this.table;
                    this.tableModel.Properties = this.Grid.Model.Properties;
                    this.tableModel.SetOptions(this.Grid.Options);
                    this.tableModel.QueryCellModel += new GridQueryCellModelEventHandler(tableModel_QueryCellModel);
                    this.table.Disposed += new EventHandler(table_Disposed);
                }

                return this.tableModel;
            }
        }

        /// <summary>
        /// Returns the <see cref="GridTable"/> to be displayed in the
        /// <see cref="GridNestedTableControlCellRenderer.Control"/> (a <see cref="GridNestedTableControl"/>) of a
        /// <see cref="GridNestedTableControlCellRenderer"/>
        /// of this model.
        /// </summary>
        public GridTable RelatedTable
        {
            get
            {
                return this.table;
            }
        }

        /// <summary>
        /// Gets / sets the <see cref="ChildTable"/> that should be displayed in the
        /// <see cref="GridNestedTableControlCellRenderer.Control"/> (a <see cref="GridNestedTableControl"/>) of a
        /// <see cref="GridNestedTableControlCellRenderer"/>
        /// of this model.
        /// </summary>
        public ChildTable FilteredChildTable
        {
            get
            {
                return this.RelatedTableModel.FilteredChildTable;
            }

            set
            {
                this.RelatedTableModel.FilteredChildTable = value;
            }
        }

        /// <summary>
        /// Returns the parent <see cref="GridTableModel"/> that owns this cell model.
        /// </summary>
        public new GridTableModel Grid
        {
            get
            {
                return (GridTableModel) base.Grid;
            }
        }

        private void tableModel_QueryCellModel(object sender, GridQueryCellModelEventArgs e)
        {
            if (e.CellModel == null)
            {
                if (!e.CellType.StartsWith("RT"))
                {
                    GridModel parentTableModel = this.Grid;
                    e.CellModel = parentTableModel.CellModels[e.CellType].CreateCopy(parentTableModel);
                }
            }
        }

        private void table_Disposed(object sender, EventArgs e)
        {
            Table table = (Table) sender;
            table.Disposed -= new EventHandler(this.table_Disposed);
        }
    }
}

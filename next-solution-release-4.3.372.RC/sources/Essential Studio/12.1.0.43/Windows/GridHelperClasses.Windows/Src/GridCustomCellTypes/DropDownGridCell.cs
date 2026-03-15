//-------------------------------------------------------------------------------------------------
// <copyright file="DropDownGridCell.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Text;
    using System.Runtime.Serialization;

    using Syncfusion.Diagnostics;

    using Syncfusion.Windows.Forms;
    using Syncfusion.Windows.Forms.Grid;

    /// <summary>
    /// Implements a data model for DropDownGrid cell.
    /// </summary>
    public class DropDownGridCellModel
        : GridDropDownGridCellModel
    {
        private GridControlBase _embbeddedGrid;

        /// <summary>
        /// Gets or sets the grid to be embedded in the cell.
        /// </summary>
        public GridControlBase EmbeddedGrid
        {
            get
            {
                if (this._embbeddedGrid == null)
                {
                    this._embbeddedGrid = new GridControlBaseImp();
                }

                return this._embbeddedGrid;
            }

            set
            {
                this._embbeddedGrid = value;
            }
        }
        /// <summary>
        /// DropDownGridCellModel 
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected DropDownGridCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Constructor for DropDownGridCellModel.
        /// </summary>
        /// <param name="grid">The grid model.</param>
        public DropDownGridCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Creates renderer.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new DropDownGridCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Implements a renderer for DropDownGrid cell.
    /// </summary>
    public class DropDownGridCellRenderer : GridDropDownGridCellRenderer
    {
        GridControlBase grid;

        /// <summary>
        /// Constructor for DropDownGridCellRenderer.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        public DropDownGridCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.DisableTextBox = true;
            DropDownButton = new GridCellComboBoxButton(this);
            this.grid = null;
        }
        /// <summary>
        /// Is called when initializing the DropDownGridCell
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        protected override void OnInitialize(int rowIndex, int colIndex)
        {
            this.grid  = ((DropDownGridCellModel)this.Model).EmbeddedGrid;
            this.grid.Dock = DockStyle.Fill;
            base.OnInitialize(rowIndex, colIndex);
        }
        /// <summary>
        /// InitializeDropDownContainer
        /// </summary>
        protected override void InitializeDropDownContainer()
        {
            base.InitializeDropDownContainer();

            // this.DropDownContainer.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.DropDownContainer.IgnoreDialogKey = true;
        }
        /// <summary>
        /// Creates ann inner control to the grid
        /// </summary>
        /// <param name="grid">GridControlBase</param>
        /// <returns>grid</returns>
        protected override Control CreateInnerControl(out GridControlBase grid)
        {
            grid = this.grid;
            grid.Dock = DockStyle.Fill;
            grid.CausesValidation = false;
            return grid;
        }
    }
}

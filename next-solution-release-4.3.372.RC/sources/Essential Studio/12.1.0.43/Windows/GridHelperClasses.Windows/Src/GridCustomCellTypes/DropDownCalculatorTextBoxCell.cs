//-------------------------------------------------------------------------------------------------
// <copyright file="DropDownCalculatorTextBoxCell.cs" company="Syncfusion">
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
    using System.Collections.Generic;
    using System.Text;
    using System.Drawing;
    using System.ComponentModel;
    using System.Runtime.Serialization;   
    using System.Windows.Forms;

    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms.Tools;
    using Syncfusion.Styles;
    using Syncfusion.Windows.Forms;

    #region Cell Model
    /// <summary>
    /// Implements a data model for the DropDownCalculator textbox cell.
    /// </summary>
    public class DropDownCalculatorTextBoxCellModel : GridStaticCellModel
    {
        /// <summary>
        /// Constructor for DropDownCalculatorTextBoxCellModel class
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected DropDownCalculatorTextBoxCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Constructor for DropDownCalculatorTextBoxCellModel.
        /// </summary>
        /// <param name="grid">The grid model.</param>
        public DropDownCalculatorTextBoxCellModel(GridModel grid)
            : base(grid)
        {
            ButtonBarSize = new Size(14, 20);
        }

        /// <summary>
        /// Creates renderer.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new DropDownCalculatorTextBoxCellRenderer(control, this);
        }
        /// <summary>
        /// OnQueryPrefferedClientSize
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        /// <param name="queryBounds">GridQueryBounds</param>
        /// <returns></returns>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size s = base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
            s.Width = s.Width + ButtonBarSize.Width;
            return s;
        }
    }
    #endregion

    #region Cell Renderer
    class DropDownCalculatorTextBoxCellRenderer : GridStaticCellRenderer
    {
        internal CalculatorControl calci;

        public DropDownCalculatorTextBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            DropDownPart = new GridDropDownCellImp(this);
            DropDownButton = new GridCellComboBoxButton(this);
        }

        void calci_ValueCalculated(object sender, CalculatorValueCalculatedEventArgs arg)
        {
            GridCurrentCell cc = this.Grid.CurrentCell;
            this.Grid.Model[cc.RowIndex, cc.ColIndex].CellValue = this.calci.Value;
        }

        public new GridDropDownContainer DropDownContainer
        {
            get { return (GridDropDownContainer)base.DropDownContainer; }
        }

        public override void DropDownContainerShowingDropDown(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GridStyleInfo style = this.Grid.Model[RowIndex, ColIndex];
            this.calci = style.Control as CalculatorControl;
            this.calci.ShowDisplayArea = false;
            this.calci.Size = new Size(260, 180);

            if (this.DropDownContainer != null)
            {
                this.DropDownContainer.Controls.Add(this.calci);
            }

            this.DropDownContainer.Size = this.calci.Size;
            this.calci.ValueCalculated += new CalculatorValueCalculatedEventHandler(this.calci_ValueCalculated);
        }

        protected override bool OnSaveChanges()
        {
            Grid.Model[RowIndex, ColIndex].CellValue = this.calci.Value;
            return true;
        }

        public override void DropDownContainerCloseDropDown(object sender, Syncfusion.Windows.Forms.PopupClosedEventArgs e)
        {
            this.calci.ValueCalculated -= new CalculatorValueCalculatedEventHandler(this.calci_ValueCalculated);
            this.DropDownContainer.Controls.Remove(this.calci);
        }
    }
    #endregion
}

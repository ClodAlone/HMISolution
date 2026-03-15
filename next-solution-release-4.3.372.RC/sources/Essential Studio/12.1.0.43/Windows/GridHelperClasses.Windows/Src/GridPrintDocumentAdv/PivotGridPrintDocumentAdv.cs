//-------------------------------------------------------------------------------------------------
// <copyright file="PivotGridPrintDocumentAdv.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 20012 - 2017. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.ComponentModel;
using Syncfusion.Windows.Forms.PivotAnalysis;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.PivotAnalysis.Base;

namespace Syncfusion.GridHelperClasses
{
    /// <summary>
    ///    Implements printing support for a PivotGridControl.
    /// </summary>
    [ToolboxItem(false)]
    public class PivotGridPrintDocumentAdv : GridPrintDocumentAdv
    {
        PivotGridControl m_grid;
        /// <summary>
        /// Class used for performing printing operation
        /// </summary>
        /// <param name="grid">GridControlBase</param>
        public PivotGridPrintDocumentAdv(GridControlBase grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridPrintDocument"/> for a grid.
        /// </summary>
        /// <param name="grid">The parent grid for this object.</param>
        public PivotGridPrintDocumentAdv(PivotGridControl grid)
            : this(grid.TableControl)
        {
            m_grid = grid;
            grid.TableModel.Properties.PrintRowHeader = false;
            grid.TableModel.Properties.PrintColHeader = false;
        }

        /// <summary>
        /// Represents the methods that handles <see cref="DrawCustomHeaderStyleEventHandler"/> and <see cref="DrawCustomHeaderStyleEventHandler"/> events
        /// </summary>
        /// <param name="sender">The source of the event</param>
        /// <param name="e">A <see cref="GridPrintHeaderFooterTemplateArgs"/> that contains the event data</param>
        public delegate void DrawCustomHeaderStyleEventHandler(object sender, DrawCustomHeaderStyleArgs e);

        /// <summary>
        /// Handle this event to draw Header to the Grid print document
        /// </summary>
        public event DrawCustomHeaderStyleEventHandler DrawCustomHeaderStyle;

        /// <summary>
        /// Raises the <see cref="OnDrawHeaderStyle"/> event
        /// </summary>
        /// <param name="e">A <see cref="GridPrintHeaderFooterTemplateArgs"/>that contains the event data</param>
        protected virtual void OnDrawHeaderStyle(DrawCustomHeaderStyleArgs e)
        {
            if (this.DrawCustomHeaderStyle != null)
            {
                this.DrawCustomHeaderStyle(this, e);
            }
        }

        ////Override the OnPrintPage to provide the printing logic for the document.
        /// <override/>
        protected override void OnPrintPage(PrintPageEventArgs ev)
        {
            if (m_grid is PivotGridControl)
            {
                GridStyleInfo cellstyle = new GridStyleInfo();

                DrawCustomHeaderStyleArgs args = new DrawCustomHeaderStyleArgs(BrushInfo.Empty);
                this.OnDrawHeaderStyle(args);

                m_grid.TableControl.BeginUpdate();
                for (int i = 0; i <= m_grid.TableControl.Model.RowCount; i++)
                {
                    for (int j = 0; j <= m_grid.TableControl.Model.ColCount; j++)
                    {
                        PivotCellInfo pivotcell = m_grid.PivotEngine[i, j];

                        if (pivotcell != null)
                        {
                            if (pivotcell.CellType == (PivotCellType.HeaderCell | PivotCellType.ColumnHeaderCell)
                                || pivotcell.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell)
                                || pivotcell.CellType == (PivotCellType.CalculationHeaderCell | PivotCellType.ColumnHeaderCell)
                                || pivotcell.CellType == (PivotCellType.ExpanderCell | PivotCellType.ColumnHeaderCell)
                                || pivotcell.CellType == (PivotCellType.ExpanderCell | PivotCellType.RowHeaderCell)
                                || pivotcell.CellType == (PivotCellType.TotalCell | PivotCellType.ColumnHeaderCell)
                                || pivotcell.CellType == (PivotCellType.TotalCell | PivotCellType.RowHeaderCell))
                            {
                                cellstyle = m_grid.TableControl.Model[i + 1, j + 1];
                                if (args.Handled && args.Brush.Style != BrushStyle.None)
                                    cellstyle.Interior = args.Brush;
                                else
                                    cellstyle.Interior = GetBrush(m_grid, cellstyle);
                            }
                        }
                    }
                }
                m_grid.TableControl.EndUpdate(true);
            }
            base.OnPrintPage(ev);
        }

        /// <summary>
        /// Returns the modified BrushInfo for the respective GridVisualStyles for headers cells.
        /// </summary>
        /// <param name="grid">The PivotGridControl.</param>
        /// <param name="style">The cell style.</param>
        /// <returns>returns the BrushInfo</returns>
        internal BrushInfo GetBrush(PivotGridControl grid, GridStyleInfo style)
        {
            BrushInfo br = new BrushInfo();
            if ((style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && grid.TableControl.ThemesEnabled || ((style.Themed && grid.TableControl.ThemesEnabled) && ((grid.TableControl.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)))))
            {
                if (grid.TableControl.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                {
                    switch (grid.TableControl.Model.Options.GridVisualStyles)
                    {
                        case GridVisualStyles.Office2007Blue:
                            br = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(249, 252, 255), Color.FromArgb(197, 222, 255));
                            break;
                        case GridVisualStyles.Office2007Black:
                            br = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(248, 248, 248), Color.FromArgb(223, 223, 223));
                            break;
                        case GridVisualStyles.Office2007Silver:
                            br = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(241, 243, 243), Color.FromArgb(200, 201, 202));
                            break;
                        case GridVisualStyles.Office2010Blue:
                            br = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(241, 245, 249), Color.FromArgb(218, 231, 245));
                            break;
                        case GridVisualStyles.Office2010Black:
                            br = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(106, 106, 106), Color.FromArgb(89, 89, 89));
                            style.TextColor = Color.White;
                            break;
                        case GridVisualStyles.Office2010Silver:
                            br = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(223, 227, 232), Color.FromArgb(183, 188, 193));
                            break;
                        case GridVisualStyles.Metro:
                            br = new BrushInfo(GradientStyle.None, Color.FromArgb(92,92,92), Color.FromArgb(246, 247, 247));
                            style.TextColor = Color.FromArgb(92, 92, 92);
                            break;
                        default:
                            br = new BrushInfo(GradientStyle.None, Color.FromArgb(255, 255, 255), Color.FromArgb(197, 222, 255));
                            break;
                    }
                }
            }
            return br;
        }
    }

    /// <summary>
    /// Provides data for the <see cref="DrawCustomHeaderStyleArgs"/> and <see cref="DrawCustomHeaderStyleArgs"/> events
    /// </summary>
    /// <remarks>To draw the Header / Footer for the Grid Print document, handle the <see cref="DrawCustomHeaderStyleArgs"/> / <see cref="DrawCustomHeaderStyleArgs"/> events
    /// </remarks>
    public class DrawCustomHeaderStyleArgs : SyncfusionHandledEventArgs
    {
        BrushInfo brush;
        /// <summary>
        /// Initializes a new <see cref="DrawCustomHeaderStyleArgs"/>
        /// </summary>
        /// <param name="brushInfo">Rectangle area to draw Header / Footer</param>
        public DrawCustomHeaderStyleArgs(BrushInfo brushInfo)
        {
            this.brush = brushInfo;
        }

        /// <summary>
        /// Sets the custom header style
        /// </summary>
        public BrushInfo Brush
        {
            get
            {
                return this.brush;
            }
            set
            {
                this.brush = value;
            }
        }
    }
}

//-------------------------------------------------------------------------------------------------
// <copyright file="GridInCell.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using Syncfusion.Diagnostics;
using Syncfusion.Schedule;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Schedule
{
    /// <summary>
    /// Used for the cell in the AllDay panel at the top of the calendar.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridInCellModel : GridGenericControlCellModel
    {
        protected GridInCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public GridInCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
        }

        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridInCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Used for the cell in the AllDay panel at the top of the calendar.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridInCellRenderer : GridGenericControlCellRenderer
    {
        private CellEmbeddedGrid activeGrid;

        public GridInCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.SupportsFocusControl = true;
        }

        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            if (this.ShouldDrawFocused(rowIndex, colIndex))
            {
                if (style.Control is CellEmbeddedGrid)
                {
                    activeGrid = (CellEmbeddedGrid)style.Control;
                }

                base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            }
            else
            {
                // Draw a static grid
                if (style.Control is CellEmbeddedGrid)
                {
                    CellEmbeddedGrid grid = (CellEmbeddedGrid)style.Control;

                    grid.Size = clientRectangle.Size;
                    grid.DefaultColWidth = clientRectangle.Size.Width;

                    grid.DrawGrid(g, clientRectangle, true);
                }
            }
        }

        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            TraceUtil.TraceCurrentMethodInfo(m.ToString());

            // forward keyboard events to child grid that would otherwise 
            // be handled by parent grid (right arrow, page down etc.)
            if (activeGrid != null && activeGrid.Focused)
            {
                return activeGrid.InitiateProcessKeyEventArgs(ref m);
            }

            return base.ProcessKeyEventArgs(ref m);
        }
    }

    /// <summary>
    /// Used for the cell in the AllDay panel at the top of the calendar.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class CellEmbeddedGrid : GridControl
    {
        private ScheduleControl schedule;
        private int topm = 4;
        private int leftm = 10;
        private int intensity = 100;

        public int Intensity
        {
            get
            {
                return intensity;
            }

            set
            {
                intensity = value;
            }
        }

        public int LeftMargin
        {
            get { return leftm; }
            set { leftm = value; }
        }

        public int TopMargin
        {
            get { return topm; }
            set { topm = value; }
        }

        public CellEmbeddedGrid(ScheduleControl schedule)
        {
            this.ColWidths[0] = 0;
            this.Properties.DisplayHorzLines = false;
            this.Properties.BackgroundColor = Color.Pink;
            this.Model.Options.DefaultGridBorderStyle = GridBorderStyle.None;
            this.RowCount = 1;
            ////this.ColCount = 1;

            this.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
            this.VScrollBehavior = GridScrollbarMode.Disabled;
            this.HScrollBehavior = GridScrollbarMode.Disabled;
            this.BorderStyle = BorderStyle.None;
            this.Location = new Point(-10000, -10000);
            this.ActivateCurrentCellBehavior = GridCellActivateAction.ClickOnCell;
            this.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.WhenGridActive;
            this.schedule = schedule;
        }

        ////functionality that was in this override moved into ScheduleGrid.
        ////protected override void OnCellDrawn(GridDrawCellEventArgs e)

        internal bool InitiateProcessKeyEventArgs(ref Message m)
        {
            return base.ProcessKeyEventArgs(ref m);
        }
    }
}

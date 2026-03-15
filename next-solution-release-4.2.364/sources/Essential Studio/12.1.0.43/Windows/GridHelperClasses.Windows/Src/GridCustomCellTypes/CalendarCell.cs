//-------------------------------------------------------------------------------------------------
// <copyright file="CalendarCell.cs" company="Syncfusion">
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
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Windows.Forms;
    using System.IO;

    using Syncfusion.Windows.Forms;
    using Syncfusion.Windows.Forms.Grid;

    /// <summary>
    /// Implements a data model for a calendar cell.
    /// </summary>
    public class CalendarCellModel : GridGenericControlCellModel
    {
        /// <summary>
        /// Constructor of CalendarCellModel class.
        /// </summary>
        /// <param name="grid">The grid model.</param>
        public CalendarCellModel(GridModel grid)
            : base(grid)
        {
        }
        /// <summary>
        /// Constructor of CalendarCellModel class.
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected CalendarCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Creates the cell renderer.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>returns the GridCellRendererBase</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new CalendarCellRenderer(control, this);
        }

        static Size calendarControlSize = Size.Empty;

        /// <summary>
        /// Gets the size of calendar control.
        /// </summary>
        public static Size CalendarControlSize
        {
            get
            {
                if (calendarControlSize.IsEmpty)
                {
                    // Determine default size of MonthCalendar when shown in a form
                    TopLevelWindow f = new TopLevelWindow();
                    f.Location = new Point(10000, 10000);
                    f.Size = new Size(500, 500);
                    f.ShowWindowTopMost();
                    MonthCalendar drawCalendar = new MonthCalendar();
                    f.Controls.Add(drawCalendar);
                    f.Visible = true;
                    calendarControlSize = drawCalendar.Size;
                    f.Dispose();
                }

                return calendarControlSize;
            }
        }
        /// <summary>
        /// Calculates and returns a specified size
        /// </summary>
        /// <param name="g">Graphical parameter</param>
        /// <param name="rowIndex">Indicates the RowIndex</param>
        /// <param name="colIndex">Indicates the ColIndex</param>
        /// <param name="style">Indicates the Style Parameter</param>
        /// <param name="queryBounds">Sets up the bounds of the client</param>
        /// <returns></returns>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size size = CalendarControlSize;
            size.Width += 8;
            size.Height += 8;
            return size;
        }

        /// <summary>
        /// Initializes the calendar control.
        /// </summary>
        /// <param name="c">Calendar control.</param>
        /// <param name="style">Cell style.</param>
        public static void InitializeCalendar(MonthCalendar c, GridStyleInfo style)
        {
            InitializeCalendar(c, style.CellValue);
        }

        /// <summary>
        /// Initializes the calendar control.
        /// </summary>
        /// <param name="c">Calendar control.</param>
        /// <param name="controlValue">Value for the calendar.</param>
        public static void InitializeCalendar(MonthCalendar c, object controlValue)
        {
            if (controlValue is DateTime)
            {
                c.SetDate((DateTime)controlValue);
            }
            else if (controlValue is string && ((string)controlValue).Length > 0)
            {
                c.SetDate(DateTime.Parse((string)controlValue));
            }
            else
            {
                c.SetDate(DateTime.Today);
            }
        }
    }

    /// <summary>
    /// Implements the renderer for a Calendar cell.
    /// </summary>
    public class CalendarCellRenderer : GridGenericControlCellRenderer
    {
        MonthCalendar m_editCalendar;

        /// <summary>
        /// Constructor of CalendarCellRenderer.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        public CalendarCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            SupportsFocusControl = true;
        }
        /// <summary>
        /// Disposes the used object
        /// </summary>
        /// <param name="disposing">Decides whether to dispose the object or not</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.m_editCalendar != null)
                {
                    this.m_editCalendar.DateChanged -= new DateRangeEventHandler(this.calendar_DateChanged);

                    // Do not dispose m_editCalendar. Style takes care of this.
                    this.m_editCalendar = null;
                }
            }

            base.Dispose(disposing);
        }
        /// <summary>
        /// Saves the changes to the Calendar cells
        /// </summary>
        /// <returns>bool</returns>
        protected override bool OnSaveChanges()
        {
            this.Grid.Model[RowIndex, ColIndex].CellValue = this.m_editCalendar.SelectionStart;
            return true;
        }
        /// <summary>
        /// It is triggered when the cell is activated.
        /// </summary>
        protected override void OnActivated()
        {
            // Works around some interaction problem with PropertyGrid specific to this sample: force redrawing of
            // calendar after property grid was refreshed.
            Timer t = new Timer();
            t.Tick += new EventHandler(this.t_Tick);
            t.Interval = 200;
            t.Start();

            base.OnActivated();
        }

        private void t_Tick(object sender, EventArgs e)
        {
            Timer t = (Timer)sender;
            t.Tick += new EventHandler(this.t_Tick);
            t.Dispose();

            if (this.m_editCalendar != null)
            {
                this.m_editCalendar.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets active calendar for editing current cell
        /// </summary>
        MonthCalendar EditCalendar
        {
            get
            {
                return this.m_editCalendar;
            }

            set
            {
                MonthCalendar drawCalendar = value;
                if (!Object.ReferenceEquals(this.m_editCalendar, drawCalendar))
                {
                    if (this.m_editCalendar != null)
                    {
                        this.m_editCalendar.DateChanged -= new DateRangeEventHandler(this.calendar_DateChanged);
                    }

                    this.m_editCalendar = drawCalendar;
                    SetControl(this.m_editCalendar);

                    if (this.m_editCalendar != null)
                    {
                        this.m_editCalendar.DateChanged += new DateRangeEventHandler(this.calendar_DateChanged);
                    }
                }
            }
        }
        /// <summary>
        /// It is triggered each time when the cells are drawn
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="clientRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            clientRectangle.Size = GridUtil.Max(clientRectangle.Size, CalendarCellModel.CalendarControlSize);

            MonthCalendar drawCalendar = style.Control as MonthCalendar;

            if (drawCalendar != null)
            {
                if (this.ShouldDrawFocused(rowIndex, colIndex))
                {
                    this.EditCalendar = drawCalendar;
                }
                else
                {
                    CalendarCellModel.InitializeCalendar(drawCalendar, style);
                }
            }

            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlValue"></param>
        protected override void InitializeControlText(object controlValue)
        {
            MonthCalendar drawCalendar = StyleInfo.Control as MonthCalendar;

            if (drawCalendar != null)
            {
                this.EditCalendar = drawCalendar;
                CalendarCellModel.InitializeCalendar(this.m_editCalendar, controlValue);
            }
        }
        /// <summary>
        /// It is triggered when the focus in cell is changed
        /// </summary>
        protected override void OnHasFocusControlChanged()
        {
            if (!this.HasFocusControl)
            {
                if (this.m_editCalendar != null)
                {
                    Grid.SuspendLayout();
                    this.m_editCalendar.Location = new Point(10000, 10000);
                    Grid.ResumeLayout(false);
                }
            }

            base.OnHasFocusControlChanged();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        /// <param name="innerBounds">Rectangle</param>
        /// <param name="buttonsBounds">Rectangle</param>
        /// <returns>Rectangle</returns>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            Rectangle r = base.OnLayout(rowIndex, colIndex, style, innerBounds, buttonsBounds);

            r.Inflate(-1, -1);

            // center windows forms calendar inside cell.
            r = GridUtil.CenterInRect(r, CalendarCellModel.CalendarControlSize);
            r.Size = GridUtil.Max(r.Size, CalendarCellModel.CalendarControlSize);
            return r;
        }

        private void calendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            // Raise CurrentCellChanging event and set CurrentCell.Modified = true
            if (base.NotifyCurrentCellChanging())
            {
                ControlValue = this.m_editCalendar.SelectionStart;
                base.NotifyCurrentCellChanged();
            }
        }

        // We chose to provide "Calendar" controls in MainForm.cs:
        // The benefit of this is the controls will always be sticking with the cell
        // when the user inserts/remove or moves cells.
        /*
         *         for (int row = 2; row <= 6; row++)
            {
                if (row % 2 == 0)
                {
                    style = gridControl1[row, 2];
                    style.CellType = "Calendar";
                    style.Control = new MonthCalendar();
                    style = gridControl1[row, 3];
                    style.CellType = "Calendar";
                    style.Control = new MonthCalendar();
                }
            }
         */
    }
}

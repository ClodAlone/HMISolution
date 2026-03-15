//-------------------------------------------------------------------------------------------------
// <copyright file="EdgeCell.cs" company="syncfusion">
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
using Syncfusion.Schedule;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Schedule
{
    /// <summary>
    /// Used for the cell in on the edge of the navigation calendars.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class EdgeCellModel : GridGenericControlCellModel
    {
        protected EdgeCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public EdgeCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
        }

        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new EdgeCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Used for the cell in on the edge of the navigation calendars.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class EdgeCellRenderer : GridGenericControlCellRenderer
    {
        private GridControl activeGrid;
        internal NavigationCalendar navigationPanel;

        public EdgeCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.SupportsFocusControl = true;
            activeGrid = grid as GridControl;
        }

        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            bool ok = true;
            string s = " ";
            Color headerColor = Color.Empty;
            Color selectionColor = Color.Empty;
            Color weekNumberColor = Color.Empty;
            if (this.navigationPanel.scheduleGridGrid != null && this.navigationPanel.scheduleGridGrid.Schedule != null)
            {
                headerColor = this.navigationPanel.scheduleGridGrid.Schedule.Appearance.NavigationCalendarHeaderColor;
                selectionColor = this.navigationPanel.scheduleGridGrid.Schedule.Appearance.NavigationCalendarSelectionColor;
                weekNumberColor = this.navigationPanel.scheduleGridGrid.Schedule.Appearance.NavigationCalendarWeekNumberColor;
            }
            else
            {
                headerColor = this.navigationPanel.appearance.NavigationCalendarHeaderColor;
                selectionColor = this.navigationPanel.appearance.NavigationCalendarSelectionColor;
                weekNumberColor = this.navigationPanel.appearance.NavigationCalendarWeekNumberColor;
            }
            if (this.navigationPanel.ShowWeekNumbers && (colIndex == 1))
            {
                bool rtl = this.Grid.IsRightToLeft();
                    Rectangle r = clientRectangle;
                r.X = rtl ? r.X - 5: r.X + navigationPanel.weekNumberIndent;
                    r.Y += 1;
                    r.Width = r.Width - navigationPanel.weekNumberIndent + 1;
                    if (rowIndex % navigationPanel.rowsPerCal == 1 && this.navigationPanel.scheduleGridGrid != null)
                    {
                        using (Pen p = new Pen(headerColor))
                        {
                            g.DrawLine(p, r.Left, r.Bottom - 2, r.Right, r.Bottom - 2);
                        }

                        s = style.Text;
                    }
                    else if (rowIndex % navigationPanel.rowsPerCal != 0)
                    {
                        DateTime dt = (DateTime)style.CellValue;
                        int cal = rowIndex / navigationPanel.rowsPerCal;
                        if ((rowIndex % navigationPanel.rowsPerCal) > 1)
                        {
                            DateTime dtV = this.navigationPanel.DateValue.AddMonths(cal);
                            ok = !((cal == 0 && (100 * dt.Year) + dt.Month > (100 * dtV.Year) + dtV.Month)
                                || (cal > 0 && cal < this.navigationPanel.numberCalendars - 1 && dt.Month != dtV.Month)
                                || (cal == this.navigationPanel.numberCalendars - 1 && (100 * dt.Year) + dt.Month < (100 * dtV.Year) + dtV.Month));
                        }

                        if (this.navigationPanel.SelectedDates.IndexOf(dt) > -1)
                        {
                            ////mark if selected
                            if (ok)
                            {
                                using (Brush b1 = new SolidBrush(selectionColor))
                                {
                                    if (this.navigationPanel.scheduleGridGrid.IsRightToLeft())
                                    {
                                        g.FillRectangle(b1, new Rectangle(r.X + 4, r.Y - 1, r.Width - 4, r.Height));
                                    }
                                    else
                                    {
                                        g.FillRectangle(b1, new Rectangle(r.X, r.Y - 1, r.Width, r.Height));
                                    }
                                }
                            }
                        }

                        using (Pen p = new Pen(headerColor))
                        {
                            if (rtl)
                            {
                                g.DrawLine(p, r.Right, r.Top - 1, r.Right, r.Bottom);
                            }
                            else
                            {
                                g.DrawLine(p, r.Left/* + navigationPanel.weekNumberIndent*/, r.Top - 1, r.Left/* + navigationPanel.weekNumberIndent*/, r.Bottom);
                            }   
                        }

                        if (ok)
                        {
                            s = dt.Day.ToString();
                            Font metroFont = new System.Drawing.Font("Segoe UI", 6.50F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            using (Brush b = new SolidBrush(headerColor))
                            {
                                using (Font f = (this.navigationPanel.appearance.VisualStyle == GridVisualStyles.Metro) ? metroFont :  new Font(style.Font.Facename, style.Font.Size - 2.08f))
                                {
                                    int weekNumber = 0;
                                    TimeSpan ts = TimeSpan.Zero;

                                    if (this.navigationPanel.appearance.ISO8601CalenderFormat)
                                        weekNumber = GetWeekOfYearIso8601(dt);
                                    else
                                    {
                                        DateTime dt1 = dt.AddDays(6);
                                        dt1 = dt1.AddDays(-dt1.Day + 1).AddMonths(-dt1.Month + 1);
                                        dt1 = dt1.AddDays(-6);
                                        ts = dt - dt1;
                                    }
                                    
                                    using (Brush b1 = new SolidBrush(weekNumberColor))
                                    {
                                        if (rtl)
                                        {
                                            if (this.navigationPanel.appearance.ISO8601CalenderFormat)
                                                g.DrawString(weekNumber.ToString(), f, b1, (float)(r.Left + navigationPanel.weekNumberIndent + 25), (float)r.Top + 2);
                                            else
                                                g.DrawString((1 + (ts.Days / 7)).ToString(), f, b1, (float)(r.Left + navigationPanel.weekNumberIndent + 25), (float)r.Top + 2);
                                            
                                        }
                                        else
                                        {
                                            if (this.navigationPanel.appearance.ISO8601CalenderFormat)
                                                g.DrawString(weekNumber.ToString(), f, b1, (float)(r.Left - navigationPanel.weekNumberIndent), (float)r.Top + 2);
                                            else
                                                g.DrawString((1 + (ts.Days / 7)).ToString(), f, b1, (float)(r.Left - navigationPanel.weekNumberIndent), (float)r.Top + 2);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    GridStaticCellRenderer.DrawText(g, s, style.GdipFont, r, style, style.TextColor, rtl);
            }
        }

        /// <summary>
        /// Gets the Week number based on ISO 8601 standard.
        /// </summary>
        /// <param name="oDate"></param>
        /// <returns></returns>
        public int GetWeekOfYearIso8601(System.DateTime oDate)
        {
            this.navigationPanel.appearance.Culture.DateTimeFormat.CalendarWeekRule = System.Globalization.CalendarWeekRule.FirstFourDayWeek;
            System.Globalization.Calendar oCal = new System.Globalization.GregorianCalendar();
            
            DayOfWeek day = oCal.GetDayOfWeek(oDate);

            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                oDate = oDate.AddDays(3);
            }
            // Return the week of our adjusted day
            return oCal.GetWeekOfYear(oDate, this.navigationPanel.appearance.Culture.DateTimeFormat.CalendarWeekRule, this.navigationPanel.appearance.WeekCalendarStartDayOfWeek);
        }

        private bool isWeekNumber(int rowIndex, int colIndex)
        {
            bool ok = false;
            DateTime dt;
            if (DateTime.TryParse(Grid.Model[rowIndex, colIndex].Text, out dt))
            {
                int cal = rowIndex / navigationPanel.rowsPerCal;
                if (rowIndex % navigationPanel.rowsPerCal > 1)
                {
                    DateTime dtV = this.navigationPanel.DateValue.AddMonths(cal);
                    ok = !((cal == 0 && (100 * dt.Year) + dt.Month > (100 * dtV.Year) + dtV.Month)
                        || (cal > 0 && cal < this.navigationPanel.numberCalendars - 1 && dt.Month != dtV.Month)
                        || (cal == this.navigationPanel.numberCalendars - 1 && (100 * dt.Year) + dt.Month < (100 * dtV.Year) + dtV.Month));
                }
            }
           
            return ok;
        }

        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            bool hit = (this.Grid.RightToLeft == RightToLeft.Yes)
                ? (e.X > this.Grid.Width - this.navigationPanel.weekNumberIndent)
                : (e.X < this.navigationPanel.weekNumberIndent);
            if (hit && isWeekNumber(rowIndex, colIndex))
            {
                this.Grid.BeginUpdate();
                this.navigationPanel.SelectedDates.BeginUpdate();
                if (this.navigationPanel.scheduleGridGrid.ScheduleType != ScheduleViewType.Week)
                {
                    this.navigationPanel.scheduleGridGrid.SwitchTo(ScheduleViewType.Week);
                }

                this.navigationPanel.SelectedDates.Clear();
                ////off by one since ScheduleViewType.Week starts on Monday
                int row = this.Grid.IsRightToLeft() ? rowIndex - 1 : rowIndex;
                this.navigationPanel.SelectedDates.AddRange(
                    new DateTime[]
                    {
                                    (DateTime)Grid.Model[rowIndex, colIndex].CellValue,
                                    (DateTime)Grid.Model[rowIndex, colIndex+1].CellValue,
                                    (DateTime)Grid.Model[rowIndex, colIndex+2].CellValue,
                                    (DateTime)Grid.Model[rowIndex, colIndex+3].CellValue,
                                    (DateTime)Grid.Model[rowIndex, colIndex+4].CellValue,
                                    (DateTime)Grid.Model[rowIndex, colIndex+5].CellValue,
                                    (DateTime)Grid.Model[rowIndex, colIndex+6].CellValue 
                    });
                this.navigationPanel.SelectedDates.EndUpdate();
                this.Grid.EndUpdate(true);
            }
        }
    }
}

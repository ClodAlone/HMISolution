//-------------------------------------------------------------------------------------------------
// <copyright file="ColorComboBox.cs" company="syncfusion">
// Copyright (c) syncfusion.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Schedule;
////using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Schedule
{
    /// <summary>
    /// A dropdown list with colors assoicated with each list item.
    /// </summary>
    [ToolboxItem(false), Syncfusion.Documentation.DocumentationExclude()]
    public class ColorComboBox : Syncfusion.Windows.Forms.Tools.ComboBoxBase
    {
        private ListObjectList data;
        private GridControl grid;
        private bool showColor = true;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ColorComboBox()
        {
            this.RightToLeft = ScheduleControl.isMirrored ? RightToLeft.Yes : RightToLeft.No;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
            this.FlatStyle = Syncfusion.Windows.Forms.Tools.ComboFlatStyle.Standard;
        }

        /// <summary>
        /// Gets or sets whether or not a color splotch should be drawn in the dropdown.
        /// </summary>
        public bool ShowColor
        {
            get
            {
                return showColor;
            }

            set
            {
                showColor = value;
            }
        }

        #region Drawing events

        private int w = 10;
        private int h = 3;

        /// <summary>
        /// Overridden to draw color if ShowColor is true.
        /// </summary>
        /// <param name="e">The PaintEventArgs.</param>
        protected override void DrawEditPortion(PaintEventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            if (data == null && this.ListControl.DataSource != null)
            {
                ((GridListControl)ListControl).ItemHeight = 17;

                ((GridListControl)ListControl).FillLastColumn = true;
                data = this.ListControl.DataSource as ListObjectList;
                grid = ((GridListControl)ListControl).Grid;

                grid.CellDrawn += new GridDrawCellEventHandler(Grid_CellDrawn);
                grid.DrawCell += new GridDrawCellEventHandler(grid_DrawCell);
                grid.WantTabKey = false;
            }

            ////just call the baseclass after resetting the bordermargin back
            if (!ShowColor)
            {
                grid.TableStyle.BorderMargins.Left = 2;
                base.DrawEditPortion(e);
                return;
            }

            grid.TableStyle.BorderMargins.Left = 2 * w;

            Rectangle bounds = this.ClientRectangle;
            bounds.Offset(2, 2);
            bounds.Height -= 4;
            bounds.Width -= 4;
            
            using (Brush b = new SolidBrush(this.BackColor))
            {
                e.Graphics.FillRectangle(b, bounds);
            }

            if ((data != null) && (this.ListControl.SelectedIndex > -1))
            {
                ListObject item = this.data[this.ListControl.SelectedIndex] as ListObject;
                Rectangle rect = new Rectangle(new Point(bounds.Location.X + (w / 2), bounds.Location.Y + h), new Size(w, bounds.Height - (2 * h)));
                if (ScheduleControl.isMirrored)
                {
                    rect.X = bounds.Width - rect.Width - 1;
                }

                using (Brush b = new SolidBrush(item.ColorMember))
                {
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 1, rect.Height + 1));
                    e.Graphics.FillRectangle(b, rect);
                }

                if (ScheduleControl.isMirrored)
                {
                    rect.Offset(-bounds.Width, 0);
                    rect.Width = bounds.Width + rect.Width - (rect.X / 2) - TextBox.Location.X;
                    rect.Y -= 2;
                    rect.Height += 2;
                   
                    GridStaticCellRenderer.DrawText(e.Graphics, item.DisplayMember, this.Font, rect, grid.TableStyle, this.ForeColor, false, ScheduleControl.isMirrored);
                }
                else
                {
                    if (onControl && !DroppedDown)
                    {
                        using (Brush b = new SolidBrush(SystemColors.HighlightText))
                        {
                            using (Brush b1 = new SolidBrush(SystemColors.Highlight))
                            {
                                Rectangle r = this.ClientRectangle;
                                r.Offset((int)(rect.X + (1.5f * w)), 1);
                                r.Inflate(0, -3);
                                r.Width = (int)e.Graphics.MeasureString(item.DisplayMember, this.Font).Width + 1;
                                r.Height -= 1;
                                e.Graphics.FillRectangle(b1, r);
                            }

                            e.Graphics.DrawString(item.DisplayMember, this.Font, b, rect.X + (1.5f * w), rect.Y);
                        }
                    }
                    else
                    {
                        using (Brush b = new SolidBrush(this.ForeColor))
                        {
                            e.Graphics.DrawString(item.DisplayMember, this.Font, b, rect.X + (1.5f * w), rect.Y - 3);
                        }
                    }
                }
            }
        }

        /// <override/> 
        /// <returns>Popup location.</returns>
        protected override Point CorrectPopupLocation(Point location)
        {
            Point pt = base.CorrectPopupLocation(location);
            if (ScheduleControl.isMirrored)
            {
                pt.Offset(-this.ClientSize.Width, 0);
            }

            return pt;
        }

        ////used to draw color block in the list of the dropdown
        private void Grid_CellDrawn(object sender, GridDrawCellEventArgs e)
        {
            if (this.DesignMode || !ShowColor)
            {
                return;
            }

            if (data != null && e.RowIndex > 0)
            {
                Rectangle bounds = e.Bounds;
                ////bounds.Offset(2,0);
                ListObject item = this.data[e.RowIndex - 1] as ListObject;
                Rectangle rect = new Rectangle(new Point(bounds.Location.X + (w / 2), bounds.Location.Y + h), new Size(w, bounds.Height - (2 * h)));
                if (ScheduleControl.isMirrored)
                {
                    rect.X = bounds.Width - rect.Width - 3;
                }

                using (Brush b = new SolidBrush(item.ColorMember))
                {
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(rect.X - 1, rect.Y - 1, rect.Width + 1, rect.Height + 1));
                    e.Graphics.FillRectangle(b, rect);
                }
            }
        }

        ////used to draw text offset so colorblock will show without overlapping.
        private void grid_DrawCell(object sender, GridDrawCellEventArgs e)
        {
            if (this.DesignMode || !ShowColor)
            {
                return;
            }

            Rectangle rect = e.Bounds;
            if (ScheduleControl.isMirrored)
            {
                rect.Offset(-2 * w, 0);
            }
            else
            {
                rect.Offset(2 * w, 0);
            }

            GridStaticCellRenderer.DrawText(e.Graphics, e.Style.Text, e.Style.GdipFont, rect, e.Style, e.Style.TextColor, ScheduleControl.isMirrored);
            e.Cancel = true;
        }
        #endregion

        /// <override/> 
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            onControl = true;
            object save = ((GridListControl)ListControl).SelectedValue;
            ((GridListControl)ListControl).SelectedValue = -1;
            ((GridListControl)ListControl).SelectedValue = save;
        }

        private bool onControl = false;

        /// <override/> 
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (onControl && !DroppedDown)
            {
                using (Pen p = new Pen(Color.Black, 1))
                {
                    e.Graphics.DrawRectangle(p, e.ClipRectangle);
                }
            }
        }

        /// <override/> 
        protected override void OnLeave(EventArgs e)
        {
            onControl = false;
            base.OnLeave(e);
         }
    }
}

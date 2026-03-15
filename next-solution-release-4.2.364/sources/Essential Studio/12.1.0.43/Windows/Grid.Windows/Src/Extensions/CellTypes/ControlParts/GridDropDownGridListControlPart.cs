//-------------------------------------------------------------------------------------------------
// <copyright file="GridDropDownGridListControlPart.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.Windows.Forms;
using System.Text;
using System.Security;
using System.Security.Permissions;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the grid that can be displayed in a drop-down window for 
    /// a combo box. Handles autoscrolling and resize to fit contents.
    /// </summary>
    [ToolboxItem(false)]
    public class GridDropDownGridListControlPart : GridListControl
    {
        // Fields
        private int dropDownRows = 5;
        int optimalWidth = 0;
        bool allowSizing = true;

        /// <summary>
        /// Initializes a new <see cref="GridDropDownGridListControlPart"/> control.
        /// </summary>
        public GridDropDownGridListControlPart()
        {
            this.SetStyle(ControlStyles.Selectable, false);
            this.SetStyle(ControlStyles.UserMouse, true);
            grid.WantKeys = false;
            grid.SetWindowStyle(ControlStyles.Selectable, false);
            grid.CausesValidation = false;

            this.BorderStyle = BorderStyle.None;
        }

        GridDropDownContainer dropDownContainer = null;
        CancelEventHandler dropDownContainerBeforeCloseUp;

        /// <override/>
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        protected override void WndProc(ref Message msg)
        {
            if (msg.Msg == 0x21/*WM_MOUSEACTIVATE*/)
            {
                if (Parent is GridDropDownContainer && dropDownContainerBeforeCloseUp == null)
                {
                    // Sometimes when clicking on the list control the PopupControlContainer
                    // ParentLostFocus method is called. In this method the 
                    // curFocusControl = Control.FromHandle(NativeMethods.GetFocus())
                    // will return a ParkingWindow which then causes the dropdown to
                    // be closed.
                    // To workaround this problem we can can the cancellable BeforeCloseUp event
                    // which is called from the PopupControlContainer. We set a timeout
                    // that 200 ms after a WM_MOUSEACTIVATE event the current dropdown
                    // should not be hidden.
                    //
                    // The problem was detected with the "Modify" button of a GridTableDropDownListCellRenderer.
                    dropDownContainer = (GridDropDownContainer)Parent;
                    dropDownContainerBeforeCloseUp = new CancelEventHandler(dropDownContainer_BeforeCloseUp);
                    dropDownContainer.BeforeCloseUp += dropDownContainerBeforeCloseUp;
                }

                cancelCloseUpTicks = Environment.TickCount + 200;
                msg.Result = (IntPtr)3; ////MA_NOACTIVATE
                return;
            }

            base.WndProc(ref msg);
        }

        int cancelCloseUpTicks = int.MinValue;

        private void dropDownContainer_BeforeCloseUp(object sender, CancelEventArgs e)
        {
            if (Environment.TickCount < cancelCloseUpTicks)
            {
                e.Cancel = true;
            }

            cancelCloseUpTicks = int.MinValue;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dropDownContainerBeforeCloseUp != null && dropDownContainer != null)
                {
                    dropDownContainer.BeforeCloseUp -= dropDownContainerBeforeCloseUp;
                    dropDownContainerBeforeCloseUp = null;
                    dropDownContainer = null;
                }
            }
           
            base.Dispose(disposing);
        }
        
        /// <override/>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (!allowSizing || Dock == DockStyle.Fill)
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }
            else
            {
                // Compute the size we want to be, DropDownHolder will use this size.
                if ((specified & (BoundsSpecified.Height | BoundsSpecified.Width)) != 0)
                {
                    if ((specified & BoundsSpecified.Height) != 0)
                    {
                        height = grid.RowHeights.GetTotal(0, Math.Min(grid.RowCount, dropDownRows)) + 2;
                        if (BorderStyle == BorderStyle.FixedSingle)
                        {
                            height += 2;
                        }
                        else if (BorderStyle == BorderStyle.Fixed3D)
                        {
                            height += 4;
                        }
                    }

                    if ((specified & BoundsSpecified.Width) != 0)
                    {
                        optimalWidth = Math.Max(this.GetOptimalWidth(), width) + 1;
                        if (BorderStyle == BorderStyle.FixedSingle)
                        {
                            optimalWidth += 2;
                        }
                        else if (BorderStyle == BorderStyle.Fixed3D)
                        {
                            optimalWidth += 4;
                        }
                    }
                }

                base.SetBoundsCore(x, y, optimalWidth, height, specified);
            }
        }

        /// <override/>
        protected override void OnBindingContextChanged(EventArgs e)
        {
            ////base.OnBindingContextChanged (e);
        }

        /// <summary>
        /// Determines the optimal width of the drop-down window based on
        /// current column widths in this grid.
        /// </summary>
        /// <returns>Drop-down window width.</returns>
        protected virtual int GetOptimalWidth()
        {
            PerformLayout();
            int minWidth = grid.ColWidths.GetTotal(0, grid.ColCount);
            if (grid.RowCount > this.dropDownRows)
            {
                minWidth = minWidth + SystemInformation.VerticalScrollBarWidth;
            }

            return minWidth;
        }

        // Properties

        /// <summary>
        /// Gets or sets the number of visible rows when the window is dropped-own.
        /// </summary>
        public int DropDownRows
        {
            get
            {
                return this.dropDownRows;
            }

            set
            {
                this.dropDownRows = value;
                if (this.IsHandleCreated)
                {
                    this.Height = grid.RowHeights.GetTotal(0, Math.Min(grid.RowCount, dropDownRows)) + 2;
                }
            }
        }

        /// <internalonly/>
        /// <summary>Gets or sets a value indicating whether to allow modify SetBoundsCore. Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude]
        public bool AllowModifySetBoundsCore
        {
            get
            {
                return allowSizing;
            }

            set
            {
                allowSizing = value;
            }
        }
    }
}


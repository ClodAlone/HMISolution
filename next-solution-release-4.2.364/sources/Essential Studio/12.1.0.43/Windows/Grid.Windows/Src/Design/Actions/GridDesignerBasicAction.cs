//-------------------------------------------------------------------------------------------------
// <copyright file="GridDesignerBasicAction.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Styles;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.InternalMenus;

namespace Syncfusion.Windows.Forms.Grid.Design
{
    /// <exclude/>
    /// <summary>
    /// This class is the common base class for actions such as
    /// MenuActions, Editactions or plugins
    /// </summary>
    internal abstract class GridDesignerBasicAction : BasicAction
    {
        /// <summary>
        ///     Used to set the state of Toolbar/Menu items that are tied to the action.
        ///     Inherited classes may override to change functionality.
        /// </summary>
        /// <param name="selected" type="bool">
        ///     <para>
        ///         Whether or not the sourceObject is selected
        ///     </para>
        /// </param>
        /// <param name="sourceObject" type="object">
        ///     <para>
        ///         The object tied to the action.
        ///     </para>
        /// </param>
        public virtual void SetState(bool selected, object sourceObject)
        {
            if (sourceObject == null)
            {
                return;
            }

            if (typeof(ToolBarButton).IsInstanceOfType(sourceObject))
            {
                ((ToolBarButton)sourceObject).Pushed = selected;
            }
            else if (typeof(MenuItem).IsInstanceOfType(sourceObject))
            {
                ((MenuItem)sourceObject).Checked = selected;
            }
#if SyncfusionFramework2_0 ////2.0 framework            
            else if (typeof(ToolStripButton).IsInstanceOfType(sourceObject))
            {
                ((ToolStripButton)sourceObject).CheckState = selected ? CheckState.Checked : CheckState.Unchecked;
            }
#endif
        }

        /// <summary>
        ///   Gets currently active Grid, if more than one are present
        /// </summary>
        protected GridControlBase ActiveGrid
        {
            get
            {
                if (MainWindow != null)
                {
                    if (MainWindow.ActiveControl is Form)
                    {
                        return GetGrid((Form)MainWindow.ActiveControl);
                    }
                    else if (MainWindow.ActiveControl != null)
                    {
                        return GetGrid((Form)MainWindow.ActiveControl.FindForm());
                    }
                }

                return null;
            }
        }

        protected GridControlBase GetGrid(Form form)
        {
            if (form != null)
            {
                Control control = form.ActiveControl;

                while (control is ContainerControl)
                {
                    control = ((ContainerControl)control).ActiveControl;
                }

                // Could be a focused textbox
                if (!(control is GridControlBase))
                {
                    control = control.Parent;
                }

                if (control is GridControlBase)
                {
                    GridControlBase grid = (GridControlBase)control;

                    // Active cell ...
                    if (control != null && grid == null)
                    {
                        grid = control.Parent is GridControlBase ? (GridControlBase)grid : null;
                    }

                    return grid;
                }
            }

            return null;
        }

        /// <summary>
        ///     Applies the supplied style to the supplied GridControlBase object
        /// </summary>
        protected static void ApplyStyle(GridStyleInfo style, GridControlBase grid)
        {
            if (grid != null)
            {
                GridRangeInfoList ranges;
                grid.Selections.GetSelectedRanges(out ranges, true);
                foreach (GridRangeInfo range in ranges)
                {
                    grid.Model.ChangeCells(range, style, StyleModifyType.Override);
                }
            }
        }

        /// <summary>
        /// Gets or sets the main window of the design editor
        /// </summary>
        protected new GridFrame MainWindow
        {
            get
            {
                return base.MainWindow as GridFrame;
            }

            set
            {
                base.MainWindow = value;
            }
        }
    }
}

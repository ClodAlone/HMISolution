//-------------------------------------------------------------------------------------------------
// <copyright file="GridAccessibility.cs" company="syncfusion">
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
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid.GridInternal;

namespace Syncfusion.Windows.Forms.Grid
{
    internal class GridControlRowAccessibleObject : Control.ControlAccessibleObject
    {
        internal GridControlBaseImp grid;
        int rowIndex;

        public GridControlRowAccessibleObject(GridControlBaseImp grid, int rowIndex)
            : base(grid)
        {
            this.grid = grid;
            this.rowIndex = rowIndex;
        }

        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        internal GridControlBaseAccessibleObject GridAccessibilityObject
        {
            get
            {
                return grid.AccessibilityObject as GridControlBaseAccessibleObject;
            }
        }

        public override /*AccessibleObject*/ void Select(AccessibleSelection flags)
        {
            if ((flags & AccessibleSelection.TakeFocus) != 0)
            {
                if (!grid.Focused)
                {
                    grid.Focus();
                }
            }

            if ((flags & AccessibleSelection.TakeSelection) != 0)
            {
                GridAccessibilityObject.SelectChild(this, flags);
            }
        } // end of method Select

        /// <summary>
        /// Navigate to the next or previous grid entry.
        /// </summary>
        /// <param name="navdir">One of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.</param>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents one of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The navigation attempt fails.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override /*AccessibleObject*/ AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return GridAccessibilityObject.NavigateFromChild(this, navdir);
        } // end of method Navigate
        
        /// <summary>
        /// Performs the default action associated with this accessible object.
        /// </summary>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The default action for the control cannot be performed.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override /*AccessibleObject*/ void DoDefaultAction()
        {
            this.Select((AccessibleSelection.TakeSelection | AccessibleSelection.TakeFocus));
        } // end of method DoDefaultAction

        /// <summary>
        /// Returns the currently focused child, if any.
        /// Returns this if the object itself is focused.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that specifies the currently focused child. This method returns the calling object if the object itself is focused. Returns null if no object has focus.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The control cannot be retrieved.
        /// </exception>
        public override /*AccessibleObject*/ AccessibleObject GetFocused()
        {
            return this.GridAccessibilityObject.GetFocused();
        } // end of method GetFocused
        
        public override /*AccessibleObject*/ AccessibleStates State
        {
            get
            {
                AccessibleStates accessibleStates = AccessibleStates.Selectable | AccessibleStates.Focusable;

                int rowIndex = RowIndex;
                if (grid.Model.SelectedRanges.Contains(GridRangeInfo.Row(rowIndex)))
                {
                    accessibleStates |= AccessibleStates.Selected;
                }

                if (grid.CurrentCell.HasCurrentCellAt(rowIndex))
                {
                    accessibleStates |= AccessibleStates.Focused;
                }

                if (rowIndex < grid.TopRowIndex || rowIndex > grid.ViewLayout.LastVisibleRow)
                {
                    accessibleStates |= AccessibleStates.Offscreen | AccessibleStates.Invisible;
                }

                accessibleStates |= AccessibleStates.MultiSelectable;

                return accessibleStates;
            } // end of method get_State
        }

        public override /*AccessibleObject*/ AccessibleRole Role
        {
            get
            {
                return AccessibleRole.Row;
            } // end of method get_Role
        }

        public override /*AccessibleObject*/ AccessibleObject Parent
        {
            get
            {
                return grid.AccessibilityObject;
            } // end of method get_Parent
        }

        public override /*AccessibleObject*/ string Name
        {
            get
            {
                return "Row " + RowIndex.ToString();
            } // end of method get_Name
        }

        public override /*AccessibleObject*/ string DefaultAction
        {
            get
            {
                return "Click";
            } // end of method get_DefaultAction
        }

        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                Rectangle r = grid.RangeInfoToRectangle(GridRangeInfo.Row(rowIndex));
                r.Intersect(grid.ClientRectangle);
                return this.grid.RectangleToScreen(r);
            } // end of method get_Bounds
        }

        public override string Description
        {
            get
            {
                return "Row " + RowIndex.ToString();
            }
        }

        GridControlCellAccessibleObjectsIndexer cellAccessibleObjects = null;

        internal GridControlCellAccessibleObjectsIndexer CellAccessibleObjects
        {
            get
            {
                if (cellAccessibleObjects == null)
                {
                    cellAccessibleObjects = new GridControlCellAccessibleObjectsIndexer(this);
                }

                return cellAccessibleObjects;
            }
        }

        public override int GetChildCount()
        {
            return grid.Model.ColCount;
        }

        // Gets the Accessibility object of the cell idetified by index.
        public override AccessibleObject GetChild(int index)
        {
            if (index < grid.Model.ColCount)
            {
                return CellAccessibleObjects[index];
            }

            return null;
        }

        // Helper function that is used by the GridControlRowAccessibleObject's accessibility object
        // to navigate between sibiling controls. Specifically, this function is used in
        // the GridControlRowAccessibleObject.Navigate function.
        internal AccessibleObject NavigateFromChild(GridControlCellAccessibleObject child, AccessibleNavigation navdir)
        {
            int index = child.ColIndex - 1;

            switch (navdir)
            {
                case AccessibleNavigation.FirstChild:
                    index = 0;
                    break;

                case AccessibleNavigation.LastChild:
                    index = GetChildCount() - 1;
                    break;

                case AccessibleNavigation.Left:
                case AccessibleNavigation.Previous:
                case AccessibleNavigation.Up:
                    if (index > 0)
                    {
                        index--;
                    }

                    break;

                case AccessibleNavigation.Right:
                case AccessibleNavigation.Next:
                case AccessibleNavigation.Down:
                    if (index < GetChildCount())
                    {
                        index++;
                    }

                    break;
            }

            return this.GetChild(index);
        }

        // Helper function that is used by the grid's accessibility object
        // to select a specific grid control. Specifically, this function is used
        // in the grid.gridAccessibleObject.Select function.
        internal void SelectChild(GridControlCellAccessibleObject child, AccessibleSelection selection)
        {
            ////To simulate a click AccessibleSelection.TakeFocus|AccessibleSelection.TakeSelection
            ////To select a target item by simulating CTRL + click AccessibleSelection.TakeFocus|AccessibleSelection.AddSelection
            ////To cancel selection of a target item by simulating CTRL + click AccessibleSelection.TakeFocus|AccessibleSelection.RemoveSelection
            ////To simulate SHIFT + click AccessibleSelection.TakeFocus|AccessibleSelection.ExtendSelection

            ////To select a range of objects and put focus on the last object Specify AccessibleSelection.TakeFocus on the starting object to set the selection anchor. Then call Select again and specify AccessibleSelection.TakeFocus|AccessibleSelection.ExtendSelection on the last object.
            ////To deselect all objects Specify AccessibleSelection.TakeSelection on any object. This flag deselects all selected objects except the one just selected. Then call Select again and specify AccessibleSelection.RemoveSelection on the same object.

            int colIndex = child.ColIndex;

            //// Determine which selection action should occur, based on the
            //// AccessibleSelection value.
            if ((selection & AccessibleSelection.TakeFocus) != 0)
            {
                if ((selection & AccessibleSelection.TakeSelection) != 0)
                {
                    this.grid.CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.ScrollInView);
                }

                if ((selection & AccessibleSelection.AddSelection) != 0)
                {
                    this.grid.Selections.Add(GridRangeInfo.Cell(rowIndex, colIndex));
                }

                if ((selection & AccessibleSelection.RemoveSelection) != 0)
                {
                    this.grid.Selections.Remove(GridRangeInfo.Cell(rowIndex, colIndex));
                }

                if ((selection & AccessibleSelection.ExtendSelection) != 0)
                {
                    int index1 = this.grid.CurrentCell.RowIndex;
                    this.grid.Selections.Add(GridRangeInfo.Cells(rowIndex, Math.Min(colIndex, index1), rowIndex, Math.Max(colIndex, index1)));
                }
            }
        }

        public override AccessibleObject GetSelected()
        {
            if (grid.CurrentCell.HasCurrentCellAt(RowIndex) && grid.CurrentCell.ColIndex > 0)
            {
                return GetChild(grid.CurrentCell.ColIndex - 1);
            }

            return base.GetSelected();
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            Point point = grid.PointToClient(new Point(x, y));
            GridRangeInfo range = grid.PointToRangeInfo(point);
            if (!range.IsEmpty && range.Top == RowIndex && range.Left > 0)
            {
                return CellAccessibleObjects[range.Left - 1];
            }

            return base.HitTest(x, y);
        }

        ////        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        ////        {
        ////            GridControlCellAccessibleObject accObj = GetSelected() as GridControlCellAccessibleObject;
        ////            if (accObj != null)
        ////                return this.NavigateFromChild(accObj, navdir);
        ////            return base.Navigate(navdir);
        ////        }
    }

    internal class GridControlCellAccessibleObject : AccessibleObject
    {
        GridControlBaseImp grid;
        int rowIndex;
        int colIndex;

        public GridControlCellAccessibleObject(GridControlBaseImp grid, int rowIndex, int colIndex)
        {
            this.grid = grid;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }

        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        internal GridControlBaseAccessibleObject GridAccessibilityObject
        {
            get
            {
                return grid.AccessibilityObject as GridControlBaseAccessibleObject;
            }
        }

        internal GridControlRowAccessibleObject RowAccessibilityObject
        {
            get
            {
                return grid.RowAccessibleObjects[rowIndex - 1] as GridControlRowAccessibleObject;
            }
        }

        public override /*AccessibleObject*/ void Select(AccessibleSelection flags)
        {
            if ((flags & AccessibleSelection.TakeFocus) != 0)
            {
                if (!grid.Focused)
                {
                    grid.Focus();
                }
            }

            if ((flags & AccessibleSelection.TakeSelection) != 0)
            {
                RowAccessibilityObject.SelectChild(this, flags);
            }
        } //// end of method Select

        /// <summary>
        /// Navigate to the next or previous grid entry.
        /// </summary>
        /// <param name="navdir">One of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.</param>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents one of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The navigation attempt fails.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override /*AccessibleObject*/ AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return RowAccessibilityObject.NavigateFromChild(this, navdir);
        } // end of method Navigate

        public override /*AccessibleObject*/ void DoDefaultAction()
        {
            this.Select((AccessibleSelection.TakeSelection | AccessibleSelection.TakeFocus));
        } // end of method DoDefaultAction

        /// <summary>
        /// Returns the currently focused child, if any.
        /// Returns this if the object itself is focused.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that specifies the currently focused child. This method returns the calling object if the object itself is focused. Returns null if no object has focus.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The control cannot be retrieved.
        /// </exception>
        public override /*AccessibleObject*/ AccessibleObject GetFocused()
        {
            return this.RowAccessibilityObject.GetFocused();
        } // end of method GetFocused
        
        public override /*AccessibleObject*/ AccessibleStates State
        {
            get
            {
                AccessibleStates accessibleStates = AccessibleStates.Selectable | AccessibleStates.Focusable;

                int rowIndex = ColIndex;
                if (grid.Model.SelectedRanges.Contains(GridRangeInfo.Cell(rowIndex, colIndex)))
                {
                    accessibleStates |= AccessibleStates.Selected;
                }

                if (grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                {
                    accessibleStates |= AccessibleStates.Focused;
                }

                if (rowIndex < grid.TopRowIndex || rowIndex > grid.ViewLayout.LastVisibleRow
                    || colIndex < grid.LeftColIndex || colIndex > grid.ViewLayout.LastVisibleCol)
                {
                    accessibleStates |= AccessibleStates.Offscreen | AccessibleStates.Invisible;
                }

                accessibleStates |= AccessibleStates.MultiSelectable;

                return accessibleStates;
            } // end of method get_State
        }

        public override /*AccessibleObject*/ AccessibleRole Role
        {
            get
            {
                return AccessibleRole.Cell;
            } // end of method get_Role
        }

        public override /*AccessibleObject*/ AccessibleObject Parent
        {
            get
            {
                return this.RowAccessibilityObject;
            } // end of method get_Parent
        }

        public override /*AccessibleObject*/ string Name
        {
            get
            {
                return "Column " + ColIndex.ToString();
            } // end of method get_Name
        }

        public override /*AccessibleObject*/ string DefaultAction
        {
            get
            {
                return "Click";
            } // end of method get_DefaultAction
        }

        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                Rectangle r = grid.RangeInfoToRectangle(GridRangeInfo.Cell(rowIndex, colIndex));
                r.Intersect(grid.ClientRectangle);
                return this.grid.RectangleToScreen(r);
            } // end of method get_Bounds
        }

        public override string Description
        {
            get
            {
                return "Column " + ColIndex.ToString();
            }
        }

        public override string Value
        {
            get
            {
                return grid.Model[RowIndex, ColIndex].FormattedText;
            }

            set
            {
                grid.Model[RowIndex, ColIndex].FormattedText = value;
            }
        }
    }

    internal class GridControlColHeaderAccessibleObject : AccessibleObject
    {
        GridControlBase grid;
        int colIndex;

        public GridControlColHeaderAccessibleObject(GridControlBase grid, int colIndex)
        {
            this.grid = grid;
            this.colIndex = colIndex;
        }

        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }

        internal GridControlBaseAccessibleObject GridAccessibilityObject
        {
            get
            {
                return grid.AccessibilityObject as GridControlBaseAccessibleObject;
            }
        }

        /// <summary>
        /// Modifies the selection or moves the keyboard focus of the accessible object.
        /// </summary>
        /// <param name="flags">One of the <see cref="T:System.Windows.Forms.AccessibleSelection"/> values.</param>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The selection cannot be performed.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override /*AccessibleObject*/ void Select(AccessibleSelection flags)
        {
            if ((flags & AccessibleSelection.TakeFocus) != 0)
            {
                if (!grid.Focused)
                {
                    grid.Focus();
                }

                grid.CurrentCell.MoveTo(grid.CurrentCell.RowIndex, ColIndex, GridSetCurrentCellOptions.ScrollInView);
            }
        } // end of method Select

        /// <summary>
        /// Navigate to the next or previous grid entry.
        /// </summary>
        /// <param name="navdir">One of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.</param>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that represents one of the <see cref="T:System.Windows.Forms.AccessibleNavigation"/> values.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The navigation attempt fails.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode"/>
        /// </PermissionSet>
        public override /*AccessibleObject*/ AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            return GridAccessibilityObject.NavigateFromChild(this, navdir);
        } // end of method Navigate

        public override /*AccessibleObject*/ void DoDefaultAction()
        {
            this.Select((AccessibleSelection.TakeSelection | AccessibleSelection.TakeFocus));
        } // end of method DoDefaultAction

        /// <summary>
        /// Returns the currently focused child, if any.
        /// Returns this if the object itself is focused.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Windows.Forms.AccessibleObject"/> that specifies the currently focused child. This method returns the calling object if the object itself is focused. Returns null if no object has focus.
        /// </returns>
        /// <exception cref="T:System.Runtime.InteropServices.COMException">
        /// The control cannot be retrieved.
        /// </exception>
        public override /*AccessibleObject*/ AccessibleObject GetFocused()
        {
            return this.GridAccessibilityObject.GetFocused();
        } // end of method GetFocused

        public override /*AccessibleObject*/ AccessibleStates State
        {
            get
            {
                AccessibleStates accessibleStates = AccessibleStates.Selectable;

                if (colIndex < grid.LeftColIndex || colIndex > grid.ViewLayout.LastVisibleCol)
                {
                    accessibleStates |= AccessibleStates.Offscreen | AccessibleStates.Invisible;
                }

                return accessibleStates;
            } // end of method get_State
        }

        public override /*AccessibleObject*/ AccessibleRole Role
        {
            get
            {
                return AccessibleRole.ColumnHeader;
            } // end of method get_Role
        }

        public override /*AccessibleObject*/ AccessibleObject Parent
        {
            get
            {
                return grid.AccessibilityObject;
            } // end of method get_Parent
        }

        public override /*AccessibleObject*/ string Name
        {
            get
            {
                string name = this.grid.Model[0, colIndex].Text;
                if (GridUtil.IsEmpty(name))
                {
                    name = GridRangeInfo.Col(ColIndex).ToString();
                }

                return name;
            } // end of method get_Name
        }

        public override /*AccessibleObject*/ string DefaultAction
        {
            get
            {
                return "Select";
            } // end of method get_DefaultAction
        }

        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                return this.grid.RectangleToScreen(grid.RangeInfoToRectangle(GridRangeInfo.Cell(0, colIndex)));
            } // end of method get_Bounds
        }

        public override string Description
        {
            get
            {
                string name = this.grid.Model[0, colIndex].Text;
                if (GridUtil.IsEmpty(name))
                {
                    name = GridRangeInfo.Col(ColIndex).ToString();
                }

                return name;
            }
        }
    }

    internal class GridControlCellAccessibleObjectsIndexer
    {
        ArrayList data;
        GridControlRowAccessibleObject rowAcc;

        internal GridControlCellAccessibleObjectsIndexer(GridControlRowAccessibleObject rowAcc)
        {
            this.rowAcc = rowAcc;
            this.data = new ArrayList();
        }

        internal GridControlCellAccessibleObject GetItem(int index)
        {
            // returns null if ControlBaseCell is not found
            return index >= 0 && index < data.Count ? data[index] as GridControlCellAccessibleObject : null;
        }

        internal void SetItem(int index, GridControlCellAccessibleObject accObj)
        {
            if (index >= data.Count)
            {
                object[] newItems = new object[index - data.Count + 1];
                data.AddRange(newItems);
            }

            data[index] = accObj;
        }

        internal void ResetItem(int index)
        {
            if (index < data.Count)
            {
                data[index] = null;
            }
        }

        [Browsable(false)]
        public GridControlCellAccessibleObject /*IGridData*/ this[int index]
        {
            get
            {
                GridControlCellAccessibleObject accObj = GetItem(index);
                if (accObj == null)
                {
                    accObj = new GridControlCellAccessibleObject(rowAcc.grid, rowAcc.RowIndex, index + 1);
                    SetItem(index, accObj);
                }

                return accObj;
            }
        }
    }

    internal class GridControlRowAccessibleObjectsIndexer
    {
        ArrayList data;
        internal GridControlBaseImp grid;

        internal GridControlRowAccessibleObjectsIndexer(GridControlBaseImp grid)
        {
            this.grid = grid;
            this.data = new ArrayList();
        }

        internal GridControlRowAccessibleObject GetItem(int index)
        {
            // returns null if ControlBaseRow is not found
            return index >= 0 && index < data.Count ? data[index] as GridControlRowAccessibleObject : null;
        }

        internal void SetItem(int index, GridControlRowAccessibleObject accObj)
        {
            if (index >= data.Count)
            {
                object[] newItems = new object[index - data.Count + 1];
                data.AddRange(newItems);
            }

            data[index] = accObj;
        }

        internal void ResetItem(int index)
        {
            if (index < data.Count)
            {
                data[index] = null;
            }
        }

        [Browsable(false)]
        public GridControlRowAccessibleObject /*IGridData*/ this[int index]
        {
            get
            {
                GridControlRowAccessibleObject accObj = GetItem(index);
                if (accObj == null)
                {
                    accObj = grid.CreateRowAccessibilityInstance(index);
                    SetItem(index, accObj);
                }

                return accObj;
            }
        }
    }

    internal class GridControlColHeaderAccessibleObjectsIndexer
    {
        ArrayList data;
        GridControlBaseImp grid;

        internal GridControlColHeaderAccessibleObjectsIndexer(GridControlBaseImp grid)
        {
            this.grid = grid;
            this.data = new ArrayList();
        }

        internal GridControlColHeaderAccessibleObject GetItem(int index)
        {
            // returns null if ControlBaseRow is not found
            return index < data.Count ? data[index] as GridControlColHeaderAccessibleObject : null;
        }

        internal void SetItem(int index, GridControlColHeaderAccessibleObject accObj)
        {
            if (index >= data.Count)
            {
                object[] newItems = new object[index - data.Count + 1];
                data.AddRange(newItems);
            }

            data[index] = accObj;
        }

        internal void ResetItem(int index)
        {
            if (index < data.Count)
            {
                data[index] = null;
            }
        }

        [Browsable(false)]
        public GridControlColHeaderAccessibleObject /*IGridData*/ this[int index]
        {
            get
            {
                GridControlColHeaderAccessibleObject accObj = GetItem(index);
                if (accObj == null)
                {
                    accObj = grid.CreateColHeaderAccessibilityInstance(index);
                    SetItem(index, accObj);
                }

                return accObj;
            }
        }
    }

    internal class GridControlBaseAccessibleObject : Control.ControlAccessibleObject
    {
        GridControlBaseImp grid;

        public GridControlBaseAccessibleObject(GridControlBaseImp owner)
            : base(owner)
        {
            this.grid = owner;
            this.grid.CurrentCellActivated += new EventHandler(grid_CurrentCellActivated);
        }

        private void grid_CurrentCellActivated(object sender, EventArgs e)
        {
            if (grid.CurrentCell.RowIndex > 0 && grid.CurrentCell.ColIndex > 0)
            {
                int childId = grid.CurrentCell.RowIndex - 1 + grid.Model.ColCount;
                NotifyClients(AccessibleEvents.Focus, childId);
                NotifyClients(AccessibleEvents.Selection, childId);
            }
        }

        // Gets the role for the grid. This is used by accessibility programs.
        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.Table;
            }
        }

        public override /*AccessibleObject*/ string Name
        {
            get
            {
                return this.grid.AccessibleName;
            } // end of method get_Name
        }

        public override /*AccessibleObject*/ Rectangle Bounds
        {
            get
            {
                return this.grid.RectangleToScreen(this.grid.ClientRectangle);
            } // end of method get_Bounds
        }

        public override string Description
        {
            get
            {
                return this.grid.AccessibleDescription;
            }
        }

        public override string Help
        {
            get
            {
                return string.Empty;
            }
        }

        public override AccessibleObject Parent
        {
            get
            {
                return grid.AccessibilityObject;
            }
        }

        // Gets the state for the grid. This is used by accessibility programs.
        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = AccessibleStates.None;

                return state;
            }
        }

        // The grid objects are "child" controls in terms of accessibility so
        // return the number of ChartLengend objects.
        public override int GetChildCount()
        {
            return grid.Model.RowCount + grid.Model.ColCount;
        }

        // Gets the Accessibility object of the cell idetified by index.
        public override AccessibleObject GetChild(int index)
        {
            if (index < grid.Model.ColCount)
            {
                return grid.ColHeaderAccessibleObjects[index];
            }

            index -= grid.Model.ColCount;
            if (index < grid.Model.RowCount)
            {
                return grid.RowAccessibleObjects[index];
            }

            return null;
        }

        public override string Value
        {
            get
            {
                return grid.Text;
            }

            set
            {
                grid.Text = value;
            }
        }

        // Helper function that is used by the GridControlRowAccessibleObject's accessibility object
        // to navigate between sibiling controls. Specifically, this function is used in
        // the GridControlRowAccessibleObject.Navigate function.
        internal AccessibleObject NavigateFromChild(AccessibleObject child, AccessibleNavigation navdir)
        {
            int index = -1;
            GridControlRowAccessibleObject rowAcc = child as GridControlRowAccessibleObject;
            if (rowAcc != null)
            {
                index = rowAcc.RowIndex - 1 + grid.Model.ColCount;
            }
            else
            {
                GridControlColHeaderAccessibleObject colHeaderAcc = child as GridControlColHeaderAccessibleObject;
                index = colHeaderAcc.ColIndex - 1;
            }

            switch (navdir)
            {
                case AccessibleNavigation.FirstChild:
                    index = 0;
                    break;

                case AccessibleNavigation.LastChild:
                    index = GetChildCount() - 1;
                    break;

                case AccessibleNavigation.Left:
                case AccessibleNavigation.Previous:
                case AccessibleNavigation.Up:
                    if (index > 0)
                    {
                        index--;
                    }

                    break;

                case AccessibleNavigation.Right:
                case AccessibleNavigation.Next:
                case AccessibleNavigation.Down:
                    if (index < GetChildCount())
                    {
                        index++;
                    }

                    break;
            }

            return this.GetChild(index);
        }

        //// Helper function that is used by the grid's accessibility object
        //// to select a specific grid control. Specifically, this function is used
        //// in the grid.gridAccessibleObject.Select function.
        internal void SelectChild(GridControlRowAccessibleObject child, AccessibleSelection selection)
        {
            ////To simulate a click AccessibleSelection.TakeFocus|AccessibleSelection.TakeSelection
            ////To select a target item by simulating CTRL + click AccessibleSelection.TakeFocus|AccessibleSelection.AddSelection
            ////To cancel selection of a target item by simulating CTRL + click AccessibleSelection.TakeFocus|AccessibleSelection.RemoveSelection
            ////To simulate SHIFT + click AccessibleSelection.TakeFocus|AccessibleSelection.ExtendSelection

            ////To select a range of objects and put focus on the last object Specify AccessibleSelection.TakeFocus on the starting object to set the selection anchor. Then call Select again and specify AccessibleSelection.TakeFocus|AccessibleSelection.ExtendSelection on the last object.
            ////To deselect all objects Specify AccessibleSelection.TakeSelection on any object. This flag deselects all selected objects except the one just selected. Then call Select again and specify AccessibleSelection.RemoveSelection on the same object.

            int rowIndex = child.RowIndex;

            // Determine which selection action should occur, based on the
            // AccessibleSelection value.
            if ((selection & AccessibleSelection.TakeFocus) != 0)
            {
                if ((selection & AccessibleSelection.TakeSelection) != 0)
                {
                    this.grid.CurrentCell.MoveTo(rowIndex, this.grid.CurrentCell.ColIndex, GridSetCurrentCellOptions.ScrollInView);
                }

                if ((selection & AccessibleSelection.AddSelection) != 0)
                {
                    this.grid.Selections.Add(GridRangeInfo.Row(rowIndex));
                }

                if ((selection & AccessibleSelection.RemoveSelection) != 0)
                {
                    this.grid.Selections.Remove(GridRangeInfo.Row(rowIndex));
                }

                if ((selection & AccessibleSelection.ExtendSelection) != 0)
                {
                    int index1 = this.grid.CurrentCell.RowIndex;
                    this.grid.Selections.Add(GridRangeInfo.Rows(Math.Min(rowIndex, index1), Math.Max(rowIndex, index1)));
                }
            }
        }

        public override AccessibleObject GetFocused()
        {
            if (this.grid.Focused)
            {
                return GetSelected();
            }
            else
            {
                return base.GetFocused();
            }
        }

        public override AccessibleObject GetSelected()
        {
            if (grid.CurrentCell.HasCurrentCell && grid.CurrentCell.RowIndex > 0)
            {
                return GetChild(grid.CurrentCell.RowIndex - 1 + grid.Model.ColCount);
            }

            return base.GetSelected();
        }

        public override AccessibleObject HitTest(int x, int y)
        {
            Point point = grid.PointToClient(new Point(x, y));
            GridRangeInfo range = grid.PointToRangeInfo(point);
            if (!range.IsEmpty && range.Top > 0)
            {
                return grid.RowAccessibleObjects[range.Top - 1];
            }

            return base.HitTest(x, y);
        }

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
        {
            GridControlRowAccessibleObject accObj = GetSelected() as GridControlRowAccessibleObject;
            if (accObj != null)
            {
                return this.NavigateFromChild(accObj, navdir);
            }

            return base.Navigate(navdir);
        }
    }
}

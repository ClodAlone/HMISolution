#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Automation.Provider;
using System.Drawing;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Forms;
using Syncfusion.Collections;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Automation of the Grid UI
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridUIAProvider : IRawElementProviderFragmentRoot, ISelectionProvider, IScrollProvider
    {
        GridControl Grid = null;
        GridDataBoundGrid DataGrid = null;

        /// <summary>
        /// assign the grid control
        /// </summary>
        /// <param name="grid">Contol either GridControl or GridDataBoundGrid</param>
        public GridUIAProvider(Control grid)
        {
            if (grid is GridControl)
            {
                this.Grid = (GridControl)grid;
            }
            else if (grid is GridDataBoundGrid)
            {
                this.DataGrid = (GridDataBoundGrid)grid;
            }

        }

        #region IRawElementProviderFragmentRoot Members
        /// <summary>
        /// get the element provider from the point
        /// </summary>
        /// <param name="x">x axis point</param>
        /// <param name="y">y axis point</param>
        /// <returns></returns>
        public IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
        {
            System.Drawing.Point p = new System.Drawing.Point((int)x, (int)y);

            if (this.Grid != null)
            {
                p = Grid.PointToClient(p);

                GridRangeInfo info = this.Grid.PointToRangeInfo(p);

                return this.Grid[info.Top, info.Left].Provider;
            }
            else if (this.DataGrid != null)
            {
                p = DataGrid.PointToClient(p);

                GridRangeInfo info = this.DataGrid.PointToRangeInfo(p);

                return this.DataGrid[info.Top, info.Left].Provider;
            }
            else
                return null;
        }
        /// <summary>
        /// Get the focus on the Raw Element Provider
        /// </summary>
        /// <returns>returns the IRawElementProviderFragment</returns>
        public IRawElementProviderFragment GetFocus()
        {
            if (this.Grid != null)
            {
                return this.Grid.Model[Grid.Model.CurrentCellInfo.RowIndex, Grid.Model.CurrentCellInfo.ColIndex].Provider;
            }
            else if (this.DataGrid != null)
            {
                return this.Grid.Model[Grid.Model.CurrentCellInfo.RowIndex, Grid.Model.CurrentCellInfo.ColIndex].Provider;
            }
            else
                return null;
        }

        #endregion

        #region IRawElementProviderFragment Members
        /// <summary>
        /// get the bounding rectangle of the Grid
        /// </summary>
        public System.Windows.Rect BoundingRectangle
        {
            get 
            {
                if (this.Grid != null)
                {
                    Rectangle rect = this.Grid.RectangleToScreen(this.Grid.ClientRectangle);
                    return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
                }
                else if(this.DataGrid!=null)
                {
                    Rectangle rect = this.Grid.RectangleToScreen(this.Grid.ClientRectangle);
                    return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
                }
                else
                {
                    return new Rect();
                }
            }
        }

        
        /// <summary>
        /// get the FragmentRoot of Raw Element Provider
        /// </summary>

        public IRawElementProviderFragmentRoot FragmentRoot
        {
            get 
            {
                return this;
            }
        }
        /// <summary>
        /// get the collection of the Embedded FragmentRoot of Raw Element Provider
        /// </summary>
        /// <returns>returns the collections of Element ProviderSimple</returns>
        public IRawElementProviderSimple[] GetEmbeddedFragmentRoots()
        {
            return null;
        }
        /// <summary>
        /// Get the collection of Runtime ID
        /// </summary>
        /// <returns>collection of is</returns>
        public int[] GetRuntimeId()
        {
            if (this.Grid != null)
            {
                return new int[] { this.Grid.GetHashCode() };
            }
            else if (this.DataGrid != null)
            {
                return new int[] { this.DataGrid.GetHashCode() };
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// get the IROW element provider Fragment by the NavigateDirection
        /// </summary>
        /// <param name="direction">Navigate Direction</param>
        /// <returns>returns the Irow element provider</returns>
        public IRawElementProviderFragment Navigate(NavigateDirection direction)
        {
            switch (direction)
            {
                case NavigateDirection.FirstChild:
                    if (this.Grid != null)
                    {
                        return this.Grid.Model[0, 0].Provider;
                    }
                    else if (this.DataGrid != null)
                    {
                        return this.DataGrid.Model[0, 0].Provider;
                    }
                    else
                        return null;
                case NavigateDirection.LastChild:
                    return this.Grid.Model[this.Grid.Model.RowCount-1,this.Grid.Model.ColCount-1].Provider;
                default:
                    return null;
            }
        }
        /// <summary>
        /// set the focus on grid which is automated
        /// </summary>
        public void SetFocus()
        {
            if (this.Grid != null)
                this.Grid.Focus();
            else if (this.DataGrid != null)
                this.DataGrid.Focus();
        }

        #endregion

        #region IRawElementProviderSimple Members

        /// <summary>
        /// Get the pattern provider by the patern id
        /// </summary>
        /// <param name="patternId"> id of value pattern</param>
        /// <returns>returns the pattern provider</returns>
        public object GetPatternProvider(int patternId)
        {
            if (patternId == SelectionPatternIdentifiers.Pattern.Id)
            {
                return this;
            }
            else if (patternId == ValuePatternIdentifiers.Pattern.Id)
            {
                return this;
            }
            /* Not Implemented */
            //else if (patternId == ScrollPatternIdentifiers.Pattern.Id)
            //{
            //    return this;
            //}
            else
                return null;
        }
        /// <summary>
        /// get the value based on the property id
        /// </summary>
        /// <param name="propertyId">id of Automation Element</param>
        /// <returns>returns the property value</returns>
        public object GetPropertyValue(int propertyId)
        {
            if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
            {
                return System.Windows.Automation.ControlType.Table.Id;
            }
            else
                return null;
        }
        /// <summary>
        /// get the grid row element provider
        /// </summary>
        public IRawElementProviderSimple HostRawElementProvider
        {
            get
            {
               return AutomationInteropProvider.HostProviderFromHandle(this.Grid.FindParentForm().Handle);
            }
        }
        /// <summary>
        /// get the automation provider options
        /// </summary>
        public ProviderOptions ProviderOptions
        {
            get 
            {
                return System.Windows.Automation.Provider.ProviderOptions.ServerSideProvider| System.Windows.Automation.Provider.ProviderOptions.ServerSideProvider;
            }
        }

        #endregion

        #region ISelectionProvider Members
        /// <summary>
        /// get the boolean value for multi select.
        /// </summary>
        public bool CanSelectMultiple
        {
            get { return true; }
        }

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <returns>null</returns>
        public IRawElementProviderSimple[] GetSelection()
        {
            return null;
        }
        /// <summary>
        /// Not Implemented.
        /// </summary>
        public bool IsSelectionRequired
        {
            get 
            { 
                return false; 
            }
        }

        #endregion

        #region IScrollProvider Members
        /// <summary>
        /// Not Implemented.
        /// </summary>
        public double HorizontalScrollPercent
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Not Implemented.
        /// </summary>
        public double HorizontalViewSize
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Not Implemented.
        /// </summary>
        public bool HorizontallyScrollable
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// scroll the grid to he purticulat amount in horizontal and vertical maner
        /// </summary>
        /// <param name="horizontalAmount">value of horizontal scroll Amount</param>
        /// <param name="verticalAmount">value of vertical scroll Amount</param>
        public void Scroll(System.Windows.Automation.ScrollAmount horizontalAmount, System.Windows.Automation.ScrollAmount verticalAmount)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// set the scroll percent of the Grid
        /// </summary>
        /// <param name="horizontalPercent">value of horizontal scroll Percent</param>
        /// <param name="verticalPercent">value of vertical scroll Percent</param>
        public void SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// gets value of vertiacal scroll as double value
        /// </summary>
        public double VerticalScrollPercent
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// gets size of vertiacal view source the double value
        /// </summary>
        public double VerticalViewSize
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// gets the bool value for the VerticallyScrollable
        /// </summary>
        public bool VerticallyScrollable
        {
            get { throw new NotImplementedException(); }
        }

        #endregion




        
    }

    /// <summary>
    /// Gris cell provider of UI for automation
    /// </summary>
    public class GridCellUIAProvider : IRawElementProviderFragment, IValueProvider, ISelectionItemProvider
    {
        GridControl Grid = null;
        GridDataBoundGrid DataGrid = null;
        GridStyleInfo Info = null;
        /// <summary>
        /// assign the control for GridCellUIAProvider
        /// </summary>
        /// <param name="grid">Control either GridControl or GridDataBoundGrid </param>
        /// <param name="info">GridStyleInfo of cell</param>
        public GridCellUIAProvider(Control grid, GridStyleInfo info)
        {
            if (grid is GridControl)
            {
                this.Grid = (GridControl)grid;

            }
            else if (grid is GridDataBoundGrid)
            {
                this.DataGrid = (GridDataBoundGrid)grid;
            }
            this.Info = info;
        }

        
        #region IRawElementProviderFragment Members
        /// <summary>
        /// get the bounding rectangle of the Grid
        /// </summary>
        public System.Windows.Rect BoundingRectangle
        {
            get 
            {
                if (this.Grid != null)
                {
                    Rectangle r = Grid.RangeInfoToRectangle(GridRangeInfo.Cell(Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex));

                    r.Intersect(Grid.ClientRectangle);

                    r = Grid.RectangleToScreen(r);
                    return new Rect(r.X, r.Y, r.Width, r.Height);
                }
                else if (this.DataGrid != null)
                {
                    Rectangle r = DataGrid.RangeInfoToRectangle(GridRangeInfo.Cell(Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex));

                    r.Intersect(DataGrid.ClientRectangle);

                    r = DataGrid.RectangleToScreen(r);

                    return new Rect(r.X, r.Y, r.Width, r.Height);
                }
                else
                    return new Rect();
            }
        }

        /// <summary>
        /// get the FragmentRoot of Raw Element Provider
        /// </summary>
        public IRawElementProviderFragmentRoot FragmentRoot
        {
            get 
            {
                if (this.Grid != null)
                    return this.Grid.Provider;
                else if (this.DataGrid != null)
                    return this.DataGrid.Provider;
                else
                    return null;
            }
        }
        /// <summary>
        /// get the collection of the Embedded FragmentRoot of Raw Element Provider
        /// </summary>
        /// <returns>returns the collections of Element ProviderSimple</returns>
        public IRawElementProviderSimple[] GetEmbeddedFragmentRoots()
        {
            return null;
        }
        /// <summary>
        /// Get the collection of Runtime ID
        /// </summary>
        /// <returns>collection of is</returns>
        public int[] GetRuntimeId()
        {
            return new int[] {Info.CellIdentity.RowIndex,Info.CellIdentity.ColIndex};
        }
        /// <summary>
        /// get the IROW element provider Fragment by the NavigateDirection
        /// </summary>
        /// <param name="direction">Navigate Direction</param>
        /// <returns>returns the Irow element provider</returns>
        public IRawElementProviderFragment Navigate(NavigateDirection direction)
        {
            switch(direction)
            {
                //case NavigateDirection.FirstChild:
                //    return this.Grid[0, 0].Provider;
                //case NavigateDirection.LastChild:
                //    return this.Grid[Grid.RowCount - 1, Grid.ColCount - 1].Provider;
                case NavigateDirection.NextSibling:
                    {
                        if (this.Grid != null)
                        {
                            if (Info.CellIdentity.ColIndex + 1 <= this.Grid.Model.ColCount)
                            {
                                return this.Grid[Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex + 1].Provider;
                            }
                            else if (Info.CellIdentity.RowIndex + 1 <= this.Grid.Model.RowCount)
                            {
                                return this.Grid[Info.CellIdentity.RowIndex + 1, 0].Provider;
                            }
                            else
                                return null;
                        }
                        else if (this.DataGrid != null)
                        {
                            if (Info.CellIdentity.ColIndex + 1 <= this.DataGrid.Model.ColCount)
                            {
                                return this.DataGrid[Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex + 1].Provider;
                            }
                            else if (Info.CellIdentity.RowIndex + 1 <= this.DataGrid.Model.RowCount)
                            {
                                return this.DataGrid[Info.CellIdentity.RowIndex + 1, 0].Provider;
                            }
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                case NavigateDirection.PreviousSibling:
                    {
                        if (Info.CellIdentity.ColIndex > 0)
                        {
                            if (this.Grid != null)
                                return this.Grid[Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex - 1].Provider;
                            else if (this.DataGrid != null)
                                return this.DataGrid[Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex - 1].Provider;
                            else
                                return null;
                        }
                        else if (Info.CellIdentity.RowIndex > 0)
                        {
                            if (this.Grid != null)
                                return this.Grid[Info.CellIdentity.RowIndex, this.Grid.Model.ColCount - 1].Provider;
                            else if (this.DataGrid != null)
                                return this.Grid[Info.CellIdentity.RowIndex, this.Grid.Model.ColCount - 1].Provider;
                            else
                                return null;
                        }
                        else
                            return null;
                    }
                default:
                    return null;
            }
        }
        /// <summary>
        /// set the focus on automation element
        /// </summary>
        public void SetFocus()
        {

        }

        #endregion

        #region IRawElementProviderSimple Members
        /// <summary>
        /// Get the pattern provider by the patern id
        /// </summary>
        /// <param name="patternId"> id of value pattern</param>
        /// <returns>returns the pattern provider</returns>
        public object GetPatternProvider(int patternId)
        {
            if (patternId == ValuePatternIdentifiers.Pattern.Id)
            {
                return this;
            }
            else if(patternId == SelectionItemPatternIdentifiers.Pattern.Id)
            {
                return this;
            }
            else
                return null;
        }
        /// <summary>
        /// get the value based on the property id
        /// </summary>
        /// <param name="propertyId">id of Automation Element</param>
        /// <returns>returns the property value</returns>
        public object GetPropertyValue(int propertyId)
        {
           
            if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id)
            {
                if (this.Info.CellType == "PushButton")
                    return this.Info.Description;
                else
                    return this.Value;
            }
            else if (propertyId == AutomationElementIdentifiers.NameProperty.Id)
            {
                return "GridCell_R" + this.Info.CellIdentity.RowIndex + "_C" + this.Info.CellIdentity.ColIndex;
            }
            else if (propertyId == AutomationElementIdentifiers.LabeledByProperty.Id)
            {
                return null;
            }
            else if (propertyId == AutomationElementIdentifiers.IsExpandCollapsePatternAvailableProperty.Id)
            {
                return false;
            }
            else if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
            {

                //GetStyleInfoProperty(this.Info, AutomationElementIdentifiers.ControlTypeProperty.ProgrammaticName, this.Grid.Handle);

                switch (Info.CellType)
                {
                    case "TextBox":
                        return System.Windows.Automation.ControlType.Text.Id;
                    case "CheckBox":
                        return System.Windows.Automation.ControlType.CheckBox.Id;
                    case "PushButton":
                        return System.Windows.Automation.ControlType.Button.Id;
                    case "NumericUpDown":
                        return System.Windows.Automation.ControlType.Spinner.Id;
                    case "ComboBox":
                        return System.Windows.Automation.ControlType.ComboBox.Id;
                    case "Header":
                    case "RowHeaderCell":
                    case "ColumnHeaderCell":
                        return System.Windows.Automation.ControlType.Header.Id;
                    case "ColorEdit":
                    case "MonthCalendar":
                    case "DropDownGrid":
                    default:
                        return System.Windows.Automation.ControlType.Custom.Id;
                }
            }
            else if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id)
            {
                return this.Info.CellIdentity;
            }
            else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
            {
                if (this.Grid != null)
                {
                    return Grid.Focused && Grid.CurrentCell.RowIndex == Info.CellIdentity.RowIndex
                        && Grid.CurrentCell.ColIndex == Info.CellIdentity.ColIndex;
                }
                else if (this.DataGrid != null)
                {
                    return DataGrid.Focused && DataGrid.CurrentCell.RowIndex == Info.CellIdentity.RowIndex
                       && DataGrid.CurrentCell.ColIndex == Info.CellIdentity.ColIndex;
                }
                else
                    return null;
            }
            else if (propertyId == AutomationElementIdentifiers.ClickablePointProperty.Id)
            {
                return null;
            }
            else if (propertyId == AutomationElementIdentifiers.ItemStatusProperty.Id)
            {
                return "Un Identified";
            }
            else if (propertyId == AutomationElementIdentifiers.IsEnabledProperty.Id)
            {
                return true;
            }
            else if (propertyId == AutomationElementIdentifiers.IsKeyboardFocusableProperty.Id)
            {
                return true;
            }
            else if (propertyId == AutomationElementIdentifiers.FrameworkIdProperty.Id)
            {
                return "Syncfusion";
            }
            else if (propertyId == AutomationElementIdentifiers.IsControlElementProperty.Id)
            {
                return true;
            }
            else if (propertyId == AutomationElementIdentifiers.IsContentElementProperty.Id)
            {
                return true;
            }
            else if (propertyId == AutomationElementIdentifiers.IsOffscreenProperty.Id)
            {
                return false;
            }
            else if (propertyId == AutomationElementIdentifiers.ItemTypeProperty.Id)
            {
                return "Syncfusion GridCell";
            }
            else if (propertyId == AutomationElementIdentifiers.LocalizedControlTypeProperty.Id)
            {
                return "Syncfsuion GridCell";
            }

            return null;
        }
        /// <summary>
        /// const value of ProviderOptionUseComThreading
        /// </summary>
        public const int ProviderOptionUseComThreading = 0x20;
        /// <summary>
        /// get the grid row element provider
        /// </summary>
        public IRawElementProviderSimple HostRawElementProvider
        {
            get 
            {
                if (this.Grid != null)
                    return this.Grid.Provider;
                else if (this.DataGrid != null)
                    return this.DataGrid.Provider;
                else
                    return null;
            }
        }
        /// <summary>
        /// get the automation provider options
        /// </summary>
        public ProviderOptions ProviderOptions
        {
            get { return (ProviderOptions)((int)ProviderOptions.ServerSideProvider | ProviderOptionUseComThreading); }//return System.Windows.Automation.Provider.ProviderOptions.ServerSideProvider; }
        }

        #endregion

        #region IValueProvider Members
        /// <summary>
        /// get the bool value if the grid is readonly
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// set the value to formated text of GridStyleInfo
        /// </summary>
        /// <param name="value">Formated Text</param>
        public void SetValue(string value)
        {
            Info.FormattedText = value;
        }
        /// <summary>
        /// get the formated text as value 
        /// </summary>
        public string Value
        {
            get { return Info.FormattedText; }
        }

        #endregion

        #region ISelectionItemProvider Members
        /// <summary>
        /// Ass item to selected item /* Not Implemented */
        /// </summary>
        public void AddToSelection()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// return the true value is item is selected /* Not Implemented */
        /// </summary>
        public bool IsSelected
        {
            get { return false; }
        }
        
        /* Not Implemented */
        /// <summary>
        /// remove the item from slection
        /// </summary>
        public void RemoveFromSelection()
        {
            
        }
        
        /* Not Implemented */
        /// <summary>
        /// select the item
        /// </summary>
        public void Select()
        {
            
        }
        /// <summary>
        /// get the selection container of grid  which holds the selected records
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                if (this.Grid != null)
                    return Grid.Provider;
                else if (this.DataGrid != null)
                    return DataGrid.Provider;
                else
                    return null;
            }
        }

        #endregion

    }
}

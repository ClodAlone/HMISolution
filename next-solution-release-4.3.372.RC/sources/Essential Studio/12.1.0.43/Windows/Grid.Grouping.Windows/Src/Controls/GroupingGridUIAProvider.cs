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

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// Provide the grouping grid UIAprovider class.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GroupingGridUIAProvider : IRawElementProviderFragmentRoot, ISelectionProvider, IScrollProvider
    {
        GridTableControl Grid = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        public GroupingGridUIAProvider(GridTableControl grid)
        {
            this.Grid = grid;
        }

        #region IRawElementProviderFragmentRoot Members
        /// <summary>
        /// Determine the raw element provider.
        /// </summary>
        /// <param name="x">double</param>
        /// <param name="y">double</param>
        /// <returns></returns>
        public IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
        {
            System.Drawing.Point p = new System.Drawing.Point((int)x, (int)y);

            p = Grid.PointToClient(p);

            GridRangeInfo info = this.Grid.PointToRangeInfo(p);

            return this.Grid.Model[info.Top, info.Left].Provider;
        }
        /// <summary>
        /// Determine the raw element provider focus.
        /// </summary>
        /// <returns></returns>
        public IRawElementProviderFragment GetFocus()
        {
            return this.Grid.Model[Grid.Model.CurrentCellInfo.RowIndex, Grid.Model.CurrentCellInfo.ColIndex].Provider;
        }

        #endregion

        #region IRawElementProviderFragment Members
        /// <summary>
        /// Determine the bound rectangle
        /// </summary>
        public System.Windows.Rect BoundingRectangle
        {
            get
            {
                    Rectangle rect = this.Grid.RectangleToScreen(this.Grid.ClientRectangle);
                    return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
            }
        }

        /// <summary>
        /// Determine the raw element provider frament root.
        /// </summary>
        public IRawElementProviderFragmentRoot FragmentRoot
        {
            get
            {
                return this;
            }
        }
        /// <summary>
        /// Gets the embedded fragment roots.
        /// </summary>
        /// <returns></returns>
        public IRawElementProviderSimple[] GetEmbeddedFragmentRoots()
        {
            return null;
        }
        /// <summary>
        /// Gets the runtime id.
        /// </summary>
        /// <returns></returns>
        public int[] GetRuntimeId()
        {
            return new int[] { this.Grid.GetHashCode() };
        }
        /// <summary>
        /// Gets the raw element provider.
        /// </summary>
        /// <param name="direction">Navigate direction</param>
        /// <returns>Raw element provider fragment</returns>
        public IRawElementProviderFragment Navigate(NavigateDirection direction)
        {
            switch (direction)
            {
                case NavigateDirection.Parent:
                    return this.Grid.Model[0, 0].Provider;
                case NavigateDirection.FirstChild:
                    return this.Grid.Model[0, 0].Provider;
                case NavigateDirection.LastChild:
                    return this.Grid.Model[this.Grid.Model.RowCount - 1, this.Grid.Model.ColCount - 1].Provider;
                default:
                    return null;
            }
        }
        /// <summary>
        /// Determine set the focus
        /// </summary>
        public void SetFocus()
        {
            this.Grid.Focus();
        }

        #endregion

        #region IRawElementProviderSimple Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patternId"></param>
        /// <returns></returns>
        public object GetPatternProvider(int patternId)
        {
            if (patternId == ValuePatternIdentifiers.Pattern.Id)
            {
                return this;
            }
            else  if (patternId == SelectionPatternIdentifiers.Pattern.Id)
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
        /// 
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public object GetPropertyProvider(int propertyId)
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public object GetPropertyValue(int propertyId)
        {
            if (propertyId == AutomationElementIdentifiers.ControlTypeProperty.Id)
            {
                return ControlType.Table.Id;
            }
            else
                return null;
        }
        /// <summary>
        /// 
        /// </summary>
        public IRawElementProviderSimple HostRawElementProvider
        {
            get
            {
                return AutomationInteropProvider.HostProviderFromHandle(this.Grid.FindParentForm().Handle);
            }
        }
        /// <summary>
        /// Gets the provider options.
        /// </summary>
        public ProviderOptions ProviderOptions
        {
            get
            {
                return System.Windows.Automation.Provider.ProviderOptions.ServerSideProvider;
            }
        }

       

        #endregion

        #region ISelectionProvider Members
        /// <summary>
        /// Gets can select multiple.
        /// </summary>
        public bool CanSelectMultiple
        {
            get { return true; }
        }

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <returns></returns>
        public IRawElementProviderSimple[] GetSelection()
        {
            return null;
        }
        /// <summary>
        /// Gets the selection required.
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
        /// Gets the horizontal scroll precent.
        /// </summary>
        public double HorizontalScrollPercent
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Gets the horizontal view size.
        /// </summary>
        public double HorizontalViewSize
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool HorizontallyScrollable
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// Determine the scroll positions.
        /// </summary>
        /// <param name="horizontalAmount">horizontal amount</param>
        /// <param name="verticalAmount">vertical amount</param>
        public void Scroll(System.Windows.Automation.ScrollAmount horizontalAmount, System.Windows.Automation.ScrollAmount verticalAmount)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// NotImplemented
        /// </summary>
        /// <param name="horizontalPercent"></param>
        /// <param name="verticalPercent"></param>
        public void SetScrollPercent(double horizontalPercent, double verticalPercent)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// NotImplemented
        /// </summary>
        public double VerticalScrollPercent
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// NotImplemented
        /// </summary>
        public double VerticalViewSize
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// NotImplemented
        /// </summary>
        public bool VerticallyScrollable
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

    }




    /// <summary>
    /// 
    /// </summary>
    public class GroupingGridCellUIAProvider : IRawElementProviderFragment, IValueProvider, ISelectionItemProvider
    {
        GridTableControl Grid = null;
        GridTableCellStyleInfo Info = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="info"></param>
        public GroupingGridCellUIAProvider(Control grid, GridTableCellStyleInfo info)
        {
            this.Grid = (GridTableControl)grid;
            this.Info = info;
        }


        #region IRawElementProviderFragment Members
        /// <summary>
        /// 
        /// </summary>
        public System.Windows.Rect BoundingRectangle
        {
            get
            {
                Rectangle r = Grid.RangeInfoToRectangle(GridRangeInfo.Cell(Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex));

                r.Intersect(Grid.ClientRectangle);

                r = Grid.RectangleToScreen(r);

                return new Rect(r.X, r.Y, r.Width, r.Height);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IRawElementProviderFragmentRoot FragmentRoot
        {
            get
            {
                return this.Grid.Provider;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IRawElementProviderSimple[] GetEmbeddedFragmentRoots()
        {
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int[] GetRuntimeId()
        {
            return new int[] { Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public IRawElementProviderFragment Navigate(NavigateDirection direction)
        {
            switch (direction)
            {
                //case NavigateDirection.FirstChild:
                //    return this.Grid[0, 0].Provider;
                //case NavigateDirection.LastChild:
                //    return this.Grid[Grid.RowCount - 1, Grid.ColCount - 1].Provider;
                case NavigateDirection.NextSibling:
                    {
                        if (Info.CellIdentity.ColIndex + 1 < this.Grid.Model.ColCount)
                            {
                                return this.Grid.Model[Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex + 1].Provider;
                            }
                            else if (Info.CellIdentity.RowIndex + 1 < this.Grid.Model.RowCount)
                            {
                                return this.Grid.Model[Info.CellIdentity.RowIndex + 1, 0].Provider;
                            }
                            else
                                return null;
                    }
                case NavigateDirection.PreviousSibling:
                    {
                        if (Info.CellIdentity.ColIndex > 0)
                        {
                            return this.Grid.Model[Info.CellIdentity.RowIndex, Info.CellIdentity.ColIndex - 1].Provider;
                        }
                        else if (Info.CellIdentity.RowIndex > 0)
                        {
                            return this.Grid.Model[Info.CellIdentity.RowIndex, this.Grid.Model.ColCount - 1].Provider;
                        }
                        else
                            return null;
                    }
                default:
                    return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void SetFocus()
        {

        }

        #endregion

        #region IRawElementProviderSimple Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patternId"></param>
        /// <returns></returns>
        public object GetPatternProvider(int patternId)
        {
            if (patternId == ValuePatternIdentifiers.Pattern.Id)
            {
                return this;
            }
            else if (patternId == SelectionItemPatternIdentifiers.Pattern.Id)
            {
                return this;
            }
            else
                return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        public object GetPropertyValue(int propertyId)
        {
            if (propertyId == ValuePatternIdentifiers.ValueProperty.Id)
            {
                return this.Info.CellValue;
            }
            else if (propertyId == SelectionItemPatternIdentifiers.ElementAddedToSelectionEvent.Id)
            {
                return null;
            }
            else if (propertyId == AutomationElementIdentifiers.HelpTextProperty.Id)
            {
                return this.Info.CellTipText;
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
                //return Value;
                switch (Info.CellType)
                {
                    case "TextBox":
                        return ControlType.Text.Id;
                    case "CheckBox":
                        return ControlType.CheckBox.Id;
                    case "PushButton":
                        return ControlType.Button.Id;
                    case "NumericUpDown":
                        return ControlType.Spinner.Id;
                    case "ComboBox":
                        return ControlType.ComboBox.Id;
                    case "Header":
                    case "RowHeaderCell":
                    case "ColumnHeaderCell":
                        return ControlType.HeaderItem.Id;
                    case "ColorEdit":
                    case "MonthCalendar":
                    case "DropDownGrid":
                    default:
                        return ControlType.Custom.Id;
                }
            }
            else if (propertyId == AutomationElementIdentifiers.AutomationIdProperty.Id)
            {
                return this.Info.CellIdentity;
            }
            else if (propertyId == AutomationElementIdentifiers.HasKeyboardFocusProperty.Id)
            {
                return Grid.Focused && Grid.CurrentCell.RowIndex == Info.CellIdentity.RowIndex;
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
        /// 
        /// </summary>
        public IRawElementProviderSimple HostRawElementProvider
        {
            get
            {
                return this.Grid.Provider;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public ProviderOptions ProviderOptions
        {
            get { return System.Windows.Automation.Provider.ProviderOptions.ServerSideProvider; }
        }


        #endregion

        #region IValueProvider Members
        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">string value</param>
        public void SetValue(string value)
        {
            Info.FormattedText = value;
        }
        /// <summary>
        /// 
        /// </summary>
        public string Value
        {
            get { return Info.FormattedText; }
        }

        #endregion

        #region ISelectionItemProvider Members
        /// <summary>
        /// 
        /// </summary>
        public void AddToSelection()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsSelected
        {
            get { return false; }
        }
        
        /// <summary>
        /// /* Not Implemented */
        /// </summary>
        public void RemoveFromSelection()
        {

        }

        /// <summary>
        /// /* Not Implemented */
        /// </summary>
        public void Select()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public IRawElementProviderSimple SelectionContainer
        {
            get
            {
                return Grid.Provider;
            }
        }

        #endregion
    }
}

#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
using Syncfusion.Olap.Engine;
using System.Windows.Media; 
namespace Syncfusion.Windows.Grid.Olap
#else
using Syncfusion.OlapSilverlight.Engine;
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.OlapSilverlight.Data;
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System;
    using System.Windows;
    using Syncfusion.Windows.Controls.Grid;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Threading;
    using System.Windows.Controls;
    

    public class OlapGridExpandHyperlinkCellModel : GridCellModel<OlapGridExpandHyperlinkCellRenderer>
    {
        /// <summary>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">Vertical or horizontal</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <overload>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </overload>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            OlapGridCellStyleInfoIdentity styleInfoIdentity = style.Tag as OlapGridCellStyleInfoIdentity;
            if (styleInfoIdentity != null)
            {
                if (styleInfoIdentity.Style != null)
                {
                    OlapGridExpandHyperlinkCell cellControl = new OlapGridExpandHyperlinkCell();
                    cellControl.CellDescriptor = styleInfoIdentity.CellDescriptor;
                    cellControl.DataContext = styleInfoIdentity.CellDescriptor;
                    cellControl.Style = styleInfoIdentity.Style;
                    cellControl.Measure(new Size(double.MaxValue, double.MaxValue));
                    return cellControl.DesiredSize;
                }
                else
                {
                    Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
                    if (clientSize.IsEmpty)
                        return Size.Empty;

                    Thickness margins = style.TextMargins.ToThickness();
                    // TextBoxView always seems to have this margin and I am not able to reset the margin.
                    // Therefore I am also hard-codeing it here so that TextBox behavior is properly
                    // emulated.
                    margins.Left = Math.Max(margins.Left, 2);
                    margins.Right = Math.Max(margins.Right, 2);

                    Size size = AddBorderMargins(clientSize, margins);
                    size = AddBorderMargins(size, style.BorderMargins.ToThickness());
#if SILVERLIGHT
                    size.Width += 30;
#else
                    size.Width += 20;
#endif
                    return size;
                }
            }
            return new Size(10, 10);
        }
        
        /// <summary>
        /// Gets the formatted text.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="value">The value.</param>
        /// <param name="textInfo">The text info.</param>
        /// <returns></returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            var cellIdentity = style.Tag as OlapGridCellStyleInfoIdentity;
            return cellIdentity.CellDescriptor.CellValue;
        }
    }

    public class OlapGridExpandHyperlinkCellRenderer : GridVirtualizingCellRenderer<OlapGridExpandHyperlinkCell>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapGridExpandHyperlinkCellRenderer"/> class.
        /// </summary>
        public OlapGridExpandHyperlinkCellRenderer()
        {
            this.AllowRecycle = false;   
        }
#if !SILVERLIGHT
        /// <summary>
        /// Creates the renderer element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void CreateRendererElement(OlapGridExpandHyperlinkCell uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);
            OlapGridCellStyleInfoIdentity cellIdentity = style.ModelStyle.Tag as OlapGridCellStyleInfoIdentity;
            if (cellIdentity != null)
            {
                uiElement.CellDescriptor = cellIdentity.CellDescriptor;
                OlapGridBase GridBase = this.GridControl as OlapGridBase;
                uiElement.GridControl = GridBase.OlapGrid;
                uiElement.IsHyperlinkCell = cellIdentity.IsHyperlinkCell;
                if (cellIdentity.Style != null)
                    uiElement.Style = cellIdentity.Style;
                uiElement.DataContext = cellIdentity.CellDescriptor;
                if (cellIdentity.CellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                {
                    double leftMargin = double.Parse(cellIdentity.GridModel[cellIdentity.CellRowColumnIndex.RowIndex, cellIdentity.CellRowColumnIndex.ColumnIndex].TextMargins.Left.ToString());
                    uiElement.Margin = new Thickness(leftMargin, 5, 0, 0);
                }
                else
                {
                    uiElement.Margin = new Thickness(0, 5, 0, 0);
                }

                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.HorizontalAlignment = HorizontalAlignment.Left;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
#if !SILVERLIGHT
                if (style.FlowDirection == FlowDirection.RightToLeft)
                {
                    uiElement.FlowDirection = style.FlowDirection;
                    double m11 = -1;
                    double m22 = 1;
                    double offsetX = uiElement.ActualWidth;
                    double offsetY = 0;
                    uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY);
                }
#endif

                UnwireAndWireExpandClicked(uiElement);
                if (cellIdentity.IsHyperlinkCell)
                {
                    UnwireAndWireLinkClicked(uiElement);
                }
            }
            else
            {
                UnwireLinkClicked(uiElement);
            }
            VisualContainer.SetWantsMouseInput(uiElement, true);
        }
#endif
        /// <summary>
        /// Called when [initialize content].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void OnInitializeContent(OlapGridExpandHyperlinkCell uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            OlapGridCellStyleInfoIdentity cellIdentity = style.ModelStyle.Tag as OlapGridCellStyleInfoIdentity;
            if (cellIdentity != null)
            {
                uiElement.CellDescriptor = cellIdentity.CellDescriptor;
                OlapGridBase GridBase = this.GridControl as OlapGridBase;
                uiElement.GridControl = GridBase.OlapGrid;
                uiElement.IsHyperlinkCell = cellIdentity.IsHyperlinkCell;
                if (cellIdentity.Style != null)
                    uiElement.Style = cellIdentity.Style;
                uiElement.DataContext = cellIdentity.CellDescriptor;
                if (cellIdentity.CellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                {
                    double leftMargin = double.Parse(cellIdentity.GridModel[cellIdentity.CellRowColumnIndex.RowIndex, cellIdentity.CellRowColumnIndex.ColumnIndex].TextMargins.Left.ToString());
                    uiElement.Margin = new Thickness(leftMargin, 5, 0, 0);
                }
                else
                {
                    uiElement.Margin = new Thickness(0, 5, 0, 0);
                }

                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.HorizontalAlignment = HorizontalAlignment.Left;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;

                UnwireAndWireExpandClicked(uiElement);
                if (cellIdentity.IsHyperlinkCell)
                {
                    UnwireAndWireLinkClicked(uiElement);
                }
            }
            else
            {
                UnwireLinkClicked(uiElement);
            }
            VisualContainer.SetWantsMouseInput(uiElement, true);
        }

        /// <summary>
        /// Called when [element measured].
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="size">The size.</param>
        protected override void OnElementMeasured(System.Windows.UIElement el, System.Windows.Size size)
        {
            el.Dispatcher.BeginInvoke(new Action(() =>
            {
                el.Measure(size);
            }), null);
        }

        /// <summary>
        /// Called when [element arranged].
        /// </summary>
        /// <param name="el">The el.</param>
        /// <param name="rect">The rect.</param>
        protected override void OnElementArranged(System.Windows.UIElement el, System.Windows.Rect rect)
        {
            el.Dispatcher.BeginInvoke(new Action(() =>
            {
                el.Arrange(rect);
            }), null);
            base.OnElementArranged(el, rect);
        }

        /// <summary>
        /// Handles the ExpanderClicked event of the uiElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        void uiElement_ExpanderClicked(object sender, OlapGridDrillDownEventArgs e)
        {
            OlapGridBase gridBase = this.GridControl as OlapGridBase;
            if (gridBase != null)
            {
#if SILVERLIGHT
                gridBase.OlapGrid.IsProcessing = true;
#endif
                if (gridBase.Model != null)
                {
                    gridBase.RaiseBeforeRefresh(e);
                    gridBase.Model.GridExpanderClick(e.CellDescriptor, e);
                }
            }
        }

        /// <summary>
        /// Handles the LinkClicked event of the uiElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Grid.Olap.LinkLabelEventArgs"/> instance containing the event data.</param>
        void uiElement_LinkClicked(object sender, LinkLabelEventArgs e)
        {
            OlapGridBase gridBase = this.GridControl as OlapGridBase;
            if (gridBase != null)
            {
                gridBase.RaiseLabelClick(e);
            }
        }

        /// <summary>
        /// Unwires the and wire expand clicked.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        private void UnwireAndWireExpandClicked(OlapGridExpandHyperlinkCell uiElement)
        {
            uiElement.ExpanderClicked -= new OlapGridDrillDownEventHander(uiElement_ExpanderClicked);
            uiElement.ExpanderClicked += new OlapGridDrillDownEventHander(uiElement_ExpanderClicked);
#if SILVERLIGHT
            uiElement.CellClicked += new OlapGridDrillDownEventHander(uiElement_CellClicked);
#endif
        }

#if SILVERLIGHT
        /// <summary>
        /// Handles the CellClicked event of the uiElement control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Silverlight.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        void uiElement_CellClicked(object sender, OlapGridDrillDownEventArgs e)
        {
            OlapGridBase gridBase = this.GridControl as OlapGridBase;
            if (gridBase != null)
            {
                if (gridBase.OlapGrid.OlapDataManager.CurrentReport.DrillType == DrillType.DrillReplace)
                {
                    gridBase.OlapGrid.IsProcessing = true;
                    if (e.CellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                    {
                        Items items = gridBase.OlapGrid.OlapDataManager.CurrentReport.SeriesElements;
                        ProcessCellClick(items, e);
                    }
                    if (e.CellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                    {
                        Items items = gridBase.OlapGrid.OlapDataManager.CurrentReport.CategoricalElements;
                        ProcessCellClick(items, e);
                    }
                    gridBase.OlapGrid.OlapDataManager.NotifyReportChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the previous member element.
        /// </summary>
        /// <value>The previous member element.</value>
        private MemberElement PreviousMemberElement
        {
            get;
            set;
        }

        /// <summary>
        /// Processes the cell click.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="e">The <see cref="Syncfusion.Silverlight.Grid.Olap.OlapGridDrillDownEventArgs"/> instance containing the event data.</param>
        private void ProcessCellClick(Items items, OlapGridDrillDownEventArgs e)
        {
            foreach (Item item in items)
            {
                if (item.ElementValue is DimensionElement)
                {
                    if ((item.ElementValue as DimensionElement).Hierarchy.UniqueName == (e.CellDescriptor.Tag as Member).UniqueName.Split('.')[0] + "." + (e.CellDescriptor.Tag as Member).UniqueName.Split('.')[1])
                    {
                        MemberElement memberElement = null;
                        foreach (LevelElement _levelElement in ((item.ElementValue as DimensionElement).Hierarchy.LevelElements as LevelElementCollection))
                        {
                            if (_levelElement.MemberElements.Count != 0)
                            {
                                GetMemberElement(_levelElement.MemberElements, e.CellDescriptor.Tag as Member);
                                if (PreviousMemberElement.ParentMemberElement != null)
                                {
                                    PreviousMemberElement.ParentMemberElement.ChildMemberElements.Clear();
                                }
                                if (PreviousMemberElement.ParentMemberElement == null && PreviousMemberElement.IsParentLevel == true)
                                {
                                    _levelElement.MemberElements.Clear();
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the previous member element.
        /// </summary>
        /// <param name="memberElementCollection">The member element collection.</param>
        /// <param name="member">The member.</param>
        private void GetMemberElement(MemberElementCollection memberElementCollection, Member member)
        {
            foreach (MemberElement _memberElement in memberElementCollection)
            {
                if (_memberElement.UniqueName != member.UniqueName && _memberElement.ChildMemberElements.Count > 0)
                    GetMemberElement(_memberElement.ChildMemberElements, member);
                else
                    PreviousMemberElement = _memberElement;
            }
        }
#endif
        /// <summary>
        /// Unwires the and wire link clicked.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        private void UnwireAndWireLinkClicked(OlapGridExpandHyperlinkCell uiElement)
        {
            uiElement.LinkClicked -= new LinkLabelClickEventHander(uiElement_LinkClicked);
            uiElement.LinkClicked += new LinkLabelClickEventHander(uiElement_LinkClicked);
        }

        /// <summary>
        /// Unwires the link clicked.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        private void UnwireLinkClicked(OlapGridExpandHyperlinkCell uiElement)
        {
            uiElement.LinkClicked -= new LinkLabelClickEventHander(uiElement_LinkClicked);
        }
    }
}

#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using Syncfusion.Windows.Controls.Grid;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;

#if !SILVERLIGHT
using Syncfusion.PivotAnalysis.Base;
using Syncfusion.Windows.Controls.PivotGrid.Resources;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// A cell model for the <see cref="PivotGridHyperlinkCellRenderer"/>.
    /// </summary>
  
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotGridRowGroupBarModel : GridCellModel<PivotGridRowGroupBarRenderer>
    {
        /// <summary>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="T:Syncfusion.Windows.Controls.Grid.GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">Vertical or horizontal</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <overload>
        /// Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </overload>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            PivotGridStyleInfoIdentity styleInfoIdentity = style.CellIdentity as PivotGridStyleInfoIdentity;
            if (styleInfoIdentity != null)
            {
#if !SILVERLIGHT
                if (styleInfoIdentity.Style != null)
                {
                    PivotGridRowGroupBar cellControl = new PivotGridRowGroupBar();
                    return cellControl.DesiredSize;
                }
                else
                {
                    Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
                    if (clientSize.IsEmpty)
                        return Size.Empty;

                    Thickness margins = style.TextMargins.ToThickness();

                    margins.Left = Math.Max(margins.Left, 2);
                    margins.Right = Math.Max(margins.Right, 2);

                    Size size = AddBorderMargins(clientSize, margins);
                    size = AddBorderMargins(size, style.BorderMargins.ToThickness());
                    size.Width += 20;
                    return size;

                }
#else
                Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
                if (clientSize.IsEmpty)
                    return Size.Empty;

                Thickness margins = style.TextMargins.ToThickness();

                margins.Left = Math.Max(margins.Left, 2);
                margins.Right = Math.Max(margins.Right, 2);

                Size size = AddBorderMargins(clientSize, margins);
                size = AddBorderMargins(size, style.BorderMargins.ToThickness());
                size.Width += 20;
                return size;
#endif
            }
            return new Size(10, 10);
        }
    }
    /// <summary>
    /// Implements the renderer part of a PivotGrid row grouping bar cell.
    /// </summary>
 
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotGridRowGroupBarRenderer : GridVirtualizingCellRenderer<PivotGridRowGroupBar>
    {

#if !SILVERLIGHT
        /// <summary>
        /// Creates the renderer element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void CreateRendererElement(PivotGridRowGroupBar uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);
            uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
            if (uiElement.GridControlBase != null)
            {
                uiElement.GridControl = uiElement.GridControlBase.GridControl;
            }
#if !SILVERLIGHT
            if (this.GridControl.FlowDirection == FlowDirection.RightToLeft)
            {
                uiElement.FlowDirection = this.GridControl.FlowDirection;
                double m11 = -1;
                double m22 = 1;
                double offsetX = uiElement.ActualWidth;
                double offsetY = 0;
                uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY);
            }
#endif
        } 
#endif

        /// <summary>
        /// Called when [initialize content].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void OnInitializeContent(PivotGridRowGroupBar uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
            if (uiElement.GridControlBase != null)
            {
                uiElement.GridControl = uiElement.GridControlBase.GridControl;
            }
            uiElement.FlowDirection = this.GridControl.FlowDirection;
        }
#if !SILVERLIGHT
        /// <summary>
        /// arrange the cells UIElement children. 
        /// The UIElement is arranged on the canvas with a call to <see cref="ArrangeUIElement"/>.
        /// </summary>
        /// <param name="aca">The arange cell layout information.</param>
        /// <param name="uiElement">The PivotGridRowGroupBar</param>
        /// <param name="style">The cell style info.</param>
        protected override void ArrangeUIElement(Cells.ArrangeCellArgs aca, PivotGridRowGroupBar uiElement, GridRenderStyleInfo style)
        {
            base.ArrangeUIElement(aca, uiElement, style); 
            uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
            if (uiElement.GridControlBase != null)
            {
                uiElement.GridControl = uiElement.GridControlBase.GridControl;
            }
            if (this.GridControl.FlowDirection == FlowDirection.RightToLeft)
            {
                double m11 = -1;
                double m22 = 1;
                double offsetX = uiElement.ActualWidth;
                double offsetY = 0;
                uiElement.LayoutTransform = new MatrixTransform(m11, 0, 0, m22, offsetX, offsetY);
            }
        }
#endif
    }
    /// <summary>
    ///A ContentControl derived class that aggregates the functionality of a PivotGridRowGroupBar object into a content control
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotGridRowGroupBar : ContentControl
    {
#if !SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="PivotGridRowGroupBar"/> class.
        /// </summary>
        static PivotGridRowGroupBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotGridRowGroupBar), new FrameworkPropertyMetadata(typeof(PivotGridRowGroupBar)));
        }
#else

        /// <summary>
        /// Initializes the <see cref="PivotGridRowGroupBar"/> class.
        /// </summary>
        public PivotGridRowGroupBar()
        {
            DefaultStyleKey = typeof(PivotGridRowGroupBar);
        }
#endif
        /// <summary>
        /// Gets or sets the grid control base.
        /// </summary>
        /// <value>The grid control base.</value>
        public PivotGridControlBase GridControlBase { get; set; }

        /// <summary>
        /// Gets or sets the Grid control.
        /// </summary>
        /// <value>The grid control.</value>
        public PivotGridControl GridControl { get; set; }

        /// <summary>
        /// Gets or sets the row list.
        /// </summary>
        /// <value>The row list.</value>
        public PivotGroupingItemsControl RowList { get; set; }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.RowList = GetTemplateChild("PART_RowList") as PivotGroupingItemsControl;
#if !SILVERLIGHT
            this.RowList.QueryContinueDrag += new QueryContinueDragEventHandler(RowList_QueryContinueDrag);
#endif

            if (RowList != null)
            {
                if (this.GridControl.PivotRows.Count > 0)
                {
                    if (this.GridControl.GroupingBar.AllowSorting)
                    {
                        this.RowList.Items.Clear();
                        this.RowList.DataContext = this.GridControl.PivotRows;
                        this.RowList.ItemsSource = this.GridControl.PivotRows;
                        this.GridControl.GroupingBar.RowHeaderArea = RowList;
                    }
                    else
                    {
                        ResourceDictionary resource = new ResourceDictionary()
                        {
#if !SILVERLIGHT
                             Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                             Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                        };

                        this.RowList.Items.Clear();
                        this.RowList.DataContext = this.GridControl.PivotRows;
                        this.RowList.ItemTemplate = resource["PivotRowItemTemplateWithoutSort"] as DataTemplate;
                        this.RowList.ItemsSource = this.GridControl.PivotRows;
                        this.GridControl.GroupingBar.RowHeaderArea = RowList;
                    }
                }
                else
                {
                    this.RowList.ItemTemplate = null;
#if !SILVERLIGHT
                    ResourceWrapperKeys rsWrapperKeys = new ResourceWrapperKeys();
                    
                    this.RowList.Items.Add(new TextBlock()
                    {
                        Text = rsWrapperKeys.pivotGridDropRowFields,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new Thickness(3, 0, 0, 0),
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 12
                    });
#else


                        this.RowList.Items.Add(new TextBlock()
                        {
                            Text = "Drop row fields here",
                            VerticalAlignment = System.Windows.VerticalAlignment.Center,
                            Margin = new Thickness(3, 0, 0, 0),
                            FontFamily = new FontFamily("Segoe UI"),
                            FontSize = 12
                        });


#endif 
                    this.GridControl.GroupingBar.RowHeaderArea = RowList;
                }

                this.GridControlBase.LayoutUpdated += new EventHandler(GridControlBase_LayoutUpdated);
                  
#if !SILVERLIGHT

                //// Adding delete filter item command
                CommandBindings.Add(new CommandBinding(PivotGridCommands.SortPivotItem, this.GridControlBase.GridControl.GroupingBar.SortPivotItemExecuted, this.GridControlBase.GridControl.GroupingBar.SortPivotItemCanExecute));
                //// Adding show filter popop command
                CommandBindings.Add(new CommandBinding(PivotGridCommands.ShowFilter, this.GridControlBase.GridControl.GroupingBar.ShowFilterExecuted, this.GridControlBase.GridControl.GroupingBar.ShowFilterCanExecute));

                CommandBindings.Add(new CommandBinding(PivotGridCommands.ShowFieldList, this.GridControlBase.GridControl.GroupingBar.ShowFieldListExecuted, this.GridControlBase.GridControl.GroupingBar.ShowFieldListCanExecute));

                CommandBindings.Add(new CommandBinding(PivotGridCommands.ReloadData, this.GridControlBase.GridControl.GroupingBar.ReloadDataExecuted, this.GridControlBase.GridControl.GroupingBar.ReloadDataCanExecute));

                CommandBindings.Add(new CommandBinding(PivotGridCommands.Order, this.GridControlBase.GridControl.GroupingBar.OrderExecuted, this.GridControlBase.GridControl.GroupingBar.OrderCanExecute));

                CommandBindings.Add(new CommandBinding(PivotGridCommands.DeleteItem, this.GridControlBase.GridControl.GroupingBar.DeleteItemExecuted, this.GridControlBase.GridControl.GroupingBar.DeleteItemCanExecute));

                this.ContextMenuOpening += new ContextMenuEventHandler(PivotGridRowGroupBar_ContextMenuOpening);

#else
                this.RowList.CommandExecute += new CommandExecuteChanged(this.GridControlBase.GridControl.GroupingBar.PivotItem_CommandExecute);
#endif
            }

            if (this.GridControl.GroupingBar.ColumnHeaderArea != null && this.GridControl.GroupingBar.ColumnHeaderArea.ItemsSource == null)
            {
                if (this.GridControl.PivotColumns.Count > 0)
                {
                    ResourceDictionary resource = new ResourceDictionary()
                            {
#if !SILVERLIGHT
                                Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                                Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                            };

                    this.GridControl.GroupingBar.ColumnHeaderArea.Items.Clear();
                    this.GridControl.GroupingBar.ColumnHeaderArea.DataContext = this.GridControl.PivotColumns;
                    this.GridControl.GroupingBar.ColumnHeaderArea.ItemsSource = this.GridControl.PivotColumns;
                    this.GridControl.GroupingBar.ColumnHeaderArea.ItemTemplate = resource["PivotColumnItemTemplate"] as DataTemplate;
                }
                else
                {
                    this.GridControl.GroupingBar.ColumnHeaderArea.ItemTemplate = null;
                }
            }

            if (this.GridControl != null && this.GridControl.GroupingBar != null)
            {

                this.GridControl.GroupingBar.ApplyEmptyTemplate();
#if !SILVERLIGHT
                if (this.GridControl.GroupingBar.FieldList != null && this.GridControl.PivotFields.Count > 0 && this.GridControl.GroupingBar.FieldList.PivotItemPanel.ItemTemplate == null)
                {
                    this.GridControl.GroupingBar.FieldList.PivotItemPanel.ItemTemplate = this.GridControl.GroupingBar.FieldList.Resources["PivotItemTemplate"] as DataTemplate;
                    this.GridControl.GroupingBar.FieldList.PivotItemPanel.DataContext = this.GridControl.PivotFields;
                    this.GridControl.GroupingBar.FieldList.PivotItemPanel.ItemsSource = this.GridControl.PivotFields;
                }
#endif 
            }
        }


#if !SILVERLIGHT
        /// <summary>
        /// Calls when the item in the row grouping bar renders
        /// </summary>
        /// <param name="drawingContext">DrawingContext</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                if(this.GridControl.GroupingBar!=null)
                for (int i = 0; i < this.GridControl.GroupingBar.RowHeaderArea.Items.Count; i++)
                {
                    ListBoxItem item = this.GridControl.GroupingBar.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                    {
                        PivotGridGroupingBar.SetDisabled(item, this.GridControl);
                    }
                }
            }
            base.OnRender(drawingContext);
        }
        void PivotGridRowGroupBar_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            this.Focus();
        }

        void RowList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (this.GridControl.GroupingBar != null && this.GridControl.GroupingBar.FieldList != null &&
                this.GridControl.GroupingBar.FieldList.PivotItemPanel != null && !this.GridControl.GroupingBar.FieldList.IsDrag)
            {
                this.GridControl.GroupingBar.FieldList.RemoveAdorner(AdornerLayer.GetAdornerLayer(this.GridControl.GroupingBar.FieldList.PivotItemPanel));
            }
        }

#endif

        /// <summary>
        /// Handles the LayoutUpdated event of the GridControlBase control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void GridControlBase_LayoutUpdated(object sender, EventArgs e)
        {
            CalculatePivotRowItemWidth();
            if (this.GridControlBase.PivotEngine.PivotRows.Count > 0 || !this.GridControl.AllowRowHeaderAreaAutoSizing)
            {
                this.GridControl.GroupingBar.CalculateComputationInfoWidth();
            }
#if SILVERLIGHT
    this.GridControl.GroupingBar.CalculateComputationInfoItemsWidth();
#endif
      //  this.GridControl.CheckScrollBarVisibility();
            //this.GridControlBase.GridControl.GroupingBar.RowHeaderArea.LayoutUpdated += new EventHandler(RowHeaderArea_LayoutUpdated);
            this.GridControlBase.LayoutUpdated -= new EventHandler(GridControlBase_LayoutUpdated);
        }

        //void RowHeaderArea_LayoutUpdated(object sender, EventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    this.CalculatePivotRowItemWidth();
        //}

        /// <summary>
        /// Calculates the width of the pivot row item.
        /// </summary>
        public void CalculatePivotRowItemWidth()
        {
            ItemContainerGenerator containerGenerator = this.RowList.ItemContainerGenerator;
#if !SILVERLIGHT
            if (containerGenerator != null && containerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
#else
            if(containerGenerator !=null)
#endif

            {
                if (this.GridControlBase.PivotEngine.PivotRows.Count > 0)
                {
                    for (int i = 0; i < this.GridControlBase.PivotEngine.PivotRows.Count; i++)
                    {
                        PivotItem pivotItem = this.GridControlBase.PivotEngine.PivotRows[i];
                        Size size = Common.GetTextSize(pivotItem.FieldHeader);
                        double columnWidth = this.GridControlBase.Model.ColumnWidths[i];
#if SILVERLIGHT
                        double requiredWidth = 50 + size.Width;
#else
                        double requiredWidth = 50 + size.Width;
#endif
                        if (requiredWidth > columnWidth && this.GridControlBase.Model.ColumnWidths.LineCount >i)
                        {
                            //if (requiredWidth < this.GridControlBase.PivotEngine.PivotCalculations.Count * 40)
                            //{
                            //    //requiredWidth = this.GridControlBase.PivotEngine.PivotCalculations.Count * 40;
                            //}

                            this.GridControlBase.Model.ColumnWidths[i] = requiredWidth;
                        }

                        ListBoxItem item = containerGenerator.ContainerFromIndex(i) as ListBoxItem;

                        if (item != null)
                        {
//#if !SILVERLIGHT
                           
                            if (i < (this.GridControlBase.PivotEngine.PivotRows.Count - 1))
                            {
                                item.Width = this.GridControlBase.Model.ColumnWidths[i];

                                if (i == 0)
                                {
                                    item.Width -= 1;
                                    item.Margin = new Thickness(1, 0, 0, 0);
                                }
                                else
                                    item.Width = this.GridControlBase.Model.ColumnWidths[i]-2 ;
                            }
                            else                            
                            {
                                item.Width = this.GridControlBase.Model.ColumnWidths[i] - 2;
                            }

//#else
//                            item.Width = this.GridControlBase.Model.ColumnWidths[i] + 1;
//#endif

                            if (this.GridControlBase.Model.HeaderRows > 1)
                            {
                                item.Height = 31;
                            }
                            else
                            {
                                this.GridControlBase.Model.RowHeights[0] = 33;
                                item.Height = 31;
                            }

                            item.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                            this.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                            //item.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                            //this.GridControlBase.GridControl.GroupingBar.RowHeaderArea.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            //this.GridControlBase.GridControl.GroupingBar.RowHeaderArea.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                            //this.GridControlBase.GridControl.GroupingBar.RowHeaderArea.Height = 35;

                            //this.RowList.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                            //this.RowList.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                            //this.RowList.Height = 35;
                            //this.GridControlBase.GridControl.GroupingBar.RowHeaderArea.Margin = new Thickness(1, 0, 0, 0);
                        }
                    }
                }
                else
                {
#if !SILVERLIGHT
                    ResourceWrapperKeys rsWrapperKeys = new ResourceWrapperKeys();
                    Size size = Common.GetTextSize(rsWrapperKeys.pivotGridDropRowFields);
#else

                    Size size = Common.GetTextSize("Drop row fields here");
#endif
                    this.GridControlBase.Model.ColumnWidths[0] = size.Width + 15;
                    if (this.GridControl.AllowRowHeaderAreaAutoSizing)
                    {
                        if (size.Width < this.GridControl.PivotCalculations.Count * 40)
                        {
                            this.GridControlBase.ColumnWidths[0] = this.GridControl.PivotCalculations.Count * 40;
                        }
                    }

                    this.GridControl.GroupingBar.ComputationInfoWidth.Width = new GridLength(this.GridControlBase.ColumnWidths[0]);
                }
            }
        }
    }
}

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

#if !SILVERLIGHT
using Syncfusion.PivotAnalysis.Base;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Input;
using Syncfusion.Windows.Controls.PivotGrid.Resources;
using Syncfusion.Windows.Shared;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using Syncfusion.Silverlight.Controls.PivotGrid.Resources;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// Defines the model part of a cell type.
    /// </summary>
     public class PivotGridExpandCellModel : GridCellModel<PivotGridExpandCellRenderer>
    {
        /// <summary>
        ///  Calculates the preferred size of the cell based on its contents, including margins and any buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">Vertical or horizontal</param>
        /// <returns>The optimal size of the cell.</returns>
        public override Size CalculatePreferredCellSize(int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size clientSize = OnQueryPrefferedClientSize(rowIndex, colIndex, style, queryBounds);
            if (clientSize.IsEmpty)
                return Size.Empty;

            Thickness margins = style.TextMargins.ToThickness();
            // TextBoxView always seems to have this margin and I am not able to reset the margin.
            // Therefore I am also hard-coding it here so that TextBox behavior is properly
            // emulated.
            margins.Left = Math.Max(margins.Left, 2);
            margins.Right = Math.Max(margins.Right, 2);

            Size size = AddBorderMargins(clientSize, margins);
            size = AddBorderMargins(size, style.BorderMargins.ToThickness());
            size.Width += 20;
            return size;          
        }
    }
    /// <summary>
    /// Implements the renderer part of a PivotGrid expander cell.
    /// </summary>
    public class PivotGridExpandCellRenderer : GridVirtualizingCellRenderer<PivotExpanderCell>
    {

#if !SILVERLIGHT
        /// <summary>
        /// Creates the renderer element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void CreateRendererElement(PivotExpanderCell uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);
            PivotGridStyleInfoIdentity identity = style.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
            if (identity != null)
            {
                uiElement.StyleInfo = style.ModelStyle;
                uiElement.PivotCellInfo = identity.PivotCellInfo;
                //uiElement.IsExpanded = identity.IsExpanded;
                uiElement.IsHyperlinkCell = identity.IsHyperlinkCell;
                uiElement.EnableContextMenu = identity.EnableContextMenu;
                uiElement.ToolTipEnabled = identity.ToolTipEnabled;
                uiElement.CellIdentity = identity;
                uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
                uiElement.IsEnabledOnMouseOver = uiElement.GridControlBase.EnableHyperlinkOnMouseOver;
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
            }
        } 
#endif

        /// <summary>
        /// Called when [initialize content].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void OnInitializeContent(PivotExpanderCell uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            PivotGridStyleInfoIdentity identity = style.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
            if (identity != null)
            {
                uiElement.StyleInfo = style.ModelStyle;
                uiElement.PivotCellInfo = identity.PivotCellInfo;
                //uiElement.IsExpanded = identity.IsExpanded;
                uiElement.IsHyperlinkCell = identity.IsHyperlinkCell;
                uiElement.EnableContextMenu = identity.EnableContextMenu;
                uiElement.ToolTipEnabled   = identity.ToolTipEnabled;
                uiElement.CellIdentity = identity;
                uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
                uiElement.HorizontalAlignment = style.HorizontalAlignment;
                uiElement.VerticalAlignment = style.VerticalAlignment;
            }
        }

    }
    /// <summary>
    /// Class that holds the members of Expander Cell
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotExpanderCell : Control
    {

#if !SILVERLIGHT
        static PivotExpanderCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PivotExpanderCell), new FrameworkPropertyMetadata(typeof(PivotExpanderCell)));
           
        }
        /// <summary>
        /// Initializes the <see cref="PivotExpanderCell"/> class.
        /// </summary>
        public PivotExpanderCell()
        {
            CommandBindings.Add(new CommandBinding(PivotGridCommands.ExpandItem, this.ExpandItemExecuted, this.ExpandItemCanExecuted));
        }
#else
        public PivotExpanderCell()
        {
            DefaultStyleKey = typeof(PivotExpanderCell);
        }
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotExpanderCell.IsHyperlinkCell"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotExpanderCell.IsHyperlinkCell"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsHyperlinkCellProperty =
#if !SILVERLIGHT
           DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(PivotExpanderCell), new UIPropertyMetadata(false, null));
#else
           DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(PivotExpanderCell), new PropertyMetadata(false, null));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotExpanderCell.IsEnabledOnMouseOve"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotExpanderCell.IsEnabledOnMouseOver"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty IsEnabledOnMouseOverProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("IsEnabledOnMouseOver", typeof(bool), typeof(PivotExpanderCell), new UIPropertyMetadata(false));
#else
            DependencyProperty.Register("IsEnabledOnMouseOver", typeof(bool), typeof(PivotExpanderCell), new PropertyMetadata(false));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotExpanderCell.EnableContextMenu"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotExpanderCell.EnableContextMenu"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty EnableContextMenuProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("EnableContextMenu", typeof(bool), typeof(PivotExpanderCell), new UIPropertyMetadata(true, null));
#else
           DependencyProperty.Register("EnableContextMenu", typeof(bool), typeof(PivotExpanderCell), new PropertyMetadata(true, null));
#endif
        // Using a DependencyProperty as the backing store for ShowFieldChooser.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotExpanderCell.ToolTipEnabled"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotExpanderCell.ToolTipEnabled"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ToolTipEnabledProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(PivotExpanderCell), new UIPropertyMetadata(false, null));
#else
 DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(PivotExpanderCell), new PropertyMetadata(false, PivotExpanderCell.OnToolTipEnabledPropertyChanged));
#endif
        static void OnToolTipEnabledPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            PivotGridCellStyle gridCellStyle = dependencyObject as PivotGridCellStyle;
            if (gridCellStyle != null)
            {
                if (gridCellStyle.InternalSourceStyle != null)
                    gridCellStyle.InternalSourceStyle.ToolTipEnabled = gridCellStyle.ToolTipEnabled;
            }
        }
        /// <summary>
        /// Gets or sets the style information
        /// </summary>
        public GridStyleInfo StyleInfo
        {
            get;
            set;
        }
        /// <summary>
        /// Gets the state of the Expander whether it is expanded or collapsed
        /// </summary>
        public bool IsExpanded 
        {
            get
            {
                if (this.CellIdentity != null)
                    return this.CellIdentity.IsExpanded;
                
                return false;
            }
            set
            {
            }
        }
        /// <summary>
        /// Gets or sets the text in the cell
        /// </summary>
        public UIElement CellText { get; set; }

        /// <summary>
        /// Gets or sets whether the grid cells are hyperlink
        /// </summary>
        public bool IsHyperlinkCell
        {
            get{ return (bool)GetValue(IsHyperlinkCellProperty); }
            set{ SetValue(IsHyperlinkCellProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether to enable the hyperlink in row pivot cell only on mouse over.
        /// </summary>
        public bool IsEnabledOnMouseOver
        {
            get { return (bool)GetValue(IsEnabledOnMouseOverProperty); }
            set { SetValue(IsEnabledOnMouseOverProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the grid cells can have ContextMenu (Applies only for Expander Cells)
        /// </summary>
        public bool EnableContextMenu
        {
            get { return (bool)GetValue(EnableContextMenuProperty); }
            set { SetValue(EnableContextMenuProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether the grid cells can have ToolTip
        /// </summary>
        public bool ToolTipEnabled
        {
            get { return (bool)GetValue(ToolTipEnabledProperty); }
            set { SetValue(ToolTipEnabledProperty, value); }
        }
        /// <summary>
        /// Holds identity information such as row and column index for the current cell
        /// </summary>
        public PivotGridStyleInfoIdentity CellIdentity
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the text of the cell
        /// </summary>
        public string Text
        {
            get
            {
                if (this.PivotCellInfo != null)
                {
                    if (this.PivotCellInfo.Tag == null)
                        return this.PivotCellInfo.FormattedText;
                    else
                    {
                        GridStyleInfo styleInfo = this.PivotCellInfo.Tag as GridStyleInfo;
                        return ((PivotGridStyleInfoIdentity)this.GridControlBase.Model[styleInfo.RowIndex, styleInfo.ColumnIndex].CellIdentity).PivotCellInfo.FormattedText;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        /// <summary>
        /// Gets or sets the GridControlBase instance
        /// </summary>
        public PivotGridControlBase GridControlBase { get; set; }
        /// <summary>
        /// Gets or sets the Pivot cell informations
        /// </summary>
        public PivotCellInfo PivotCellInfo { get; set; }

#if !SILVERLIGHT
        /// <summary>
        /// Gets or sets the Expander
        /// </summary>
        public Expander InternalExpander { get; set; }
        ResourceWrapperKeys resourceWrapperKeys = new ResourceWrapperKeys();
        /// <summary>
        /// Calls before Mouse right button up event occurs
        /// </summary>
        /// <param name="e">An event argument</param>
        protected override void OnPreviewMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonUp(e);
            if (this.EnableContextMenu)
            {
                if (this.StyleInfo != null && this.PivotCellInfo != null && this.ContextMenu != null)
                {
                    this.ContextMenu.Items.Clear();
                    this.ContextMenu.Items.Add(GenerateContextMenuItems());
                    foreach (MenuItem mainObj in this.ContextMenu.Items)
                    {
                        SetItemProperties(mainObj);
                    }
                }
            }
        }

        internal MenuItem GenerateContextMenuItems()
        {
            string groupName = this.GridControlBase.GetNameAt(this.CellIdentity.ColumnIndex);
            MenuItem menuItem = new MenuItem() { Header = resourceWrapperKeys.PivotGridExpandCollapseHeader };
            MenuItem item = null;
            if (!string.IsNullOrEmpty(groupName))
            {
                item = new MenuItem()
                {
                    Header = resourceWrapperKeys.PivotGridExpandGroup + " \"" + groupName + "\"",
                    ToolTip = resourceWrapperKeys.PivotGridExpandGroup + " \"" + groupName + "\""
                };
                menuItem.Items.Add(item);

                item = new MenuItem()
                {
                    Header = resourceWrapperKeys.PivotGridCollapseGroup + " \"" + groupName + "\"",
                    ToolTip = resourceWrapperKeys.PivotGridExpandGroup + " \"" + groupName + "\""
                };
                menuItem.Items.Add(item);
            }
            
            item = new MenuItem()
            {
                Header = resourceWrapperKeys.PivotGridExpandAllHeader,
                ToolTip = resourceWrapperKeys.PivotGridExpandAllHeader
            };
            menuItem.Items.Add(item);
            item = new MenuItem()
            {
                Header = resourceWrapperKeys.PivotGridCollapseAllHeader,
                ToolTip = resourceWrapperKeys.PivotGridCollapseAllHeader
            };
            menuItem.Items.Add(item);

            item = new MenuItem()
            {
                Header = resourceWrapperKeys.PivotGridExpandHeader,
                ToolTip = resourceWrapperKeys.PivotGridExpandHeader
            };
            menuItem.Items.Add(item);

            item = new MenuItem()
            {
                Header = resourceWrapperKeys.PivotGridCollapseHeader,
                ToolTip = resourceWrapperKeys.PivotGridCollapseHeader
            };
            menuItem.Items.Add(item);
           
            return menuItem;
        }
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.InternalExpander = GetTemplateChild("PART_Expander") as Expander;
            this.ContextMenu = GetTemplateChild("PART_ExpanderContextMenu") as ContextMenu;
            SkinStorage.SetVisualStyle(this.ContextMenu, (Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this.GridControlBase.GridControl).ToString()));
            if (this.ContextMenu != null)
                this.ContextMenu.Visibility = this.EnableContextMenu ? Visibility.Visible : Visibility.Collapsed;
            if (this.CellIdentity.IsHyperlinkCell)
            {
                this.CellText = GetTemplateChild("PART_ValueTextBlock") as UIElement;

                if (this.CellText != null)
                {
                    CellText.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(CellText_PreviewMouseLeftButtonDown);
                }
            }

            if (this.InternalExpander != null)
            {
                this.InternalExpander.PreviewMouseLeftButtonDown += (sender, args) =>
                    {
                        if (this.StyleInfo != null && this.PivotCellInfo != null)
                        {
                            if (this.PivotCellInfo.Tag == null)
                            {
                                this.GridControlBase.GridControl.RaiseOnCollapsing(new CollapsingEventArgs(this.PivotCellInfo), this.StyleInfo);
                            }
                            else
                            {
                                PivotGridStyleInfoIdentity cellIdentity = ((GridStyleInfo)this.PivotCellInfo.Tag).Identity as PivotGridStyleInfoIdentity;
                                this.GridControlBase.CurrentCell.Deactivate();
                                if(cellIdentity != null)
                                this.GridControlBase.GridControl.RaiseOnExpanding(
                                    new ExpandingEventArgs(this.PivotCellInfo),
                                    this.GridControlBase.GridControl.PivotEngine[cellIdentity.RowIndex, cellIdentity.ColumnIndex]);
                            }
                        }
                    };
            }
        }

        /// <summary>
        /// Sets the command and color for the MenuItems.(recursive method)
        /// </summary>
        /// <param name="obj">MenuItem for which to set the properties.</param>
        internal void SetItemProperties(MenuItem obj)
        {
            obj.CommandBindings.Add(new CommandBinding(PivotGridCommands.ExpandItem, this.ExpandItemExecuted, this.ExpandItemCanExecuted));
            obj.Command = PivotGridCommands.ExpandItem;
            foreach (MenuItem subObj in obj.Items)
                SetItemProperties(subObj);
        }

        /// <summary>
        /// CanExecute for the Expand/Collapse operation for the given command
        /// </summary>
        private void ExpandItemCanExecuted(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Handles the PreviewMouseLeftButtonDown event of the CellText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void CellText_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.GridControlBase.GridControl.RaiseHyperlinkCellClick(new HyperlinkCellClickEventArgs(this.PivotCellInfo, this.StyleInfo.CellRowColumnIndex, e));
        }

#else
        public Style ExpanderStyle
        {
            get
            {
                if (this.GridControlBase != null)
                    return this.GridControlBase.GridControl.ExpanderStyle;
                return null;
            }
        }

        private ToggleButton m_InternalExpander;

        public ToggleButton InternalExpander 
        {
            get
            {
                return m_InternalExpander;
            }
            set
            {
                if (m_InternalExpander != value)
                {
                    m_InternalExpander = value;
                    m_InternalExpander.DataContext = this.PivotCellInfo;
                }
            }
        }

        public TextBlock CellValueTextBlock { get; set; }
        private ContextMenuAdv expanderContextMenu;
        ResourceWrapper resourceWrapper = new ResourceWrapper();
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.InternalExpander = GetTemplateChild("PART_Expander") as ToggleButton;
            this.CellValueTextBlock = GetTemplateChild("PART_CellValueTextBlock") as TextBlock;
            this.expanderContextMenu = GetTemplateChild("PART_ExpandCollapseContextMenu") as ContextMenuAdv;
            Syncfusion.Windows.Controls.Theming.SkinManager.SetVisualStyle(this.expanderContextMenu, Syncfusion.Windows.Controls.Theming.SkinManager.GetVisualStyle(this.GridControlBase.GridControl));
            this.expanderContextMenu.Visibility = this.EnableContextMenu ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            if (CellValueTextBlock != null)
            {
                if (this.GridControlBase !=null && this.GridControlBase.GridControl != null && this.GridControlBase.GridControl.ColumnHeaderCellStyle != null
                    && this.GridControlBase.GridControl.RowHeaderCellStyle != null)
                {
                    if ((this.PivotCellInfo.CellType.ToString().Contains(PivotCellType.RowHeaderCell.ToString()) &&
                        this.GridControlBase.GridControl.RowHeaderCellStyle.IsHyperlinkCell) ||
                        (this.PivotCellInfo.CellType.ToString().Contains(PivotCellType.ColumnHeaderCell.ToString()) &&
                        this.GridControlBase.GridControl.ColumnHeaderCellStyle.IsHyperlinkCell))
                    {
                        this.CellValueTextBlock.MouseEnter += new System.Windows.Input.MouseEventHandler(CellValueTextBlock_MouseEnter);
                        this.CellValueTextBlock.MouseLeave += new System.Windows.Input.MouseEventHandler(CellValueTextBlock_MouseLeave);
                        this.CellValueTextBlock.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(CellValueTextBlock_MouseLeftButtonDown);
                    }
                }

                if (this.EnableContextMenu)
                { this.CellValueTextBlock.MouseRightButtonDown += new MouseButtonEventHandler(_MouseRightButtonDown); }
                
            }
            this.InternalExpander.MouseEnter += new System.Windows.Input.MouseEventHandler(InternalExpander_MouseEnter);

            if (this.InternalExpander != null)
            {
                this.InternalExpander.Click += (sender, args) =>
                {
                    if (this.StyleInfo != null && this.PivotCellInfo != null)
                    {
                       if (this.StyleInfo != null && this.PivotCellInfo != null)
                        {
                            if (this.PivotCellInfo.Tag == null)
                            {
                                this.GridControlBase.GridControl.RaiseOnCollapsing(new CollapsingEventArgs(this.PivotCellInfo), this.StyleInfo);
                            }
                            else
                            {
                                PivotGridStyleInfoIdentity cellIdentity = ((GridStyleInfo)this.PivotCellInfo.Tag).Identity as PivotGridStyleInfoIdentity;
                                this.GridControlBase.CurrentCell.Deactivate();
                                this.GridControlBase.GridControl.RaiseOnExpanding(new ExpandingEventArgs(this.PivotCellInfo),
                                    this.GridControlBase.GridControl.PivotEngine[cellIdentity.RowIndex, cellIdentity.ColumnIndex]);
                            }

                            this.InternalExpander.IsChecked = this.CellIdentity.IsExpanded;

                        }
                    }
                };
                if (this.EnableContextMenu)
                { this.InternalExpander.MouseRightButtonDown += new MouseButtonEventHandler(_MouseRightButtonDown); }
            }
        }

        void _MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (this.StyleInfo != null && this.PivotCellInfo != null && this.expanderContextMenu != null)
            {
                this.expanderContextMenu.Items.Clear();

                ContextMenuItemAdv mainItem = new ContextMenuItemAdv() { Header = resourceWrapper.ExpandCollapse, Tag = "Expand/Collapse" };
                ContextMenuItemAdv item = new ContextMenuItemAdv() { Header = resourceWrapper.Expand+ " " + this.PivotCellInfo.FormattedText ,Tag="Expand"};
                ToolTipService.SetToolTip(item, resourceWrapper.Expand+ ": " + this.PivotCellInfo.UniqueText);
                mainItem.Items.Add(item);
                item = new ContextMenuItemAdv() { Header =resourceWrapper.Collapse +" " + this.PivotCellInfo.FormattedText, Tag = "Collapse" };
                ToolTipService.SetToolTip(item, resourceWrapper.Collapse+": " + this.PivotCellInfo.UniqueText);
                mainItem.Items.Add(item);
                item = new ContextMenuItemAdv() { Header = resourceWrapper.ExpandEntire, Tag = "Expand Entire Field" };
                ToolTipService.SetToolTip(item, item.Header);
                mainItem.Items.Add(item);
                item = new ContextMenuItemAdv() { Header = resourceWrapper.CollapseEntire, Tag = "Collapse Entire Field" };
                ToolTipService.SetToolTip(item, item.Header);
                mainItem.Items.Add(item);

                this.expanderContextMenu.Items.Add(mainItem);
                foreach (Syncfusion.Windows.Shared.ContextMenuItemAdv obj in this.expanderContextMenu.Items)
                {
                    SetItemProperties(obj);
                }
            }
        }

        /// <summary>
        /// Sets and register color and Click events respectively for the MenuItems.(recursive method)
        /// </summary>
        /// <param name="obj">ContextMenuItemAdv for which to set the properties.</param>
        private void SetItemProperties(Syncfusion.Windows.Shared.ContextMenuItemAdv obj)
        {
            obj.Click += new RoutedEventHandler(PivotExpanderCell_Click);
            foreach (ContextMenuItemAdv subObj in obj.Items)
                SetItemProperties(subObj);
        }
       
#endif

        /// <summary>
        /// Executes the Expand/Collapse operation for the given command
        /// </summary>
#if !SILVERLIGHT
        private void ExpandItemExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            this.GridControlBase.GridControl.Cursor = Cursors.Wait;
            MenuItem item = e.OriginalSource as MenuItem;
            string collapseGroup = this.PivotCellInfo.Key;
           

            if (item.Header.ToString().Contains(resourceWrapperKeys.PivotGridExpandAllHeader))
#else
        void PivotExpanderCell_Click(object sender, RoutedEventArgs e)
        {
            string collapseGroup = this.PivotCellInfo.Key;
            Syncfusion.Windows.Shared.ContextMenuItemAdv item = sender as Syncfusion.Windows.Shared.ContextMenuItemAdv;
            if (item.Tag.Equals("Expand/Collapse")){ return; }

            if (item.Tag.ToString().Contains("Expand Entire Field"))
#endif
            {
                if (this.GridControlBase.PivotEngine != null)
                {
                    if (this.PivotCellInfo.CellType.ToString().Contains("RowHeaderCell"))
                    {
                        for (int row = this.GridControlBase.PivotEngine.PivotColumns.Count + (!this.GridControlBase.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.GridControlBase.PivotEngine.RowCount; row++)
                        {
                            for (int column = 0; column < this.GridControlBase.PivotEngine.PivotRows.Count - 1; column++)
                            {
                               this.GridControlBase.ExpandGroup(row, column);
                            }
                        }
                    }
                    else if (this.PivotCellInfo.CellType.ToString().Contains("ColumnHeaderCell"))
                    {
                        for (int column = this.GridControlBase.PivotEngine.PivotRows.Count; column < this.GridControlBase.PivotEngine.ColumnCount; column++)
                        {
                            for (int row = 0; row < this.GridControlBase.PivotEngine.PivotColumns.Count - 1; row++)
                            {
                                this.GridControlBase.ExpandGroup(row, column);
                            }
                        }
                    }
                    if (this.GridControlBase._listOfCollapsedCells != null) this.GridControlBase._listOfCollapsedCells.Clear();
                    this.GridControlBase.InvalidateCells();
                }
            }
#if !SILVERLIGHT
            else if (item.Header.ToString().Contains(resourceWrapperKeys.PivotGridCollapseAllHeader))
#else
            else if (item.Tag.ToString().Contains("Collapse Entire Field"))
#endif
            {
                if (this.PivotCellInfo.CellType.ToString().Contains("RowHeaderCell"))
                {
                    this.GridControlBase.CollapseGroupRow();
                }
                else if (this.PivotCellInfo.CellType.ToString().Contains("ColumnHeaderCell"))
                {
                    this.GridControlBase.CollapseGroupColumn();
                }
                this.GridControlBase.InvalidateCells();
            }
#if !SILVERLIGHT
            else if (item.Header.ToString().Contains(resourceWrapperKeys.PivotGridExpandGroup))
#else
            else if (item.Tag.ToString().Contains("Expand"))
#endif
            {
                if (this.PivotCellInfo.CellType.ToString().Contains("RowHeaderCell"))
                {
                    for (int row = this.GridControlBase.PivotEngine.PivotColumns.Count + (!this.GridControlBase.PivotEngine.ShowCalculationsAsColumns ? 1 : 0); row < this.GridControlBase.PivotEngine.RowCount; row++)
                    {
                        ExpandGroup(row, this.CellIdentity.ColumnIndex,this.PivotCellInfo.Key);
                    }
                }
                else if (this.PivotCellInfo.CellType.ToString().Contains("ColumnHeaderCell"))
                {
                    for (int column = this.GridControlBase.PivotEngine.PivotRows.Count; column < this.GridControlBase.PivotEngine.ColumnCount; column++)
                    {
                        ExpandGroup(this.CellIdentity.RowIndex, column,this.PivotCellInfo.Key);
                    }
                }
                this.GridControlBase.InvalidateCells();
            }
#if !SILVERLIGHT

            else if (item.Header.ToString().Contains(resourceWrapperKeys.PivotGridCollapseGroup))
#else
            else if (item.Tag.ToString().Contains("Collapse"))
#endif
            {
                if (this.PivotCellInfo.CellType.ToString().Contains("RowHeaderCell"))
                {
                    this.GridControlBase.CollapseGroupRow(this.CellIdentity.ColumnIndex, collapseGroup );
                }
                else if (this.PivotCellInfo.CellType.ToString().Contains("ColumnHeaderCell"))
                {
                    this.GridControlBase.CollapseGroupColumn(this.CellIdentity.RowIndex,collapseGroup);
                }
                this.GridControlBase.InvalidateCells();
            }
#if !SILVERLIGHT
            else if (item.Header.ToString().Equals(resourceWrapperKeys.PivotGridCollapseHeader))
#else
            else if (item.Tag.ToString().Contains("Collapse"))
#endif
            {
                if (this.PivotCellInfo.Tag == null)
                    this.GridControlBase.GridControl.RaiseOnCollapsing(new CollapsingEventArgs(this.PivotCellInfo), this.StyleInfo);
            }
#if !SILVERLIGHT
            else if (item.Header.ToString().Equals(resourceWrapperKeys.PivotGridExpandHeader))
#else
            else if (item.Tag.ToString().Contains("Expand"))
#endif
            {
                if (this.PivotCellInfo.Tag != null)
                {
                    PivotGridStyleInfoIdentity cellIdentity = ((GridStyleInfo)this.PivotCellInfo.Tag).Identity as PivotGridStyleInfoIdentity;
                    this.GridControlBase.CurrentCell.Deactivate();
                    this.GridControlBase.GridControl.RaiseOnExpanding(new ExpandingEventArgs(this.PivotCellInfo), this.GridControlBase.GridControl.PivotEngine[cellIdentity.RowIndex, cellIdentity.ColumnIndex]);
                }
            }
            this.GridControlBase.GridControl.Cursor = Cursors.Arrow;
        }

        ///// <summary>
        ///// Expand the group for the provided row and column
        ///// </summary>
        /// <param name="row">Row index of the expander cell.</param>
        /// <param name="column">Column index of the expander cell.</param>
        /// <param name="CellKey">key of the expander cell</param>
        private void ExpandGroup(int row, int column, string CellKey)
        {
            PivotCellInfo cellInfo = this.GridControlBase.PivotEngine[row, column];
            if (cellInfo != null && cellInfo.Key == CellKey)
            {
                if (cellInfo.Tag != null)
                {
                    this.GridControlBase.ExpandGroup(cellInfo, cellInfo.Tag as GridStyleInfo);
                }
            }
        }

        private void ExpandGroup(int row, int column)
        {
            PivotCellInfo cellInfo = this.GridControlBase.PivotEngine[row, column];
            if (cellInfo != null)
            {
                if (cellInfo.Tag != null)
                {
                    this.GridControlBase.ExpandGroup(cellInfo, cellInfo.Tag as GridStyleInfo);
                }
            }
        }

#if SILVERLIGHT        
        /// <summary>
        /// Handles the MouseLeftButtonDown event of the CellValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void CellValueTextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var cellrowcol = this.GridControlBase.PointToCellRowColumnIndex(e.GetPosition(this.GridControlBase));
            this.GridControlBase.GridControl.RaiseHyperlinkCellClick(new HyperlinkCellClickEventArgs(this.PivotCellInfo, cellrowcol, e ));
        }

        /// <summary>
        /// Handles the MouseLeave event of the CellValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void CellValueTextBlock_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", false);         
        }

        /// <summary>
        /// Handles the MouseEnter event of the CellValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void CellValueTextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "MouseOver", false);
        }


        /// <summary>
        /// Handles the MouseEnter event of the InternalExpander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void InternalExpander_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            VisualStateManager.GoToState(this.InternalExpander, "MouseOver", false);
        }
#endif

    }

#if SILVERLIGHT
    public class ExpanderPathConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            PivotCellInfo cellInfo = ((ToggleButton)value).Tag as PivotCellInfo;
            if (cellInfo != null)
            {
                if (cellInfo.Tag == null)
                {
                    return "M0,2L0,3 5,3 5,2z";

                }
                else
                    return "M0,2L0,3 2,3 2,5 3,5 3,3 5,3 5,2 3,2 3,0 2,0 2,2z";
            }

            return null;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
#endif
}

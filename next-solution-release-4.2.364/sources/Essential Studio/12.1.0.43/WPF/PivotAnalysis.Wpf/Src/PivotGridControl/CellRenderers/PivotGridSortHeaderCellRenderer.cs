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
using System.Windows.Data;
using System.ComponentModel;
using System.Windows.Shapes;
using Syncfusion.Windows.Shared;
using System.Collections;
using System.Globalization;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Shared;
using Syncfusion.Silverlight.Controls.PivotGrid.Resources;
using System.ComponentModel;
using System.Windows.Shapes;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// A cell model for the <see cref="PivotGridHyperlinkCellRenderer"/>.
    /// </summary>
    public class PivotGridSortHeaderCellModel : GridCellModel<PivotGridSortCellRenderer>
    {
        /// <summary>
        /// Preferred size for the cells is measuring
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        /// <param name="queryBounds">GridQueryBounds</param>
        /// <returns>Size</returns>

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
            size.Width += 30; //30 is from the sort & filter glyphs
            return size;
        }
    }
    /// <summary>
    /// Implements the renderer part of a PivotGrid sortable header cell.
    /// </summary>
    public class PivotGridSortCellRenderer : GridVirtualizingCellRenderer<PivotSortHeaderCell>
    {


#if !SILVERLIGHT
        /// <summary>
        /// Creates the renderer element.
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void CreateRendererElement(PivotSortHeaderCell uiElement, GridRenderStyleInfo style)
        {
            base.CreateRendererElement(uiElement, style);
            PivotGridStyleInfoIdentity identity = style.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
            if (identity != null)
            {
                uiElement.StyleInfo = style.ModelStyle;
                uiElement.PivotCellInfo = identity.PivotCellInfo;
                //uiElement.IsExpanded = identity.IsExpanded;
                uiElement.IsHyperlinkCell = identity.IsHyperlinkCell;
                uiElement.ToolTipEnabled = identity.ToolTipEnabled;
                uiElement.CellIdentity = identity;
                uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
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
            }
        }
#endif

        private Visibility _sortingIndicator = Visibility.Collapsed;
        /// <summary>
        /// Gets or sets sorting indicator
        /// </summary>
        public Visibility SortingIndicator
        {
            get
            {
                return _sortingIndicator;
            }
            set
            {
                _sortingIndicator = value;
            }
        }

        /// <summary>
        /// Called when [initialize content].
        /// </summary>
        /// <param name="uiElement">The UI element.</param>
        /// <param name="style">The style.</param>
        public override void OnInitializeContent(PivotSortHeaderCell uiElement, GridRenderStyleInfo style)
        {
            base.OnInitializeContent(uiElement, style);
            PivotGridStyleInfoIdentity identity = style.ModelStyle.CellIdentity as PivotGridStyleInfoIdentity;
            if (identity != null)
            {
                uiElement.StyleInfo = style.ModelStyle;
                uiElement.PivotCellInfo = identity.PivotCellInfo;
                uiElement.IsHyperlinkCell = identity.IsHyperlinkCell;
                uiElement.ToolTipEnabled = identity.ToolTipEnabled;
                uiElement.CellIdentity = identity;
                uiElement.GridControlBase = this.GridControl as PivotGridControlBase;
                uiElement.Background = style.ModelStyle.Background;
                uiElement.Foreground = style.ModelStyle.Foreground;
                uiElement.FontFamily = style.ModelStyle.Font.FontFamily;
                uiElement.FontSize = style.ModelStyle.Font.FontSize;
                uiElement.FontWeight = style.ModelStyle.Font.FontWeight;
                uiElement.HorizontalAlignment = style.HorizontalAlignment;
                uiElement.VerticalAlignment = style.VerticalAlignment;
                uiElement.SortingIndicator = this.SortingIndicator;

            }
        }
#if !SILVERLIGHT
        /// <summary>
        /// Calls when the cells are rendering
        /// </summary>
        /// <param name="dc">DrawingContext</param>
        /// <param name="rca">RenderCellArgs</param>
        /// <param name="cellInfo">GridRenderStyleInfo</param>
        protected override void OnRender(DrawingContext dc, Cells.RenderCellArgs rca, GridRenderStyleInfo cellInfo)
        {
            base.OnRender(dc, rca, cellInfo);
            PivotGridControlBase grid = GridControl as PivotGridControlBase;

            if (grid != null && grid.sortHeaderList != null && grid.sortHeaderList.Count > 1)
            {
                int loc = grid.sortHeaderList.IndexOf(grid.PivotEngine.ResolveColumnIndex(cellInfo.ColumnIndex)) + 1;
                if (loc > 0)
                {
                    Rect r = rca.CellRect;
                    r.X = r.Right - 5;
                    FormattedText t = new FormattedText(loc.ToString(), cellInfo.GetCulture(true), cellInfo.FlowDirection, cellInfo.Typeface, 10, cellInfo.Foreground);
                    dc.DrawText(t, r.Location);
                }
            }
         }
#endif
    }
    /// <summary>
    /// Class that holds the members of Sort headerCell
    /// </summary>
#if SyncfusionFramework4_0 
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PivotSortHeaderCell : Control
    {
        /// <summary>
        /// Initializes the <see cref="PivotSortHeaderCell"/> class.
        /// </summary>
        public PivotSortHeaderCell()
        {
            DefaultStyleKey = typeof(PivotSortHeaderCell);
#if !SILVERLIGHT

            CommandBindings.Add(new CommandBinding(PivotGridCommands.ShowCalculationFilter, ShowFilterExecuted, ShowFilterCanExecute));
#endif
        }
#if !SILVERLIGHT
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.FilterListBoxItems"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.FilterListBoxItems"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty FilterListBoxItemsProperty = DependencyProperty.Register("FilterListBoxItems", typeof(List<FilterChoice>), typeof(PivotSortHeaderCell), new UIPropertyMetadata(null, null));
        /// <summary>
        /// Gets or sets the FilterListBox items
        /// </summary>
        public List<FilterChoice> FilterListBoxItems
        {
            get
            {
                return (List<FilterChoice>)this.GetValue(FilterListBoxItemsProperty);
            }
            set
            {
                this.SetValue(FilterListBoxItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets the context menu on sort header cell.
        /// </summary>
        /// <returns>context menu</returns>
        public ContextMenu GetContextMenu()
        {
            return this.GetTemplateChild("Part_SortHeaderContext") as ContextMenu;
        }
#endif
        private CheckBox allCheckBox;
        /// <summary>
        /// Gets or sets the checkBox instance of "All" in Filter Popup
        /// </summary>
        public CheckBox AllCheckBox
        {
            get { return allCheckBox; }
            set { allCheckBox = value; }
        }
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Path sortpath = GetTemplateChild("colSortPath") as Path;
#if !SILVERLIGHT
            this.ColumnFilterPopup = GetTemplateChild("PART_ColumnFilterPopup") as ColumnFilterPopup;
            this.AllCheckBox = GetTemplateChild("PART_AllCheckBox") as CheckBox;
            this.ContextMenu = GetTemplateChild("Part_SortHeaderContext") as ContextMenu;
            SkinStorage.SetVisualStyle(this.ContextMenu, (Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this.GridControlBase.GridControl).ToString()));
         
            if (this.AllCheckBox != null)
            {
                this.AllCheckBox.Click += AllCheckBox_Click;
            }
            if (this.ColumnFilterPopup != null)
            {
                this.ColumnFilterPopup.OkButton = GetTemplateChild("PART_btnOK") as Button;
                this.ColumnFilterPopup.CancelButton = GetTemplateChild("PART_btnCancel") as Button;
                this.ColumnFilterPopup.GridControl = this.GridControlBase.GridControl;
                this.ColumnFilterPopup.RightCornerThumb = GetTemplateChild("PART_thumbRightCorner") as Thumb;
                this.ColumnFilterPopup.PopupBorder = GetTemplateChild("PART_FilterPopupBorder") as Border;
            }
#endif
            Button filterBtn = GetTemplateChild("PART_btnFilter") as Button;

#if SILVERLIGHT
            if (this.GridControlBase.GridControl.VisualStyle == Windows.Controls.Theming.VisualStyle.Office2010Black || this.GridControlBase.GridControl.VisualStyle == Windows.Controls.Theming.VisualStyle.Blend)
#else
            if (this.GridControlBase != null && (this.GridControlBase.GridControl.VisualStyle == PivotGridVisualStyle.Office2007Black || this.GridControlBase.GridControl.VisualStyle == PivotGridVisualStyle.Blend))
#endif
                if (sortpath != null) sortpath.Fill = new SolidColorBrush(Colors.White);

            GridStyleInfo styleInfo = PivotCellInfo != null && this.PivotCellInfo.Tag is GridStyleInfo
                 ? this.PivotCellInfo.Tag as GridStyleInfo : null;
#if !SILVERLIGHT

            if (filterBtn != null)
            {
                if (this.GridControlBase != null)
                {
                    if ((styleInfo != null && this.GridControlBase.PivotEngine.CanFilterColumn(styleInfo.ColumnIndex)))
                    {
                        filterBtn.Visibility = Visibility.Visible;
                    }
                    else if (styleInfo == null)
                    {
                        //row pivot
                        if (PivotCellInfo != null)
                        {
                            string formatted = PivotCellInfo.FormattedText;
                            PivotItem pi = this.GridControlBase.PivotEngine.PivotRows.Select(m => m).FirstOrDefault(m => m.FieldHeader == formatted);
                            if (pi != null && pi.AllowFilter)
                                filterBtn.Visibility = Visibility.Visible;
                            else
                                filterBtn.Visibility = Visibility.Collapsed;
                        }
                        else
                            filterBtn.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    filterBtn.Visibility = Visibility.Collapsed;
                }
            }
#endif
        }
#if !SILVERLIGHT

        void AllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (this.AllCheckBox.IsChecked.HasValue)
            {
                if (this.AllCheckBox.IsChecked.Value)
                {
                    this.FilterListBoxItems.ForEach(i => i.IsChecked = true);
                    ColumnFilterPopup.OkButton.IsEnabled = true;
                    ColumnFilterPopup.IsFilterChanged = true;
                }
                else
                {
                    this.FilterListBoxItems.ForEach(i => i.IsChecked = false);
                    ColumnFilterPopup.OkButton.IsEnabled = false;
                }
            }
        }
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.IsHyperlinkCell"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.IsHyperlinkCell"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty HyperlinkCellProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(PivotSortHeaderCell), new UIPropertyMetadata(false, null));
#else
 DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(PivotSortHeaderCell), new PropertyMetadata(false, null));
#endif

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.ToolTipEnabled"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.ToolTipEnabled"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ToolTipEnabledProperty =
#if !SILVERLIGHT
 DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(PivotSortHeaderCell), new UIPropertyMetadata(false, null));
#else
 DependencyProperty.Register("ToolTipEnabled", typeof(bool), typeof(PivotSortHeaderCell), new PropertyMetadata(false, PivotSortHeaderCell.OnToolTipEnabledPropertyChanged));
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
        /// Gets or sets the Style related information
        /// </summary>
        public GridStyleInfo StyleInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cell text
        /// </summary>

        public UIElement CellText { get; set; }

        /// <summary>
        /// Gets or sets whether the grid cells are hyperlink
        /// </summary>
        public bool IsHyperlinkCell
        {
            get { return (bool)GetValue(HyperlinkCellProperty); }
            set { SetValue(HyperlinkCellProperty, value); }
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
        /// gets or sets the cell identity
        /// </summary>
        public PivotGridStyleInfoIdentity CellIdentity
        {
            get;
            set;
        }
        /// <summary>
        /// gets or sets the text
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
#if !SILVERLIGHT

        internal ColumnFilterPopup ColumnFilterPopup
        {
            get { return (ColumnFilterPopup)GetValue(ColumnFilterPopupProperty); }
            set { SetValue(ColumnFilterPopupProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterPopup.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.ColumnFilterPopup"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.ColumnFilterPopup"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty ColumnFilterPopupProperty =
            DependencyProperty.Register("ColumnFilterPopup", typeof(ColumnFilterPopup), typeof(PivotSortHeaderCell), null);

        /// <summary>
        /// Identifies the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.SortingIndicator"/> dependency property.
        /// </summary>
        /// 
        /// <returns>
        /// The identifier for the <see cref="P:Syncfusion.Windows.Controls.PivotGrid.PivotSortHeaderCell.SortingIndicator"/> dependency property.
        /// </returns>
        public static readonly DependencyProperty SortingIndicatorProperty =

 DependencyProperty.Register("SortingIndicator", typeof(Visibility), typeof(PivotSortHeaderCell), new UIPropertyMetadata(Visibility.Collapsed, null));
        /// <summary>
        /// Gets or sets the sorting order indication
        /// </summary>
        public Visibility SortingIndicator
        {
            get { return (Visibility)GetValue(SortingIndicatorProperty); }
            set { SetValue(SortingIndicatorProperty, value); }
        }

#else
        public static readonly DependencyProperty SortingIndicatorProperty =

DependencyProperty.Register("SortingIndicator", typeof(Visibility), typeof(PivotSortHeaderCell), new PropertyMetadata(Visibility.Collapsed, null));

        public Visibility SortingIndicator
        {
            get { return (Visibility)GetValue(SortingIndicatorProperty); }
            set { SetValue(SortingIndicatorProperty, value); }
        }
       
        
#endif
        /// <summary>
        /// gets or sets the gridcontrolbase
        /// </summary>
        public PivotGridControlBase GridControlBase { get; set; }
        /// <summary>
        /// gets or sets the pivotcell information
        /// </summary>
        public PivotCellInfo PivotCellInfo { get; set; }
#if !SILVERLIGHT

        internal void ShowFilterCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            Button filterbtn = e.OriginalSource as Button;
            PivotSortHeaderCell header = sender as PivotSortHeaderCell;
            if (filterbtn != null)
            {
                if (header != null && header.StyleInfo != null
                    && this.GridControlBase.PivotEngine.CanFilterColumn(header.StyleInfo.ColumnIndex))
                {
                    if (!(this.GridControlBase.PivotEngine.PivotRows.Count == 0 && header.StyleInfo.ColumnIndex == 0))
                    {
                        filterbtn.Visibility = Visibility.Visible;
                        e.CanExecute = true;
                        e.Handled = true;
                    }
                }
                else if (header.StyleInfo == null)
                {
                    //row pivot
                    if (PivotCellInfo != null)
                    {
                        string formatted = PivotCellInfo.FormattedText;
                        PivotItem pi = this.GridControlBase.PivotEngine.PivotRows.Select(m => m).FirstOrDefault(m => m.FieldHeader == formatted);
                        if (pi != null && pi.AllowFilter && !(this.GridControlBase.PivotEngine.PivotRows.Count == 0))
                        {
                            filterbtn.Visibility = Visibility.Visible;
                            e.CanExecute = true;
                            e.Handled = true;
                        }
                        else
                            filterbtn.Visibility = Visibility.Collapsed;
                    }
                    else
                        filterbtn.Visibility = Visibility.Collapsed;
                }
                else
                {
                    filterbtn.Visibility = Visibility.Collapsed;
                }
            }
        }

        private static string[] formats = new string[]
                  {
                          "MM/dd/yyyy HH:mm:ss tt",
                          "MM/dd/yyyy HH:mm:ss",
                          "M/dd/yyyy H:mm:ss tt",
                          "M/dd/yyyy H:mm:ss"        
                  };
        private static DateTime ParseDate(string input)
        {
            return DateTime.ParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        List<FilterChoice> hidden = null;
        
        internal void ShowFilterExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Button button = e.OriginalSource as Button;
            this.ColumnFilterPopup.CancelCollectionList = new List<FilterChoice>();
            if (button != null && this.GridControlBase != null)
            {
                if (this.ColumnFilterPopup.PopupBorder != null)
                {
                    switch (this.GridControlBase.GridControl.VisualStyle)
                    {
                        case PivotGridVisualStyle.Blend:
                            break;
                        case PivotGridVisualStyle.Office2003:
                            break;
                        case PivotGridVisualStyle.Office2007Black:
                        case PivotGridVisualStyle.Office2007Blue:
                        case PivotGridVisualStyle.Office2007Silver:
                            break;
                        case PivotGridVisualStyle.Office2010Blue:
                        case PivotGridVisualStyle.Office2010Silver:
                        case PivotGridVisualStyle.Office2010Black:
                            break;
                        case PivotGridVisualStyle.Transparent:
                            this.ColumnFilterPopup.PopupBorder.Background = Brushes.Transparent;
                            break;
                        case PivotGridVisualStyle.Metro:
                        case PivotGridVisualStyle.Default:
                            this.ColumnFilterPopup.PopupBorder.Background = Brushes.White;
                            break;
                    }
                }
                this.ColumnFilterPopup.CanExecute = false;
                int colIndex = this.GridControlBase.CurrentCell.ColumnIndex;

                string colName = this.GridControlBase.PivotEngine.GetFieldNameAtIndex(colIndex);
                ColumnFilterPopup.ColIndex = colIndex;
                PopulateFilterPopUp(colIndex, colName);

                this.ColumnFilterPopup.PlacementTarget = button;
                this.ColumnFilterPopup.IsOpen = true;
                this.ColumnFilterPopup.Closed += new EventHandler(ColumnFilterPopup_Closed);
                this.ColumnFilterPopup.FilterListBoxItems = this.FilterListBoxItems;
                this.ColumnFilterPopup.IsFilterChanged = false;
            }
            
        }

        void ColumnFilterPopup_Closed(object sender, EventArgs e)
        {
            foreach (FilterChoice item in FilterListBoxItems)
            {
                if (item.Text != null)
                {
                    item.PropertyChanged -= new PropertyChangedEventHandler(FilterElement_PropertyChanged);
                }
            }
            
            if (this.ColumnFilterPopup.CanExecute)
            {
                if (ColumnFilterPopup.FilterPopUpCollection.Count == 0 && ColumnFilterPopup.FilteredColumnlist != null && ColumnFilterPopup.FilteredColumnlist.Count > 0)
                {
                    ColumnFilterPopup.FilteredColumnlist.Clear();
                    ColumnFilterPopup.Exclusions.Clear();
                }
                HashSet<string> tempUnCheck = new HashSet<string>();
                HashSet<string> tempReCheck = new HashSet<string>();
                HashSet<string> tempNotVisible = new HashSet<string>();
                 
                foreach (FilterChoice item in FilterListBoxItems)
                {
                    if (item.IsChecked == false)
                    {
                        tempUnCheck.Add(item.Text);
                    }
                    else if (item.IsChecked == true)
                    {
                        tempReCheck.Add(item.Text);
                    }
                     
                }

                int c = GridControlBase.PivotEngine.ResolveColumnIndex(this.ColumnFilterPopup.ColIndex);
                for (int i = 1; i < GridControlBase.PivotEngine.RowCount - GridControlBase.PivotEngine.PivotRows.Count - 1; ++i)
                {
                    PivotCellInfo pci = this.GridControlBase.GridControl.PivotEngine.PivotValues[i, c];
                    if (pci != null)
                    {
                        bool isValue = pci.CellType == PivotCellType.ValueCell;
                        bool isRowPivot = pci.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell);
                        if (isValue || isRowPivot)
                        {
                            if (!tempReCheck.Contains(pci.FormattedText))
                            {
                                if (pci.FormattedText == null)
                                {
                                    tempNotVisible.Add(pci.UniqueText);
                                }
                                else
                                {
                                    tempNotVisible.Add(pci.FormattedText);
                                }
                            }
                        }
                    }
                }

                string colName = this.GridControlBase.GridControl.PivotEngine.GetFieldNameAtIndex(this.ColumnFilterPopup.ColIndex);

                if (tempNotVisible.Count == 0 || AllCheckBox.IsChecked == true)
                {
                    if (ColumnFilterPopup.Exclusions.ContainsKey(colName))
                    {
                        ColumnFilterPopup.Exclusions.Remove(colName);
                    }
                }
                else
                {
                    if (ColumnFilterPopup.Exclusions.ContainsKey(colName))
                    {
                        ColumnFilterPopup.Exclusions[colName] = tempNotVisible;
                    }
                    else
                    {
                        ColumnFilterPopup.Exclusions.Add(colName, tempNotVisible);
                    }
                }
                if (ColumnFilterPopup.FilterPopUpCollection.Any(x => x.Key == colName))
                {
                    ColumnFilterPopup.FilterPopUpCollection.Remove(colName);
                }

                ColumnFilterPopup.FilterPopUpCollection.Add(colName, FilterListBoxItems);

                //temporarily store the current column indexes to avoid repeatedly looking them.
                int[] tempColIndexes = new int[ColumnFilterPopup.Exclusions.Count];
                int k2 = 0;
                foreach (string col in ColumnFilterPopup.Exclusions.Keys)
                {
                    tempColIndexes[k2++] = this.GridControlBase.GridControl.InternalGrid.GetColumnIndexFromName(col);
                }

                int startRow = (this.GridControlBase.GridControl.PivotEngine.PivotColumns.Count != 0 ? this.GridControlBase.GridControl.PivotEngine.PivotColumns.Count : 0) + (this.GridControlBase.GridControl.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);
                int rowCount = this.GridControlBase.GridControl.PivotEngine.ShowGrandTotals ? this.GridControlBase.GridControl.PivotEngine.RowCount - 1 : this.GridControlBase.GridControl.PivotEngine.RowCount;
                int colIndexKeyLoc = this.GridControlBase.GridControl.PivotEngine.GetHiddenRowKeyValueColumnIndex();
                 for (int i = startRow; i < rowCount; i++)
                {
                    PivotCellInfo pci = this.GridControlBase.GridControl.PivotEngine[i, this.ColumnFilterPopup.ColIndex];
                    PivotCellInfo pciKey = this.GridControlBase.GridControl.PivotEngine[i, colIndexKeyLoc];
                    bool isValue = pci.CellType == PivotCellType.ValueCell;
                    bool isRowPivot = pci.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell);
                    if (pci != null && ((isValue && tempUnCheck.Contains(pci.FormattedText))
                        || (isRowPivot && tempUnCheck.Contains(pci.FormattedText))))
                    {
                            this.GridControlBase.GridControl.PivotEngine.HiddenRowIndexes.Add(pciKey);
                    }
                    else if (pci != null && tempReCheck.Contains(pci.FormattedText) && (isValue || isRowPivot))
                    {
                        bool needToShow = true;

                        int k1 = 0;
                        foreach (string col in ColumnFilterPopup.Exclusions.Keys)
                        {
                            string s = this.GridControlBase.GridControl.InternalGrid.PivotEngine[i, tempColIndexes[k1]].FormattedText;
                            if (ColumnFilterPopup.Exclusions[col].Contains(s))
                            {
                                needToShow = false;
                                break;
                            }
                            k1++;
                        }
                        if (needToShow)
                        {
                            this.GridControlBase.GridControl.PivotEngine.HiddenRowIndexes.Remove(pciKey);
                        }
                    }
                }

                if (ColumnFilterPopup.FilteredColumnlist.Any(x => x == colName))
                    ColumnFilterPopup.FilteredColumnlist.Remove(colName);
                ColumnFilterPopup.FilteredColumnlist.Add(colName);
                this.GridControlBase.ApplyFilters(true);

                this.GridControlBase.GridControl.RaiseFilterActionCompleted(new FilterActionCompletedEventArgs()
                {
                    FieldName = colName,
                    Choices = ColumnFilterPopup.FilterPopUpCollection[colName]
                });
                // Remove the item from FilterPopupCollection
                if (!(ColumnFilterPopup.FilterPopUpCollection[colName].Any(x => x.IsChecked == false)) && !(ColumnFilterPopup.FilterPopUpCollection[colName].Any(x => x.IsChecked == null)))
                {
                    if (ColumnFilterPopup.FilterPopUpCollection.Count > 0 && ColumnFilterPopup.FilteredColumnlist.Count > 0)
                    {
                        ColumnFilterPopup.FilterPopUpCollection.Remove(colName);
                        ColumnFilterPopup.FilteredColumnlist.Remove(colName);
                    }
                }
            }
            this.ColumnFilterPopup.Closed -= new EventHandler(ColumnFilterPopup_Closed);
            if (hidden != null && hidden.Count > 0)
            {
                foreach (FilterChoice fc in hidden)
                    fc.IsChecked = false;
                hidden.Clear();
            }
        }
#endif
        internal class FilterItem
        {
            /// <summary>
            /// Gets or sets text of filter item
            /// </summary>
            public string Text { get; set; }
            /// <summary>
            /// Gets or sets the SortKey
            /// </summary>
            public IComparable SortKey { get; set; }
            /// <summary>
            /// Gets or sets the status whether the filter item is checked or unchecked
            /// </summary>
            public bool? IsChecked { get; set; } 
            /// <summary>
            /// method used to compare the text
            /// </summary>
            /// <param name="obj">Object</param>
            /// <returns>bool</returns>
            public override bool Equals(object obj)
            {
                if (obj == null && Text == null)
                    return true;
                if (Text == null)
                    return false;
                if (obj == null)
                    return false;
                return this.Text.Equals(obj.ToString());
            }
            public override int GetHashCode()
            {
                if (Text == null)
                    return base.GetHashCode();
                else
                    return Text.GetHashCode();
            }
            public override string ToString()
            {
                if (Text == null)
                    return "";
                return Text;
            }
        }

        internal class FilterItemSorter : IComparer<FilterItem>
        {
            public int Compare(FilterItem x, FilterItem y)
            {
                if (x.SortKey == null && y.SortKey == null)
                    return 0;
                else if (x.SortKey == null && y.SortKey != null)
                    return -1;
                else if (y.SortKey == null)
                    return 1;
                else
                {
                    if (x.SortKey.GetType() == y.SortKey.GetType())
                    {
                        return x.SortKey.CompareTo(y.SortKey);
                    }
                    else
                    {
                        return x.SortKey.ToString().CompareTo(y.SortKey.ToString());
                    }
                }
            }
        }

        HashSet<string> tempExclude = new HashSet<string>();
#if !SILVERLIGHT

        void PopulateFilterPopUp(int colIndex, string colName)
        {
            HashSet<FilterItem> temp = new HashSet<FilterItem>();
            if (ColumnFilterPopup.FilterPopUpCollection.Count == 0 && ColumnFilterPopup.FilteredColumnlist != null && ColumnFilterPopup.FilteredColumnlist.Count > 0)
            {
                ColumnFilterPopup.FilteredColumnlist.Clear();
                ColumnFilterPopup.Exclusions.Clear();
            }

            //save the colIndexes
            Dictionary<string, int> colIndexes = new Dictionary<string, int>();
            foreach (string col in ColumnFilterPopup.Exclusions.Keys)
            {
                colIndexes.Add(col, this.GridControlBase.PivotEngine.ResolveColumnIndex(this.GridControlBase.GetColumnIndexFromName(col)));
            }

            int startRow = (this.GridControlBase.PivotEngine.PivotColumns.Count != 0 ? this.GridControlBase.PivotEngine.PivotColumns.Count : 0) + (this.GridControlBase.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);
            int rowCount = this.GridControlBase.PivotEngine.ShowGrandTotals ? this.GridControlBase.PivotEngine.RowCount - 1 : this.GridControlBase.PivotEngine.RowCount;
           
            int colLookupLoc = this.GridControlBase.PivotEngine.ResolveColumnIndex(colIndex);
            int loc = colLookupLoc - this.GridControlBase.PivotEngine.PivotRows.Count;
            bool isRowPivot = loc < 0 || (loc >= 0  && this.GridControlBase.PivotEngine.PivotCalculations[loc].SummaryType == SummaryType.DisplayIfDiscreteValuesEqual);
            for (int i = startRow; i < rowCount; i++)
            {
                PivotCellInfo pci = this.GridControlBase.PivotEngine.PivotValues[i, colLookupLoc];
                if (pci.UniqueText == "x")
                    continue;

                if (pci != null && (pci.CellType == PivotCellType.ValueCell || pci.CellType == (PivotCellType.HeaderCell | PivotCellType.RowHeaderCell)))
                {
                    bool? b = true;
                   foreach(string col in ColumnFilterPopup.FilteredColumnlist)
                    {
                       int c = colIndexes[col];
                       PivotCellInfo pci1 = this.GridControlBase.PivotEngine.PivotValues[i, colIndexes[col]];
                        if (pci1 != null && ColumnFilterPopup.Exclusions[col].Contains(pci1.FormattedText))
                        {
                            if (col == colName && b == true)
                            {
                                b = false;
                            }
                            else
                            {
                                b = null;
                            }
                        }
                    }
                    if (b != null)
                    {
                        temp.Add(new FilterItem() { IsChecked = b, Text = pci.FormattedText, SortKey = isRowPivot ? pci.FormattedText as IComparable : pci.DoubleValue });
                    }
                    else
                    {
                        tempExclude.Add(pci.FormattedText);
                    }

                }
            }


            List<FilterChoice> tempFilterChoices = new List<FilterChoice>();
            List<FilterItem> filterElements = new List<FilterItem>(temp);
             
            FilterChoice filterChoice = null;
            filterElements.Sort(new FilterItemSorter());
            bool nullString = false;
            bool? allCheckBoxStatus = true;
            bool nullChecked = true;
            for (int i = 0; i < filterElements.Count; i++)
            {
                if (filterElements[i].Text != null)
                {
                    tempFilterChoices.Add(filterChoice = new FilterChoice() { Text = filterElements[i].Text, IsChecked = filterElements[i].IsChecked });
                    if (filterChoice.IsChecked.HasValue && filterChoice.IsChecked.Value == false)
                        allCheckBoxStatus = null;
                    filterChoice.PropertyChanged += new PropertyChangedEventHandler(FilterElement_PropertyChanged);
                }
                else
                {
                    nullString = true;
                    nullChecked = nullChecked && filterElements[i].IsChecked.HasValue && filterElements[i].IsChecked.Value;
                }
            }

            if (nullString)
            {
                tempFilterChoices.Add(filterChoice = new FilterChoice() { Text = "Null", IsChecked = nullChecked });
                filterChoice.PropertyChanged += new PropertyChangedEventHandler(FilterElement_PropertyChanged);
            }
            FilterListBoxItems = tempFilterChoices;
            AllCheckBox.IsChecked = allCheckBoxStatus;
        }

        private void FilterElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FilterChoice filterChoice = (sender as FilterChoice);
            if (filterChoice != null && filterChoice.IsChecked == null)
                return;
            if (filterChoice.Text == "Null")
                filterChoice.Text = null;
            if (!(FilterListBoxItems.Any(x => x.IsChecked == false || x.IsChecked == null)))
            {
                AllCheckBox.IsChecked = true;
                ColumnFilterPopup.OkButton.IsEnabled = true;
            }
            else
            {
                if (FilterListBoxItems.Any(x => x.IsChecked == true))
                {
                    ColumnFilterPopup.OkButton.IsEnabled = true;
                    AllCheckBox.IsChecked = null;
                }
                else
                {
                    ColumnFilterPopup.OkButton.IsEnabled = false;
                    AllCheckBox.IsChecked = false;
                }
            }

            ColumnFilterPopup.IsFilterChanged = true;
        }
#endif
    }

   /// <summary>
   /// Class holds the members used to maintain filter item in FilterPopup
   /// </summary>
    public class FilterChoice:NotificationObject
    {
        private string _text;
        /// <summary>
        /// Gets or sets the text of the filter item in FilterPopup list
        /// </summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }

        }

        private  bool? _isChecked;
        /// <summary>
        /// Gets or sets whether the Filter item at FilterPopup window in checked state or not
        /// </summary>
        public bool? IsChecked
        {
            get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                RaisePropertyChanged("IsChecked");
            }

        }
       
    }
    /// <summary>
    /// Event handler for the <see cref="FilterActionCompletedEventHandler"/> event.
    /// </summary>
    /// <param name="sender">The PivotGridControl raising the event.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void FilterActionCompletedEventHandler(object sender, FilterActionCompletedEventArgs e);
    /// <summary>
    /// Class to FilterActionCompleted event
    /// </summary>
    public class FilterActionCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the Pviotitem's field name
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Gets or sets the filter choices
        /// </summary>
        public List<FilterChoice> Choices { get; set; }
    }

#if !SILVERLIGHT
    /// <summary>
    /// Class that holds FilterButton members
    /// </summary>
    public class FilterButton : Button
    {
        /// <summary>
        /// Initializes the <see cref="FilterButton"/> class.
        /// </summary>
        public FilterButton()
        {
            DefaultStyleKey = typeof (FilterButton);
        }
    }
#endif
}

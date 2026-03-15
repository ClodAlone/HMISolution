#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

#if !SILVERLIGHT
namespace Syncfusion.Windows.Grid.Olap
#else
namespace Syncfusion.Silverlight.Grid.Olap
#endif
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Shapes;
    using Syncfusion.Olap.Engine;
    using System.ComponentModel;
    using System.Windows.Data;    
    using Syncfusion.Windows.Controls.Grid;
    using System.Windows.Controls.Primitives;

#if SILVERLIGHT
    using Syncfusion.OlapSilverlight.Engine;    
    using Syncfusion.OlapSilverlight.Reports;
    using Syncfusion.OlapSilverlight.Data;
    using Syncfusion.Windows.Shared;
#else
    using Syncfusion.Olap.Reports;
    using Syncfusion.Olap.Data;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Grid.Olap.Resources;
    using System.Globalization;
#endif

    /// <summary>
    /// OlapGrid Cell
    /// </summary>
    /// <remarks>
    /// This cell allows the user to drill up and down through the OLAP data also it has
    /// an ability to render itself as a hyperlink control
    /// </remarks>
    /// 
    #if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
    #endif
    [TemplatePart(Name = "PART_InnerTextBlock", Type = typeof(TextBlock))]
    public class OlapGridExpandHyperlinkCell : ContentControl
    {
        #region Dependency Property Declaration

        /// <summary>
        /// Describes the cell structure
        /// </summary>
        public static readonly DependencyProperty CellDescriptorProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapGridExpandHyperlinkCell), new UIPropertyMetadata(null));
#else
            DependencyProperty.Register("CellDescriptor", typeof(PivotCellDescriptor), typeof(OlapGridExpandHyperlinkCell), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets or sets weather this instance is Hyperlink cell or not
        /// </summary>
        /// <remarks>
        /// If this property it marked as true then this will have as an hyperlink control
        /// </remarks>
        public static readonly DependencyProperty IsHyperlinkCellProperty =
#if !SILVERLIGHT
            DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(OlapGridExpandHyperlinkCell), new UIPropertyMetadata(false));
#else
            DependencyProperty.Register("IsHyperlinkCell", typeof(bool), typeof(OlapGridExpandHyperlinkCell), new PropertyMetadata(false));
#endif

        #endregion

        #region Initilize/Finalize

#if !SILVERLIGHT
        /// <summary>
        /// Initializes the <see cref="OlapGridExpandHyperlinkCell"/> class.
        /// </summary>
        static OlapGridExpandHyperlinkCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OlapGridExpandHyperlinkCell), new FrameworkPropertyMetadata(typeof(OlapGridExpandHyperlinkCell)));
        }
#else
        /// <summary>
        /// Initializes the <see cref="OlapGridExpandHyperlinkCell"/> class.
        /// </summary>
        public OlapGridExpandHyperlinkCell()
        {
            DefaultStyleKey = typeof(OlapGridExpandHyperlinkCell);
        }

#endif

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the header cell expander clicked.
        /// </summary>
        public event OlapGridDrillDownEventHander ExpanderClicked;
#if SILVERLIGHT
        public event OlapGridDrillDownEventHander CellClicked;
#endif
        
        /// <summary>
        /// Occurs when link cell clicked.
        /// </summary>
        public event LinkLabelClickEventHander LinkClicked;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the Expander Style.
        /// </summary>
        /// <value>The expander style.</value>
        public Style ExpanderStyle
        {
            get
            {
                return this.GridControl.ExpanderStyle;
            }
            
        }

        /// <summary>
        /// Gets or sets the OlapGrid Control
        /// </summary>
        /// <value>The grid control.</value>
        public OlapGrid GridControl { get; set; }

        /// <summary>
        /// Gets or sets the cell descriptor.
        /// </summary>
        /// <value>The cell descriptor.</value>
        public PivotCellDescriptor CellDescriptor
        {
            get { return (PivotCellDescriptor)GetValue(CellDescriptorProperty); }
            set { SetValue(CellDescriptorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hyperlink cell.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is hyperlink cell; otherwise, <c>false</c>.
        /// </value>
        public bool IsHyperlinkCell
        {
            get { return (bool)GetValue(IsHyperlinkCellProperty); }
            set { SetValue(IsHyperlinkCellProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Current Cell is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpanded
        {
            get
            {
                if (this.CellDescriptor != null)
                {
                    if (CellDescriptor.ExpandableState == ExpandableState.Expanded)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
            set
            {                
            }
        }

        /// <summary>
        /// Gets or sets the whether the CellDescriptor has children.
        /// </summary>
        /// <value>The has children.</value>
        public Visibility HasChildren
        {
            get
            {
                if (this.CellDescriptor != null)
                {
                    if (this.CellDescriptor.HasChildren)
                    {
                        return Visibility.Visible;
                    }
                    else
                        return Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the Cell Value Textblock.
        /// </summary>
        /// <value>The cell value text block.</value>
        public TextBlock CellValueTextBlock { get; set; }

        /// <summary>
        /// Gets or sets the Expander Cell.
        /// </summary>
        /// <value>The internal expander.</value>
        public ToggleButton InternalExpander { get; set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the MouseUp event of the innerBorder control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void innerBorder_MouseUp(object sender, MouseButtonEventArgs e)
        {
#if !SILVERLIGHT
            if (e.ChangedButton == MouseButton.Left)
#endif
            {
                if (this.ExpanderClicked != null)
                {
                    //// Triggering the event when mouse left button is down
                    this.ExpanderClicked(this, new OlapGridDrillDownEventArgs
                    {
                        CellDescriptor = this.CellDescriptor,
                        ShowDefaultIndicator = true,
                    });
                }
            }
        }
        
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
#if SILVERLIGHT
            base.MouseLeftButtonDown += new MouseButtonEventHandler(OlapGridExpandHyperlinkCell_MouseLeftButtonDown);
            if (this.GridControl != null && this.GridControl.OlapDataManager.ItemSource == null)
            {
                ContextMenuAdv contextMenu = GetTemplateChild("PART_ContextMenuAdv") as ContextMenuAdv;
                contextMenu.Opened += new RoutedEventHandler(contextMenu_Opened);
            }
#else
            if (this.GridControl != null && this.GridControl.OlapDataManager.ItemSource == null)
            {
                ContextMenu contextMenu = GetTemplateChild("PART_ContextMenu") as ContextMenu;
                SkinStorage.SetVisualStyle(contextMenu,SkinStorage.GetVisualStyle(this.GridControl));
                InitializeContextMenu(contextMenu);
            }
#endif

#if !SILVERLIGHT
            Border innerBorder = GetTemplateChild("PART_InnerBorder") as Border;
            Border brd_PathBorder = GetTemplateChild("PART_PathBorder") as Border;
            ToggleButton expandbutton = GetTemplateChild("expand") as ToggleButton;
            expandbutton.Click += new RoutedEventHandler(expandbutton_Click);
            if (innerBorder != null)
            {
                innerBorder.MouseUp += new MouseButtonEventHandler(innerBorder_MouseUp);
            }
#else
              this.InternalExpander = GetTemplateChild("PART_Expander") as ToggleButton;
              if (this.InternalExpander != null)
              {
                  InternalExpander.Click += new RoutedEventHandler(InternalExpander_Click);
              }
#endif
              else
              {
                  UIElement customExpanderControl = GetTemplateChild("PART_Expander") as UIElement;
                  if (customExpanderControl != null)
                  {
#if !SILVERLIGHT
                      customExpanderControl.MouseUp += new MouseButtonEventHandler(innerBorder_MouseUp);
#else
                      customExpanderControl.MouseLeftButtonDown += new MouseButtonEventHandler(innerBorder_MouseUp);
#endif
                  }
              }

              this.CellValueTextBlock = GetTemplateChild("PART_InnerTextBlock") as TextBlock;
              if (this.CellValueTextBlock != null)
              {
#if !SILVERLIGHT
                  CellValueTextBlock.MouseDown += new MouseButtonEventHandler(CellValueTextBlock_MouseDown);
#else
                  if (this.CellDescriptor.CellType == PivotCellDescriptorType.RowHeader && this.IsHyperlinkCell ||
                      this.CellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader && this.IsHyperlinkCell)
                  {
                      CellValueTextBlock.MouseEnter += new MouseEventHandler(CellValueTextBlock_MouseEnter);
                      CellValueTextBlock.MouseLeave += new MouseEventHandler(CellValueTextBlock_MouseLeave);
                      CellValueTextBlock.MouseLeftButtonDown += new MouseButtonEventHandler(CellValueTextBlock_MouseDown);
                  }
                  ////Added code for TextWrapping only in Silverlight
                  if (this.CellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader)
                  {
                      this.CellValueTextBlock.TextWrapping = this.GridControl.ColumnHeaderStyle.TextWrapping; 
                  }
                  else if (this.CellDescriptor.CellType == PivotCellDescriptorType.RowHeader)
                  {
                      this.CellValueTextBlock.TextWrapping = this.GridControl.RowHeaderStyle.TextWrapping;
                  }
#endif
              }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Handles the opened event of the ContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void contextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var contextMenu = sender as ContextMenu;
            InitializeContextMenu(contextMenu);
        }

        private void InitializeContextMenu(ContextMenu contextMenu)
        {
            if (contextMenu != null)
            {
                contextMenu.Items.Clear();
                if (this.CellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader && this.GridControl != null)
                {
                    if (this.GridControl.EnableColumnHeaderContextMenu)
                    {
                        ContextMenuPopulation(contextMenu);
                    }
                    else
                    {
                        contextMenu.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
                else if (this.CellDescriptor.CellType == PivotCellDescriptorType.RowHeader && this.GridControl != null)
                {
                    if (this.GridControl.EnableRowHeaderContextMenu)
                    {
                        ContextMenuPopulation(contextMenu);
                    }
                    else
                    {
                        contextMenu.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Can execute context menu .
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private void ContextMenuCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Context menu executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private void ContextMenuExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            string _headerText = (sender as MenuItem).Header.ToString();
            bool isValid = RefreshOnContextMenuItemSelection(_headerText, this.GridControl.OlapDataManager.CurrentReport.SeriesElements);
            if (!isValid)
                RefreshOnContextMenuItemSelection(_headerText, this.GridControl.OlapDataManager.CurrentReport.CategoricalElements);
        }
#else
        /// <summary>
        /// Handles the opened event of the ContextMenu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void contextMenu_Opened(object sender, RoutedEventArgs e)
        {
            var contextMenu = sender as ContextMenuAdv;
            if (contextMenu != null)
            {
                contextMenu.Items.Clear();
                if (this.CellDescriptor.CellType == PivotCellDescriptorType.ColumnHeader && this.GridControl != null && this.GridControl.EnableColumnHeaderContextMenu)
                {
                    ContextMenuPopulation(contextMenu);
                }
                else if (this.CellDescriptor.CellType == PivotCellDescriptorType.RowHeader && this.GridControl != null && this.GridControl.EnableRowHeaderContextMenu)
                {
                    ContextMenuPopulation(contextMenu);
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the ContextMenuItemAdv control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void contextMenuItemAdv_Click(object sender, RoutedEventArgs e)
        {
            string _headerText = (sender as ContextMenuItemAdv).Header.ToString();
            bool isValid = RefreshOnContextMenuItemSelection(_headerText, this.GridControl.OlapDataManager.CurrentReport.SeriesElements);
            if (!isValid)
                RefreshOnContextMenuItemSelection(_headerText, this.GridControl.OlapDataManager.CurrentReport.CategoricalElements);
        }
#endif

        /// <summary>
        /// Context menu population.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="m_contextMenu">The m_context menu.</param>
#if SILVERLIGHT
        private void ContextMenuPopulation(ContextMenuAdv contextMenu)
#else
        private void ContextMenuPopulation(ContextMenu contextMenu)
#endif
        {
            Member member = this.CellDescriptor.Tag as Member;

            if (contextMenu != null && member != null)
            {
#if SILVERLIGHT
                var temp = this.GridControl.OlapDataManager.CurrentCubeSchema != null ? true : false;
                LevelCollection _levelCollection = this.GridControl.OlapDataManager.CurrentCubeSchema.Dimensions.Select(i => i).Where(i => i.Name == member.UniqueName.Split('.')[0].Replace("[","").Replace("]","")).FirstOrDefault().Hierarchies.Select(j => j).Where(j => j.Name == member.ParentHierarchy).FirstOrDefault().Levels;
                ContextMenuItemAdv _mainContextMenuItemAdv = new ContextMenuItemAdv() { Header = "Expand/Collapse", Background = this.Background, Foreground = this.Foreground };
                ContextMenuItemAdv _subContextMenuItemAdv = null;
                for (int i = this.GridControl.OlapDataManager.ShowLevelTypeAll ? 0 : 1; i < _levelCollection.Count; i++)
                {
                    _subContextMenuItemAdv = new Syncfusion.Windows.Shared.ContextMenuItemAdv() { Header = "Entirely to \"" + _levelCollection[i].Name + "\"", Background = this.Background, Foreground = this.Foreground };
                    _subContextMenuItemAdv.Click += new RoutedEventHandler(contextMenuItemAdv_Click);
                    _mainContextMenuItemAdv.Items.Add(_subContextMenuItemAdv);
                }
                string tempString = "Collapse ";
                for (int i = this.GridControl.OlapDataManager.ShowLevelTypeAll ? 0 : 1; i < _levelCollection.Count; i++)
                {

                    _subContextMenuItemAdv = new ContextMenuItemAdv() { Header = tempString + "\"" + this.CellDescriptor.CellValue + "\" to \"" + _levelCollection[i].Name + "\"", Background = this.Background, Foreground = this.Foreground };
                    _subContextMenuItemAdv.Click += new RoutedEventHandler(contextMenuItemAdv_Click);
                    _mainContextMenuItemAdv.Items.Add(_subContextMenuItemAdv);
                    if (_levelCollection[i].UniqueName == member.LevelUniqueName)
                        tempString = "Expand ";
                }
                contextMenu.Items.Add(_mainContextMenuItemAdv);
#else
                if((this.GridControl.OlapDataManager.CurrentCubeSchema.Dimensions.Select(i => i).Where(i => i.Name == member.ParentDimension).FirstOrDefault() !=null))
                {
                    LevelCollection _levelCollection = this.GridControl.OlapDataManager.CurrentCubeSchema.Dimensions.Select(i => i).Where(i => i.Name == member.ParentDimension).FirstOrDefault().Hierarchies.Select(j => j).Where(j => j.Name == member.ParentHierarchy).FirstOrDefault().Levels;
                    MenuItem _mainMenuItem = new MenuItem() { Header = SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Expand") + "/" + SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Collapse")};
                    MenuItem _subMenuItem = new MenuItem();
                    string localizedString = SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Entirely") + " " + SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_To").ToLower();
                    for (int i = (this.GridControl.OlapDataManager as Syncfusion.Olap.Manager.OlapDataManager).ShowLevelTypeAll ? 0 : 1; i < _levelCollection.Count; i++)
                    {
                        _subMenuItem = new MenuItem() { Header = localizedString + " \"" + _levelCollection[i].Name + "\""};
                        _subMenuItem.CommandBindings.Add(new CommandBinding(OlapGridCommands.ContextMenu, this.ContextMenuExecuted, this.ContextMenuCanExecute));
                        _subMenuItem.Command = OlapGridCommands.ContextMenu;
                        _mainMenuItem.Items.Add(_subMenuItem);
                    }
                    string tempString = SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Collapse") + " ";
                    for (int i = (this.GridControl.OlapDataManager as Syncfusion.Olap.Manager.OlapDataManager).ShowLevelTypeAll ? 0 : 1; i < _levelCollection.Count; i++)
                    {
                        _subMenuItem = new MenuItem() { Header = tempString + "\"" + this.CellDescriptor.CellValue + "\" " + SR.GetString("OlapGrid_ContextMenu_To").ToLower() + " \"" + _levelCollection[i].Name + "\""};
                        _subMenuItem.CommandBindings.Add(new CommandBinding(OlapGridCommands.ContextMenu, this.ContextMenuExecuted, this.ContextMenuCanExecute));
                        _subMenuItem.Command = OlapGridCommands.ContextMenu;
                        _mainMenuItem.Items.Add(_subMenuItem);
                        if (_levelCollection[i].UniqueName == member.LevelUniqueName)
                            tempString = SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Expand") + " ";
                    }
                    contextMenu.Items.Add(_mainMenuItem);
                }
#endif
                contextMenu.Visibility = contextMenu.Items.Count < 1 ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// Refresh control on context menu item selection.
        /// </summary>
        /// <param name="_headerText">The _header text.</param>
        private bool RefreshOnContextMenuItemSelection(string _headerText, Items _items)
        {
            Member _member = this.CellDescriptor.Tag as Member;
            foreach (Item _item in _items)
            {
                if (_item.ElementValue is DimensionElement)
                {
                    DimensionElement _dimensionElement = _item.ElementValue as DimensionElement;
                    if (_dimensionElement.Hierarchy.Name == _member.ParentHierarchy)
                    {
                        LevelElementCollection _levelElementCollection = (_item.ElementValue as DimensionElement).Hierarchy.LevelElements;
#if SILVERLIGHT
                         if (_headerText.Contains("Entirely to"))
#else
                        if (_headerText.Contains(SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Entirely") + " " + SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_To").ToLower()))
#endif
                        {
                            foreach (LevelElement _level in _levelElementCollection)
                                _level.MemberElements.Clear();
                            _dimensionElement.DrillState = DrillState.ExpandToLevel;
#if SILVERLIGHT
                            _dimensionElement.DrillUpDownLevel = _headerText.Replace("Entirely to \"", "").Replace("\"", "");
#else
                            _dimensionElement.DrillUpDownLevel = _headerText.Replace(SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Entirely") + " " + SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_To").ToLower() + " \"", "").Replace("\"", "");
#endif
                        }
                        else
                        { 
#if SILVERLIGHT
                            if (_headerText.Contains("Collapse")) 
#else
                            if (_headerText.Contains(SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Collapse")))
#endif
                                _dimensionElement.DrillState = DrillState.CollapseToLevel;
                            else
                                _dimensionElement.DrillState = DrillState.ExpandToLevel;
#if SILVERLIGHT
                            string[] _value = _headerText.Replace("\"", "").Replace("Expand ", "").Replace("Collapse ", "").Split(new string[] { " to " }, StringSplitOptions.RemoveEmptyEntries);
#else
                            string[] _value = _headerText.Replace("\"", "").Replace(SR.GetString(CultureInfo.CurrentUICulture,"OlapGrid_ContextMenu_Expand") + " ", "").Replace(SR.GetString("OlapGrid_ContextMenu_Collapse") + " ", "").Split(new string[] { " " + SR.GetString("OlapGrid_ContextMenu_To").ToLower() + " " }, StringSplitOptions.RemoveEmptyEntries);
#endif
                            _dimensionElement.DrillUpDownMember = _value[0];
                            _dimensionElement.DrillUpDownLevel = _value[1];
                        }
                        this.GridControl.DataBind();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Handles the Click event of the expandbutton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void expandbutton_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExpanderClicked != null)
            {
                //// Triggering the event when mouse left button is down
                this.ExpanderClicked(this, new OlapGridDrillDownEventArgs
                {
                    CellDescriptor = this.CellDescriptor,
                    ShowDefaultIndicator = true,
                });
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Handles the MouseLeftButtonDown event of the OlapGridExpandHyperlinkCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OlapGridExpandHyperlinkCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                if (this.CellClicked != null)
                {
                    //// Triggering the event when mouse left button is down
                    this.CellClicked(this, new OlapGridDrillDownEventArgs
                    {
                        CellDescriptor = this.CellDescriptor,
                        ShowDefaultIndicator = true,
                    });
                }
            }
        }
#endif
        /// <summary>
        /// Handles the Click event of the InternalExpander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void InternalExpander_Click(object sender, RoutedEventArgs e)
        {
            if (this.ExpanderClicked != null)
            {
                this.InternalExpander.IsChecked = this.IsExpanded;
                //// Triggering the event when mouse left button is down
                this.ExpanderClicked(this, new OlapGridDrillDownEventArgs
                {
                    CellDescriptor = this.CellDescriptor,
                    ShowDefaultIndicator = true,
                });
            }
        }

        /// <summary>
        /// Handles the MouseDown event of the CellValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void CellValueTextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.LinkClicked != null)
            {
                //// Triggering when the hyperlink cell is clicked
                this.LinkClicked(this, new LinkLabelEventArgs(CellDescriptor));
            }
        }

#if SILVERLIGHT
        /// <summary>
        /// Handles the MouseLeave event of the CellValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void CellValueTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this, "Normal", true);
        }

        /// <summary>
        /// Handles the MouseEnter event of the CellValueTextBlock control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void CellValueTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            VisualStateManager.GoToState(this , "MouseOver", true);
        }        
        
#endif
        #endregion
    }

#if !SILVERLIGHT

    public class HeaderCellToolTipConvertor : IValueConverter
    {
        #region IValueConverter Members

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
            GridRenderStyleInfo rendererStyleInfo = value as GridRenderStyleInfo;
            if (rendererStyleInfo != null)
            {
                PivotCellDescriptor cellDescriptor = rendererStyleInfo.CellValue as PivotCellDescriptor;

                if (cellDescriptor != null)
                {
                    OlapGridBase gridBase = rendererStyleInfo.GridControl as OlapGridBase;
                    if (gridBase != null)
                    {
                        PivotEngine engine = gridBase.DataManager.GetExpandedRows(cellDescriptor);
                        if (engine != null)
                        {
                            foreach (PivotColumnDescriptor pvtColumnDescriptor in engine.TableColumns)
                            {
                                foreach (PivotCellDescriptor pvtCellDescriptor in pvtColumnDescriptor.Cells)
                                {
                                    pvtCellDescriptor.ExpandableState = ExpandableState.None;
                                    pvtCellDescriptor.HasChildren = false;
                                }
                            }
                        }
                        return engine;
                    }
                }
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

        #endregion
    }

    public class HeaderCellMemberPropertyToolTipConverter : IValueConverter
    {
        #region IValueConverter Members

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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            GridRenderStyleInfo rendererStyleInfo = value as GridRenderStyleInfo;
            if (rendererStyleInfo != null)
            {
                PivotCellDescriptor cellDescriptor = rendererStyleInfo.CellValue as PivotCellDescriptor;
                if (rendererStyleInfo != null && cellDescriptor != null)
                {
                    OlapGridBase gridBase = rendererStyleInfo.GridControl as OlapGridBase;
                    if (gridBase != null)
                    {
                        PivotEngine engine = gridBase.DataManager.GetMemberProperties(cellDescriptor);
                        return engine;
                    }
                }
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

    #endregion
    }

#endif
}

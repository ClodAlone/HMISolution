// <copyright file="ChartToolBar.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Windows.Media.Imaging;
    using Syncfusion.Windows.Chart;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Licensing;
    using System.Windows.Markup;
    using System.Xml;
    using System.Text;

    #region ChartToolBarCommands
    /// <summary>
    /// Represents commands that can be invoked in <see cref="ChartToolBar"/>.
    /// </summary>
    /// <remarks>
    /// Commanding is an input mechanism in Windows Presentation Foundation 
    /// which provides input handling at a more semantic level than device input.
    /// </remarks>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public static class ChartToolBarCommands
    {
        #region Members
        /// <summary>
        /// Initializes c_switchPrinting
        /// </summary>
        private readonly static RoutedUICommand c_switchPrinting = new RoutedUICommand("SwitchPrinting", "SwitchPrinting", typeof(ChartToolBarCommands));

        /// <summary>
        /// Initializes c_changeChartType
        /// </summary>
        private readonly static RoutedUICommand c_changeChartType = new RoutedUICommand("ChangeChartType", "ChangeChartType", typeof(ChartToolBarCommands));

        /// <summary>
        /// Initializes c_showHideLegend
        /// </summary>
        private readonly static RoutedUICommand c_showHideLegend = new RoutedUICommand("ShowHideLegend", "ShowHideLegend", typeof(ChartToolBarCommands));

        /// <summary>
        /// Initializes c_switch3DMode
        /// </summary>
        private readonly static RoutedUICommand c_switch3DMode = new RoutedUICommand("Switch3DMode", "Switch3DMode", typeof(ChartToolBarCommands));

        /// <summary>
        /// Initializes c_ChangeColorPalette
        /// </summary>
        private readonly static RoutedUICommand c_ChangeColorPalette = new RoutedUICommand("ChangeColorPalette", "ChangeColorPalette", typeof(ChartToolBarCommands));

        /// <summary>
        /// Initializes c_EnableZooming
        /// </summary>
        private readonly static RoutedUICommand c_EnableZooming = new RoutedUICommand("EnableZooming", "EnableZooming", typeof(ChartToolBarCommands));

        /// <summary>
        /// Initializes c_ShowPropertiesDialogue
        /// </summary>
        private readonly static RoutedUICommand c_propertiesDialogue = new RoutedUICommand("PropertiesDialogue", "PropertiesDialogue", typeof(ChartToolBarCommands));

        #endregion

        #region Properties

        /// <summary>
        /// Gets the switch printing.
        /// </summary>
        /// <value>The switch printing.</value>
        public static RoutedUICommand SwitchPrinting
        {
            get
            {
                return c_switchPrinting;
            }
        }

        /// <summary>
        /// Gets the enable zooming.
        /// </summary>
        /// <value>The enable zooming.</value>
        public static RoutedUICommand EnableZooming
        {
            get
            {
                return c_EnableZooming;
            }
        }

        /// <summary>
        /// Gets the type of the change chart.
        /// </summary>
        /// <value>The type of the change chart.</value>
        public static RoutedUICommand ChangeChartType
        {
            get
            {
                return c_changeChartType;
            }
        }

        /// <summary>
        /// Gets the show hide legend.
        /// </summary>
        /// <value>The show hide legend.</value>
        public static RoutedUICommand ShowHideLegend
        {
            get
            {
                return c_showHideLegend;
            }
        }

        /// <summary>
        /// Gets the switch3 D mode.
        /// </summary>
        /// <value>The switch3 D mode.</value>
        public static RoutedUICommand Switch3DMode
        {
            get
            {
                return c_switch3DMode;
            }
        }

        /// <summary>
        /// Gets the change color palette.
        /// </summary>
        /// <value>The change color palette.</value>
        public static RoutedUICommand ChangeColorPalette
        {
            get
            {
                return c_ChangeColorPalette;
            }
        }

        /// <summary>
        /// Gets the Properties Dialogue.
        /// </summary>
        /// <value>The Properties Dialogue.</value>
        public static RoutedUICommand PropertiesDialogue
        {
            get
            {
                return c_propertiesDialogue;
            }
        }
        #endregion
    }
    #endregion

    #region ChartToolBar
    /// <summary>
    /// Represents ChartToolBar Class.
    /// </summary>
    /// <remarks>
    /// Chart Toolbar hosts various items to perform Chart functions such as Print,
    /// Save, Change chart type and so on.
    /// </remarks>
    /// <example>
    /// <code language="XAML">
    /// &lt;syncfusion:Chart.ToolBar &gt; &lt;syncfusion:ChartToolBar
    /// CloseButtonVisibility="True" TitleBarVisibility="True" Header="True" /&gt;
    /// &lt;/syncfusion:Chart.ToolBar&gt;
    /// </code>
    /// </example>
    /// <seealso cref="ChartToolBar"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    
    public class ChartToolBar : HeaderedItemsControl, IChartSerializer
    {
        #region Dependency Properties
        /// <summary>
        /// Identifies the ItemsOrientation dependency property.
        /// </summary>
        public static DependencyProperty ItemsOrientationProperty =
          DependencyProperty.Register("ItemsOrientation", typeof(Orientation), typeof(ChartToolBar), new PropertyMetadata(Orientation.Horizontal));

        /// <summary>
        /// Identifies the HeaderBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
           DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(ChartToolBar), new PropertyMetadata(Brushes.Transparent));

        /// <summary>
        /// Identifies the CloseButtonVisibility dependency property.
        /// </summary>     
        public static readonly DependencyProperty CloseButtonVisibilityProperty =
          DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(ChartToolBar), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the TitleBarVisibility dependency property.
        /// </summary>  
        public static readonly DependencyProperty TitleBarVisibilityProperty =
        DependencyProperty.Register("TitleBarVisibility", typeof(Visibility), typeof(ChartToolBar), new UIPropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>  
        public static readonly DependencyProperty SelectedItemProperty =
          DependencyProperty.Register("SelectedItem", typeof(ToolBarItem), typeof(ChartToolBar), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedItemChanged)));

        #endregion

        #region Properties
        /// <summary>
        /// Declares the ToolBar CloseButton
        /// </summary>
        private Button closeButton;

        /// <summary>
        /// Gets or sets the ToolBar CloseButton
        /// </summary>
        /// <value>The CloseButton.</value>
        public Button CloseButton
        {
            get { return closeButton; }
            set { closeButton = value; }
        }

        /// <summary>
        /// Gets or sets the Items Orientation. 
        /// </summary>
        /// <value>The ItemsOrientation.</value>
        public Orientation ItemsOrientation
        {
            get
            {
                return (Orientation)this.GetValue(ChartToolBar.ItemsOrientationProperty);
            }

            set
            {
                this.SetValue(ChartToolBar.ItemsOrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Header Background. 
        /// </summary>
        /// <value>The HeaderBackground.</value>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Close Button Visibility
        /// </summary>
        /// <value>The CloseButtonVisibility.</value>
        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the TitleBar Visibility
        /// </summary>
        /// <value>The TitleBarVisibility.</value>
        public Visibility TitleBarVisibility
        {
            get { return (Visibility)GetValue(TitleBarVisibilityProperty); }
            set { SetValue(TitleBarVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Selected Item
        /// </summary>
        /// <value>The SelectedItem.</value>
        public ToolBarItem SelectedItem
        {
            get
            {
                return (ToolBarItem)GetValue(SelectedItemProperty);
            }

            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes static members of the <see cref="ChartToolBar"/> class.
        /// </summary>
        static ChartToolBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartToolBar), new FrameworkPropertyMetadata(typeof(ChartToolBar)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartToolBar"/> class.
        /// </summary>
        public ChartToolBar()
        {
            CommandBinding switchPrintingBinding = new CommandBinding(ChartToolBarCommands.SwitchPrinting, new ExecutedRoutedEventHandler(OnSwitchPrintingCommand));
            CommandBinding changeChartTypeBinding = new CommandBinding(ChartToolBarCommands.ChangeChartType, new ExecutedRoutedEventHandler(OnChangeChartTypeCommand));
            CommandBinding showHideLegendBinding = new CommandBinding(ChartToolBarCommands.ShowHideLegend, new ExecutedRoutedEventHandler(OnShowHideLegendCommand));
            CommandBinding switch3DModeBinding = new CommandBinding(ChartToolBarCommands.Switch3DMode, new ExecutedRoutedEventHandler(OnSwitch3DModeCommand));
            CommandBinding colorPaletteBinding = new CommandBinding(ChartToolBarCommands.ChangeColorPalette, new ExecutedRoutedEventHandler(OncolorPaletteCommand));
            CommandBinding enableZooming = new CommandBinding(ChartToolBarCommands.EnableZooming, new ExecutedRoutedEventHandler(OnEnableZoomingCommand));
            CommandBinding propertiesDialogueBinding = new CommandBinding(ChartToolBarCommands.PropertiesDialogue, new ExecutedRoutedEventHandler(OnPropertiesDialogueCommand));

            CommandManager.RegisterClassCommandBinding(typeof(Chart), switchPrintingBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), changeChartTypeBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), showHideLegendBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), switch3DModeBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), colorPaletteBinding);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), enableZooming);
            CommandManager.RegisterClassCommandBinding(typeof(Chart), propertiesDialogueBinding);
            this.MouseEnter += new MouseEventHandler(ChartToolBar_MouseEnter);
            this.AddItems();       
            
        }

        void ChartToolBar_MouseEnter(object sender, MouseEventArgs e)
        {
            FocusManager.SetFocusedElement(this, e.MouseDevice.Captured);
            this.Focus();
        }

        #endregion

       
        /// <summary>
        /// Invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"></see>.
        /// </summary>
        /// <seealso cref="ChartToolBar"/>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Button button = GetTemplateChild("PART_ToolBarCloseButton") as Button;
            if (button != null)
            {
                this.CloseButton = button;
            }
        }


        internal void Dispose()
        {
            if (this.Items != null)
            {
                this.Items.Clear();
            }

            this.MouseEnter -= ChartToolBar_MouseEnter;
            this.ItemsPanel = null;

            if (this.Template != null)
            {
                if (((ControlTemplate)(this.Template)).Resources != null)
                {
                    ((ControlTemplate)(this.Template)).Resources.MergedDictionaries.Clear();
                }
            }
        }

        #region Member functions



        /// <summary>
        /// Adds the items.
        /// </summary>
        private void AddItems()
        {
            ChartResourceWrapper wrapper = new ChartResourceWrapper();
            ////Print Item            
            ToolBarItem toolBarItemPrint = new ToolBarItem();
            toolBarItemPrint.ToolTip = wrapper.ToolbarPrint; //"Print";
            toolBarItemPrint.ItemImageSource = ChartResources.ToolBarItemPrint;
            toolBarItemPrint.Command = ApplicationCommands.Print;          
            this.Items.Add(toolBarItemPrint);

            ////Print Item
            ToolBarItem toolBarItemSwitchPrintMode = new ToolBarItem();
            toolBarItemSwitchPrintMode.ToolTip = wrapper.ToolbarSwitchPrint;// "Switch Print Mode";
            toolBarItemSwitchPrintMode.ItemImageSource = ChartResources.ToolBarItemSwitchPrint;
            toolBarItemSwitchPrintMode.Command = ChartToolBarCommands.SwitchPrinting;
          
            this.Items.Add(toolBarItemSwitchPrintMode);

            ////Save Item
            ToolBarItem toolBarItemSave = new ToolBarItem();
            toolBarItemSave.ToolTip = wrapper.Save; //" ";
            toolBarItemSave.ItemImageSource = ChartResources.ToolBarItemSave;
            toolBarItemSave.Command = ApplicationCommands.Save;
        
            this.Items.Add(toolBarItemSave);

            ////Copy Item
            ToolBarItem toolBarItemCopy = new ToolBarItem();
            toolBarItemCopy.ToolTip = wrapper.Copy; //"Copy";
            toolBarItemCopy.ItemImageSource = ChartResources.ToolBarItemCopy;
            toolBarItemCopy.Command = ApplicationCommands.Copy;
         
            this.Items.Add(toolBarItemCopy);

            ////Legend
            ToolBarItem toolBarItemLegend = new ToolBarItem();
            toolBarItemLegend.ToolTip = wrapper.ToolbarLegend; // "Show/Hide Legend";
            toolBarItemLegend.ItemImageSource = ChartResources.ToolBarItemLegend;
            toolBarItemLegend.Command = ChartToolBarCommands.ShowHideLegend;
      
            this.Items.Add(toolBarItemLegend);

            ////Enable Zooming
            ToolBarItem toolBarItemZooming = new ToolBarItem();
            toolBarItemZooming.ToolTip = wrapper.EnableZooming; // "Enable Zooming";
            toolBarItemZooming.ItemImageSource = ChartResources.ToolBarItemZoom;
            toolBarItemZooming.Command = ChartToolBarCommands.EnableZooming;        
            this.Items.Add(toolBarItemZooming);

            ////Color Palette
            ToolBarItem toolBarItemPalette = new ToolBarItem();
            toolBarItemPalette.IsDropDown = true;
            toolBarItemPalette.ToolTip = wrapper.ColorPalette;// "Color Palette";
            toolBarItemPalette.ItemImageSource = ChartResources.ToolBarItemColorPalette;
            toolBarItemPalette.MouseLeftButtonDown += new MouseButtonEventHandler(ToolBarItemPalette_MouseLeftButtonDown);
            toolBarItemPalette.KeyDown += new KeyEventHandler(ToolBarItemPalette_KeyDown);
            Array palettes = Enum.GetValues(typeof(ChartColorPalette));
            foreach (ChartColorPalette palette in palettes)
            {
                ToolBarItem paletteitem = new ToolBarItem();
                paletteitem.Tag = palette.ToString();
                object str = ChartDataUtils.GetPropertyDescriptor(wrapper, palette.ToString());
                paletteitem.Text = str == null ? palette.ToString() : str.ToString();
                paletteitem.BorderBrush = Brushes.Transparent;
                paletteitem.Command = ChartToolBarCommands.ChangeColorPalette;
                paletteitem.CommandParameter = palette;          
                paletteitem.MinHeight = 18;
                paletteitem.HorizontalAlignment = HorizontalAlignment.Stretch;
                toolBarItemPalette.Items.Add(paletteitem);
            }

            this.Items.Add(toolBarItemPalette);

            ////Chart Type
            ToolBarItem toolBarItemType = new ToolBarItem();
            toolBarItemType.ToolTip = wrapper.ChangeType;// "Change Type";
            toolBarItemType.IsDropDown = true;
            toolBarItemType.ItemImageSource = ChartResources.ToolBarItemType;
            toolBarItemType.MouseLeftButtonDown += new MouseButtonEventHandler(ToolBarItemType_MouseLeftButtonDown);
            toolBarItemType.KeyDown += new KeyEventHandler(ToolBarItemType_KeyDown); 
            string[] types = Enum.GetNames(typeof(ChartTypes));
            foreach (string name in types)
            {
                ToolBarItem typeItem = new ToolBarItem();
                typeItem.MinHeight = 18;
                object str = ChartDataUtils.GetPropertyDescriptor(wrapper, name);
                typeItem.Text = str == null ? name : str.ToString();
                typeItem.BorderBrush = Brushes.Transparent;
                ChartTypes type = (ChartTypes)Enum.Parse(typeof(ChartTypes), name);
                typeItem.Tag = ChartSeries.KnownType(type);
                typeItem.HorizontalAlignment = HorizontalAlignment.Stretch;
                typeItem.Command = ChartToolBarCommands.ChangeChartType;
                typeItem.CommandParameter = ChartSeries.KnownType(type);
                toolBarItemType.Items.Add(typeItem);
            }

            this.Items.Add(toolBarItemType);

            ////Chart Property Dialog
            ToolBarItem toolBarItemProperties = new ToolBarItem();
            toolBarItemProperties.ToolTip = wrapper.Properties;// "Properties";
            toolBarItemProperties.ItemImageSource = ChartResources.ToolBarItemProperties;
            toolBarItemProperties.Command = ChartToolBarCommands.PropertiesDialogue;
            this.Items.Add(toolBarItemProperties);

        }

        /// <summary>
        /// Sets the chart palette.
        /// </summary>
        /// <param name="item">The UIElement item.</param>
        private void SetChartPalette(UIElement item)
        {
            Chart chart = VisualUtils.FindAncestor(this, typeof(Chart)) as Chart;
            if (item.GetType() == typeof(ToolBarItem))
            {
                ToolBarItem toolBarItem = item as ToolBarItem;
                foreach (ToolBarItem paletteItem in toolBarItem.Items)
                {
                    if (chart.Areas[0].Series[0] != null)
                    {
                        paletteItem.IsSelected = paletteItem.Tag.ToString() == chart.Areas[0].ColorModel.Palette.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Sets the charttype.
        /// </summary>
        /// <param name="item">The UIElement item.</param>
        private void SetCharttype(UIElement item)
        {            
            Chart chart = VisualUtils.FindAncestor(this, typeof(Chart)) as Chart;
           
            if (item.GetType() == typeof(ToolBarItem))
            {
                ToolBarItem toolBarItem = item as ToolBarItem;
                foreach (ToolBarItem typeItem in toolBarItem.Items)
                {
                    if (chart.Areas[0].Series[0] != null)
                    {
                        typeItem.IsSelected = typeItem.Tag.GetType() == chart.Areas[0].Series[0].ChartType.GetType();
                    }
                }
            }
        }

        #endregion

        #region Events
        /// <summary>
        /// Represents Selected Item changed.
        /// </summary>
        public event ChartToolBarEventHandler SelectedItemChanged;

        /// <summary>
        /// Called when the Selected Item is changed.
        /// </summary>
        /// <param name="d">The DependencyObject.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs instance containing the event data.</param> 
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartToolBar instance = (ChartToolBar)d;
            instance.OnSelectedItemChanged(e);
            if (instance.SelectedItemChanged != null)
            {
                if (e.OldValue != null)
                {       
                    instance.SelectedItemChanged(instance, new ChartToolBarArgs(instance, (ToolBarItem)e.OldValue, (ToolBarItem)e.NewValue));
                }
            }
        }

        /// <summary>
        /// Invoked when <see cref="SelectedItem"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            ToolBarItem toolBarItem = e.OldValue as ToolBarItem;
            ToolBarItem newToolBarItem = e.NewValue as ToolBarItem;

            if (toolBarItem != null && toolBarItem.Tag == null)
            {
                if (toolBarItem != newToolBarItem)
                {
                    toolBarItem.IsSelected = false;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the toolBarItemType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        public void ToolBarItemType_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement item = sender as UIElement;
            SetCharttype(item);
        }
        /// <summary>
        /// Handles the KeyDown event of the toolBarItemType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void ToolBarItemType_KeyDown(object sender, KeyEventArgs e)
        {
            UIElement item = sender as UIElement;
            SetCharttype(item);           
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the toolBarItemPalette control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        public void ToolBarItemPalette_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement item = sender as UIElement;
            SetChartPalette(item);
        }

        /// <summary>
        /// Handles the KeyDown event of the toolBarItemPalette control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void ToolBarItemPalette_KeyDown(object sender, KeyEventArgs e)
        {
            UIElement item = sender as UIElement;
            SetChartPalette(item);
        }

        /// <summary>
        /// Handles the KeyDown event of the toolBarItemProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        public void ToolBarItemProperties_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
             //ChartProperties.PropertyItem prop = new ChartProperties.PropertyItem();           
             //prop.Activate();           
             //prop.Show();
             //prop.BringIntoView();
            // prop.Focus();
        }

        /// <summary>
        /// Handles the ChartCommands.Switch3DMode routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSwitch3DModeCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;
            foreach (ChartArea c_Area in chart.Areas)
            {
                c_Area.View3DMode = !c_Area.View3DMode;
            }
        }

        /// <summary>
        /// Handles the <see cref="ChartToolBarCommands.EnableZooming"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnEnableZoomingCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;
            foreach (ChartArea c_Area in chart.Areas)
            {
                if (c_Area is SyncChartAreas)
                {
                    SyncChartAreas area = c_Area as SyncChartAreas;
                    if (!area.Areas[0].ZoomSwitched)
                    {
                        ChartAreaCommands.SwitchZooming.Execute(null, c_Area);
                    }
                    else
                    {
                        ChartAreaCommands.CancelZooming.Execute(null, c_Area);
                    }
                }
                else
                {
                    if (!c_Area.ZoomSwitched)
                    {
                        ChartAreaCommands.SwitchZooming.Execute(null, c_Area);
                    }
                    else
                    {
                        ChartAreaCommands.CancelZooming.Execute(null, c_Area);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="ChartToolBarCommands.ShowHideLegend"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnShowHideLegendCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;
            ChartToolBar tool = target as ChartToolBar;
            if (chart != null && chart.Legends.Count > 0)
            {
                foreach (ChartLegend legend in chart.Legends)
                {
                    if (legend.Visibility == Visibility.Visible)
                    {
                        legend.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        legend.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="ChartToolBarCommands.ChangeColorPalette"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OncolorPaletteCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;
            foreach (ChartArea area in chart.Areas)
            {
                area.ColorModel.Palette = (ChartColorPalette)args.Parameter;
            }
        }

        /// <summary>
        /// Handles the <see cref="ChartToolBarCommands.ChangeChartType"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnChangeChartTypeCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;
            foreach (ChartArea area in chart.Areas)
            {
                foreach (ChartSeries series in area.Series)
                {
                    series.Type = (ChartTypes)Enum.Parse(typeof(ChartTypes), args.Parameter.ToString());
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="ChartToolBarCommands.SwitchPrinting"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSwitchPrintingCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chart = target as Chart;

            if (chart != null)
            {
                chart.SwitchPrintingMode();
            }
        }

        /// <summary>
        /// Handles the <see cref="ChartToolBarCommands.PropertiesDialogue"/> routed command;
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="args">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnPropertiesDialogueCommand(object target, ExecutedRoutedEventArgs args)
        {
            Chart chartValue = target as Chart;          
            if (chartValue != null)
            {                
                //if (chartValue.propertyItem == null)
                //{                    
                //    chartValue.propertyItem = new PropertyItem();                   
                //}   
            
                //chartValue.propertyItem.SetValues(chartValue);                              
                //chartValue.propertyItem.ShowDialog();   
                chartValue.ShowPropertyDialog();                                                    
            }
        }
        #endregion

        #region IChartSerializer Members

        /// <summary>
        /// Method declaration for Serialize
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string _xamlString;
            StringBuilder outstr = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(outstr, settings));
            //this string need for turning on expression saving mode 
            dsm.XamlWriterMode = XamlWriterMode.Expression;
            XamlWriter.Save(this, dsm);
            _xamlString = outstr.ToString();
            string [] xamltags = _xamlString.Split('>');
            _xamlString = xamltags[0];
            _xamlString += "/>";
            return _xamlString;
        }

        /// <summary>
        /// Method declaration for DeSerialize
        /// </summary>
        /// <param name="xamlString"></param>
        /// <returns></returns>
        public object Deserialize(string xamlString)
        {
            return XamlReader.Parse(xamlString);
        }

        #endregion
    }

    #endregion

    #region ToolBarItem
    /// <summary>
    /// Represents Chart ToolBarItem class
    /// </summary>
    /// <seealso cref="ChartToolBar"/>
    #if SyncfusionFramework4_0
        [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class ToolBarItem :MenuItem
    {
        #region Dependency Properties
        /// <summary>
        /// Identifies the IsPressed dependency property.
        /// </summary> 
        public new static readonly DependencyProperty IsPressedProperty =
        DependencyProperty.Register("IsPressed", typeof(bool), typeof(ToolBarItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the IsSelected dependency property.
        /// </summary> 
        public static readonly DependencyProperty IsSelectedProperty =
  DependencyProperty.Register("IsSelected", typeof(bool), typeof(ToolBarItem), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsSelectedChanged)));

        /// <summary>
        /// Identifies the IsDropDown dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownProperty =
  DependencyProperty.Register("IsDropDown", typeof(bool), typeof(ToolBarItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the Text dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(string), typeof(ToolBarItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ItemImageSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemImageSourceProperty =
     DependencyProperty.Register("ItemImageSource", typeof(ImageSource), typeof(ToolBarItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsOpen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
DependencyProperty.Register("IsOpen", typeof(bool), typeof(ToolBarItem), new UIPropertyMetadata(false));
     
      #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the Item ImageSource
        /// </summary>
        /// <value>The ItemImageSource.</value>
        public ImageSource ItemImageSource
        {
            get
            {
                return (ImageSource)GetValue(ItemImageSourceProperty);
            }

            set
            {
                SetValue(ItemImageSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Item's text
        /// </summary>
        /// <value>The Text value.</value>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the popup is kept opened
        /// </summary>
        /// <value>The IsOpen.</value>
        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        ///    <c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public new bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            set { SetValue(IsPressedProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(ToolBarItem.IsSelectedProperty);
            }

            set
            {
                SetValue(ToolBarItem.IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop down.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is drop down; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropDown
        {
            get
            {
                return (bool)GetValue(ToolBarItem.IsDropDownProperty);
            }

            set
            {
                SetValue(ToolBarItem.IsDropDownProperty, value);
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="ToolBarItem"/> class.
        /// </summary>
        static ToolBarItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolBarItem), new FrameworkPropertyMetadata(typeof(ToolBarItem)));
            
        }
        
        #endregion

        #region Helper Methods
        /// <summary>
        /// Moves the focus to next item.
        /// </summary>
        private void MoveFocusToNextItem()
        {
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
            UIElement focusedElement = Keyboard.FocusedElement as UIElement;

            if (focusedElement != null)
            {
                focusedElement.MoveFocus(request);
            }
        }

        /// <summary>
        /// Moves the focus to previous item.
        /// </summary>
        private void MoveFocusToPreviousItem()
        {
            TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Previous);
            UIElement focusedElement = Keyboard.FocusedElement as UIElement;

            if (focusedElement != null)
            {
                focusedElement.MoveFocus(request);
            }
        }
        #endregion

        #region Events

       
        /// <summary>
        /// Called when ToolBarItem is selected changed.
        /// </summary>
        /// <param name="d">The ToolBarItem DependencyObject.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolBarItem toolBarItem = d as ToolBarItem;
            ChartToolBar toolbar = VisualUtils.FindAncestor(toolBarItem, typeof(ChartToolBar)) as ChartToolBar;
            if (toolBarItem.IsSelected == true)
            {
                if (toolBarItem.Command != ChartToolBarCommands.ChangeChartType && toolBarItem.Command != ChartToolBarCommands.ChangeColorPalette)
                {
                    toolbar.SelectedItem = toolBarItem;
                }
            }
            
        }             
        
        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseLeave"/>�attached event is raised on this element. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (Mouse.Captured == null && Keyboard.FocusedElement != this)
            {
                IsPressed = false;
                //IsSelected = false;
                IsOpen = false;
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.MouseDown"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. This event data reports details about the mouse button that was pressed and the handled state.</param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                IsOpen = true;
                //this.IsSelected = false;
                if (this.Command != ChartToolBarCommands.ChangeChartType || this.Command != ChartToolBarCommands.ChangeColorPalette)
                {

                    //ToolBarItem item =(ToolBarItem)e.Source;
                    //if(this.Items.Count>0)
                    //{
                    //    for (int i = 0; i < this.Items.Count; i++)
                    //    {
                    //        ToolBarItem toolbaritem = this.Items[i] as ToolBarItem;
                    //        toolbaritem.IsSelected = false;
                    //    }
                    //}
                   
                    //this.IsSelected = true;
                    this.IsPressed = true;
                    this.IsOpen = true;
                }
                if (this.Parent != null && this.Parent is ToolBarItem)
                {
                    foreach (ToolBarItem item in (this.Parent as ToolBarItem).Items)
                    {
                        if (this.Text != null && this.Text.ToString().Equals((item as ToolBarItem).Text.ToString()))
                        {
                            item.IsSelected = true;
                        }
                        else
                        {
                            item.IsSelected = false;
                        }
                    }
                }
                else
                {
                    this.IsSelected = true;
                }
            }
        }

        /// <summary>
        /// Handles MouseLeftButtonDown event.
        /// </summary>
        /// <param name="e">The MouseButtonEventArgs</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (Command != null)
            {
                Command.Execute(CommandParameter);
            }
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.GotKeyboardFocus"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {           
            base.OnGotKeyboardFocus(e);
        }
   
        /// <summary>
        /// Invoked when the <see cref="E:System.Windows.UIElement.KeyDown"/> event is received.
        /// </summary>
        /// <param name="e">Information about the event.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (this.IsDropDown && this.IsPressed == false)
                {
                    this.IsPressed = true;                   
                    this.IsSelected = true;
                    this.IsOpen = true;
                    if (this.Items.Count > 0)
                    {
                        ToolBarItem toolbaritem = this.Items[0] as ToolBarItem;                       
                        Keyboard.Focus(toolbaritem);
                      
                    }
                }

                if (!this.IsDropDown)
                {
                    if (Command != null)
                    {
                        Command.Execute(CommandParameter);
                        ToolBarItem parentToolBarItem = VisualUtils.FindSomeParent(this, typeof(ToolBarItem)) as ToolBarItem;
                        if (parentToolBarItem == null)
                        {
                            this.IsSelected = true;
                        }
                    }
                }
              
            }
            if (e.Key == Key.Escape)
            {
                if (!this.IsDropDown)
                {

                    ToolBarItem parentToolBarItem = VisualUtils.FindSomeParent(this, typeof(ToolBarItem)) as ToolBarItem;
                    if (parentToolBarItem != null)
                    {
                        parentToolBarItem.IsPressed = false;
                        parentToolBarItem.IsOpen = false;
                        Keyboard.Focus(parentToolBarItem);
                      
                        e.Handled = true;
                        return;
                    }       
                }
            }
            ////Right key works fine for all items by default. When the items are present inside a popup, the below code is required to move focus to the next item
            if (e.Key == Key.Right)
            {
                if (!this.IsDropDown)
                {
                        ToolBarItem parentToolBarItem = VisualUtils.FindSomeParent(this, typeof(ToolBarItem)) as ToolBarItem;
                        if (parentToolBarItem != null)
                        {
                            parentToolBarItem.IsPressed = false;
                            parentToolBarItem.IsOpen = false;
                            Keyboard.Focus(parentToolBarItem);
                            MoveFocusToNextItem();
                            e.Handled = true;
                            return;
                        }                    
                }
            }

            ////Left key works fine for all items by default. When the items are present inside a popup, the below code is required to move focus to the previous item
            if (e.Key == Key.Left)
            {
                if (!this.IsDropDown)
                {
                    ToolBarItem parentToolBarItem = VisualUtils.FindSomeParent(this, typeof(ToolBarItem)) as ToolBarItem;
                    if (parentToolBarItem != null)
                    {
                        parentToolBarItem.IsPressed = false;
                        parentToolBarItem.IsOpen = false;
                        Keyboard.Focus(parentToolBarItem);
                        MoveFocusToPreviousItem();
                        e.Handled = true;
                        return;
                    }
                }
            }

            base.OnKeyDown(e);                    
        }             

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Keyboard.LostKeyboardFocus"/>�attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.KeyboardFocusChangedEventArgs"/> that contains event data.</param>
        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);          
        }
        #endregion

        #region ICommand Interface Memembers
        /// <summary>
        /// Represents the Command dependency property.
        /// </summary>
        public new static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ToolBarItem));

        /// <summary>
        /// Represents the Commandtarget dependency property.
        /// </summary>
        public new static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(ToolBarItem), new PropertyMetadata((IInputElement)null));

        /// <summary>
        /// Gets or sets the CommandParameter dependency property.
        /// </summary>
        public new static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ToolBarItem), new PropertyMetadata((object)null));

        /// <summary>
        /// Gets or sets the command that will be executed when the command source is invoked.
        /// </summary>
        /// <value></value>
        public new ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the object that the command is being executed on.
        /// </summary>
        /// <value></value>
        public new IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)GetValue(CommandTargetProperty);
            }

            set
            {
                SetValue(CommandTargetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a user defined data value that can be passed to the command when it is executed.
        /// </summary>
        /// <value></value>
        /// <returns>The command specific data.</returns>
        public new object CommandParameter
        {
            get
            {
                return (object)GetValue(CommandParameterProperty);
            }

            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }

        #endregion
    }
    #endregion

    
}
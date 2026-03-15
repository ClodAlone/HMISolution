// <copyright file="ChartAreaContextMenu.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Shared;
    using System.Windows.Media.Imaging;


    /// <summary>
    /// Delegate for ChartContextMenuEventHandler  
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ChartContextMenuEventHandler(object sender, ChartContextMenuEventArgs e);


    /// <summary>
    /// Represents context menu that is shown when mouse right-clicks on <see cref="ChartArea"/>
    /// </summary>
    /// <exclude/>
    /// <seealso cref="ChartAreaContextMenu"/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAreaContextMenu : ContextMenu, IDisposable
    {

        /// <summary>
        /// Event for ContextMenu Opening
        /// </summary>
        public event ChartContextMenuEventHandler Opening;


        internal void OnOpening(object sender, ChartContextMenuEventArgs args)
        {
            if (Opening != null)
            {
                Opening(sender, args);

                if (args.Handle == false)
                {
                    this.Items.Add(args.CurrentMenuItem);
                }
                return;
            }

            this.Items.Add(args.CurrentMenuItem);
        }

        #region Members
       
        /// <summary>
        /// The ResourceDictionary with ContextMenu strings for localization
        /// </summary>
        private ResourceDictionary resourceDictionaryContextMenu;
        private ResourceDictionary rd;
        private ChartResourceWrapper resourceWrapper;

        /// <summary>
        /// Initializes m_zoomSeriesMenu
        /// </summary>
        private MenuItem m_zoomSeriesMenu;

        /// <summary>
        /// Initializes m_seriesMenu
        /// </summary>
        private MenuItem m_seriesMenu;

        /// <summary>
        /// Initializes m_palettesMenu
        /// </summary>
        private MenuItem m_palettesMenu;

        private MenuItem m_printMenu;

        private MenuItem m_styleMenu;

        /// <summary>
        /// Initializes m_AreaAnnotation
        /// </summary>
        private MenuItem m_AreaAnnotation;

        #endregion

        #region Properties
        /// <summary>
        /// Gets the area that context menu should be shown on.
        /// </summary>
        /// <value>The <see cref="ChartArea"/>.</value>
        private ChartArea Area
        {
            get
            {
                return this.PlacementTarget as ChartArea;
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes static members of the <see cref="ChartAreaContextMenu"/> class.
        /// </summary>
        static ChartAreaContextMenu()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChartAreaContextMenu), new FrameworkPropertyMetadata(typeof(ChartAreaContextMenu)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartAreaContextMenu"/> class.
        /// </summary>
        public ChartAreaContextMenu()
        {
            try
            {
                ////ResourceDictionary file for localizing strings
                resourceDictionaryContextMenu = ChartDictionaries.LangDictionary;
                //resourceDictionaryContextMenu = new SharedResourceDictionary()
                //{
                //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/LangDictionary.xaml", UriKind.RelativeOrAbsolute)
                //};

                resourceWrapper = new ChartResourceWrapper();
            }
            catch
            {
            }
            //    Initialize();

        }




        #endregion

        #region Implementation

        /// <summary>
        /// Called when the <see cref="E:System.Windows.Controls.ContextMenu.Opened"></see> event occurs.
        /// </summary>
        /// <param name="e">The event data for the <see cref="E:System.Windows.Controls.ContextMenu.Opened"></see> event.</param>
        protected override void OnOpened(RoutedEventArgs e)
        {
            Initialize();
            if (this.Area.View3DMode)
            {
                m_zoomSeriesMenu.IsEnabled = false;
            }
            else
            {
                m_zoomSeriesMenu.IsEnabled = true;
            }

            SetBinding(FlowDirectionProperty, new Binding("FlowDirection") { Source = Area });
            m_zoomSeriesMenu.CommandTarget = this.Area;
            this.Items.Clear();

            switch (this.Area.ContextMenuType)
            {
                case ContextMenuTypes.Default:
                    {

                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_zoomSeriesMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_seriesMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_palettesMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_printMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_styleMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_AreaAnnotation });

                        this.PrepareZoomSeriesMenu();
                        this.PreparePaletteMenu();
                        this.PrepareSeriesMenu();
                        this.PreparePrintMenu();
                        this.PrepareStyleMenu();
                        this.PrepareAnnotaitonMenu();
                        
                    }

                    break;
                case ContextMenuTypes.Custom:
                    {
                        if (Area != null)
                        {
                            if (Area.CustomContextMenuItems != null)
                            {
                                foreach (MenuItem item in Area.CustomContextMenuItems)
                                {
                                    this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = item });
                                }
                            }
                        }
                    }
                    break;
                case ContextMenuTypes.DefaultWithCustom:
                    {
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_zoomSeriesMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_seriesMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_palettesMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_printMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_styleMenu });
                        this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = m_AreaAnnotation });

                        if (Area != null)
                        {
                            if (Area.CustomContextMenuItems != null)
                            {
                                foreach (MenuItem item in Area.CustomContextMenuItems)
                                {
                                    this.OnOpening(this, new ChartContextMenuEventArgs { CurrentMenuItem = item });
                                }

                            }
                        }

                        this.PrepareZoomSeriesMenu();
                        this.PreparePaletteMenu();
                        this.PrepareSeriesMenu();
                        this.PreparePrintMenu();
                        this.PrepareStyleMenu();
                        this.PrepareAnnotaitonMenu();
                    }

                    break;
            }

            //ResourceDictionary rd = ChartDictionaries.ChartAreaContextMenuDictionary;
            ResourceDictionary rd = new SharedResourceDictionary()
            {
                Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.Chart.Templates.xaml", UriKind.RelativeOrAbsolute)
            };            
            if (rd != null && this.Area != null)
            {
                Chart chart = this.Area.Parent == null && this.Area.IsSync == true ? this.Area.ChartAreaParent.Parent as Chart : this.Area.Parent as Chart;
                //string skin1 = SkinStorage.GetVisualStyle(this.Area);
                if (chart != null && (chart as Chart).ChartVisualStyle == ChartStyles.Blend)
                {
                    this.Style = rd["BlendChartAreaContextMenuStyle"] as Style;
                }
                else if (chart != null && (chart).ChartVisualStyle == ChartStyles.Metro)
                {
                    this.Style = rd["MetroThemeChartAreaContextMenuStyle"] as Style;
                }
                else
                {
                    this.Style = rd["DefaultChartAreaContextMenuStyle"] as Style;
                }
            }


            base.OnOpened(e);
        }

        /// <summary>
        /// Called when a context menu's visual parent changes.
        /// </summary>
        /// <param name="oldParent">The object that the context menu was previously attached to.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            //   ClearValue(SkinStorage.VisualStylesListProperty);
            BindingOperations.ClearBinding(this, SkinStorage.VisualStyleProperty);
            if (Area != null)
            {
                Binding visualStyleBinding = new Binding() { Source = Area };
                visualStyleBinding.Path = new PropertyPath(SkinStorage.VisualStyleProperty);
                SetBinding(SkinStorage.VisualStyleProperty, visualStyleBinding);
            }
            base.OnVisualParentChanged(oldParent);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            if (resourceDictionaryContextMenu != null)
            {
                if (m_zoomSeriesMenu == null)
                {
                    Image actionsIcon = new Image();
                    actionsIcon.Source = ChartResources.Actions;

                    m_zoomSeriesMenu = new MenuItem();
                    string zoomString = resourceWrapper.ContextMenuZooming; //resourceDictionaryContextMenu["contextMenuZooming"] as string;
                    m_zoomSeriesMenu.Header = zoomString;
                    m_zoomSeriesMenu.Icon = actionsIcon;
                    m_zoomSeriesMenu.Command = ChartAreaCommands.SwitchZooming;
                }

                if (m_seriesMenu == null)
                {
                    Image seriesIcon = new Image();
                    seriesIcon.Source = ChartResources.Series;

                    m_seriesMenu = new MenuItem();
                    string seriesString = resourceWrapper.ContextMenuSeries; //resourceDictionaryContextMenu["contextMenuSeries"] as string;
                    m_seriesMenu.Header = seriesString;
                    m_seriesMenu.Icon = seriesIcon;
                }

                if (m_palettesMenu == null)
                {
                    Image palettesIcon = new Image();
                    palettesIcon.Source = ChartResources.Palettes;

                    m_palettesMenu = new MenuItem();
                    m_palettesMenu.Icon = palettesIcon;
                    string palettesString = resourceWrapper.ContextMenuPalettes; // resourceDictionaryContextMenu["contextMenuPalettes"] as string;
                    m_palettesMenu.Header = palettesString;
                }

                if (m_printMenu == null)
                {
                    Image printIcon = new Image();
                    printIcon.Source = ChartResources.ToolBarItemPrint;
                    m_printMenu = new MenuItem();
                    m_printMenu.Icon = printIcon;
                    string printString = resourceWrapper.PrintDialogPrint; //resourceDictionaryContextMenu["printDialogPrint"] as string;
                    m_printMenu.Header = printString;
                }

                if (m_styleMenu == null)
                {
                    Image styleIcon = new Image();
                    styleIcon.Source = ChartResources.Palettes;
                    m_styleMenu = new MenuItem();
                    m_styleMenu.Icon = styleIcon;
                    string styleString = resourceWrapper.Style; //resourceDictionaryContextMenu["style"] as string;
                    m_styleMenu.Header = styleString;
                }
				//Sets the properties for the menuItem m_AreaAnnotation
                if (m_AreaAnnotation == null)
                {
                    Image annotImage = new Image();
                    annotImage.Source = ChartResources.annotation;
                    m_AreaAnnotation = new MenuItem();
                    m_AreaAnnotation.Icon = annotImage;
                    m_AreaAnnotation.Header = "Add Annotation";
                }

                //    this.Items.Clear();
                //    this.Items.Add(m_zoomSeriesMenu);
                //    this.Items.Add(m_seriesMenu);
                //    this.Items.Add(m_palettesMenu);
                //    this.Items.Add(m_printMenu);
                //    this.Items.Add(m_styleMenu);

                //    if (Area != null)
                //    {
                //        if (Area.CustomContextMenuItems != null)
                //        {
                //            foreach (MenuItem item in Area.CustomContextMenuItems)
                //            {
                //                this.Opening(this, new ChartContextMenuEventArgs { CurrentMenuItem = item });

                //                this.Items.Add(item);
                //            }
                //        }
                //    }
            }

            ////this.Items.Add(m_zoomSeriesMenu);
            ////this.Items.Add(m_seriesMenu);
            ////this.Items.Add(m_palettesMenu);    
        }

        /// <summary>
        /// Gets the menu item by specified <see cref="ChartSeries"/>.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <returns>Returns the MenuItem</returns>
        private MenuItem GetMenuItemBy(ChartSeries series)
        {
            MenuItem item = new MenuItem();
            Ellipse ellipse = new Ellipse();

            ellipse.Width = 16;
            ellipse.Height = 16;
            ellipse.Stroke = Brushes.Black;

            bool colorEach = (series.ColorEach == null ? false : (bool)series.ColorEach);

            if (colorEach && series.ColorEachDependent)
                ellipse.Fill = series.GetColorEachImageBrush();
            else
                ellipse.Fill = series.Interior;

            item.Tag = series;
            item.Header = series.Label;
            item.Icon = ellipse;

            string[] types = Enum.GetNames(typeof(ChartTypes));
            foreach (string name in types)
            {
                MenuItem typeItem = new MenuItem();
                ChartTypes type = (ChartTypes)Enum.Parse(typeof(ChartTypes), name);
                typeItem.Header = type.ToFriendlyString();
                typeItem.Tag = ChartSeries.KnownType(type);
                typeItem.Click += new RoutedEventHandler(OnTypeItemClick);

                typeItem.IsChecked = typeItem.Tag.GetType() == series.ChartType.GetType();

                item.Items.Add(typeItem);
            }

            return item;
        }

        private void PreparePrintMenu()
        {
            m_printMenu.Command = ChartCommands.SwitchPrinting;
        }

        private void PrepareStyleMenu()
        {
            //m_styleMenu.Items.Clear();
            //Array styles = Enum.GetValues(typeof(ChartStyles));

            //foreach (ChartStyles style in styles)
            //{
            //    MenuItem item = new MenuItem();
            //    item.Header = style.ToString();
            //    item.Command = ChartAreaCommands.ChangeStyle;
            //    item.CommandParameter = style;
            //    item.CommandTarget = this.Area;
            //    m_styleMenu.Items.Add(item);
            //}

            m_styleMenu.Items.Clear();
            Array styles = Enum.GetValues(typeof(ChartStyles));

            StackPanel mainPanel = new StackPanel();
            mainPanel.Orientation = Orientation.Vertical;
            StackPanel subPanel = new StackPanel();

            for (int i = 0; i < 48; i++)
            {
                if (i % 8 == 0)
                {
                    subPanel = new StackPanel();
                    subPanel.Orientation = Orientation.Horizontal;
                    mainPanel.Children.Add(subPanel);
                }


                MenuItem item = new MenuItem();
                item.Margin = new Thickness(5);
                item.Width = 55;
                item.Height = 30;
                if ((i < 48))
                {
                    string style = (i + 1).ToString() + ".png";
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri("pack://application:,,,/Syncfusion.Chart.Wpf;component//Resources/Style" + style, UriKind.RelativeOrAbsolute));
                    item.Icon = image;

                }


                item.ToolTip = ((Syncfusion.Windows.Chart.ChartStyles[])(styles))[i].ToString();

                item.Command = ChartAreaCommands.ChangeStyle;
                item.CommandParameter = ((Syncfusion.Windows.Chart.ChartStyles[])(styles))[i];
                item.CommandTarget = this.Area;


                subPanel.Children.Add(item);


            }

            MenuItem item1 = new MenuItem();
            item1.Header = mainPanel;
            m_styleMenu.Items.Add(item1);

        }


        /// <summary>
        /// Prepares the series menu.
        /// </summary>
        private void PrepareSeriesMenu()
        {
            m_seriesMenu.Items.Clear();

            SyncChartAreas syncChartArea = this.Area.ChartAreaParent;
            if (syncChartArea != null)
            {
                if (syncChartArea.IsSyncChartArea == true)
                {
                    foreach (ChartArea chartArea in syncChartArea.Areas)
                    {
                        foreach (ChartSeries ser in chartArea.Series)
                        {
                            m_seriesMenu.Items.Add(this.GetMenuItemBy(ser));
                        }
                    }
                }
            }
            else
            {
                if (this.Area != null)
                {
                    foreach (ChartSeries ser in this.Area.Series)
                    {
                        m_seriesMenu.Items.Add(this.GetMenuItemBy(ser));
                    }
                }
            }
        }

        private void PrepareAnnotaitonMenu()
        {            
			//Sets the Command to the MenuItem
            m_AreaAnnotation.Command = ChartCommands.AddAnnotation;
        }
        /// <summary>
        /// Prepares zoom series menu.
        /// </summary>
        private void PrepareZoomSeriesMenu()
        {
            m_zoomSeriesMenu.Items.Clear();
            SyncChartAreas syncChartArea = this.Area.ChartAreaParent;

            if (syncChartArea != null)
            {
                if (syncChartArea.IsSyncChartArea == true)
                {
                    SetValue(SyncChartAreas.ZoomSwitchedProperty, true);
                    Binding binding = new Binding();
                    MenuItem seriesSelectingMenuItem = new MenuItem();
                    seriesSelectingMenuItem.IsCheckable = true;
                    ////Binding IsChecked property on MenuItem to ZoomAllAxes property on Area.
                    binding.Source = Area;
                    binding.Path = new PropertyPath(ChartArea.ZoomAllAxesProperty);
                    binding.Mode = BindingMode.TwoWay;
                    binding.Converter = new ZoomableToCheckedConverter();
                    binding.ConverterParameter = Area;
                    BindingOperations.SetBinding(seriesSelectingMenuItem, MenuItem.IsCheckedProperty, binding);
                    string zoomAllString = resourceWrapper.ContextMenuZoomAll; //resourceDictionaryContextMenu["contextMenuZoomAll"] as string;
                    seriesSelectingMenuItem.Header = zoomAllString;
                    seriesSelectingMenuItem.Command = ChartAreaCommands.SwitchZooming;
                    seriesSelectingMenuItem.CommandTarget = this.Area;
                    ////Adding "Zoom all" MenuItem to submenu of context menu.
                    m_zoomSeriesMenu.Items.Add(seriesSelectingMenuItem);
                    m_zoomSeriesMenu.Items.Add(new Separator());
                    foreach (ChartArea chartArea in syncChartArea.Areas)
                    {
                        foreach (ChartSeries chartSeries in chartArea.Series)
                        {
                            prepareZoomSeriesBasedOnChartArea(seriesSelectingMenuItem, binding, chartSeries, chartArea);
                        }
                    }
                }
            }
            else
            {
                if (this.Area != null)
                {
                    Binding binding = new Binding();
                    MenuItem seriesSelectingMenuItem = new MenuItem();
                    seriesSelectingMenuItem.IsCheckable = true;
                    ////Binding IsChecked property on MenuItem to ZoomAllAxes property on Area.
                    binding.Source = Area;
                    binding.Path = new PropertyPath(ChartArea.ZoomAllAxesProperty);
                    binding.Mode = BindingMode.TwoWay;
                    binding.Converter = new ZoomableToCheckedConverter();
                    binding.ConverterParameter = Area;
                    BindingOperations.SetBinding(seriesSelectingMenuItem, MenuItem.IsCheckedProperty, binding);
                    string zoomAllString = resourceWrapper.ContextMenuZoomAll; //resourceDictionaryContextMenu["contextMenuZoomAll"] as string;
                    seriesSelectingMenuItem.Header = zoomAllString;
                    seriesSelectingMenuItem.Command = ChartAreaCommands.SwitchZooming;
                    seriesSelectingMenuItem.CommandTarget = this.Area;
                    ////Adding "Zoom all" MenuItem to submenu of context menu.
                    m_zoomSeriesMenu.Items.Add(seriesSelectingMenuItem);
                    m_zoomSeriesMenu.Items.Add(new Separator());

                    foreach (ChartSeries chartSeries in this.Area.Series)
                    {
                        prepareZoomSeriesBasedOnChartArea(seriesSelectingMenuItem, binding, chartSeries, this.Area);
                    }
                }
            }

            //resourceDictionaryContextMenu = null;
        }
        private void prepareZoomSeriesBasedOnChartArea(MenuItem seriesSelectingMenuItem, Binding binding, ChartSeries chartSeries, ChartArea chartArea)
        {
            seriesSelectingMenuItem = new MenuItem();
            seriesSelectingMenuItem.IsCheckable = true;
            ////Binding IsChecked property on MenuItem to IsZooming property on Series.
            binding = new Binding();
            binding.Source = chartSeries;
            binding.Path = new PropertyPath(ChartSeries.IsZoomableProperty);
            binding.Mode = BindingMode.TwoWay;
            binding.Converter = new ZoomableToCheckedConverter();
            binding.ConverterParameter = chartSeries;
            BindingOperations.SetBinding(seriesSelectingMenuItem, MenuItem.IsCheckedProperty, binding);
            ////Adding ellipse and textblock with series name to stack panel
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 16;
            ellipse.Height = 16;
            ellipse.Stroke = Brushes.Black;
            ellipse.Fill = chartSeries.Interior;
            ellipse.Margin = new Thickness(0, 0, 3, 0);
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.Children.Add(ellipse);
            TextBlock seriesName = new TextBlock();
            seriesName.Text = chartSeries.Label;
            stackPanel.Children.Add(seriesName);
            ////Setting stack panel as child of submenu item..
            seriesSelectingMenuItem.Header = stackPanel;
            seriesSelectingMenuItem.Tag = chartSeries;
            seriesSelectingMenuItem.Checked += new RoutedEventHandler(SeriesItem_Checked);
            ////Assigning a comand.
            seriesSelectingMenuItem.Command = ChartAreaCommands.SwitchZooming;
            seriesSelectingMenuItem.CommandTarget = chartArea;
            m_zoomSeriesMenu.Items.Add(seriesSelectingMenuItem);
        }

        /// <summary>
        /// Raises when the seriesItem is checked
        /// </summary>
        /// <param name="sender">The object sender</param>
        /// <param name="e">The RoutedEventArgs e</param>
        /// <remarks></remarks>
        private void SeriesItem_Checked(object sender, RoutedEventArgs e)
        {
            ////Setting ZoomAllAxes to false to prevent unexpected context menu behavior.
            if (this.Area != null)
            {
                SyncChartAreas syncChartArea = this.Area.ChartAreaParent;
                if (syncChartArea != null)
                {
                    if (syncChartArea.IsSyncChartArea == true)
                    {
                        foreach (ChartArea chartArea in syncChartArea.Areas)
                        {
                            chartArea.ZoomAllAxes = false;
                        }
                    }
                }
                else
                    this.Area.ZoomAllAxes = false;
            }
        }

        /// <summary>
        /// Prepares the palette menu.
        /// </summary>
        private void PreparePaletteMenu()
        {
            m_palettesMenu.Items.Clear();

            if (this.Area != null)
            {
                Array palettes = Enum.GetValues(typeof(ChartColorPalette));

                foreach (ChartColorPalette palette in palettes)
                {
                    MenuItem item = new MenuItem();
                    item.Header = palette.ToFriendlyString();
                    item.Icon = this.Area.ColorModel.GetIcon(16, 16, palette);
                    item.Command = ChartAreaCommands.ChangePalette;
                    item.CommandParameter = palette;
                    item.CommandTarget = this.Area;

                    m_palettesMenu.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Called when type item is click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnTypeItemClick(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (item != null)
            {
                MenuItem serItem = item.Parent as MenuItem;
                ChartSeries series = serItem.Tag as ChartSeries;

                series.ChartType = (ChartType)item.Tag;
                item.IsChecked = true;
            }
        }
        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
			//Removes the memory alloted
            m_palettesMenu = null;
            m_seriesMenu = null;
            m_zoomSeriesMenu = null;
            m_AreaAnnotation = null;
            this.resourceWrapper = null;
            if (this.resourceDictionaryContextMenu != null)
            {
                this.resourceDictionaryContextMenu.MergedDictionaries.Clear();
                this.resourceDictionaryContextMenu.Clear();
                this.resourceDictionaryContextMenu = null;
            }
            if (this.rd != null)
            {
                this.rd.MergedDictionaries.Clear();
                this.rd.Clear();
                this.rd = null;
            }
        }

        #endregion
    }

    


    /// <summary>
    /// ChartContextMenuEventArgs class implematation
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartContextMenuEventArgs
    {
        MenuItem m_item = null;

        /// <summary>
        /// Get and Set HandleProperty
        /// </summary>
        public bool Handle { get; set; }


        /// <summary>
        /// Get and Set Header Property
        /// </summary>
        public object Header
        {
            get
            {
                return CurrentMenuItem.Header;
            }
        }
        /// <summary>
        /// Get and Sey CurrentMenuItem
        /// </summary>
        public MenuItem CurrentMenuItem
        {
            get { return m_item; }
            internal set { m_item = value; }
        }
    }
}

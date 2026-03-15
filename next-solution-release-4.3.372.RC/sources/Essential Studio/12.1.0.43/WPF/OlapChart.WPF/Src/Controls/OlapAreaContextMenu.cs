#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Syncfusion.Windows.Chart;
    using System.ComponentModel;
    /// <summary>
    /// Represents Olap area context menu.
    /// </summary>
    /// 
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public class OlapAreaContextMenu : ContextMenu
    {
        #region Members
        private MenuItem m_chartTypesMenu;
        private MenuItem m_palettesMenu;
        private MenuItem m_zoomSeriesMenu;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapAreaContextMenu"/> class.
        /// </summary>
        /// <param name="area">The <see cref="OlapArea"/>.</param>
        public OlapAreaContextMenu(OlapArea area)
            : base()
        {
            //Making 3 main menus.
            m_zoomSeriesMenu = new MenuItem()
            {
                Icon = new Image() { Source = ChartResources.Actions },
                Header = "Zooming",
                Command = ChartAreaCommands.SwitchZooming,
                CommandTarget = area
            };
            m_chartTypesMenu = new MenuItem() { Icon = new Image() { Source = ChartResources.Series }, Header = "Series" };
            m_palettesMenu = new MenuItem() { Icon = new Image() { Source = ChartResources.Palettes }, Header = "Palettes" };

            //Preparing palettes submenu.
            foreach (ChartColorPalette palette in Enum.GetValues(typeof(ChartColorPalette)))
            {
                m_palettesMenu.Items.Add(new MenuItem()
                {
                    Header = palette.ToString(),
                    Icon = area.ColorModel.GetIcon(16, 16, palette),
                    Command = ChartAreaCommands.ChangePalette,
                    CommandTarget = area,
                    CommandParameter = palette
                });
            }
        
            //Setting types submenu as binding to OlapArea.ChartType via the converter that returns menuItems.
            Binding typesBinding = new Binding("ChartType");
            typesBinding.Source = typesBinding.ConverterParameter = area;
            typesBinding.Converter = new AreaTypeToItemsCollectionConverter();
            //BindingOperations.SetBinding(m_chartTypesMenu, MenuItem.ItemsSourceProperty, typesBinding);
            m_chartTypesMenu.SetBinding(MenuItem.ItemsSourceProperty, typesBinding);
            //Building context menu.
            Items.Add(m_zoomSeriesMenu);
            Items.Add(m_chartTypesMenu);
            Items.Add(m_palettesMenu);
        }
        #endregion
    }

    /// <summary>
    /// Representing AreaTypeToItemsCollectionConverter
    /// </summary>
    class AreaTypeToItemsCollectionConverter : IValueConverter
    {
        #region Constructor
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OlapArea area = parameter as OlapArea;
            List<MenuItem> menuItems = new List<MenuItem>();
            if (area != null)
            {
                MenuItem typeSubMenuItem;
                foreach (ChartTypes type in Enum.GetValues(typeof(ChartTypes)))
                {
                    typeSubMenuItem = new MenuItem() { Header = type.ToString(), IsChecked = area.ChartType == type, Tag = type };
                    typeSubMenuItem.Click += (object sender, RoutedEventArgs e) =>
                      {
                          area.ChartType = (ChartTypes)(sender as MenuItem).Tag;
                      };
                    menuItems.Add(typeSubMenuItem);
                }
            }
            return menuItems;
        }
        #endregion

        #region Event
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Conversion back is not supported");
        }
        #endregion

        #region IValueConverter Members

        #endregion
    }
}

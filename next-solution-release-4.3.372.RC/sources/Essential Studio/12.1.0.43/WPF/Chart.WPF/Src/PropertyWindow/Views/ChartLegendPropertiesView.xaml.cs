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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Interaction logic for ChartLegendPropertiesView.xaml
    /// </summary>
    public partial class ChartLegendPropertiesView : UserControl
    {
        /// <summary>
        /// Called when instance created for ChartLegendPropertiesview class
        /// </summary>
        public ChartLegendPropertiesView()
        {
            InitializeComponent();
            ChartLegend legend = new ChartLegend();
            DockPosition.ItemsSource = Enum.GetValues(typeof(ChartDock));            
            this.CheckBoxVisibility.ItemsSource = Enum.GetValues(typeof(Visibility));
            this.IconVisibility.ItemsSource = Enum.GetValues(typeof(Visibility));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if ((this.DataContext as Chart).Legends.Count <= 0)
            {
                this.IsEnabled = false;
            }
            this.LegendNo.SelectedIndex = 0;
            if (LegendNo.SelectedItem != null)
            {
                this.DockPosition.SelectedItem = Chart.GetDock((LegendNo.SelectedItem as ChartLegend));
            }
        }

        private void DockPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Chart.SetDock(((sender as ComboBox).DataContext as ChartLegend), (ChartDock)(sender as ComboBox).SelectedItem);
        }
    }

    /// <summary>
    /// Return the modified ChartLegend collection from the given values
    /// </summary>
    public class ChartLegendNameConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(ChartLegendsCollection))
            {
                ChartLegendsCollection EleCollection = value as ChartLegendsCollection;
                foreach (ChartLegend ele in EleCollection)
                {
                    if (ele.Name == null || ele.Name == string.Empty)
                    {
                        ele.Name = "ChartLegend" + ((EleCollection as ObservableCollection<ChartLegend>).IndexOf(ele) + 1);
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}

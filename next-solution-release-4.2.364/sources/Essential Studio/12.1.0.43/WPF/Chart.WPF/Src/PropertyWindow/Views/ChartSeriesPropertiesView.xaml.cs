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
    /// Interaction logic for ChartSeriesPropertiesView.xaml
    /// </summary>
    public partial class ChartSeriesPropertiesView : UserControl
    {
        /// <summary>
        /// Called when instance created for ChartSeriesPropertiesView
        /// </summary>
        public ChartSeriesPropertiesView()
        {
            InitializeComponent();
            //seriesNo.SelectedIndex = 0;
            TypeCombo.ItemsSource = Enum.GetValues(typeof(ChartTypes));
            LegendIconCombo.ItemsSource = Enum.GetValues(typeof(ChartLegendIcon));
            EmptyPointStyleCombo.ItemsSource = Enum.GetValues(typeof(EmptyPointStyle));
            AnimationOptionsCombo.ItemsSource = Enum.GetValues(typeof(AnimationOptions));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null)
            {
                this.IsEnabled = false;
                return;
            }
            if ((this.DataContext as ChartArea).Series.Count <= 0)
            {
                this.IsEnabled = false;
            }
            else { this.IsEnabled = true; }
            this.seriesNo.SelectedIndex = 0;
        }
    }

    /// <summary>
    /// Return the modified ChartSeriesCollection from the given value
    /// </summary>
    public class ChartSeriesNameConverter : IValueConverter
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
            if (value.GetType() == typeof(ChartSeriesCollection))
            {
                ChartSeriesCollection EleCollection = value as ChartSeriesCollection;
                foreach (ChartSeries ele in EleCollection)
                {
                    if (ele.Name == null || ele.Name == string.Empty)
                    {
                        ele.Name = "ChartSeries" + ((EleCollection as ObservableCollection<ChartSeries>).IndexOf(ele) + 1);
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

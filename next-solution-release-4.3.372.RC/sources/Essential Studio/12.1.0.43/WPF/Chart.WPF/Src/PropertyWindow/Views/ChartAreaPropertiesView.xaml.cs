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
    /// Interaction logic for ChartAreaPropertiesView.xaml
    /// </summary>
    public partial class ChartAreaPropertiesView : UserControl
    {
        /// <summary>
        /// Called when Instance created for ChartAreaPropertiesView
        /// </summary>
        public ChartAreaPropertiesView()
        {
            InitializeComponent();
            //this.ChartAreasList.SelectedIndex = 0;
            this.CheckBoxVisibility.ItemsSource = Enum.GetValues(typeof(Visibility));
            this.IconVisibility.ItemsSource = Enum.GetValues(typeof(Visibility));
            this.AlternatingFillDirection.ItemsSource = Enum.GetValues(typeof(Orientation));
            this.AlternatingFillMode.ItemsSource = Enum.GetValues(typeof(AlternatingFillMode));
            this.DockPosition.ItemsSource = Enum.GetValues(typeof(ChartDock));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null)
            { 
                this.IsEnabled = false; 
                return; 
            }
            if ((this.DataContext as Chart).Areas.Count <= 0)
            {
                this.IsEnabled = false;
                
            }
            this.ChartAreasList.SelectedIndex = 0;
            if(ChartAreasList.SelectedItem != null && (ChartAreasList.SelectedItem as ChartArea).Legend != null)
                this.DockPosition.SelectedItem = Chart.GetDock((ChartAreasList.SelectedItem as ChartArea).Legend);
        }

        private void DockPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Chart.SetDock(((sender as ComboBox).DataContext as ChartLegend), (ChartDock)(sender as ComboBox).SelectedItem);
        }       
    }

    /// <summary>
    /// Return bool value based on the given object is legend or not. 
    /// </summary>
    public class LegendEnableConverter : IValueConverter
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
            if (value is ChartLegend)
            {
                if ((value as ChartLegend) != null)
                {
                    return true;
                }                
            }
            return false;
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
    /// <summary>
    /// Return the modified  ChartAreasCollection 
    /// </summary>
    public class ChartControlsNameConverter : IValueConverter
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
            if (value.GetType() == typeof(ChartAreasCollection))
            {
                ChartAreasCollection EleCollection = value as ChartAreasCollection;
                foreach (ChartArea ele in EleCollection)
                {
                    if (ele.Name == null || ele.Name == string.Empty)
                    {
                        ele.Name = "ChartArea" + ((EleCollection as ObservableCollection<ChartArea>).IndexOf(ele) + 1);
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

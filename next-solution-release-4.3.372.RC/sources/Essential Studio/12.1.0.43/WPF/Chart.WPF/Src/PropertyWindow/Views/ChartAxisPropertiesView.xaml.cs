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
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Chart
{
    /// <summary>
    /// Interaction logic for ChartAxisPropertiesView.xaml
    /// </summary>
    public partial class ChartAxisPropertiesView : UserControl
    {
        /// <summary>
        /// Constructor implementation for ChartAxisPropertiesView
        /// </summary>
        public ChartAxisPropertiesView()
        {
            InitializeComponent();
            //AxisNo.SelectedIndex = 0;
            ValueTypeComboBox.ItemsSource = Enum.GetValues(typeof(ChartValueType));
            OrientationCombo.ItemsSource = Enum.GetValues(typeof(Orientation));
            HeaderAlignmentCombo.ItemsSource = Enum.GetValues(typeof(ChartAlignment));
            RangeCalculationModeCombo.ItemsSource = Enum.GetValues(typeof(RangeCalculationMode));
            ChartRangePaddingTypeCombo.ItemsSource = Enum.GetValues(typeof(ChartRangePaddingType));
            ChartLabelIntersectAction.ItemsSource = Enum.GetValues(typeof(ChartLabelIntersectAction));
            ChartAxisLabelsModeCombo.ItemsSource = Enum.GetValues(typeof(ChartAxisLabelsMode));
            EdgeLabelDrawingModeCombo.ItemsSource = Enum.GetValues(typeof(EdgeLabelsDrawingMode));
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext == null)
            {
                this.IsEnabled = false;
                return;
            }
            if ((this.DataContext as ChartArea).Axes.Count <= 0)
            {
                this.IsEnabled = false;
            }
            else { this.IsEnabled = true; }
            this.AxisNo.SelectedIndex = 0;
            if(AxisNo.SelectedItem != null)
                this.RangeCalculationModeCombo.SelectedItem = (AxisNo.SelectedItem as ChartAxis).RangeCalculationMode;
        }
      
    }

    /// <summary>
    /// Returns bool value based on the valuetype
    /// </summary>
    public class AxisTypeToEnabledConverter : IValueConverter
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
            if(value != null)
            {
                ChartValueType valuetype = (ChartValueType)value ;
                if (parameter.ToString() == "Lograthimic" && valuetype == ChartValueType.Logarithmic) 
                {
                    return true;
                }
                else if (parameter.ToString() == "DateTime" && valuetype == ChartValueType.DateTime)
                {
                    return true;
                }
                else if (parameter.ToString() == "Double" && valuetype == ChartValueType.Double)
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
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Return modified ChartAxisName in ChartAxesCollection
    /// </summary>
    public class ChartAxisNameConverter : IValueConverter
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
            if (value.GetType() == typeof(ChartAxesCollection))
            {
                ChartAxesCollection EleCollection = value as ChartAxesCollection;
                foreach (ChartAxis ele in EleCollection)
                {
                    if (ele.Name == null || ele.Name == string.Empty)
                    {
                        ele.Name = "ChartAxis" + ((EleCollection as ObservableCollection<ChartAxis>).IndexOf(ele) + 1);
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

    /// <summary>
    /// Return pen value from the given Brush value
    /// </summary>
    public class BrushToPenConverter : IValueConverter
    {
        #region IValueConverter Members
        static double Thickness;

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value != null && value.GetType() == typeof(Pen))
            {
                Thickness = (value as Pen).Thickness;
                return (value as Pen).Brush;
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
            if(value != null)
            {
                return new Pen(value as Brush, Thickness);
            }
            return value;
        }

        #endregion
    }


}

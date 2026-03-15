// <copyright file="ChartArea.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Chart
{
    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Effects;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using Syncfusion.Licensing;
    using System.Linq;
    using Syncfusion.Windows.Shared;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Text;
    /// <summary>
    /// Represents Axes State To Margin Converter.
    /// </summary>  
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class AxesStateToMarginConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// If the method returns null, the valid null value is used.
        /// A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.
        /// A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <seealso cref="AxesStateToMarginConverter"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Thickness axesThickness = (Thickness)values[0];
            if (axesThickness != null)
            {
                double value1 = 0.0;
                if (values[1] != DependencyProperty.UnsetValue)
                {
                    value1 = (double)values[1];
                }

                double value2 = 0.0;
                if (values[2] != DependencyProperty.UnsetValue)
                {
                    value2 = (double)values[2];
                }

                return new Thickness(axesThickness.Left, axesThickness.Top, axesThickness.Right + value1, axesThickness.Bottom + value2);
            }

            return new Thickness(0, 0, 0, 0);
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <seealso cref="AxesStateToMarginConverter"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Conversion back for area's margin is not supported");
        }
        #endregion
    }

    /// <summary>
    ///Return Thickness value fron the given value
    /// </summary>
    public class AxesLeftMarginConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() == typeof(Thickness))
            {
                return new Thickness(((Thickness)value).Left, 0, ((Thickness)value).Right, 0);
            }

            return null;

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
    }

    /// <summary>
    /// return value for panel implementation
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PanelConverter : IValueConverter
    {

        #region IValueConverter Members


        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that to the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        /// <seealso cref="PanelConverter"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null)
            {
                if ((string) parameter == "SyncChartAreas")
                {
                    var template = value as ItemsPanelTemplate;

                }
            }

            return value;
        }

        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding target to the binding source.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that to the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        /// <seealso cref="PanelConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Return the bool value
    /// </summary>
    public class ScrollBarVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
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
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Represents IsZoomable series to checked conntext menu items converters.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class ZoomableToCheckedConverter : IValueConverter
    {
        #region IValueConverter Members

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
        /// <seealso cref="ZoomableToCheckedConverter"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is ChartSeries)
            {
                ChartSeries chartSeries = (ChartSeries)parameter;
                return (bool)value && chartSeries.Area.ZoomSwitched && !chartSeries.Area.ZoomAllAxes;
            }

            if (parameter is ChartArea)
            {
                SyncChartAreas syncChartArea = ((ChartArea)parameter).ChartAreaParent;
                if (syncChartArea != null)
                {
                    if (syncChartArea.IsSyncChartArea == true)
                    {
                        return (bool)value && syncChartArea.Areas[0].ZoomSwitched; ;
                    }
                }
                else
                    return (bool)value && ((ChartArea)parameter).ZoomSwitched;
            }

            throw new NotImplementedException();
        }

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
        /// <seealso cref="ZoomableToCheckedConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    /// <summary>
    /// Return String value from the given Annotation Template objects.
    /// </summary>
    public class AnnotationConverter : IMultiValueConverter
    {

        #region IMultiValueConverter Members

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="values">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <seealso cref="AnnotationConverter"/>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string stringvalue = values[0] as string;
            ChartAnnotationLabel label = values[1] as ChartAnnotationLabel;
            if (stringvalue != null)
            {
                TextBlock text = new TextBlock() { Text = stringvalue, DataContext = label };
                return text;
            }
                return values[0];
            
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetTypes">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <seealso cref="AnnotationConverter"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Return thickness value from the given multiple objects.
    /// </summary>
    public class MarginMulConverter : IMultiValueConverter
    {

        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != null && values[1] != null)
            {
                double v1 = (double)values[0];
                double v2 = (double)values[1];
                Thickness t = new Thickness();
                if (v1 == 0 && v2 == 0)
                {
                    t = new Thickness(-(5), -(5), 0, 0);
                }
                else
                {
                    t = new Thickness(-(v1 / 2), -(v2 / 2), 0, 0);
                }
                return t;
            }
            else
            {
                return values;
            }
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <param name="value">The value that the binding target produces.</param><param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    /// <summary>
    /// Represents chart control paths converter.
    /// </summary>
    /// <exclude/>
    public class ChartPathsConverter : TypeConverter
    {
        #region Constants
        /// <summary>
        /// Initializes c_pathGroup
        /// </summary>
        private const string C_pathGroup = "path";

        /// <summary>
        /// Initializes c_regex
        /// </summary>
        private const string C_regex = @"[\[](?<path>.+)?[\]]|(?<path>[^, ]+)";
        #endregion

        #region Implementation
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                List<string> paths = new List<string>();
                MatchCollection matchPaths = Regex.Matches(value as string, C_regex);

                foreach (Match match in matchPaths)
                {
                    if (match.Success)
                    {
                        paths.Add(match.Groups[C_pathGroup].Value);
                    }
                }

                return paths.ToArray();
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to. </param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string[])) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed. </param><param name="value">The <see cref="T:System.Object"/> to convert. </param><param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to. </param><exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception><exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType() == typeof(string[]))
            {
                string _stringValue = string.Empty;
                string[] _arrayValue = value as string[];
                foreach (string s in _arrayValue)
                {
                    _stringValue += (s + ",");
                }
                _stringValue = _stringValue.Remove(_stringValue.Length - 1);
                return _stringValue;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion
    }


    /// <summary>
    /// Represents color converter class.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ColorModelToPaletteConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        internal int value = -1;
        /// <summary>
        /// The Convert method
        /// </summary>
        /// <param name="values">The object values</param>
        /// <param name="targetType">The targetType</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="ColorModelToPaletteConverter"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartSeries series = values[1] as ChartSeries;
            Brush[] brushes = values[0] as Brush[];
            if (brushes != null && series != null && series.Area != null && series.Area.Series.IndexOf(series) >= 0)
            {
                //if ()
                //{
                return brushes[series.Area.Series.IndexOf(series) % brushes.Length];
                //}
                //else
                //{
                //    value++;
                //   return brushes[value%brushes.Length];
                //}
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The ConvertBack method
        /// </summary>
        /// <param name="value">The object value</param>
        /// <param name="targetTypes">The targetTypes</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="ColorModelToPaletteConverter"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Convert back on palette multibinding is not supported");
        }

        #endregion
    }

    /// <summary>
    /// Converts datasource, x, y, multivalues to IChartData.
    /// </summary>
    /// <exclude/>
    internal class ChartDataMultibindingConverter : IMultiValueConverter
    {
        #region Members
        /// <summary>
        /// Initializes m_providedValue
        /// </summary>
        private ChartBindingData _mProvidedValue = new ChartBindingData();
        #endregion

        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IEnumerable source = values[0] as IEnumerable;

            if (source == null)
            {
                return DependencyProperty.UnsetValue;
            }
            else
            {
                if (_mProvidedValue != null)
                {
                    _mProvidedValue.Source = null;
                }

                if (_mProvidedValue != null)
                {
                    _mProvidedValue.Dispose();

                }
                return _mProvidedValue = new ChartBindingData();
            }
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        internal void Dispose()
        {
            if (this._mProvidedValue != null)
            {
                this._mProvidedValue.Dispose();
            }
        }

        #endregion
    }
    /// <summary>
    /// Convert x,y values to thickness
    /// </summary>
    public class ColumnMarginConverter : IMultiValueConverter
    {
        
            #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            {
                if (!(values[0] is double)) return null;
                if (!(values[1] is double)) return null;

               double left = (double)values[0];
               double top = (double)values[1];
               return new Thickness(left,top,0,0);
            }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <param name="value">The value that the binding target produces.</param><param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
            {
                throw new NotImplementedException();
            }

            #endregion
       

    }
    /// <summary>
    /// return Thickness  value from the given value.
    /// </summary>
    public class MarginConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChartAxis axis = value as ChartAxis;
            if (axis != null)
            {
                if (axis.Orientation == Orientation.Horizontal)
                {
                    return "Left";
                }
                else
                {
                    return "Bottom";
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    //public class LableConverter : IValueConverter
    //{

    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        ChartAxis axis = value as ChartAxis;
    //        if (axis != null)
    //        {
    //            return axis.InteractiveCursorTemplate;
    //        }
    //        return value;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion


    //}

    internal class ColorSwitcher : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="values">The value that the binding target produces.</param>
        /// <param name="targetType">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <seealso cref="ColorSwitcher"/>
        object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values[0];
        }


        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <seealso cref="ColorSwitcher"/>
        object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Represents TemplateSwitcher
    /// </summary>
    internal class TemplateSwitcher : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// If the method returns null, the valid null value is used.
        /// A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.
        /// A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <seealso cref="TemplateSwitcher"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Control control = values[0] as Control;
            DataTemplate template = values[1] as DataTemplate;
            ////If content is control that provides its own template Chart.LabelTemplate should not
            ////be considered and ContentControl.ContentTemplate should be left unset.
            if (control != null && control.Template != null)
            {
                return DependencyProperty.UnsetValue;
            }
            ////Otherwise we provide Chart.AnnotationLabelTemplate as a template for Content of ContentControl.
            return template;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <seealso cref="TemplateSwitcher"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Multibinding template selection cannot be converted back");
        }

        #endregion

        #region IMultiValueConverter Members

        //object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}

        //object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }

    /// <summary>
    /// Return AxesCollection from the given  Object
    /// </summary>
    public class AxesConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length == 3)
            {
                bool isreversedAxis = (bool)values[1];
                IEnumerable axesCollection = values[0] as IEnumerable;
                if (axesCollection == null)
                {
                    return null;
                }

                return isreversedAxis ? axesCollection.OfType<ChartAxis>().Reverse<ChartAxis>() : axesCollection.OfType<ChartAxis>();
            }

            return null;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <param name="value">The value that the binding target produces.</param><param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    /// <summary>
    /// Represents AdornmentContentConverter
    /// </summary>
    public class AdornmentContentConverter : IMultiValueConverter
    {
        #region Members
        /// <summary>
        /// Initializes m_cachedSeries
        /// </summary>
        public ChartSeries m_cachedSeries;

        /// <summary>
        /// Initializes m_sumValues
        /// </summary>
        public double m_sumValues;
        #endregion

        #region IMultiValueConverter Members
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.
        /// If the method returns null, the valid null value is used.
        /// A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.
        /// A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <seealso cref="AdornmentContentConverter"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object initialValue = values[0];
            LabelContent labelContent = (LabelContent)values[1];
            ChartSegment correspondingSegment = values[2] as ChartSegment;
            string labelFormat = values[3] as string;
            string labelDateTimeFormat = values[4] as string;
            ChartAdornment adornment = values[2] as ChartAdornment;
            bool isLabelContentPathSet = adornment.Series.AdornmentsInfo.m_isLabelContentPathSet;

            if (correspondingSegment != null && correspondingSegment.Series != m_cachedSeries &&
              (labelContent == LabelContent.YofTot || labelContent == LabelContent.Percentage))
            {
                m_cachedSeries = correspondingSegment.Series;
                for (int i = 0; i < m_cachedSeries.PointsCount; i++)
                {
                    if (!m_cachedSeries.GetPoint(i).EmptyPoint)
                        m_sumValues += m_cachedSeries.GetPoint(i).Y;
                }
            }

            IChartDataPoint correspondingPoint = correspondingSegment.CorrespondingPoints[0].DataPoint;
            object retValue;

            switch (labelContent)
            {
                case LabelContent.Percentage:
                    retValue = (correspondingPoint.Y / m_sumValues * 100).ToString(labelFormat, CultureInfo.CurrentCulture) + "%";
                    break;
                case LabelContent.XValue:
                    retValue = correspondingPoint.X.ToString(labelFormat, CultureInfo.CurrentCulture);
                    break;
                case LabelContent.YValue:
                    retValue = correspondingPoint.Y.ToString(labelFormat, CultureInfo.CurrentCulture);
                    break;
                case LabelContent.YofTot:
                    retValue = correspondingPoint.Y.ToString(labelFormat, CultureInfo.CurrentCulture) + " of " + m_sumValues.ToString(labelFormat, CultureInfo.CurrentCulture);
                    break;
                case LabelContent.DateTime:
                    retValue = DateTime.FromOADate(correspondingPoint.X).ToString(labelDateTimeFormat, CultureInfo.CurrentCulture);
                    break;
                default:
                    retValue = (!isLabelContentPathSet && adornment.m_labelValue != null) ? adornment.m_labelValue : initialValue;
                    break;
            }

            return retValue;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <seealso cref="AdornmentContentConverter"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Conversion back is not supported for adornment content multibinding expression");
        }
        #endregion
    }


    /// <summary>
    /// Return the double value for AnimationEffect
    /// </summary>
     #if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
    #endif
    public class AnimationEnableEffectConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="values">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            Double myvalues = (Double)values;
            myvalues = myvalues - 5;
            return myvalues;

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
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Use to determine the Effects layer path based on the Series type.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class EffectPathConverter : IValueConverter
    {
        /// <summary>
        /// Choose the external layer path based on the chart type.
        /// </summary>
        /// <param name="values">Binding value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">Converter parameter</param>
        /// <param name="culture">culture information</param>
        /// <returns>object value</returns>     
        /// <seealso cref="EffectPathConverter"/>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values != null)
            {
                ChartSegment segments = values as ChartSegment;
                if (segments != null)
                {
                    foreach (var item in segments.CorrespondingPoints)
                    {
                        if (item.DataPoint.EmptyPoint)
                        {
                            if (segments.Series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior && segments.Series.ShowEmptyPoints == true)
                            {
                                DataTemplate temp = new DataTemplate();
                                ResourceDictionary rd = ChartDictionaries.GenericSeriesGUIDictionary;
                                //ResourceDictionary rd = new SharedResourceDictionary()
                                //{
                                //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                                //};
                                return rd["EmptyPointSymbolAndInterior"] as DataTemplate;
                            }
                            else if (segments.Series.EmptyPointStyle == EmptyPointStyle.Symbol && segments.Series.ShowEmptyPoints == true)
                            {
                                DataTemplate temp = new DataTemplate();
                                ResourceDictionary rd = ChartDictionaries.GenericSeriesGUIDictionary;
                                //ResourceDictionary rd = new SharedResourceDictionary()
                                //{
                                //    Source = new Uri("/Syncfusion.Chart.Wpf;component/Themes/Generic.SeriesGUI.xaml", UriKind.RelativeOrAbsolute)
                                //};
                                return rd["EmptyPointInterior"] as DataTemplate;
                            }

                        }
                        else
                        {
                            ChartTypes types = segments.Series.Type;
                            if (types == ChartTypes.Bubble || types == ChartTypes.Line || types == ChartTypes.Area || types == ChartTypes.Scatter)
                            {
                                if (segments.Series.EmptyPointStyle == EmptyPointStyle.SymbolAndInterior)
                                {
                                    return new DataTemplate();
                                }
                            }
                            else
                            {
                                if (types == ChartTypes.Bar || types == ChartTypes.StackingBar || types == ChartTypes.StackingBar100 || types == ChartTypes.Gantt || types == ChartTypes.Tornado)
                                {
                                    return ChartDataUtils.GetResourceByString("BarEffectsPath");
                                }
                                else
                                {
                                    return ChartDataUtils.GetResourceByString("ColumnEffectsPath");
                                }
                            }
                        }
                    }

                }
                else
                    return null;

            }

            return new DataTemplate();
        }

        /// <summary>
        /// Convertback to Normal value.
        /// </summary>
        /// <param name="value">value binding</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">converter  parameter</param>
        /// <param name="culture">culture information</param>
        /// <returns>object value</returns>
        /// <seealso cref="EffectPathConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }

    /// <summary>
    /// Return visibility  value based on the given value
    /// </summary>
    public class Visibilityselector : IValueConverter
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
            if (value != null)
            {
                ChartSeries series = value as ChartSeries;
                if (series != null)
                {
                    if (series.EnableEffects)
                    {
                        return Visibility.Visible;
                    }
                    else
                    {
                        return Visibility.Visible;
                    }
                }
            }
            return null;
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
    /// Return Double value.
    /// </summary>
    public class DebugErrorConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            var myvalues = (Double)value;
            if (double.IsNaN(myvalues))
            {
                myvalues = 0;
            }
            return myvalues;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType,
            object parameter, System.Globalization.CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }



    /// <summary>
    /// Use to determine the Visibility of Chart Series Line segement effects
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class LineEffectsConverter : IMultiValueConverter
    {
        /// <summary>
        /// Convert the bool type to Visibility and determine the last line segment visibility
        /// </summary>
        /// <param name="values">Binding value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">Converter parameter</param>
        /// <param name="culture">culture information</param>
        /// <returns>object value</returns>
        /// <seealso cref="LineEffectsConverter"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartSegment segment = values[0] as ChartSegment;
            if (segment != null && segment.Series != null)
            {
                int index = segment.Series.Segments.Count - 1;
                return (index >= 0 && segment.Equals(segment.Series.Segments[index]) == true && (bool)values[1]) ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (values[0] == null)
            {
                return Visibility.Collapsed;
            }

            return (values[1] is bool && (bool)values[1]) == true ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Convertback to Normal value.
        /// </summary>
        /// <param name="value">value binding</param>
        /// <param name="targetTypes">target type</param>
        /// <param name="parameter">converter  parameter</param>
        /// <param name="culture">culture information</param>
        /// <returns>object value</returns>
        /// <seealso cref="LineEffectsConverter"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Use to determine the Visibility of Chart Series External Effects layer
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class EnableEffectsConverter : IValueConverter
    {
        /// <summary>
        /// Convert the bool type to Visibility
        /// </summary>
        /// <param name="values">Binding value</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">Converter parameter</param>
        /// <param name="culture">culture information</param>
        /// <returns>object value</returns>
        /// <seealso cref="EnableEffectsConverter"/>
        public object Convert(object values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ChartSegment segment = values as ChartSegment;
            bool effect;
            if (segment != null)
            {
                if (segment.CorrespondingPoints[0].DataPoint.EmptyPoint && (segment.Series.Type == ChartTypes.Line || segment.Series.Type == ChartTypes.Column || segment.Series.Type == ChartTypes.Bubble || segment.Series.Type == ChartTypes.Scatter || segment.Series.Type == ChartTypes.Bar || segment.Series.Type == ChartTypes.StackingColumn || segment.Series.Type == ChartTypes.StackingColumn100 || segment.Series.Type == ChartTypes.StackingBar || segment.Series.Type == ChartTypes.StackingBar100))
                {
                    effect = false;
                }
                else
                    effect = segment.Series.EnableEffects;
                return ((bool)effect) == true ? Visibility.Visible : Visibility.Collapsed;
            }
            return null;

        }

        /// <summary>
        /// Convertback to Normal value.
        /// </summary>
        /// <param name="value">value binding</param>
        /// <param name="targetType">target type</param>
        /// <param name="parameter">converter  parameter</param>
        /// <param name="culture">culture information</param>     
        /// <returns>object value</returns>
        /// <seealso cref="EnableEffectsConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException();
        }
    }


    /// <summary>
    /// Converts tick placement position according to the axis orientation
    /// and opposed position properties.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartAxisTypeConverter :
      IMultiValueConverter
    {
        #region IMultiValueConverter Members
        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <seealso cref="ChartAxisTypeConverter"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TickBarPlacement pl = TickBarPlacement.Top;
            if (values[0] is Orientation && values[1] is bool)
            {
                string par = parameter as string;
                Orientation orientation = (Orientation)values[0];
                bool opposedPosition = (bool)values[1];

                switch (par)
                {
                    case "TickBar":
                        {
                            if (orientation == Orientation.Vertical && opposedPosition)
                            {
                                pl = TickBarPlacement.Right;
                            }
                            else if (orientation == Orientation.Vertical && !opposedPosition)
                            {
                                pl = TickBarPlacement.Left;
                            }
                            else if (orientation == Orientation.Horizontal && !opposedPosition)
                            {
                                pl = TickBarPlacement.Bottom;
                            }
                            else if (orientation == Orientation.Horizontal && opposedPosition)
                            {
                                pl = TickBarPlacement.Top;
                            }

                            return pl;
                        }

                    case "Title":
                        {
                            if (orientation == Orientation.Vertical && opposedPosition)
                            {
                                return 90d;
                            }
                            else if (orientation == Orientation.Vertical && !opposedPosition)
                            {
                                return 270d;
                            }

                            break;
                        }
                }
            }

            return 0d;
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <seealso cref="ChartAxisTypeConverter"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
    /// <summary>
    /// Return Thickness value from given double value.
    /// </summary>
    public class LabelMarginConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is ChartAxis)
            {
                if ((value as ChartAxis).Orientation == Orientation.Horizontal)
                {
                    return new Thickness(0, 3, 0, 3);
                }
                else
                {
                    return new Thickness(3, 0, 3, 0);
                }
            }
            return new Thickness(3);
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// Return double value from the given value.
    /// </summary>
    public class AxisMarginConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness mythickness = new Thickness(0, 0, 0, 0);
            if (value != null)
            {
                ChartAxis axis = value as ChartAxis;
                if (axis != null)
                {
                    var newPadding = axis.Margin;

                    if (axis.Orientation == (Orientation.Horizontal))
                    {
                        newPadding.Left = 0;
                        newPadding.Right = 0;
                    }
                    else if (axis.Orientation == Orientation.Vertical)
                    {
                        newPadding.Top = 0;
                        newPadding.Bottom = 0;
                    }
                    return newPadding;
                }
                else
                    if (axis != null)
                        return axis.Margin;
                return 0;
            }
            else
                return mythickness;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    /// <summary>
    /// Represents DoubleArrayConverter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DoubleArrayConverter : TypeConverter
    {
        #region Members
        /// <summary>
        /// Initializes m_doubleCollectionConverter
        /// </summary>
        private DoubleCollectionConverter m_doubleCollectionConverter = new DoubleCollectionConverter();
        #endregion

        #region Public methods
        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"></see> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return m_doubleCollectionConverter.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return m_doubleCollectionConverter.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result = m_doubleCollectionConverter.ConvertFrom(context, culture, value);

            if (result is DoubleCollection)
            {
                DoubleCollection doubleCollection = result as DoubleCollection;
                double[] doubleArray = new double[doubleCollection.Count];

                doubleCollection.CopyTo(doubleArray, 0);

                return doubleArray;
            }

            return result;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"></see>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        /// <exception cref="T:System.ArgumentNullException">The destinationType parameter is null. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            DoubleCollection doubleCollection = new DoubleCollection(value as double[]);

            return m_doubleCollectionConverter.ConvertTo(context, culture, doubleCollection, destinationType);
        }
        #endregion
    }

    /// <summary>
    /// Represents Chart List data Converter class.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ChartListDataConverter : TypeConverter
    {
        #region Constants
        /// <summary>
        /// Declares Constant C_xGroup
        /// </summary>
        private const string C_xGroup = "x";

        /// <summary>
        /// Declares Constant C_yGroup
        /// </summary>
        private const string C_yGroup = "y";

        /// <summary>
        /// Declares Constant C_regexSplitter
        /// </summary>
        private const string C_regexSplitter = "[, ]+";

        /// <summary>
        /// Declares Constant C_regex
        /// </summary>
        private const string C_regex = "(?<x>[^ ,]+)([ ]*[ ,][ ]*)(([{](?<y>[^}]+)[}])|(?<y>[^ ,]+))";
        #endregion

        #region Implementation
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"></see> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <seealso cref="ChartListDataConverter"/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to. </param>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(ChartListData) || base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"></see> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        ///  <seealso cref="ChartListDataConverter"/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string sData = value as string;

            if (sData != null)
            {
                ChartListData points = new ChartListData();
                MatchCollection mathes = Regex.Matches(sData, C_regex);

                foreach (Match match in mathes)
                {
                    if (match.Success)
                    {
                        double x = Convert.ToDouble(match.Groups[C_xGroup].Value, CultureInfo.InvariantCulture);
                        string[] yStrings = Regex.Split(match.Groups[C_yGroup].Value, C_regexSplitter);
                        double[] yValues = new double[yStrings.Length];

                        for (int i = 0; i < yValues.Length; i++)
                        {
                            yValues[i] = Convert.ToDouble(yStrings[i], CultureInfo.InvariantCulture);
                        }

                        points.AddPoint(x, yValues);
                    }
                }

                return points;
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context. </param><param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed. </param><param name="value">The <see cref="T:System.Object"/> to convert. </param><param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to. </param><exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception><exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value.GetType() == typeof(ChartListData))
            {
                ChartListData points = value as ChartListData;
                string sData = string.Empty;
                if (points != null)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        if (i != 0)
                        {
                            sData += ",";
                        }
                        points.ToString();
                        sData += (points[i].X + ",{" + ConvertData(points[i].Values) + "}");
                    }
                }
                return sData;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        #endregion

        #region Helper

        /// <summary>
        /// Method implementation for convert double values to string values
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ConvertData(double[] value)
        {
            StringBuilder _returnString = new StringBuilder();
            foreach (double _val in value)
            {
                _returnString.Append(_val);
                _returnString.Append(",");
            }
            _returnString.Remove(_returnString.Length - 1, 1);
            return _returnString.ToString();
        }

        #endregion
    }


    /// <summary>
    /// return bool value 
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]    
#endif
    public class DisplayUnitConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if(value!=null)
            {
                if(value.ToString() == "AutoDetect")
                {
                    return Visibility.Collapsed;
                }
                else
                    return Visibility.Visible;
                //ChartAxis myvalue = value as ChartAxis;
                //if (myvalue.EnableSmartAxisLabel == true)
                //{
                //    return Visibility.Visible;
                //}
                //else
                //    return Visibility.Hidden;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    /// <summary>
    /// return the object value
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DisplayUnitsConverter : IValueConverter
    {

        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value != null)
            {
                switch (value.ToString())
                {
                    case "None":
                        return "";
                    case "TenThousands":
                        return "Ten Thousands";
                    case "HundredThousands":
                        return "Hundred Thousands";
                    case "TenMillions":
                        return "Ten Millions";
                    case "HundredMillions":
                        return "Hundred Millions";
                    default:
                        return value;
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    /// <summary>
    /// Converter class used to convert combination of horizontal and vertical alignment to
    /// <see cref="ChartAlignment"/> value.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TitleToAlignmentConverter : IValueConverter
    {
        #region Implementation
        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding source to the binding target.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        /// <seealso cref="TitleToAlignmentConverter"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChartAlignment alignment = (ChartAlignment)value;

            if (object.Equals(parameter, "Vertical"))
            {
                VerticalAlignment vAlignment = VerticalAlignment.Stretch;

                switch (alignment)
                {
                    case ChartAlignment.Near:
                        vAlignment = VerticalAlignment.Top;
                        break;
                    case ChartAlignment.Center:
                        vAlignment = VerticalAlignment.Center;
                        break;
                    case ChartAlignment.Far:
                        vAlignment = VerticalAlignment.Bottom;
                        break;
                }

                return vAlignment;
            }
            else if (object.Equals(parameter, "Horizontal"))
            {
                HorizontalAlignment hAlignment = HorizontalAlignment.Stretch;

                switch (alignment)
                {
                    case ChartAlignment.Near:
                        hAlignment = HorizontalAlignment.Left;
                        break;
                    case ChartAlignment.Center:
                        hAlignment = HorizontalAlignment.Center;
                        break;
                    case ChartAlignment.Far:
                        hAlignment = HorizontalAlignment.Right;
                        break;
                }

                return hAlignment;
            }

            return null;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.  
        /// </returns>
        /// <seealso cref="TitleToAlignmentConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }
        #endregion
    }


    /// <summary>
    /// Return Visibility value from the given value
    /// </summary>
     #if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
     #endif
    public class AxisVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding source to the binding target.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        /// <seealso cref="AxisVisibilityConverter"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ChartAxis axis = value as ChartAxis;
            if (axis != null && targetType == typeof(System.Boolean))
            {
                return axis.IsEnabled ? true : false;
            }
            if (axis != null)
            {
                return axis.AxisVisibility;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        /// <summary>
        /// Converts a value. The data binding engine calls this method when it propagates a value from the binding source to the binding target.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"></see>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"></see> indicates that the converter produced no value and that the binding uses the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see>, if available, or the default value instead.A return value of <see cref="T:System.Windows.Data.Binding"></see>.<see cref="F:System.Windows.Data.Binding.DoNothing"></see> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"></see> or default value.
        /// </returns>
        /// <seealso cref="AxisVisibilityConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }



    /// <summary>
    /// Represents ShowSymbolConvertor class.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal class ShowSymbolConvertor : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        /// <summary>
        /// The Convert method
        /// </summary>
        /// <param name="values">The object values</param>
        /// <param name="targetType">The targetType</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="ShowSymbolConvertor"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0].ToString() == "Visible" && values[1].ToString() == "Visible" && (bool)values[2])
            {
                ////if both ShowSymbol and IconVisibility are visible, then show the symbol in legend icon
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// The ConvertBack method
        /// </summary>
        /// <param name="value">The object value</param>
        /// <param name="targetTypes">The targetTypes</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="ShowSymbolConvertor"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    

    public class LegendIconInteriorConvertor : IMultiValueConverter
    {
        #region IMultiValueConverter Members
        /// <summary>
        /// The Convert method
        /// </summary>
        /// <param name="values">The object values</param>
        /// <param name="targetType">The targetType</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="ShowSymbolConvertor"/>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[1] != DependencyProperty.UnsetValue && values[2] is ChartSeries)
            {
                ChartSeries series = values[2] as ChartSeries;
                bool colorEach = (series.ColorEach == null ? false : (bool)series.ColorEach);

                if (colorEach && series.ColorEachDependent)
                    return series.GetColorEachImageBrush();
                else
                    return series.Interior;
            }
            else if (values[0] != DependencyProperty.UnsetValue)
                return values[0];
            return null;
        }

        /// <summary>
        /// The ConvertBack method
        /// </summary>
        /// <param name="value">The object value</param>
        /// <param name="targetTypes">The targetTypes</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="culture">The culture</param>
        /// <returns>Returns the value</returns>
        /// <seealso cref="ShowSymbolConvertor"/>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


    #region Converters
    /// <summary>
    /// Represents VisibilityConverter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

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
        /// <seealso cref="VisibilityConverter"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool visibility = (bool)value;
            if (parameter.ToString() == "Reverse")
            {
                if (visibility == true)
                {
                    ////Makes the DropDown arrow visible
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            else
            {
                if (visibility == false)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

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
        /// <seealso cref="VisibilityConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// Represents ContentToVisibilityConverter
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ContentToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

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
        /// <seealso cref="ContentToVisibilityConverter"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// <seealso cref="ContentToVisibilityConverter"/>
        /// </returns>
        /// <seealso cref="ContentToVisibilityConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    #endregion

    /// <summary>
    /// Represents chart area corner radius to thickness converter.
    /// </summary>
    /// <remarks>
    /// This class should be used in chart templates to prevent chart's corners intersection with area.
    /// </remarks>
    /// <exclude/>
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    public sealed class ChartAreaCornerRadiusToThicknessConverter : IValueConverter
    {
        #region IValueConverter Members
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
        /// <seealso cref="ChartAreaCornerRadiusToThicknessConverter"/>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CornerRadius radius = (CornerRadius)value;
            if (radius != null)
            {
                double coeff = 1 - Math.Cos(Math.PI / 4);
                return new Thickness(
                    ChartMath.Max(new double[] { radius.TopLeft, radius.BottomLeft }) * coeff,
                                     ChartMath.Max(new double[] { radius.TopLeft, radius.TopRight }) * coeff,
                                     ChartMath.Max(new double[] { radius.TopRight, radius.BottomRight }) * coeff,
                                     ChartMath.Max(new double[] { radius.BottomLeft, radius.BottomRight }) * coeff);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
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
        /// <seealso cref="ChartAreaCornerRadiusToThicknessConverter"/>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// Return Thickness value based on given object values.
    /// </summary>
    public sealed class ChartPaddingConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        /// <summary>
        /// Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.
        /// </summary>
        /// <returns>
        /// A converted value.If the method returns null, the valid null value is used.A return value of <see cref="T:System.Windows.DependencyProperty"/>.<see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding"/>.<see cref="F:System.Windows.Data.Binding.DoNothing"/> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue"/> or the default value.
        /// </returns>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding"/> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue"/> indicates that the source binding has no value to provide for conversion.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue)
            {
                Thickness radiusPadding;
                Thickness padding = (Thickness)values[0];
                CornerRadius radius = (CornerRadius)values[1];
                if (radius != null && padding != null)
                {
                    double coeff = 1 - Math.Cos(Math.PI / 4);
                    radiusPadding = new Thickness(
                        ChartMath.Max(new double[] { radius.TopLeft, radius.BottomLeft }) * coeff,
                                         ChartMath.Max(new double[] { radius.TopLeft, radius.TopRight }) * coeff,
                                         ChartMath.Max(new double[] { radius.TopRight, radius.BottomRight }) * coeff,
                                         ChartMath.Max(new double[] { radius.BottomLeft, radius.BottomRight }) * coeff);
                    padding.Bottom = Math.Max(padding.Bottom, radiusPadding.Bottom);
                    padding.Left = Math.Max(padding.Left, radiusPadding.Left);
                    padding.Right = Math.Max(padding.Right, radiusPadding.Right);
                    padding.Top = Math.Max(padding.Top, radiusPadding.Top);
                    return padding;
                }
                else
                {
                    return new Thickness(0);
                }
            }
            else
            {
                return new Thickness(0);
            }
        }

        /// <summary>
        /// Converts a binding target value to the source binding values.
        /// </summary>
        /// <returns>
        /// An array of values that have been converted from the target value back to the source values.
        /// </returns>
        /// <param name="value">The value that the binding target produces.</param><param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// Return content value from the given value.
    /// </summary>
    public class ContentConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            InteractiveCursorLabelContent content = value as InteractiveCursorLabelContent;
            if (content != null)
            {
                //if (content.DataPoint != null)
                if (content.X.GetType().Name == "DateTime")
                {
                    return content.X.ToString() + "," + Math.Round((double)content.Y).ToString();
                }
                return (content.X.GetType().Name == "String" ? content.X : Math.Round((double) content.X).ToString()) + "," + Math.Round((double) content.Y).ToString();
            }
            return 0;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    /// <summary>
    /// return the thickness value based on the given value
    /// </summary>
    public class ThicknessConverter : IValueConverter
    {
        #region ThicknessConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            if (value is Thickness)
            {
                Thickness thick = (Thickness)value;
                double value1 = thick.Bottom;
                return value1;
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
        public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    
}
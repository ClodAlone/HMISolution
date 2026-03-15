// <copyright file="NodeIdToTypeConverter.cs" company="$registerdorganization$">
// Copyright (c) 2010 Microsoft. All Right Reserved
// </copyright>
// <author>Claudio</author>
// <email></email>
// <date>2010-07-23</date>
// <summary>A value converter for WPF and Silverlight data binding</summary>

namespace CommandManager.UserControls.Converters
{
    using System;
    using System.Windows.Data;
    using Opc.Ua;
    using Opc.Ua.Helpers;
    using OPCUAViewModel;
    /// <summary>
    /// A Value converter
    /// </summary>
    public class ArgumentValueConverter : IValueConverter, IMultiValueConverter
    {
        Argument argument;
        NodeIdViewModel nodeidViewModel;

        #region IMultiValueConverter
        //
        // Summary:
        //     Converts source values to a value for the binding target. The data binding engine
        //     calls this method when it propagates the values from source bindings to the binding
        //     target.
        //
        // Parameters:
        //   values:
        //     The array of values that the source bindings in the System.Windows.Data.MultiBinding
        //     produces. The value System.Windows.DependencyProperty.UnsetValue indicates that
        //     the source binding has no value to provide for conversion.
        //
        //   targetType:
        //     The type of the binding target property.
        //
        //   parameter:
        //     The converter parameter to use.
        //
        //   culture:
        //     The culture to use in the converter.
        //
        // Returns:
        //     A converted value.If the method returns null, the valid null value is used.A
        //     return value of System.Windows.DependencyProperty.System.Windows.DependencyProperty.UnsetValue
        //     indicates that the converter did not produce a value, and that the binding will
        //     use the System.Windows.Data.BindingBase.FallbackValue if it is available, or
        //     else will use the default value.A return value of System.Windows.Data.Binding.System.Windows.Data.Binding.DoNothing
        //     indicates that the binding does not transfer the value or use the System.Windows.Data.BindingBase.FallbackValue
        //     or the default value.
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length == 0)
                return null;

            if (values.Length == 1)
                return Convert(values[0], targetType, null, culture);
            else
                return Convert(values[0], targetType, values[1], culture);
        }

        //
        // Summary:
        //     Converts a binding target value to the source binding values.
        //
        // Parameters:
        //   value:
        //     The value that the binding target produces.
        //
        //   targetTypes:
        //     The array of types to convert to. The array length indicates the number and types
        //     of values that are suggested for the method to return.
        //
        //   parameter:
        //     The converter parameter to use.
        //
        //   culture:
        //     The culture to use in the converter.
        //
        // Returns:
        //     An array of values that have been converted from the target value back to the
        //     source values.
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetTypes == null || targetTypes.Length == 0)
                return null;

            return new object[] { ConvertBack(value, targetTypes[0], nodeidViewModel, culture) };
        }
        #endregion

        #region IValueConverter Members

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI. 
        /// </summary>
        /// <param name="value">The source data being passed to the target </param>
        /// <param name="targetType">The Type of data expected by the target dependency property.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property. </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            nodeidViewModel = parameter as NodeIdViewModel;

            if (value is Argument && nodeidViewModel != null)
            {
                try
                {
                    argument = value as Argument;

                    INode datatype = nodeidViewModel.session.NodeCache.Find(argument.DataType);

                    BuiltInType builtinType = TypeInfo.GetBuiltInType(argument.DataType, nodeidViewModel.session.TypeTree);
                    var typeinfo = new TypeInfo(builtinType, argument.ValueRank);

                    uint arraySizeOneDimension = (argument.ArrayDimensions != null && argument.ArrayDimensions.Count > 0) ? argument.ArrayDimensions[0] : 0;

                    Variant variant = new Variant();
                    var v = ChangeTypeHelper.ChangeType(argument.Value, builtinType, arraySizeOneDimension); // prechange type base on currenthread localization first
                    if (arraySizeOneDimension == 0)
                        variant = new Variant(Opc.Ua.TypeInfo.Cast(v, builtinType));
                    else if (v is Array)
                        variant = new Variant(Opc.Ua.TypeInfo.CastArray((v as Array), builtinType, builtinType,
                            ChangeTypeHelper.CastArrayElement));

                    return variant.ToString();
                }
                catch
                {
                    return value;
                }
                // return TypeInfo.GetSystemType((value as Argument).DataType, EncodeableFactory.GlobalFactory).ToString();
            }

            return value;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings. 
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The Type of data expected by the source object.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic. </param>
        /// <param name="culture">The culture of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            nodeidViewModel = parameter as NodeIdViewModel;

            if (argument != null && nodeidViewModel != null)
            {
                INode datatype = nodeidViewModel.session.NodeCache.Find(argument.DataType);

                BuiltInType builtinType = TypeInfo.GetBuiltInType(argument.DataType, nodeidViewModel.session.TypeTree);

                try
                {
                    switch (builtinType)
                    {
                        case BuiltInType.Boolean:
                        case BuiltInType.Byte:
                        case BuiltInType.SByte:
                        case BuiltInType.Int16:
                        case BuiltInType.UInt16:
                        case BuiltInType.Int32:
                        case BuiltInType.UInt32:
                        case BuiltInType.Int64:
                        case BuiltInType.UInt64:
                        case BuiltInType.Float:
                        case BuiltInType.Double:
                        case BuiltInType.Enumeration:
                            argument.Value = System.Convert.ChangeType(value, TypeInfo.GetSystemType(builtinType, argument.ValueRank));
                            break;
                        case BuiltInType.Number:
                            argument.Value = System.Convert.ChangeType(value, TypeInfo.GetSystemType(BuiltInType.Double, argument.ValueRank));
                            break;
                        case BuiltInType.Integer:
                            argument.Value = System.Convert.ChangeType(value, TypeInfo.GetSystemType(BuiltInType.Int64, argument.ValueRank));
                            break;
                        case BuiltInType.UInteger:
                            argument.Value = System.Convert.ChangeType(value, TypeInfo.GetSystemType(BuiltInType.UInt64, argument.ValueRank));
                            break;
                        case BuiltInType.NodeId:
                            argument.Value = NodeId.Parse(value as String);
                            break;
                        case BuiltInType.ExpandedNodeId:
                            argument.Value = ExpandedNodeId.Parse(value as String);
                            break;
                        case BuiltInType.DateTime:
                            argument.Value = DateTime.Parse(value as String);
                            break;
                        case BuiltInType.QualifiedName:
                            argument.Value = QualifiedName.Parse(value as String);
                            break;
                        case BuiltInType.String:
                            argument.Value = value as String;
                            break;
                        case BuiltInType.LocalizedText:
                            argument.Value = new LocalizedText(value as String);
                            break;
                        default:
                            return argument;
                    }
                }
                catch
                { }
            }

            return argument;
        }

        #endregion
    }
}

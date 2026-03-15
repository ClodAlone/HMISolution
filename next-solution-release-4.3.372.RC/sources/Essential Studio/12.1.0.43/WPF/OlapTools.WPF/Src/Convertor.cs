#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Olap
{
    using System;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using Syncfusion.Olap.Data;

    [ValueConversion(typeof(MetaTreeNode), typeof(DrawingImage))]
    public class ImageConverter : IValueConverter
    {
        #region Private Constant Variables

        private const string c_Key = @"/Syncfusion.OlapTools.WPF;component/Themes/ToolsDictionary.xaml";

        #endregion

        #region Public Methods

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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MetaTreeNode mtNode = value as MetaTreeNode;
            if (mtNode != null)
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri(c_Key, UriKind.RelativeOrAbsolute);
                if (mtNode.NodeType == MetaTreeNodeType.CalculatedMember || mtNode.NodeType == MetaTreeNodeType.CalculatedMemberGroup)
                {
                    return resourceDictionary["CalcMember"] as DrawingImage;
                }

                if (mtNode.NodeType == MetaTreeNodeType.Cube)
                {
                    return resourceDictionary["Cube"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.Dimension)
                {
                    return resourceDictionary["Dimension"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.DisplayFolder)
                {
                    return resourceDictionary["FolderClose"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.Hierarchy)
                {
                    if (mtNode.ChildNodes.Count > 1)
                    {
                        return resourceDictionary["Hierarchy2"] as DrawingImage;
                    }
                    else
                    {
                        return resourceDictionary["Hierarchy1"] as DrawingImage;
                    }
                }
                else if (mtNode.NodeType == MetaTreeNodeType.NamedSet)
                {
                    return resourceDictionary["NamedSet"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.Level)
                {
                    switch (mtNode.LevelDepth)
                    {
                        case 1:
                            return resourceDictionary["Level1"] as DrawingImage;
                        case 2:
                            return resourceDictionary["Level2"] as DrawingImage;
                        case 3:
                            return resourceDictionary["Level3"] as DrawingImage;
                        case 4:
                            return resourceDictionary["Level4"] as DrawingImage;
                        case 5:
                            return resourceDictionary["Level5"] as DrawingImage;
                        case 6:
                            return resourceDictionary["Level6"] as DrawingImage;
                        case 7:
                            return resourceDictionary["Level7"] as DrawingImage;
                        case 8:
                            return resourceDictionary["Level8"] as DrawingImage;
                        case 9:
                            return resourceDictionary["Level9"] as DrawingImage;
                        case 10:
                            return resourceDictionary["Level10"] as DrawingImage;
                        case 11:
                            return resourceDictionary["Level11"] as DrawingImage;
                        case 12:
                            return resourceDictionary["Level12"] as DrawingImage;
                        case 13:
                            return resourceDictionary["Level13"] as DrawingImage;
                        case 14:
                            return resourceDictionary["Level14"] as DrawingImage;
                        case 15:
                            return resourceDictionary["Level15"] as DrawingImage;
                        case 16:
                            return resourceDictionary["Level16"] as DrawingImage;
                        default:
                            return resourceDictionary["Level1"] as DrawingImage;
                    }
                }
                else if (mtNode.NodeType == MetaTreeNodeType.Measure)
                {
                    return resourceDictionary["Measures"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.KPI_ROOT)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.KPI)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.KPI_Value)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.KPI_Goal)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.KPI_Status)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.KPI_Trend)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.MeasureGroup)
                {
                    return resourceDictionary["FolderClose"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.Member)
                {
                    return resourceDictionary["Member"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.VirtualKPIGroup)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.VirtualKPIMember)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.VirtualKPI_Value)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.VirtualKPI_Status)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.VirtualKPI_Goal)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.VirtualKPI_Trend)
                {
                    return resourceDictionary["KPI"] as DrawingImage;
                }
                else if (mtNode.NodeType == MetaTreeNodeType.None)
                {
                    return null;
                }

                return null;
            }

            return null;
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }

        #endregion

    }

    /// <summary>
    /// Converting the Width and Height of MetaTreeNode Elements
    /// </summary>
    public class WidthHeightConvertor : IValueConverter
    {
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MetaTreeNode mtNode = value as MetaTreeNode;
            if (mtNode != null)
            {
                if (mtNode.NodeType == MetaTreeNodeType.Level)
                {
                    if (mtNode.LevelDepth == 1)
                    {
                        return 4;
                    }
                    else if (mtNode.LevelDepth == 2)
                    {
                        return 9;
                    }
                    else if (mtNode.LevelDepth > 16 || mtNode.LevelDepth < 1)
                    {
                        return 4;
                    }
                }
            }
            return 16;
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region IValueConverter Members


        #endregion
    }

    /// <summary>
    /// Converting the Visibility to Boolean value.
    /// </summary>
    public class IntBoolConvertor : IValueConverter
    {
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int? integerValue = value as int?;
            if (integerValue != null)
            {
                if ((int)integerValue == 2)
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool? boolean = value as bool?;
            if (boolean != null)
            {
                if ((bool)boolean)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
            return 3;
        }

        #region IValueConverter Members


        #endregion
    }

    /// <summary>
    /// Converting the Boolean to Integer Value.
    /// </summary>
    public class NodeTypeVisibilityConvertor : IValueConverter
    {
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is MetaTreeNodeType)
            {
                MetaTreeNodeType nodeType = (MetaTreeNodeType)value;
                if (nodeType == MetaTreeNodeType.Measure ||
                    nodeType == MetaTreeNodeType.KPI_Goal ||
                    nodeType == MetaTreeNodeType.KPI_Status ||
                    nodeType == MetaTreeNodeType.KPI_Trend ||
                    nodeType == MetaTreeNodeType.KPI_Value)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #region IValueConverter Members


        #endregion
    }

    /// <summary>
    /// Converting the NodeType of namedset to Collapsible
    /// </summary>
    public class NamedSetVisibilityConvertor : IValueConverter
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
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is MetaTreeNodeType)
            {
                MetaTreeNodeType nodeType = (MetaTreeNodeType)value;
                if (nodeType == MetaTreeNodeType.NamedSet)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Visible;
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
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class TreeViewExpanderConvertor : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MetaTreeNode metaTreeNode = value as MetaTreeNode;
            if (metaTreeNode != null)
            {
                if (metaTreeNode.NodeCheckedType == MetaTreeNodeCheckedType.SomeChildChecked)
                    return true;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }



}
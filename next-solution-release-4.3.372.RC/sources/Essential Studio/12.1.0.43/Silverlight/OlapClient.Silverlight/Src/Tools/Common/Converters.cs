#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using Syncfusion.OlapSilverlight.Data;

namespace Syncfusion.Silverlight.Tools.Olap
{
    public class ImageConverter:IValueConverter
    {
        #region Private Constant Variables

        private const string resourceUri = @"/Syncfusion.OlapClient.Silverlight;component/Tools/CubeDimensionBrowser/ImageDictionary.xaml";

        #endregion

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MetaTreeNode treeNode = value as MetaTreeNode;
            if (treeNode != null)
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri(resourceUri, UriKind.RelativeOrAbsolute);
                
                if (treeNode.NodeType == MetaTreeNodeType.CalculatedMember || treeNode.NodeType == MetaTreeNodeType.CalculatedMemberGroup)
                {
                    return resourceDictionary["CalcMember"] as DataTemplate;
                }
                
                if (treeNode.NodeType == MetaTreeNodeType.Cube)
                {
                    return resourceDictionary["Cube"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.Dimension)
                {
                    return resourceDictionary["Dimension"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.DisplayFolder)
                {
                    return resourceDictionary["CloseFolder"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.Hierarchy)
                {
                    if (treeNode.ChildNodes.Count > 1)
                    {
                        return resourceDictionary["DHierarchy"] as DataTemplate;
                    }
                    else
                    {
                        return resourceDictionary["AHierarchy"] as DataTemplate;
                    }
                }
                else if (treeNode.NodeType == MetaTreeNodeType.NamedSet)
                {
                    return resourceDictionary["NamedSet"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.Level)
                {
                    switch (treeNode.LevelDepth)
                    {
                        case 1:
                            return resourceDictionary["Level_1"] as DataTemplate;
                        case 2:
                            return resourceDictionary["Level_2"] as DataTemplate;
                        case 3:
                            return resourceDictionary["Level_3"] as DataTemplate;
                        case 4:
                            return resourceDictionary["Level_4"] as DataTemplate;
                        case 5:
                            return resourceDictionary["Level_5"] as DataTemplate;
                        case 6:
                            return resourceDictionary["Level_6"] as DataTemplate;
                        case 7:
                            return resourceDictionary["Level_7"] as DataTemplate;
                        case 8:
                            return resourceDictionary["Level_8"] as DataTemplate;
                        case 9:
                            return resourceDictionary["Level_9"] as DataTemplate;
                        case 10:
                            return resourceDictionary["Level_10"] as DataTemplate;
                        case 11:
                            return resourceDictionary["Level_11"] as DataTemplate;
                        case 12:
                            return resourceDictionary["Level_12"] as DataTemplate;
                        case 13:
                            return resourceDictionary["Level_13"] as DataTemplate;
                        case 14:
                            return resourceDictionary["Level_14"] as DataTemplate;
                        case 15:
                            return resourceDictionary["Level_15"] as DataTemplate;
                        default:
                            return resourceDictionary["Level_1"] as DataTemplate;
                    }
                }
                else if (treeNode.NodeType == MetaTreeNodeType.Measure)
                {
                    return resourceDictionary["Measure"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_ROOT)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_Value)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_Goal)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_Status)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_Trend)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.MeasureGroup)
                {
                    return resourceDictionary["CloseFolder"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.Member)
                {
                    return resourceDictionary["Member"] as DataTemplate;
                }
                else if ((treeNode.NodeType == MetaTreeNodeType.VirtualKPIGroup) || (treeNode.NodeType == MetaTreeNodeType.VirtualKPIMember) || (treeNode.NodeType == MetaTreeNodeType.VirtualKPI_Goal) || (treeNode.NodeType == MetaTreeNodeType.VirtualKPI_Status) || (treeNode.NodeType == MetaTreeNodeType.VirtualKPI_Trend) || (treeNode.NodeType == MetaTreeNodeType.VirtualKPI_Value))
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.None)
                {
                    return null;
                }

                return null;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class MemberIcon : IValueConverter
    {
        #region IValueConverter Members

        #region Private Constant Variables

        private const string resourceUri = @"/Syncfusion.OlapClient.Silverlight;component/Tools/CubeDimensionBrowser/ImageDictionary.xaml";

        #endregion

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MetaTreeNode treeNode = value as MetaTreeNode;
            if (treeNode != null)
            {
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri(resourceUri, UriKind.RelativeOrAbsolute);

                if (treeNode.NodeType == MetaTreeNodeType.Member)
                {
                    return resourceDictionary["Member"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_ROOT)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_Value)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_Goal)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_Status)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
                else if (treeNode.NodeType == MetaTreeNodeType.KPI_Trend)
                {
                    return resourceDictionary["KPI"] as DataTemplate;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //throw new NotImplementedException();
            return null;
        }

        #endregion
    }
}

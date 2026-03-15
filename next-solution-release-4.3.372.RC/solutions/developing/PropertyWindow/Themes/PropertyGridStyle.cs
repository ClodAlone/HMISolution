using PropertyControl.ComponentService;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using Utilities.WPF;
using Mindscape.WpfElements.WpfPropertyGrid;

namespace PropertyControl
{
    public class GroupToExpandedConverter : IValueConverter
    {
        #region Declarations

        readonly static List<String> groupExpanded = new List<String>();

        internal static bool bAtLeastOneGroupExpanded = false;

        #endregion


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Boolean))
                throw new InvalidOperationException("The target must be a Boolean");

            var expander = value as Expander;
            if (expander == null)
                throw new InvalidOperationException("The value must be a Expander");

            bool bExpanded = NeedToBeExpanded(expander);
            if (bExpanded)
                bAtLeastOneGroupExpanded = true;

            return bExpanded;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region Methods
        public static void NotifyExpanded(Expander expander)
        {
            var groupName = GetComposedGroupName(expander);
            if (!String.IsNullOrEmpty(groupName))
            {
                if (Properties.Settings.Default.ShowPropertyGroupsExpanded)
                {
                    if (GroupToExpandedConverter.groupExpanded.Contains(groupName))
                        GroupToExpandedConverter.groupExpanded.Remove(groupName);
                }
                else
                {
                    if (!GroupToExpandedConverter.groupExpanded.Contains(groupName))
                        GroupToExpandedConverter.groupExpanded.Add(groupName);
                }
            }
        }

        public static void NotifyCollapsed(Expander expander)
        {
            var groupName = GetComposedGroupName(expander);
            if (!String.IsNullOrEmpty(groupName))
            {
                if (Properties.Settings.Default.ShowPropertyGroupsExpanded)
                {
                    if (!GroupToExpandedConverter.groupExpanded.Contains(groupName))
                        GroupToExpandedConverter.groupExpanded.Add(groupName);
                }
                else
                {
                    if (GroupToExpandedConverter.groupExpanded.Contains(groupName))
                        GroupToExpandedConverter.groupExpanded.Remove(groupName);
                }
            }
        }

        public static bool NeedToBeExpanded(Expander expander)
        {
            var groupName = GetComposedGroupName(expander);
            if (!String.IsNullOrEmpty(groupName))
            {
                if (Properties.Settings.Default.ShowPropertyGroupsExpanded)
                    return !groupExpanded.Contains(groupName);
                else
                    return groupExpanded.Contains(groupName);
            }
            else
                return Properties.Settings.Default.ShowPropertyGroupsExpanded;
        }

        static string GetComposedGroupName(Expander expander)
        {
            String groupName = GetGroupName(expander);
            if (!String.IsNullOrEmpty(groupName))
            {
                var parent = expander.FindParent<Expander>();
                while (parent != null)
                {
                    var parentName = GetGroupName(parent);
                    if (String.IsNullOrEmpty(parentName))
                        break;
                    groupName = String.Format("{0}\\{1}", parentName, groupName);
                    parent = parent.FindParent<Expander>();
                }
            }

            return groupName;
        }

        static string GetGroupName(Expander expander)
        {
            if (expander.DataContext is CollectionViewGroup)
            {
                var group = expander.DataContext as CollectionViewGroup;
                return group.Name as String;
            }
            else if (expander.DataContext is PropertyGridRow)
            {
                var node = (expander.DataContext as PropertyGridRow).Node;
                return node.HumanName;
            }

            return null;
        }
        #endregion
    }

    public class NodeToExpandedConverter : IMultiValueConverter
    {
        #region IValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var expander = values[0] as Expander;
            bool isExpanded = GroupToExpandedConverter.NeedToBeExpanded(expander);

            var treeViewItem = values[1] as TreeViewItem;
            if (treeViewItem.HasItems)
            {
                expander.Collapsed += (s, e) =>
                {
                    treeViewItem.IsExpanded = false;
                };
                expander.Expanded += (s, e) =>
                {
                    treeViewItem.IsExpanded = true;
                };
            }
            return isExpanded;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

    public sealed class NodeIndentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var treeViewItem = value as TreeListViewItem;
            if (treeViewItem != null)
            {
                int indent = (int)treeViewItem.Level * 14;
                if (treeViewItem.HasItems)
                    indent += 16;

                if (targetType == typeof(Thickness))
                {
                    return new Thickness(indent, 0, 0, 0);
                }

                return indent;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public sealed class GroupItemToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var viewGroup = value as CollectionViewGroup;
            if (viewGroup != null && (viewGroup.Name as String) == Properties.Resources.AdvancedGroupName)
                return new Thickness(-20, 0, 20, 0);
            else
                return new Thickness(0, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    //[ValueConversion(typeof(Boolean), typeof(Double))]
    //public class IsCheckedToAngleConverter : IValueConverter
    //{

    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (targetType != typeof(Double))
    //            throw new InvalidOperationException("The target must be a Double");

    //        if (!(value is Boolean))
    //            throw new InvalidOperationException("The value must be a Boolean");

    //        return (Boolean)value ? 90.0 : 0.0;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}

    //[ValueConversion(typeof(System.Windows.Media.Brush), typeof(System.Windows.Media.Brush))]
    //public class SolidColorBrushMultiplyConververter : IValueConverter
    //{

    //    #region IValueConverter Members

    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (targetType != typeof(System.Windows.Media.Brush))
    //            throw new InvalidOperationException("The target must be a System.Windows.Media.Brush");

    //        var color = value as System.Windows.Media.SolidColorBrush;
    //        if (color == null)
    //            return value;

    //        try
    //        {
    //            var coefficent = System.Convert.ToSingle(parameter, System.Globalization.CultureInfo.InvariantCulture);
    //            var newColor = System.Windows.Media.Color.Multiply(color.Color, coefficent);
    //            newColor.A = color.Color.A;
    //            return new System.Windows.Media.SolidColorBrush(newColor);
    //        }
    //        catch
    //        {
    //            return value;
    //        }
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}

    public class GroupStyleSelector : StyleSelector
    {
        public Style ExpanderNameGroupStyle { get; set; }
        public Style ExpanderNameAdvancedStyle { get; set; }
        public Style EmptyNameGroupStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var viewGroup = item as CollectionViewGroup;
            if (viewGroup != null && viewGroup.Name is String && String.IsNullOrEmpty(viewGroup.Name as String))
                return EmptyNameGroupStyle;

            if (ExpanderNameAdvancedStyle != null && viewGroup != null && (viewGroup.Name as String) == Properties.Resources.AdvancedGroupName)
                return ExpanderNameAdvancedStyle;
            else
                return ExpanderNameGroupStyle;
        }
    }
}
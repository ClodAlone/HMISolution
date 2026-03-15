#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.Generic;
#if WINRT
using Windows.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using Windows.UI;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    public abstract class TreeMapLevel : DependencyObject, IDisposable, INotifyPropertyChanged
    {
        #region Constructor
        public TreeMapLevel()
        {
            DataSource = new List<object>();
            TreeMapItems = new List<TreeMapItem>();
#if WINRT
            ColorMapping = new UniColorMapping { Color = ColorHelper.FromArgb(255, 0, 191, 255) };
#else
            ColorMapping = new UniColorMapping { Color = Color.FromArgb(255, 0, 191, 255) };
#endif
        }
        #endregion

        #region Dependency Properties

        #region GroupingPath
        internal string GroupingPath
        {
            get { return (string)GetValue(GroupingPathProperty); }
            set { SetValue(GroupingPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupingPath.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty GroupingPathProperty =
            DependencyProperty.Register("GroupingPath", typeof(string), typeof(TreeMapLevel), new PropertyMetadata(null));
        #endregion

        #region GroupingGap
        internal double GroupingGap
        {
            get { return (double)GetValue(GroupingGapProperty); }
            set { SetValue(GroupingGapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupingGap.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty GroupingGapProperty =
            DependencyProperty.Register("GroupingGap", typeof(double), typeof(TreeMapLevel), new PropertyMetadata(0d));
        #endregion

        #region ColorMapping
        public ColorMapping ColorMapping
        {
            get { return (ColorMapping)GetValue(ColorMappingProperty); }
            set { SetValue(ColorMappingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorMapping.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorMappingProperty =
            DependencyProperty.Register("ColorMapping", typeof(ColorMapping), typeof(TreeMapLevel), new PropertyMetadata(null, OnColorMappingChanged));

        private static void OnColorMappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLevel)
            {
                TreeMapLevel treeMapLevel = d as TreeMapLevel;
                treeMapLevel.ColorMapping.EvaluateColorMapping(treeMapLevel.TreeMapItems);
            }
        }
        #endregion

        #region HeaderHeight
        public double HeaderHeight
        {
            get { return (double)GetValue(HeaderHeightProperty); }
            set { SetValue(HeaderHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderHeightProperty =
            DependencyProperty.Register("HeaderHeight", typeof(double), typeof(TreeMapLevel), new PropertyMetadata(0d, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLevel)
            {
                TreeMapLevel treeMapLevel = d as TreeMapLevel;
                if (treeMapLevel.TreeMapItems.Count > 0 && treeMapLevel.TreeMapItems[0].TreeMap != null)
                {
                    treeMapLevel.TreeMapItems[0].TreeMap.UpdateTreeMapItems();
                }
            }
        }
        #endregion

        #region LevelHeaderPath
        internal string LevelHeaderPath
        {
            get { return (string)GetValue(LevelHeaderPathProperty); }
            set { SetValue(LevelHeaderPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelHeaderPath.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LevelHeaderPathProperty =
            DependencyProperty.Register("LevelHeaderPath", typeof(string), typeof(TreeMapLevel), new PropertyMetadata(null));
        #endregion

        #region HeaderTemplate
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register("HeaderTemplate", typeof(DataTemplate), typeof(TreeMapLevel), new PropertyMetadata(null, OnPropertyChanged));
        #endregion

        #region LabelTemplate
        public DataTemplate LabelTemplate
        {
            get { return (DataTemplate)GetValue(LabelTemplateProperty); }
            set { SetValue(LabelTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelTemplateProperty =
            DependencyProperty.Register("LabelTemplate", typeof(DataTemplate), typeof(TreeMapLevel), new PropertyMetadata(null, OnPropertyChanged));
        #endregion

        #region ShowLabels
        public bool ShowLabels
        {
            get { return (bool)GetValue(ShowLabelsProperty); }
            set { SetValue(ShowLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowLabels.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLabelsProperty =
            DependencyProperty.Register("ShowLabels", typeof(bool), typeof(TreeMapLevel), new PropertyMetadata(false, OnShowLabelsChanged));

        private static void OnShowLabelsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLevel)
            {
                var treeMapLevel = d as TreeMapLevel;
                treeMapLevel.OnPropertyChanged("ShowLabels");
            }
        }
        #endregion

        #region DataSource
        internal List<object> DataSource
        {
            get { return (List<object>)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(List<object>), typeof(TreeMapLevel), new PropertyMetadata(null));
        #endregion

        #region TreeMapItems
        public List<TreeMapItem> TreeMapItems
        {
            get { return (List<TreeMapItem>)GetValue(TreeMapItemsProperty); }
            internal set { SetValue(TreeMapItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreeMapItems. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TreeMapItemsProperty =
            DependencyProperty.Register("TreeMapItems", typeof(List<TreeMapItem>), typeof(TreeMapLevel), new PropertyMetadata(null, OnTreeMapItemsChanged));

        private static void OnTreeMapItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLevel)
            {
                TreeMapLevel treeMapLevel = d as TreeMapLevel;
                if (treeMapLevel.HeaderHeight != 0 && treeMapLevel.ColorMapping != null && treeMapLevel.TreeMapItems.Count > 0)
                {
                    treeMapLevel.ColorMapping.EvaluateColorMapping(treeMapLevel.TreeMapItems);
                }
                foreach (TreeMapItem treeMapItem in treeMapLevel.TreeMapItems)
                {
                    treeMapItem.TreeMapLevel = treeMapLevel;
                }
            }
        }
        #endregion

        #region LevelLabelPath
        internal string LevelLabelPath
        {
            get { return (string)GetValue(LevelLabelPathProperty); }
            set { SetValue(LevelLabelPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LevelLabelPath.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LevelLabelPathProperty =
            DependencyProperty.Register("LevelLabelPath", typeof(string), typeof(TreeMapLevel), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Internal Properties

#if !WINDOWS_PHONE
        internal string drillDownHeader; 
#endif

        #endregion

        #region IDisposable Method
        public void Dispose()
        {
            if (TreeMapItems != null)
            {
                TreeMapItems.Clear();
                TreeMapItems = null;
            }
            if (DataSource != null)
            {
                DataSource.Clear();
                DataSource = null;
            }
            ColorMapping = null;
        } 
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class TreeMapFlatLevel : TreeMapLevel
    {
        #region Dependency Properties

        #region GroupPath
        public string GroupPath
        {
            get { return (string)GetValue(GroupPathProperty); }
            set { SetValue(GroupPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupPathProperty =
            DependencyProperty.Register("GroupPath", typeof(string), typeof(TreeMapFlatLevel), new PropertyMetadata(null, OnGroupPathChanged));

        private static void OnGroupPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapFlatLevel)
            {
                (d as TreeMapFlatLevel).GroupingPath = e.NewValue.ToString();
            }
        }
        #endregion

        #region GroupGap
        public double GroupGap
        {
            get { return (double)GetValue(GroupGapProperty); }
            set { SetValue(GroupGapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GroupGap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GroupGapProperty =
            DependencyProperty.Register("GroupGap", typeof(double), typeof(TreeMapFlatLevel), new PropertyMetadata(0d, OnGroupGapChanged));

        private static void OnGroupGapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapFlatLevel)
            {
                (d as TreeMapFlatLevel).GroupingGap = (double)e.NewValue;
            }
        }
        #endregion

        #endregion
    }

    public class TreeMapHierarchicalLevel : TreeMapLevel
    {
        #region Dependency Properties

        #region ChildPath
        public string ChildPath
        {
            get { return (string)GetValue(ChildPathProperty); }
            set { SetValue(ChildPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildPathProperty =
            DependencyProperty.Register("ChildPath", typeof(string), typeof(TreeMapHierarchicalLevel), new PropertyMetadata(null, OnChildPathChanged));

        private static void OnChildPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapHierarchicalLevel)
            {
                (d as TreeMapHierarchicalLevel).GroupingPath = e.NewValue.ToString();
            }
        }
        #endregion

        #region ChildGap
        public double ChildGap
        {
            get { return (double)GetValue(ChildGapProperty); }
            set { SetValue(ChildGapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildGap.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildGapProperty =
            DependencyProperty.Register("ChildGap", typeof(double), typeof(TreeMapHierarchicalLevel), new PropertyMetadata(0d, OnChildGapChanged));

        private static void OnChildGapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapHierarchicalLevel)
            {
                (d as TreeMapHierarchicalLevel).GroupingGap = (double)e.NewValue;
            }
        }
        #endregion

        #region LabelPath
        public string LabelPath
        {
            get { return (string)GetValue(LabelPathProperty); }
            set { SetValue(LabelPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelPathProperty =
            DependencyProperty.Register("LabelPath", typeof(string), typeof(TreeMapHierarchicalLevel), new PropertyMetadata(null, OnLabelPathChanged));

        private static void OnLabelPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapHierarchicalLevel)
            {
                (d as TreeMapHierarchicalLevel).LevelLabelPath = e.NewValue.ToString();
            }
        }
        #endregion

        #region HeaderPath
        public string HeaderPath
        {
            get { return (string)GetValue(HeaderPathProperty); }
            set { SetValue(HeaderPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderPathProperty =
            DependencyProperty.Register("HeaderPath", typeof(string), typeof(TreeMapHierarchicalLevel), new PropertyMetadata(null, OnHeaderPathChanged));

        private static void OnHeaderPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapHierarchicalLevel)
            {
                (d as TreeMapHierarchicalLevel).LevelHeaderPath = e.NewValue.ToString();
            }
        }
        #endregion

        #endregion
    }
}

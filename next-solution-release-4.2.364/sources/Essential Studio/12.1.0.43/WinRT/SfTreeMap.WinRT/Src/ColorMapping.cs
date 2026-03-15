#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using System.Linq;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    #region ColorMapping

    public class ColorMapping : DependencyObject
    {
        #region Implementation

        public virtual void EvaluateColorMapping(List<TreeMapLeafNode> leafNodes)
        {
            ColorMapping colorMapping = this;
            int itemsCount = leafNodes.Count;

            switch (GetType().Name)
            {
                case "UniColorMapping":
                    (colorMapping as UniColorMapping).SetUniColor(leafNodes);
                    break;

                case "RangeBrushColorMapping":
                    (colorMapping as RangeBrushColorMapping).SetRangeBrushColor(leafNodes);
                    break;

                case "DesaturationColorMapping":
                    (colorMapping as DesaturationColorMapping).SetDesaturatedColor(leafNodes);
                    break;

                case "PaletteColorMapping":
                    (colorMapping as PaletteColorMapping).SetPaletteColor(leafNodes);
                    break;

                default:
#if WINRT
                    Color defaultColor = ColorHelper.FromArgb(255, 174, 217, 96);
#else
                    Color defaultColor = Color.FromArgb(255, 174, 217, 96);
#endif
                    for (int i = itemsCount - 1; i >= 0; i--)
                        leafNodes[i].MappedColor = new SolidColorBrush(defaultColor);
                    break;
            }
        }

        public virtual void EvaluateColorMapping(List<TreeMapItem> treeMapItems)
        {
            ColorMapping colorMapping = this;
            int itemsCount = treeMapItems.Count;

            switch (GetType().Name)
            {
                case "UniColorMapping":
                    (colorMapping as UniColorMapping).SetUniColor(treeMapItems, true);
                    break;

                case "RangeBrushColorMapping":
                    (colorMapping as RangeBrushColorMapping).SetRangeBrushColor(treeMapItems, true);
                    break;

                case "DesaturationColorMapping":
                    (colorMapping as DesaturationColorMapping).SetDesaturatedColor(treeMapItems, true);
                    break;

                case "PaletteColorMapping":
                    (colorMapping as PaletteColorMapping).SetPaletteColor(treeMapItems, true);
                    break;

                default:
#if WINRT
                    Color defaultColor = ColorHelper.FromArgb(255, 119, 216, 216);
#else
                    Color defaultColor = Color.FromArgb(255, 119, 216, 216);
#endif
                    for (int i = itemsCount - 1; i >= 0; i--)
                        treeMapItems[i].HeaderColor = new SolidColorBrush(defaultColor);
                    break;
            }
        }

        internal void EvaluateColorMapping(List<TreeMapItem> treeMapItems, bool isHeader)
        {
            ColorMapping colorMapping = this;
            int itemsCount = treeMapItems.Count;

            switch (GetType().Name)
            {
                case "UniColorMapping":
                    (colorMapping as UniColorMapping).SetUniColor(treeMapItems, isHeader);
                    break;

                case "RangeBrushColorMapping":
                    (colorMapping as RangeBrushColorMapping).SetRangeBrushColor(treeMapItems, isHeader);
                    break;

                case "DesaturationColorMapping":
                    (colorMapping as DesaturationColorMapping).SetDesaturatedColor(treeMapItems, isHeader);
                    break;

                case "PaletteColorMapping":
                    (colorMapping as PaletteColorMapping).SetPaletteColor(treeMapItems, isHeader);
                    break;

                default:
#if WINRT
                    Color defaultColor = ColorHelper.FromArgb(255, 119, 216, 216);
#else
                    Color defaultColor = Color.FromArgb(255, 119, 216, 216);
#endif
                    for (int i = itemsCount - 1; i >= 0; i--)
                        treeMapItems[i].HeaderColor = new SolidColorBrush(defaultColor);
                    break;
            }
        }

        public void SetColor(TreeMapLeafNode item, Brush brush)
        {
            item.MappedColor = brush;
        }

        #endregion
    }

    #endregion

    #region RangeBrushColorMapping

    public class RangeBrushColorMapping : ColorMapping
    {
        #region Constructor

        public RangeBrushColorMapping()
        {
            Brushes = new ObservableCollection<RangeBrush>();
        }

        #endregion

        #region Dependency Properties

        #region Brushes
        public ObservableCollection<RangeBrush> Brushes
        {
            get { return (ObservableCollection<RangeBrush>)GetValue(BrushesProperty); }
            set { SetValue(BrushesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Brushes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BrushesProperty =
            DependencyProperty.Register("Brushes", typeof(ObservableCollection<RangeBrush>), typeof(RangeBrushColorMapping), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Implementation

        internal void SetRangeBrushColor(List<TreeMapLeafNode> leafNodes)
        {
            int itemsCount = leafNodes.Count;

            if (Brushes != null)
                for (int i = itemsCount - 1; i >= 0; i--)
                {
                    foreach (RangeBrush rangeBrush in Brushes)
                    {
                        if (leafNodes[i].ColorWeight >= rangeBrush.From && leafNodes[i].ColorWeight <= rangeBrush.To)
                        {
                            leafNodes[i].MappedColor = new SolidColorBrush(rangeBrush.Color);
                            break;
                        }
                    }
                }
        }

        internal void SetRangeBrushColor(List<TreeMapItem> treeMapItems, bool isHeader)
        {
            int itemsCount = treeMapItems.Count;

            if (Brushes != null)
            {
                for (int i = itemsCount - 1; i >= 0; i--)
                {
                    foreach (RangeBrush rangeBrush in Brushes)
                    {
                        if (treeMapItems[i].ColorWeight >= rangeBrush.From && treeMapItems[i].ColorWeight <= rangeBrush.To)
                        {
                            if (isHeader)
                                treeMapItems[i].HeaderColor = new SolidColorBrush(rangeBrush.Color);
                            else
                                treeMapItems[i].Background = new SolidColorBrush(rangeBrush.Color);
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }

    #endregion

    #region UniColorMapping

    public class UniColorMapping : ColorMapping
    {
        #region Dependency Properties

        #region Brush
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
#if WINRT
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(UniColorMapping), new PropertyMetadata(ColorHelper.FromArgb(255, 174, 217, 96)));
#else
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(UniColorMapping), new PropertyMetadata(Color.FromArgb(255, 174, 217, 96)));
#endif
        #endregion

        #endregion

        #region Implementation

        internal void SetUniColor(List<TreeMapLeafNode> leafNodes)
        {
            int itemsCount = leafNodes.Count;
            for (int i = itemsCount - 1; i >= 0; i--)
                leafNodes[i].MappedColor = new SolidColorBrush(Color);
        }

        internal void SetUniColor(List<TreeMapItem> treeMapItem, bool isHeader)
        {
            int itemsCount = treeMapItem.Count;
            for (int i = itemsCount - 1; i >= 0; i--)
            {
                if (isHeader)
                    treeMapItem[i].HeaderColor = new SolidColorBrush(Color);
                else
                    treeMapItem[i].Background = new SolidColorBrush(Color);
            }
        }

        #endregion
    }

    #endregion

    #region PaletteColorMapping

    public class PaletteColorMapping : ColorMapping
    {
        #region Constructor

        public PaletteColorMapping()
        {
            Colors = new ObservableCollection<Brush>();
        }

        #endregion

        #region Dependency Properties

        #region Colors
        public ObservableCollection<Brush> Colors
        {
            get { return (ObservableCollection<Brush>)GetValue(ColorsProperty); }
            set { SetValue(ColorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Colors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorsProperty =
            DependencyProperty.Register("Colors", typeof(ObservableCollection<Brush>), typeof(PaletteColorMapping), new PropertyMetadata(null));
        #endregion

        #endregion

        #region Implementation

        internal void SetPaletteColor(List<TreeMapLeafNode> leafNodes)
        {
            leafNodes = new List<TreeMapLeafNode>(leafNodes.OrderByDescending(x => x.ColorWeight));

            for (int i = 0; i < leafNodes.Count; i++)
            {
                if (Colors.Count > 0)
                {
                    leafNodes[i].MappedColor = Colors[i % (Colors.Count)];
                }
            }
        }

        internal void SetPaletteColor(List<TreeMapItem> treeMapItems, bool isHeader)
        {
            treeMapItems = new List<TreeMapItem>(treeMapItems.OrderByDescending(x => x.ColorWeight));
            for (int i = 0; i < treeMapItems.Count; i++)
            {
                if (Colors.Count > 0)
                {
                    if (isHeader)
                        treeMapItems[i].HeaderColor = Colors[i % (Colors.Count)];
                    else
                        treeMapItems[i].Background = Colors[i % (Colors.Count)];
                }
            }
        }

        #endregion
    }

    #endregion

    #region DesaturationColorMapping

    public class DesaturationColorMapping : ColorMapping
    {
        #region Dependency Properties

        #region Color
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
#if WINRT
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(DesaturationColorMapping), new PropertyMetadata(ColorHelper.FromArgb(255, 0, 191, 255)));
#else
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(DesaturationColorMapping), new PropertyMetadata(Color.FromArgb(255, 0, 191, 255)));
#endif
        #endregion

        #region From
        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(double), typeof(DesaturationColorMapping), new PropertyMetadata(1d));
        #endregion

        #region To
        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double), typeof(DesaturationColorMapping), new PropertyMetadata(0d));
        #endregion

        #region RangeMinimum
        public double RangeMinimum
        {
            get { return (double)GetValue(RangeMinimumProperty); }
            set { SetValue(RangeMinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeMinimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeMinimumProperty =
            DependencyProperty.Register("RangeMinimum", typeof(double), typeof(DesaturationColorMapping), new PropertyMetadata(0d));
        #endregion

        #region RangeMaximum
        public double RangeMaximum
        {
            get { return (double)GetValue(RangeMaximumProperty); }
            set { SetValue(RangeMaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RangeMaximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RangeMaximumProperty =
            DependencyProperty.Register("RangeMaximum", typeof(double), typeof(DesaturationColorMapping), new PropertyMetadata(0d));
        #endregion

        #endregion

        #region Implementation

        internal void SetDesaturatedColor(List<TreeMapLeafNode> list)
        {
            list = new List<TreeMapLeafNode>(list.OrderByDescending(x => x.ColorWeight));

            double from = From;
            double to = To;
            if (@from < 0 || @from > 1)
                @from = 1;
            if (to > 1 || to < 0)
                to = 0;

            double interval = (@from - to) / list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                list[i].MappedColor = new SolidColorBrush(Color);

                if (list[i].ColorWeight >= RangeMinimum && list[i].ColorWeight <= RangeMaximum)
                {
                    list[i].MappedColor.Opacity = @from;
                    @from = @from - interval;
                }
            }
        }

        internal void SetDesaturatedColor(List<TreeMapItem> treeMapItems, bool isHeader)
        {
            treeMapItems = new List<TreeMapItem>(treeMapItems.OrderByDescending(x => x.ColorWeight));

            double from = From;
            double to = To;
            if (@from < 0 || @from > 1)
                @from = 1;
            if (to > 1 || to < 0)
                to = 0;

            double interval = (@from - to) / treeMapItems.Count;
            for (int i = 0; i < treeMapItems.Count; i++)
            {
                if (isHeader)
                    treeMapItems[i].HeaderColor = new SolidColorBrush(Color);
                else
                    treeMapItems[i].Background = new SolidColorBrush(Color);

                if (treeMapItems[i].ColorWeight >= RangeMinimum && treeMapItems[i].ColorWeight <= RangeMaximum)
                {
                    if (isHeader)
                        treeMapItems[i].HeaderColor.Opacity = @from;
                    else
                        treeMapItems[i].Background.Opacity = @from;
                    @from = @from - interval;
                }
            }
        }

        #endregion
    }

    #endregion

    #region RangeBrush

    public class RangeBrush : DependencyObject
    {
        #region Dependency Properties

        #region Color
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(RangeBrush), new PropertyMetadata(Colors.Transparent));
        #endregion

        #region From
        public double From
        {
            get { return (double)GetValue(FromProperty); }
            set { SetValue(FromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for From.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(double), typeof(RangeBrush), new PropertyMetadata(0d));
        #endregion

        #region To
        public double To
        {
            get { return (double)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for To.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double), typeof(RangeBrush), new PropertyMetadata(0d));
        #endregion

        #region LegendLabel
        public string LegendLabel
        {
            get { return (string)GetValue(LegendLabelProperty); }
            set { SetValue(LegendLabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LegendLabel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendLabelProperty =
            DependencyProperty.Register("LegendLabel", typeof(string), typeof(RangeBrush), new PropertyMetadata(null));
        #endregion

        #endregion
    }

    #endregion
}


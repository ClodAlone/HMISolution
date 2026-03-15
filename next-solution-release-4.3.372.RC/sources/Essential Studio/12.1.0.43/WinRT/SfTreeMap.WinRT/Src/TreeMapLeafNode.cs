#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.TreeMap
{
    public class TreeMapLeafNode : DependencyObject
    {
        #region Dependency Properties

        #region Weight
        internal double Weight
        {
            get { return (double)GetValue(WeightProperty); }
            set { SetValue(WeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Weight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WeightProperty =
            DependencyProperty.Register("Weight", typeof(double), typeof(TreeMapLeafNode), new PropertyMetadata(0d));
        #endregion

        #region ColorWeight
        internal double ColorWeight
        {
            get { return (double)GetValue(ColorWeightProperty); }
            set { SetValue(ColorWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorWeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ColorWeightProperty =
            DependencyProperty.Register("ColorWeight", typeof(double), typeof(TreeMapLeafNode), new PropertyMetadata(0d));
        #endregion

        #region AreaByWeight
        internal double AreaByWeight
        {
            get { return (double)GetValue(AreaByWeightProperty); }
            set { SetValue(AreaByWeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AreaByWeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AreaByWeightProperty =
            DependencyProperty.Register("AreaByWeight", typeof(double), typeof(TreeMapLeafNode), new PropertyMetadata(0d));
        #endregion

        #region LeftPosition
        internal double LeftPosition
        {
            get { return (double)GetValue(LeftPositionProperty); }
            set { SetValue(LeftPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty LeftPositionProperty =
            DependencyProperty.Register("LeftPosition", typeof(double), typeof(TreeMapLeafNode), new PropertyMetadata(0d));
        #endregion

        #region TopPosition
        internal double TopPosition
        {
            get { return (double)GetValue(TopPositionProperty); }
            set { SetValue(TopPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TopPositionProperty =
            DependencyProperty.Register("TopPosition", typeof(double), typeof(TreeMapLeafNode), new PropertyMetadata(0d));
        #endregion

        #region Height
        internal double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Height.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(TreeMapLeafNode), new PropertyMetadata(0d));
        #endregion

        #region Width
        internal double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Width.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(TreeMapLeafNode), new PropertyMetadata(0d));
        #endregion

        #region IsSelected
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(TreeMapLeafNode), new PropertyMetadata(false, OnSelectionChanged));

        private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLeafNode)
            {
                var leafnode = (d as TreeMapLeafNode);
                if (leafnode.TreeMapNode is Grid)
                {
                    ((leafnode.TreeMapNode as Grid).Children[0] as Rectangle).Stroke = (bool)e.NewValue ? leafnode.TreeMap.HighlightBorderBrush : leafnode.borderStroke;
                    ((leafnode.TreeMapNode as Grid).Children[0] as Rectangle).StrokeThickness = (bool)e.NewValue ? leafnode.TreeMap.HighlightBorderThickness : leafnode.borderThickness;
                }
                else if (leafnode.TreeMapNode is Border)
                {
                    (leafnode.TreeMapNode as Border).BorderBrush = (bool)e.NewValue ? leafnode.TreeMap.HighlightBorderBrush : leafnode.borderStroke;
                    (leafnode.TreeMapNode as Border).BorderThickness = (bool)e.NewValue ? new Thickness(leafnode.TreeMap.HighlightBorderThickness) : new Thickness(leafnode.borderThickness);
                }
            }
        }
        #endregion

        #region TreeMapNode
        internal object TreeMapNode
        {
            get { return GetValue(TreeMapNodeProperty); }
            set { SetValue(TreeMapNodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreeMapNode.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TreeMapNodeProperty =
            DependencyProperty.Register("TreeMapNode", typeof(object), typeof(TreeMapLeafNode), new PropertyMetadata(null, OnTreeMapNodeChanged));

        private static void OnTreeMapNodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeMapLeafNode)
            {
                var leadNode = (d as TreeMapLeafNode);
                if (leadNode.TreeMapNode is Grid)
                {
                    leadNode.borderStroke = ((leadNode.TreeMapNode as Grid).Children[0] as Rectangle).Stroke;
                    leadNode.borderThickness = ((leadNode.TreeMapNode as Grid).Children[0] as Rectangle).StrokeThickness;
                }
                else if (leadNode.TreeMapNode is Border)
                {
                    leadNode.borderStroke = (leadNode.TreeMapNode as Border).BorderBrush;
                    leadNode.borderThickness = (leadNode.TreeMapNode as Border).BorderThickness.Left;
                }
            }
        }
        #endregion

        #region TreeMap
        internal SfTreeMap TreeMap
        {
            get { return (SfTreeMap)GetValue(TreeMapProperty); }
            set { SetValue(TreeMapProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TreeMap.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TreeMapProperty =
            DependencyProperty.Register("TreeMap", typeof(SfTreeMap), typeof(TreeMapLeafNode), new PropertyMetadata(null));
        #endregion

        #region ParentNode
        internal TreeMapItem ParentNode
        {
            get { return (TreeMapItem)GetValue(ParentNodeProperty); }
            set { SetValue(ParentNodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ParentNode.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ParentNodeProperty =
            DependencyProperty.Register("ParentNode", typeof(TreeMapItem), typeof(TreeMapLeafNode), new PropertyMetadata(null));
        #endregion

        #region MappedColor
        public Brush MappedColor
        {
            get { return (Brush)GetValue(MappedColorProperty); }
            internal set { SetValue(MappedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MappedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MappedColorProperty =
            DependencyProperty.Register("MappedColor", typeof(Brush), typeof(TreeMapLeafNode), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));
        #endregion

        #region Label
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            internal set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(TreeMapLeafNode), new PropertyMetadata(null)); 
        #endregion

        #region Data
        public object Data
        {
            get { return GetValue(DataProperty); }
            internal set { SetValue(DataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Node.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(TreeMapLeafNode), new PropertyMetadata(null));
        #endregion       

        #endregion

        #region Private Properties
        Brush borderStroke;
        double borderThickness; 
        #endregion
    }
}

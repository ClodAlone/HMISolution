#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
using Windows.Foundation;
#else
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Maps
{
    /// <summary>
    /// Represents the Bubble class in the SfMap.
    /// </summary>
    /// <remarks>
    /// Bubbles are used to represent the under bound object member's value in the map. Bubbles are added in the <see cref="ShapeFileLayer"/> Bubbles read only collection.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public class Bubble : DependencyObject
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Maps.Bubble">Bubble</see> class. 
        /// </summary>
        /// <remarks>
        /// Bubbles are internally added to the <see cref="ShapeFileLayer"/> Bubbles read only collection. All the members in the Bubbles will be internally set.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public Bubble()
        {
        } 

        #endregion

        #region Internal fields

        internal int index;
        internal Point midpoint = new Point();

        #endregion

        #region Dependency Properties

        #region BubbleValue
        /// <summary>
        /// Gets  values for the bubbles.
        /// </summary>
        /// <value>Type :<see cref="Object"/></value>
        /// <remarks>
        /// BubbleValue property used to represent the value to be bounded with Bubbles.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public object BubbleValue
        {
            get { return GetValue(BubbleValueProperty); }
            internal set { SetValue(BubbleValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BubbleValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubbleValueProperty =
            DependencyProperty.Register("BubbleValue", typeof(object), typeof(Bubble), new PropertyMetadata(0d));
        #endregion

        #region BubbleItem
        /// <summary>
        /// Gets the BubbleItem. This item will be determine bubble type.
        /// </summary>
        /// <value>
        /// Type :<see cref="Shape"/>
        /// </value>
        /// <remarks>
        /// This property is used to determine the type of the bubble to be shown on the map. Currently it will be set as <see cref="Ellipse"/>
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public FrameworkElement BubbleItem
        {
            get { return (FrameworkElement)GetValue(BubbleItemProperty); }
            internal set { SetValue(BubbleItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BubbleItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubbleItemProperty =
            DependencyProperty.Register("BubbleItem", typeof(FrameworkElement), typeof(Bubble), new PropertyMetadata(null));
        #endregion

        #region BubbleColorValue
        /// <summary>
        /// Gets  values for the bubbles.
        /// </summary>
        /// <value>Type :<see cref="Object"/></value>
        /// <remarks>
        /// BubbleValue property used to represent the value to be bounded with Bubbles.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public object BubbleColorValue
        {
            get { return GetValue(BubbleColorValueProperty); }
            internal set { SetValue(BubbleColorValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BubbleValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BubbleColorValueProperty =
            DependencyProperty.Register("BubbleColorValueValue", typeof(object), typeof(Bubble), new PropertyMetadata(0d));
        #endregion

        #region Margin
        /// <summary>
        /// Gets the Margin of a bubble in the map.
        /// </summary>
        /// <remarks>
        /// This is read only property to internally set the margin of the bubbles on the map. Based on the margin value bubbles are arranged on the shapes.
        /// </remarks>
        [ClassReference(IsReviewed = false)]
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            internal set { SetValue(MarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Margin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register("Margin", typeof(Thickness), typeof(Bubble), new PropertyMetadata(null));
        #endregion         

        #endregion
    }
}

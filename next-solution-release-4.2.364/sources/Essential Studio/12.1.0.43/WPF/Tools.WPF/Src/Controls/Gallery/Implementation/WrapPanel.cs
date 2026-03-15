// <copyright file="WrapPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

#define DEBUG
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is used as item panel for <see cref="GalleryGroup"/> in <see cref="Gallery"/> in
    /// standard visual mode.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WrapPanelExt : Panel
    {
        #region Enums
        /// <summary>
        /// Contains running animations.
        /// </summary>
        public enum RunningAnimation
        {
            /// <summary>
            /// Layout animation.
            /// </summary>
            Layout,

            /// <summary>
            /// The animation which is used when the new child is added.
            /// </summary>
            New,

            /// <summary>
            /// Drag animation.
            /// </summary>
            Drag,

            /// <summary>
            /// Resize animation.
            /// </summary>
            Resize
        }
        #endregion

        #region Private fields
        /// <summary>
        /// Invokes every time when the item renders.
        /// </summary>
        private readonly EventHandler CompositionTarget_RenderingHandler;

        /// <summary>
        /// Length of the minimal value.
        /// </summary>
        private long m_lastTick = long.MinValue;

        /// <summary>
        /// Utilized for arranged items caching.
        /// </summary>
        private ItemsCollection m_arrangedItems;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="WrapPanelExt"/> class.
        /// </summary>
        static WrapPanelExt()
        {
            Gallery.AllowedAnimationsProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(AllowedAnimations.All, FrameworkPropertyMetadataOptions.Inherits));

            Gallery.AllowedItemResizeModeProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(AllowedItemResizeModes.None, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, null, CoerceAllowedItemResizeModeValueCallback));

            Gallery.ItemWidthProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
            Gallery.ItemHeightProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(50d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
            Gallery.ItemMinWidthProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
            Gallery.ItemMinHeightProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
            Gallery.ItemMaxWidthProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
            Gallery.ItemMaxHeightProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(100d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
            Gallery.AllowVaryingItemSizeProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));
            Gallery.SpaceLimitBetweenItemsProperty.AddOwner(typeof(WrapPanelExt), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

            HorizontalAlignmentProperty.OverrideMetadata(typeof(WrapPanelExt), new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, OnHorizontalAlignmentChanged));
            VerticalAlignmentProperty.OverrideMetadata(typeof(WrapPanelExt), new FrameworkPropertyMetadata(VerticalAlignment.Top, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, OnVerticalAlignmentChanged));

            RoutedEvent ev = GalleryItem.VisibilityChangedEvent.AddOwner(typeof(WrapPanelExt));
            EventManager.RegisterClassHandler(typeof(WrapPanelExt), ev, new RoutedEventHandler(VisibilityChangedEventHandler));

            ev = GalleryItem.DraggedEvent.AddOwner(typeof(WrapPanelExt));
            EventManager.RegisterClassHandler(typeof(WrapPanelExt), ev, new RoutedEventHandler(DraggedEventHandler));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrapPanelExt"/> class.
        /// </summary>
        public WrapPanelExt()
        {
            CompositionTarget_RenderingHandler = new EventHandler(CompositionTarget_Rendering);
            Loaded += new RoutedEventHandler(WrapPanelExt_Loaded);
            Unloaded += new RoutedEventHandler(WrapPanelExt_Unloaded);
        }
        #endregion

        #region	Properties
        /// <summary>
        /// Gets or sets the value indicating the allowed animations.
        /// </summary>
        /// <value>
        /// Type: <see cref="AllowedAnimations"/>
        /// <para/>
        /// Default value is AllowedAnimations.All.
        /// </value>                
        /// <seealso cref="AllowedAnimations"/> enum.
        public AllowedAnimations AllowedAnimations
        {
            get
            {
                return (AllowedAnimations)GetValue(Gallery.AllowedAnimationsProperty);
            }

            set
            {
                SetValue(Gallery.AllowedAnimationsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents width of arranged items. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 50.
        /// </value>
        public double ItemWidth
        {
            get
            {
                return (double)GetValue(Gallery.ItemWidthProperty);
            }

            set
            {
                SetValue(Gallery.ItemWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents height of arranged items. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 50.
        /// </value>
        public double ItemHeight
        {
            get
            {
                return (double)GetValue(Gallery.ItemHeightProperty);
            }

            set
            {
                SetValue(Gallery.ItemHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the allow varying item.
        /// </summary>
        /// <value>The size of the allow varying item.</value>
        public bool AllowVaryingItemSize
        {
            get
            {
                return (bool)GetValue(Gallery.AllowVaryingItemSizeProperty);
            }

            set
            {
                SetValue(Gallery.AllowVaryingItemSizeProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the minimal value of <see cref="ItemWidth"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        public double ItemMinWidth
        {
            get
            {
                return (double)GetValue(Gallery.ItemMinWidthProperty);
            }

            set
            {
                SetValue(Gallery.ItemMinWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimal value of <see cref="ItemHeight"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        public double ItemMinHeight
        {
            get
            {
                return (double)GetValue(Gallery.ItemMinHeightProperty);
            }

            set
            {
                SetValue(Gallery.ItemMinHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximal value of <see cref="ItemWidth"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 100.
        /// </value>
        public double ItemMaxWidth
        {
            get
            {
                return (double)GetValue(Gallery.ItemMaxWidthProperty);
            }

            set
            {
                SetValue(Gallery.ItemMaxWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximal value of <see cref="ItemHeight"/> property. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        public double ItemMaxHeight
        {
            get
            {
                return (double)GetValue(Gallery.ItemMaxHeightProperty);
            }
            
            set
            {
                SetValue(Gallery.ItemMaxHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that describes how item size should be changed, when panel size is changing. 
        /// This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="AllowedItemResizeModes"/>
        /// <para/>
        /// Default value is AllowedItemResizeModes.None
        /// </value>
        /// <seealso cref="AllowedItemResizeModes"/> enum.
        public AllowedItemResizeModes AllowedItemResizeMode
        {
            get
            {
                // for fixing of incorrect work of CoerceAllowedItemResizeModeValueCallback method
                return (HorizontalAlignment == HorizontalAlignment.Stretch) ? AllowedItemResizeModes.Resize : (AllowedItemResizeModes)GetValue(Gallery.AllowedItemResizeModeProperty);
            }

            set
            {
                SetValue(Gallery.AllowedItemResizeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that represents space limit between items when <see cref="AllowedItemResizeMode"/> set to 
        /// Space value. This is dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Double"/>
        /// <para/>
        /// Default value is 0.
        /// </value>
        public double SpaceLimitBetweenItems
        {
            get
            {
                return (double)GetValue(Gallery.SpaceLimitBetweenItemsProperty);
            }

            set
            {
                SetValue(Gallery.SpaceLimitBetweenItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets collection of arranged items.
        /// </summary>
        /// <value>
        /// Type: <see cref="ItemsCollection"/>
        /// </value>
        public ItemsCollection ArrangedItems
        {
            get
            {
                if (null == m_arrangedItems)
                {
                    m_arrangedItems = new ItemsCollection();

                    for (int i = 0; i < VisualChildrenCount; ++i)
                    {
                        GalleryItem item = (GalleryItem)GetVisualChild(i);

                        if (Visibility.Collapsed != item.Visibility)
                        {
                            m_arrangedItems.Add(item);
                        }
                    }
                }

                return m_arrangedItems;
            }
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets the value that represents item location point.
        /// </summary>
        /// <param name="obj">Object which location is needed.</param>
        /// <returns>
        /// Location point.
        /// </returns>
        public static Point GetItemLocation(DependencyObject obj)
        {
            return (Point)obj.GetValue(ItemLocationProperty);
        }

        /// <summary>
        /// Sets the value that represents item location point.
        /// </summary>
        /// <param name="obj">Given item.</param>
        /// <param name="value">Value of new location.</param>
        public static void SetItemLocation(DependencyObject obj, Point value)
        {
            obj.SetValue(ItemLocationProperty, value);
        }

        /// <summary>
        /// Gets the value that represents item target location point.
        /// </summary>
        /// <param name="obj">Given item.</param>
        /// <returns>
        /// Item target location point.
        /// </returns>
        public static Point GetItemTarget(DependencyObject obj)
        {
            return (Point)obj.GetValue(ItemTargetProperty);
        }

        /// <summary>
        /// Sets the value that represents item target location point.
        /// </summary>
        /// <param name="obj">Target item.</param>
        /// <param name="value">Target location point.</param>
        public static void SetItemTarget(DependencyObject obj, Point value)
        {
            obj.SetValue(ItemTargetProperty, value);
        }

        /// <summary>
        /// Gets the value that describes item animation velocity.
        /// </summary>
        /// <param name="obj">Given item.</param>
        /// <returns>
        /// Velocity vector.
        /// </returns>
        public static Vector GetVelocity(DependencyObject obj)
        {
            return (Vector)obj.GetValue(VelocityProperty);
        }

        /// <summary>
        /// Sets the value that describes item animation velocity.
        /// </summary>
        /// <param name="obj">Given item.</param>
        /// <param name="value">New velocity for given item.</param>
        public static void SetVelocity(DependencyObject obj, Vector value)
        {
            obj.SetValue(VelocityProperty, value);
        }

        /// <summary>
        /// Gets the value that describes item running animation.
        /// </summary>
        /// <param name="obj">Given item.</param>
        /// <returns>
        /// Current running animation.
        /// </returns>
        /// <seealso cref="RunningAnimation"/> enum.
        public static RunningAnimation GetRunningAnimation(DependencyObject obj)
        {
            return (RunningAnimation)obj.GetValue(RunningAnimationProperty);
        }

        /// <summary>
        /// Sets the value that describes item running animation.
        /// </summary>
        /// <param name="obj">Given item.</param>
        /// <param name="value">Current running animation.</param>
        /// <seealso cref="RunningAnimation"/> enum.
        public static void SetRunningAnimation(DependencyObject obj, RunningAnimation value)
        {
            obj.SetValue(RunningAnimationProperty, value);
        }
        #endregion

        #region	Implementation
        /// <summary>
        /// Invoked when the <see cref="System.Windows.Media.VisualCollection"/> of a visual object is modified.
        /// </summary>
        /// <param name="visualAdded">The <see cref="System.Windows.Media.Visual"/> that was added to the collection.</param>
        /// <param name="visualRemoved">The <see cref="System.Windows.Media.Visual"/> that was removed from the collection.</param>
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            GalleryGroup visualgroup = (GalleryGroup)VisualUtils.FindAncestor((Visual)visualAdded, typeof(GalleryGroup));
            if (visualgroup != null)
            {
                visualgroup.OnItemGenerated(new RoutedEventArgs(GalleryGroup.ItemGeneratedEvent,visualgroup));
            }
            if (null != m_arrangedItems)
            {
                if (null != visualAdded)
                {
                    GalleryItem item = (GalleryItem)visualAdded;

                    if (Visibility.Collapsed != item.Visibility)
                    {
                        int vcc = VisualChildrenCount;

                        if (GetVisualChild(vcc - 1) != item)
                        {
                            for (int i = 0; i < vcc; ++i)
                            {
                                GalleryItem gItem = (GalleryItem)GetVisualChild(i);

                                if (gItem == item)
                                {
                                    m_arrangedItems.Insert(i, item);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            m_arrangedItems.Add(item);
                        }
                    }
                }

                if (null != visualRemoved)
                {
                    GalleryItem item = (GalleryItem)visualRemoved;

                    if (m_arrangedItems.Contains(item))
                    {
                        m_arrangedItems.Remove(item);
                    }
                }
            }

            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        /// <summary>
        /// Arranges the content.
        /// </summary>
        /// <param name="finalSize">Available size.</param>
        /// <returns>
        /// Size in which the content is arranged.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return ArrangeMeasure(finalSize, false);
        }

        /// <summary>
        /// Measures children in anticipation of arranging them during
        /// the ArrangeOverride pass.
        /// </summary>
        /// <param name="constraint">Size given for panel.</param>
        /// <returns>
        /// Size which is needed for the panel.
        /// </returns>
        protected override Size MeasureOverride(Size constraint)
        {
            return ArrangeMeasure(constraint, true);
        }

        /// <summary>
        /// Gets seconds from ticks.
        /// </summary>
        /// <param name="diff">Value of ticks.</param>
        /// <returns>
        /// Returns the Seconds.
        /// </returns>
        private static double SecondsFromTicks(long diff)
        {
            double seconds = diff / (double)10000000; // 1 tick = 100-nanoseconds, so 10,000,000
            return seconds;
        }

        /// <summary>
        /// Invoked when horizontal alignment is changed.
        /// </summary>
        /// <param name="sender">WrapPanelExt class instance.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnHorizontalAlignmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WrapPanelExt panel = (WrapPanelExt)sender;

            panel.CoerceValue(Gallery.AllowedItemResizeModeProperty);
        }

        /// <summary>
        /// Invoked when Vertical alignment is changed.
        /// </summary>
        /// <param name="sender">WrapPanelExt class instance.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void OnVerticalAlignmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            WrapPanelExt panel = (WrapPanelExt)sender;

            panel.CoerceValue(Gallery.AllowedItemResizeModeProperty);
        }
        /// <summary>
        /// Coerces allowed item resize mode.
        /// </summary>
        /// <param name="d">Dependency object, the change occurs
        /// on.</param>
        /// <param name="baseValue">Base value.</param>
        /// <returns>
        /// Allowed item resize mode.
        /// </returns>
        private static object CoerceAllowedItemResizeModeValueCallback(DependencyObject d, object baseValue)
        {
            WrapPanelExt panel = (WrapPanelExt)d;

            if (panel.VerticalAlignment == VerticalAlignment.Stretch || panel.HorizontalAlignment == HorizontalAlignment.Stretch)
            {
                return AllowedItemResizeModes.Resize;
            }
            else
            {
                return baseValue;
            }
            //return  baseValue;
        }

        /// <summary>
        /// Invoked when visibility of item is changed.
        /// </summary>
        /// <param name="sender">Item panel child.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void VisibilityChangedEventHandler(object sender, RoutedEventArgs e)
        {
            GalleryItem item = (GalleryItem)e.OriginalSource;
            WrapPanelExt panel = (WrapPanelExt)sender;

            if (item.Visibility == Visibility.Collapsed)
            {
                if (panel.m_arrangedItems != null)
                {
                    panel.m_arrangedItems.Remove(item);
                }
            }
            else
            {
                panel.m_arrangedItems = null;
            }
        }

        /// <summary>
        /// Invoked when item is dropped.
        /// </summary>
        /// <param name="sender">Object in which the change occurs.</param>
        /// <param name="e">The instance containing the event data.</param>
        private static void DraggedEventHandler(object sender, RoutedEventArgs e)
        {
            ((WrapPanelExt)sender).m_arrangedItems = null;
        }

        /// <summary>
        /// Corrects item size.
        /// </summary>
        /// <param name="itemWidth">Item width.</param>
        /// <param name="itemHeight">Item height.</param>
        private void CorrectItemSize(ref double itemWidth, ref double itemHeight)
        {
            if (itemWidth > ItemMaxWidth)
            {
                itemWidth = ItemMaxWidth;
            }

            if (itemWidth < ItemMinWidth)
            {
                itemWidth = ItemMinWidth;
            }

            if (itemHeight > ItemMaxHeight)
            {
                itemHeight = ItemMaxHeight;
            }

            if (itemHeight < ItemMinHeight)
            {
                itemHeight = ItemMinHeight;
            }
        }

        /// <summary>
        /// Arranges the measure.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        /// <param name="justMeasure">if set to <c>true</c> [just measure].</param>
        /// <returns>The size used</returns>
        private Size ArrangeMeasure(Size finalSize, bool justMeasure)
        {
            double y = 0;
            double space = 0;

            ItemsCollection visibleCollection = ArrangedItems;

            int visibleCount = visibleCollection.Count;

            double itemWidthOriginal = ItemWidth;
            double itemHeightOriginal = ItemHeight;
            double itemWidth = itemWidthOriginal;
            double itemHeight = itemHeightOriginal;

            CorrectItemSize(ref itemWidth, ref itemHeight);

            double widthAvailable = finalSize.Width;

            int supposedCountPerLine;

            if (AllowedItemResizeMode == AllowedItemResizeModes.Resize)
            {
                supposedCountPerLine = Math.Min((int)(widthAvailable / itemWidth), visibleCount);
                double supposedLineWidth = supposedCountPerLine * itemWidth;

                if (supposedCountPerLine > 1 && supposedLineWidth > widthAvailable + (itemWidth * 0.1d))
                {
                    supposedCountPerLine--;
                    supposedLineWidth -= itemWidth;
                }

                itemWidth = widthAvailable / supposedCountPerLine;
                itemHeight = itemHeightOriginal * itemWidth / itemWidthOriginal;
            }
            else
            {
                supposedCountPerLine = Math.Min((int)Math.Truncate(widthAvailable / itemWidth), visibleCount);

                if (AllowedItemResizeMode == AllowedItemResizeModes.Space)
                {
                    space = Math.Abs((supposedCountPerLine * itemWidth) - widthAvailable) / supposedCountPerLine;

                    if (space > SpaceLimitBetweenItems)
                    {
                        space = SpaceLimitBetweenItems;
                    }
                }
            }

            if (supposedCountPerLine == 0)
            {
                supposedCountPerLine = 1;
            }

            CorrectItemSize(ref itemWidth, ref itemHeight);

            double availablewidth = 0.0;
            int countperline = 0;
            int iEnd = 0;
            ArrayList Itemperlinelist=new ArrayList();
            ArrayList Alignmentspacelist = new ArrayList();

            if (AllowVaryingItemSize)
            {
                for (int i = 0; i < visibleCount; i++)
                {
                    if (AllowVaryingItemSize)
                    {
                        GalleryItem visiblegalleryitem = (visibleCollection[i] as GalleryItem);

                        if (visiblegalleryitem != null)
                        {
                            if (visiblegalleryitem.Width > 0)
                            {
                                itemWidth = visiblegalleryitem.Width;
                            }
                            if (visiblegalleryitem.Height > 0)
                            {
                                itemHeight = visiblegalleryitem.Height;
                            }

                            if (availablewidth + itemWidth > widthAvailable)
                            {
                                Itemperlinelist.Add(countperline);
                                Alignmentspacelist.Add(widthAvailable - availablewidth);
                                availablewidth = itemWidth;
                                countperline = 1;
                            }
                            else
                            {
                                if (itemWidth > 0)
                                {
                                    availablewidth = availablewidth + itemWidth+SpaceLimitBetweenItems;
                                    countperline++;
                                    if (Itemperlinelist.Count == 0)
                                    {
                                        if (i == visibleCount - 1)
                                        {
                                            Itemperlinelist.Add(countperline);
                                            Alignmentspacelist.Add(widthAvailable - availablewidth-SpaceLimitBetweenItems*countperline);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                int k = 0;
               while( k < visibleCount)
                {
                    for (int j = 0; j < Itemperlinelist.Count; j++)
                    {
                        iEnd =k+Convert.ToInt32(Itemperlinelist[j]);
                        if (iEnd > visibleCount)
                        {
                            iEnd = visibleCount;
                        }
                        if (!justMeasure)
                        {
                            if (HorizontalAlignment == HorizontalAlignment.Right)
                            {
                                ArrangeLine(Convert.ToDouble(Alignmentspacelist[j]), y, iEnd - 1, k - 1, -itemWidthOriginal, itemHeightOriginal, visibleCollection, -space, -1, true);
                            }
                            else
                                if (HorizontalAlignment == HorizontalAlignment.Center)
                                {
                                    double x = Convert.ToDouble(Alignmentspacelist[j])/2;

                                    ArrangeLine(x, y, k, iEnd, itemWidthOriginal, itemHeightOriginal, visibleCollection, space, 1, false);
                                }
                                else
                                {
                                    ArrangeLine(0, y, k, iEnd, itemWidthOriginal, itemHeightOriginal, visibleCollection, space, 1, false);
                                }
                        }
                        else
                        {
                                for (int n = k; n < iEnd; n++)
                                {
                                    visibleCollection[n].Measure(finalSize);
                                }
                            }
                        if (iEnd < visibleCount)
                        {
                            k = k + Convert.ToInt32(Itemperlinelist[j]);
                        }
                        if(j==Itemperlinelist.Count-1)
                        {
                            k = visibleCount;
                        }
                        y += itemHeight;
                    }
                }
            }
            else
            {
                for (int i = 0; i < visibleCount; i += supposedCountPerLine)
                {

                    iEnd = Math.Min(visibleCount, i + supposedCountPerLine);

                    if (!justMeasure)
                    {
                        if (HorizontalAlignment == HorizontalAlignment.Right)
                        {
                            ArrangeLine(widthAvailable - itemWidth, y, iEnd - 1, i - 1, -itemWidth, itemHeight, visibleCollection, -space, -1, true);
                        }
                        else
                            if (HorizontalAlignment == HorizontalAlignment.Center)
                            {
                                double x = (widthAvailable - ((iEnd - i) * (itemWidth + space))) / 2;

                                ArrangeLine(x, y, i, iEnd, itemWidth, itemHeight, visibleCollection, space, 1, false);
                            }
                            else
                            {
                                ArrangeLine(0, y, i, iEnd, itemWidth, itemHeight, visibleCollection, space, 1, false);
                            }
                    }
                    else
                    {
                        if (i > 0)
                        {
                            for (int n = i; n < iEnd; n++)
                            {
                                visibleCollection[n].Measure(finalSize);
                            }
                        }
                    }

                    y += itemHeight;
                }
            }

            finalSize.Height = y;

            return finalSize;
        }

        /// <summary>
        /// Arranges items from start to end on single line.
        /// </summary>
        /// <param name="x">x coordinate of item
        /// location.</param>
        /// <param name="y">y coordinate of item
        /// location</param>
        /// <param name="start">Starting index.</param>
        /// <param name="end">Ending index.</param>
        /// <param name="width">Width of the item.</param>
        /// <param name="height">Height of the item.</param>
        /// <param name="itemsCollection">Collection of
        /// GalleryItem
        /// instances that are
        /// arranged.</param>
        /// <param name="space">Space between items.</param>
        /// <param name="step">Indicates direction
        /// of step at different
        /// values of
        /// HorizontalAlignment
        /// property.</param>
        /// <param name="horizontalAlignmentRight">True, if
        /// HorizontalAlignment
        /// property value is
        /// HorizontalAlignment.Right,
        /// otherwise, false.</param>
        private void ArrangeLine(double x, double y, int start, int end, double width, double height, ItemsCollection itemsCollection, double space, int step, bool horizontalAlignmentRight)
        {
            Point rightHorizontalAlignmentStartPoint = new Point(x, 0);

            for (int i = start; i != end; i += step)
            {
                UIElement element = itemsCollection[i];
                if (element != null)
                {
                    Point newOffset = new Point(x, y);
                     Size itemSize=new Size(0,0);
                     if (AllowVaryingItemSize)
                     {
                         if ((element as GalleryItem).Width > 0)
                         {
                             width = (element as GalleryItem).Width;
                         }
                         if((element as GalleryItem).Height>0)
                         {
                             height = (element as GalleryItem).Height;
                         }
                         itemSize = new Size(Math.Abs(width),height);
                     }
                     else
                     {
                         itemSize = new Size(Math.Abs(width), height);
                     }
                    element.SetValue(ItemTargetProperty, newOffset);

                    if (element.ReadLocalValue(ItemLocationProperty) == DependencyProperty.UnsetValue)
                    {
                        element.Arrange(new Rect(newOffset, itemSize));

                        if (horizontalAlignmentRight)
                        {
                            element.SetValue(ItemLocationProperty, rightHorizontalAlignmentStartPoint);
                        }
                    }
                    else
                    {
                        Point currentOffset = (Point)element.GetValue(ItemLocationProperty);

                        if (Math.Abs(newOffset.Y - currentOffset.Y) < 1
                            && (AllowedAnimations & AllowedAnimations.Layout) == AllowedAnimations.Layout)
                        {
                            WrapPanelExt.SetRunningAnimation(element, RunningAnimation.Resize);
                        }

                        element.Arrange(new Rect(currentOffset, itemSize));
                    }

                    x += width + space;
                }
            }
        }

        /// <summary>
        /// Updates item coordinates when animation is running.
        /// </summary>
        /// <param name="item">Item panel child.</param>
        /// <param name="seconds">Value that represents
        /// seconds.</param>
        /// <param name="dampening">Value that represents
        /// allowed animation dampening.</param>
        /// <param name="attractionFactor">Attraction factor for
        /// allowed animation.</param>
        /// <param name="allowedAnimations">Allowed animation.</param>
        /// <returns>
        /// True, if item should be rendered; otherwise, false.
        /// </returns>
        private bool UpdateItem(UIElement item, double seconds, double dampening, double attractionFactor, AllowedAnimations allowedAnimations)
        {
            Point current = (Point)item.GetValue(ItemLocationProperty);
            Point target = (Point)item.GetValue(ItemTargetProperty);

            Vector velocity = (Vector)item.GetValue(VelocityProperty);

            Vector diff = target - current;

            if (diff.Length > 0.1 || velocity.Length > 0.1)
            {
                RunningAnimation running = GetRunningAnimation(item);

                bool bAnimate = true;

                if ((running == RunningAnimation.Drag && ((allowedAnimations & AllowedAnimations.Drag) != AllowedAnimations.Drag))
                    ||
                    (running == RunningAnimation.New && ((allowedAnimations & AllowedAnimations.New) != AllowedAnimations.New))
                    ||
                    (running == RunningAnimation.Layout && ((allowedAnimations & AllowedAnimations.Layout) != AllowedAnimations.Layout))
                    ||
                    (running == RunningAnimation.Resize && ((allowedAnimations & AllowedAnimations.Resize) != AllowedAnimations.Resize)))
                {
                    bAnimate = false;
                }

                if (bAnimate)
                {
                    velocity.X *= dampening;
                    velocity.Y *= dampening;

                    velocity += diff;

                    Vector delta = velocity * seconds * attractionFactor;

                    double maxVelocity = 100;
                    delta *= (delta.Length > maxVelocity) ? (maxVelocity / delta.Length) : 1;

                    current += delta;

                    item.SetValue(ItemLocationProperty, current);
                    item.SetValue(VelocityProperty, velocity);
                }
                else
                {
                    item.SetValue(ItemLocationProperty, target);
                    item.ClearValue(VelocityProperty);
                    item.ClearValue(RunningAnimationProperty);
                }

                return true;
            }

            item.ClearValue(RunningAnimationProperty);

            return false;
        }

        /// <summary>
        /// Handles the Loaded event of the WrapPanelExt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WrapPanelExt_Loaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        /// <summary>
        /// Handles the Unloaded event of the WrapPanelExt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void WrapPanelExt_Unloaded(object sender, RoutedEventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_RenderingHandler;
        }

        /// <summary>
        /// Handles the Rendering event of the CompositionTarget control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (IsVisible)
            {
                long nowTick = DateTime.Now.Ticks;
                long diff = nowTick - m_lastTick;
                m_lastTick = nowTick;

                double seconds = SecondsFromTicks(diff);
                seconds = 1d / 20d;

                double attractionFactor = 2.0d;
                Random rnd = new Random();
                bool bChanged = false;
                AllowedAnimations allowed = (AllowedAnimations)GetValue(Gallery.AllowedAnimationsProperty);

                foreach (UIElement child in Children)
                {
                    double factor = (rnd.NextDouble() * 0.3d) + 0.6d;
                    double itemAttractionFactor = attractionFactor + (rnd.NextDouble() * 3d) - 1.5d;
                    bChanged |= UpdateItem(child, seconds, factor, itemAttractionFactor, allowed);
                }

                if (bChanged)
                {
                    InvalidateArrange();
                }
            }
        }
        #endregion

        #region	Dependency properties
        /// <summary>
        /// Identifies ItemLocation dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemLocationProperty =
            DependencyProperty.RegisterAttached("ItemLocation", typeof(Point), typeof(WrapPanelExt), new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Identifies ItemTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTargetProperty =
            DependencyProperty.RegisterAttached("ItemTarget", typeof(Point), typeof(WrapPanelExt));

        /// <summary>
        /// Identifies Velocity dependency property.
        /// </summary>
        public static readonly DependencyProperty VelocityProperty =
            DependencyProperty.RegisterAttached("Velocity", typeof(Vector), typeof(WrapPanelExt));

        /// <summary>
        /// Identifies <see cref="RunningAnimation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RunningAnimationProperty =
            DependencyProperty.RegisterAttached("RunningAnimation", typeof(RunningAnimation), typeof(WrapPanelExt), new UIPropertyMetadata(RunningAnimation.Layout));
        #endregion
    }
}

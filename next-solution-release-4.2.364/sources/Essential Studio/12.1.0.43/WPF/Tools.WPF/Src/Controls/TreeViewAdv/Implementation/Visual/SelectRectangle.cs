// <copyright file="SelectRectangle.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

#region file using

using System.Windows;
using System.Windows.Media;

#endregion file using

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a element that displays selected rectangle for Blend skin.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SelectRectangle : FrameworkElement
    {
        #region Constants

        /// <summary>
        /// Default weight of the rectangle.
        /// </summary>
        private const int C_defaultWeight = 1000;

        #endregion Constants

        #region Dependency properties

        /// <summary>
        /// Identifies TargetItem. Length dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetItemProperty =
            DependencyProperty.Register("TargetItem", typeof(TreeViewItemAdv), typeof(SelectRectangle), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// Identifies Fill. Length dependency property.
        /// </summary>
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(SelectRectangle), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion Dependency properties

        #region Properties

        /// <summary>
        /// Gets or sets target item for select rectangle.
        /// </summary>
        public TreeViewItemAdv TargetItem
        {
            get
            {
                return (TreeViewItemAdv)GetValue(TargetItemProperty);
            }

            set
            {
                SetValue(TargetItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Brush that specifies how to paint the select rectangle.
        /// </summary>
        public Brush Fill
        {
            get
            {
                return (Brush)GetValue(FillProperty);
            }

            set
            {
                SetValue(FillProperty, value);
            }
        }

        #endregion Properties

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectRectangle"/> class.
        /// </summary>
        public SelectRectangle()
        {
            Loaded += new RoutedEventHandler(SelectRectangle_Loaded);
        }

        #endregion Initialization

        #region Implementation

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect rect = GetFillRect();
            drawingContext.DrawRectangle(Fill, null, rect);
        }

        /// <summary>
        /// Gets the fill rect.
        /// </summary>
        /// <returns>Rect of fill rect</returns>
        private Rect GetFillRect()
        {
            Rect rect = Rect.Empty;
            double height = 0;
            double offset = C_defaultWeight;

            if (TargetItem != null)
            {
                if (TargetItem.IsExpanded)
                {
                    height = (TargetItem.CompleteHeaderElement == null) ?
                        TargetItem.DesiredSize.Height - (Margin.Bottom + Margin.Top) :
                        TargetItem.CompleteHeaderElement.DesiredSize.Height - (Margin.Bottom + Margin.Top);
                }
                else
                {
                    height = TargetItem.DesiredSize.Height - (Margin.Bottom + Margin.Top);
                }

                rect = new Rect(-offset, Margin.Top, 2 * offset, height);
            }
            else
            {
                rect = new Rect(0, 0, 0, 0);
            }

            return rect;
        }

        /// <summary>
        /// Handles the Loaded event of the SelectRectangle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SelectRectangle_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
            Loaded -= new RoutedEventHandler(SelectRectangle_Loaded);
        }

        #endregion Implementation
    }
}
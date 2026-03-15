// <copyright file="LinearClipper.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !(Silverlight4 || WINDOWS_PHONE_7)
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE || WINDOWS_PHONE_7
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Syncfusion.WP.Primitives 
#else
#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Syncfusion.Windows.Primitives
#else
#if SILVERLIGHT
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Syncfusion.Tools.Primitives
#else
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Syncfusion.UI.Xaml.Primitives 
#endif
#endif
#endif
{
    /// <summary>
    /// Clips the content of the control in a given direction.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [ClassReference(IsReviewed = false, ShouldInclude = false)]
    public class LinearClipper : Clipper
    {
        #region Dependency Properties
        /// <summary>
        /// Gets or sets the clipped edge.
        /// </summary>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the ExpandDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(
                "ExpandDirection",
                typeof(ExpandDirection),
                typeof(LinearClipper),
                new PropertyMetadata(ExpandDirection.Right, OnExpandDirectionChanged));

       
        #endregion public ExpandDirection ExpandDirection

        #region Override Methods

        /// <summary>
        /// Updates the clip geometry.
        /// </summary>
        protected override void ClipContent()
        {
            if (ExpandDirection == ExpandDirection.Right)
            {
                double width = this.RenderSize.Width * RatioVisible;
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, width, this.RenderSize.Height) };
            }
            else if (ExpandDirection == ExpandDirection.Left)
            {
                double width = this.RenderSize.Width * RatioVisible;
                double rightSide = this.RenderSize.Width - width;
                this.Clip = new RectangleGeometry { Rect = new Rect(rightSide, 0, width, this.RenderSize.Height) };
            }
            else if (ExpandDirection == ExpandDirection.Up)
            {
                double height = this.RenderSize.Height * RatioVisible;
                double bottom = this.RenderSize.Height - height;
                this.Clip = new RectangleGeometry { Rect = new Rect(0, bottom, this.RenderSize.Width, height) };
            }
            else if (ExpandDirection == ExpandDirection.Down)
            {
                double height = this.RenderSize.Height * RatioVisible;
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.RenderSize.Width, height) };
            }
        }

        #endregion

        #region Callback Methods

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">ExpandDirectionView that changed its ExpandDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnExpandDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LinearClipper source = (LinearClipper)d;
            ExpandDirection oldValue = (ExpandDirection)e.OldValue;
            ExpandDirection newValue = (ExpandDirection)e.NewValue;
            source.OnExpandDirectionChanged(oldValue, newValue);
        }

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="oldValue">Old value.</param>
        /// <param name="newValue">New value.</param>        
        protected virtual void OnExpandDirectionChanged(ExpandDirection oldValue, ExpandDirection newValue)
        {
            ClipContent();
        }

        #endregion
    }
    /// <summary>
    /// Represents an enum list for the direction to expand.
    /// </summary>
    public enum ExpandDirection
    {
        /// <summary>
        /// Specifies that the <see cref="T:System.Windows.Controls.Expander" />
        /// control opens in the down direction.
        /// </summary>
        Down = 0,

        /// <summary>
        /// Specifies that the <see cref="T:System.Windows.Controls.Expander" />
        /// control opens in the up direction.
        /// </summary>
        Up = 1,

        /// <summary>
        /// Specifies that the <see cref="T:System.Windows.Controls.Expander" />
        /// control opens in the left direction.
        /// </summary>
        Left = 2,

        /// <summary>
        /// Specifies that the <see cref="T:System.Windows.Controls.Expander" />
        /// control opens in the right direction.
        /// </summary>
        Right = 3,
    }
}

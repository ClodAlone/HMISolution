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
using System.Windows.Input;
#if !WinRT
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Controls.Grid
{

#else
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Syncfusion.WinRT.Controls.Grid
{

#endif
    /// <summary>
    /// Defines the direction of the drag indicator.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public enum ArrowIndicatorDirection
    {
        Up,
        Down
    }

#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class AnimatedGrid : ContentControl
    {
        public AnimatedGrid()
        {
            this.DefaultStyleKey = typeof(AnimatedGrid);
        }

        public static readonly DependencyProperty InnerBrushProperty = DependencyProperty.Register("InnerBrush", typeof(Brush), typeof(AnimatedGrid), 
#if !WinRT
            new PropertyMetadata(Brushes.White));
#else
 new PropertyMetadata(new SolidColorBrush(Colors.White)));
#endif

        public static readonly DependencyProperty OuterBrushProperty = DependencyProperty.Register("OuterBrush", typeof(Brush), typeof(AnimatedGrid), 
#if !WinRT
            new PropertyMetadata(Brushes.Black));
#else
            new PropertyMetadata(Colors.Black));
#endif

        public Brush InnerBrush
        {
            get
            {
                return (Brush)this.GetValue(AnimatedGrid.InnerBrushProperty);
            }

            set
            {
                this.SetValue(AnimatedGrid.InnerBrushProperty, value);
            }
        }

        public Brush OuterBrush
        {
            get
            {
                return (Brush)this.GetValue(AnimatedGrid.OuterBrushProperty);
            }

            set
            {
                this.SetValue(AnimatedGrid.OuterBrushProperty, value);
            }
        }

        private bool isTemplateApplied = false;
        private Border upBorder = null;
        private Border downBorder = null;
#if !WinRT
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            if (this.IndicatorVisibility == Visibility.Visible)
            {
                SetVisualState();
            }

            this.upBorder = this.GetTemplateChild("PART_UpBorder") as Border;
            this.downBorder = this.GetTemplateChild("PART_DownBorder") as Border;
            SetBorderVisibility(this, this.ArrowIndicatorDirection);
        }
#else
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.isTemplateApplied = true;
            if (this.IndicatorVisibility == Visibility.Visible)
            {
                SetVisualState();
            }

            this.upBorder = this.GetTemplateChild("PART_UpBorder") as Border;
            this.downBorder = this.GetTemplateChild("PART_DownBorder") as Border;
            SetBorderVisibility(this, this.ArrowIndicatorDirection);
        }
#endif

        private void SetVisualState()
        {
            if (this.ArrowIndicatorDirection == ArrowIndicatorDirection.Up)
            {
                VisualStateManager.GoToState(this, "UpIndicatorState", false);
            }
            else if (this.ArrowIndicatorDirection == ArrowIndicatorDirection.Down)
            {
                VisualStateManager.GoToState(this, "DownIndicatorState", false);
            }
        }

        public static readonly DependencyProperty IndicatorVisibilityProperty = DependencyProperty.Register(
            "IndicatorVisibility",
            typeof(Visibility),
            typeof(AnimatedGrid),
            new PropertyMetadata(Visibility.Collapsed, OnIndicatorVisibilityChanged));

        private static void OnIndicatorVisibilityChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var grid = dpo as AnimatedGrid;
            if (!grid.isTemplateApplied)
            {
                return;
            }

            var visibility = (Visibility)args.NewValue;
            if (visibility == Visibility.Visible)
            {
                grid.SetVisualState();
            }
            else
            {
                VisualStateManager.GoToState(grid, "Normal", false);
            }
        }

        public Visibility IndicatorVisibility
        {
            get
            {
                return (Visibility)this.GetValue(AnimatedGrid.IndicatorVisibilityProperty);
            }

            set
            {
                this.SetValue(AnimatedGrid.IndicatorVisibilityProperty, value);
            }
        }

        public static readonly DependencyProperty ArrowIndicatorDirectionProperty = DependencyProperty.Register(
            "ArrowIndicatorDirection",
            typeof(ArrowIndicatorDirection),
            typeof(AnimatedGrid),
#if !WinRT
            new PropertyMetadata(OnArrowIndicatorDirectionChanged));
#else
            new PropertyMetadata(ArrowIndicatorDirection.Up, OnArrowIndicatorDirectionChanged));
#endif

        private static void OnArrowIndicatorDirectionChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var animatedGrid = dpo as AnimatedGrid;
            if (animatedGrid.upBorder == null || animatedGrid.downBorder == null || !animatedGrid.isTemplateApplied)
            {
                return;
            }

            var direction = (ArrowIndicatorDirection)args.NewValue;
            SetBorderVisibility(animatedGrid, direction);
        }

        private static void SetBorderVisibility(AnimatedGrid animatedGrid, ArrowIndicatorDirection direction)
        {
            if (direction == ArrowIndicatorDirection.Up)
            {
                animatedGrid.upBorder.Visibility = Visibility.Visible;
                animatedGrid.downBorder.Visibility = Visibility.Collapsed;
            }
            else if (direction == ArrowIndicatorDirection.Down)
            {
                animatedGrid.upBorder.Visibility = Visibility.Collapsed;
                animatedGrid.downBorder.Visibility = Visibility.Visible;
            }
        }

        public ArrowIndicatorDirection ArrowIndicatorDirection
        {
            get
            {
                return (ArrowIndicatorDirection)this.GetValue(AnimatedGrid.ArrowIndicatorDirectionProperty);
            }

            set
            {
                this.SetValue(AnimatedGrid.ArrowIndicatorDirectionProperty, value);
            }
        }
    }
}

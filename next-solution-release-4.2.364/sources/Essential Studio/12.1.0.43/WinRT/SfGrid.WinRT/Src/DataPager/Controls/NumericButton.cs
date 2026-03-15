#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;


#if WinRT
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

#endif

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Syncfusion.UI.Xaml.Controls.DataPager
{
    [TemplatePart(Name = "PART_CurrentPageIndicator", Type = typeof(Rectangle))]
    public sealed class NumericButton : ContentControl
    {
        #region Private Members

        private Rectangle highlighter;
#if !SILVERLIGHT
        private Point pointerPressedPosition;
#endif
        private Brush previousForgroundBrush;

        #endregion

        #region Ctor

        public NumericButton()
        {
            this.DefaultStyleKey = typeof (NumericButton);
        }

        #endregion

        #region Dependency Registration

        public int PageIndex
        {
            get { return (int) GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof (int), typeof (NumericButton), new PropertyMetadata(0));

        public bool IsCurrentPage
        {
            get { return (bool) GetValue(IsCurrentPageProperty); }
            set { SetValue(IsCurrentPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCurrentPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCurrentPageProperty =
            DependencyProperty.Register("IsCurrentPage", typeof (bool), typeof (NumericButton),
                                        new PropertyMetadata(false, OnIsCurrentPageChanged));

        public bool IsElipsisElement
        {
            get { return (bool) GetValue(IsElipsisElementProperty); }
            set { SetValue(IsElipsisElementProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsElipsisElement.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsElipsisElementProperty =
            DependencyProperty.Register("IsElipsisElement", typeof (bool), typeof (NumericButton),
                                        new PropertyMetadata(false));

        public ElipsisPosition ElipsisPosition
        {
            get { return (ElipsisPosition) GetValue(ElipsisPositionProperty); }
            set { SetValue(ElipsisPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ElipsisPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ElipsisPositionProperty =
            DependencyProperty.Register("ElipsisPosition", typeof (ElipsisPosition), typeof (NumericButton),
                                        new PropertyMetadata(ElipsisPosition.None));

        public Brush HighlightingThemeBrush
        {
            get { return (Brush)GetValue(HighlightingThemeBrushProperty); }
            set { SetValue(HighlightingThemeBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightingThemeBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightingThemeBrushProperty =
            DependencyProperty.Register("HighlightingThemeBrush", typeof(Brush), typeof(NumericButton), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        public Brush HighlightForegroundBrush
        {
            get { return (Brush)GetValue(HighlightForegroundBrushProperty); }
            set { SetValue(HighlightForegroundBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightForegroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlightForegroundBrushProperty =
            DependencyProperty.Register("HighlightForegroundBrush", typeof(Brush), typeof(NumericButton), new PropertyMetadata(new SolidColorBrush(Colors.White), OnHighlightForegroundBrushChanged));

        

        #endregion

        #region Dependency Call Back

        private static void OnIsCurrentPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var numericButton = sender as NumericButton;
            if (numericButton.highlighter == null)
                return;

            if (!numericButton.IsElipsisElement)
                numericButton.SetAsCurrentPage((bool) args.NewValue);
        }

        private static void OnHighlightForegroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = d as NumericButton;
            if (button.IsCurrentPage)            
                button.Foreground = button.HighlightForegroundBrush;                                                       
        }

        #endregion

        #region Overrides

#if WinRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            highlighter = this.GetTemplateChild("PART_CurrentPageIndicator") as Rectangle;
            if (highlighter != null)
            {
                SetAsCurrentPage(this.IsCurrentPage);
            }
        }

#if WinRT
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            pointerPressedPosition = e.GetCurrentPoint(null).Position;
        }
#elif WPF
        protected override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);
            pointerPressedPosition = e.TouchDevice.GetTouchPoint(null).Position;
        }
#endif
       
#if WinRT
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            var releasedPosition = e.GetCurrentPoint(null).Position;
            if(Math.Abs(releasedPosition.X-this.pointerPressedPosition.X)>5)
                return;
            this.SetCurrentPage();
            
            base.OnPointerReleased(e);
        }
#elif WPF
        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SetCurrentPage();
            
            base.OnMouseUp(e);
        }

        protected override void OnTouchUp(System.Windows.Input.TouchEventArgs e)
        {
            var releasedPosition = e.TouchDevice.GetTouchPoint(null).Position;
            if (Math.Abs(releasedPosition.X - this.pointerPressedPosition.X) > 5)
                return;
            this.SetCurrentPage();
            base.OnTouchUp(e);
        }

#else
        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            this.SetCurrentPage();
            base.OnMouseLeftButtonUp(e);
        }
#endif

        #endregion

        #region Helper Methods

        private void SetCurrentPage()
        {
            if (this.Parent != null)
            {
                bool elipsisClicked = false;
                var index = this.PageIndex - 1;
                var panel = this.Parent as NumericButtonPanel;
                if (this.IsElipsisElement)
                {
                    if (this.ElipsisPosition == ElipsisPosition.Right)
                    {
                        index = panel.DataPager.PageIndex + panel.DataPager.NumericButtonCount;
                        index = index >= panel.DataPager.PageCount ? panel.DataPager.PageCount - 1 : index;
                    }
                    else
                    {
                        index = panel.DataPager.PageIndex - panel.DataPager.NumericButtonCount;
                        index = index < 0 ? 0 : index;
                    }
                    elipsisClicked = true;
                }
                panel.DataPager.MoveToPage(index, elipsisClicked);
            }
        }

        private void SetAsCurrentPage(bool value)
        {
            if (previousForgroundBrush == null)
            {
                previousForgroundBrush = this.Foreground;
            }
            if (value)
            {                
#if WPF                
                if ((this.Parent as NumericButtonPanel).DataPager.NeedsFocusToCurrentPage)
                    Keyboard.Focus(this);
#endif
                this.highlighter.Visibility = Visibility.Visible;
                var contentWidth = this.GetContentWidth();
#if WinRT
                if (contentWidth > 40)
#else
                if (contentWidth > 35)
#endif
                {
                    this.highlighter.Width = contentWidth;
                }
                else
                {
#if WinRT
                    this.highlighter.Width = 40;
#else
                    this.highlighter.Width = 25;
#endif
                }
                
                if (this.highlighter.DesiredSize.Width == 0)
                {
                    this.highlighter.Measure(new Size(double.MaxValue,double.MaxValue));
                }

                VisualStateManager.GoToState(this, "CurrentPage", true);                        
                this.Foreground = this.HighlightForegroundBrush;
            }
            else
            {
                 this.highlighter.Visibility = Visibility.Collapsed;
                VisualStateManager.GoToState(this, "Normal", false);               
                this.Foreground = previousForgroundBrush;
            }
        }

        private double GetContentWidth()
        {
            var textBlock = new TextBlock
            {
                Text = this.Content.ToString(),
                FontFamily = new FontFamily("Segoe UI"),
#if WinRT
                FontSize = 18,
#else
                FontSize = 14,
#endif
                Padding = new Thickness(5)
            };
           
            var parentBorder = new Border { Child = textBlock ,Height = 50};
            {
#if WinRT
                parentBorder.Padding = new Thickness(5);
#else
                parentBorder.Padding = new Thickness(3);
#endif
                textBlock.MaxHeight = this.highlighter.Height;
                var totalWidth = double.MaxValue;
                parentBorder.MaxWidth = totalWidth;
                parentBorder.Measure(new Size(double.MaxValue, double.MaxValue));
               
            }
            return parentBorder.DesiredSize.Width;
        }

        #endregion
    }
}
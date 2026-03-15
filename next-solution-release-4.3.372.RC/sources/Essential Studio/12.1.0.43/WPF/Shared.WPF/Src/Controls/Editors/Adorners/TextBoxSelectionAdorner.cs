#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace Syncfusion.Windows.Shared
{
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    internal class TextBoxSelectionAdorner : Adorner
    {
        private Thumb selectionThumb1, selectionThumb2;
        private VisualCollection visualCollection;
        private bool isCalledByDragDelta;
        private bool isCalledByMouseUp;
        private double widthChange2;
        private double widthChange1;

        private ThumbDirection thumbDirection = ThumbDirection.None;
        private TextBox iTextBox;

        public TextBoxSelectionAdorner(UIElement adornedElement) :
            base(adornedElement)
        {
            selectionThumb1 = new Thumb();
            selectionThumb2 = new Thumb();
            visualCollection = new VisualCollection(this);
            selectionThumb1.Height = selectionThumb1.Width = selectionThumb2.Height = selectionThumb2.Width = 30;
            selectionThumb1.Margin = selectionThumb2.Margin = new Thickness(-15, 0, 0, 0);
            selectionThumb1.DragDelta += new DragDeltaEventHandler(selectionThumb_DragDelta);
            selectionThumb2.DragDelta += new DragDeltaEventHandler(selectionThumb_DragDelta);
            selectionThumb1.Visibility = System.Windows.Visibility.Collapsed;
            selectionThumb2.Visibility = System.Windows.Visibility.Collapsed;
            visualCollection.Add(selectionThumb1);
            visualCollection.Add(selectionThumb2);
            iTextBox = (adornedElement as TextBox);
            selectionThumb1.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(selectionThumb_PreviewMouseDown);
            selectionThumb2.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(selectionThumb_PreviewMouseDown);
            selectionThumb1.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(selectionThumb_PreviewMouseUp);
            selectionThumb2.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(selectionThumb_PreviewMouseUp);
            if (iTextBox != null)
            {
                iTextBox.PreviewMouseDown += new System.Windows.Input.MouseButtonEventHandler(iTextBox_PreviewMouseDown);
                iTextBox.PreviewMouseUp += new System.Windows.Input.MouseButtonEventHandler(iTextBox_PreviewMouseUp);
                iTextBox.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(iTextBox_PreviewKeyDown);
                iTextBox.SelectionChanged += new RoutedEventHandler(iTextBox_SelectionChanged);
                iTextBox.LostFocus += new RoutedEventHandler(iTextBox_LostFocus);
            }
            ResourceDictionary rd = new ResourceDictionary
                {
                    Source =
                        new Uri("/Syncfusion.Shared.WPF;component/Controls/Editors/Themes/Generic.xaml",
                                UriKind.RelativeOrAbsolute)
                };
            Style thumbStyle = rd["SelectionThumbStyle"] as Style;
            selectionThumb1.Style = thumbStyle;
            selectionThumb2.Style = thumbStyle;
        }

        private void selectionThumb_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.StylusDevice != null)
            {
                HandleThumbVisiblity();
            }
            else
            {
                if (selectionThumb2 != null) 
                    selectionThumb2.Visibility = Visibility.Collapsed;
                if (selectionThumb1 != null) 
                    selectionThumb1.Visibility = Visibility.Collapsed;
            }
        }

        private void iTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (selectionThumb1 != null) 
                selectionThumb1.Visibility = Visibility.Collapsed;
            if (selectionThumb2 != null) 
                selectionThumb2.Visibility = Visibility.Collapsed;
        }

        private void selectionThumb_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                if (selectionThumb1 != null)
                    selectionThumb1.Visibility = Visibility.Collapsed;
                if (selectionThumb2 != null)
                    selectionThumb2.Visibility = Visibility.Collapsed;
            }
        }

        private void iTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var textBox = this.iTextBox;
            if (selectionThumb2 != null && (selectionThumb1 != null && (textBox.Text != null && (textBox != null && (textBox.Text.Length > 0 && iTextBox.SelectionLength == 0 && !isCalledByDragDelta && !isCalledByMouseUp && selectionThumb1.Visibility == Visibility.Visible && selectionThumb2.Visibility == Visibility.Visible)))))
            {
                FormattedText formattedText = new FormattedText(this.iTextBox.Text.Substring(0, this.iTextBox.SelectionStart),
                                                                Thread.CurrentThread.CurrentUICulture, this.iTextBox.FlowDirection,
                                                                new Typeface(iTextBox.FontFamily, iTextBox.FontStyle, iTextBox.FontWeight, iTextBox.FontStretch),
                                                                iTextBox.FontSize,
                                                                iTextBox.Foreground
                                                                );
                double selectionStartWidth = formattedText.Width;
                int selLength;
                if (iTextBox.SelectionLength == 0)
                    selLength = iTextBox.SelectionStart;
                else
                    selLength = iTextBox.SelectionLength + iTextBox.SelectionStart;
                formattedText = new FormattedText(this.iTextBox.Text.Substring(0, selLength),
                                                                Thread.CurrentThread.CurrentUICulture, this.iTextBox.FlowDirection,
                                                                new Typeface(iTextBox.FontFamily, iTextBox.FontStyle, iTextBox.FontWeight, iTextBox.FontStretch),
                                                                iTextBox.FontSize,
                                                                iTextBox.Foreground
                                                                );
                double selectionEndWidth = formattedText.Width;
                selectionThumb1.Arrange(new Rect(selectionStartWidth, iTextBox.RenderSize.Height - 15, 30, 30));

                selectionThumb1.Tag = new ThumbPosition { Position = iTextBox.SelectionStart, StartingPoint = selectionStartWidth };
                selectionThumb2.Arrange(new Rect(selectionEndWidth, iTextBox.RenderSize.Height - 15, 30, 30));
                selectionThumb2.Tag = new ThumbPosition { Position = selLength, StartingPoint = selectionEndWidth };
            }
        }

        public void iTextBox_PreviewMouseUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            isCalledByMouseUp = true;
            if (e.StylusDevice != null)
            {
                HandleThumbVisiblity();
            }
            else
            {
                if (selectionThumb2 != null)
                    selectionThumb2.Visibility = Visibility.Collapsed;
                if (selectionThumb1 != null)
                    selectionThumb1.Visibility = Visibility.Collapsed;
            }
            isCalledByMouseUp = false;
        }

        private void HandleThumbVisiblity()
        {
            if (selectionThumb1 != null && (selectionThumb2 != null && (iTextBox != null && (iTextBox.SelectionLength == 0 && (selectionThumb2.Visibility == Visibility.Visible || selectionThumb1.Visibility == Visibility.Visible)))))
            {
                selectionThumb2.Visibility = Visibility.Collapsed;
                selectionThumb1.Visibility = Visibility.Collapsed;
            }
            else
            {
                var textBox = this.iTextBox;
                if (textBox != null && (iTextBox != null && (!string.IsNullOrEmpty(textBox.Text) && iTextBox.SelectionStart <= iTextBox.Text.Length)))
                {
                    if (selectionThumb1 != null) 
                        selectionThumb1.Visibility = Visibility.Hidden;
                    if (selectionThumb2 != null) 
                        selectionThumb2.Visibility = Visibility.Hidden;
                    FormattedText formattedText = new FormattedText(this.iTextBox.Text.Substring(0, this.iTextBox.SelectionStart),
                                                                    Thread.CurrentThread.CurrentUICulture, this.iTextBox.FlowDirection,
                                                                    new Typeface(iTextBox.FontFamily, iTextBox.FontStyle, iTextBox.FontWeight, iTextBox.FontStretch),
                                                                    iTextBox.FontSize,
                                                                    iTextBox.Foreground
                        );
                    double selectionStartWidth = formattedText.Width;
                    int selLength;
                    if (iTextBox.SelectionLength == 0)
                        selLength = iTextBox.SelectionStart;
                    else
                        selLength = iTextBox.SelectionLength + iTextBox.SelectionStart;
                    formattedText = new FormattedText(this.iTextBox.Text.Substring(0, selLength),
                                                      Thread.CurrentThread.CurrentUICulture, this.iTextBox.FlowDirection,
                                                      new Typeface(iTextBox.FontFamily, iTextBox.FontStyle, iTextBox.FontWeight, iTextBox.FontStretch),
                                                      iTextBox.FontSize,
                                                      iTextBox.Foreground
                        );
                    double selectionEndWidth = formattedText.Width;

                    if (selectionThumb1 != null)
                    {
                        selectionThumb1.Tag = new ThumbPosition { Position = iTextBox.SelectionStart, StartingPoint = selectionStartWidth };
                        selectionThumb1.Arrange(new Rect(selectionStartWidth, iTextBox.RenderSize.Height - 15, 30, 30));
                    }

                    if (selectionThumb2 != null)
                    {
                        selectionThumb2.Arrange(new Rect(selectionEndWidth, iTextBox.RenderSize.Height - 15, 30, 30));
                        selectionThumb2.Tag = new ThumbPosition { Position = selLength, StartingPoint = selectionEndWidth };
                    }

                    if (selectionThumb1 != null) 
                        selectionThumb1.Visibility = Visibility.Visible;
                    if (selectionThumb2 != null) 
                        selectionThumb2.Visibility = Visibility.Visible;
                }
                else
                {
                    if (selectionThumb2 != null) 
                        selectionThumb2.Visibility = Visibility.Collapsed;
                    if (selectionThumb1 != null) 
                        selectionThumb1.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void iTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (selectionThumb2 != null) 
                selectionThumb2.Visibility = Visibility.Collapsed;
            if (selectionThumb1 != null) 
                selectionThumb1.Visibility = Visibility.Collapsed;
        }

        private void iTextBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.StylusDevice == null)
            {
                if (selectionThumb2 != null) 
                    selectionThumb2.Visibility = Visibility.Collapsed;
                if (selectionThumb1 != null) 
                    selectionThumb1.Visibility = Visibility.Collapsed;
            }
        }

        private void selectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            isCalledByDragDelta = true;
            var textBox = this.iTextBox;
            if (textBox != null)
            {
                if (textBox.Text != null)
                {
                    FormattedText formattedText = new FormattedText(textBox.Text.Substring(0, iTextBox.Text.Length),
                                                                    Thread.CurrentThread.CurrentUICulture, textBox.FlowDirection,
                                                                    new Typeface(iTextBox.FontFamily, iTextBox.FontStyle, iTextBox.FontWeight, iTextBox.FontStretch),
                                                                    iTextBox.FontSize,
                                                                    iTextBox.Foreground
                        );
                    if (e.HorizontalChange > 0d)
                    {
                        if (thumbDirection == ThumbDirection.Backward)
                        {
                            if (sender == selectionThumb2)
                                widthChange2 = 0d;
                            else
                                widthChange1 = 0d;
                        }
                        thumbDirection = ThumbDirection.Forward;
                        if (sender == selectionThumb2)
                            widthChange2 += e.HorizontalChange;
                        else
                            widthChange1 += e.HorizontalChange;

                        if (sender != null)
                        {
                            var pos2 = ((Thumb) sender).Tag;
                            if (pos2 != null)
                            {
                                ThumbPosition tPos2 = (ThumbPosition)pos2;
                                if (tPos2.Position < iTextBox.Text.Length)
                                {
                                    FormattedText formattedTextInner = new FormattedText(textBox.Text.Substring(tPos2.Position, 1),
                                                                                         Thread.CurrentThread.CurrentUICulture, textBox.FlowDirection,
                                                                                         new Typeface(iTextBox.FontFamily, iTextBox.FontStyle, iTextBox.FontWeight, iTextBox.FontStretch),
                                                                                         iTextBox.FontSize,
                                                                                         iTextBox.Foreground
                                        );

                                    double horizontalChange;
                                    if (sender == selectionThumb2)
                                        horizontalChange = widthChange2;
                                    else
                                        horizontalChange = widthChange1;
                                    if (horizontalChange >= formattedTextInner.Width)
                                    {
                                        tPos2.Position = tPos2.Position + 1;
                                        ((Thumb)sender).Tag = tPos2;
                                        ThumbPosition tPos1 = new ThumbPosition();
                                        if (sender == selectionThumb2)
                                        {
                                            if (selectionThumb1 != null) tPos1 = (ThumbPosition)selectionThumb1.Tag;
                                        }
                                        else
                                            tPos1 = (ThumbPosition)selectionThumb2.Tag;
                                        if (tPos2.Position > tPos1.Position)
                                        {
                                            if (iTextBox.SelectionLength + 1 <= iTextBox.Text.Length)
                                                iTextBox.SelectionLength = iTextBox.SelectionLength + 1;
                                        }
                                        else if (tPos2.Position < tPos1.Position)
                                        {
                                            if (iTextBox.SelectionStart + 1 <= iTextBox.Text.Length)
                                            {
                                                iTextBox.SelectionStart = iTextBox.SelectionStart + 1;
                                            }
                                            if (iTextBox.SelectionLength > 0 && tPos1.Position != iTextBox.Text.Length)
                                                iTextBox.SelectionLength = iTextBox.SelectionLength - 1;
                                        }
                                        else
                                        {
                                            iTextBox.SelectionLength = 0;
                                            if (iTextBox.SelectionStart + 1 <= iTextBox.Text.Length)
                                            {
                                                iTextBox.SelectionStart = iTextBox.SelectionStart + 1;
                                            }
                                        }
                                        if (sender == selectionThumb2)
                                            widthChange2 = 0d;
                                        else
                                            widthChange1 = 0d;
                                    }
                                }
                            }
                        }
                    }
                    else if (e.HorizontalChange < 0d)
                    {
                        if (thumbDirection == ThumbDirection.Forward)
                        {
                            if (sender == selectionThumb2)
                                widthChange2 = 0d;
                            else
                                widthChange1 = 0d;
                        }
                        thumbDirection = ThumbDirection.Backward;
                        if (sender == selectionThumb2)
                            widthChange2 += Math.Abs(e.HorizontalChange);
                        else
                            widthChange1 += Math.Abs(e.HorizontalChange);

                        if (sender != null)
                        {
                            var pos2 = ((Thumb) sender).Tag;
                            if (pos2 != null)
                            {
                                ThumbPosition tPos2 = (ThumbPosition)pos2;
                                if (tPos2.Position > 0)
                                {
                                    if (textBox.Text.Length > tPos2.Position - 1)
                                    {
                                        FormattedText formattedTextInner = new FormattedText(textBox.Text.Substring(tPos2.Position - 1, 1),
                                                                                             Thread.CurrentThread.CurrentUICulture, textBox.FlowDirection,
                                                                                             new Typeface(iTextBox.FontFamily, iTextBox.FontStyle, iTextBox.FontWeight, iTextBox.FontStretch),
                                                                                             iTextBox.FontSize,
                                                                                             iTextBox.Foreground
                                            );
                                        double horizontalChange;
                                        if (selectionThumb2 != null && sender == selectionThumb2)
                                            horizontalChange = widthChange2;
                                        else
                                            horizontalChange = widthChange1;
                                        if (horizontalChange >= formattedTextInner.Width)
                                        {
                                            tPos2.Position = tPos2.Position - 1;
                                            ((Thumb)sender).Tag = tPos2;
                                            ThumbPosition tPos1 = new ThumbPosition();
                                            if (selectionThumb2 != null && sender == selectionThumb2)
                                            {
                                                if (selectionThumb1.Tag != null)
                                                    tPos1 = (ThumbPosition)selectionThumb1.Tag;
                                            }
                                            else if (selectionThumb2 != null)
                                                if (selectionThumb2.Tag != null)
                                                    tPos1 = (ThumbPosition)selectionThumb2.Tag;
                                            if (tPos2.Position > tPos1.Position)
                                            {
                                                if (iTextBox.SelectionLength > 0)
                                                    iTextBox.SelectionLength = iTextBox.SelectionLength - 1;
                                            }
                                            else if (tPos2.Position < tPos1.Position)
                                            {
                                                if (iTextBox.SelectionStart > 0)
                                                {
                                                    iTextBox.SelectionStart = iTextBox.SelectionStart - 1;
                                                    iTextBox.SelectionLength = iTextBox.SelectionLength + 1;
                                                }
                                            }
                                            else
                                            {
                                                iTextBox.SelectionLength = 0;
                                            }

                                            if (selectionThumb2 != null && sender == selectionThumb2)
                                                widthChange2 = 0d;
                                            else
                                                widthChange1 = 0d;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ThumbPosition tPostion = new ThumbPosition();
                    if (selectionThumb2 != null && sender == selectionThumb2)
                    {
                        if (selectionThumb2.Tag != null) 
                            tPostion = (ThumbPosition)selectionThumb2.Tag;
                    }
                    else if (selectionThumb1 != null && selectionThumb1.Tag != null) 
                        tPostion = (ThumbPosition)selectionThumb1.Tag;
                    formattedText = new FormattedText(textBox.Text.Substring(0, tPostion.Position),
                                                      Thread.CurrentThread.CurrentUICulture, textBox.FlowDirection,
                                                      new Typeface(iTextBox.FontFamily, iTextBox.FontStyle, iTextBox.FontWeight, iTextBox.FontStretch),
                                                      iTextBox.FontSize,
                                                      iTextBox.Foreground
                        );
                    if (selectionThumb2 != null && sender == selectionThumb2)
                        selectionThumb2.Arrange(new Rect(formattedText.Width, iTextBox.RenderSize.Height - 15, 30, 30));
                    else if (selectionThumb1 != null)
                        selectionThumb1.Arrange(new Rect(formattedText.Width, iTextBox.RenderSize.Height - 15, 30, 30));
                }
            }

            isCalledByDragDelta = false;
        }

        protected override int VisualChildrenCount { get { return visualCollection.Count; } }

        protected override Visual GetVisualChild(int index)
        {
            if (visualCollection.Count > index) 
                return visualCollection[index];

            return null;
        }
    }

    internal struct ThumbPosition
    {
        public int Position { get; set; }

        public double StartingPoint { get; set; }
    }

    internal enum ThumbDirection
    {
        None, Forward, Backward
    }
}
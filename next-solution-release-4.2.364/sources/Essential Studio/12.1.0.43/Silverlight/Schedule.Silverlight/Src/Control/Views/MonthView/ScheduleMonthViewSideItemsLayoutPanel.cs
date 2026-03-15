#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace Syncfusion.Windows.Controls.Schedule
{
    /// <summary>
    ///  class that holds sideView items panel for schedule month view
    /// </summary>
    public class ScheduleMonthViewSideItemsLayoutPanel : Panel
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleMonthViewSideItemsLayoutPanel"/>
        /// class.
        /// </summary>
        public ScheduleMonthViewSideItemsLayoutPanel()
        { 
            
        }


        /// <summary>
        /// Provides the behavior for the "measure" pass of Silverlight layout. Classes can override this method to define their own measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of child object allotted sizes.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Children.Count <= 0)
            {
                return new Size();
            }

#if SILVERLIGHT
            int visibleItemsCount = this.Children.Where(el => el.Visibility != Visibility.Collapsed).Count();
#else
            int visibleItemsCount = this.Children.ToTypedList<UIElement>().Where(el => el.Visibility != Visibility.Collapsed).Count();
#endif         
            Size itemSize = availableSize;
            itemSize.Width = 20d;

            if (!double.IsInfinity(itemSize.Height))
            {
                itemSize.Height /= visibleItemsCount;
            }

            double maxWidth = 0.0;
            double height = 0.0;

            foreach (UIElement item in this.Children)
            {
                if (item.Visibility != Visibility.Collapsed)
                {
                    item.Measure(itemSize);
                    height += double.IsInfinity(itemSize.Height) ? item.DesiredSize.Height : itemSize.Height;
                    maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
                }
            }

            if (double.IsInfinity(itemSize.Height))
            {
                itemSize.Width = maxWidth;
                itemSize.Height = height / visibleItemsCount;

                foreach (UIElement item in this.Children)
                {
                    if (item.Visibility != Visibility.Collapsed)
                    {
                        item.Measure(itemSize);
                    }
                }
            }

            if (double.IsInfinity(availableSize.Height))
            {
                availableSize.Height = height;
            }

            if (double.IsInfinity(availableSize.Width))
            {
                availableSize.Width = maxWidth;
            }
            return availableSize;
        }

        /// <summary>
        /// Provides the behavior for the "arrange" pass of Silverlight layout. Classes can override this method to define their own arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children.Count <= 0)
            {
                return finalSize;
            }

            int visibleItemsCount = 0;

            foreach (UIElement item in this.Children)
            {
                if (item.Visibility != Visibility.Collapsed)
                {
                    ++visibleItemsCount;
                }
            }
            double itemHeight = finalSize.Height / visibleItemsCount;
            Rect itemRect = new Rect(0, 0, finalSize.Width, itemHeight);
            foreach (UIElement item in this.Children)
            {
                if (item.Visibility != Visibility.Collapsed)
                {
                    item.Arrange(itemRect);
                    itemRect.Y += itemHeight;
                }
            }
           this.SetUpDates();
            return finalSize;
        }


        private void SetUpDates()
        {
            var ItemsControl = this.Children.OfType<ScheduleMonthViewSideContentControl>();
            if (ItemsControl == null) return;
            bool needChange = false;
            foreach (ScheduleMonthViewSideContentControl ctrl in ItemsControl)
            {
                string str = this.GetLongForm(ctrl.Dates);
#if SILVERLIGHT
               
                if ((ctrl.ActualHeight - (37)) < this.MeasureText(ctrl, str).Width)
                    needChange = true;
#else
                FormattedText textmeasured = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(ctrl.FontFamily, ctrl.FontStyle, ctrl.FontWeight, ctrl.FontStretch), ctrl.FontSize, new SolidColorBrush(Colors.Black));
                if ((ctrl.ActualHeight - (37) < textmeasured.WidthIncludingTrailingWhitespace))
                     needChange = true;
#endif
            }
            if (needChange)
                foreach (ScheduleMonthViewSideContentControl ctrl in ItemsControl)
                    ctrl.SideText = this.GetShortForm(ctrl.Dates);           
            else
                foreach (ScheduleMonthViewSideContentControl ctrl in ItemsControl)
                    ctrl.SideText = this.GetLongForm(ctrl.Dates);
        }

        private string GetLongForm(ObservableCollection<DateTime> dates)
        {
            string SideText;
            DateTime date = dates.First();
            DateTime dateLast = dates.LastOrDefault();
            if (date.Month == dateLast.Month)
                {
                    SideText = String.Format("{0}-{1}/{2}", date.Day.ToString(), dateLast.Day.ToString(),date.ToString("MMM"));
                }
                else
                {
                    SideText = String.Format("{0}/{1}-{2}/{3}", date.Day.ToString(), date.ToString("MMM"), dateLast.Day.ToString(), dateLast.ToString("MMM"));
                }
            return SideText;
        }

        private string GetShortForm(ObservableCollection<DateTime> dates)
        {
            string SideText;
            DateTime date = dates.First();
            DateTime dateLast = dates.LastOrDefault();
            if (date.Month == dateLast.Month)
            {
                SideText = String.Format("{0}-{1}/{2}", date.Day.ToString(), dateLast.Day.ToString(), date.Month);
            }
            else
            {
                SideText = String.Format("{0}/{1}-{2}/{3}", date.Day.ToString(), date.Month, dateLast.Day.ToString(), dateLast.Month);
            }
            return SideText;
        }

        private Size MeasureText(ScheduleMonthViewSideContentControl control, string text)
        {
            var textBlock = new System.Windows.Controls.TextBlock()
            {
                FontSize = control.FontSize,
                FontFamily = control.FontFamily,
                Text = text,
                TextWrapping = TextWrapping.NoWrap,
                FontStretch = control.FontStretch,
                FontWeight = control.FontWeight,
                FontStyle = control.FontStyle,
                HorizontalAlignment = control.HorizontalAlignment,
                VerticalAlignment = control.VerticalAlignment
            };

            var parentBorder = new System.Windows.Controls.Border() { Child = textBlock };
            textBlock.MaxHeight = 20d;
            var totalWidth = Double.Epsilon;
            parentBorder.MaxWidth = totalWidth;
            var reservedTextSize = new Size(textBlock.ActualWidth, textBlock.ActualHeight);
            textBlock.Measure(new Size(double.MaxValue, double.MaxValue));
            parentBorder.Measure(new Size(double.MaxValue, double.MaxValue));
            parentBorder.Arrange(new Rect(0, 0, textBlock.ActualWidth, textBlock.ActualHeight));
            if (parentBorder.ActualWidth > 0 && parentBorder.ActualHeight > 0)
            {
                // very odd, when query is for column widths, return the parent border's actual width and height
                var finalSize = new Size(parentBorder.ActualWidth, parentBorder.ActualHeight);
                return finalSize;
            }
            return reservedTextSize;
        }
    
    }

    /// <summary>
    ///  Class that holds side text block in Schedule
    /// </summary>
    public class ScheduleSideTextBlock : ItemsControl
    {
        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleSideTextBlock"/> class.
        /// </summary>
        public ScheduleSideTextBlock()
        {
            this.DefaultStyleKey = typeof(ScheduleSideTextBlock);
           
        }

        /// <summary>
        /// Gets or sets string value
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for Text.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ScheduleSideTextBlock), new PropertyMetadata(null, OnTextChanged));

        private static void OnTextChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            ScheduleSideTextBlock control = dpo as ScheduleSideTextBlock;
            control.GenerateSideValues();
        }
        private void GenerateSideValues()
        {
            char[] tex = this.Text.ToCharArray().Reverse<char>().ToArray();
            this.Items.Clear();
            for (int i = 0; i < tex.Length; i++)
            {            
                TextBlock t = new TextBlock();
                t.FontSize = this.FontSize;
                t.Margin = new Thickness(0, 0, 0, -(this.FontSize - 3));             
                t.Padding = new Thickness(0);
                t.HorizontalAlignment = HorizontalAlignment.Center;
                t.VerticalAlignment = VerticalAlignment.Center;
                t.Text = tex[i].ToString();
                t.RenderTransform = new RotateTransform() { Angle = -90 };
                t.RenderTransformOrigin = new Point(0.5, 0.5);
                this.Items.Add(t);
            }             
        }
    }
}

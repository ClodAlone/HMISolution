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
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using System.Globalization;

namespace Syncfusion.Windows.Controls.Schedule
{
    /// <summary>
    ///  Class that holds headeredLayout panel for monthView of the Schedule
    /// </summary>
    public class ScheduleMonthViewHeaderItemsLayoutPanel: Panel
    {
        #region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="UniformStackPanel"/> class.
		/// </summary>
        public ScheduleMonthViewHeaderItemsLayoutPanel()
		{
           
		}          	

		#endregion				

		#region Overrides

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

			
				if (!double.IsInfinity(itemSize.Width))
				{
					itemSize.Width /= visibleItemsCount;
				}

				double maxHeight = 0.0;
				double width = 0.0;
               
				foreach (UIElement item in this.Children)
				{
					if (item.Visibility != Visibility.Collapsed)
					{
						item.Measure(itemSize);
						width += double.IsInfinity(itemSize.Width) ? item.DesiredSize.Width : itemSize.Width;
                        
						maxHeight = Math.Max(maxHeight, item.DesiredSize.Height);
					}
				}


                if (double.IsInfinity(itemSize.Width))
                {
                    itemSize.Width = width / visibleItemsCount;
                    itemSize.Height = maxHeight;

                    foreach (UIElement item in this.Children)
                    {
                        if (item.Visibility != Visibility.Collapsed)
                        {
                            item.Measure(itemSize);
                        }
                    }
                }

				if (double.IsInfinity(availableSize.Width))
				{
					availableSize.Width = width;
				}

				if (double.IsInfinity(availableSize.Height))
				{
					availableSize.Height = maxHeight;
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
            double itemWidth = finalSize.Width / visibleItemsCount;
            Rect itemRect = new Rect(0, 0, itemWidth, finalSize.Height);
            foreach (UIElement item in this.Children)
            {
                if (item.Visibility != Visibility.Collapsed)
                {
                    item.Arrange(itemRect);
                    Size size = item.DesiredSize;
                    itemRect.X += itemWidth;
                }
            }

            this.SetUpDates();
            return finalSize;
        }

        
        private void SetUpDates()
        {
            var ItemsControl = this.Children.OfType<ScheduleMonthViewHeaderControl>();
            if (ItemsControl == null) return;
            bool needChange = false;         
            foreach (ScheduleMonthViewHeaderControl ctrl in ItemsControl)
            {
                string str = ctrl.DayOfWeek;
#if SILVERLIGHT
                if (ctrl.ActualWidth < this.MeasureText(ctrl,str).Width)
#else
                FormattedText textmeasured = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(ctrl.FontFamily, ctrl.FontStyle, ctrl.FontWeight, ctrl.FontStretch), ctrl.FontSize, new SolidColorBrush(Colors.Black));
                if (ctrl.ActualWidth < textmeasured.WidthIncludingTrailingWhitespace)
#endif             
                    needChange = true;                
            }
            if (needChange)
            {
                foreach (ScheduleMonthViewHeaderControl ctrl in ItemsControl)
                    ctrl.DayText = ctrl.DayOfWeek.Substring(0, 3);         
               
            }
            else
            {
                foreach (ScheduleMonthViewHeaderControl ctrl in ItemsControl)
                    ctrl.DayText = ctrl.DayOfWeek.ToString();
            }
            
        }

        private Size MeasureText(ScheduleMonthViewHeaderControl control, string text)
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
        #endregion
    }
}

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
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Syncfusion.UI.Xaml.Schedule
{
    public class MaxHeightPanel : Panel
    {

        #region Overrides

        #region MeasureOverride

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Children.Count == 0)
            {
                return Size.Empty;
            }

            var totalHeight = 0d;            
            foreach (UIElement child in this.Children)
            {
                var appControl = child as ScheduleAllDaysAppointmentItemsControl;
                var appControlItemsCount = 0;
                if (appControl != null )
                {
                   
                    appControlItemsCount = appControl.Items.Count;
                    child.Measure(availableSize);
                    totalHeight += child.DesiredSize.Height + (appControlItemsCount *3);
                    continue;
                }
                child.Measure(availableSize);
                totalHeight += child.DesiredSize.Height;
            }          
            availableSize.Height = totalHeight > 0 ? totalHeight + 20 : 0;

            return availableSize;
        }

        #endregion

        #region ArrangeOverride

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children.Count == 0)
            {
                return Size.Empty;
            }

            foreach (UIElement child in this.Children)
            {
                if (child.DesiredSize.Width <= 0 || child.DesiredSize.Height <= 0) continue;
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
            return finalSize;
        }

        #endregion

        #endregion
    }
}
